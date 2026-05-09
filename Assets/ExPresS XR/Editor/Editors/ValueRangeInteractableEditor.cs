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
    [CustomEditor(typeof(ValueRangeInteractable<,,>), true)]
    [CanEditMultipleObjects]
    public class ValueRangeInteractableEditor : UnityEditor.XR.Interaction.Toolkit.Interactables.XRBaseInteractableEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._ValueDescriptor"/>.</summary>
        protected SerializedProperty _valueDescriptor;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._valueVisualizer"/>.</summary>
        protected SerializedProperty _valueVisualizer;
        

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._inputDisabled"/>.</summary>
        protected SerializedProperty _inputDisabled;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._zeroValueOnRelease"/>.</summary>
        protected SerializedProperty _zeroValueOnRelease;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._requireDirectInteraction"/>.</summary>
        protected SerializedProperty _requireDirectInteraction;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._allowNearFarInteraction"/>.</summary>
        protected SerializedProperty _allowNearFarInteraction;


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

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnValueChangedSingle"/>.</summary>
        protected SerializedProperty _onValueChangedSingle;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnValueChanged"/>.</summary>
        protected SerializedProperty _onValueChanged;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnValueSelected"/>.</summary>
        protected SerializedProperty _onValueSelected;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnValueReset"/>.</summary>
        protected SerializedProperty _onValueReset;


        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnInputDisabled"/>.</summary>
        protected SerializedProperty _onInputDisabled;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnInputEnabled"/>.</summary>
        protected SerializedProperty _onInputEnabled;


        protected IRangeInteractorInternal _rangeInteractableInternal;

        protected static bool _showSounds = false;
        protected static bool _showValueEvents = false;

        protected virtual string SnapToMinButtonLabel
        {
            get => "Snap to Min";
        }

        protected virtual string SnapToMaxButtonLabel
        {
            get => "Snap to Max";
        }

        protected virtual string PrintValueButtonLabel
        {
            get => "Snap to Max";
        }

        protected virtual string ResetButtonLabel
        {
            get => "Reset";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _valueDescriptor = serializedObject.FindProperty("_valueDescriptor");
            _valueVisualizer = serializedObject.FindProperty("_valueVisualizer");

            _inputDisabled = serializedObject.FindProperty("_inputDisabled");
            _zeroValueOnRelease = serializedObject.FindProperty("_zeroValueOnRelease");
            _requireDirectInteraction = serializedObject.FindProperty("_requireDirectInteraction");
            _allowNearFarInteraction = serializedObject.FindProperty("_allowNearFarInteraction");

            _snapSound = serializedObject.FindProperty("_snapSound");
            _minValueSound = serializedObject.FindProperty("_minValueSound");
            _maxValueSound = serializedObject.FindProperty("_maxValueSound");
            _moveSound = serializedObject.FindProperty("_moveSound");
            _moveDeltaSoundThreshold = serializedObject.FindProperty("_moveDeltaSoundThreshold");
            _defaultAudioPlayer = serializedObject.FindProperty("_defaultAudioPlayer");

            _onMinValue = serializedObject.FindProperty("OnMinValue");
            _onMaxValue = serializedObject.FindProperty("OnMaxValue");
            _onSnapped = serializedObject.FindProperty("OnSnapped");
            _onValueChangedSingle = serializedObject.FindProperty("OnValueChangedSingle");
            _onValueChanged = serializedObject.FindProperty("OnValueChanged");
            _onValueSelected = serializedObject.FindProperty("OnValueSelected");
            _onValueReset = serializedObject.FindProperty("OnValueReset");

            _onInputDisabled = serializedObject.FindProperty("OnInputDisabled");
            _onInputEnabled = serializedObject.FindProperty("OnInputEnabled");

            _rangeInteractableInternal = (IRangeInteractorInternal)target;
        }


        protected override void DrawProperties()
        {
            DrawRangeProperties();
            DrawSoundsFoldout();
            DrawButtons();

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
            DrawPostRangeProperties();
        }

        protected virtual void DrawPostRangeProperties()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_inputDisabled);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _rangeInteractableInternal.InternalUpdateInputDisabled();
            }
            EditorGUILayout.PropertyField(_zeroValueOnRelease);
            EditorGUILayout.PropertyField(_requireDirectInteraction);
            EditorGUILayout.PropertyField(_allowNearFarInteraction);
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
            EditorGUILayout.PropertyField(_snapSound, true);
            EditorGUILayout.PropertyField(_minValueSound, true);
            EditorGUILayout.PropertyField(_maxValueSound, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_moveSound, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_defaultAudioPlayer, true);
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
            EditorGUILayout.PropertyField(_onValueChangedSingle, true);
            EditorGUILayout.PropertyField(_onValueChanged, true);
            EditorGUILayout.PropertyField(_onValueSelected, true);
            EditorGUILayout.PropertyField(_onValueReset, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onInputDisabled, true);
            EditorGUILayout.PropertyField(_onInputEnabled, true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawButtons()
        {
            float buttonWidth = (EditorGUIUtility.currentViewWidth - 24.0f) / 2.0f;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(SnapToMinButtonLabel, GUILayout.Width(buttonWidth)))
            {
                _rangeInteractableInternal.SetValueToMinValue();
            }

            if (GUILayout.Button(SnapToMaxButtonLabel, GUILayout.Width(buttonWidth)))
            {
                _rangeInteractableInternal.SetValueToMaxValue();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(PrintValueButtonLabel, GUILayout.Width(buttonWidth)))
            {
                Debug.Log(_rangeInteractableInternal.ToString());
            }

            if (GUILayout.Button(ResetButtonLabel, GUILayout.Width(buttonWidth)))
            {
                _rangeInteractableInternal.ResetValue();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
    }
}