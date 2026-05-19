using System;
using System.Collections;
using UnityEngine;

public class SpriteDance : MonoBehaviour
{
    [Range(0, 3)]
    public int startIndex;
    [Range(0f, 3f)]
    public float interval = 1f;
    public float rotation = 15;

    [HideInInspector] static float _startAt = 0f;
    float _nextUpdate;
    int _patternIx;
    DanceState[] _pattern = { DanceState.Left, DanceState.Up, DanceState.Right, DanceState.Up };


    void Start()
    {
        _patternIx = startIndex;

        if (_startAt == 0f || Time.time - _startAt > 1f)
            _startAt = Time.time;

        float elapsed = Time.time - _startAt;
        float beatsElapsed = Mathf.Floor(elapsed / interval);
        _nextUpdate = _startAt + (beatsElapsed + 1) * interval;

        StartCoroutine(DoDance());
    }

    void Update()
    {
        DanceState state = _pattern[_patternIx];

        switch (state)
        {
            case DanceState.Left:
                transform.localRotation = Quaternion.Euler(0f, 0f, -rotation);
                break;
            case DanceState.Right:
                transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
                break;
            case DanceState.Up:
                transform.localRotation = Quaternion.identity;
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unexpected dance state {state}");
        }
    }

    IEnumerator DoDance()
    {
        while (true)
        {
            yield return new WaitUntil(() => Time.time > _nextUpdate);
            _patternIx = (_patternIx + 1) % _pattern.Length;
            _nextUpdate += interval;
        }
    }

    enum DanceState { Up, Left, Right }

}
