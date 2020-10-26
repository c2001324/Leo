
public enum ComponentState
{
    PendingInititalize,     //等待初始化
    Inititalize,                //初始化完成
    PendingDestroy,     //等待被销毁
    Destroy                     //已销毁
}

public interface IComponent
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool Initialize(Entity entity);

    /// <summary>
    /// 初始化完成
    /// </summary>
    void InitializeComplete();

    /// <summary>
    /// 开始销毁
    /// </summary>
    void BeginDestroy();

    /// <summary>
    /// 销毁完成
    /// </summary>
    void DestroyComplete();
}