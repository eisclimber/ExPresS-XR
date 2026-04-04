using UnityEngine;
using UnityEditor;
using ExPresSXR.Misc.Timing;

namespace ExPresSXR.Editor.Editors
{
    [CustomPropertyDrawer(typeof(TimingUnit))]
    public class TimingUnitDrawer : PropertyDrawer
    {
        private const int PROPERTY_SPACING = 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            Rect positionRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Disable the dropdown and remove the indentation
            EditorGUI.LabelField(positionRect, label, EditorStyles.boldLabel);
            positionRect = new Rect(positionRect.x,
                                    positionRect.y + EditorGUIUtility.singleLineHeight + PROPERTY_SPACING,
                                    positionRect.width,
                                    EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("_waitTime"));

            positionRect = new Rect(positionRect.x,
                                    positionRect.y + EditorGUIUtility.singleLineHeight + PROPERTY_SPACING,
                                    positionRect.width,
                                    EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("_remainingTime"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 3 * EditorGUIUtility.singleLineHeight + 2 * PROPERTY_SPACING;
        }
    }
}