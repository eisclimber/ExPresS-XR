using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DataGatheringBinding))]
public class DataGatheringBindingDrawer : PropertyDrawer
{
    private const int PROPERTY_SPACING = 2;
    private const string NO_VALUE_TEXT = "No Value";

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect positionRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

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
                // Reset _memberIdx if the targetObject changed
                property.FindPropertyRelative("_memberIdx").intValue = -1;
            }
            UpdateMembers(property, property.FindPropertyRelative("_targetObject").objectReferenceValue as GameObject);

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


    private bool IsObjectPropertyNull(SerializedProperty property)
    {
        return property == null || property.objectReferenceValue == null;
    }

    private void UpdateMembers(SerializedProperty property, GameObject targetObject)
    {
        SerializedProperty availableMemberNames = property.FindPropertyRelative("_memberNameList");
        SerializedProperty prettyMemberNames = property.FindPropertyRelative("_prettyMemberNameList");

        availableMemberNames.ClearArray();
        prettyMemberNames.ClearArray();

        int i = 0;

        if (targetObject != null)
        {
            // Methods from the GameObject itself
            foreach (MemberInfo info in targetObject.GetType().GetMembers())
            {
                if (DataGatheringBinding.IsValidMemberInfo(info))
                {
                    availableMemberNames.InsertArrayElementAtIndex(i);
                    availableMemberNames.GetArrayElementAtIndex(i).stringValue =
                                    string.Format("{0}/{1}", "Game Object", info.Name);

                    prettyMemberNames.InsertArrayElementAtIndex(i);
                    prettyMemberNames.GetArrayElementAtIndex(i).stringValue =
                                string.Format("{0}/{1}", "Game Object", GetPrettifiedMemberName(info));

                    i++;
                }
            }

            // Methods from all the  other components
            foreach (Component component in targetObject.GetComponents<Component>())
            {
                // TODO Multiple same components
                foreach (MemberInfo info in component.GetType().GetMembers())
                {
                    if (DataGatheringBinding.IsValidMemberInfo(info))
                    {
                        availableMemberNames.InsertArrayElementAtIndex(i);
                        availableMemberNames.GetArrayElementAtIndex(i).stringValue = 
                                string.Format("{0}/{1}", component.GetType().FullName, info.Name);

                        prettyMemberNames.InsertArrayElementAtIndex(i);
                        prettyMemberNames.GetArrayElementAtIndex(i).stringValue =
                                string.Format("{0}/{1}", component.GetType().Name, GetPrettifiedMemberName(info));

                        i++;
                    }
                }
            }
        }
    }

    private string GetPrettifiedMemberName(MemberInfo info)
    {
        Type infoType = DataGatheringBinding.GetMemberValueType(info);
        string prettyName = info.Name;

        if (primitiveTypeKeywords.ContainsKey(infoType))
        {
            prettyName = String.Format("{0} {1}", primitiveTypeKeywords[infoType], info.Name);
        }
        if (info.MemberType == MemberTypes.Method)
        {
            // Add brackets to functions
            prettyName += "()";
        }

        return prettyName;
    }

    static Dictionary<Type, string> primitiveTypeKeywords = new Dictionary<Type, string>
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "short" },
        { typeof(string), "string" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(ushort), "ushort" },
    };
}