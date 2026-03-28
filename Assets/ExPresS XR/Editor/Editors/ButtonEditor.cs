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
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._pressed"/>.</summary>
        protected SerializedProperty _pressed;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._toggleMode"/>.</summary>
        protected SerializedProperty _toggleMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._maxInteractionDistance"/>.</summary>
        protected SerializedProperty _maxInteractionDistance;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._pressedSound"/>.</summary>
        protected SerializedProperty _pressedSound;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._releasedSound"/>.</summary>
        protected SerializedProperty _releasedSound;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._toggledDownSound"/>.</summary>
        protected SerializedProperty _toggledDownSound;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable._toggledUpSound"/>.</summary>
        protected SerializedProperty _toggledUpSound;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnPressed"/>.</summary>
        protected SerializedProperty _onPressed;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnReleased"/>.</summary>
        protected SerializedProperty _onReleased;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnTogglePressed"/>.</summary>
        protected SerializedProperty _onTogglePressed;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnToggleReleased"/>.</summary>
        protected SerializedProperty _onToggleReleased;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ValueRangeInteractable.OnToggleModeChanged"/>.</summary>
        protected SerializedProperty _onToggleModeChanged;

        protected Button _button;

        protected override string SnapToMinButtonLabel
        {
            get => "Set Released";
        }

        protected override string SnapToMaxButtonLabel
        {
            get => "Set Pressed";
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            _pressed = serializedObject.FindProperty("_pressed");
            _toggleMode = serializedObject.FindProperty("_toggleMode");

            _maxInteractionDistance = serializedObject.FindProperty("_maxInteractionDistance");

            _pressedSound = serializedObject.FindProperty("_pressedSound");
            _releasedSound = serializedObject.FindProperty("_releasedSound");
            _toggledDownSound = serializedObject.FindProperty("_toggledDownSound");
            _toggledUpSound = serializedObject.FindProperty("_toggledUpSound");

            _onPressed = serializedObject.FindProperty("OnPressed");
            _onReleased = serializedObject.FindProperty("OnReleased");

            _onTogglePressed = serializedObject.FindProperty("OnTogglePressed");
            _onToggleReleased = serializedObject.FindProperty("OnToggleReleased");

            _onToggleModeChanged = serializedObject.FindProperty("OnToggleModeChanged");

            _button = (Button)target;
        }

        /// <inheritdoc />
        protected override void DrawRangeProperties()
        {
            DrawPressButtons();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_toggleMode);
            EditorGUILayout.PropertyField(_inputDisabled);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _rangeInteractableInternal.InternalUpdateValue();
            }
            base.DrawRangeProperties();
        }

        /// <inheritdoc />
        protected override void DrawPostRangeProperties()
        {
            EditorGUILayout.PropertyField(_zeroValueOnRelease);
            if (!_zeroValueOnRelease.boolValue)
            {
                EditorGUILayout.HelpBox("It is recommended to keep 'ZeroValueOnRelease' set to 'true' to ensure proper snap-back when moving the hand of the button.", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(_requireDirectInteraction);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws buttons to invoke presses via the editor.
        /// </summary>
        protected virtual void DrawPressButtons()
        {
            bool toggle = _toggleMode.boolValue;
            bool inputDisabled = _inputDisabled.boolValue;

            EditorGUI.BeginDisabledGroup(inputDisabled);
            if (!toggle)
            {
                if (GUILayout.Button("Press"))
                {
                    _button.InternalForceNextPressState();
                    _button.Pressed = true;
                    // serializedObject.ApplyModifiedProperties();
                    // Reset press manually delayed
                    EditorApplication.delayCall += UndoEditorButtonPress;
                    _button.UpdateValueVisualization();
                }
            }
            else
            {
                serializedObject.ApplyModifiedProperties();
                bool pressed = _pressed.boolValue;
                string label = pressed ? "Toggle Up" : "Toggle Down";

                if (GUILayout.Button(label))
                {
                    serializedObject.UpdateIfRequiredOrScript();
                    _button.InternalForceNextPressState();
                    _button.Pressed = !pressed;
                    _button.UpdateValueVisualization();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <inheritdoc />
        protected override void DrawSoundsProperties()
        {
            EditorGUI.indentLevel++;
            // We don't care for the other sounds... the naming here is just simpler to understand
            EditorGUILayout.PropertyField(_pressedSound, true);
            EditorGUILayout.PropertyField(_releasedSound, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_toggledDownSound, true);
            EditorGUILayout.PropertyField(_toggledUpSound, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_defaultAudioPlayer, true);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        /// <inheritdoc />
        protected override void DrawValueEvents()
        {
            EditorGUI.indentLevel++;
            // Same thing here -> Use pressed events 
            EditorGUILayout.PropertyField(_onPressed, true);
            EditorGUILayout.PropertyField(_onReleased, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onTogglePressed, true);
            EditorGUILayout.PropertyField(_onToggleReleased, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onToggleModeChanged, true);
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

        protected void UndoEditorButtonPress()
        {
            _button.InternalForceNextPressState();
            _button.Pressed = false;
            _button.UpdateValueVisualization();
        }
    }
}