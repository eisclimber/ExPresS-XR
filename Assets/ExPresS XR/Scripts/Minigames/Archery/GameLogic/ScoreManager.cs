using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


namespace ExPresSXR.Minigames.Archery.GameLogic
{
    /// <summary>
    /// Implements the basic logic of an arrow game, starting the timer, registering the score managers and starting the spawners.  
    /// The score can be increase or decreased using the respective functions. To further handle how values are processed, you can choose how a score increase should be handled.
    /// This makes it possible to visualize hitting a target in multiple different ways simultaneously, i.e. count its points and count the hits.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        /// <summary>
        /// Unique set of ScoreManagers accessible from a global scope.
        /// To add a score manager, set '_registerAsGlobal' of the respective component to true.
        /// </summary>
        /// <returns></returns>
        public static readonly HashSet<ScoreManager> GlobalScoreManagers = new();

        /// <summary>
        /// Registers a ScoreManager to be globally accessible.
        /// </summary>
        /// <param name="manager">Score Manager to be registered.</param>
        public static void RegisterScoreMangerGlobally(ScoreManager manager)
        {
            if (!GlobalScoreManagers.Add(manager))
            {
                Debug.LogWarning($"Not registering ScoreManager '{manager}' globally, as it is.");
            }
        }

        /// <summary>
        /// Unregisters a ScoreManager from being globally accessible.
        /// </summary>
        /// <param name="manager">Score Manager to be registered.</param>
        public static void UnregisterScoreMangerGlobally(ScoreManager manager)
        {
            if (!GlobalScoreManagers.Remove(manager))
            {
                Debug.LogWarning($"Could not unregister ScoreManager '{manager}', as it is not registered globally.");
            }
        }

        /// <summary>
        /// Combines a singe ScoreManager with the GlobalScoreManagers, returning a list without duplicates.
        /// </summary>
        /// <param name="manager">ScoreManager to be combined with the global score managers.</param>
        public static ScoreManager[] MergeWithGlobalManagers(ScoreManager manager) => MergeWithGlobalManagers(new ScoreManager[] { manager });

        /// <summary>
        /// Combines ScoreManagers with the GlobalScoreManagers, returning a list without duplicates.
        /// </summary>
        /// <param name="manager">ScoreManagers to be combined with the global score managers.</param>
        public static ScoreManager[] MergeWithGlobalManagers(IEnumerable<ScoreManager> managers)
                => (managers != null ? GlobalScoreManagers.Union(managers) : GlobalScoreManagers).ToArray();


        /// <summary>
        /// Processes a raw value according to the rules of the provided ScoreDeltaHandling.
        /// </summary>
        /// <param name="scoreDelta">Raw score delta value.</param>
        /// <param name="handling">How the value should be processed.</param>
        /// <returns>Modified value according to ScoreDeltaHandling.</returns>
        public static int HandleScoreDelta(int scoreDelta, ScoreDeltaHandling handling)
        {
            return handling switch
            {
                ScoreDeltaHandling.Absolute => Mathf.Abs(scoreDelta),
                ScoreDeltaHandling.Count => 1,
                ScoreDeltaHandling.Ignore => 0,
                _ => scoreDelta,
            };
        }

        /// <summary>
        /// How fast the score in-/decreases per default.
        /// </summary>
        [SerializeField]
        [Tooltip("How fast the score in-/decreases per default.")]
        private int _stepSize = 1;

        /// <summary>
        /// Values are processed when the score is altered via AlterScore().
        /// </summary>
        [SerializeField]
        [Tooltip("Values are processed when the score is altered via AlterScore().")]
        private ScoreDeltaHandling _alterScoreHandling = ScoreDeltaHandling.Raw;

        /// <summary>
        /// Values are processed when the score is increased.
        /// </summary>
        [SerializeField]
        [Tooltip("Values are processed when the score is increased.")]
        private ScoreDeltaHandling _increaseHandling = ScoreDeltaHandling.Raw;

        /// <summary>
        /// Values are processed when the score is decreased.
        /// </summary>
        [SerializeField]
        [Tooltip("Values are processed when the score is decreased.")]
        private ScoreDeltaHandling _decreaseHandling = ScoreDeltaHandling.Raw;

        /// <summary>
        /// If the score can be negative.
        /// </summary>
        [SerializeField]
        [Tooltip("If the score can be negative.")]
        private bool _allowNegative;

        /// <summary>
        /// If enabled, the score manager will be globally accessible as part of the 'GlobalScoreManagers'.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled, the score manager will be globally accessible as part of the 'GlobalScoreManagers'.")]
        private bool _registerAsGlobal;

        /// <summary>
        /// Reference to the text displaying the score.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the text displaying the score.")]
        private TMP_Text _scoreText;

        /// <summary>
        /// Current score displayed.
        /// </summary>
        private int _score = 0;
        public int Score
        {
            get => _score;
            private set
            {
                _score = _allowNegative ? value : Mathf.Max(value, 0);

                if (_scoreText != null)
                {
                    _scoreText.text = Score.ToString();
                }
            }
        }

        private void OnEnable()
        {
            if (_registerAsGlobal)
            {
                RegisterScoreMangerGlobally(this);
            }
        }

        private void OnDisable()
        {
            if (_registerAsGlobal)
            {
                UnregisterScoreMangerGlobally(this);
            }
        }

        /// <summary>
        /// Increases or decreases the score by the default step size.
        /// </summary>
        /// <param name="increase">Wether to increase the score or not.</param>
        public void AlterScore(bool increase) => AlterScore(_stepSize, increase);

        /// <summary>
        /// Increases or decreases the score by the provided step size.
        /// </summary>
        /// <param name="points">The amount of points to change the score by.</param>
        /// <param name="increase">Wether to increase the score or not.</param>
        public void AlterScore(int points, bool increase) => Score += HandleScoreDelta(increase ? points : -points, _alterScoreHandling);

        /// <summary>
        /// Increase the score by the default amount.
        /// </summary>
        [ContextMenu("Increase Score")]
        public void IncreaseScore() => IncreaseScore(_stepSize);

        /// <summary>
        /// Increase the score by the provided amount.
        /// </summary>
        /// <param name="amount">The amount to increase the score by.</param>
        public void IncreaseScore(int amount) => Score += HandleScoreDelta(amount, _increaseHandling);


        /// <summary>
        /// Decrease the score by the default amount.
        /// </summary>
        [ContextMenu("Decrease Score")]
        public void DecreaseScore() => DecreaseScore(_stepSize);

        /// <summary>
        /// Decrease the score by the provided amount.
        /// </summary>
        /// <param name="amount">The amount to increase the score by.</param>
        public void DecreaseScore(int amount) => Score -= HandleScoreDelta(amount, _decreaseHandling);

        /// <summary>
        /// Resets the score back to zero.
        /// </summary>
        [ContextMenu("Reset Score")]
        public void ResetScore() => Score = 0;


        /// <summary>
        /// Describes how a score delta is handled. If the processed value is added/subtracted depends on the function called (increase vs decrease).
        /// - Raw: Use raw value, allowing negative deltas.
        /// - Absolute: Use absolute value, will always be positive.
        /// - Count: Use 1, disregarding the delta.
        /// - Ignore: Use 0
        /// </summary>
        public enum ScoreDeltaHandling
        {
            Raw, // Use raw value, allowing negative deltas.
            Absolute, // Use absolute value, will always be positive.
            Count, // Use 1, disregarding the delta.
            Ignore // Use 0
        }
    }
}