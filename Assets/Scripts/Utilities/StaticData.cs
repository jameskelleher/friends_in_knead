public static class StaticData
{
    public static int FriendsMade;
    public static int Score;
    public static int NewHighScoreOrdinal;
    public static GameState currentState;
    public static Difficulty difficulty;
    
}

public enum GameState { Null, Title, Tutorial, Game, GameOver }
public enum Difficulty { Easy, Hard }
