static public class EndlessRunnerGameManager 
{
    public delegate void GameEvent();

    static public event GameEvent GameStart;

    static public event GameEvent GameOver;

    static public void TriggerGameStart()
    {
        if (GameStart != null)
            GameStart();
    }

    static public void TriggerGameOver()
    {
        if (GameOver != null)
            GameOver();
    }
}
