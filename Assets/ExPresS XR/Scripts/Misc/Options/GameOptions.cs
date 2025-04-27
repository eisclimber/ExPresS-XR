using UnityEngine;

namespace ExPresSXR.Misc.Options
{
    /// <summary>
    /// !ATTENTION! This is just a demo implementation with demo values. You'll need to adjust this class to your needs.
    /// Also keep in mind that this is not a full save system, rather lets you store some simple values easily.
    /// </summary>
    public class GameOptions : MonoBehaviour
    {
        #region Key Names & Constants
        /// <summary>
        /// Define the keys for storing the values here, making them public to be used elsewhere if needed. 
        /// </summary>
        public const string SELECTED_EXPERIENCE_PLAYER_PREF = "ExperienceType";
        public const string MADE_WITH_PLAYER_PREF = "MadeWith";
        public const string SUBTITLES_PLAYER_PREF = "Subtitles";

        /// <summary>
        /// Constant for checking if a game was made with ExPresS XR.
        /// </summary>
        public const string EXPRESSXR_MADE_WITH_NAME = "ExPresS XR";
        #endregion

        #region Default Values
        [SerializeField]
        [Tooltip("Categorizes the experience type.")]
        protected ExperienceType _defaultExperience = ExperienceType.None;

        [SerializeField]
        [Tooltip("With what this game was made with.")]
        protected string _defaultMadeWith = MADE_WITH_PLAYER_PREF;

        [SerializeField]
        [Tooltip("If subtitles are enabled or not.")]
        protected bool _defaultSubtitlesEnabled = true;

        [Space]

        [SerializeField]
        [Tooltip("Create missing values on awake.")]
        protected bool _createMissingValuesOnAwake = true;
        # endregion


        #region Getters and Setters
        /// <summary>
        /// Enum values must be converted to integers.
        /// </summary>
        public static int SelectedExperience
        {
            get => PlayerPrefs.HasKey(SELECTED_EXPERIENCE_PLAYER_PREF) ? PlayerPrefs.GetInt(SELECTED_EXPERIENCE_PLAYER_PREF) : 0;
            set => PlayerPrefs.SetInt(SELECTED_EXPERIENCE_PLAYER_PREF, value);
        }

        /// <summary>
        /// The actual enums can still be used in code like this.
        /// </summary>
        public static ExperienceType SelectedExperienceType
        {
            get => (ExperienceType)SelectedExperience;
            set => SelectedExperience = (int)value;
        }


        // ============================================

        /// <summary>
        /// Stores simple strings (floats and ints are possible too).
        /// </summary>
        public static string MadeWith
        {
            get => PlayerPrefs.HasKey(MADE_WITH_PLAYER_PREF) ? PlayerPrefs.GetString(MADE_WITH_PLAYER_PREF) : "";
            set => PlayerPrefs.SetString(MADE_WITH_PLAYER_PREF, value);
        }

        /// <summary>
        /// Values can be converted to booleans like this.
        /// </summary>
        /// <value></value>
        public static bool MadeWithExPresSXR
        {
            get => MadeWith == EXPRESSXR_MADE_WITH_NAME;
            set => MadeWith = EXPRESSXR_MADE_WITH_NAME;
        }

        // ============================================

        /// <summary>
        /// Storing booleans as 0 and 1 integers.
        /// </summary>
        public static bool SubtitlesEnabled
        {
            get => PlayerPrefs.HasKey(SUBTITLES_PLAYER_PREF) && PlayerPrefs.GetInt(SUBTITLES_PLAYER_PREF) != 0;
            set => PlayerPrefs.SetInt(SUBTITLES_PLAYER_PREF, value ? 1 : 0);
        }
        #endregion

        #region Basic Functionality
        /// <summary>
        /// Make sure to add(copy&paste) new values here to ensure correct functionality.
        /// </summary>
        protected virtual void Awake()
        {
            CreateMissingValues();

            if (PlayerPrefs.HasKey(SELECTED_EXPERIENCE_PLAYER_PREF))
            {
                Debug.Log($"Loaded value of '{SELECTED_EXPERIENCE_PLAYER_PREF}': {SelectedExperience}");
            }

            if (PlayerPrefs.HasKey(SUBTITLES_PLAYER_PREF))
            {
                Debug.Log($"Loaded value of '{SUBTITLES_PLAYER_PREF}': {SubtitlesEnabled}");
            }

            if (PlayerPrefs.HasKey(SUBTITLES_PLAYER_PREF))
            {
                Debug.Log($"Loaded value of '{SUBTITLES_PLAYER_PREF}': {SubtitlesEnabled}");
            }
        }

        protected virtual void CreateMissingValues()
        {
            if (!_createMissingValuesOnAwake)
            {
                return;
            }

            if (!PlayerPrefs.HasKey(SELECTED_EXPERIENCE_PLAYER_PREF))
            {
                Debug.LogWarning($"Did not find key '{SELECTED_EXPERIENCE_PLAYER_PREF}' in PlayerPrefs, creating entry with value '{_defaultExperience}'.");
                SelectedExperienceType = _defaultExperience;
            }

            if (!PlayerPrefs.HasKey(MADE_WITH_PLAYER_PREF))
            {
                Debug.LogWarning($"Did not find key '{MADE_WITH_PLAYER_PREF}' in PlayerPrefs, creating entry with value '{_defaultMadeWith}'.");
            }

            if (!PlayerPrefs.HasKey(SUBTITLES_PLAYER_PREF))
            {
                Debug.LogWarning($"Did not find key '{SUBTITLES_PLAYER_PREF}' in PlayerPrefs, creating entry with value '{_defaultSubtitlesEnabled}'.");
                SubtitlesEnabled = _defaultSubtitlesEnabled;
            }
        }

        /// <summary>
        /// Updates the values to default values. Use when changing the default values.
        /// </summary>
        [ContextMenu("Force Update Default Values")]
        protected virtual void ForceUpdateValues()
        {
            Debug.LogWarning($"Forcefully setting key '{SELECTED_EXPERIENCE_PLAYER_PREF}' in PlayerPrefs to '{_defaultExperience}'.");
            SelectedExperienceType = _defaultExperience;
            Debug.LogWarning($"Forcefully setting key '{MADE_WITH_PLAYER_PREF}' in PlayerPrefs to '{_defaultMadeWith}'.");
            MadeWith = _defaultMadeWith;
            Debug.LogWarning($"Forcefully setting key '{SUBTITLES_PLAYER_PREF}' in PlayerPrefs to '{_defaultSubtitlesEnabled}'.");
            SubtitlesEnabled = _defaultSubtitlesEnabled;
        }
        #endregion

        #region Custom GameOption Values
        public enum ExperienceType
        {
            None = 0,
            Exhibition = 1,
            Experience = 2
        }
        #endregion

        #region GameOptionConditionals
        /// <summary>
        /// Add boolean values here to be used with GameOptionsConditionalElement.
        /// Make sure to add a return value in GetValueOfConditional() too.
        /// </summary>
        public enum GameOptionConditionals
        {
            MadeWithExPresSXR,
            Subtitles
        }

        /// <summary>
        /// Returns the conditional value saved in the PlayerPrefs.
        /// </summary>
        /// <param name="conditional">Conditional value to check.</param>
        /// <returns>Wether or not the condition is true.</returns>
        public static bool GetValueOfConditional(GameOptionConditionals conditional)
        {
            return conditional switch
            {
                GameOptionConditionals.MadeWithExPresSXR => MadeWithExPresSXR,
                GameOptionConditionals.Subtitles => SubtitlesEnabled,
                _ => false
            };
        }
        #endregion
    }
}