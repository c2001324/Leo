public static class GameEvent
{
    public static class Scene
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

    public static class Level
    {
        public static void FireOnBuildLevel(LevelInstance level)
        {
            onBuildLevel.Invoke(level);
        }

        public static void FireOnBuildLevelComplete(LevelInstance level)
        {
            onBuildLevelComplete.Invoke(level);
        }

        public static void FireOnDestroyLevel(LevelInstance level, DestroyLevelType type)
        {
            onDestroyLevel.Invoke(level, type);
        }

        public static void FireOnDestroyLevelComplete(LevelInstance level, DestroyLevelType type)
        {
            onDestroyLevelComplete.Invoke(level, type);
        }

        public static void FireOnBeforeEnterLevel(LevelInstance level)
        {
            onBeforeEnterLevel.Invoke(level);
        }

        public static void FireOnEnterLevel(LevelInstance level)
        {
            onEnterLevel.Invoke(level);
        }

        public static void FireOnBeforeExitLevel(LevelInstance level, DestroyLevelType type)
        {
            onBeforeExitLevel.Invoke(level, type);
        }

        public static void FireOnExitLevel(LevelInstance level, DestroyLevelType type)
        {
            onExitLevel.Invoke(level, type);
        }

        public static LevelEvent onBuildLevel = new LevelEvent();
        public static LevelEvent onBuildLevelComplete = new LevelEvent();

        public static LevelEventEx onDestroyLevel = new LevelEventEx();
        public static LevelEventEx onDestroyLevelComplete = new LevelEventEx();

        public static LevelEvent onBeforeEnterLevel = new LevelEvent();
        public static LevelEvent onEnterLevel = new LevelEvent();

        public static LevelEventEx onBeforeExitLevel = new LevelEventEx();
        public static LevelEventEx onExitLevel = new LevelEventEx();

        public class LevelEvent : CustomEvent<LevelInstance> { }
        public class LevelEventEx : CustomEvent<LevelInstance, DestroyLevelType> { }
    }

    public static class Area
    {
        public static void FireOnBuildArea(LevelInstance level, AreaInstance area)
        {
            onBuildArea.Invoke(level, area);
        }

        public static void FireOnBuildAreaComplete(LevelInstance level, AreaInstance area)
        {
            onBuildAreaComplete.Invoke(level, area);
        }

        public static void FireOnDestroyArea(LevelInstance level, AreaInstance area, DestroyLevelType type)
        {
            onDestroyArea.Invoke(level, area, type);
        }

        public static void FireOnDestroyAreaComplete(LevelInstance level, AreaInstance area, DestroyLevelType type)
        {
            onDestroyAreaComplete.Invoke(level, area, type);
        }

        public static void FireOnBeforeEnterArea(LevelInstance level, AreaInstance area)
        {
            onBeforeEnterArea.Invoke(level, area);
        }

        public static void FireOnEnterArea(LevelInstance level, AreaInstance area)
        {
            onEnterArea.Invoke(level, area);
        }

        public static void FireOnBeforeExitArea(LevelInstance level, AreaInstance area, DestroyLevelType type)
        {
            onBeforeExitArea.Invoke(level, area, type);
        }

        public static void FireOnExitArea(LevelInstance level, AreaInstance area, DestroyLevelType type)
        {
            onExitArea.Invoke(level, area, type);
        }

        public static AreaEvent onBuildArea = new AreaEvent();
        public static AreaEvent onBuildAreaComplete = new AreaEvent();

        public static AreaEvent onBeforeEnterArea = new AreaEvent();
        public static AreaEvent onEnterArea = new AreaEvent();

        public static AreaEventEx onDestroyArea = new AreaEventEx();
        public static AreaEventEx onDestroyAreaComplete = new AreaEventEx();

        public static AreaEventEx onBeforeExitArea = new AreaEventEx();
        public static AreaEventEx onExitArea = new AreaEventEx();

        public class AreaEvent : CustomEvent<LevelInstance, AreaInstance> { }
        public class AreaEventEx : CustomEvent<LevelInstance, AreaInstance, DestroyLevelType> { }
    }

    public static class Room
    {
        public static void FireOnBeforeEnterRoom(RoomInstance room)
        {
            onBeforeEnterRoom.Invoke(room);
        }

        public static void FireOnEnterRoom(RoomInstance room)
        {
            onEnterRoom.Invoke(room);
        }

        public static void FireOnBeforeExitRoom(RoomInstance room)
        {
            onBeforeExitRoom.Invoke(room);
        }

        public static void FireOnExitRoom(RoomInstance room)
        {
            onExitRoom.Invoke(room);
        }

        public static void FireOnCompleteRoom(RoomInstance room)
        {
            onClearRoom.Invoke(room);
        }


        public static RoomEvent onBeforeEnterRoom = new RoomEvent();
        public static RoomEvent onEnterRoom = new RoomEvent();

        public static RoomEvent onBeforeExitRoom = new RoomEvent();
        public static RoomEvent onExitRoom = new RoomEvent();
        public static RoomEvent onClearRoom = new RoomEvent();

        public class RoomEvent : CustomEvent<RoomInstance> { }
    }

}