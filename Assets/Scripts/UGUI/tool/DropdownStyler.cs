using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownStyler : MonoBehaviour
{
    [Header("字体设置")]
    public Color fontColor = Color.black;
    public int fontSize = 14;

    [Header("背景设置")]
    public Color backgroundColor = Color.white;

    private TMP_Dropdown dropdown;

    private void OnEnable()
    {
        ApplyChanges();
    }

    private void OnValidate()
    {
        ApplyChanges();
    }

    public void ApplyChanges()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();

        if (dropdown == null) return;

        // 1. 下拉列表选项文字
        if (dropdown.itemText != null)
        {
            dropdown.itemText.color = fontColor;
            dropdown.itemText.fontSize = fontSize;
        }

        // 2. 顶部选中项文字
        if (dropdown.captionText != null)
        {
            dropdown.captionText.color = fontColor;
            dropdown.captionText.fontSize = fontSize;
        }

        // 3. 选项背景
        if (dropdown.itemImage != null)
        {
            dropdown.itemImage.color = backgroundColor;
        }

#if UNITY_EDITOR
        // Play 模式下不能标记场景为 dirty，否则报错
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }
}
