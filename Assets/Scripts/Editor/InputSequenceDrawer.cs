#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InputSequence))]
public class InputSequenceDrawer : PropertyDrawer
{
    float topPadding = 8f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineH = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect row = new Rect(position.x, position.y, position.width, lineH);

        row.y += topPadding;

        // draw the Name field
        var nameProp = property.FindPropertyRelative("name");
        EditorGUI.PropertyField(row, nameProp);
        row.y += lineH + spacing;

        // draw sprites
        bool spritesExpanded = EditorPrefs.GetBool(property.propertyPath + "_sprites", false);
        spritesExpanded = EditorGUI.Foldout(row, spritesExpanded, "Sprites");
        EditorPrefs.SetBool(property.propertyPath + "_sprites", spritesExpanded);
        row.y += lineH + spacing;

        if (spritesExpanded)
        {
            // draw the Happy Sprite field
            var happySpriteProp = property.FindPropertyRelative("happySprite");
            EditorGUI.PropertyField(row, happySpriteProp);
            row.y += lineH + spacing;

            // draw the Sad Sprite field
            var sadSpriteProp = property.FindPropertyRelative("sadSprite");
            EditorGUI.PropertyField(row, sadSpriteProp);
            row.y += lineH + spacing;

            // draw the Silhouette field
            var silhouetteProp = property.FindPropertyRelative("silhouette");
            EditorGUI.PropertyField(row, silhouetteProp);
            row.y += lineH + spacing;
        }

        // draw the Ignore field
        var ignoreProp = property.FindPropertyRelative("ignore");
        EditorGUI.PropertyField(row, ignoreProp);
        row.y += lineH + spacing;

        bool stepsExpanded = EditorPrefs.GetBool(property.propertyPath + "_steps", false);
        stepsExpanded = EditorGUI.Foldout(row, stepsExpanded, "Steps");
        EditorPrefs.SetBool(property.propertyPath + "_steps", stepsExpanded);
        row.y += lineH + spacing;

        if (stepsExpanded)
        {
            // draw the Input Steps
            var stepsProp = property.FindPropertyRelative("steps");

            float gap = 24f;  // gap between columns
            float colWidth = (position.width - gap) / 2f;

            float savedLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60f;  // adjust as needed

            for (int i = 0; i < stepsProp.arraySize; i += 2)
            {
                var leftProp = stepsProp.GetArrayElementAtIndex(i);
                float leftH = EditorGUI.GetPropertyHeight(leftProp);
                Rect leftRect = new Rect(position.x, row.y, colWidth, leftH);
                EditorGUI.PropertyField(leftRect, leftProp, GUIContent.none);

                if (i + 1 < stepsProp.arraySize)
                {
                    var rightProp = stepsProp.GetArrayElementAtIndex(i + 1);
                    float rightH = EditorGUI.GetPropertyHeight(rightProp);
                    Rect rightRect = new Rect(position.x + colWidth + gap, row.y, colWidth, rightH);
                    EditorGUI.PropertyField(rightRect, rightProp, GUIContent.none);
                }

                row.y += leftH + spacing;
            }
            EditorGUIUtility.labelWidth = savedLabelWidth;
        }

        // at the very end, before EndProperty
        row.y += spacing * 2;
        Rect divider = new Rect(position.x, row.y, position.width, 1f);
        EditorGUI.DrawRect(divider, new Color(0.5f, 0.5f, 0.5f, 0.3f));


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineH = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        bool spritesExpanded = EditorPrefs.GetBool(property.propertyPath + "_sprites", false);
        float spritesHeight = spritesExpanded ? 3 * (lineH + spacing) : 0f;

        var stepsProp = property.FindPropertyRelative("steps");

        bool stepsExpanded = EditorPrefs.GetBool(property.propertyPath + "_steps", false);
        float stepsHeight = 0f;
        if (stepsExpanded)
        {
            for (int i = 0; i < stepsProp.arraySize; i += 2)
            {
                var prop = stepsProp.GetArrayElementAtIndex(i);
                stepsHeight += EditorGUI.GetPropertyHeight(prop) + spacing;
            }
        }

        float bottomPadding = 5f;

        // you (probably) need to increment the number after "return" if adding a new field
        return topPadding + 4 * (lineH + spacing) + 3 * spacing + spritesHeight + stepsHeight + bottomPadding;
    }
}

#endif
