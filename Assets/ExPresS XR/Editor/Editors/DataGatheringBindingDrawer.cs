using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Experimentation.DataGathering;
using ExPresSXR.Interaction.ButtonQuiz;
using System.Text.RegularExpressions;
using UnityEditor.Localization.Plugins.XLIFF.V12;


namespace ExPresSXR.Editor.Editors
{
    [CustomPropertyDrawer(typeof(DataGatheringBinding))]
    public class DataGatheringBindingDrawer : PropertyDrawer
    {
        private const int PROPERTY_SPACING = 2;
        private const string NO_VALUE_TEXT = "No Value";


        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            Rect positionRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (property.isExpanded = EditorGUI.Foldout(positionRect, property.isExpanded, label))
            {
                positionRect = new Rect(positionRect.x,
                                        positionRect.y + EditorGUIUtility.singleLineHeight + PROPERTY_SPACING,
                                        positionRect.width,
                                        EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("exportColumnName"));

                positionRect = new Rect(positionRect.x,
                                        positionRect.y + EditorGUIUtility.singleLineHeight + PROPERTY_SPACING,
                                        positionRect.width,
                                        EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(positionRect, property.FindPropertyRelative("_targetObject"));
                if (EditorGUI.EndChangeCheck())
                {
                    // New targetObject = reset memberIndex
                    property.FindPropertyRelative("_memberIdx").intValue = -1;
                    UpdateMemberList(property);
                }

                positionRect = new Rect(positionRect.x,
                                        positionRect.y + EditorGUIUtility.singleLineHeight + PROPERTY_SPACING,
                                        positionRect.width,
                                        EditorGUIUtility.singleLineHeight);

                bool memberSelectionEnabled = IsObjectPropertyNull(property.FindPropertyRelative("_targetObject"));

                EditorGUI.BeginDisabledGroup(memberSelectionEnabled);

                string[] popupOptions = GetPopupMemberNames(property.FindPropertyRelative("_prettyMemberNameList"));
                SerializedProperty memberIdx = property.FindPropertyRelative("_memberIdx");

                // Popup (Add subtract 1 to account for an invalid member)
                memberIdx.intValue = EditorGUI.Popup(positionRect, "Value To Save", memberIdx.intValue + 1, popupOptions) - 1;

                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return 4 * EditorGUIUtility.singleLineHeight + 4 * PROPERTY_SPACING;
            }
            return EditorGUIUtility.singleLineHeight;
        }

        public string[] GetPopupMemberNames(SerializedProperty property)
        {
            if (property != null && property.isArray && property.arrayElementType == "string")
            {
                string[] members = new string[property.arraySize + 1];
                members[0] = NO_VALUE_TEXT;
                for (int i = 0; i < property.arraySize; i++)
                {
                    members[i + 1] = property.GetArrayElementAtIndex(i).stringValue;
                }
                return members;
            }
            return new string[] { NO_VALUE_TEXT };
        }

        private void UpdateMemberList(SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();
            int ownIndex = ParseOwnArrayIndex(property.propertyPath);
            DataGatherer dataGatherer = (DataGatherer)property.serializedObject.targetObject;
            dataGatherer.dataBindings[ownIndex].UpdateMemberList();
        }


        private bool IsObjectPropertyNull(SerializedProperty property) => property == null || property.objectReferenceValue == null;


        /// <summary>
        /// Extract the index of the currently displayed DataGatheringBinding from the propertyPath.
        /// The propertyPath should look something like this: _dataBindings.Array.data[1].
        /// Returns -1 if not possible.
        /// </summary>
        /// <param name="path">The propertyPath to be parse.</param>
        /// <returns>The index of the property or -1 if not possible.</returns>
        private int ParseOwnArrayIndex(string path)
        {
            int startIndex = path.IndexOf("[") + 1;
            int endIndex = path.IndexOf("]");

            if (startIndex != -1 && endIndex != -1)
            {
                string indexStr = path.Substring(startIndex, endIndex - startIndex);
                int index;
                if (int.TryParse(indexStr, out index))
                {
                    return index;
                }
            }
            // Error or not an array element
            return -1;
        }
    }
}