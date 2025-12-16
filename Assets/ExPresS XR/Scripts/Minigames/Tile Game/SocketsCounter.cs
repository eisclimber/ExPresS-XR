using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Minigames.TileGame
{
    public class SocketsCounter : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor[] _sockets;

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                bool wasEmpty = _count == 0;
                bool hasFirst = _count == 1;
                _count = value;

                OnCountChanged.Invoke(_count);

                if (wasEmpty && _count > 0)
                {
                    OnFirstSocketFilled.Invoke();
                }

                if (hasFirst && _count > 1)
                {
                    OnSecondSocketFilled.Invoke();
                }

                if (_count >= Capacity)
                {
                    OnAllSocketsFilled.Invoke();
                }
            }
        }

        public int Capacity
        {
            get => _sockets != null ? _sockets.Length : 0;
        }


        public UnityEvent<SelectEnterEventArgs> OnSocketsSelect;
        public UnityEvent<SelectExitEventArgs> OnSocketsDeselect;
        public UnityEvent<int> OnCountChanged;
        public UnityEvent OnFirstSocketFilled;
        public UnityEvent OnSecondSocketFilled;
        public UnityEvent OnAllSocketsFilled;

        private void OnEnable()
        {
            foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket in _sockets)
            {
                socket.selectEntered.AddListener(IncreaseBoxCount);
                socket.selectExited.AddListener(DecreaseBoxCount);
            }
        }

        private void OnDisable()
        {
            foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket in _sockets)
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
    }
}