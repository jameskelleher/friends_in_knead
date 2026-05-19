using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class Scoreboard
{
    public const int MaxEntries = 10;
    public List<ScoreEntry> entries = new();
    public static Scoreboard instance = null;

    public Scoreboard()
    {
        for (int i = 0; i < MaxEntries; i++)
            entries.Add(new ScoreEntry(0, "JAM"));
    }

    static string Path(string filename) => System.IO.Path.Combine(Application.persistentDataPath, filename);
    static string DefaultFilename => StaticData.difficulty == Difficulty.Easy ? "scores_easy.json" : "scores_hard.json";

    public static Scoreboard LoadScoreboard(string fn = null)
    {
        fn ??= DefaultFilename;
        string path = Path(fn);

        Scoreboard board;

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                board = JsonUtility.FromJson<Scoreboard>(json);
                Debug.Log($"Loaded {board.entries.Count} scores");
            }
            catch (System.Exception e)
            {
                Debug.Log($"Failed to parse scores file: {e.Message}");
                board = new Scoreboard();
            }
        }
        else
        {
            Debug.Log("No scores file found, creating new scoreboard");
            board = new Scoreboard();
        }

        return board;
    }

    public void Save(string fn = null)
    {
        fn ??= DefaultFilename;
        string path = Path(fn);
        try
        {
            string json = JsonUtility.ToJson(this, prettyPrint: true);
            File.WriteAllText(path, json);
            Debug.Log("scores saved");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to save scores: {e.Message}");
        }
    }

    public int Compare(int newScore)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].score < newScore)
                return i;
        }
        return MaxEntries;
    }

    public void Update(int newScore, string newInitials)
    {
        entries.Add(new ScoreEntry(newScore, newInitials));
        entries = entries.OrderByDescending(e => e.score).Take(MaxEntries).ToList();
    }

    public bool IsHighScore(int newScore) => Compare(newScore) < MaxEntries;
}

[System.Serializable]
public class ScoreEntry
{
    public int score;
    public string initials;

    public ScoreEntry(int score, string initials)
    {
        this.score = score;
        this.initials = initials;
    }
}