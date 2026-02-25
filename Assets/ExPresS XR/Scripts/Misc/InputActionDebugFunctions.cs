using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// Debug utility for working with input actions.
    /// </summary>
    public class InputActionDebugFunctions : MonoBehaviour
    {

        /// <summary>
        /// Input action invoking the first event.
        /// </summary>
        public InputActionReference TestInputAction1;

        /// <summary>
        /// Input action invoking the second event.
        /// </summary>
        public InputActionReference TestInputAction2;

        /// <summary>
        /// Input action invoking the third event.
        /// </summary>
        public InputActionReference TestInputAction3;

        /// <summary>
        /// Input action invoking the fourth event.
        /// </summary>
        public InputActionReference TestInputAction4;

        /// <summary>
        /// Event invoked when performing `TestInputAction1`.
        /// </summary>
        public UnityEvent OnTestInput1;

        /// <summary>
        /// Event invoked when performing `TestInputAction2`.
        /// </summary>
        public UnityEvent OnTestInput2;

        /// <summary>
        /// Event invoked when performing `TestInputAction3`.
        /// </summary>F
        public UnityEvent OnTestInput3;

        /// <summary>
        /// Event invoked when performing `TestInputAction4`.
        /// </summary>
        public UnityEvent OnTestInput4;


        private void Awake()
        {
            if (TestInputAction1 != null)
            {
                TestInputAction1.action.performed += TestInput1Callback;
            }

            if (TestInputAction2 != null)
            {
                TestInputAction2.action.performed += TestInput2Callback;
            }

            if (TestInputAction3 != null)
            {
                TestInputAction3.action.performed += TestInput3Callback;
            }

            if (TestInputAction4 != null)
            {
                TestInputAction4.action.performed += TestInput4Callback;
            }
        }

        // Callbacks for checks on input
        private void TestInput1Callback(InputAction.CallbackContext context) => OnTestInput1.Invoke();

        private void TestInput2Callback(InputAction.CallbackContext context) => OnTestInput2.Invoke();

        private void TestInput3Callback(InputAction.CallbackContext context) => OnTestInput3.Invoke();

        private void TestInput4Callback(InputAction.CallbackContext context) => OnTestInput4.Invoke();


        // Print for debug

        /// <summary>
        /// Debug function 1.
        /// </summary>
        public void PrintFunc1() => Debug.Log("Called test function 1");

        /// <summary>
        /// Debug function 2.
        /// </summary>

        public void PrintFunc2() => Debug.Log("Called test function 2");

        /// <summary>
        /// Debug function 3.
        /// </summary>

        public void PrintFunc3() => Debug.Log("Called test function 3");

        /// <summary>
        /// Debug function 4.
        /// </summary>

        public void PrintFunc4() => Debug.Log("Called test function 4");
    }
}