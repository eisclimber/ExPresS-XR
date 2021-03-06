using System;
using System.Reflection;
using UnityEngine;

namespace ExPresSXR.Experimentation.DataGathering
{
    [System.Serializable]
    public class DataGatheringBinding
    {
        [SerializeField]
        public string exportColumnName;


        [SerializeField]
        private GameObject _targetObject;

        [SerializeField]
        private Component _targetComponent;

        [SerializeField]
        private MemberInfo _targetMemberInfo;


        // List of all members
        [SerializeField]
        private string[] _memberNameList = new string[0];

        // Prettified List of all members
        [SerializeField]
        private string[] _prettyMemberNameList = new string[0];


        // Index of components in the list
        [SerializeField]
        private int _memberIdx;


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
            string memberName = GetTargetMemberName();
            if (_targetObject != null && memberName != null)
            {
                string[] splitName = memberName.Split('/');
                if (splitName.Length != 2)
                {
                    return false;
                }

                foreach (Component component in _targetObject.GetComponents<Component>())
                {
                    if (IsComponentMatch(component, splitName[0]))
                    {
                        _targetComponent = component;
                        _targetMemberInfo = component.GetType().GetMember(splitName[1])[0];
                        return true;
                    }
                }
            }
            else
            {
                _targetComponent = null;
                _targetMemberInfo = null;
            }
            return false;
        }


        private bool IsComponentMatch(Component component, string requiredFullName)
        {
            return (component != null && component.GetType().FullName == requiredFullName);
        }


        public string GetBindingValue()
        {
            if (ValidateBinding())
            {
                object result = null;
                switch (_targetMemberInfo.MemberType)
                {
                    case MemberTypes.Method:
                        result = ((MethodInfo)_targetMemberInfo).Invoke(_targetComponent, new object[0]);
                        break;
                    case MemberTypes.Field:
                        result = ((FieldInfo)_targetMemberInfo).GetValue(_targetComponent);
                        break;
                    case MemberTypes.Property:
                        result = ((PropertyInfo)_targetMemberInfo).GetValue(_targetComponent);
                        break;
                }

                if (result != null)
                {
                    return result.ToString();
                }
            }
            return "";
        }

        public string GetBindingDescription()
        {
            return ("The binding to object '" + _targetObject
                + "', component '" + _targetComponent?.name + "' and value/function '"
                + _targetMemberInfo?.Name + "' will be exported to column '"
                + exportColumnName + "'.");
        }


        //////////////////////////////////////////
        //          Static functions
        //////////////////////////////////////////

        public static bool IsTypeExportable(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(Vector2)
            || type == typeof(Vector3) || type == typeof(Quaternion);
        }

        public static bool IsValidMemberInfo(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Method)
            {
                MethodInfo methodInfo = (MethodInfo)info;
                // Methods should not be of type void nor have any parameters and has no special name (e.g. auto-generated seeters)
                return IsTypeExportable(methodInfo.ReturnType) && methodInfo.GetParameters().Length <= 0 && !methodInfo.IsSpecialName;
            }
            // A property is valid if it can be read
            bool validProperty = (info.MemberType == MemberTypes.Property
                                && ((PropertyInfo)info).CanRead
                                && IsTypeExportable(((PropertyInfo)info).PropertyType));
            // fields are considered valid
            bool validField = (info.MemberType == MemberTypes.Field
                                && IsTypeExportable(((FieldInfo)info).FieldType));
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