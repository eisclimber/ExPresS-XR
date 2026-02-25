using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Minigames.Common;
using ExPresSXR.Misc;

namespace ExPresSXR.Minigames.Puzzle
{
    /// <summary>
    /// The main logic of a socket-based puzzle game.
    /// </summary>
    public class PuzzleGame : MonoBehaviour
    {
        /// <summary>
        /// Default width of the puzzle (in tiles).
        /// </summary>
        public const int DEFAULT_FIELD_WIDTH = 4;

        /// <summary>
        /// Default height of the puzzle (in tiles).
        /// </summary>
        public const int DEFAULT_FIELD_HEIGHT = 3;

        /// <summary>
        /// Dimensions of the board (in tiles).
        /// This is mainly for creating an index. If you puzzle is not based on tiles, just set a width according to the number of pieces and a height of 1.
        /// </summary>
        [SerializeField]
        [Tooltip("Dimensions of the board (in tiles).\nsThis is mainly for creating an index. If you puzzle is not based on tiles, just set a width according to the number of pieces and a height of 1.")]
        private Vector2Int _boardSize = new(DEFAULT_FIELD_WIDTH, DEFAULT_FIELD_HEIGHT);

        /// <summary>
        /// List of the sockets of the puzzle. Must match `NumPieces`.
        /// </summary>
        [SerializeField]
        [Tooltip("List of the sockets of the puzzle. Must match `NumPieces`.")]
        private ObjectSubmitSocketInteractor[] _boardSockets;

        /// <summary>
        /// List of the pieces of the puzzle. Must match `NumPieces`.
        /// </summary>
        [SerializeField]
        [Tooltip("List of the pieces of the puzzle. Must match `NumPieces`.")]
        private GameObject[] _piecePrefabs;

        [Space]

        /// <summary>
        /// If the game should start automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("If the game should start automatically.")]
        private bool _autoStart;

        [Space]

        /// <summary>
        /// Prefab (i.e. ScoreNumbers) to be spawned when scoring.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab (i.e. ScoreNumbers) to be spawned when scoring.")]
        private GameObject _scoreNumbersPrefab;

        /// <summary>
        /// Offset with which the `_scoreNumbersPrefab` is spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Offset with which the `_scoreNumbersPrefab` is spawned.")]
        private Vector3 _scoreNumbersOffset = new(0.0f, 0.1f, -0.1f);

        /// <summary>
        /// Scale with which the `_scoreNumbersPrefab` is spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Scale with which the `_scoreNumbersPrefab` is spawned.")]
        private float _scoreNumbersScale = 0.35f;

        [Space]

        /// <summary>
        /// Points granted for submitting a piece.
        /// </summary>
        [SerializeField]
        [Tooltip("Points granted for submitting a piece.")]
        private int _pieceScore = 100;

        /// <summary>
        /// Grants bonus points when the next tile is submitted within this time after the previous one.
        /// </summary>
        [SerializeField]
        [Tooltip("Grants bonus points when the next tile is submitted within this time after the previous one.")]
        private float _speedBonusThreshold = 1.0f;

        /// <summary>
        /// Bonus points granted when submitting the next tile within `_speedBonusThreshold`.
        /// </summary>
        [SerializeField]
        [Tooltip("Bonus points granted when submitting the next tile within `_speedBonusThreshold`.")]
        private int _speedBonus = 25;

        /// <summary>
        /// Bonus prefix added to the ScoreNumbers when a speed bonus is granted.
        /// </summary>
        [SerializeField]
        [Tooltip("Bonus prefix added to the ScoreNumbers when a speed bonus is granted.")]
        private string _speedBonusPrefix = "Speed Bonus: ";
        public string SpeedBonusPrefix
        {
            get => _speedBonusPrefix;
            set => _speedBonusPrefix = value;
        }

        [Space]

        /// <summary>
        /// Current number of submitted pieces.
        /// </summary>
        [SerializeField]
        [ReadonlyInInspector]
        [Tooltip("Current number of submitted pieces.")]
        private int _numCompleted;
        public int NumCompleted
        {
            get => _numCompleted;
        }

        /// <summary>
        /// Boolean list which pieces were submitted.
        /// </summary>
        [SerializeField]
        [ReadonlyInInspector]
        [Tooltip("Boolean list which pieces were submitted.")]
        private bool[] _pieceSubmitted;
        public bool[] PieceSubmitted
        {
            get => _pieceSubmitted;
        }

        /// <summary>
        /// The current score of the puzzle.
        /// </summary>
        [SerializeField]
        [ReadonlyInInspector]
        [Tooltip("The current score of the puzzle.")]
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

        /// <summary>
        /// Number of pieces in the puzzle.
        /// </summary>
        public int NumPieces
        {
            get => _boardSize.x * _boardSize.y;
        }

        /// <summary>
        /// Number of unsubmitted pieces.
        /// </summary>
        public int NumUnsubmittedPieces
        {
            get => NumPieces - _numCompleted;
        }

        private float _lastSubmissionTime;


        /// <summary>
        /// Emitted when the puzzle was started.
        /// </summary>
        public UnityEvent OnStarted;

        /// <summary>
        /// Emitted when a piece is submitted, providing its index.
        /// </summary>
        public UnityEvent<int> OnPieceSubmitted;

        /// <summary>
        /// Emitted when the score changes (i.e. a piece is submitted), providing the score.
        /// </summary>
        public UnityEvent<int> OnScoreChanged;

        /// <summary>
        /// Emitted when the puzzle was completed.
        /// </summary>
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

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected void Start()
        {
            if (_autoStart)
            {
                StartGame();
            }
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        [ContextMenu("Start Game")]
        public void StartGame()
        {
            ResetPuzzle();
            _lastSubmissionTime = Time.time;
            OnStarted.Invoke();
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
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

        /// <summary>
        /// Calculates the bonus score for submitting a piece.
        /// </summary>
        protected int CalculateSubmissionBonusScore()
        {
            float submissionSpeed = _lastSubmissionTime - Time.time;
            _lastSubmissionTime = Time.time;
            return submissionSpeed <= _speedBonusThreshold ? _speedBonus : 0;
        }

        private void DisplaySocketScore(int idx, int score, int bonus)
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
                scoreNumbers.SetupScoreData(score, bonus, "", bonus > 0 ? _speedBonusPrefix : "");
            }
        }

        /// <summary>
        /// Instantiates the puzzle piece with the provided id in the socket.
        /// </summary>
        /// <param name="idx">Puzzle piece idx to spawn.</param>
        /// <param name="socket">Socket to spawn the piece in.</param>
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

        /// <summary>
        /// Returns the piece prefab if the provided index is valid and the piece was unsubmitted.
        /// </summary>
        /// <param name="idx">Idx of the piece.</param>
        /// <returns>Piece Prefab or null.</returns>
        public GameObject GetUnsubmittedPieceAt(int idx) => idx >= 0 && idx < NumPieces ? _piecePrefabs[idx] : null;

        /// <summary>
        /// Converts a puzzle piece idx to a board position (Vector2).
        /// </summary>
        /// <param name="idx">Idx to convert.</param>
        /// <returns>Vector2 representing the idx on the board.</returns>
        public Vector2Int IdxToBoardPos(int idx) => new(idx % _boardSize.x, idx / _boardSize.x);

        /// <summary>
        /// Resets the progress of the puzzle. Pieces must be cleaned up manually.
        /// </summary>
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