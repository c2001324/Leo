using UnityEngine;
using HexMap;

public static class GameEvent
{
    public static class SceneEvent
    {
        public static void FireOnBeginEnterScene(GameSceneType type)
        {
            onBeginEnterScene.Invoke(type);
        }

        public static void FireOnEnterSceneComplete(GameSceneType type)
        {
            onEnterSceneComplete.Invoke(type);
        }

        public static void FireOnBeginExitScene(GameSceneType type)
        {
            onBeginExitScene.Invoke(type);
        }

        public static void FireOnExitSceneComplete(GameSceneType type)
        {
            onExitSceneComplete.Invoke(type);
        }

        public static GameSceneEvent onBeginEnterScene = new GameSceneEvent();
        public static GameSceneEvent onEnterSceneComplete = new GameSceneEvent();
        public static GameSceneEvent onBeginExitScene = new GameSceneEvent();
        public static GameSceneEvent onExitSceneComplete = new GameSceneEvent();

        public class GameSceneEvent : CustomEvent<GameSceneType> { }
    }

    public static class LevelEvent
    {
        public static void FireOnBeginBuildLevel(LevelInstance level)
        {
            Debug.Log("onBeginBuildLevel");
            onBeginBuildLevel.Invoke(level);
        }

        public static void FireOnBuildLevelComplete(LevelInstance level)
        {
            Debug.Log("onBuildLevelComplete");
            onBuildLevelComplete.Invoke(level);
        }

        public static void FireOnBeginDestroyLevel(LevelInstance level, ExitLevelType type)
        {
            Debug.Log("onBeginDestroyLevel");
            onBeginDestroyLevel.Invoke(level, type);
        }

        public static void FireOnDestroyLevelComplete(LevelInstance level, ExitLevelType type)
        {
            Debug.Log("onDestroyLevelComplete");
            onDestroyLevelComplete.Invoke(level, type);
        }

        public static void FireOnBeforeEnterLevel(LevelInstance level)
        {
            Debug.Log("onBeforeEnterLevel");
            onBeforeEnterLevel.Invoke(level);
        }

        public static void FireOnEnterLevel(LevelInstance level)
        {
            Debug.Log("onEnterLevel");
            onEnterLevel.Invoke(level);
        }

        public static void FireOnBeforeExitLevel(LevelInstance level, ExitLevelType type)
        {
            Debug.Log("onBeforeExitLevel");
            onBeforeExitLevel.Invoke(level, type);
        }

        public static void FireOnExitLevel(LevelInstance level, ExitLevelType type)
        {
            Debug.Log("onExitLevel");
            onExitLevel.Invoke(level, type);
        }

        public static BuildLevelEvent onBeginBuildLevel = new BuildLevelEvent();
        public static BuildLevelEvent onBuildLevelComplete = new BuildLevelEvent();

        public static DestroyLevelEvent onBeginDestroyLevel = new DestroyLevelEvent();
        public static DestroyLevelEvent onDestroyLevelComplete = new DestroyLevelEvent();

        public static BuildLevelEvent onBeforeEnterLevel = new BuildLevelEvent();
        public static BuildLevelEvent onEnterLevel = new BuildLevelEvent();

        public static DestroyLevelEvent onBeforeExitLevel = new DestroyLevelEvent();
        public static DestroyLevelEvent onExitLevel = new DestroyLevelEvent();

        public class BuildLevelEvent : CustomEvent<LevelInstance> { }
        public class DestroyLevelEvent : CustomEvent<LevelInstance, ExitLevelType> { }
    }

    public static class AreaEvent
    {
        public static void FireOnBeginBuildArea(LevelInstance level, AreaInstance area)
        {
            Debug.Log("onBeginBuildArea");
            onBeginBuildArea.Invoke(level, area);
        }

        public static void FireOnBuildAreaComplete(LevelInstance level, AreaInstance area)
        {
            Debug.Log("onBuildAreaComplete");
            onBuildAreaComplete.Invoke(level, area);
        }

        public static void FireOnBeginDestroyArea(LevelInstance level, AreaInstance area, ExitAreaType type)
        {
            Debug.Log("onBeginDestroyArea");
            onBeginDestroyArea.Invoke(level, area, type);
        }

        public static void FireOnDestroyAreaComplete(LevelInstance level, AreaInstance area, ExitAreaType type)
        {
            Debug.Log("onDestroyAreaComplete");
            onDestroyAreaComplete.Invoke(level, area, type);
        }

        public static void FireOnBeforeEnterArea(LevelInstance level, AreaInstance area)
        {
            Debug.Log("onBeforeEnterArea");
            onBeforeEnterArea.Invoke(level, area);
        }

