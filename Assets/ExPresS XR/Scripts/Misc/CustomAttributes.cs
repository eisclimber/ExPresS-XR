using UnityEngine;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// Allows adding an attribute that displays the property as disabled in the inspector, without making it readonly.
    /// </summary>
    public class ReadonlyInInspector : PropertyAttribute { }

    
    /// <summary>
    /// Always expands the property referenced if it uses a foldout.
    /// </summary>
    public class AlwaysExpanded : PropertyAttribute { }
}
