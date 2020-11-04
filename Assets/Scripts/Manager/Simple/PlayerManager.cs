using System.Collections;
using System.Collections.Generic;
using Untility;
using UnityEngine;

/// <summary>
/// 关卡的通用配置
/// </summary>
public class PlayerManager : Singleton<PlayerManager>, ISimpleInitManager
{

    public IEnumerable<EAvatar> avatars { get { return m_AvatarList; } }
    //玩家可以操控的Avatar
    List<EAvatar> m_AvatarList = new List<EAvatar>();

    public bool hasInitializePlayer { get; private set; }

    public void Initialize()
    {
        hasInitializePlayer = false;
    }

    public void AddAvatar(EAvatar avatar)
    {
        if (!m_AvatarList.Contains(avatar))
        {
            m_AvatarList.Add(avatar);
            GameEvent.PlayerEvent.FireOnPlayerBorn(avatar);
        }
    }

    public void RemoveAvatar(EAvatar avatar)
    {
        m_AvatarList.Remove(avatar);
    }

    public bool HasEntity(Entity entity)
    {
        EAvatar avatar = entity as EAvatar;
        if (avatar != null && m_AvatarList.Contains(avatar))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 初始化玩家
    /// </summary>
    /// <param name="level"></param>
    public void InitilaizePlayer(RoomInstance room)
    {
        if (!hasInitializePlayer)
        {
            hasInitializePlayer = true;
            EAvatar avatar = EntityManager.instance.Create<EAvatar>("default_player", room, room.hexRoom.FindClearCell(), null);
            avatar.teamComponent.SetTeamId(TeamId.Team_0);
            avatar.controllerComponent.SetController(new InputController());
            avatar.propertyComponent.SetInitProperty(new PropertyValue() { actionPriority = 10, movePointMax = 5 , hpMax = 10, attackRange = 1});
            avatar.propertyComponent.Born();
            AddAvatar(avatar);

            avatar = EntityManager.instance.Create<EAvatar>("default_monster_0", room, room.hexRoom.FindClearCell(), null);
            avatar.teamComponent.SetTeamId(TeamId.Team_1);
            avatar.controllerComponent.SetController(new InputController());
            avatar.propertyComponent.SetInitProperty(new PropertyValue() { actionPriority = 2, movePointMax = 5, hpMax = 10, attackRange = 1 });
            avatar.propertyComponent.Born();

            avatar = EntityManager.instance.Create<EAvatar>("default_monster_1", room, room.hexRoom.FindClearCell(), null);
            avatar.teamComponent.SetTeamId(TeamId.Team_1);
            avatar.controllerComponent.SetController(new InputController());
            avatar.propertyComponent.SetInitProperty(new PropertyValue() { actionPriority = 3, movePointMax = 5, hpMax = 10, attackRange = 1 });
            avatar.propertyComponent.Born();
        }
    }

    public void DestoryPlayer()
    {
        hasInitializePlayer = false;
    }
}