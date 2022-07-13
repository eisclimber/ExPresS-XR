using UnityEngine;


namespace ExPresSXR.Misc
{
    public class ObjectSpinner : MonoBehaviour
    {
        public Vector3 rotation;
        public float speed;

        private void Update()
        {
            transform.Rotate(speed * Time.deltaTime * rotation);
        }
    }
}