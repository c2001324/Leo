public class RoomData
{
    public static RoomData Create(int index, int deep, JAreaConfig.JRoomConfig config)
    {
        RoomData data = new RoomData();
        data.Initialize(index, deep, config);
        return data;
    }

    RoomData() { }

    public int index { get; private set; }

    public RoomType roomType { get; private set; }

    public string contentName { get; private set; }

    public int deep { get; private set; }
    
    void Initialize(int index, int deep, JAreaConfig.JRoomConfig config)
    {
        this.index = index;
        this.deep = deep;
        roomType = config.roomType;
        contentName = config.content;
    }

    public void SetParents(params RoomData[] rooms)
    {

    }

    public void SetChild(params RoomData[] child)
    {

    }
}