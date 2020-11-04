using UnityEngine;

/// <summary>
/// 战场内物体的UI元素
/// </summary>
public abstract class UIFollowEntity : MonoBehaviour
{
    Transform m_FollowTarget;

    bool m_HasInitialized = false;

    Vector3 m_PositionOffset = Vector3.zero;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (!m_HasInitialized)
        {
            m_HasInitialized = true;
            OnInitialize();
        }
    }

    protected abstract void OnInitialize();

    public void SetVisable(bool b)
    {
        gameObject.SetActive(b);
    }

    public void SetFollowTarget(Transform target)
    {
        ResetFollowTarget();
        m_FollowTarget = target;
        gameObject.SetActive(true);
    }

    public void SetOffset(Vector3 offset)
    {
        m_PositionOffset = offset;
    }

    public void ResetFollowTarget()
    {
        m_FollowTarget = null;
        gameObject.SetActive(false);
        m_PositionOffset = Vector3.zero;
    }

    public virtual void ResetData()
    {

    }
    

    private void Update()
    {
        //跟随
        if (m_FollowTarget != null)
        {
            transform.position = CameraManager.instance.camera.WorldToScreenPoint(m_FollowTarget.position + m_PositionOffset);
        }
    }

 
}
