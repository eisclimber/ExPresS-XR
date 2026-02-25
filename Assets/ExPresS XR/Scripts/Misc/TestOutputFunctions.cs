using UnityEngine;


namespace ExPresSXR.Misc
{
    /// <summary>
    /// A small script for some quick debug print functions. 
    /// </summary>
    public class TestOutputFunctions : MonoBehaviour
    {
        /// <summary>
        /// Output when calling `DebugPrint1()`.
        /// </summary>
        public string TestMessage1 = "Test Output 1";

        /// <summary>
        /// Output when calling `DebugPrint2()`.
        /// </summary>
        public string TestMessage2 = "Test Output 2";

        /// <summary>
        /// Output when calling `DebugPrint3()`.
        /// </summary>
        public string TestMessage3 = "Test Output 3";

        /// <summary>
        /// Output when calling `DebugPrint4()`.
        /// </summary>
        public string TestMessage4 = "Test Output 4";

        /// <summary>
        /// Prints the value of `TestMessage1`.
        /// </summary>
        public void DebugPrint1() => Debug.Log(TestMessage1);

        /// <summary>
        /// Prints the value of `TestMessage2`.
        /// </summary>

        public void DebugPrint2() => Debug.Log(TestMessage2);

        /// <summary>
        /// Prints the value of `TestMessage3`.
        /// </summary>

        public void DebugPrint3() => Debug.Log(TestMessage3);

        /// <summary>
        /// Prints the value of `TestMessage4`.
        /// </summary>

        public void DebugPrint4() => Debug.Log(TestMessage4);
    }
}