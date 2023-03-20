using System;
using System.Reflection;
using UnityEngine;
using System.Text.RegularExpressions;

namespace ExPresSXR.Experimentation.DataGathering
{
    [Serializable]
    public class DataGatheringBinding
    {
        private const string GAME_OBJECT_FULL_NAME = "Game Object";

        public string exportColumnName = "";


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


        public void SetClassDefaults()
        {
            exportColumnName = "";
            _targetObject = null;
            _targetComponent = null;
            _targetMemberInfo = null;
            _memberNameList = new string[0];
            _prettyMemberNameList = new string[0];
            _memberIdx = -1;
        }


        private string GetTargetMemberName()
        {
            if (_targetObject != null && _memberNameList != null
                && _memberIdx >= 0 && _memberIdx < _memberNameList.Length)
            {
                return _memberNameList[_memberIdx];
            }
            return null;
        }

        public bool ValidateBinding()
        {
            string targetMemberName = GetTargetMemberName();
            if (_targetObject != null && targetMemberName != null)
            {
                string[] splitName = targetMemberName.Split('/');
                if (splitName.Length != 2)
                {
                    return false;
                }

                string compFullName = ExtractComponentNumber(splitName[0], out int compNumber);
                string memberName = splitName[1];

                if (FindGameObjectBinding(compFullName, memberName)
                        || FindIthComponent(compFullName, memberName, compNumber))
                {
                    return true;
                }
            }
            // Binding invalid reset value
            _targetComponent = null;
            _targetMemberInfo = null;
            return false;
        }

        private string ExtractComponentNumber(string compFullName, out int compNumber)
        {
            compNumber = 0;

            // Matches any number in brackets and a space in front at the end of the name
            Match match = Regex.Match(compFullName, @" \(\d+\)$");

            if (match.Success)
            {
                // Remove padding literals from number value
                string numberString = match.Value[2..^1];
                // Debug.Log("Length: " + match.Value.Length + " x Idx: " + match.Index + "  x  " + compFullName.Length);
                compNumber = int.Parse(numberString);
                // Cut the number from the name name
                return compFullName[..match.Index];
            }
            // FullName does not contain a number -> must be the first or a unique component
            return compFullName;
        }

        // Returns true upon success
        private bool FindGameObjectBinding(string classFullName, string memberName)
        {
            if (IsGameObjectBinding(classFullName))
            {
                _targetComponent = null;
                _targetMemberInfo = _targetObject.GetType().GetMember(memberName)[0];
                Debug.Log(_targetMemberInfo);
                return true;
            }
            return false;
        }

        // Returns true upon success
        private bool FindIthComponent(string compFullName, string memberName, int compNumber)
        {
            int matchingComps = 0;

            foreach (Component component in _targetObject.GetComponents<Component>())
            {
                if (IsComponentMatch(component, compFullName))
                {
                    if (matchingComps == compNumber)
                    {
                        _targetComponent = component;
                        _targetMemberInfo = component.GetType().GetMember(memberName)[0];
                        return true;
                    }
                    else
                    {
                        matchingComps++;
                    }
                }
            }
            return false;
        }


        private bool IsComponentMatch(Component component, string requiredFullName)
            => component != null && component.GetType().FullName == requiredFullName;


        private bool IsGameObjectBinding(string classFullName) 
            => classFullName == GAME_OBJECT_FULL_NAME;


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
                        result = ((MethodInfo)_targetMemberInfo).Invoke(valueProvider, new object[0]);
                        break;
                    case MemberTypes.Field:
                        result = ((FieldInfo)_targetMemberInfo).GetValue(valueProvider);
                        break;
                    case MemberTypes.Property:
                        result = ((PropertyInfo)_targetMemberInfo).GetValue(valueProvider);
                        break;
                }

                if (result != null)
                {
                    return result.ToString();
                }
            }
            Debug.LogError("The DataGatherer could not validate the "
                    + GetBoundObjectDescription()
                    + ". An empty string will be exported.");
            return "";
        }

        public string GetBindingDescription()
        {
            return "The " + GetBoundObjectDescription() + " will be exported to column '"
                + exportColumnName + "'.";
        }

        public string GetBoundObjectDescription()
        {
            return "Binding to object '" + _targetObject
                + "', component '" + _targetComponent?.name + "' and value/function '"
                + _targetMemberInfo?.Name + "'";
        }


        //////////////////////////////////////////
        //          Static functions
        //////////////////////////////////////////

        public static bool IsTypeExportable(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(Vector2)
            || type == typeof(Vector3) || type == typeof(Quaternion);
        }

        public static bool IsExportableMemberInfo(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Method)
            {
                MethodInfo methodInfo = (MethodInfo)info;
                // Methods should not be of type void nor have any parameters and has no special name (e.g. auto-generated setters)
                return IsTypeExportable(methodInfo.ReturnType) && methodInfo.GetParameters().Length <= 0 && !methodInfo.IsSpecialName;
            }
            // A property is valid if it can be read
            bool validProperty = info.MemberType == MemberTypes.Property
                                && ((PropertyInfo)info).CanRead
                                && IsTypeExportable(((PropertyInfo)info).PropertyType);
            // fields are considered valid
            bool validField = info.MemberType == MemberTypes.Field
                                && IsTypeExportable(((FieldInfo)info).FieldType);
            return validProperty || validField;
        }

        public static Type GetMemberValueType(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Method
                && IsTypeExportable(((MethodInfo)info).ReturnType))
            {
                return ((MethodInfo)info).ReturnType;
            }
            else if (info.MemberType == MemberTypes.Property
                && IsTypeExportable(((PropertyInfo)info).PropertyType))
            {
                return ((PropertyInfo)info).PropertyType;
            }
            else if (info.MemberType == MemberTypes.Field
                && IsTypeExportable(((FieldInfo)info).FieldType))
            {
                return ((FieldInfo)info).FieldType;
            }

            return null;
        }
    }
}