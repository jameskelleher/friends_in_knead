using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioDifficultyLevel> difficultyLevels = new();
    [SerializeField] bool _mute;

    [HideInInspector] public AudioDifficultyLevel currentLevel;

    int _difficultyIx;
    AudioSource[] _sources;
    int _sourceIx = 10;

    void Awake()
    {
        _sources = new AudioSource[10];
        for (int i = 0; i < 10; i++)
            _sources[i] = gameObject.AddComponent<AudioSource>();

        _difficultyIx = StaticData.difficulty == Difficulty.Easy ? 0 : 3;
        currentLevel = difficultyLevels[_difficultyIx];
    }

    void OnValidate()
    {
        if (_sources == null) return;
        foreach (var src in _sources)
            src.volume = _mute ? 0 : 1;
    }

    public bool NextLevel()
    {
        double oldBpm = difficultyLevels[_difficultyIx].bpm;
        // return true if we game over, otherwise false
        _difficultyIx++;
        if (_difficultyIx < difficultyLevels.Count)
        {
            currentLevel = difficultyLevels[_difficultyIx];
            double newBpm = difficultyLevels[_difficultyIx].bpm;
            Debug.Log($"bpm: {oldBpm} -> {newBpm}");
            return false;
        }

        return true;
    }

    public bool mute
    {
        get => _mute;
        set
        {
            _mute = value;
            foreach (var src in _sources)
                src.volume = value ? 0 : 1;
        }
    }

    public double SecondsPerBeat() => 60.0 / currentLevel.bpm;

    AudioSource NextSource() => _sources[_sourceIx++ % _sources.Length];

    public void LoadIntro(double dspTime) => LoadClip(dspTime, currentLevel.intro);
    public void LoadBeat(double dspTime) => LoadClip(dspTime, currentLevel.beat);

    void LoadClip(double dspTime, AudioClip clip)
    {
        if (clip == null) { Debug.LogWarning("ScheduleClip: clip is null"); return; }

        AudioSource src = NextSource();
        src.clip = clip;
        src.PlayScheduled(dspTime);
    }
}

[Serializable]
public struct AudioDifficultyLevel
{
    public double bpm;
    public AudioClip intro;
    public AudioClip beat;
}
