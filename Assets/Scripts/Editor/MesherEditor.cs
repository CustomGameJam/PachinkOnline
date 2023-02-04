using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Mesher))]
// ReSharper disable once CheckNamespace
public class MesherEditor : Editor
{
    private SerializedProperty _current;
    private SerializedProperty _data;

    private SerializedProperty data => _data.GetArrayElementAtIndex(_current.intValue);

    private void OnEnable()
    {
        _current = serializedObject.FindProperty("current");
        _data = serializedObject.FindProperty("data");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.IntSlider(_current, 0, _data.arraySize - 1);

        EditorGUILayout.LabelField("Element " + _current.intValue);

        var type = data.FindPropertyRelative("type");
        var scale = data.FindPropertyRelative("scale");
        var offset = data.FindPropertyRelative("offset");

        EditorGUILayout.PropertyField(type);
        switch ((MeshType)type.enumValueIndex)
        {
            case MeshType.Cube:
                var response = EditorGUILayout.Vector2IntField(
                    "Scale",
                    new Vector2Int(scale.vector3IntValue.x, scale.vector3IntValue.y)
                );
                scale.vector3IntValue = (Vector3Int)response;
                break;
            case MeshType.Ramp:
                EditorGUILayout.PropertyField(scale);
                break;
        }

        EditorGUILayout.PropertyField(offset);

        RenderButtons();

        if (serializedObject.ApplyModifiedProperties())
        {
            MeshUtil.Render(_data);
        }
    }

    private void RenderButtons()
    {
        RenderCreateRemoveButtons();
    }

    private void RenderCreateRemoveButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Remove") &&
            EditorUtility.DisplayDialog("Remove Element " + _current.intValue, "Are you sure?", "Yes", "No"))
        {
            _data.DeleteArrayElementAtIndex(_current.intValue);
            _current.intValue--;
        }

        if (GUILayout.Button("Create New") &&
            EditorUtility.DisplayDialog("Create New Object", "Are you sure?", "Yes", "No"))
        {
            _data.InsertArrayElementAtIndex(_current.intValue);
            _current.intValue++;
        }

        GUILayout.EndHorizontal();
    }
}