using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Window
{
    private Queue<(float value, float time)> _queue;
    private float _duration;

    private float _max;
    private float _min;

    private float _freqNoiseCuttoff = 0.05f;

    public float amplitude;
    public float frequency;

    public Window(float duration)
    {
        _duration = duration;
        _queue = new Queue<(float, float)>();
    }

    public void Update(float value, float time)
    {
        _queue.Enqueue((value, time));
        while (_queue.Peek().time < time - _duration)
            _queue.Dequeue();

        _max = _queue.Max(item => item.value);
        _min = _queue.Min(item => item.value);

        if (_queue.Count() <= 1) return;

        // Average of the absolute difference between consecutive samples divided by time delta
        float avgSlope = _queue
            .Zip(_queue.Skip(1), (a, b) => Mathf.Abs(b.value - a.value) / (b.time - a.time))
            .Average();

        amplitude = (_max - _min) / 2f;
        frequency = amplitude > _freqNoiseCuttoff ? avgSlope / (2f * amplitude) : 0f;
    }
}