using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialRunner : MonoBehaviour
{
    public Hand left, right;
    public BeatManager beats;
    public TMP_Text instructions;
    public SequenceLibrary tutorialSequences;

    [Range(0f, 10f)]
    public float textDelay = 2f;
    public float timeoutWindowDuration = 5;

    TutorialState _tutorialState;
    float _timeout = 0f;
    int _beatIx = 0;

    void Awake()
    {
        GameState state = StaticData.currentState;
        if (state != GameState.Tutorial)
            gameObject.SetActive(false);
        else
            instructions.gameObject.SetActive(true);    
    }

    void Start()
    {
        StartCoroutine(RunTutorial());
    }

    void Update()
    {
        if (_tutorialState == TutorialState.Pending || _tutorialState == TutorialState.CheckRest)
        {
            _timeout = 0f;
            return;
        }

        if (_timeout > timeoutWindowDuration)
        {
            SceneNav.GoToTitle();
            return;
        }

        if (left.IsResting && right.IsResting)
            _timeout += Time.deltaTime;
        else
            _timeout = 0f;

    }

    IEnumerator RunTutorial()
    {
        instructions.text = "";
        _tutorialState = TutorialState.Pending;

        yield return new WaitForSeconds(1f);

        instructions.text = "FRIENDS IN KNEAD is a game about KNEADING to the BEAT :)";

        yield return new WaitForSeconds(textDelay);

        instructions.text = "For this tutorial rhythm is turned off...";

        yield return new WaitForSeconds(textDelay);

        instructions.text = "...so that you can learn PROPER TECHNIQUE";

        yield return new WaitForSeconds(textDelay);

        instructions.text = "Do not slap or poke the dough. MASSAGE it with PASSION";

        yield return new WaitForSeconds(textDelay);

        instructions.text = "The little gloves request that you knead GENTLY";

        yield return new WaitForSeconds(textDelay);

        _tutorialState = TutorialState.CheckSoft;
        yield return SetupSequence(0);

        yield return DoSequence();

        _tutorialState = TutorialState.Pending;
        beats.beats.ForEach(beat => Destroy(beat.gameObject));
        beats.beats.Clear();

        instructions.text = "The big gloves want you to put a little more OOMPH in it";

        yield return new WaitForSeconds(textDelay);

        _tutorialState = TutorialState.CheckHard;
        yield return SetupSequence(1);

        yield return DoSequence();

        _tutorialState = TutorialState.Pending;
        beats.beats.ForEach(beat => Destroy(beat.gameObject));
        beats.beats.Clear();

        instructions.text = "Sometimes you just need to REST a while!";

        yield return new WaitForSeconds(textDelay);

        _tutorialState = TutorialState.CheckRest;
        yield return SetupSequence(2);

        yield return DoRestSequence();

        _tutorialState = TutorialState.Pending;
        beats.beats.ForEach(beat => Destroy(beat.gameObject));
        beats.beats.Clear();

        instructions.text = "Let's get started - DOUGH YOU HAVE WHAT IT TAKES??";

        yield return new WaitForSeconds(textDelay);

        StaticData.difficulty = Difficulty.Easy;
        SceneNav.GoToGame();
    }

    IEnumerator SetupSequence(int ix)
    {
        instructions.text = "";
        beats.currentSequence = tutorialSequences.sequences[ix];
        beats.PositionBeats(recreate: true);
        _beatIx = 0;

        yield return new WaitForSeconds(1f);

        beats.OpenBeatWindow(0, doRotate: false);
    }

    IEnumerator DoSequence()
    {
        while (_beatIx < 8)
        {
            if (left.needsToReset || right.needsToReset)
            {
                yield return null;
                continue;
            }

            switch (beats.Poll())
            {
                case Result.Success:
                    float elapsed = 0f;
                    bool noOvershoot = true;
                    while (elapsed < 0.5f)
                    {
                        yield return null;
                        if (beats.Poll() == Result.Miss)
                        {
                            Debug.Log("overshot a soft press");
                            noOvershoot = false;
                            break;
                        }
                        elapsed += Time.deltaTime;
                    }
                    if (noOvershoot)
                    {
                        beats.CloseBeatWindow(_beatIx);
                        _beatIx++;
                        if (_beatIx < 8)
                            beats.OpenBeatWindow(_beatIx, doRotate: false);
                    }
                    else
                    {
                        yield return new WaitForSeconds(1.0f);
                        beats.beats[_beatIx].SetResult(Result.Ready);
                    }
                    break;
                case Result.Miss:
                    Debug.Log("miss");
                    yield return new WaitForSeconds(1.0f);
                    beats.beats[_beatIx].SetResult(Result.Ready);
                    break;
                default:
                    yield return null;
                    break;
            }
        }
    }

    IEnumerator DoRestSequence()
    {
        while (_beatIx < 8)
        {

            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                beats.Poll();
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (beats.CloseBeatWindow(_beatIx) == Result.Success)
            {
                _beatIx++;
                if (_beatIx < 8)
                {
                    yield return new WaitForSeconds(0.25f);
                    beats.OpenBeatWindow(_beatIx, doRotate: false);
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
                beats.beats[_beatIx].SetResult(Result.Ready);
                yield return new WaitForSeconds(0.5f);
                beats.OpenBeatWindow(_beatIx, doRotate: false);
            }
        }
    }

    enum TutorialState { Pending, CheckSoft, CheckHard, CheckRest }
}
