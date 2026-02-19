using System;
using ExPresSXR.Misc;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UiAudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip _clickClip;


    [SerializeField]
    private AudioClip _hoverOnClip;

    [SerializeField]
    private AudioClip _hoverOffClip;


    [SerializeField]
    private AudioClip _buttonDownClip;

    [SerializeField]
    private AudioClip _buttonUpClip;

    [SerializeField]
    private AudioClip _scrollClip;


    [SerializeField]
    private AudioSource _audioSource;


    public void PlayButtonClick() => PlayAudio(_clickClip);


    public void PlayButtonHoverOn() => PlayAudio(_hoverOnClip);

    public void PlayButtonHoverOff() => PlayAudio(_hoverOffClip);

    public void PlayButtonDown() => PlayAudio(_buttonDownClip);

    public void PlayButtonUp() => PlayAudio(_buttonUpClip);

    public void StartPlayScrollClamped(float value)
    {
        if ((_audioSource.clip != _scrollClip || !_audioSource.isPlaying) && value >= 0.0f && value <= 1.0f)
        {
            PlayAudio(_scrollClip);
        }
    }

    public void StartPlayScrollEventTrigger(BaseEventData _)
    {
        if (_audioSource.clip != _scrollClip || !_audioSource.isPlaying)
        {
            PlayAudio(_scrollClip);
        }
    }
    
    public void StartPlayScroll() => PlayAudio(_scrollClip);

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
            Undo.RecordObject(btn, "Add Audio Ui Callbacks");

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

            EditorUtility.SetDirty(btn);
            PrefabUtility.RecordPrefabInstancePropertyModifications(btn);
        }
    }
#endif
}
