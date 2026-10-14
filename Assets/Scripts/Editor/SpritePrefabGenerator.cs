#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.U2D;

/// <summary>
/// Prefab 组件类型
/// </summary>
public enum SpritePrefabType
{
    /// <summary>UGUI Image</summary>
    Image,
    /// <summary>Sprite Renderer（世界空间/2D）</summary>
    SpriteRenderer
}

/// <summary>
/// 从 Sprite Atlas 批量生成 Sprite Prefab
/// 遍历图集的 packables（原始资源），为每个 Sprite 生成 Prefab
/// </summary>
public class SpritePrefabGenerator : EditorWindow
{
    [Header("配置")]
    public SpriteAtlas sourceAtlas;
    public SpritePrefabType prefabType = SpritePrefabType.Image;
    public string outputFolder = "Scripts/UGUI/简单的背包系统/Resources/Sprite";
    public string prefabNamePrefix = "";
    public string prefabNameSuffix = "";
    public bool setNativeSize = true;
    public bool raycastTarget = false;

    private Vector2 scrollPos;
    private List<Sprite> spriteList = new List<Sprite>();
    private string[] spriteNames;

    [MenuItem("Tools/Sprite Prefab Generator")]
    public static void ShowWindow()
    {
        var window = GetWindow<SpritePrefabGenerator>("Sprite Prefab 生成器");
        window.minSize = new Vector2(450, 550);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Atlas 批量生成 Prefab", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sourceAtlas = (SpriteAtlas)EditorGUILayout.ObjectField(
            "源图集", sourceAtlas, typeof(SpriteAtlas), false);

        prefabType = (SpritePrefabType)EditorGUILayout.EnumPopup("Prefab 类型", prefabType);

        outputFolder = EditorGUILayout.TextField("输出目录", outputFolder);
        prefabNamePrefix = EditorGUILayout.TextField("文件名前缀", prefabNamePrefix);
        prefabNameSuffix = EditorGUILayout.TextField("文件名后缀", prefabNameSuffix);

        EditorGUILayout.Space();

        if (prefabType == SpritePrefabType.Image)
        {
            raycastTarget = EditorGUILayout.Toggle("开启射线检测", raycastTarget);
        }
        setNativeSize = EditorGUILayout.Toggle("自动适应尺寸", setNativeSize);

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(sourceAtlas == null);

        if (GUILayout.Button("扫描图集", GUILayout.Height(25)))
        {
            ScanAtlas();
        }

        if (GUILayout.Button("生成所有 Prefab", GUILayout.Height(40)))
        {
            GenerateAllPrefabs();
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        if (spriteNames != null && spriteNames.Length > 0)
        {
            GUILayout.Label($"共 {spriteNames.Length} 个 Sprite：", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var name in spriteNames)
            {
                EditorGUILayout.LabelField("• " + name);
            }
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// 从图集的 packables 里获取所有原始 Sprite
    /// </summary>
    private void ScanAtlas()
    {
        spriteList.Clear();
        spriteNames = null;

        if (sourceAtlas == null) return;

        // 获取图集里的所有 packables（原始资源）
        var packables = sourceAtlas.GetPackables();

        foreach (var packable in packables)
        {
            string assetPath = AssetDatabase.GetAssetPath(packable);

            // 如果是文件夹，递归找里面所有 Sprite
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { assetPath });
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (sprite != null && !spriteList.Contains(sprite))
                    {
                        spriteList.Add(sprite);
                    }
                }
            }
            // 如果是单个纹理或 sprite，直接加载
            else
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite != null && !spriteList.Contains(sprite))
                {
                    spriteList.Add(sprite);
                }
            }
        }

        spriteNames = new string[spriteList.Count];
        for (int i = 0; i < spriteList.Count; i++)
        {
            spriteNames[i] = spriteList[i].name;
        }

        Debug.Log($"从图集扫描到 {spriteList.Count} 个 Sprite");
    }

    /// <summary>
    /// 生成所有 Prefab
    /// </summary>
    private void GenerateAllPrefabs()
    {
        if (sourceAtlas == null)
        {
            EditorUtility.DisplayDialog("错误", "请先拖入 Sprite Atlas", "确定");
            return;
        }

        ScanAtlas();

        if (spriteList.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "图集里没有扫描到 Sprite", "确定");
            return;
        }

        // 确保输出目录存在
        string fullOutputPath = Path.Combine("Assets", outputFolder);
        if (!AssetDatabase.IsValidFolder(fullOutputPath))
        {
            string[] folders = outputFolder.Split('/');
            string current = "Assets";
            foreach (var folder in folders)
            {
                string next = Path.Combine(current, folder);
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, folder);
                }
                current = next;
            }
        }

        int successCount = 0;
        for (int i = 0; i < spriteList.Count; i++)
        {
            var sprite = spriteList[i];
            if (sprite == null) continue;

            string prefabName = $"{prefabNamePrefix}{sprite.name}{prefabNameSuffix}";
            string prefabPath = Path.Combine(fullOutputPath, prefabName + ".prefab");

            // 创建临时 GameObject
            var go = new GameObject(prefabName);

            if (prefabType == SpritePrefabType.Image)
            {
                // UGUI Image 模式
                var image = go.AddComponent<Image>();
                image.sprite = sprite;
                image.type = Image.Type.Simple;
                image.raycastTarget = raycastTarget;

                if (setNativeSize)
                {
                    image.SetNativeSize();
                }
            }
            else
            {
                // Sprite Renderer 模式
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = 0;

                if (setNativeSize)
                {
                    // Sprite Renderer 的尺寸由 sprite 本身决定，无需额外设置
                }
            }

            // 保存为 Prefab
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

            // 删除临时物体
            DestroyImmediate(go);

            successCount++;
            EditorUtility.DisplayProgressBar("生成 Prefab", $"正在生成 {prefabName}", (float)i / spriteList.Count);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        Debug.Log($"完成！共生成 {successCount} 个 Prefab，路径：{fullOutputPath}");
        EditorUtility.DisplayDialog("完成", $"成功生成 {successCount} 个 Sprite Prefab！", "确定");
    }
}
#endif
