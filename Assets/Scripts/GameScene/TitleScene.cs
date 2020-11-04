public class TitleScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.Title; } }

    protected override void OnEnterScene(GameSceneParam param)
    {
        UIManager.instance.OpenUI<UITitleWnd>(false);
        EnterComplete(null);
    }

    protected override void OnExitScene(GameSceneParam param)
    {
        UIManager.instance.CloseUI<UITitleWnd>();
        ExitComplete(null);
    }
}