using System.Collections.Generic;
using System.Linq;
using ExPresSXR.Experimentation.DataGathering;
using ExPresSXR.Interaction;
using ExPresSXR.Misc;
using UnityEngine;

namespace ExPresSXR.Minigames.Puzzle
{
    public class PuzzleSpawner : MonoBehaviour
    {
        [SerializeField]
        private SocketsPuzzleLogic _puzzle;

        [SerializeField]
        private PutBackSocketInteractor[] _respawnSockets;

        [SerializeField]
        private bool _spawnTilesOnStart;

        [SerializeField]
        private List<int> _unsubmittedPieces;

        [SerializeField]
        [ReadonlyInInspector]
        private int[] _idxInSockets;

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

            // Spawn first tiles
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
                // _idxInSockets is free is equal to -1
                if (_idxInSockets[i] < 0)
                {
                    // Setup and register new spawned instance
                    _idxInSockets[i] = idx;
                    _puzzle.InstantiatePieceInSocket(idx, _respawnSockets[i]);
                    return;
                }
            }
            Debug.LogError($"Was not able to spawn the idx {idx}. There seems to be no free socket.", this);
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