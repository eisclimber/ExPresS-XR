using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class DataGatheringBinding
{
    [SerializeField]
    public GameObject targetObject = null;

    [SerializeField]
    public string targetComponentName = "";

    [SerializeField]
    public string targetValueName = "";

    [SerializeField]
    public string exportColumnName = "";


    // References to the target member that provide the data
    private MemberInfo _valueMemberInfo;
    private Component _valueComponent;


    public bool bindingIsValid
    {
        get => (_valueMemberInfo != null && _valueComponent != null);
    }


    // Returns an 
    public string GetBindingValue()
    {
        if (bindingIsValid)
        {
            object result = null;
            switch (_valueMemberInfo.MemberType)
            {
                case MemberTypes.Method:
                    result = ((MethodInfo)_valueMemberInfo).Invoke(_valueComponent, new object[0]);
                    break;
                case MemberTypes.Field:
                    result = ((FieldInfo)_valueMemberInfo).GetValue(_valueComponent);
                    break;
                case MemberTypes.Property:
                    result = ((PropertyInfo)_valueMemberInfo).GetValue(_valueComponent);
                    break;
            }

            if (result != null)
            {
                return result.ToString();
            }
        }
        return "";
    }


    public bool ValidateBinding()
    {
        if (targetObject == null)
        {
            _valueMemberInfo = null;
            _valueComponent = null;
            Debug.LogError("No target object was assigned.");
            return false;
        }

        foreach (Component component in targetObject.GetComponents(typeof(Component)))
        {
            if (IsComponentMatch(component.GetType().ToString()))
            {
                // Maybe use members && Check if get_
                MemberInfo[] members = component.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (MemberInfo member in members)
                {
                    if (IsMemberMatch(member))
                    {
                        _valueMemberInfo = member;
                        _valueComponent = component;
                        return true;
                    }
                }
                Debug.LogErrorFormat("No accessible member with name '{0}' in component '{1}' was found.",
                    targetValueName,
                    targetComponentName);
                _valueMemberInfo = null;
                _valueComponent = null;
                return false;
            }
        }
        Debug.LogErrorFormat("No component '{0}' was found in GameObject '{1}'.", targetComponentName, targetObject);
        _valueMemberInfo = null;
        _valueComponent = null;
        return false;
    }

    private bool IsComponentMatch(string fullName)
    {
        // Either use full name (unique) or the common name
        string[] splitName = fullName.Split('.');
        string commonName = splitName[splitName.Length - 1];

        // Filter spaces and use it to compare 
        // (Classes can't have spaces but will be displayed in Unity)
        string spacelessComponentName = Regex.Replace(targetComponentName, @"\s+", "");

        return (spacelessComponentName == fullName || spacelessComponentName == commonName);
    }


    private bool IsMemberMatch(MemberInfo memberInfo)
    {
        return IsMemberNameMatch(memberInfo) && IsTypeValid(memberInfo);
    }


    private bool IsMemberNameMatch(MemberInfo memberInfo)
    {
        string getterName = targetValueName;
        // Remove hungarian notation prefix
        if (getterName.StartsWith("m_") && targetValueName.Length >= 2)
        {
            getterName = getterName.Substring(2, getterName.Length - 2);
        }
        // Add prefix for auto-generated getters
        getterName = "get_" + getterName;

        // Check if name matches
        return (targetValueName == memberInfo.Name || getterName == memberInfo.Name);
    }

    private bool IsTypeValid(MemberInfo memberInfo)
    {
        if (memberInfo.MemberType == MemberTypes.Method)
        {
            MethodInfo methodInfo = (MethodInfo)memberInfo;
            if (methodInfo.ReturnType == typeof(void) || methodInfo.GetParameters().Length > 0)
            {
                Debug.LogErrorFormat("Method with name {0} was found but was not a Method with a non-void return type and zero peramters.", memberInfo.Name);
                return false;
            }
            return true;
        }
        else if ((memberInfo.MemberType != MemberTypes.Property && ((PropertyInfo)memberInfo).CanRead) && memberInfo.MemberType != MemberTypes.Field)
        {
            Debug.LogErrorFormat("Member with name {0} was found but it was neither a Field or a readable Property.", memberInfo.Name);
        }

        return true;
    }
}