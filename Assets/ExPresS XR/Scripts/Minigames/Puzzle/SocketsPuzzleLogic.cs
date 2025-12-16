using System;
using ExPresSXR.Interaction;
using ExPresSXR.Minigames.Common;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Puzzle
{
    public class SocketsPuzzleLogic : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _boardSize = new(4, 3);

        [SerializeField]
        private ObjectSubmitSocketInteractor[] _boardSockets;

        [SerializeField]
        private GameObject[] _piecePrefabs;

        [SerializeField]
        private GameObject _defaultPiece;

        [SerializeField]
        private GameObject _scoreDisplayPrefab;

        [SerializeField]
        private float _pointsDisplayUpOffset = 0.1f;

        [SerializeField]
        private float _pointsDisplayScale = 0.35f;

        [SerializeField]
        private float _completionDelay = 0.5f;

        [SerializeField]
        public string _speedBonusPrefix = "Speed Bonus: ";
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

        public int NumPieces
        {
            get => _boardSize.x * _boardSize.y;
        }

        public int NumUnsubmittedPieces
        {
            get => NumPieces - _numCompleted;
        }

        public UnityEvent<int> OnPieceSubmitted;
        public UnityEvent OnCompleted;

        private void Awake()
        {
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

        private void HandlePieceSubmission(int idx)
        {
            if (idx >= 0 && idx < NumPieces && !_pieceSubmitted[idx])
            {
                _numCompleted++;
                _pieceSubmitted[idx] = true;

                OnPieceSubmitted.Invoke(idx);
                if (_numCompleted == NumPieces)
                {
                    Invoke(nameof(NotifyCompletion), _completionDelay);
                }
            }
            else
            {
                Debug.LogError("Failed to handle submission. Either the idx was invalid or it was already submitted.", this);
            }
        }

        public void DisplaySocketScore(int idx, int score, int bonus)
        {
            if (_scoreDisplayPrefab == null)
            {
                Debug.LogError("No score display set, can't spawn a score.");
            }

            if (idx < 0 && idx >= NumPieces)
            {
                Debug.LogError("Invalid socket idx, ignoring.");
                return;
            }

            Vector3 socketWorldPos = _boardSockets[idx].transform.position + _boardSockets[idx].transform.up * _pointsDisplayUpOffset;
            Vector3 socketLocalPos = transform.worldToLocalMatrix * socketWorldPos;
            GameObject scoreDisplayInstance = Instantiate(_scoreDisplayPrefab, socketLocalPos, Quaternion.identity, transform);
            scoreDisplayInstance.transform.localScale = Vector3.one * _pointsDisplayScale;
            if (scoreDisplayInstance.TryGetComponent(out ScoreNumbers scoreNumbers))
            {
                scoreNumbers.SetupScore(score, bonus, "", _speedBonusPrefix);
            }
        }

        public void InstantiatePieceInSocket(int idx, PutBackSocketInteractor socket)
        {
            GameObject prefab = GetUnsubmittedPieceAtOrDefaultPrefab(idx);

            if (prefab != null)
            {
                Vector2Int boardPos = IdxToBoardPos(idx);
                socket.PutBackPrefab = prefab;

                if (socket.PutBackInteractable is UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable piece)
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

        public GameObject GetUnsubmittedPieceAtOrDefaultPrefab(int idx)
        {
            if (idx >= 0 && idx < NumPieces)
            {
                return _piecePrefabs[idx] != null ? _piecePrefabs[idx] : _defaultPiece;
            }
            return null;
        }

        public Vector2Int IdxToBoardPos(int idx) => new(idx % _boardSize.x, idx / _boardSize.x);

        public void ResetPuzzle()
        {
            _pieceSubmitted = new bool[NumPieces];
            _numCompleted = 0;
        }

        [ContextMenu("Complete next tile")]
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

        [ContextMenu("Emit Completion Event")]
        private void NotifyCompletion() => OnCompleted.Invoke();

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