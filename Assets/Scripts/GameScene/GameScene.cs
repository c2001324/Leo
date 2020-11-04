public abstract class GameScene
{
    public abstract GameSceneType type { get; }

    public void EnterScene(GameSceneParam param)
    {
        OnEnterScene(param);
    }

    public void ExitScene(GameSceneParam param)
    {
        OnExitScene(param);
        //关闭共同的窗口
        UIManager.instance.CloseUI<UICombatMenuWnd>();
        UIManager.instance.CloseUI<UICombatMainWnd>();
        UIManager.instance.CloseUI<UIHeadWnd>();
    }

    protected abstract void OnEnterScene(GameSceneParam param);

    protected abstract void OnExitScene(GameSceneParam param);

    public delegate void GameSceneHandle(GameScene gameScene, GameSceneParam param);

    protected void EnterComplete(GameSceneParam param)
    {
        onEnterComplete.Invoke(this, param);
    }

    protected void ExitComplete(GameSceneParam param)
    {
        onExitComplete.Invoke(this, param);
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

public class GameSceneParam
{
    public ExitLevelType exitLevelType;
    public int enterLevelIndex;
    public int seed;
}