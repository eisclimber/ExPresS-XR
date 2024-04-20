using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using System.Linq;

namespace ExPresSXR.Experimentation.DataGathering
{
    [Serializable]
    public class DataGatheringBinding
    {
        /// <summary>
        /// The header value for the column storing values of this binding.
        /// </summary>
        public string exportColumnName = "";

        /// <summary>
        /// Separator used for the header, will be controlled by the DataGatherer controlling this binding.
        /// </summary>
        private char _headerSeparator = CsvUtility.DEFAULT_COLUMN_SEPARATOR;
        public char headerSeparator
        {
            get => _headerSeparator;
            set
            {
                _headerSeparator = value;

                if (AttributeHelpers.HasAttribute<HeaderReplacementAttribute>(_targetMemberInfo))
                {
                    exportColumnName = AttributeHelpers.GetReplacementHeader(_targetMemberInfo, _headerSeparator);
                }
            }
        }


        [SerializeField]
        private GameObject _targetObject = null;

        [SerializeField]
        private Component _targetComponent = null;

        [SerializeField]
        private MemberInfo _targetMemberInfo = null;


        // List of all members
        [SerializeField]
        private string[] _memberNameList = new string[0];

        // Prettified List of all members
        [SerializeField]
        private string[] _prettyMemberNameList = new string[0];


        // Index of components in the list
        [SerializeField]
        private int _memberIdx = -1;

        /// <summary>
        /// Default Constructor. Sets the defaults, i.e. an empty binding.
        /// </summary>
        public DataGatheringBinding() { }

        /// <summary>
        /// Creates a DataGatheringBinding that is already bound to the member specified by the component and name.
        /// The later must be a pretty name (i.e. as displayed in the menu, including the annotation for methods).
        /// If the name is invalid, it will be bound only to the GameObject but no value will be selected.
        /// </summary>
        /// <param name="targetComponent">Component(&Object) to be bound.</param>
        /// <param name="valueName">Name of the member to bind to.</param>
        public DataGatheringBinding(Component targetComponent, string valueName, string exportColumnName = "")
        {
            _targetObject = targetComponent.gameObject;
            _targetComponent = targetComponent;
            this.exportColumnName = exportColumnName;
            UpdateMemberList();
            _memberIdx = Array.FindIndex(_prettyMemberNameList, s => s.EndsWith(valueName));
            ValidateBinding();
        }


        /// <summary>
        /// Checks if the current binding is valid, meaning the class, component and MemberInfo are valid.
        /// </summary>
        /// <returns>If the binding is valid.</returns>
        public bool ValidateBinding()
        {
            string targetMemberName = GetTargetMemberName();
            if (_targetObject != null && !string.IsNullOrEmpty(targetMemberName))
            {
                string[] splitName = targetMemberName.Split('/');
                if (splitName.Length != 2)
                {
                    UpdateInvocationInfo(null);
                    return false;
                }

                string compFullName = DataGatheringHelpers.ExtractComponentNumber(splitName[0], out int compNumber);
                string memberName = splitName[1];

                if (DataGatheringHelpers.IsGameObjectBinding(compFullName))
                {
                    UpdateInvocationInfo(_targetObject.GetType().GetMember(memberName)[0]);
                    return true;
                }
                else
                {
                    (MemberInfo info, Component comp) = SelectIthComponent(compFullName, memberName, compNumber);
                    UpdateInvocationInfo(info, comp);
                    return true;
                }
            }
            UpdateInvocationInfo(null);
            return false;
        }

