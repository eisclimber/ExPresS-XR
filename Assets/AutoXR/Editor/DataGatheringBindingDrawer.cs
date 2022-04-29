using UnityEditor;
using UnityEngine;

// [CustomPropertyDrawer(typeof(DataGatheringBinding))]
public class DataGatheringBindingDrawer : PropertyDrawer
{
    // // Draw the property inside the given rect
    // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    // {
    //     // Using BeginProperty / EndProperty on the parent property means that
    //     // prefab override logic works on the entire property.
    //     EditorGUI.BeginProperty(position, label, property);

    //     // Draw label
    //     position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //     // Don't make child fields be indented
    //     var indent = EditorGUI.indentLevel;
    //     EditorGUI.indentLevel = 0;

    //     // Calculate rects
    //     var objectRect = new Rect(position.x, position.y, 30, position.height);
    //     var componentRect = new Rect(position.x + 35, position.y, 50, position.height);
    //     var valueRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);
    //     // var exportColumnRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

    //     // Draw fields - pass GUIContent.none to each so they are drawn without labels
    //     EditorGUI.PropertyField(objectRect, property.FindPropertyRelative("targetObject"), GUIContent.none);
    //     EditorGUI.PropertyField(componentRect, property.FindPropertyRelative("targetComponentName"), GUIContent.none);
    //     EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("targetValueName"), GUIContent.none);
    //     // EditorGUI.PropertyField(exportColumnRect, property.FindPropertyRelative("exportColumnName"), GUIContent.none);

    //     // Set indent back to what it was
    //     EditorGUI.indentLevel = indent;

    //     EditorGUI.EndProperty();
    // }
}