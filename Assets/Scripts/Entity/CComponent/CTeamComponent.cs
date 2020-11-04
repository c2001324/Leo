using System;
using System.Collections.Generic;

public class CTeamComponent : CComponent
{
    public TeamId teamId { get; private set; }

    protected override void OnInitializeComplete()
    {
        base.OnInitializeComplete();
        teamId = TeamId.None;
    }

    public void SetTeamId(TeamId newTeamId)
    {
        if (newTeamId != teamId)
        {
            TeamId oldId = teamId;
            teamId = newTeamId;
            onTeamIdChanged.Invoke(this, newTeamId, oldId);
        }
    }

    public static Relation GetRelation(TeamId a, TeamId b)
    {
        if (a == TeamId.None || b == TeamId.None)
        {
            return Relation.Enemy;
        }
        else if (a == b)
        {
            return Relation.Teammate;
        }
        else
        {
            return Relation.Enemy;
        }
    }

    public static Relation GetRelation(CTeamComponent a, CTeamComponent b)
    {
        if (a == null || b == null)
        {
            return Relation.None;
        }

        if (a.entity == b.entity)
        {
            return Relation.Self;
        }
        else
        {
            return GetRelation(a.teamId, b.teamId);
        }
    }

    public static Relation GetRelation(Entity a, Entity b)
    {
        return GetRelation(a.GetCComponent<CTeamComponent>(), b.GetCComponent<CTeamComponent>());
    }

    public Relation GetRelation(CTeamComponent other)
    {
        return GetRelation(this, other);
    }

    public Relation GetRelation(Entity other)
    {
        return GetRelation(entity, other);
    }

    public Relation GetRelation(TeamId otherId)
    {
        return GetRelation(teamId, otherId);
    }

    public TeamIdChangedEvent onTeamIdChanged = new TeamIdChangedEvent();

    public class TeamIdChangedEvent : CustomEvent<CTeamComponent, TeamId, TeamId> { }


}

public enum Relation
{
    None,
    Self,
    Teammate,
    Enemy,
}

public enum TeamId
{
    None,       //不属于任何一队
    Team_0,     //默认为玩家
    Team_1,     
    Team_2,
    Team_3,
    Team_4,
    Team_5,
    Team_6,
}