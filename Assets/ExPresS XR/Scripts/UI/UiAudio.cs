using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExPresSXR.UI
{
    /// <summary>
    /// Class for playing common UI Audios triggered by various UI elements.
    /// 
    /// Provides automatic setup utility via the context menu.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class UiAudio : MonoBehaviour
    {
        /// <summary>
        /// Audio clip that is played when a button is clicked.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio clip that is played when a button is clicked.")]
        private AudioClip _clickClip;


        /// <summary>
        /// Audio clip that is played when a button is clicked.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio clip that is played when a button is clicked.")]
        private AudioClip _hoverOnClip;

        /// <summary>
        /// Audio clip that is played when a button is clicked.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio clip that is played when a button is clicked.")]
        private AudioClip _hoverOffClip;


        /// <summary>
        /// Audio clip that is played when a button is clicked.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio clip that is played when a button is clicked.")]
        private AudioClip _buttonDownClip;

        /// <summary>
        /// Audio clip that is played when a button is clicked.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio clip that is played when a button is clicked.")]
        private AudioClip _buttonUpClip;

        /// <summary>
        /// Audio clip that is played when a button is clicked.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio clip that is played when a button is clicked.")]
        private AudioClip _scrollClip;


        /// <summary>
        /// Audio clip that is played when a button is clicked.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio clip that is played when a button is clicked.")]
        private AudioSource _audioSource;

        /// <summary>
        /// Plays the click audio clip.
        /// </summary>
        public void PlayButtonClick() => PlayAudio(_clickClip);


        /// <summary>
        /// Plays the hover on audio clip.
        /// </summary>
        public void PlayButtonHoverOn() => PlayAudio(_hoverOnClip);

        /// <summary>
        /// Plays the hover off audio clip.
        /// </summary>
        public void PlayButtonHoverOff() => PlayAudio(_hoverOffClip);

        /// <summary>
        /// Plays the button down audio clip.
        /// </summary>
        public void PlayButtonDown() => PlayAudio(_buttonDownClip);

        /// <summary>
        /// Plays the button up audio clip.
        /// </summary>
        public void PlayButtonUp() => PlayAudio(_buttonUpClip);

        /// <summary>
        /// Plays the scroll audio clip if the provided value is between 0.0f and 1.0f (inclusive).
        /// This can be used to prevent playing the scrolling sound of scrollbars when scrolling a the ends and using `elastic` movement.
        /// </summary>
        /// <param name="value">Value to check if in range to play the sound.</param>
        public void StartPlayScrollClamped(float value)
        {
            if ((_audioSource.clip != _scrollClip || !_audioSource.isPlaying) && value >= 0.0f && value <= 1.0f)
            {
                PlayAudio(_scrollClip);
            }
        }

        /// <summary>
        /// Plays the scroll audio clip.
        /// </summary>
        /// <param name="_">The event data (not used).</param>
        public void StartPlayScrollEventTrigger(BaseEventData _)
        {
            if (_audioSource.clip != _scrollClip || !_audioSource.isPlaying)
            {
                PlayAudio(_scrollClip);
            }
        }

        /// <summary>
        /// Plays the scroll audio clip.
        /// </summary>
        public void StartPlayScroll() => PlayAudio(_scrollClip);

        /// <summary>
        /// Plays the provided audio clip.
        /// </summary>
        /// <param name="clip">Clip to be played.</param>
        public void PlayAudio(AudioClip clip)
        {
            if (clip)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }
        }

#if UNITY_EDITOR
    [ContextMenu("Add Callbacks to Buttons")]
    private void AddButtonUiCallbacks() => AddUiCallbacksToObjectsWithType<Button>();

    [ContextMenu("Add Callbacks to Toggles")]
    private void AddToggleUiCallbacks() => AddUiCallbacksToObjectsWithType<Toggle>();

    [ContextMenu("Add Callbacks to Scrollbars")]
    private void AddScrollbarUiCallbacks() => AddUiCallbacksToObjectsWithType<Scrollbar>();


    private void AddUiCallbacksToObjectsWithType<T>() where T : MonoBehaviour
    {
        T[] objs = Resources.FindObjectsOfTypeAll<T>();

        foreach (T btn in objs)
        {
            UnityEditor.Undo.RecordObject(btn, "Add Audio Ui Callbacks");

            if (!btn.gameObject.TryGetComponent(out EventTrigger trigger))
            {
                trigger = btn.gameObject.AddComponent<EventTrigger>();
                Debug.Log($"Adding EventTrigger to button: {btn}");
            }
            AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, EventTriggerType.PointerEnter, PlayButtonHoverOn, false);
            AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, EventTriggerType.PointerExit, PlayButtonHoverOff, false);
            AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, EventTriggerType.PointerDown, PlayButtonDown, false);
            AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, EventTriggerType.PointerUp, PlayButtonUp, false);

            Debug.Log("Done setting up audio events for buttons to with events.");

            UnityEditor.EditorUtility.SetDirty(btn);
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(btn);
        }
    }
#endif
    }
}