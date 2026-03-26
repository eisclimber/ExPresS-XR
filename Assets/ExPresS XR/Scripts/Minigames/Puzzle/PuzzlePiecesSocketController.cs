using System.Collections.Generic;
using UnityEngine;
using ExPresSXR.Misc;
using ExPresSXR.Interaction.Interactors;

namespace ExPresSXR.Minigames.Puzzle
{
    /// <summary>
    /// Handles(re-)spawning pieces of the puzzle.
    /// </summary>
    public class PuzzlePiecesSocketController : MonoBehaviour
    {
        /// <summary>
        /// Reference the puzzle to handle the pieces from.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference the puzzle to handle the pieces from.")]
        private PuzzleGame _puzzle;

        /// <summary>
        /// Sockets to spawn pieces in.
        /// </summary>
        [SerializeField]
        [Tooltip("Sockets to spawn pieces in.")]
        private PutBackSocketInteractor[] _respawnSockets;

        /// <summary>
        /// If pieces should be spawned automatically on start.
        /// </summary>
        [SerializeField]
        [Tooltip("If pieces should be spawned automatically on start.")]
        private bool _spawnTilesOnStart;

        /// <summary>
        /// List of unsubmitted pieces that still can be spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("List of unsubmitted pieces that still can be spawned.")]
        private List<int> _unsubmittedPieces;


        /// <summary>
        /// List of index currently held in the `_respawnSockets`.
        /// </summary>
        [SerializeField]
        [Tooltip("List of index currently held in the `_respawnSockets`.")]
        [ReadonlyInInspector]
        private int[] _idxInSockets;

        /// <summary>
        /// Number of sockets to spawn pieces in.
        /// </summary>
        public int NumSpawnSockets
        {
            get => _respawnSockets.Length;
        }

        private void Awake()
        {
            if (_puzzle == null)
            {
                Debug.LogWarning("No puzzle provided for the spawner. Skipping setup.", this);
                return;
            }

            // Setup arrays
            _unsubmittedPieces = new();
            for (int i = 0; i < _puzzle.NumPieces; i++)
            {
                _unsubmittedPieces.Add(i);
            }

            _idxInSockets = new int[NumSpawnSockets];
            for (int i = 0; i < NumSpawnSockets; i++)
            {
                _idxInSockets[i] = -1;
            }

            // Setup submission
            _puzzle.OnPieceSubmitted.AddListener(HandlePieceSubmission);
        }

        private void Start()
        {
            if (_spawnTilesOnStart)
            {
                RerollSocketsTiles();
            }
        }

        private void SpawnNewRandomTile() => SpawnTile(PopRandomTile());

        private void SpawnTile(int idx)
        {
            if (idx < 0)
            {
                return;
            }

            for (int i = 0; i < NumSpawnSockets; i++)
            {
                // If _idxInSockets is free is equal to -1
                if (_idxInSockets[i] < 0)
                {
                    // Setup and register new spawned instance
                    _idxInSockets[i] = idx;
                    _puzzle.InstantiatePieceInSocket(idx, _respawnSockets[i]);
                    return;
                }
            }
            Debug.LogError($"Was not able to spawn the next piece with idx {idx}. There seems to be no free socket.", this);
        }


        private int PopRandomTile()
        {
            if (_unsubmittedPieces.Count <= 0)
            {
                return -1;
            }

            int unsubmittedIdx = Random.Range(0, _unsubmittedPieces.Count);
            int tileIdx = _unsubmittedPieces[unsubmittedIdx];
            _unsubmittedPieces.RemoveAt(unsubmittedIdx);
            return tileIdx;
        }

        /// <summary>
        /// Returns a tile to the pool of possible tiles to be spawned.
        /// </summary>
        /// <param name="idx"></param>
        public void ReturnTile(int idx)
        {
            if (idx < 0)
            {
                // No tile -> Do nothing
                return;
            }

            _unsubmittedPieces.Add(idx);

            for (int i = 0; i < NumSpawnSockets; i++)
            {
                if (_idxInSockets[i] == idx)
                {
                    _idxInSockets[i] = -1;
                    _respawnSockets[i].ForceClearPutBackInteractable();
                }
            }
        }

        /// <summary>
        /// Rerolls the tiles currently in the sockets.
        /// </summary>
        [ContextMenu("Reroll Socket Tiles")]
        public void RerollSocketsTiles()
        {
            // Clear sockets
            for (int i = 0; i < NumSpawnSockets; i++)
            {
                ReturnTile(_idxInSockets[i]);
            }

            // Refill sockets
            for (int i = 0; i < NumSpawnSockets; i++)
            {
                SpawnNewRandomTile();
            }
        }

        private void HandlePieceSubmission(int idx)
        {
            int nextTile = PopRandomTile();
            // Free submitted socket
            int freedSocketIdx = System.Array.IndexOf(_idxInSockets, idx);
            if (freedSocketIdx >= 0)
            {
                // Need to clean up the socket first
                _idxInSockets[freedSocketIdx] = -1;
                _respawnSockets[freedSocketIdx].PutBackPrefab = null;
            }
            SpawnTile(nextTile);
        }
    }
}