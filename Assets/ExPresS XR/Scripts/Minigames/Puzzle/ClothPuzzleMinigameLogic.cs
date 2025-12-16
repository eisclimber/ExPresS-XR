using System.Collections;
using ExPresSXR.Minigames.Common;
using ExPresSXR.Misc;
using ExPresSXR.Misc.Timing;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Puzzle
{
    public class ClothPuzzleMinigameLogic : MinigameLogic
    {
        [SerializeField]
        private SocketsPuzzleLogic[] _puzzles;


        [SerializeField]
        [ReadonlyInInspector]
        private int _currentPuzzleIdx = -1;
        public int CurrentPuzzleIdx
        {
            get => _currentPuzzleIdx;
            set
            {
                bool changed = _currentPuzzleIdx != value;
                _lastTileSubmissionTime = Time.time; // Reset tile submission time
                if (!changed)
                {
                    return;
                }

                _currentPuzzleIdx = value;
                CurrentPuzzle = _currentPuzzleIdx >= 0 && _currentPuzzleIdx < NumPuzzles ? _puzzles[_currentPuzzleIdx] : null;

                if (_currentPuzzleIdx == NumPuzzles)
                {
                    EndGame();
                }
                else
                {
                    (_currentPuzzleIdx == 0 ? OnFirstPuzzleShown : OnPuzzleCompleted).Invoke();
                }
            }
        }

        [ReadonlyInInspector]
        public SocketsPuzzleLogic _currentPuzzle;
        public SocketsPuzzleLogic CurrentPuzzle
        {
            get => _currentPuzzle;
            private set
            {
                UnregisterPuzzleLogic(true);
                _currentPuzzle = value;
                RegisterPuzzleLogic(true);
            }
        }

        [Space]

        [SerializeField]
        private float _maxGameTime = 60.0f;

        [SerializeField]
        private float _puzzleSwitchDelay = 0.5f;

        [SerializeField]
        private Timer _gameTimer;

        [Space]

        [SerializeField]
        private Transform _puzzleBonusLocation;

        [SerializeField]
        private GameObject _timeBonusPrefab;


        [Space]

        [SerializeField]
        [TextArea(2, 5)]
        private string _puzzleScorePrefix = "Completion Bonus:\n";
        public string PuzzleScorePrefix
        {
            get => _puzzleScorePrefix;
            set => _puzzleScorePrefix = value;
        }

        [SerializeField]
        private string _completionBonusPrefix = "Time Bonus: ";
        public string CompletionBonusPrefix
        {
            get => _completionBonusPrefix;
            set => _completionBonusPrefix = value;
        }

        [Space]

        [SerializeField]
        private ClothPuzzleScoreCalculator _scoreCalculator;

        public int NumPuzzles
        {
            get => _puzzles.Length;
        }


        private float _lastTileSubmissionTime;
        private Coroutine _puzzleSwitchDelayCoroutine;


        public UnityEvent OnFirstPuzzleShown;
        public UnityEvent OnPuzzleCompleted;
        public UnityEvent OnNewPuzzle; // Delayed by _puzzleSwitchDelay after OnPuzzleCompleted

        public UnityEvent OnAllPuzzlesCompleted;


        private void OnEnable()
        {
            RegisterPuzzleLogic(false);

            if (_gameTimer != null)
            {
                _gameTimer.OnTimeout.AddListener(EndGame);
            }
        }

        private void OnDisable()
        {
            UnregisterPuzzleLogic(false);

            if (_gameTimer != null)
            {
                _gameTimer.OnTimeout.RemoveListener(EndGame);
            }
        }

        public override void StartGame()
        {
            CurrentPuzzleIdx = 0;
            base.StartGame();
            _gameTimer.StartTimer(_maxGameTime);
        }

        public override void EndGame()
        {
            _gameTimer.StopTimer();

            StartCoroutine(SwitchRugsDelayed(-1)); // Hide rug delayed

            _gameTimer.StopTimer(); // just to be sure:)
            base.EndGame();
        }

        public void HandlePuzzleCompletion()
        {
            if (_puzzleSwitchDelayCoroutine != null)
            {
                StopCoroutine(_puzzleSwitchDelayCoroutine);
            }

            bool lastPuzzle = _puzzles.Length > 0 ? _currentPuzzle == _puzzles[_puzzles.Length - 1] : false;
            float remainingTime = _gameTimer.RemainingTime; // A bit sketchy, but should not be a race condition... I think...
            ClothPuzzleScoreCalculator.CompositePoints score = lastPuzzle ? _scoreCalculator.CalculateAllCompletionBonus(remainingTime, _maxGameTime) : _scoreCalculator.CalculatePuzzleCompletionBonus();
            _puzzleSwitchDelayCoroutine = StartCoroutine(SwitchRugsDelayed(CurrentPuzzleIdx + 1, score));
        }


        private void SpawnPuzzleCompletionScore(int score, int bonus, string scorePrefix = "", string bonusPrefix = "")
        {
            // Debug.Log("Spawning time bonus instance but no instance set: " + score + " and " + bonus);
            if (_timeBonusPrefab == null)
            {
                return;
            }

            GameObject scoreDisplayInstance = Instantiate(_timeBonusPrefab, _puzzleBonusLocation);
            if (scoreDisplayInstance.TryGetComponent(out ScoreNumbers scoreNumbers))
            {
                scoreNumbers.SetupScore(score, bonus, scorePrefix, bonusPrefix);
            }
        }

        public void AddPieceScore(int idx)
        {
            float currentTileSubmissionTime = Time.time;
            ClothPuzzleScoreCalculator.CompositePoints pieceScore = _scoreCalculator.CalculateTilePlacementScore(currentTileSubmissionTime, _lastTileSubmissionTime);
            _lastTileSubmissionTime = currentTileSubmissionTime;
            _currentPuzzle.DisplaySocketScore(idx, pieceScore.BasePoints, pieceScore.TimeBonus);
            AddScore(pieceScore.TotalPoints);
        }

        private void RegisterPuzzleLogic(bool handleActive)
        {
            if (_currentPuzzle != null)
            {
                if (handleActive)
                {
                    _currentPuzzle.gameObject.SetActive(true);
                }
                _currentPuzzle.OnPieceSubmitted.AddListener(AddPieceScore);
                _currentPuzzle.OnCompleted.AddListener(HandlePuzzleCompletion);
            }
        }

        private void UnregisterPuzzleLogic(bool handleActive)
        {
            if (_currentPuzzle != null)
            {
                if (handleActive)
                {
                    _currentPuzzle.gameObject.SetActive(false);
                }
                _currentPuzzle.OnPieceSubmitted.RemoveListener(AddPieceScore);
                _currentPuzzle.OnCompleted.RemoveListener(HandlePuzzleCompletion);
            }
        }

        public void ShowFirstPuzzle()
        {
            if (_puzzleSwitchDelayCoroutine != null)
            {
                StopCoroutine(_puzzleSwitchDelayCoroutine);
            }
            _puzzleSwitchDelayCoroutine = StartCoroutine(SwitchRugsDelayed(0));
        }

        private IEnumerator SwitchRugsDelayed(int puzzleIdx, ClothPuzzleScoreCalculator.CompositePoints score = null)
        {
            yield return new WaitForSeconds(_puzzleSwitchDelay);

            if (score != null)
            {
                AddScore(score.TotalPoints);
                SpawnPuzzleCompletionScore(score.BasePoints, score.TimeBonus, _puzzleScorePrefix, _completionBonusPrefix);
            }

            CurrentPuzzleIdx = puzzleIdx;
        }
    }
}