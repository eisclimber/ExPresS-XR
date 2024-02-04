using System;
using System.Linq;
using System.Reflection;
using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;

namespace ExPresSXR.Experimentation.DataGathering
{
    // Helper class to annotate multi Columns
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public class MultiColumnValueAttribute : Attribute { }


    // Helper class to annotate multi Columns
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

    // Helper class to annotate multi Columns
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
        public static string GetReplacementHeader(MemberInfo memberInfo, char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
        {
            HeaderReplacementAttribute headerAttribute = GetAttribute<HeaderReplacementAttribute>(memberInfo);
            return headerAttribute != null ? headerAttribute.GetHeaders(sep) : "";
        }

        public static bool HasAttribute<T>(MemberInfo memberInfo) where T : Attribute
            => memberInfo != null && GetAttribute<T>(memberInfo) != null;


        public static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            object[] customAttributes = memberInfo?.GetCustomAttributes(typeof(T), false);
            return customAttributes != null && customAttributes.Length > 0 ? (T)customAttributes[0] : null;
        }

        public static bool TryGetAttribute<T>(MemberInfo memberInfo, out T attribute) where T : Attribute
        {
            attribute = GetAttribute<T>(memberInfo);
            return attribute != null;
        }
    }
}