        public static void FireOnEnterArea(LevelInstance level, AreaInstance area)
        {
            Debug.Log("onEnterArea");
            onEnterArea.Invoke(level, area);
        }

        public static void FireOnBeforeExitArea(LevelInstance level, AreaInstance area, ExitAreaType type)
        {
            Debug.Log("onBeforeExitArea");
            onBeforeExitArea.Invoke(level, area, type);
        }

        public static void FireOnExitArea(LevelInstance level, AreaInstance area, ExitAreaType type)
        {
            Debug.Log("onExitArea");
            onExitArea.Invoke(level, area, type);
        }

        public static BuildAreaEvent onBeginBuildArea = new BuildAreaEvent();
        public static BuildAreaEvent onBuildAreaComplete = new BuildAreaEvent();

        public static DestroyAreaEvent onBeginDestroyArea = new DestroyAreaEvent();
        public static DestroyAreaEvent onDestroyAreaComplete = new DestroyAreaEvent();


        public static BuildAreaEvent onBeforeEnterArea = new BuildAreaEvent();
        public static BuildAreaEvent onEnterArea = new BuildAreaEvent();

        public static DestroyAreaEvent onBeforeExitArea = new DestroyAreaEvent();
        public static DestroyAreaEvent onExitArea = new DestroyAreaEvent();

        public class BuildAreaEvent : CustomEvent<LevelInstance, AreaInstance> { }
        public class DestroyAreaEvent : CustomEvent<LevelInstance, AreaInstance, ExitAreaType> { }
    }

    public static class RoomEvent
    {
        public static void FireOnBeforeEnterRoom(RoomInstance room)
        {
            Debug.Log("onBeforeEnterRoom");
            onBeforeEnterRoom.Invoke(room);
        }

        public static void FireOnEnterRoom(RoomInstance room)
        {
            Debug.Log("onEnterRoom");
            onEnterRoom.Invoke(room);
        }

        public static void FireOnBeforeExitRoom(RoomInstance room, ExitRoomType type)
        {
            Debug.Log("onBeforeExitRoom");
            onBeforeExitRoom.Invoke(room, type);
        }

        public static void FireOnExitRoom(RoomInstance room, ExitRoomType type)
        {
            Debug.Log("onExitRoom");
            onExitRoom.Invoke(room, type);
        }

        public static void FireOnBeginBuildRoom(RoomInstance room)
        {
            Debug.Log("onBeginBuildRoom");
            onBeginBuildRoom.Invoke(room);
        }

        public static void FireOnBuildRoomComplete(RoomInstance room)
        {
            Debug.Log("onBuildRoomComplete");
            onBuildRoomComplete.Invoke(room);
        }

        public static void FireOnBeginDestroyRoom(RoomInstance room)
        {
            Debug.Log("onBeginDestroyRoom");
            onBeginDestroyRoom.Invoke(room);
        }

        public static void FireOnDestroyRoomComplete(RoomInstance room)
        {
            Debug.Log("onDestroyRoomComplete");
            onDestroyRoomComplete.Invoke(room);
        }

        public static void FireOnCompleteRoom(RoomInstance room)
        {
            onClearRoom.Invoke(room);
        }

        public static RoomCommonEvent onBeforeEnterRoom = new RoomCommonEvent();
        public static RoomCommonEvent onEnterRoom = new RoomCommonEvent();

        public static ExitRoomEvent onBeforeExitRoom = new ExitRoomEvent();
        public static ExitRoomEvent onExitRoom = new ExitRoomEvent();

        public static RoomCommonEvent onBeginBuildRoom = new RoomCommonEvent();
        public static RoomCommonEvent onBuildRoomComplete = new RoomCommonEvent();

        public static RoomCommonEvent onBeginDestroyRoom = new RoomCommonEvent();
        public static RoomCommonEvent onDestroyRoomComplete = new RoomCommonEvent();
        public static RoomCommonEvent onClearRoom = new RoomCommonEvent();

        public class RoomCommonEvent : CustomEvent<RoomInstance> { }

        public class ExitRoomEvent : CustomEvent<RoomInstance, ExitRoomType> { }
    }

    public static class HexRoomEvent
    {
        public static void FireOnSelectCell(HexRoom room, HexCell cell)
        {
            onSelectCell.Invoke(room, cell);
        }

        public static void FireOnUnselectCell(HexRoom room, HexCell cell)
        {
            onUnselectCell.Invoke(room, cell);
        }

