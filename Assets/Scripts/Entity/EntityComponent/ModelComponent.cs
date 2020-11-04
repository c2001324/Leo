using UnityEngine;

/// <summary>
/// 模型组件
/// </summary>
public class ModelComponent : EntityComponentBase
{
    public EntityModel model { get; private set; }

    bool m_ModelVisible = true;

    CTeamComponent m_TeamComponent;

    /// <summary>
    ///  是否可以被选择
    /// </summary>
    public bool isSelectable { get; set; }

    protected override void OnInitializeComplete()
    {
        isSelectable = true;
        m_TeamComponent = entity.GetCComponent<CTeamComponent>();
        base.OnInitializeComplete();
    }

    public void SetModelVisible(bool b)
    {
        m_ModelVisible = b;
        if (model != null)
        {
            model.visiable = b;
        }
    }

    public void SetModel(EntityModel model)
    {
        if (model != null)
        {
            ResetModel();
            model.AttachToModelComponent(this);
            this.model = model;
            SetModelVisible(m_ModelVisible);
            onSetModel.Invoke(model);
        }
    }

    public void ResetModel()
    {
        if (model != null)
        {
            model.DeattachFromModelComponent();
            onRemoveModel.Invoke(null);
            model = null;
        }
    }

    public void Select(EntityModelSelectType type)
    {
        if (isSelectable)
        {
            entity.OnSelect(type);
            onSelect.Invoke();
            model.SetHighlight(CTeamComponent.GetRelation(TeamId.Team_0, m_TeamComponent.teamId));
        }
    }

    public void UnSelect(EntityModelSelectType type)
    {
        if (isSelectable)
        {
            entity.OnUnselect(type);
            onUnselect.Invoke();
            model.CloseHighlight();
        }
    }

    public void LeftClick(EntityModelSelectType type)
    {
        if (isSelectable)
        {
            entity.OnLeftClick(type);
            onLeftClick.Invoke();
        }
    }

    public void RightClick(EntityModelSelectType type)
    {
        if (isSelectable)
        {
            entity.OnRightClick(type);
            onRightClick.Invoke();
        }
    }

    readonly public ModelChangedEvent onSetModel = new ModelChangedEvent();
    readonly public ModelChangedEvent onRemoveModel = new ModelChangedEvent();

    readonly public MouseEvent onSelect = new MouseEvent();
    readonly public MouseEvent onUnselect = new MouseEvent();
    readonly public MouseEvent onLeftClick = new MouseEvent();
    readonly public MouseEvent onRightClick = new MouseEvent();

    public class ModelChangedEvent : CustomEvent<EntityModel> { }

    public class MouseEvent : CustomEvent { }
}

public enum EntityModelSelectType
{
    FromModel,
    FromCell
}