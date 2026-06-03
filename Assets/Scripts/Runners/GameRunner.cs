using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameRunner : MonoBehaviour
{
    public BeatManager beats;
    public LivesManager lives;
    public InputManager input;
    public AudioManager gameAudio;
    public SequenceLibrary sequenceLibrary;
    public TMP_Text roundText;
    public TMP_Text bPMText;
    public TMP_Text scoreText;
    public Dough dough;
    public GameObject pastrySilhouette;
    [Min(0)]
    public int roundsToNextLevel = 1;
    [Range(0.01f, 0.99f)]
    public double windowDurationPerBeat = 0.9;
    public bool running;


    double _nextEventDspTime;
    int _roundNumber;
    bool _gameOver;
    int _missTolerance;
    const int BEATS_PER_BAR = 4;

    void Awake()
    {
        GameState state = StaticData.currentState;
        if (state == GameState.Tutorial)
            gameObject.SetActive(false);
        else
            lives.gameObject.SetActive(true);

        if (StaticData.difficulty == Difficulty.Easy)
            SetupEasy();
        else
            SetupHard();
    }

    void Start()
    {
        _nextEventDspTime = AudioSettings.dspTime + 2;
        _roundNumber = 0;
        _gameOver = false;

        StaticData.FriendsMade = 0;
        StaticData.Score = 0;
        StaticData.NewHighScoreOrdinal = 0;

        roundText.text = "0";
        bPMText.text = "0";
        scoreText.text = "0";
    }

    void Update()
    {
        if (!running)
        {
            _nextEventDspTime = System.Math.Max(_nextEventDspTime, AudioSettings.dspTime + 0.5);
            return;
        }

        if (beats.Poll() == Result.Success)
            dough.LoopDoughSprites();

        if (running && _nextEventDspTime - AudioSettings.dspTime < 0.5f)
            ScheduleNextGameLoop();
    }

    void OnDisable()
    {
        roundText.text = "";
        bPMText.text = "";
        scoreText.text = "";
    }

    void SetupEasy()
    {
        lives.livesCount = 5;
        _missTolerance = 3;
    }

    void SetupHard()
    {
        lives.livesCount = 3;
        _missTolerance = 2;
    }

    void ScheduleNextGameLoop()
    {
        if (_gameOver)
        {
            // go to game over screen
            StartCoroutine(ScheduleGoToEnd(_nextEventDspTime));
            running = false;
            return;
        }

        // BAR 1 ACTIONS (count-in section)
        roundText.text = (_roundNumber + 1).ToString();
        bPMText.text = gameAudio.currentLevel.bpm.ToString();
        double secondsPerBeat = gameAudio.SecondsPerBeat();

        gameAudio.LoadIntro(_nextEventDspTime);      // load the count-in music

        StartCoroutine(SetupNextSequence(_nextEventDspTime));
        StartCoroutine(DoCountdown(_nextEventDspTime, secondsPerBeat));
        StartCoroutine(ToggleHeartbeats(_nextEventDspTime, secondsPerBeat));

        // BAR 2-3 ACTIONS (player input section)
        _nextEventDspTime += secondsPerBeat * BEATS_PER_BAR;
        gameAudio.LoadBeat(_nextEventDspTime);       // load the first bar of gameplay music

        for (int i = 0; i < BEATS_PER_BAR * 2; i++)  // schedule each beat's input window
        {
            double beatDspTime = _nextEventDspTime + secondsPerBeat * i;
            StartCoroutine(ScheduleInputWindow(beatDspTime, secondsPerBeat * windowDurationPerBeat, i));
        }

        _nextEventDspTime += secondsPerBeat * BEATS_PER_BAR;
        gameAudio.LoadBeat(_nextEventDspTime);       // load the second bar of gameplay music

        // BAR 4 ACTIONS (results section)
        _nextEventDspTime += secondsPerBeat * BEATS_PER_BAR;
        gameAudio.LoadIntro(_nextEventDspTime);      // load the results screen music
        StartCoroutine(EvaluateScore(_nextEventDspTime, secondsPerBeat));

        _nextEventDspTime += secondsPerBeat * BEATS_PER_BAR;
    }

    IEnumerator DoCountdown(double eventDspTime, double secondsPerBeat)
    {
        const int BEATS_PER_BAR = 4;

        for (var i = 0; i < BEATS_PER_BAR; i++)
        {
            yield return WaitUntilDspTime(eventDspTime + secondsPerBeat * i);
        }
    }

    IEnumerator SetupNextSequence(double eventDspTime)
    {
        yield return WaitUntilDspTime(eventDspTime);

        beats.missedBeats = 0;

        List<InputSequence> sequences = sequenceLibrary.sequences.Where(s => !s.ignore).ToList();

        if (sequences.Count == 0)
            throw new System.ArgumentOutOfRangeException("no valid input sequences");

        var candidates = sequences.Where(s => s.name != beats.currentSequence.name).ToList();
        beats.currentSequence = candidates.Count > 0
            ? candidates[Random.Range(0, candidates.Count)]
            : sequences[Random.Range(0, sequences.Count)];

        pastrySilhouette.SetActive(true);
        pastrySilhouette.GetComponent<SpriteRenderer>().sprite = beats.currentSequence.silhouette;

        input.SetHandReaction(Result.Ready);
        beats.PositionBeats(recreate: true);
        dough.LoopDoughSprites(reset: true);
    }

    IEnumerator ToggleHeartbeats(double eventDspTime, double secondsPerBeat)
    {
        // 4 bars * 4 beats per bar * 2 half-beats per beat = 32
        int numHalfBeats = 32;
        double secondsPerHalfBeat = secondsPerBeat / 2.0;
        for (int i = 0; i < numHalfBeats; i++)
        {
            yield return WaitUntilDspTime(eventDspTime + secondsPerHalfBeat * i);
            lives.ToggleHeartbeat();
        }
    }

    IEnumerator ScheduleInputWindow(double eventDspTime, double windowDuration, int beatIx)
    {
        yield return WaitUntilDspTime(eventDspTime);

        beats.OpenBeatWindow(beatIx);

        yield return WaitUntilDspTime(eventDspTime + windowDuration);

        beats.CloseBeatWindow(beatIx);
    }

    IEnumerator EvaluateScore(double eventDspTime, double secondsPerBeat)
    {
        yield return WaitUntilDspTime(eventDspTime);

        beats.RhythmRotate(reset: true);

        Debug.Log($"missed {beats.missedBeats} beats");

        if (beats.missedBeats <= _missTolerance)
        {
            StaticData.FriendsMade++;
            input.SetHandReaction(Result.Success);
            dough.SetResultSprite(beats.currentSequence.happySprite, secondsPerBeat);
            if (beats.missedBeats == 0)
                StaticData.Score += 20;
        }
        else
        {
            _gameOver = _gameOver || lives.LoseLife();
            input.SetHandReaction(Result.Miss);
            dough.SetResultSprite(beats.currentSequence.sadSprite, secondsPerBeat / 2);
        }

        Debug.Log($"current score: {StaticData.Score}");
        scoreText.text = StaticData.Score.ToString();

        _roundNumber++;

        if (_roundNumber % roundsToNextLevel == 0)
            _gameOver = _gameOver || gameAudio.NextLevel();
    }


    IEnumerator ScheduleGoToEnd(double eventDspTime)
    {
        yield return WaitUntilDspTime(eventDspTime);

        SceneNav.GoToGameOver();
    }


    IEnumerator WaitUntilDspTime(double eventDspTime)
    {
        // yield return new WaitUntil(() => AudioSettings.dspTime >= eventDspTime);
        double sleepThreshold = 0.02; // stop sleeping 20ms early
        double waitTime = eventDspTime - AudioSettings.dspTime - sleepThreshold;
        if (waitTime > 0)
            yield return new WaitForSecondsRealtime((float)waitTime);

        // spin the remaining ~20ms
        while (AudioSettings.dspTime < eventDspTime)
            yield return null;
    }
}
