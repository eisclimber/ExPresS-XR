using UnityEngine;


namespace ExPresSXR.Misc
{
    public class TestOutputFunctions : MonoBehaviour
    {
        public string TestMessage1 = "Test Output 1";
        public string TestMessage2 = "Test Output 2";
        public string TestMessage3 = "Test Output 3";
        public string TestMessage4 = "Test Output 4";

        public void DebugPrint1() => Debug.Log(TestMessage1);

        public void DebugPrint2() => Debug.Log(TestMessage2);

        public void DebugPrint3() => Debug.Log(TestMessage3);

        public void DebugPrint4() => Debug.Log(TestMessage4);
    }
}