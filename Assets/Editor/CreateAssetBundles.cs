using System.IO;
using UnityEditor;
using UnityEngine;
using Untility;
using UnityEngine.U2D;

public class CreateAssetBundles
{
    /// <summary>
    /// 打包资源需要 屏蔽 InControlUtility My_HKLM_GetString
    /// </summary>
    [MenuItem("打包资源/打包到Windows平台")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
    }

    [MenuItem("打包资源/打包到安卓平台")]
    static void BuildAllAssetBundlesAndroid()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }

    [MenuItem("打包资源/打包到IOS平台")]
    static void BuildAllAssetBundlesIOS()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
    }
}