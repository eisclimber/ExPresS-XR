using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace ExPresSXR.Misc
{
    public class InputActionDebugFunctions : MonoBehaviour
    {

        public InputActionReference testInputAction1;
        public InputActionReference testInputAction2;
        public InputActionReference testInputAction3;
        public InputActionReference testInputAction4;

        public UnityEvent OnTestInput1;
        public UnityEvent OnTestInput2;
        public UnityEvent OnTestInput3;
        public UnityEvent OnTestInput4;


        private void Awake() {
            if (testInputAction1 != null)
            {
                testInputAction1.action.performed += TestInput1Callback;
            }

            if (testInputAction2 != null)
            {
                testInputAction2.action.performed += TestInput2Callback;
            }

            if (testInputAction3 != null)
            {
                testInputAction3.action.performed += TestInput3Callback;
            }

            if (testInputAction4 != null)
            {
                testInputAction4.action.performed += TestInput4Callback;
            }
        }

        // Callbacks for checks on input
        private void TestInput1Callback(InputAction.CallbackContext context) => OnTestInput1.Invoke();

        private void TestInput2Callback(InputAction.CallbackContext context) => OnTestInput2.Invoke();

        private void TestInput3Callback(InputAction.CallbackContext context) => OnTestInput3.Invoke();

        private void TestInput4Callback(InputAction.CallbackContext context) => OnTestInput4.Invoke();

        
        // Print for debug
        public void PrintFunc1() => Debug.Log("Called test function 1");

        public void PrintFunc2() => Debug.Log("Called test function 2");

        public void PrintFunc3() => Debug.Log("Called test function 3");

        public void PrintFunc4() => Debug.Log("Called test function 4");
    }
}