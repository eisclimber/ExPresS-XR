using UnityEditor;
using UnityEngine;
using ExPresSXR.Misc;

namespace ExPresSXR.Editor.Editors
{
    [CustomPropertyDrawer(typeof(ReadonlyInInspector))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }

    [CustomPropertyDrawer(typeof(AlwaysExpanded))]
    public class AlwaysExpandedDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Ensure it's expanded for height calculation too
            property.isExpanded = true;

            // Let Unity calculate the correct height including children
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Force the property to be expanded
            property.isExpanded = true;

            // Draw the default field
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}