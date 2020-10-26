public class JAreaConfig
{
    public string name;
    //深度范围
    public JIntRange deep;

    //房间池 （如果需要实现从多个房间中选一个的话。可以在这里配置）
    public JRoomPoolConfig[] roomPoolConfigs;
    //单独概率出现的房间
    public JRandomRoomConfig[] roomConfigs;
    //根据类型来配置房间
    public JRandomRoomTypeConfig[] roomTypeConfigs;

    #region 房间内容配置
    public class JRoomPoolConfig
    {
        public JWeightRange<int> count;     //选出的房间数量，默认为1
        public JWeightRange<JRoomConfig> roomConfigs;    //房间的配置和权重
    }

    public class JRandomRoomConfig
    {
        public JWeightRange<int> count; //默认全为1
        public JRoomConfig roomConfig;
    }

    public class JRandomRoomTypeConfig
    {
        public RoomType roomType;
        public JWeightRange<int> count; //为空时默认为1
        public JWeightRange<string> contents;
    }

    public class JRoomConfig
    {
        public RoomType roomType;
        public string content;

        public static JRoomConfig CreateFromType(JRandomRoomTypeConfig randomTypeConfig, System.Random random)
        {
            JRoomConfig config = new JRoomConfig();
            config.roomType = randomTypeConfig.roomType;
            config.content = randomTypeConfig.contents.GetValue(random);
            return config;
        }
    }
    #endregion
}

