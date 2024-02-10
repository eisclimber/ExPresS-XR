using System;
using System.Linq;
using System.Reflection;
using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;

namespace ExPresSXR.Experimentation.DataGathering
{
    /// <summary>
    /// Helper attribute to annotate multi Columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public class MultiColumnValueAttribute : Attribute { }


    /// <summary>
    /// Helper attribute for header replacements with multi Columns.
    /// The header-fields can be provided as individual parameters for the annotation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public class HeaderReplacementAttribute : Attribute
    {
        private string[] _headers;

        public HeaderReplacementAttribute(params string[] headers)
        {
            _headers = headers;
        }

        public string GetHeaders(char sep) => string.Join(sep, _headers);
    }

    /// <summary>
    /// Helper attribute for header replacement notifications with multi Columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public class HeaderReplacementNoticeAttribute : Attribute
    {
        public string notice;

        public HeaderReplacementNoticeAttribute(string notice)
        {
            this.notice = notice;
        }
    }

    /// <summary>
    /// A class containing various helpers for using custom attributes.
    /// </summary>
    public static class AttributeHelpers
    {
        /// <summary>
        /// Returns the header replacement of `memberInfo` if exist, separated by `sep`.
        /// </summary>
        /// <param name="memberInfo">MemeberInfo to retrieve the header replacement from.</param>
        /// <param name="sep">Separator character to be used.</param>
        /// <returns>Returns the header replacement or an empty string.</returns>
        public static string GetReplacementHeader(MemberInfo memberInfo, char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
        {
            HeaderReplacementAttribute headerAttribute = GetAttribute<HeaderReplacementAttribute>(memberInfo);
            return headerAttribute != null ? headerAttribute.GetHeaders(sep) : "";
        }

        /// <summary>
        /// Checks if a MemberInfo is annotated with the given Attribute.
        /// </summary>
        /// <typeparam name="T">Attribute to be checked.</typeparam>
        /// <param name="memberInfo">MemberInfo to be checked.</param>
        /// <returns>If the member has the Attribute.</returns>
        public static bool HasAttribute<T>(MemberInfo memberInfo) where T : Attribute
            => memberInfo != null && GetAttribute<T>(memberInfo) != null;


        /// <summary>
        /// Retrieves an attribute from a MemberInfo if possible.
        /// </summary>
        /// <typeparam name="T">Attribute to be retrieved.</typeparam>
        /// <param name="memberInfo">MemberInfo to retrieve the attribute from.</param>
        /// <returns>The attribute or null if not found.</returns>
        public static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            object[] customAttributes = memberInfo?.GetCustomAttributes(typeof(T), false);
            return customAttributes != null && customAttributes.Length > 0 ? (T)customAttributes[0] : null;
        }

        /// <summary>
        /// Returns the attribute as out-parameter and returns true if the attribute was found.
        /// </summary>
        /// <param name="memberInfo">MemberInfo to retrieve the attribute from.</param>
        /// <param name="attribute">Parameter to be set to the reference of the attribute (or null if it does not exits)</param>
        /// <typeparam name="T">Attribute to be retrieved.</typeparam>
        /// <returns>If the attribute was found.</returns>
        public static bool TryGetAttribute<T>(MemberInfo memberInfo, out T attribute) where T : Attribute
        {
            attribute = GetAttribute<T>(memberInfo);
            return attribute != null;
        }
    }
}