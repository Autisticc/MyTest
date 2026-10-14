#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TMPFontReplacer : EditorWindow
{
    public TMP_FontAsset newFont;

    [MenuItem("Tools/TMP 批量替换字体")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontReplacer>("TMP 批量替换字体");
    }

    void OnGUI()
    {
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("新字体", newFont, typeof(TMP_FontAsset), false);

        EditorGUILayout.Space();
        if (GUILayout.Button("替换当前场景所有 TMP"))
        {
            ReplaceInScene();
        }

        if (GUILayout.Button("替换所有 Prefab 里的 TMP"))
        {
            ReplaceInPrefabs();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("替换范围：\n• 所有 TMP_Text 组件\n• TMP_InputField 的输入文本和占位符\n• TMP_Dropdown 里的选项文本\n• 包括隐藏/未激活的物体", MessageType.Info);
    }

    void ReplaceTMPInGameObject(GameObject go)
    {
        // 找所有 TMP_Text（包括 inactive 子物体）
        var tmpList = go.GetComponentsInChildren<TMP_Text>(true);
        foreach (var tmp in tmpList)
        {
            Undo.RecordObject(tmp, "Replace TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
        }
    }

    void ReplaceInScene()
    {
        // 找场景里所有根物体，然后遍历
        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        int count = 0;
        foreach (var root in rootObjects)
        {
            var tmpList = root.GetComponentsInChildren<TMP_Text>(true);
            foreach (var tmp in tmpList)
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = newFont;
                EditorUtility.SetDirty(tmp);
                count++;
            }
        }
        Debug.Log($"场景中替换了 {count} 个 TMP 组件");
    }

    void ReplaceInPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            var tmpList = prefab.GetComponentsInChildren<TMP_Text>(true);
            if (tmpList.Length == 0) continue;

            foreach (var tmp in tmpList)
            {
                tmp.font = newFont;
                EditorUtility.SetDirty(tmp);
                count++;
            }
            PrefabUtility.SavePrefabAsset(prefab);
        }
        Debug.Log($"Prefab 中替换了 {count} 个 TMP 组件");
    }
}
#endif
