using UnityEngine;
using UnityEngine.Events;

namespace ExPResSXR.Minigames.ExcavationGame
{
    [System.Serializable]
    public class ExcavationZone
    {
        [SerializeField]
        private ColorChannel.Channels _channel;

        [SerializeField]
        private float _completionValue = 0.6f;

        [SerializeField]
        private Vector2[] _positions;


        public UnityEvent OnCompleted;

        private bool _completed;

        public bool CheckCompletion(int gridWidth, Color32[] colors)
        {
            if (_completed)
            {
                return true;
            }

            foreach (Vector2 pos in _positions)
            {
                int idx = (int) (pos.y * gridWidth + pos.x);
                if (idx < 0 || idx >= colors.Length)
                {
                    Debug.LogWarning($"Skipping position {pos} with index {idx} as is not in range [0, {colors.Length}].");
                    continue;
                }
                Color color = colors[idx];

                float chanelValue = ColorChannel.GetColorChannelValue(_channel, color);
                if (chanelValue < _completionValue)
                {
                    return false;
                }
            }
            // Debug.Log("Yay, zone completed.");
            _completed = true;
            OnCompleted.Invoke();
            return true;
        }
    }
}