using UnityEditor;
using UnityEngine;

public class CreateEntityModelEditor : EditorWindow
{

    [MenuItem("创建/模型/自定义 Entity")]
    public static void Open()
    {
        Rect wr = new Rect(0, 0, 500, 500);
        CreateEntityModelEditor myWindow = (CreateEntityModelEditor)EditorWindow.GetWindowWithRect(typeof(CreateEntityModelEditor), wr, true, "自定义 Entity");
        myWindow.Init();
        myWindow.Show();
    }

    void Init()
    {
        m_Name = "EntityModel";
        m_CanSelected = true;
    }

    string m_Name;
    bool m_CanSelected;
    
    private void OnGUI()
    {
        m_Name = EditorGUILayout.TextField("名称", m_Name);
        m_CanSelected = EditorGUILayout.Toggle("是否可以选择", m_CanSelected);
        if (GUILayout.Button("创建", GUILayout.Width(200)) && Create())
        {
            //打开一个通知栏
            Close();
        }
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("关闭", GUILayout.Width(200)))
        {
            //关闭通知栏
            Close();
        }
    }

    bool Create()
    {
        GameObject sourceObj = new GameObject(m_Name);
        EntityModel m = sourceObj.GetComponent<EntityModel>();
        if (m == null)
        {
            m = sourceObj.AddComponent<EntityModel>();
        }
        EntityModel.SetEntityModelResource(m, m_CanSelected);
        return true;
    }
}
