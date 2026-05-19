using System;
using UnityEngine;

public enum InputStrength { Rest, Soft, Hard }
public enum Position { Left, Right, Both }
public enum Result { Ready, Success, Miss, NoUpdate }
public enum BeatRotation { None, Left, Right }

public struct InputPair
{
    public float left, right;

    public InputPair(float left, float right)
    {
        this.left = left;
        this.right = right;
    }
}

[Serializable]
public struct InputStrengthPartitions
{
    [Range(0f, 1f)] public float low;
    [Range(0f, 1f)] public float medium;
}

[Serializable]
public struct ScreenYPosition
{
    [Range(0f, 1f)] public float low;
    [Range(0f, 1f)] public float high;
}


// NOTE: uses InputStepDrawer.cs for rendering in inspector
[Serializable]
public struct InputStep
{
    public InputStrength strength;
    public Position position;
    public AudioClip sound;
}

// NOTE: uses InputSequenceDrawer.cs for rendering in inspector
[Serializable]
public struct InputSequence
{
    public string name;
    public Sprite happySprite;
    public Sprite sadSprite;
    public Sprite silhouette;
    public bool ignore;
    public InputStep[] steps;
}

[Serializable]
public struct BeatInfo : IEquatable<BeatInfo>
{
    public Position position;
    public InputStrength inputStrength;
    public AudioClip sound;
    public Result result;

    public override string ToString() =>
        $"pos: {position}, str: {inputStrength}, res: {result}";

    public static BeatInfo FromStep(InputStep step, Result beatState)
    {
        return new BeatInfo
        {
            position = step.position,
            inputStrength = step.strength,
            sound = step.sound,
            result = beatState
        };
    }

    public bool Equals(BeatInfo other)
    {
        bool pos = position == other.position;
        bool str = inputStrength == other.inputStrength;
        bool sta = result == other.result;
        return pos && str && sta;
    }

    public static bool operator ==(BeatInfo a, BeatInfo b) => a.Equals(b);
    public static bool operator !=(BeatInfo a, BeatInfo b) => !a.Equals(b);

    public override int GetHashCode() => HashCode.Combine(position, inputStrength, result);
    public override bool Equals(object obj) => obj is BeatInfo other && Equals(other);
}

[Serializable]
public struct BeatSpriteData : IComparable<BeatSpriteData>
{
    [HideInInspector]
    public string name;
    [HideInInspector]
    public BeatInfo beatInfo;
    public Sprite sprite;

    public int CompareTo(BeatSpriteData other)
    {
        int pos = beatInfo.position.CompareTo(other.beatInfo.position);
        if (pos != 0) return pos;
        int str = beatInfo.inputStrength.CompareTo(other.beatInfo.inputStrength);
        if (str != 0) return str;
        return beatInfo.result.CompareTo(other.beatInfo.result);
    }
}
