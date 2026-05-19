using UnityEngine.SceneManagement;

public static class SceneNav
{
    public static void GoToTitle()
    {
        StaticData.currentState = GameState.Title;
        SceneManager.LoadScene("1_Intro");
    }
    public static void GoToTutorial() {
        StaticData.currentState = GameState.Tutorial;
        SceneManager.LoadScene("3_Main");
    }
    public static void GoToGame(){
        StaticData.currentState = GameState.Game;
        SceneManager.LoadScene("3_Main");
    }
    public static void GoToGameOver()
    {
        StaticData.currentState = GameState.GameOver;
        SceneManager.LoadScene("4_End");
    }
}
