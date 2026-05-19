using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public InputManager input;
    public SFXManager sfx;
    [Header("Beat Config")]
    public GameObject beatPrefab;
    public GameObject beatsParent;
    [Range(1f, 4f)]
    public float beatZoneWidth = 0.1f;
    [Range(0.01f, 1f)]
    public float beatZoneHeight = 0.1f;
    public bool showExactBeat = false;


    [HideInInspector] public List<Beat> beats = new();
    [HideInInspector] public int missedBeats;
    [HideInInspector] public InputSequence currentSequence;

    int _beatIx = 0;

    const int BEATS_PER_SEQUENCE = 8;
    const int BEATS_PER_ROW = 4;

    void OnValidate()
    {
        if (beats == null || beats.Count() == 0)
            return;
        PositionBeats();
    }

    public Result Poll()
    {
        if (beats.Count == 0)
            return Result.NoUpdate;

        Beat beat = beats[_beatIx];

        if (!beat.isActive)
            return Result.NoUpdate;

        if (beat.isActive && beat.InvalidInput(input.left, input.right))
        {
            return UpdateResult(Result.Miss);
        }

        if (beat.SuccessDetected(input.left, input.right))
            return UpdateResult(Result.Success);

        return Result.NoUpdate;
    }

    public void PositionBeats(bool recreate = false)
    {
        if (recreate)
        {
            foreach (var beat in beats)
                Destroy(beat.gameObject);
            beats.Clear();
        }

        float stepSize = beatZoneWidth / (BEATS_PER_ROW - 1);
        float xOffset = -beatZoneWidth / 2;

        for (int i = 0; i < BEATS_PER_SEQUENCE; i++)
        {
            float y = i < BEATS_PER_ROW ? beatZoneHeight : -beatZoneHeight;
            Vector3 pos = new Vector3(xOffset + i % BEATS_PER_ROW * stepSize, y, 0f);

            if (recreate)
            {
                Beat beat = Instantiate(beatPrefab, beatsParent.transform).GetComponent<Beat>();
                beat.transform.localPosition = pos;
                beat.gameObject.SetActive(false);
                beat.Init(currentSequence.steps[i]);
                beats.Add(beat);
                beat.gameObject.SetActive(true);
            }
            else
                beats[i].UpdateDefaultPos(pos);
        }
    }

    public void OpenBeatWindow(int beatIx, bool doRotate = true)
    {
        // TODO: fix beatix issues
        _beatIx = beatIx;
        Beat beat = beats[_beatIx];
        beat.isActive = true;

        // string subMessage = "";

        // if (showExactBeat)
        // {
        //     if (beat.beatInfo.inputStrength == InputStrength.Rest)
        //         subMessage = "Rest";
        //     else
        //         subMessage = $"\n{beat.beatInfo.position} {beat.beatInfo.inputStrength}";
        // }

        // if (doRotate)
        //     beats.ForEach(beat => beat.RhythmRotate());
    }

    public Result CloseBeatWindow(int beatIx)
    {
        _beatIx = beatIx;
        Beat beat = beats[_beatIx];
        if (!beat.isActive)
            return Result.NoUpdate;

        beat.isActive = false;

        if (beat.beatInfo.result == Result.Ready)
        {
            if (beat.beatInfo.inputStrength == InputStrength.Rest)
                return UpdateResult(Result.Success);
            else
                return UpdateResult(Result.Miss);
        }

        return Result.NoUpdate;
    }

    Result UpdateResult(Result result)
    {
        Beat beat = beats[_beatIx];

        switch (result)
        {
            case Result.Success:
                if (beat.beatInfo.result != Result.Ready)
                    return Result.NoUpdate;
                sfx.PlayDefault(beat);
                StaticData.Score += 10;
                break;
            case Result.Miss:
                if (beat.beatInfo.result == Result.Miss)
                    return Result.NoUpdate;
                else if (beat.beatInfo.result == Result.Success)
                    StaticData.Score -= 10;
                missedBeats += 1;
                break;
            default:
                throw new System.ArgumentOutOfRangeException($"unexpected result {result}");
        }

        beat.SetResult(result);
        return result;

    }

    public void RhythmRotate(bool reset = false) => beats.ForEach(beat => beat.RhythmRotate(reset));

}
