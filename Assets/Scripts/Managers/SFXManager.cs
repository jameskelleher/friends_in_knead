using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    public AudioClip left, right, both;
    
    AudioSource _srcSoft, _srcHard;
    Dictionary<Position, AudioClip> _clipDict = new();

    void Start()
    {
        var sources = GetComponents<AudioSource>();
        _srcSoft = sources[0];
        _srcHard = sources[1];

        _srcSoft.pitch = 1.5f;
        _srcHard.pitch = 0.66f;

        _clipDict[Position.Left] = left;
        _clipDict[Position.Right] = right;
        _clipDict[Position.Both] = both;
    }

    public void PlayDefault(Beat beat)
    {
        AudioClip sfx = _clipDict[beat.beatInfo.position];

        if (beat.IsSoft)
            _srcSoft.PlayOneShot(sfx);
        else if (beat.IsHard)
            _srcHard.PlayOneShot(sfx);
    }

    public void PlayBeatSfx(Beat beat)
    {
        AudioClip sfx = beat.beatInfo.sound;

        if (sfx == null) return;

        if (beat.IsSoft)
            _srcSoft.PlayOneShot(sfx);

        else if (beat.IsHard)
            _srcHard.PlayOneShot(sfx);
    }
}
