using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Excavation
{
    /// <summary>
    /// Represents a completion target of the excavation game. Consisting of a set of positions 
    /// that must be uncovered for at least `_completionValue` percent on average.
    /// </summary>
    [System.Serializable]
    public class ExcavationZone
    {
        /// <summary>
        /// No use, just a description.
        /// </summary>
        [SerializeField]
        [Tooltip("No use, just a description.")]
        private string _description;

        /// <summary>
        /// Color channel that gets checked.
        /// </summary>
        [SerializeField]
        [Tooltip("Color channel that gets checked.")]
        private ColorChannel.Channels _channel;

        /// <summary>
        /// (Average) value of the channel that needs to be reached for completion.
        /// </summary>
        [SerializeField]
        [Tooltip("(Average) value of the channel that needs to be reached for completion.")]
        private float _completionValue = 0.6f;

        /// <summary>
        /// Positions of the grid needed to be completed.
        /// </summary>
        [SerializeField]
        [Tooltip("Positions of the grid needed to be completed.")]
        private Vector2Int[] _positions;

        /// <summary>
        /// Used internally for displaying in the editor.
        /// </summary>
        [SerializeField]
        [Tooltip("Used internally for displaying in the editor.")]
        private int _granularity = -1;
        public int Granularity
        {
            get => _granularity;
            set => _granularity = value;
        }


        /// <summary>
        /// Whether not the zone was completed.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether not the zone was completed.")]
        private bool _completed;

        // Events

        /// <summary>
        /// Emitted when the zone gets completed.
        /// </summary>
        public UnityEvent OnZoneCompleted;

        /// <summary>
        /// Checks if all the sections have been completed to an average of at least `_completionValue` percent.
        /// </summary>
        /// <param name="gridWidth">Width (& Height) of the excavation game grid to check.</param>
        /// <param name="colors">Array of the Colors of each section.</param>
        /// <returns>Wether or not the zone has been completed.</returns>
        public bool CheckCompletion(int gridWidth, Color32[] colors)
        {
            if (_completed)
            {
                return true;
            }

            foreach (Vector2 pos in _positions)
            {
                int idx = (int)(pos.y * gridWidth + pos.x);
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
            _completed = true;
            OnZoneCompleted.Invoke();
            return true;
        }
    }
}