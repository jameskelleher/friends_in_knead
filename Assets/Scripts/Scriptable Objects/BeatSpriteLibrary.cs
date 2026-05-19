using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeatSpriteLibrary", menuName = "Scriptable Objects/BeatSpriteLibrary")]
public class BeatSpriteLibrary : ScriptableObject
{
    public Sprite restSpriteReady;
    public Sprite restSpriteSuccess;
    public Sprite restSpriteMiss;
    public List<BeatSpriteData> entries;

    public Sprite FindSprite(BeatInfo query)
    {
        if (query.inputStrength == InputStrength.Rest)
        {
            if (query.result == Result.Ready)
                return restSpriteReady;
            else if (query.result == Result.Success)
                return restSpriteSuccess;
            else if (query.result == Result.Miss)
                return restSpriteMiss;
            else
                throw new ArgumentOutOfRangeException($"unexpected result {query.result}");
        }

        foreach (var entry in entries)
        {
            bool pos = query.position == entry.beatInfo.position;
            bool str = query.inputStrength == entry.beatInfo.inputStrength;
            bool sta = query.result == entry.beatInfo.result;

            if (pos && str && sta) return entry.sprite;
        }

        throw new ArgumentOutOfRangeException($"no sprite found for query {query}");
    }

    [ContextMenu("Regenerate Entries")]
    void RegenerateEntries()
    {
        var positions = new HashSet<Position>((Position[])Enum.GetValues(typeof(Position)));
        var strengths = new HashSet<InputStrength>((InputStrength[])Enum.GetValues(typeof(InputStrength)));
        var results = new HashSet<Result>((Result[])Enum.GetValues(typeof(Result)));

        strengths.Remove(InputStrength.Rest);

        var existing = new HashSet<BeatInfo>();
        var list = new List<BeatSpriteData>();

        foreach (var entry in entries)
        {
            var info = entry.beatInfo;
            if (positions.Contains(info.position) &&
                strengths.Contains(info.inputStrength) &&
                results.Contains(info.result) &&
                !existing.Contains(info))
            {
                list.Add(entry);
                existing.Add(info);
            }
        }

        foreach (var pos in positions)
            foreach (var str in strengths)
                foreach (var sta in results)
                {
                    var info = new BeatInfo { position = pos, inputStrength = str, result = sta };
                    if (!existing.Contains(info))
                        list.Add(new BeatSpriteData { beatInfo = info });
                }

        entries = list;
        entries.Sort();

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var info = entry.beatInfo;
            entry.name = $"{info.position} / {info.inputStrength} / {info.result}";
            entries[i] = entry;
        }
    }

    void OnValidate()
    {
        if (entries == null) return;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var info = entry.beatInfo;
            entry.name = $"{info.position} / {info.inputStrength} / {info.result}";
            entries[i] = entry;
        }
    }
}
