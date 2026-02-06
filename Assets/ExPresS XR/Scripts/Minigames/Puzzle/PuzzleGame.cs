using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Minigames.Common;
using ExPresSXR.Misc;

namespace ExPresSXR.Minigames.Puzzle
{
    public class PuzzleGame : MonoBehaviour
    {
        public const int DEFAULT_FIELD_WIDTH = 4;
        public const int DEFAULT_FIELD_HEIGHT = 3;


        [SerializeField]
        private Vector2Int _boardSize = new(DEFAULT_FIELD_WIDTH, DEFAULT_FIELD_HEIGHT);

        [SerializeField]
        private ObjectSubmitSocketInteractor[] _boardSockets;

        [SerializeField]
        private GameObject[] _piecePrefabs;
        
        [Space]

        [SerializeField]
        private bool _autoStart;

        [Space]

        [SerializeField]
        private GameObject _scoreNumbersPrefab;

        [SerializeField]
        private Vector3 _scoreNumbersOffset = new(0.0f, 0.1f, -0.1f);

        [SerializeField]
        private float _scoreNumbersScale = 0.35f;

        [Space]

        [SerializeField]
        private int _pieceScore = 100;

        [SerializeField]
        private float _speedBonusThreshold = 1.0f;

        [SerializeField]
        private int _speedBonus = 25;

        [SerializeField]
        private string _speedBonusPrefix = "Speed Bonus: ";
        public string SpeedBonusPrefix
        {
            get => _speedBonusPrefix;
            set => _speedBonusPrefix = value;
        }

        [Space]

        [SerializeField]
        [ReadonlyInInspector]
        private int _numCompleted;
        public int NumCompleted
        {
            get => _numCompleted;
        }

        [SerializeField]
        [ReadonlyInInspector]
        private bool[] _pieceSubmitted;
        public bool[] PieceSubmitted
        {
            get => _pieceSubmitted;
        }

        [SerializeField]
        [ReadonlyInInspector]
        private int _currentScore;
        public int CurrentScore
        {
            get => _currentScore;
            protected set
            {
                _currentScore = value;
                OnScoreChanged.Invoke(_currentScore);
            }
        }

        public int NumPieces
        {
            get => _boardSize.x * _boardSize.y;
        }

        public int NumUnsubmittedPieces
        {
            get => NumPieces - _numCompleted;
        }

        private float _lastSubmissionTime;


        public UnityEvent OnStarted;
        public UnityEvent<int> OnPieceSubmitted;
        public UnityEvent<int> OnScoreChanged;
        public UnityEvent OnCompleted;

        private void Awake()
        {
            if (_piecePrefabs.Length <= 0)
            {
                Debug.LogError($"No PiecePrefabs and no default prefab provided. Can not spawn any pieces...", this);
            }

            if (_boardSockets.Length != NumPieces)
            {
                Debug.LogWarning($"Number of board sockets ({_boardSockets.Length}) is not equal to number derived of the board size ({NumPieces}). Resizing...", this);
                Array.Resize(ref _boardSockets, NumPieces);
            }

            if (_piecePrefabs.Length != NumPieces)
            {
                Debug.LogWarning($"Number of pieces ({_piecePrefabs.Length}) is not equal to number derived of the board size ({NumPieces}). Resizing...", this);
                Array.Resize(ref _piecePrefabs, NumPieces);
            }
            _pieceSubmitted = new bool[NumPieces];

            ResetPuzzle();

            for (int i = 0; i < NumPieces; i++)
            {
                int x = i; // Redefine it here to prevent the index being messed up
                _boardSockets[i].OnSubmitted.AddListener(() => HandlePieceSubmission(x));
            }
        }

        protected void Start()
        {
            if (_autoStart)
            {
                StartGame();
            }
        }

        [ContextMenu("Start Game")]
        public void StartGame()
        {
            ResetPuzzle();
            _lastSubmissionTime = Time.time;
            OnStarted.Invoke();
        }

        [ContextMenu("End Game")]
        public void EndGame()
        {
            OnCompleted.Invoke();
        }

