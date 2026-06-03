using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public InputConfig inputConfig;

    [Header("Hands")]
    public Hand left;
    public Hand right;
    [Range(0f, 0.1f)]
    public float resultXOffset;

    [Header("Gameplay Control")]
    [Tooltip("normalized from 0 to 1, in terms of travel distance pct")]
    public InputStrengthPartitions partitions = new InputStrengthPartitions { low = 0.15f, medium = 0.5f };
    [Tooltip("normalized from 0 to 1, in terms of screen height pct")]
    public ScreenYPosition yRange = new ScreenYPosition { low = 0.0f, high = 0.5f };
    public bool debugBoxRanges = false;

    void Update()
    {
        InputPair input = inputConfig.ReadInput();

        // map touch input from "absolute" space to "normalized" space
        if (inputConfig.currentInput == InputType.Touch)
            input = new InputPair(
                Mathf.Clamp01(Mathf.InverseLerp(yRange.low, yRange.high, input.left)),
                Mathf.Clamp01(Mathf.InverseLerp(yRange.low, yRange.high, input.right))
            );

        left.UpdatePos(input.left, yRange, partitions);
        right.UpdatePos(input.right, yRange, partitions);

        DebugBoxRanges();
    }


    public void SetHandReaction(Result result)
    {
        left.SetResult(result, resultXOffset);
        right.SetResult(result, resultXOffset);
    }

    void DebugBoxRanges()
    {
        if (!debugBoxRanges) return;

        float height = Camera.main.orthographicSize;
        float minY = Mathf.Lerp(-height, height, yRange.low);
        float maxY = Mathf.Lerp(-height, height, yRange.high);
        Debug.DrawLine(new Vector3(-100f, minY, 0f), new Vector3(100f, minY, 0f), Color.red);
        Debug.DrawLine(new Vector3(-100f, maxY, 0f), new Vector3(100f, maxY, 0f), Color.red);
    }
}
