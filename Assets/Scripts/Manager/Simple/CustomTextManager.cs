using System.Collections.Generic;
using Untility;

/// <summary>
/// 自定义文本管理器
/// </summary>
public class CustomTextManager : Singleton<CustomTextManager>, ISimpleInitManager
{
    public void Initialize()
    {
        m_Texts = LoadJsonObject.CreateObjectFromResource<JCustomTextConfigs>("Config/CustomTextConfigs");
    }

    JCustomTextConfigs m_Texts;

    public string GetCustomText(CustomTextKey key)
    {
        string str = null;
        m_Texts.texts.TryGetValue(key, out str);
        return str;
    }
}

public class JCustomTextConfigs
{
    public Dictionary<CustomTextKey, string> texts;
}

public enum CustomTextKey
{
    WeaponPropertyDamage,
    Others,
}