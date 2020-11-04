using UnityEngine;
using UnityEditor;

public abstract class Controller
{
    public bool isControlling { get { return m_ControllerAction != null; } }

    protected IControllerAction m_ControllerAction;

    public Entity GetEntity()
    {
        if (m_ControllerAction != null)
        {
            return m_ControllerAction.GetEntity();
        }
        else
        {
            return null;
        }
    }

    public Controller()
    {
        
    }

    /// <summary>
    /// 开始控制
    /// </summary>
    public void BeginControll(IControllerAction controllerAction)
    {
        if (!isControlling)
        {
            m_ControllerAction = controllerAction;
            OnBeginControll(controllerAction);
        }
    }

    /// <summary>
    /// 结束控制
    /// </summary>
    public void StopControll(IControllerAction controllerAction)
    {
        if (isControlling)
        {
            OnStopControll(controllerAction);
            m_ControllerAction = null;
        }
    }

    protected abstract void OnBeginControll(IControllerAction controllerAction);

    protected abstract void OnStopControll(IControllerAction controllerAction);
}