        private void HandlePieceSubmission(int idx)
        {
            if (idx >= 0 && idx < NumPieces && !_pieceSubmitted[idx])
            {
                _numCompleted++;
                _pieceSubmitted[idx] = true;

                int bonus = CalculateSubmissionBonusScore();
                CurrentScore += _pieceScore + bonus;
                DisplaySocketScore(idx, _pieceScore, bonus);

                OnPieceSubmitted.Invoke(idx);
                if (_numCompleted == NumPieces)
                {
                    EndGame();
                }
            }
            else
            {
                Debug.LogError("Failed to handle submission. Either the idx was invalid or it was already submitted.", this);
            }
        }

        protected int CalculateSubmissionBonusScore()
        {
            float submissionSpeed = _lastSubmissionTime - Time.time;
            _lastSubmissionTime = Time.time;
            return submissionSpeed <= _speedBonusThreshold ? _speedBonus : 0;
        }

        public void DisplaySocketScore(int idx, int score, int bonus)
        {
            if (_scoreNumbersPrefab == null)
            {
                Debug.LogError("No score display set, can't spawn a score.");
            }

            if (idx < 0 && idx >= NumPieces)
            {
                Debug.LogError("Invalid socket idx, ignoring...");
                return;
            }

            Vector3 socketWorldPos = _boardSockets[idx].transform.position + _scoreNumbersOffset;
            Vector3 socketLocalPos = transform.worldToLocalMatrix * socketWorldPos;
            GameObject scoreDisplayInstance = Instantiate(_scoreNumbersPrefab, socketLocalPos, Quaternion.identity, transform);
            scoreDisplayInstance.transform.localScale = Vector3.one * _scoreNumbersScale;
            if (scoreDisplayInstance.TryGetComponent(out ScoreNumbers scoreNumbers))
            {
                scoreNumbers.SetupScore(score, bonus, "", bonus > 0 ? _speedBonusPrefix : "");
            }
        }

        public void InstantiatePieceInSocket(int idx, PutBackSocketInteractor socket)
        {
            GameObject prefab = GetUnsubmittedPieceAt(idx);
            if (prefab != null)
            {
                Vector2Int boardPos = IdxToBoardPos(idx);
                socket.PutBackPrefab = prefab;

                if (socket.PutBackInteractable is XRGrabInteractable piece)
                {
                    _boardSockets[idx].TargetObject = piece;
                    if (piece.TryGetComponent(out PuzzlePiece display))
                    {
                        display.PuzzlePosition = boardPos;
                    }
                }
                else
                {
                    Debug.LogError("Failed to instantiate puzzle piece as XRGrabInteractable.", this);
                }
            }
            else
            {
                Debug.LogError($"Failed to create instantiate prefab for idx: {idx}.", this);
            }
        }

        public GameObject GetUnsubmittedPieceAt(int idx) => idx >= 0 && idx < NumPieces ? _piecePrefabs[idx] : null;

        public Vector2Int IdxToBoardPos(int idx) => new(idx % _boardSize.x, idx / _boardSize.x);

        public void ResetPuzzle()
        {
            _pieceSubmitted = new bool[NumPieces];
            _numCompleted = 0;
        }

        [ContextMenu("Complete Next Tile")]
        private void InternalIncreaseCompleted()
        {
            if (_numCompleted >= NumPieces)
            {
                Debug.LogWarning("Already all completed.", this);
            }

            for (int i = 0; i < NumPieces; i++)
            {
                if (!_pieceSubmitted[i])
                {
                    Debug.Log($"Manual completion of tile with idx {i}.", this);
                    HandlePieceSubmission(i);
                    return;
                }
            }
            Debug.LogError("Failed to find uncompleted tile...", this);
        }
        
        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < NumPieces; i++)
            {
                // Skip if board socket not set
                if (_boardSockets[i] == null)
                {
                    continue;
                }

                Vector2Int pos = IdxToBoardPos(i);
                Color completionColor = _boardSockets[i].hasSelection ? Color.green : Color.white;
                GizmoUtils.DrawLabel(pos.ToString(), Vector3.zero, completionColor, _boardSockets[i].transform);
            }
        }
    }
}