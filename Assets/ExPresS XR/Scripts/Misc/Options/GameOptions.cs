using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Options
{
    public class GameOptions : MonoBehaviour
    {
        // Requires an instance to modify the values, but the values can be read from a script
        public const string SELECTED_EXPERIENCE = "ExperienceType";
        public const string SUBTITLES_PLAYER_PREF = "Subtitles";
        public const string RAY_INTERACTIONS_PLAYER_PREF = "RayInteractions";
        public const string SKIP_LADDER_PLAYER_PREF = "SkipLadder";
        public const string SKIP_ELEVATOR_PLAYER_PREF = "SkipElevator";
        public const string NO_HEAT_HAZE_PLAYER_PREFS = "NoHeatHaze";

        [SerializeField]
        private bool _createMissingValuesOnAwake = true;


        [SerializeField]
        private ExperienceType _defaultExperience = ExperienceType.Full;

        [SerializeField]
        private bool _defaultSubtitlesEnabled = true;

        [SerializeField]
        private bool _defaultRayInteractionsEnabled = false;

        [SerializeField]
        private bool _defaultSkipLadderEnabled = false;

        [SerializeField]
        private bool _defaultSkipElevatorEnabled = false;

        [SerializeField]
        private bool _defaultNoHeatHazeEnabled = false;


        public static int SelectedExperience
        {
            get => PlayerPrefs.HasKey(SELECTED_EXPERIENCE) ? PlayerPrefs.GetInt(SELECTED_EXPERIENCE) : 0;
        }
        public static ExperienceType SelectedExperienceType
        {
            get => (ExperienceType)SelectedExperience;
        }

        public static bool SubtitlesEnabled
        {
            get => PlayerPrefs.HasKey(SUBTITLES_PLAYER_PREF) && PlayerPrefs.GetInt(SUBTITLES_PLAYER_PREF) != 0;
        }

        public static bool RayInteractionsEnabled
        {
            get => PlayerPrefs.HasKey(RAY_INTERACTIONS_PLAYER_PREF) && PlayerPrefs.GetInt(RAY_INTERACTIONS_PLAYER_PREF) != 0;
        }

        public static bool SkipLadderEnabled
        {
            get => PlayerPrefs.HasKey(SKIP_LADDER_PLAYER_PREF) && PlayerPrefs.GetInt(SKIP_LADDER_PLAYER_PREF) != 0;
        }


        public static bool SkipElevatorEnabled
        {
            get => PlayerPrefs.HasKey(SKIP_ELEVATOR_PLAYER_PREF) && PlayerPrefs.GetInt(SKIP_ELEVATOR_PLAYER_PREF) != 0;
        }


        public static bool NoHeatHazeEnabled
        {
            get => PlayerPrefs.HasKey(NO_HEAT_HAZE_PLAYER_PREFS) && PlayerPrefs.GetInt(NO_HEAT_HAZE_PLAYER_PREFS) != 0;
        }


        public static bool DEBUG_AutoSkipNarration
        {
            get => false;
        }


        public UnityEvent<int> OnExperienceSelected;
        public UnityEvent<bool> OnSubtitlesChanged;
        public UnityEvent<bool> OnRayInteractionsChanged;
        public UnityEvent<bool> OnSkipLadderChanged;
        public UnityEvent<bool> OnSkipElevatorChanged;
        public UnityEvent<bool> OnNoHeatHazeChanged;


        private void Awake()
        {
            CreateMissingValues();

            if (PlayerPrefs.HasKey(SELECTED_EXPERIENCE))
            {
                Debug.Log($"Loaded value of '{SELECTED_EXPERIENCE}': {SelectedExperience}");
                OnExperienceSelected.Invoke(SelectedExperience);
            }

            if (PlayerPrefs.HasKey(SUBTITLES_PLAYER_PREF))
            {
                Debug.Log($"Loaded value of '{SUBTITLES_PLAYER_PREF}': {SubtitlesEnabled}");
                OnSubtitlesChanged.Invoke(SubtitlesEnabled);
            }

            if (PlayerPrefs.HasKey(RAY_INTERACTIONS_PLAYER_PREF))
            {
                Debug.Log($"Loaded value of '{RAY_INTERACTIONS_PLAYER_PREF}': {RayInteractionsEnabled}");
                OnRayInteractionsChanged.Invoke(RayInteractionsEnabled);
            }

            if (PlayerPrefs.HasKey(SKIP_LADDER_PLAYER_PREF))
            {
                Debug.Log($"Loaded value of '{SKIP_LADDER_PLAYER_PREF}': {SkipLadderEnabled}");
                OnSkipLadderChanged.Invoke(SkipLadderEnabled);
            }

            if (PlayerPrefs.HasKey(SKIP_ELEVATOR_PLAYER_PREF))
            {
                Debug.Log($"Loaded value of '{SKIP_ELEVATOR_PLAYER_PREF}': {SkipElevatorEnabled}");
                OnSkipElevatorChanged.Invoke(SkipElevatorEnabled);
            }

            if (PlayerPrefs.HasKey(NO_HEAT_HAZE_PLAYER_PREFS))
            {
                Debug.Log($"Loaded value of '{NO_HEAT_HAZE_PLAYER_PREFS}': {NoHeatHazeEnabled}");
                OnNoHeatHazeChanged.Invoke(NoHeatHazeEnabled);
            }
        }

        private void CreateMissingValues()
        {
            if (!_createMissingValuesOnAwake)
            {
                return;
            }

            if (!PlayerPrefs.HasKey(SELECTED_EXPERIENCE))
            {
                Debug.LogWarning($"Did not find key '{SELECTED_EXPERIENCE}' in PlayerPrefs, creating entry with value '{_defaultExperience}'.");
                SetSubtitlesEnabled(_defaultSubtitlesEnabled);
            }

            if (!PlayerPrefs.HasKey(SUBTITLES_PLAYER_PREF))
            {
                Debug.LogWarning($"Did not find key '{SUBTITLES_PLAYER_PREF}' in PlayerPrefs, creating entry with value '{_defaultSubtitlesEnabled}'.");
                SetSubtitlesEnabled(_defaultSubtitlesEnabled);
            }

            if (!PlayerPrefs.HasKey(RAY_INTERACTIONS_PLAYER_PREF))
            {
                Debug.LogWarning($"Did not find key '{RAY_INTERACTIONS_PLAYER_PREF}' in PlayerPrefs, creating entry with value '{_defaultRayInteractionsEnabled}'.");
                SetRayInteractionsEnabled(_defaultRayInteractionsEnabled);
            }

            if (!PlayerPrefs.HasKey(SKIP_LADDER_PLAYER_PREF))
            {
                Debug.LogWarning($"Did not find key '{SKIP_LADDER_PLAYER_PREF}' in PlayerPrefs, creating entry with value '{_defaultSkipLadderEnabled}'.");
                SetSkipLadderEnabled(_defaultSkipLadderEnabled);
            }

            if (!PlayerPrefs.HasKey(SKIP_ELEVATOR_PLAYER_PREF))
            {
                Debug.LogWarning($"Did not find key '{SKIP_ELEVATOR_PLAYER_PREF}' in PlayerPrefs, creating entry with value '{_defaultSkipElevatorEnabled}'.");
                SetSkipElevatorEnabled(_defaultSkipElevatorEnabled);
            }

            if (!PlayerPrefs.HasKey(NO_HEAT_HAZE_PLAYER_PREFS))
            {
                Debug.LogWarning($"Did not find key '{NO_HEAT_HAZE_PLAYER_PREFS}' in PlayerPrefs, creating entry with value '{_defaultNoHeatHazeEnabled}'.");
                SetNoHeatHazeEnabled(_defaultNoHeatHazeEnabled);
            }
        }

        [ContextMenu("Force Update Default Values")]
        private void ForceUpdateValues()
        {
            Debug.LogWarning($"Forcefully setting key '{SelectedExperience}' in PlayerPrefs to '{_defaultExperience}'.");
            SetSelectedExperienceType(_defaultExperience);
            Debug.LogWarning($"Forcefully setting key '{SUBTITLES_PLAYER_PREF}' in PlayerPrefs to '{_defaultSubtitlesEnabled}'.");
            SetSubtitlesEnabled(_defaultSubtitlesEnabled);
            Debug.LogWarning($"Forcefully setting key '{RAY_INTERACTIONS_PLAYER_PREF}' in PlayerPrefs to '{_defaultRayInteractionsEnabled}'.");
            SetRayInteractionsEnabled(_defaultRayInteractionsEnabled);
            Debug.LogWarning($"Forcefully setting key '{SKIP_LADDER_PLAYER_PREF}' in PlayerPrefs to '{_defaultSkipLadderEnabled}'.");
            SetSkipLadderEnabled(_defaultSkipLadderEnabled);
            Debug.LogWarning($"Forcefully setting key '{SKIP_ELEVATOR_PLAYER_PREF}' in PlayerPrefs to '{_defaultSkipElevatorEnabled}'.");
            SetSkipElevatorEnabled(_defaultSkipElevatorEnabled);
            Debug.LogWarning($"Forcefully setting key '{NO_HEAT_HAZE_PLAYER_PREFS}' in PlayerPrefs to '{_defaultNoHeatHazeEnabled}'.");
            SetNoHeatHazeEnabled(_defaultNoHeatHazeEnabled);
        }

        public void SetSelectedExperience(int selectedExperience) => PlayerPrefs.SetInt(SELECTED_EXPERIENCE, selectedExperience);

        public void SetSelectedExperienceType(ExperienceType experienceType) => SetSelectedExperience((int)experienceType);

        public void SetSubtitlesEnabled(bool enabled) => PlayerPrefs.SetInt(SUBTITLES_PLAYER_PREF, enabled ? 1 : 0);

        public void SetRayInteractionsEnabled(bool enabled) => PlayerPrefs.SetInt(RAY_INTERACTIONS_PLAYER_PREF, enabled ? 1 : 0);
        public void SetSkipLadderEnabled(bool enabled) => PlayerPrefs.SetInt(SKIP_LADDER_PLAYER_PREF, enabled ? 1 : 0);
        public void SetSkipElevatorEnabled(bool enabled) => PlayerPrefs.SetInt(SKIP_ELEVATOR_PLAYER_PREF, enabled ? 1 : 0);
        public void SetNoHeatHazeEnabled(bool enabled) => PlayerPrefs.SetInt(NO_HEAT_HAZE_PLAYER_PREFS, enabled ? 1 : 0);


        public static bool GetValueOfConditional(GameOptionConditionals conditional)
        {
            return conditional switch
            {
                GameOptionConditionals.Subtitles => SubtitlesEnabled,
                GameOptionConditionals.RayInteractions => RayInteractionsEnabled,
                GameOptionConditionals.SkipLadder => SkipLadderEnabled,
                GameOptionConditionals.SkipElevator => SkipElevatorEnabled,
                GameOptionConditionals.NoHeatHaze => NoHeatHazeEnabled,
                _ => false
            };
        }



        public enum ExperienceType
        {
            Full = 0,
            Short = 1,
            Exhibition = 2
        }

        public enum OptionalExperienceType
        {
            None = -1,
            Full = 0,
            Short = 1,
            Exhibition = 2
        }


        public enum GameOptionConditionals
        {
            Subtitles,
            RayInteractions,
            SkipLadder,
            SkipElevator,
            NoHeatHaze
        }
    }
}