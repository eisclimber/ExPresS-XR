using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ExPresSXR.Experimentation.DataGathering
{
    public static class DataGatheringHelpers
    {
        public const string GAME_OBJECT_FULL_NAME = "Game Object";

        #region Find Component
        public static string ExtractComponentNumber(string compFullName, out int compNumber)
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


        public static bool IsComponentMatch(Component component, string requiredFullName)
            => component != null && component.GetType().FullName == requiredFullName;


        public static bool IsGameObjectBinding(string classFullName)
            => classFullName == GAME_OBJECT_FULL_NAME;

        #endregion

        #region Validation
        public static bool AreParametersValid(MethodInfo methodInfo)
        {
            // No Parameters are always valid
            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length <= 0)
            {
                return true;
            }

            // Check parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo currentParam = parameters[i];
                // To be valid the first may be a non-optional of type char (as separator)
                // All others must be optional
                if (i == 0 && currentParam.ParameterType != typeof(char) && !currentParam.IsOptional
                    || i > 0 && !currentParam.IsOptional)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsTypeExportable(Type type)
            => type.IsPrimitive || type == typeof(string) || type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Quaternion);

        public static bool IsExportableMemberInfo(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Method)
            {
                MethodInfo methodInfo = (MethodInfo)info;
                // Methods should not be of type void nor have any parameters and has no special name (e.g. auto-generated setters)
                return IsTypeExportable(methodInfo.ReturnType) && AreParametersValid(methodInfo) && !methodInfo.IsSpecialName;
            }
            // A property is valid if it can be read
            bool validProperty = info.MemberType == MemberTypes.Property
                                && ((PropertyInfo)info).CanRead
                                && IsTypeExportable(((PropertyInfo)info).PropertyType);
            // Exposed fields are considered valid
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
        #endregion

        #region Parameters
        // Can accept parameters if the first parameter is a separator (does not guarantee validity!)
        public static bool HasSeparatorType(MethodInfo methodInfo)
        {
            // First parameter must be a character (required or optional) 
            // and starting from the second, all other parameter must be optional
            ParameterInfo[] parameters = methodInfo.GetParameters();
            return parameters.Length > 0 && parameters[0].ParameterType == typeof(char)
                && (parameters.Length <= 1 || parameters[1].IsOptional);
        }

        // Can accept parameters if the first parameter is a separator (does not guarantee validity!)
        public static bool HasOptionalSeparatorType(MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            return parameters.Length > 0 && parameters[0].ParameterType == typeof(char) && parameters[0].IsOptional;
        }

        // Generates an array for all optional parameters
        public static object[] GetMethodParameterValues(MethodInfo methodInfo, char sep)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            object[] args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i == 0 && parameters[i].ParameterType == typeof(char))
                {
                    // If the first parameter is a char, provide the specified separator
                    args[i] = sep;
                }
                else if (parameters[i].IsOptional)
                {
                    // If the parameter is optional, provide its default value
                    args[i] = parameters[i].DefaultValue;
                }
            }
            return args;
        }
        #endregion 
    }
}