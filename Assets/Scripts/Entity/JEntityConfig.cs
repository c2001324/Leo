using System;

public class JEntityConfig
{
    public string keyName;//由EntityManager初始化后再赋值，不用配置
    public string type;     //类型
    public string name; //显示的名称
    public int icon;                    //
    public string entityModel;  //模型的路径
    public float modelScale;    //模型的缩放
    public int size = 1;            //占用cell的大小
    public string describe;


    public virtual Type entityType
    {
        get
        {
            return Type.GetType(type);
        }
    }
}