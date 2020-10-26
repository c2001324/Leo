using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Untility;
using System;
using System.Text;

/// <summary>
/// 游戏作弊码
/// </summary>
public class CheatCodeManager : Singleton<CheatCodeManager>, ISyncInitManager
{
    #region 作弊码

    //系统相关
    public static string help = "help";
    public static string clear = "clear";
    public static string clearLog = "clearlog";
    //显示信息
    public static string player = "player";
    public static string roomInfo = "roominfo";
    public static string areaInfo = "areainfo";
    //作弊
    public static string room = "room";
    public static string area = "area";
    public static string level = "level";
    public static string weapon = "weapon";
    public static string pickWeapon = "pickweapon";
    public static string rune = "rune";
    public static string pickrune = "pickrune";
    public static string bomb = "bomb";
    public static string damage = "damage";
    public static string gold = "gold";
    public static string restoreHp = "restorehp";
    public static string skill = "skill";
    public static string ai = "ai";
    public static string showPlayer = "showplayer";
    public static string transparent = "transparent";
    public static string ability = "ability";

    public static string testWeapon = "testweapon";
    public static string testRune = "testrune";
    public static string spawn = "spawn";
    public static string spawnPlayer = "spawnplayer";
    public static string attach = "attach";
    #endregion


    Dictionary<GameSceneType, List<string>> m_CheatCodeCMDDict = new Dictionary<GameSceneType, List<string>>();

    public IEnumerable<ManagerProgress> Initialize()
    {
        yield return new ManagerProgress(0f, "");
        #region 系统相关
        m_CheatCodeMap.Add(help, new CheatCodeData(Help, "帮助", GameSceneType.Combat, GameSceneType.PreCombat, GameSceneType.Title, GameSceneType.Practice));
        m_CheatCodeMap.Add(clear, new CheatCodeData(Clear, "清空", GameSceneType.Combat, GameSceneType.PreCombat, GameSceneType.Title, GameSceneType.Practice));
        m_CheatCodeMap.Add(clearLog, new CheatCodeData(ClearLog, "清空日志", GameSceneType.Combat, GameSceneType.PreCombat, GameSceneType.Title, GameSceneType.Practice));
        #endregion
        //加载初始的配置文件
#if UNITY_EDITOR
        GameEvent.Scene.onEnterSceneComplete.AddListener(OnEnterGameScene);
        LoadCheatCodeConfig();
        yield return new ManagerProgress(1f, "");
#endif
    }


    #region 加载作弊文件

    void OnEnterGameScene(GameSceneType type)
    {
        TimerManager.instance.RemoveTimer(this);
        TimerManager.instance.AddDelayTimer(LoadCheatCodeDelayByGameStateChanged, null, 0.5f, this);
    }

    void LoadCheatCodeDelayByGameStateChanged(uint id, object param)
    {
        bool closeWnd;
        List<string> cmdList = GetCMDListByGameState(GameSceneManager.instance.curGameScene);

        if (cmdList == null || GameSceneManager.instance.curGameScene == GameSceneManager.instance.oldGameScene)
        {
            return;
        }

        foreach (string str in cmdList)
        {
            ExcuteCmd(str, out closeWnd);
        }
    }

    List<string> GetCMDListByGameState(GameSceneType type)
    {
        List<string> list = null;
        if (!m_CheatCodeCMDDict.TryGetValue(type, out list))
        {
            return null;
        }
        return list;
    }

    void LoadCheatCodeConfig()
    {
        TextAsset obj = Resources.Load<TextAsset>("config/CheatCode");
        if (obj != null)
        {
            MemoryStream stream = new MemoryStream(obj.bytes);
            StreamReader streamR = new StreamReader(stream, System.Text.Encoding.UTF8);
            string str = streamR.ReadLine();
            List<string> curCodeList = null;
            
            while (str != null)
            {
                if (str != "" && str != null && str.IndexOf('#') != 0)
                {
                    if (str.IndexOf('[') == 0)
                    {
                        //标题
                        string gameStateStr = str.Remove(str.Length - 1);
                        gameStateStr = gameStateStr.Remove(0, 1);
                        GameSceneType newGameState = (GameSceneType)Enum.Parse(typeof(GameSceneType), gameStateStr);
                        curCodeList = GetCMDListByGameState(newGameState);
                        if (curCodeList == null)
                        {
                            curCodeList = new List<string>();
                            m_CheatCodeCMDDict.Add(newGameState, curCodeList);
                        }
                    }
                    else
                    {
                        //普通内容
                        if (curCodeList != null)
                        {
                            curCodeList.Add(str);
                        }
                    }
                }
                str = streamR.ReadLine();
            }
            streamR.Close();
            stream.Close();
        }
    }
    #endregion

