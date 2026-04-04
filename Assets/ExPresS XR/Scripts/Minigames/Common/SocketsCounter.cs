using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Minigames.Common
{
    /// <summary>
    /// Counts the number of sockets selecting objects.
    /// </summary>
    public class SocketsCounter : MonoBehaviour
    {
        /// <summary>
        /// Sockets to be regarded.
        /// </summary>
        [SerializeField]
        [Tooltip("Sockets to be regarded.")]
        private XRSocketInteractor[] _sockets;

        [SerializeField]
        [ReadonlyInInspector]
        private int _count;
        /// <summary>
        /// Current count in the socket.
        /// </summary>
        public int Count
        {
            get => _count;
            set
            {
                bool wasEmpty = _count == 0;
                _count = value;

                OnCountChanged.Invoke(_count);

                if (!wasEmpty && _count == 0)
                {
                    OnAllSocketsEmptied.Invoke();
                }

                if (wasEmpty && _count > 0)
                {
                    OnFirstSocketFilled.Invoke();
                }

                if (_count >= Capacity)
                {
                    OnAllSocketsFilled.Invoke();
                }
            }
        }

        /// <summary>
        /// Number of sockets regarded.
        /// </summary>
        public int Capacity
        {
            get => _sockets != null ? _sockets.Length : 0;
        }

        
        /// <summary>
        /// Forwards the SelectEnterEventArgs of one of the sockets.
        /// </summary>
        public UnityEvent<SelectEnterEventArgs> OnSocketsSelect;

        /// <summary>
        /// Forwards the SelectExitEventArgs of one of the sockets.
        /// </summary>
        public UnityEvent<SelectExitEventArgs> OnSocketsDeselect;

        /// <summary>
        /// Emitted when the count changes providing the new count.
        /// </summary>
        public UnityEvent<int> OnCountChanged;

        /// <summary>
        /// Emitted when the all socket were emptied.
        /// </summary>
        public UnityEvent OnAllSocketsEmptied;

        /// <summary>
        /// Emitted when the first socket was filled.
        /// </summary>
        public UnityEvent OnFirstSocketFilled;

        /// <summary>
        /// Emitted when the all socket were filled.
        /// </summary>
        public UnityEvent OnAllSocketsFilled;

        private void OnEnable()
        {
            foreach (XRSocketInteractor socket in _sockets)
            {
                socket.selectEntered.AddListener(IncreaseBoxCount);
                socket.selectExited.AddListener(DecreaseBoxCount);
            }
        }

        private void OnDisable()
        {
            foreach (XRSocketInteractor socket in _sockets)
            {
                socket.selectEntered.RemoveListener(IncreaseBoxCount);
                socket.selectExited.RemoveListener(DecreaseBoxCount);
            }
        }

        [ContextMenu("Increase Count")]
        private void IncreaseBoxCount() => Count++;
        private void IncreaseBoxCount(SelectEnterEventArgs args)
        {
            IncreaseBoxCount();
            OnSocketsSelect.Invoke(args);
        }


        [ContextMenu("Decrease Count")]
        private void DecreaseBoxCount() => Count--;
        private void DecreaseBoxCount(SelectExitEventArgs args)
        {
            DecreaseBoxCount();
            OnSocketsDeselect.Invoke(args);
        }


        [ContextMenu("Complete")]
        private void Complete() => Count = Capacity;

        [ContextMenu("Reset Count")]
        private void ResetCount() => Count = 0;
    }
}