using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    public class MaterialSwitchingStateBehaviour : StateMachineBehaviour
    {
        [SerializeField]
        private Material _material;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Animator entered state: " + stateInfo.shortNameHash);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Animator exited state: " + stateInfo.shortNameHash);
        }
    }
}