    class CheatCodeData
    {
        public CheatCodeData(CheatCodeDelegate handle, string describe, params GameSceneType[] scenes)
        {
            m_GameScenes.AddRange(scenes);
            this.m_Handle = handle;
            this.describe = describe;
        }
        List<GameSceneType> m_GameScenes = new List<GameSceneType>();
        readonly public string describe;
        CheatCodeDelegate m_Handle;

        bool CheckGameScene(GameSceneType scenes)
        {
            return m_GameScenes.Contains(scenes);
        }

        public bool Excute(List<string> param, out bool closeWnd)
        {
            if (CheckGameScene(GameSceneManager.instance.curGameScene))
            {
                return m_Handle(param, out closeWnd);
            }
            else
            {
                closeWnd = false;
                Debug.LogError("该命令不可以在游戏状态 " + GameSceneManager.instance.curGameScene + " 下执行");
                return false;
            }
        }
    }

    delegate bool CheatCodeDelegate(List<string> param, out bool closeWnd);
    /// <summary>
    /// 作弊码表
    /// </summary>
    Dictionary<string, CheatCodeData> m_CheatCodeMap = new Dictionary<string, CheatCodeData>();

    

    public bool ExcuteCmd(string param, out bool closeWnd)
    {
        closeWnd = false;
        if (param == "")
        {
            return false;
        }

        //解析参数
        List<string> strList = ParseCmd(param);
        if (strList.Count <= 0)
        {
            return false;
        }

        string cheatCode = strList[0];
        strList.RemoveAt(0);
        return Excute(cheatCode, strList, out closeWnd);
    }


    /// <summary>
    /// 解析参数
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    List<string> ParseCmd(string param)
    {
        List<string> strList = new List<string>();
        foreach (string str in param.Split(' '))
        {
            if (str != " " && str != "")
            {
                strList.Add(str.Replace(" ", "").ToLower());
            }
        }
        return strList;
    }

    bool Excute(string code, List<string> param, out bool closeWnd)
    {
        closeWnd = false;
        CheatCodeData data = null;
        if (!m_CheatCodeMap.TryGetValue(code, out data))
        {
            return false;
        }
        return data.Excute(param, out closeWnd);
    }

    #region 执行
    bool EnterRoom(List<string> param, out bool closeWnd)
    {
        closeWnd = true;
        RoomType roomType = RoomType.Empty;

        if (param[0] == "boss")
        {
            roomType = RoomType.BossRoom;
        }
        else if (param[0] == "store")
        {
            roomType = RoomType.StoreRoom;
        }
        else if (param[0] == "start")
        {
            roomType = RoomType.StartRoom;
        }
        else if (param[0] == "box")
        {
            roomType = RoomType.TreasureRoom;
        }
        else if (param[0] == "runes")
        {
            roomType = RoomType.WeaponBuilderRoom;
        }
        else if (param[0] == "support")
        {
            roomType = RoomType.SupportRoom;
        }
        else if (param[0] == "back")
        {
            roomType = RoomType.BackRoom;
        }

        RoomData roomData = null;

        roomData = LevelManager.area.GetRoomData(roomType);

        if (roomData == null)
        {
            int roomId = -1;
            if (int.TryParse(param[0], out roomId))
            {
                roomData = LevelManager.area.GetRoomData(roomId);
            }
        }
        
        if (roomData == null)
        {
            return false;
        }
        //进入房间
        return true;
    }

    bool EnterArea(List<string> param, out bool closeWnd)
    {
        closeWnd = true;
        int areaIndex = 0;
        if (int.TryParse(param[0], out areaIndex))
        {
            AreaData nextAreaData = LevelManager.level.GetAreaData(areaIndex);
            if (nextAreaData != null)
            {
            }
        }
        
        return true;
    }

    bool EnterLevel(List<string> param, out bool closeWnd)
    {
        closeWnd = true;
        int levelIndex = 0;
        if (int.TryParse(param[0], out levelIndex))
        {
        }

        return true;
    }

    bool Help(List<string> param, out bool closeWnd)
    {
        //不关闭窗口
        closeWnd = false;
        StringBuilder builder = new StringBuilder();
        foreach (string str in m_CheatCodeMap.Keys)
        {
            builder.AppendFormat("{0} : {1}\n", str, m_CheatCodeMap[str].describe);
        }

        UIManager.instance.GetUI<UIConsoleWnd>().ShowMsg(builder.ToString());
        return true;
    }

    bool Clear(List<string> param, out bool closeWnd)
    {
        //不关闭窗口
        closeWnd = false;
        StringBuilder builder = new StringBuilder();
        foreach (string str in m_CheatCodeMap.Keys)
        {
            builder.AppendFormat("{0} : {1}\n", str, m_CheatCodeMap[str].describe);
        }

        UIManager.instance.GetUI<UIConsoleWnd>().ResetMsg();
        return true;
    }

    bool ClearLog(List<string> param, out bool closeWnd)
    {
        closeWnd = true;
        Untility.Log.instance.ClearLog();
        return true;
    }

   
    #endregion

}
