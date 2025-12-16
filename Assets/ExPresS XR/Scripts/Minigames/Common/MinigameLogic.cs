using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Common
{
    public class MinigameLogic : MonoBehaviour
    {
        [SerializeField]
        private int _gameIdx;

        [ReadonlyInInspector]
        [SerializeField]
        protected int _score;
        public virtual int Score
        {
            get => _score;
            protected set
            {
                _score = value;
                OnScoreChanged.Invoke(_score);
            }
        }

        [SerializeField]
        private float _completionEventDelay = 1.0f;

        public UnityEvent OnStarted;
        public UnityEvent<int> OnScoreChanged;
        public UnityEvent<int> OnCompleted;
        public UnityEvent<int> OnCompletedDelayed;


        public virtual void StartGame() => OnStarted.Invoke();

        public virtual void EndGame()
        {
            OnCompleted.Invoke(_score);
            Invoke(nameof(EmitDelayedCompletionEvent), _completionEventDelay);
        }

        public virtual void AddScore(int points) => Score += points;

        public virtual void ResetScore() => Score = 0;

        private void EmitDelayedCompletionEvent() => OnCompletedDelayed.Invoke(Score);
    }
}