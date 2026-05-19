using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// TODO: delete the start/restart buttons

public class EndSceneManager : MonoBehaviour
{

    [Header("Friends Made")]
    public GameObject friendsMadeObject;
    public GameObject friendsAnimation;
    public TMP_Text scoreText;
    public float friendsMadeSceneDuration = 5f;

    [Header("Initial Entry")]
    public GameObject newHighScoreObject;
    public InitialEntry initialEntry;
    public float initialEntryDelay = 2f;

    [Header("Top 10 Scoreboard")]
    public GameObject highScoresObject;
    public TMP_Text difficulty;
    public TMP_Text topFiveOrdinals;
    public TMP_Text topFiveInitials;
    public TMP_Text topFiveScores;
    public TMP_Text nextFiveOrdinals;
    public TMP_Text nextFiveInitials;
    public TMP_Text nextFiveScores;
    public TMP_Text yourScore;
    public float blinkDelay = 0.3f;
    public float highScoresDuration = 10f;

    Scoreboard _scoreboard;

    void Start()
    {
        friendsAnimation.SetActive(true);
        friendsMadeObject.SetActive(true);
        newHighScoreObject.SetActive(false);
        highScoresObject.SetActive(false);

        _scoreboard = Scoreboard.LoadScoreboard();
        StartCoroutine(DoFriendsMadeScene());
    }

    void ReadScoreboard()
    {
        List<ScoreEntry> topfive = _scoreboard.entries.Take(5).ToList();
        List<ScoreEntry> nextFive = _scoreboard.entries.Skip(5).Take(5).ToList();

        List<string> topFiveInitials = topfive.Select(entry => entry.initials).ToList();
        List<int> topFiveScores = topfive.Select(entry => entry.score).ToList();
        List<string> nextFiveInitials = nextFive.Select(entry => entry.initials).ToList();
        List<int> nextFiveScores = nextFive.Select(entry => entry.score).ToList();

        this.topFiveInitials.text = string.Join('\n', topFiveInitials);
        this.topFiveScores.text = string.Join('\n', topFiveScores);
        this.nextFiveInitials.text = string.Join('\n', nextFiveInitials);
        this.nextFiveScores.text = string.Join('\n', nextFiveScores);
    }

    IEnumerator DoFriendsMadeScene()
    {
        scoreText.text = StaticData.FriendsMade.ToString();

        yield return new WaitForSeconds(friendsMadeSceneDuration);

        if (_scoreboard.IsHighScore(StaticData.Score))
            StartCoroutine(ShowNewHighScoreEntry());
        else
            StartCoroutine(ShowHighScores());
    }

    IEnumerator ShowNewHighScoreEntry()
    {
        friendsAnimation.SetActive(false);
        friendsMadeObject.SetActive(false);
        newHighScoreObject.SetActive(true);

        int ord = _scoreboard.Compare(StaticData.Score) + 1;
        initialEntry.SetOrdinal(ord);
        StaticData.NewHighScoreOrdinal = ord;

        yield return new WaitUntil(initialEntry.IsEntered);

        string initials = initialEntry.GetInitials();
        _scoreboard.Update(StaticData.Score, initials);
        _scoreboard.Save();

        yield return new WaitForSeconds(initialEntryDelay);
        StartCoroutine(ShowHighScores());
    }

    IEnumerator ShowHighScores()
    {
        friendsAnimation.SetActive(true);
        friendsMadeObject.SetActive(false);
        newHighScoreObject.SetActive(false);
        highScoresObject.SetActive(true);

        ReadScoreboard();
        
        difficulty.text = $"{StaticData.difficulty.ToString().ToUpper()} MODE";
        yourScore.text = $"YOUR SCORE: {StaticData.Score}";

        int ord = StaticData.NewHighScoreOrdinal;
        if (1 <= ord && ord <= 5)
            StartCoroutine(BlinkOrd(ord - 1, topFiveOrdinals));
        else if (6 <= ord && ord <= 10)
            StartCoroutine(BlinkOrd((ord - 1) % 5, nextFiveOrdinals));

        yield return new WaitForSeconds(highScoresDuration);

        SceneNav.GoToTitle();
    }

    IEnumerator BlinkOrd(int ordIx, TMP_Text ordText)
    {
        bool isBlinked = false;
        string defaultText = ordText.text;
        string[] split = defaultText.Split('\n');
        split[ordIx] = " ";
        string blinkedText = string.Join('\n', split);

        while (true)
        {
            yield return new WaitForSeconds(blinkDelay);
            isBlinked = !isBlinked;
            if (isBlinked)
                ordText.text = blinkedText;
            else
                ordText.text = defaultText;
        }
    }
}

