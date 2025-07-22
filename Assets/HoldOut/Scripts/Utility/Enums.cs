namespace HoldOut
{
    public enum GameState
    {
        Initialization = 0,
        Loading = 1,
        Menu = 2,
        Game = 3,
        Pause = 4,
        GameOver = 5
    }

    public enum MenuType
    {
        Null = 0,
        LoadingMenu = 1,
        MainMenu = 2,
        HUDMenu = 3,
        PauseMenu = 4,
        GameOverMenu = 5
    }
}