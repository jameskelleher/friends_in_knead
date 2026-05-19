#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InputStep))]
public class InputStepDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var strengthProp = property.FindPropertyRelative("strength");
        var positionProp = property.FindPropertyRelative("position");
        var soundProp = property.FindPropertyRelative("sound");

        bool isRest = strengthProp.enumValueIndex == (int)InputStrength.Rest;

        float lineH = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect row = new Rect(position.x, position.y, position.width, lineH);
        var parts = property.propertyPath.Split('[', ']');
        int index = int.Parse(parts[parts.Length - 2]);

        float labelH = lineH * 0.8f;
        EditorGUI.LabelField(row, $"Step {index + 1}", EditorStyles.miniLabel);
        row.y += labelH + spacing;

        EditorGUI.PropertyField(row, strengthProp);
        row.y += lineH + spacing;

        EditorGUI.BeginDisabledGroup(isRest);
        EditorGUI.PropertyField(row, positionProp);
        EditorGUI.EndDisabledGroup();
        row.y += lineH + spacing;

        EditorGUI.PropertyField(row, soundProp);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineH = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        return 4 * lineH + 6 * spacing;
    }
}

#endif