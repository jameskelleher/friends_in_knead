#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class SequenceLibraryWindow : EditorWindow
{
    Vector2 _scrollPos;
    SerializedObject _serializedObject;
    SerializedProperty _sequencesProp;

    public static void Open(SequenceLibrary library)
    {
        var window = GetWindow<SequenceLibraryWindow>("Sequence Library");
        window._serializedObject = new SerializedObject(library);
        window._sequencesProp = window._serializedObject.FindProperty("sequences");
        window.Show();
    }

    void OnGUI()
    {
        if (_serializedObject == null)
        {
            EditorGUILayout.LabelField("No library selected.");
            return;
        }

        _serializedObject.Update();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        for (int i = 0; i < _sequencesProp.arraySize; i++)
        {
            var seqProp = _sequencesProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(seqProp);
            EditorGUILayout.Space(4);
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+"))
            _sequencesProp.arraySize++;
        if (GUILayout.Button("-") && _sequencesProp.arraySize > 0)
            _sequencesProp.arraySize--;
        EditorGUILayout.EndHorizontal();

        _serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SequenceLibrary))]
public class SequenceLibraryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Sequence Editor"))
            SequenceLibraryWindow.Open((SequenceLibrary)target);
        DrawDefaultInspector();
        EditorGUILayout.Space(4);
    }
}
#endif