        public static void FireOnLeftClickCell(HexRoom room, HexCell cell)
        {
            onLeftClickCell.Invoke(room, cell);
        }

        public static void FireOnRightClickCell(HexRoom room, HexCell cell)
        {
            onRightClickCell.Invoke(room, cell);
        }

        public static HexRoomNomarlEvent onSelectCell = new HexRoomNomarlEvent();
        public static HexRoomNomarlEvent onUnselectCell = new HexRoomNomarlEvent();
        public static HexRoomNomarlEvent onLeftClickCell = new HexRoomNomarlEvent();
        public static HexRoomNomarlEvent onRightClickCell = new HexRoomNomarlEvent();

        public class HexRoomNomarlEvent : CustomEvent<HexRoom, HexCell> { }
    }

    public static class EntityEvent
    {
        public static void FireOnBeforeEntityBorn(Entity entity)
        {
            onBeforeEntityBorn.Invoke(entity);
            onBeforeEntityNumChanged.Invoke(entity);
        }

        public static void FireOnBeforeEntityDead(Entity entity)
        {
            onBeforeEntityDead.Invoke(entity);
            onBeforeEntityNumChanged.Invoke(entity);
        }

        public static void FireOnEntityBorn(Entity entity)
        {
            onEntityBorn.Invoke(entity);
            onEntityNumChanged.Invoke(entity);
        }

        public static void FireOnEntityDead(Entity entity)
        {
            onEntityDead.Invoke(entity);
            onEntityNumChanged.Invoke(entity);
        }


        public static EntityChangedEvent onEntityNumChanged = new EntityChangedEvent();
        public static EntityChangedEvent onEntityBorn = new EntityChangedEvent();
        public static EntityChangedEvent onEntityDead = new EntityChangedEvent();

        public static EntityChangedEvent onBeforeEntityNumChanged = new EntityChangedEvent();
        public static EntityChangedEvent onBeforeEntityBorn = new EntityChangedEvent();
        public static EntityChangedEvent onBeforeEntityDead = new EntityChangedEvent();

        public class EntityChangedEvent : CustomEvent<Entity> { }
    }

    public static class PlayerEvent
    {
        public static void FireOnPlayerBorn(EAvatar avatar)
        {
            onPlayerBorn.Invoke(avatar);
        }

        public static PlayerNormalEvent onPlayerBorn = new PlayerNormalEvent();

        public class PlayerNormalEvent : CustomEvent<EAvatar> { }
    }

    public static class TurnBaseEvent
    {
        public static void FireOnCombatStart(TurnBase turnBase)
        {
            Debug.LogError("战斗开始");
            onCombatStart.Invoke(turnBase);
        }

        public static void FireOnCombatComplete(TurnBase turnBase, CombatResult result)
        {
            Debug.LogError("战斗结束");
            onCombatComplete.Invoke(turnBase, result);
        }

        public static void FireOnTurnStart(TurnBase turnBase)
        {
            Debug.LogError("回合开始 " + turnBase.turnCounter);
            onTurnStart.Invoke(turnBase);
        }

        public static void FireOnTurnComplete(TurnBase turnBase, TurnBaseRuningState result)
        {
            Debug.LogError("回合结束 " + turnBase.turnCounter);
            onTurnComplete.Invoke(turnBase, result);
        }

        public static void FireOnEntityTurnStart(CControllerComponent controller)
        {
            Debug.LogError(controller.entity.keyName + " 开始行动");
            onEntityTurnStart.Invoke(controller);
        }

        public static void FireOnEntityTurnComplete(CControllerComponent controller)
        {
            Debug.LogError(controller.entity.keyName + " 结束行动");
            onEntityTurnComplete.Invoke(controller);
        }

        public static NormalTurnBaseEvent onTurnStart = new NormalTurnBaseEvent();
        public static TurnBaseCompleteEvent onTurnComplete = new TurnBaseCompleteEvent();

        public static NormalTurnBaseEvent onCombatStart = new NormalTurnBaseEvent();
        public static CombatCompleteEvent onCombatComplete = new CombatCompleteEvent();

        public static EntityTurnBaseEvent onEntityTurnStart = new EntityTurnBaseEvent();
        public static EntityTurnBaseEvent onEntityTurnComplete = new EntityTurnBaseEvent();

        public class NormalTurnBaseEvent : CustomEvent<TurnBase> { }
        public class TurnBaseCompleteEvent : CustomEvent<TurnBase, TurnBaseRuningState> { }
        public class CombatCompleteEvent : CustomEvent<TurnBase, CombatResult> { }
        public class EntityTurnBaseEvent : CustomEvent<CControllerComponent> { }
    }
}