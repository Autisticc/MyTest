using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

/// <summary>
/// 红点树视图窗口V2
/// </summary>
public class ReddotTreeViewWindowV2 : EditorWindow
{
    private static ReddotTreeViewWindowV2 s_Window;

    private ReddotTreeViewV2 m_TreeView;

    private SearchField m_SearchField;

    [MenuItem("Window/红点树视图窗口V2")]
    private static void OpenWindow()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("警告", "运行后才能打开红点树视图窗口V2", "了解");
            return;
        }

        s_Window = GetWindow<ReddotTreeViewWindowV2>();
        s_Window.titleContent = new GUIContent("红点树视图窗口V2");
        s_Window.Show();
    }


    private void OnEnable()
    {
        m_TreeView = new ReddotTreeViewV2(new TreeViewState());

        m_SearchField = new SearchField();
        m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    private void OnPlayModeStateChange(PlayModeStateChange state)
    {
        // 退出播放模式时自动关闭窗口（运行时对象即将销毁）
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            s_Window?.Close();
        }
    }

    private void OnDestroy()
    {
        m_TreeView.OnDestroy();
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
    }

    private void OnGUI()
    {
        UpToolbar();

        TreeView();
       
        BottomToolBar();
    }

    private void UpToolbar()
    {
        m_TreeView.searchString = m_SearchField.OnGUI(new Rect(0, 0, position.width - 40f, 20f), m_TreeView.searchString);
    }

    private void TreeView()
    {
        m_TreeView.OnGUI(new Rect(0, 20f, position.width, position.height - 40f));
    }

    private void BottomToolBar()
    {
        GUILayout.BeginArea(new Rect(20f, position.height - 18f, position.width - 40f, 16f));

        using (new EditorGUILayout.HorizontalScope())
        {

            string style = "miniButton";
            if (GUILayout.Button("展开全部节点", style))
            {
                m_TreeView.ExpandAll();
            }

            if (GUILayout.Button("收起全部节点", style))
            {
                m_TreeView.CollapseAll();
            }

           
        }

        GUILayout.EndArea();
    }
}
