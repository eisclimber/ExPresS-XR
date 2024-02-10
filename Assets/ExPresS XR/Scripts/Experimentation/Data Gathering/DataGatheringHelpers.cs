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
        /// <summary>
        /// Separates the component number from the provided component name.
        /// Returns the name without the number and returns the component number to the out parameter.
        /// </summary>
        /// <param name="compFullName">Component name to extract the component number from.</param>
        /// <param name="compNumber">Value that will be filled with hte component number or -1 if not found.</param>
        /// <returns>Component name without the component number.</returns>
        public static string ExtractComponentNumber(string compFullName, out int compNumber)
        {
            // Matches any number in brackets and a space in front at the end of the name
            Match match = Regex.Match(compFullName, @" \(\d+\)$");
            compNumber = -1;
            
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

        /// <summary>
        /// Checks if the component matches the fully qualified name.
        /// </summary>
        /// <param name="component">Component to match.</param>
        /// <param name="requiredFullName">Fully qualified name to match.</param>
        /// <returns>If the component and name match.</returns>
        public static bool IsComponentMatch(Component component, string requiredFullName)
            => component != null && component.GetType().FullName == requiredFullName;


        /// <summary>
        /// Checks if a name is bound to a GameObject (and not a Component).
        /// </summary>
        /// <param name="classFullName">Fully qualified name to match.</param>
        /// <returns>true if the name matches a GameObject.</returns>
        public static bool IsGameObjectBinding(string classFullName)
            => classFullName == GAME_OBJECT_FULL_NAME;

        #endregion

        #region Validation
        /// <summary>
        /// Check if the parameters of a method can be used with the DataGatherer.
        /// Either no parameters, only optional or all optional but the first being a character (for the separator).
        /// </summary>
        /// <param name="methodInfo">MethodInfo abut the method to be checked.</param>
        /// <returns>If the method can be used for DataGathering.</returns>
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

        /// <summary>
        /// Check if a type is exportable. Either primitive, string, Vector, or Quaternion.
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>If the type is exportable.</returns>
        public static bool IsTypeExportable(Type type)
            => type.IsPrimitive || type == typeof(string) || type == typeof(Vector2) 
                                || type == typeof(Vector3) || type == typeof(Quaternion);

        /// <summary>
        /// Checks if the MemberInfo is valid for data gathering.
        /// The requirements are:
        /// - All: Must be an exportable type 
        /// - Methods: Should not be of type void nor have any parameters and have no special name (e.g. auto-generated setters).
        /// - Property: Valid if can be read.
        /// - Fields: No additional requirements.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsExportableMemberInfo(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Method)
            {
                MethodInfo methodInfo = (MethodInfo)info;
                // Methods should not be of type void nor have any parameters and have no special name (e.g. auto-generated setters)
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


        /// <summary>
        /// Returns the (return-)type of the MemberInfo.
        /// </summary>
        /// <param name="info">MemberInfo to retrieve the type from.</param>
        /// <returns>Type of the member.</returns>
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
        /// <summary>
        /// Checks if a method has parameters, the first is mandatory and of type char 
        /// while the rest are optional. (Does not guarantee validity!)
        /// </summary>
        /// <param name="methodInfo">MethodInfo to be checked.</param>
        /// <returns>If the MethodInfo accepts a mandatory separator character.</returns>
        public static bool HasSeparatorType(MethodInfo methodInfo)
        {
            // First parameter must be a character (required or optional) 
            // and starting from the second, all other parameter must be optional
            ParameterInfo[] parameters = methodInfo.GetParameters();
            return parameters.Length > 0 && parameters[0].ParameterType == typeof(char)
                && (parameters.Length <= 1 || parameters[1].IsOptional);
        }

        /// <summary>
        /// Checks if a method has only parameters and the first is of type char.
        /// (Does not guarantee validity!)
        /// </summary>
        /// <param name="methodInfo">MethodInfo to be checked.</param>
        /// <returns>If the MethodInfo accepts a optional separator character.</returns>
        public static bool HasOptionalSeparatorType(MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            return parameters.Length > 0 && parameters[0].ParameterType == typeof(char) && parameters[0].IsOptional;
        }

        /// <summary>
        /// Generates an array with the default values for all parameters
        /// but inserts the separator character if possible.
        /// </summary>
        /// <param name="methodInfo">MethodInfo for which the array should be generated.</param>
        /// <param name="sep">Separator char that may be inserted in first place.</param>
        /// <returns>An array with the default values of the functions parameters.</returns>
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