using UnityEngine;
using Untility;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 图标管理器，游戏内所有动态生成的图标都是由此创建
/// </summary>
class IconManager : Singleton<IconManager>, ISyncInitManager
{

    public IEnumerable<ManagerProgress> Initialize()
    {
        yield return new ManagerProgress(1f, "");
    }

    //所有的动态图标资源全部被打包成AssetsBundle资源放置在StreamingAssets路径下，所有的路径必须为小写
    static public string iconAssetBundles = "icon";                                            //所有的图标的资源路径
    static public string iconItemAssetBundles = iconAssetBundles + "/item";     //物品图标资源路径
    static public string iconMedicineAssetBundles = iconAssetBundles + "/medicine";     
    static public string iconBombAssetBundles = iconAssetBundles + "/bomb";
    static public string iconWeaponAssetBundles = iconAssetBundles + "/weapon";
    static public string iconRuneAssetBundles = iconAssetBundles + "/rune";     
    static public string iconBuffAssetBundles = iconAssetBundles + "/buff";     //物品图标资源路径
    static public string iconModifierAssetBundles = iconAssetBundles + "/modifier";     //物品图标资源路径
    static public string iconPlayerSkillAssetBundles = iconAssetBundles + "/playerskill";     //主动技能图标资源路径
    static public string iconCommonAssetBundles = iconAssetBundles + "/common";     //通用图标资源路径

    Sprite CreateSprite(string path, string name)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, path);
        AssetBundle ab = AssetBundle.LoadFromFile(fullPath);
        Texture2D t = ab.LoadAsset<Texture2D>(name);
        ab.Unload(false);
        if (t != null)
        {
            return Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError("加载图片 " + fullPath + "/" + name + "失败");
            return null;
        }
    }

    public Sprite CreateCommonSprite(int id)
    {
        return CreateSprite(iconCommonAssetBundles, id.ToString("0000"));
    }

    public Sprite CreateBombSprite(int id)
    {
        return CreateSprite(iconBombAssetBundles, id.ToString("0000"));
    }

    public Sprite CreateWeaponSprite(int id)
    {
        return CreateSprite(iconWeaponAssetBundles, id.ToString("0000"));
    }

    public Sprite CreateRuneSprite(int id)
    {
        return CreateSprite(iconRuneAssetBundles, id.ToString("0000"));
    }

    public Sprite CreateMedicineSprite(int id)
    {
        return CreateSprite(iconMedicineAssetBundles, id.ToString("0000"));
    }

    public Sprite CreateItemSprite(int id)
    {
        return CreateSprite(iconItemAssetBundles, id.ToString("0000"));
    }

    public Sprite CreateBuffSprite(int id)
    {
        return CreateSprite(iconBuffAssetBundles, id.ToString("0000"));
    }

    public Sprite CreateModifierSprite(int id)
    {
        return CreateSprite(iconModifierAssetBundles, id.ToString("0000"));
    }

    public Sprite CreatePlayerSkillSprite(int id)
    {
        return CreateSprite(iconPlayerSkillAssetBundles, id.ToString("0000"));
    }
}

