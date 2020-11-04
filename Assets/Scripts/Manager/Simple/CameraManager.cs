using UnityEngine;
using Untility;


public class CameraManager : Singleton<CameraManager>, ISimpleInitManager
{


    RtsCamera m_RTSCamera;

    public Camera camera { get; private set; }

    public void Initialize()
    {
        camera = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Camera>();
        m_RTSCamera = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<RtsCamera>();
        GameEvent.TurnBaseEvent.onEntityTurnStart.AddListener(OnEntityTurnStart);
    }

    void OnEntityTurnStart(CControllerComponent controller)
    {
        m_RTSCamera.JumpTo(controller.entity.transform);
        m_RTSCamera.Follow(controller.entity.transform);
    }
    

    
}

