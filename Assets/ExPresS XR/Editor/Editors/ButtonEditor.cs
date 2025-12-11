using UnityEngine;
using UnityEditor;
using ExPresSXR.Interaction.ValueRangeInteractable;


namespace ExPresSXR.Editor.Editors
{
    /// <summary>
    /// This class describes the default editor for all ValueRangeInteractables without their own editor 
    /// and can be used to derive other custom editors from it.
    /// 
    /// Please be aware that this editor inherits from the <see cref="XRBaseInteractableEditor"/> so unused properties will be drawn automatically.
    /// To mark them as used, you need to define a SerializedProperties including the docstrings(!) and assign a value to them in the constructor.
    /// </summary>
    [CustomEditor(typeof(Button), true)]
    [CanEditMultipleObjects]
    public class ButtonEditor : ValueRangeInteractableEditor
    {

        // protected override void OnEnable()
        // {
        //     base.OnEnable();

        //     _valueDescriptor = serializedObject.FindProperty("_valueDescriptor");
        //     _valueVisualizer = serializedObject.FindProperty("_valueVisualizer");

        //     _zeroValueOnRelease = serializedObject.FindProperty("_zeroValueOnRelease");
        //     _requireDirectInteraction = serializedObject.FindProperty("_requireDirectInteraction");

        //     _snapSound = serializedObject.FindProperty("_snapSound");
        //     _minValueSound = serializedObject.FindProperty("_minValueSound");
        //     _maxValueSound = serializedObject.FindProperty("_maxValueSound");
        //     _moveSound = serializedObject.FindProperty("_moveSound");
        //     _moveDeltaSoundThreshold = serializedObject.FindProperty("_moveDeltaSoundThreshold");
        //     _defaultAudioPlayer = serializedObject.FindProperty("_defaultAudioPlayer");

        //     _onMinValue = serializedObject.FindProperty("OnMinValue");
        //     _onMaxValue = serializedObject.FindProperty("OnMaxValue");
        //     _onSnapped = serializedObject.FindProperty("OnSnapped");
        //     _onValueChangedSingle = serializedObject.FindProperty("OnValueChangedSingle");
        //     _onValueChanged = serializedObject.FindProperty("OnValueChanged");
        //     _onValueChangedString = serializedObject.FindProperty("OnValueChangedString");
        //     _onValueSelected = serializedObject.FindProperty("OnValueSelected");

        //     _rangeInteractableInternal = (IRangeInteractorInternal)target;
        // }


        // protected override void DrawProperties()
        // {
        //     DrawRangeProperties();
        //     DrawSoundsFoldout();
        //     DrawButtons();
        //     base.DrawProperties();
        // }

        // protected virtual void DrawRangeProperties()
        // {
        //     EditorGUI.BeginChangeCheck();
        //     EditorGUILayout.PropertyField(_valueDescriptor);
        //     EditorGUILayout.PropertyField(_valueVisualizer);
        //     if (EditorGUI.EndChangeCheck())
        //     {
        //         serializedObject.ApplyModifiedProperties();
        //         _rangeInteractableInternal.InternalUpdateValue();
        //     }

        //     EditorGUILayout.Space();

        //     EditorGUILayout.PropertyField(_zeroValueOnRelease);
        //     EditorGUILayout.PropertyField(_requireDirectInteraction);

        //     EditorGUILayout.Space();
        // }

        // protected virtual void DrawSoundsFoldout()
        // {
        //     _showSounds = EditorGUILayout.BeginFoldoutHeaderGroup(_showSounds, "Sounds");
        //     if (_showSounds)
        //     {
        //         DrawSoundsProperties();
        //     }
        //     EditorGUILayout.EndFoldoutHeaderGroup();
        // }


        // protected virtual void DrawSoundsProperties()
        // {
        //     EditorGUI.indentLevel++;
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("_snapSound"), true);
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("_minValueSound"), true);
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxValueSound"), true);
        //     EditorGUILayout.Space();
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("_moveSound"), true);
        //     EditorGUILayout.Space();
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultAudioPlayer"), true);
        //     EditorGUI.indentLevel--;
        //     EditorGUILayout.Space();
        // }

        // protected override void DrawEvents()
        // {
        //     base.DrawEvents();

        //     EditorGUILayout.Space();

        //     DrawValueEventsFoldout();
        // }


        // protected virtual void DrawValueEventsFoldout()
        // {
        //     _showValueEvents = EditorGUILayout.BeginFoldoutHeaderGroup(_showValueEvents, "Value Events");
        //     if (_showValueEvents)
        //     {
        //         DrawValueEvents();
        //     }
        //     EditorGUILayout.EndFoldoutHeaderGroup();
        // }

        // protected virtual void DrawValueEvents()
        // {
        //     EditorGUI.indentLevel++;
        //     EditorGUILayout.PropertyField(_onMinValue, true);
        //     EditorGUILayout.PropertyField(_onMaxValue, true);
        //     EditorGUILayout.Space();
        //     EditorGUILayout.PropertyField(_onSnapped, true);
        //     EditorGUILayout.Space();
        //     EditorGUILayout.PropertyField(_onValueChangedSingle, true);
        //     EditorGUILayout.PropertyField(_onValueChanged, true);
        //     EditorGUILayout.PropertyField(_onValueChangedString, true);
        //     EditorGUILayout.PropertyField(_onValueSelected, true);
        //     EditorGUI.indentLevel--;
        // }

        // protected override void DrawButtons()
        // {
        //     float buttonWidth = (EditorGUIUtility.currentViewWidth - 24.0f) / 2.0f;
        //     GUILayout.BeginHorizontal();
        //     if (GUILayout.Button("Snap to Min", GUILayout.Width(buttonWidth)))
        //     {
        //         _rangeInteractableInternal.SetValueToMinValue();
        //     }

        //     if (GUILayout.Button("Snap to Max", GUILayout.Width(buttonWidth)))
        //     {
        //         _rangeInteractableInternal.SetValueToMaxValue();
        //     }
        //     GUILayout.EndHorizontal();
        //     GUILayout.BeginHorizontal();
        //     if (GUILayout.Button("Print Value", GUILayout.Width(buttonWidth)))
        //     {
        //         Debug.Log(_rangeInteractableInternal.ToString());
        //     }

        //     if (GUILayout.Button("Reset", GUILayout.Width(buttonWidth)))
        //     {
        //         _rangeInteractableInternal.ResetValue();
        //     }
        //     GUILayout.EndHorizontal();

        //     EditorGUILayout.Space();
        // }
    }
}