#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 一键在"引导遮罩不规则图形"场景里搭建 demo：
/// 菜单 Tools → 搭建引导遮罩不规则图形Demo
/// 会在当前场景创建一个空物体并挂上 IrregularGuideMask，运行即可看到效果。
/// </summary>
public static class IrregularGuideMaskMenu
{
    [MenuItem("Tools/搭建引导遮罩不规则图形Demo")]
    public static void BuildDemo()
    {
        var go = new GameObject("IrregularGuideMask");
        go.AddComponent<IrregularGuideMask>();
        Selection.activeGameObject = go;
        Undo.RegisterCreatedObjectUndo(go, "Build Irregular Guide Mask Demo");
        Debug.Log("已在场景创建 IrregularGuideMask。点击 Play 运行即可看到五角星引导遮罩。");
    }
}
#endif
