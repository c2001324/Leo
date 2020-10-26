using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Untility;

public class AnimationCurveEditor : EditorWindow
{
    [MenuItem("创建/动画曲线")]
    public static void Open()
    {
        Rect wr = new Rect(0, 0, 500, 500);
        AnimationCurveEditor myWindow = (AnimationCurveEditor)EditorWindow.GetWindowWithRect(typeof(AnimationCurveEditor), wr, true, "AnimationCurve Editor");
        myWindow.Init();
        myWindow.Show();
    }

    private void OnDisable()
    {
        m_AllFiles = null;
        m_CurFileInfo = null;
        m_Curve = null;
    }

    void Init()
    {
        UpdateAllFiles();
        if (m_AllFiles.Length > 0)
        {
            SelectFile(m_AllFiles[0]);
        }
    }

    private void OnEnable()
    {
        UpdateAllFiles();
    }

    FileInfo m_CurFileInfo;
    string m_Name;
    AnimationCurve m_Curve;
    FileInfo[] m_AllFiles;

    public string basePath { get { return Path.Combine(Application.dataPath, "AnimationCurve"); } }

    #region 数据
    
    #endregion

    private void OnGUI()
    {
        if (m_AllFiles == null)
        {
            UpdateAllFiles();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(150));
        foreach (FileInfo  file in m_AllFiles)
        {
            if (GUILayout.Button(file.Name))
            {
                SelectFile(file);
            }
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(20);
        EditorGUILayout.BeginVertical();
        m_Name = EditorGUILayout.TextField("名称", m_Name);
        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(m_Curve == null);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(50);
        m_Curve = EditorGUILayout.CurveField(m_Curve, GUILayout.Width(200), GUILayout.Height(200));
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(m_CurFileInfo != null);
        if (GUILayout.Button("创建", GUILayout.Width(100)))
        {
            m_Curve = new AnimationCurve();
            Save(m_Name);
            UpdateAllFiles();
            SelectFile(GetFileInfo(m_Name));
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(m_CurFileInfo == null || m_Curve == null);
        if (GUILayout.Button("保存", GUILayout.Width(100)))
        {
            Save(m_Name);
            if (m_Name != m_CurFileInfo.Name)
            {
                //保存新的
                UpdateAllFiles();
                SelectFile(GetFileInfo(m_Name));
            }
        }
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("关闭", GUILayout.Width(100)))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(m_AllFiles == null || m_AllFiles.Length == 0);
        if (GUILayout.Button("打包资源", GUILayout.Width(100)))
        {
            BuildAsset();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    void Save(string fileName)
    {
        if (m_Curve != null && m_Name != "")
        {
            JAnimationCurveData data = new JAnimationCurveData(m_Name, m_Curve);
            LoadJsonObject.WriteJsonFromObject(data, Path.Combine(basePath, fileName));
        }
    }

    void SelectFile(FileInfo file)
    {
        if (file != m_CurFileInfo)
        {
            m_CurFileInfo = file;
            m_Name = file.Name;
            m_CurFileInfo = file;
            m_Curve = LoadAcByPath(file);
        }
    }

    AnimationCurve LoadAcByPath(FileInfo file)
    {
        JAnimationCurveData data = LoadJsonObject.CreateObjectFromStreamingAssets<JAnimationCurveData>(file.FullName);
        return new AnimationCurve(data.keyDatas);
    }

    void UpdateAllFiles()
    {
        if (Directory.Exists(basePath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(basePath);
            m_AllFiles = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
        }
    }

    FileInfo GetFileInfo(string name)
    {
        foreach (FileInfo file in m_AllFiles)
        {
            if (name == file.Name)
            {
                return file;
            }
        }
        return null;
    }

    void BuildAsset()
    {
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        //打包出来的资源包名字
        buildMap[0].assetBundleName = "AnimationCurve";

        string[] enemyAsset = new string[m_AllFiles.Length];
        for (int i = 0; i < enemyAsset.Length; i++)
        {
            //获得选择 对象的路径
            enemyAsset[i] = "Assets/AnimationCurve/" + m_AllFiles[i].Name.ToLower();
        }
        buildMap[0].assetNames = enemyAsset;

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, buildMap, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
        Debug.Log("打包完成");
    }

}