using UnityEngine;

namespace ExPresSXR.Minigames.Archery.TargetSpawner.Line
{
    public class ObjectResetter : MonoBehaviour
    {
        private Vector3 startPos;
        private Vector3 startDir;

        private void Start()
        {
            startPos = gameObject.transform.position;
            startDir = gameObject.transform.eulerAngles;
        }

        /// <summary>
        /// Resets the object to it's initial starting position.
        /// </summary>
        public void ResetObject()
        {
            gameObject.transform.position = startPos;
            gameObject.transform.eulerAngles = startDir;
        }
    }
}