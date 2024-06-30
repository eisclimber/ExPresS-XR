using UnityEngine;
using UnityEditor;
using ExPresSXR.Interaction.ValueRangeInteractable;
using UnityEditor.XR.Interaction.Toolkit;

namespace ExPresSXR.Editor.Editors
{
    /// <summary>
    /// This class describes the default editor for all ValueRangeInteractables without their own editor 
    /// and can be used to derive other custom editors from it.
    /// 
    /// Please be aware that this editor inherits from the <see cref="XRBaseInteractableEditor"/> so unused properties will be drawn automatically.
    /// To mark them as used, you need to define a SerializedProperties including the docstrings(!) and assign a value to them in the constructor.
    /// </summary>
    [CustomEditor(typeof(ValueRangeInteractable<,,>), true)]
    [CanEditMultipleObjects]
    public class ValueRangeInteractableEditor : XRBaseInteractableEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._ValueDescriptor"/>.</summary>
        protected SerializedProperty _valueDescriptor;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._valueVisualizer"/>.</summary>
        protected SerializedProperty _valueVisualizer;
        

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._zeroValueOnRelease"/>.</summary>
        protected SerializedProperty _zeroValueOnRelease;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._requireDirectInteraction"/>.</summary>
        protected SerializedProperty _requireDirectInteraction;


        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._snapSound"/>.</summary>
        protected SerializedProperty _snapSound;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._minValueSound"/>.</summary>
        protected SerializedProperty _minValueSound;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._maxValueSound"/>.</summary>
        protected SerializedProperty _maxValueSound;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._moveSound"/>.</summary>
        protected SerializedProperty _moveSound;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._moveDeltaSoundThreshold"/>.</summary>
        protected SerializedProperty _moveDeltaSoundThreshold;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._defaultAudioPlayer"/>.</summary>
        protected SerializedProperty _defaultAudioPlayer;


        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnMinValue"/>.</summary>
        protected SerializedProperty _onMinValue;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnMaxValue"/>.</summary>
        protected SerializedProperty _onMaxValue;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnSnapped"/>.</summary>
        protected SerializedProperty _onSnapped;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnValueChanged"/>.</summary>
        protected SerializedProperty _onValueChanged;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnValueChangedString"/>.</summary>
        protected SerializedProperty _onValueChangedString;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnValueSelected"/>.</summary>
        protected SerializedProperty _onValueSelected;

        protected IRangeInteractorInternal _rangeInteractableInternal;

        protected static bool _showSounds = false;
        protected static bool _showValueEvents = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            _valueDescriptor = serializedObject.FindProperty("_valueDescriptor");
            _valueVisualizer = serializedObject.FindProperty("_valueVisualizer");

            _zeroValueOnRelease = serializedObject.FindProperty("_zeroValueOnRelease");
            _requireDirectInteraction = serializedObject.FindProperty("_requireDirectInteraction");

            _snapSound = serializedObject.FindProperty("_snapSound");
            _minValueSound = serializedObject.FindProperty("_minValueSound");
            _maxValueSound = serializedObject.FindProperty("_maxValueSound");
            _moveSound = serializedObject.FindProperty("_moveSound");
            _moveDeltaSoundThreshold = serializedObject.FindProperty("_moveDeltaSoundThreshold");
            _defaultAudioPlayer = serializedObject.FindProperty("_defaultAudioPlayer");

            _onMinValue = serializedObject.FindProperty("OnMinValue");
            _onMaxValue = serializedObject.FindProperty("OnMaxValue");
            _onSnapped = serializedObject.FindProperty("OnSnapped");
            _onValueChanged = serializedObject.FindProperty("OnValueChanged");
            _onValueChangedString = serializedObject.FindProperty("OnValueChangedString");
            _onValueSelected = serializedObject.FindProperty("OnValueSelected");

            _rangeInteractableInternal = (IRangeInteractorInternal)target;
        }


        protected override void DrawProperties()
        {
            DrawRangeProperties();
            DrawSoundsFoldout();
            base.DrawProperties();
        }

        protected virtual void DrawRangeProperties()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_valueDescriptor);
            EditorGUILayout.PropertyField(_valueVisualizer);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _rangeInteractableInternal.InternalUpdateValue();
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_zeroValueOnRelease);
            EditorGUILayout.PropertyField(_requireDirectInteraction);

            EditorGUILayout.Space();
        }

        protected virtual void DrawSoundsFoldout()
        {
            _showSounds = EditorGUILayout.BeginFoldoutHeaderGroup(_showSounds, "Sounds");
            if (_showSounds)
            {
                DrawSoundsProperties();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }


        protected virtual void DrawSoundsProperties()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_snapSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_minValueSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxValueSound"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_moveSound"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultAudioPlayer"), true);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        protected override void DrawEvents()
        {
            base.DrawEvents();

            EditorGUILayout.Space();

            DrawValueEventsFoldout();
        }


        protected virtual void DrawValueEventsFoldout()
        {
            _showValueEvents = EditorGUILayout.BeginFoldoutHeaderGroup(_showValueEvents, "Value Events");
            if (_showValueEvents)
            {
                DrawValueEvents();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected virtual void DrawValueEvents()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_onMinValue, true);
            EditorGUILayout.PropertyField(_onMaxValue, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onSnapped, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onValueChanged, true);
            EditorGUILayout.PropertyField(_onValueChangedString, true);
            EditorGUILayout.PropertyField(_onValueSelected, true);
            EditorGUI.indentLevel--;
        }
    }
}