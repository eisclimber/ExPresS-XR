using UnityEditor.Animations;
using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    [RequireComponent(typeof(Animator))]
    public class ColorAnimatorSwitcher : MonoBehaviour
    {
        public const string TRIGGER_PREFIX = "TrColor";

        [SerializeField]
        [Tooltip("The animator with the Animation Controller implementing the logic for switching materials.")]
        protected Animator _animator;

        /// <summary>
        /// The MeshRenderer whose material will be manipulated.
        /// </summary>
        [SerializeField]
        [Tooltip("The MeshRenderer whose material will be manipulated.")]
        protected MeshRenderer _meshRenderer;

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

        public virtual void ChangeToMaterial(Material switchMaterial)
        {
            if (_meshRenderer != null && switchMaterial != null)
            {
                _meshRenderer.material = switchMaterial;
            }
        }

        public virtual void ChangeColorWithTrigger(int triggerIdx) => ChangeColorWithTrigger(TRIGGER_PREFIX + triggerIdx);

        public virtual void ChangeColorWithTrigger(string triggerName)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(triggerName);
            }
        }

        public virtual void ChangeColorWithBool(string boolName, bool boolValue)
        {
            if (_animator != null)
            {
                _animator.SetBool(boolName, boolValue);
            }
        }

        public virtual void ChangeColorWithFloat(string floatName, float floatValue)
        {
            if (_animator != null)
            {
                _animator.SetFloat(floatName, floatValue);
            }
        }

        protected virtual void CheckAnimatorStates()
        {
            if (_animator == null)
            {
                return;
            }
            
            AnimatorController controller = _animator.runtimeAnimatorController as AnimatorController;
            AnimatorControllerLayer layer = controller.layers[0]; // Only checking the first layer for simplicity
            AnimatorStateMachine  stateMachine = layer.stateMachine;

            foreach (ChildAnimatorState child in stateMachine.states)
            {
                AnimatorState state = child.state;

                EvaluateStateBehaviours(state);
                EvaluateStateTransitions(state);
            }
        }

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

        protected virtual void EvaluateStateTransitions(AnimatorState state)
        {
            foreach (AnimatorStateTransition transition in state.transitions)
            {
                if (transition.hasExitTime)
                {
                    Debug.LogWarning(
                        $"Transition from '{state.name}' to '{transition.destinationState.name}' has a transition with Exit Time. " +
                        "This can cause delays while switching. Disabling it.", this
                    );
                    transition.hasExitTime = false;
                }

                if (transition.duration > 0.0f)
                {
                    Debug.LogWarning(
                        $"Transition from '{state.name}' to '{transition.destinationState.name}' has a transition with non-zero duration. " +
                        "This can cause delays while switching. Setting transition duration to 0.", this
                    );
                    transition.duration = 0.0f;
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

        protected virtual void OnValidate()
        {
            CheckAnimatorStates();
        }
    }
}