        /// <summary>
        /// Updates the list of available members shown in the dropdown for the binding in the inspector.
        /// </summary>
        public void UpdateMemberList()
        {
            List<string> newAvailableMembers = new();
            List<string> newPrettyMemberNames = new();

            if (_targetObject != null)
            {
                // Methods from the GameObject itself
                foreach (MemberInfo info in _targetObject.GetType().GetMembers())
                {
                    if (DataGatheringHelpers.IsExportableMemberInfo(info))
                    {
                        newAvailableMembers.Add($"Game Object/{info.Name}");
                        newPrettyMemberNames.Add($"Game Object/{DataGatheringHelpers.GetPrettifiedMemberName(info)}");
                    }
                }

                Component[] components = _targetObject.GetComponents<Component>();
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
                        if (DataGatheringHelpers.IsExportableMemberInfo(info))
                        {
                            newAvailableMembers.Add($"{compFullName}/{info.Name}");
                            newPrettyMemberNames.Add($"{compName}/{DataGatheringHelpers.GetPrettifiedMemberName(info)}");
                        }
                    }

                    compFullNameCounts[componentType.FullName] = duplicatesCount + 1;
                }
            }
            _memberNameList = newAvailableMembers.ToArray();
            _prettyMemberNameList = newPrettyMemberNames.ToArray();
        }

        /// <summary>
        /// Returns the value specified by the binding, creating and invoking the MemberInfo.
        /// </summary>
        /// <returns>The value of the binding or null.</returns>
        public string GetBindingValue()
        {
            if (ValidateBinding())
            {
                // If _targetComponent is null then the binding is to the GameObject
                object valueProvider = _targetComponent == null ? _targetObject : _targetComponent;
                object result = null;
                switch (_targetMemberInfo.MemberType)
                {
                    case MemberTypes.Method:
                        MethodInfo methodInfo = (MethodInfo)_targetMemberInfo;
                        // Allow passing a separator to other functions
                        object[] args = DataGatheringHelpers.GetMethodParameterValues(methodInfo, headerSeparator);
                        result = methodInfo.Invoke(valueProvider, args);
                        break;
                    case MemberTypes.Field:
                        result = ((FieldInfo)_targetMemberInfo).GetValue(valueProvider);
                        break;
                    case MemberTypes.Property:
                        result = ((PropertyInfo)_targetMemberInfo).GetValue(valueProvider);
                        break;
                }

                return result?.ToString() ?? "";
            }
            Debug.LogError($"The DataGatherer could not validate the {GetBoundObjectDescription()}"
                           + ". An empty string will be exported.");
            return "";
        }

        private void UpdateInvocationInfo(MemberInfo memberInfo, Component component = null)
        {
            bool hadHeaderReplacement = AttributeHelpers.HasAttribute<HeaderReplacementAttribute>(_targetMemberInfo);
            bool memberChanged = _targetMemberInfo != memberInfo;
            _targetMemberInfo = memberInfo;
            _targetComponent = component;
            bool hasHeaderReplacement = AttributeHelpers.HasAttribute<HeaderReplacementAttribute>(_targetMemberInfo);

            // Clear only if there was a replacement or if a replacement should be added/updated
            if (hadHeaderReplacement || hasHeaderReplacement)
            {
                exportColumnName = AttributeHelpers.GetReplacementHeader(_targetMemberInfo, _headerSeparator);
            }

            // Print Notice (only if changed)
            if (memberChanged && AttributeHelpers.TryGetAttribute(_targetMemberInfo, out HeaderReplacementNoticeAttribute notice))
            {
                Debug.LogWarning(notice.notice);
            }
        }


        private (MemberInfo, Component) SelectIthComponent(string compFullName, string memberName, int compNumber)
        {
            int matchingComps = 0;

            foreach (Component component in _targetObject.GetComponents<Component>())
            {
                if (DataGatheringHelpers.IsComponentMatch(component, compFullName))
                {
                    MemberInfo[] memberInfos = component.GetType().GetMember(memberName);
                    if (memberInfos == null || memberInfos.Length <= 0)
                    {
                        Debug.LogError("Found a matching component but it has no member. "
                            + $"Please reconnect the binding {component.name}({compNumber})/{memberName}.");
                        return (null, null);
                    }
                    if (matchingComps == compNumber)
                    {
                        return (memberInfos[0], component);
                    }
                    else
                    {
                        matchingComps++;
                    }
                }
            }
            return (null, null);
        }

        private string GetTargetMemberName()
        {
            if (_targetObject != null && _memberNameList != null
                && _memberIdx >= 0 && _memberIdx < _memberNameList.Length)
            {
                return _memberNameList[_memberIdx];
            }
            return "";
        }

        /// <summary>
        /// Returns a description of the binding, listing all important values.
        /// </summary>
        /// <returns>The description as string.</returns>
        public string GetBindingDescription() => $"{GetBoundObjectDescription()} will be exported to column '{exportColumnName}'.";

        /// <summary>
        /// Returns a description of the bound object.
        /// </summary>
        /// <returns>The description as string.</returns>
        public string GetBoundObjectDescription() => $"Binding to object '{_targetObject} ', component '"
                    + $"{_targetComponent?.name ?? "No Component"}' and value/function '{_targetMemberInfo?.Name ?? "No Value"}'";

        /// <summary>
        ///  Resets the values of the binding (for internal purposes).
        /// </summary>
        public void ResetToDefaults()
        {
            UpdateInvocationInfo(null);
            _targetObject = null;
            _memberNameList = new string[0];
            _prettyMemberNameList = new string[0];
            _memberIdx = -1;
            headerSeparator = CsvUtility.DEFAULT_COLUMN_SEPARATOR;
        }

        /// <summary>
        /// Checks whether the current binding is bound to a multi-column member.
        /// </summary>
        /// <returns>If it is multi-column or not.</returns>
        public bool IsBoundToMultiColumnValue() => AttributeHelpers.HasAttribute<MultiColumnValueAttribute>(_targetMemberInfo);
    }
}