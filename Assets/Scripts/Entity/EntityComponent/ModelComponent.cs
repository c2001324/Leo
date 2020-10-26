using UnityEngine;

/// <summary>
/// 模型组件
/// </summary>
public class ModelComponent : EntityComponentBase
{
    public EntityModel model { get; private set; }

    bool m_ModelVisible = true;

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


    readonly public ModelChangedEvent onSetModel = new ModelChangedEvent();
    readonly public ModelChangedEvent onRemoveModel = new ModelChangedEvent();

    public class ModelChangedEvent : CustomEvent<EntityModel> { }
}
