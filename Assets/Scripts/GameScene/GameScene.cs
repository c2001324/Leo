public abstract class GameScene
{
    public abstract GameSceneType type { get; }

    public abstract void EnterScene();

    public abstract void ExitScene();

    public delegate void GameSceneHandle(GameScene gameScene);

    protected void EnterComplete()
    {
        onEnterComplete.Invoke(this);
    }

    protected void ExitComplete()
    {
        onExitComplete.Invoke(this);
    }

    public event GameSceneHandle onEnterComplete;
    public event GameSceneHandle onExitComplete;
}

public enum GameSceneType
{
    ExitGame,       //退出游戏
    Title,              //游戏标题
    PreCombat,  //战斗准备
    Combat,         //战斗中
    Practice            //练习
}