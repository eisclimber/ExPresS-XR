using UnityEngine;


namespace ExPresSXR.Misc
{
    public class TestButtonScript : MonoBehaviour
    {
        public string testMessage1 = "Test Output 1";
        public string testMessage2 = "Test Output 2";
        public string testMessage3 = "Test Output 3";
        public string testMessage4 = "Test Output 4";

        public void DebugPrint1() => Debug.Log(testMessage1);

        public void DebugPrint2() => Debug.Log(testMessage2);

        public void DebugPrint3() => Debug.Log(testMessage3);

        public void DebugPrint4() => Debug.Log(testMessage4);
    }
}