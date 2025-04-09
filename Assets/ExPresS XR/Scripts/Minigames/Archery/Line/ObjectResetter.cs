using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class ObjectResetter : MonoBehaviour
    {
        private Vector3 startPos;
        private Vector3 startDir;

        // Start is called before the first frame update
        void Start()
        {
            startPos = gameObject.transform.position;
            startDir = gameObject.transform.eulerAngles;
        }

        public void ResetObj()
        {
            gameObject.transform.position = startPos;
            gameObject.transform.eulerAngles = startDir;
        }
    }
}