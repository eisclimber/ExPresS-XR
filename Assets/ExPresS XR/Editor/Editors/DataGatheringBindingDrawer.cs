using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Experimentation.DataGathering;
using ExPresSXR.Interaction.ButtonQuiz;
using System.Text.RegularExpressions;


namespace ExPresSXR.Editor
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
                    // Update members and reset _memberIdx if _targetObject changed
                    UpdateMembers(property, property.FindPropertyRelative("_targetObject").objectReferenceValue as GameObject);
                    property.FindPropertyRelative("_memberIdx").intValue = -1;
                }

                positionRect = new Rect(positionRect.x,
                                        positionRect.y + EditorGUIUtility.singleLineHeight + PROPERTY_SPACING,
                                        positionRect.width,
                                        EditorGUIUtility.singleLineHeight);

                bool memberSelectionEnabled = IsObjectPropertyNull(property.FindPropertyRelative("_targetObject"));

                EditorGUI.BeginDisabledGroup(memberSelectionEnabled);

                string[] popupOptions = GetPopupMemberNames(property.FindPropertyRelative("_prettyMemberNameList"));
                SerializedProperty memberIdx = property.FindPropertyRelative("_memberIdx");

                EditorGUI.BeginChangeCheck();
                // Popup (Add subtract 1 to account for an invalid member)
                memberIdx.intValue = EditorGUI.Popup(positionRect, "Value To Save", memberIdx.intValue + 1, popupOptions) - 1;

                if (EditorGUI.EndChangeCheck())
                {
                    // Change ExportColumnName if special Methods of TutorialQuiz were selected
                    TryAddingSpecialExportColumnName(property, popupOptions, memberIdx.intValue + 1);
                }

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
                    if (DataGatheringBinding.IsExportableMemberInfo(info))
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

                Component[] components = targetObject.GetComponents<Component>();
                Dictionary<string, int> compCounts = new();
                Dictionary<string, int> compFullNameCounts = new();

                foreach (Component component in components)
                {
                    // Found a duplicates of the simple/short names
                    if (compCounts.TryGetValue(component.GetType().Name, out int duplicatesCount))
                    {
                        duplicatesCount++;
                    }
                    compCounts[component.GetType().Name] = duplicatesCount;
                }

                // Methods from all the  other components
                foreach (Component component in components)
                {
                    Type componentType = component.GetType();
                    int duplicatesCount = 0;
                    string compName = componentType.Name;
                    string compFullName = componentType.FullName;
                    if (compCounts[componentType.Name] > 0)
                    {
                        // If "simple" names have duplicates try using the full names or index
                        if (compFullNameCounts.TryGetValue(componentType.FullName, out duplicatesCount))
                        {
                            compName = $"{componentType.FullName} ({duplicatesCount})";
                            compFullName = compName;
                        }
                        else
                        {
                            compName = componentType.FullName;
                        }
                    }

                    foreach (MemberInfo info in component.GetType().GetMembers())
                    {
                        if (DataGatheringBinding.IsExportableMemberInfo(info))
                        {
                            availableMemberNames.InsertArrayElementAtIndex(i);
                            availableMemberNames.GetArrayElementAtIndex(i).stringValue =
                                    $"{compFullName}/{info.Name}";

                            prettyMemberNames.InsertArrayElementAtIndex(i);
                            prettyMemberNames.GetArrayElementAtIndex(i).stringValue =
                                    $"{compName}/{GetPrettifiedMemberName(info)}";

                            i++;
                        }
                    }

                    compFullNameCounts[componentType.FullName] = duplicatesCount + 1;
                }
            }
        }

        private void TryAddingSpecialExportColumnName(SerializedProperty property, string[] prettyMembers, int idx)
        {
            if (idx >= 0 && idx < prettyMembers.Length)
            {
                string[] splitName = prettyMembers[idx].Split('/');

                // Sanity check, is pretty member name formatted
                if (splitName.Length != 2)
                {
                    Debug.LogError($"Binding '{prettyMembers[idx]}' was not formatted properly. This should not happen.");
                    return;
                }

                ExportColumnReplacement[] replacements = ExportColumnReplacement.GetStandardReplacements();
                int i = 0;
                foreach (ExportColumnReplacement replacement in replacements)
                {
                    if (replacement == null || !replacement.IsComplete())
                    {
                        Debug.LogWarning($"Replacement `{replacement}` is not valid. Please remove it via the Data Gathering Config Window!");
                        continue;
                    }

                    if (splitName[0].EndsWith(replacement.componentName))
                    {
                        string pattern = $@"\w+ {replacement.memberName}(\((char\?? sep)?\))?";
                        Match m = Regex.Match(splitName[1], pattern);

                        if (m.Success)
                        {
                            property.FindPropertyRelative("exportColumnName").stringValue = replacement.replacementHeader;
                            if (!string.IsNullOrEmpty(replacement.matchInfoMessage))
                            {
                                Debug.Log($"Special Export Column found: {replacement.matchInfoMessage}");
                            }
                            break;
                        }
                        i++;
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
                prettyName = $"{primitiveTypeKeywords[infoType]} {info.Name}";
            }
            if (info.MemberType == MemberTypes.Method)
            {
                // Add brackets and optional 'char? sep' to functions
                prettyName += DataGatheringBinding.HasSeparatorType((MethodInfo)info) ? "(char? sep)" : "()";
            }

            return prettyName;
        }

        static readonly Dictionary<Type, string> primitiveTypeKeywords = new()
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
            { typeof(Vector2), "Vector2" },
            { typeof(Vector3), "Vector3" },
            { typeof(Quaternion), "Quaternion" }
        };
    }
}