using UnityEditor.Animations;
using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    /// <summary>
    /// Uses an Animator to switch between colors.
    /// The logic for switching is implemented inside the AnimationController of the assigned Animator.
    /// For this each state must have a `MaterialSwitchingStateBehaviour` assigned with the desired material.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ColorAnimatorSwitcher : MonoBehaviour
    {
        /// <summary>
        /// Prefix assumed when changing colors via indices.
        /// </summary>
        public const string TRIGGER_PREFIX = "TrColor";

        /// <summary>
        /// The animator with the Animation Controller implementing the logic for switching materials.
        /// </summary>
        [SerializeField]
        [Tooltip("The animator with the Animation Controller implementing the logic for switching materials.")]
        protected Animator _animator;

        /// <summary>
        /// The MeshRenderer whose material will be manipulated.
        /// </summary>
        [SerializeField]
        [Tooltip("The MeshRenderer whose material will be manipulated.")]
        protected MeshRenderer _meshRenderer;

        /// <inheritdoc />
        protected virtual void Awake()
        {
            if (_animator == null && !TryGetComponent(out _animator))
            {
                Debug.LogError("ColorAnimationSwitcher requires an Animator component to function properly.");
            }

            if (_meshRenderer == null && !TryGetComponent(out _meshRenderer))
            {
                Debug.LogError("ColorAnimationSwitcher requires a MeshRenderer component to function properly.");
            }
        }

        /// <summary>
        /// Changes to the specified material.
        /// </summary>
        /// <param name="switchMaterial">Material to switch to.</param>
        public virtual void ChangeToMaterial(Material switchMaterial)
        {
            if (_meshRenderer != null && switchMaterial != null && _meshRenderer.material != switchMaterial)
            {
                _meshRenderer.material = switchMaterial;
            }
        }

        /// <summary>
        /// Sets a trigger to change the color, adding the prefix `TRIGGER_PREFIX`.
        /// </summary>
        /// <param name="triggerIdx">Index of the trigger.</param>
        public virtual void ChangeColorWithTrigger(int triggerIdx) => ChangeColorWithTrigger(TRIGGER_PREFIX + triggerIdx);

        /// <summary>
        /// Sets a trigger to change the color.
        /// </summary>
        /// <param name="triggerName">Name of the trigger.</param>
        public virtual void ChangeColorWithTrigger(string triggerName)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(triggerName);
            }
        }


        /// <summary>
        /// Sets a bool to change the color.
        /// </summary>
        /// <param name="boolName">Name of the parameter.</param>
        /// <param name="boolValue">Value to set.</param>
        public virtual void ChangeColorWithBool(string boolName, bool boolValue)
        {
            if (_animator != null)
            {
                _animator.SetBool(boolName, boolValue);
            }
        }

        /// <summary>
        /// Sets a float to change the color.
        /// </summary>
        /// <param name="floatName">Name of the parameter.</param>
        /// <param name="floatValue">Value to set.</param>
        public virtual void ChangeColorWithFloat(string floatName, float floatValue)
        {
            if (_animator != null)
            {
                _animator.SetFloat(floatName, floatValue);
            }
        }

        /// <summary>
        /// Checks the states of the animator if they can be used for color switching.
        /// </summary>
        protected virtual void CheckAnimatorStates()
        {
            if (_animator == null)
            {
                return;
            }

            AnimatorController controller = _animator.runtimeAnimatorController as AnimatorController;

            if (controller == null) // Probably not set up yet -> do noting..
            {
                return;
            }

            AnimatorControllerLayer layer = controller.layers[0]; // Only checking the first layer for simplicity
            AnimatorStateMachine stateMachine = layer.stateMachine;

            foreach (ChildAnimatorState child in stateMachine.states)
            {
                AnimatorState state = child.state;

                EvaluateStateBehaviours(state);
                EvaluateStateTransitions(state);
            }
        }

        /// <summary>
        /// Checks a state if it can be used for color switching.
        /// </summary>
        /// <param name="state">State top check.</param>
        protected virtual void EvaluateStateBehaviours(AnimatorState state)
        {
            foreach (StateMachineBehaviour behaviour in state.behaviours)
            {
                if (behaviour is MaterialSwitchingStateBehaviour materialSwitcher)
                {
                    if (materialSwitcher.SwitchMaterial == null)
                    {
                        Debug.LogWarning(
                            $"State '{state.name}' has a MaterialSwitchingStateBehaviour with no assigned material. " +
                            "This will result in no material being applied when entering this state.", this
                        );
                    }
                    return; // Found behavior successfully -> Early exit
                }
            }
            Debug.LogWarning(
                $"State '{state.name}' does not have a MaterialSwitchingStateBehaviour. "
                + "No material switch will be performed when entering the state.", this
            );
        }

        /// <summary>
        /// Checks a states transitions if it can be used for color switching.
        /// </summary>
        /// <param name="state">State top check.</param>
        protected virtual void EvaluateStateTransitions(AnimatorState state)
        {
            foreach (AnimatorStateTransition transition in state.transitions)
            {
                if (transition.conditions.Length > 0 && transition.hasExitTime)
                {
                    Debug.LogWarning(
                        $"Transition from '{state.name}' to '{transition.destinationState.name}' has a transition with conditions and Exit Time. " +
                        "This can cause delays while switching. Disabling it.", this
                    );
                    transition.hasExitTime = false;
                }

                if (transition.interruptionSource != TransitionInterruptionSource.Destination)
                {
                    Debug.LogWarning(
                        $"State from '{state.name}' to '{transition.destinationState.name}' is not interuptable. " +
                        "This can cause issues when switching states. Setting it to 'Destination'.", this
                    );
                    transition.interruptionSource = TransitionInterruptionSource.Destination;
                }
            }
        }

        /// <summary>
        /// Ensures a correct setup of the animator.
        /// </summary>
        protected virtual void OnValidate()
        {
            CheckAnimatorStates();
        }
    }
}