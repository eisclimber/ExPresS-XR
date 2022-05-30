using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(DataGatheringBinding))]
public class DataGatheringBindingDrawer : PropertyDrawer
{
    private const int PROPERTY_SPACING = 2;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        if (property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label))
        {
            // Create property fields
            SerializedProperty targetColumnName = property.FindPropertyRelative("exportColumnName");
            SerializedProperty targetObject = property.FindPropertyRelative("targetObject");
            SerializedProperty targetComponentName = property.FindPropertyRelative("targetComponentName");
            SerializedProperty targetValueName = property.FindPropertyRelative("targetValueName");

            Rect positionRect = new Rect(position.x, 
                                        position.y + EditorGUIUtility.singleLineHeight + PROPERTY_SPACING, 
                                        position.width, 
                                        EditorGUI.GetPropertyHeight(targetObject));

            // Draw normal text field for column name
            EditorGUI.PropertyField(positionRect, targetColumnName);


            positionRect = new Rect(positionRect.x, 
                                    positionRect.y + EditorGUI.GetPropertyHeight(targetObject) + PROPERTY_SPACING, 
                                    positionRect.width, 
                                    EditorGUI.GetPropertyHeight(targetColumnName));

            // Draw Object field 
            EditorGUI.PropertyField(positionRect, targetObject);
            targetObject = property.FindPropertyRelative("targetObject");

            // Draw the others only if a targetObject is specified
            EditorGUI.BeginDisabledGroup(targetObject != null);

                positionRect = new Rect(positionRect.x, 
                                        positionRect.y + EditorGUI.GetPropertyHeight(targetColumnName) + PROPERTY_SPACING, 
                                        positionRect.width, 
                                        EditorGUI.GetPropertyHeight(targetComponentName));
                
                EditorGUI.Popup(positionRect, "Component", 0, GetComponentsList());

                // Draw the others only if a targetComponentName is specified
                EditorGUI.BeginDisabledGroup(targetComponentName != null);

                    positionRect = new Rect(positionRect.x, 
                                        positionRect.y + EditorGUI.GetPropertyHeight(targetComponentName) + PROPERTY_SPACING, 
                                        positionRect.width, 
                                        EditorGUI.GetPropertyHeight(targetValueName));
                    
                    EditorGUI.Popup(positionRect, "Value",  0, GetValueList());
                EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
        float totalHeight = EditorGUI.GetPropertyHeight(property, true);

        if (property.isExpanded)
        {
            // Add bottom margin
            totalHeight += PROPERTY_SPACING;
        }

        return totalHeight;
    }


    private string[] GetComponentsList()
    {
        return new string[] {"No Component Specified", "a", "b"};
    }


    private string[] GetValueList()
    {
        return new string[] {"No Value Specified", "db"};
    }
}