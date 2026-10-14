using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(RoundedImage), true)]
[CanEditMultipleObjects]
public class RoundedImageEditor : ImageEditor
{
    SerializedProperty radiusProp;
    SerializedProperty uniformRadiusProp;
    SerializedProperty cornerRadiusProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        radiusProp = serializedObject.FindProperty("radius");
        uniformRadiusProp = serializedObject.FindProperty("uniformRadius");
        cornerRadiusProp = serializedObject.FindProperty("cornerRadius");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("圆角设置", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(radiusProp, new GUIContent("圆角半径"));
        EditorGUILayout.PropertyField(uniformRadiusProp, new GUIContent("四角统一"));

        if (!uniformRadiusProp.boolValue)
        {
            EditorGUILayout.PropertyField(cornerRadiusProp, new GUIContent("四角半径(右上,右下,左上,左下)"));
        }
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
