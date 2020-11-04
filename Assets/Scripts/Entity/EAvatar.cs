using System;
using System.Collections.Generic;
using UnityEngine;
using HexMap;

public class EAvatar : Entity
{
    public CControllerComponent controllerComponent { get; private set; }


    public CTeamComponent teamComponent { get; private set; }

    public override EntityType entityType { get { return EntityType.Avatar; } }

    UIAvatarHead m_HeadUI;

    MovementComponent m_MovementComponent;

    CAnimatorComponent m_AnimatorComponent;

    CAttackComponent m_AttackComponent;

    public CPropertyComponent propertyComponent { get; private set; }

    public CCreateDamageComponent createDamageComponent { get; private set; }

    public CTakeDamageComponent takeDamageComponent { get; private set; }

    protected override bool OnCreateComponent()
    {
        controllerComponent = AddCComponent<CControllerComponent>();
        teamComponent = AddCComponent<CTeamComponent>();
        m_MovementComponent = AddEntityComponent<MovementComponent>();
        m_AnimatorComponent = AddCComponent<CAnimatorComponent>();
        propertyComponent = AddCComponent<CPropertyComponent>();
        m_AttackComponent = AddCComponent<CAttackComponent>();
        createDamageComponent = AddCComponent<CCreateDamageComponent>();
        takeDamageComponent = AddCComponent<CTakeDamageComponent>();
        return true;
    }

    protected override bool OnInitialize(JEntityConfig config, BaseParams param)
    {
        return true;
    }

    protected override void OnBeginDestroy()
    {
        
    }

    protected override void OnDestoryComplete()
    {
        
    }

    protected override void OnBorn()
    {
        m_HeadUI = UIHeadWnd.instance.CreateAvatarHeadUI(this);
        m_HeadUI.SetFollowTarget(modelComponent.model.slot.headUI);
        base.OnBorn();
    }

    protected override void OnDead()
    {
        UIHeadWnd.instance.DestoryHeadUI(m_HeadUI);
        m_HeadUI = null;
        base.OnDead();
    }
}
