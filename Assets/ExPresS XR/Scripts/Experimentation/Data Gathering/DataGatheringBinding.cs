using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Data;

namespace ExPresSXR.Experimentation.DataGathering
{
    [Serializable]
    public class DataGatheringBinding
    {
        public string exportColumnName = "";

        private char _headerSeparator = CsvUtility.DEFAULT_COLUMN_SEPARATOR;//
        public char headerSeparator
        {
            get => _headerSeparator;
            set
            {
                _headerSeparator = value;
                UpdateExportColumnName();
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

        private string GetTargetMemberName()
        {
            if (_targetObject != null && _memberNameList != null
                && _memberIdx >= 0 && _memberIdx < _memberNameList.Length)
            {
                return _memberNameList[_memberIdx];
            }
            return "";
        }

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


        private void UpdateInvocationInfo(MemberInfo memberInfo, Component component = null)
        {
            _targetMemberInfo = memberInfo;
            _targetComponent = component;
            UpdateExportColumnName();
        }

        private void UpdateExportColumnName()
        {
            exportColumnName = _targetMemberInfo != null 
                                ? AttributeHelpers.GetReplacementHeader(_targetMemberInfo, headerSeparator) 
                                : "";
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

        public string GetBindingDescription() => $"{GetBoundObjectDescription()} will be exported to column '{exportColumnName}'.";

        public string GetBoundObjectDescription() => $"Binding to object '{_targetObject} ', component '"
                    + $"{_targetComponent?.name ?? "No Component"}' and value/function '{_targetMemberInfo?.Name ?? "No Value"}'";

        public void ResetToDefaults()
        {
            UpdateInvocationInfo(null);
            _targetObject = null;
            _memberNameList = new string[0];
            _prettyMemberNameList = new string[0];
            _memberIdx = -1;
            headerSeparator = CsvUtility.DEFAULT_COLUMN_SEPARATOR;
        }
    }
}