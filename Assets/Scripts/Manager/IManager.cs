using System.Collections.Generic;

/// <summary>
/// 不用初始化的管理器
/// </summary>
public interface IDonotInitManager
{

}

/// <summary>
/// 异步初始化的管理器
/// </summary>
public interface ISyncInitManager
{
    IEnumerable<ManagerProgress> Initialize();
}

/// <summary>
/// 简单同步初始化的管理器
/// </summary>
public interface ISimpleInitManager
{
    void Initialize();
}



public struct ManagerProgress
{
    public ManagerProgress(float p, string s)
    {
        progress = p;
        describe = s;
    }
    public float progress;
    public string describe;
}