using System.Collections.Generic;
using ExPresSXR.Minigames.Archery.GameLogic;
using UnityEngine;

/// <summary>
/// Abstract class representing a spawner for the archery game logic providing an interface for spawning
/// and passing the ScoreManagers to the spawned targets.
/// </summary>
namespace ExPresSXR.Minigames.Archery.TargetSpawner
{
    public abstract class TargetSpawnerBase : MonoBehaviour
    {
        /// <summary>
        /// A unique set of ScoreManagers that get registered with new targets. These will be combined with the GlobalScoreManagers automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("ScoreManagers that get registered with new targets. These will be combined with the GlobalScoreManagers automatically.")]
        private HashSet<ScoreManager> _scoreManagers = new();

        /// <summary>
        /// Starts spawning new target(s).
        /// </summary>
        public abstract void StartSpawning();

        /// <summary>
        /// Stops spawning target(s).
        /// </summary>
        public abstract void StopSpawning();

        /// <summary>
        /// Creates a new target.
        /// </summary>
        public abstract void CreateNewTarget();

        /// <summary>
        /// Adds another score manager. There can be multiple, but duplicates will be filtered.
        /// </summary>
        /// <param name="scoreManager">ScoreManager to be added.</param>
        public virtual void AddScoreManager(ScoreManager scoreManager)
        {
            if (!_scoreManagers.Contains(scoreManager))
            {
                _scoreManagers.Add(scoreManager);
            }
        }

        /// <summary>
        /// Removes a score manager.
        /// </summary>
        /// <param name="scoreManager">ScoreManager to be removed.</param>
        public virtual void RemoveScoreManager(ScoreManager scoreManager) => _scoreManagers.Remove(scoreManager);

        /// <summary>
        /// Returns an array of the ScoreManagers of this Spawner combined with the global ScoreManagers.
        /// </summary>
        /// <returns></returns>
        protected virtual ScoreManager[] GetAllScoreManagers() => ScoreManager.MergeWithGlobalManagers(_scoreManagers);

        /// <summary>
        /// Tries to find a Target-Component from a collision, also taking TargetProxies into account.
        /// </summary>
        /// <param name="collision">Collision to be processed.</param>
        /// <returns>A Target or null if not found.</returns>
        protected virtual Target FindTargetComponentFromCollision(Collision collision) => FindTargetComponent(collision.gameObject);

        /// <summary>
        /// Tries to find a Target-Component from a GameObject, also taking TargetProxies into account.
        /// </summary>
        /// <param name="go">GameObject to be processed.</param>
        protected virtual Target FindTargetComponent(GameObject go)
        {
            if (go == null)
            {
                return null;
            }

            if (go.TryGetComponent(out Target target))
            {
                return target;
            }
            else if (go.TryGetComponent(out TargetProxy proxy))
            {
                return proxy.Target;
            }
            return null;
        }
    }
}