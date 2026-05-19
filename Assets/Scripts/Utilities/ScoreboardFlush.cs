using UnityEngine;

public class ScoreboardFlush : MonoBehaviour
{
    void Start()
    {
        new Scoreboard().Save("scores_easy.json");
        new Scoreboard().Save("scores_hard.json");
    }

}
