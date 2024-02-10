using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEditor;
using UnityEditor.Events;

namespace ExPresSXR.Localization
{
    internal static class LocalizeComponentVideoPlayer
    {
        /// <summary>
        /// Add ContextMenu-entry to localize a VideoPlayer.
        /// </summary>
        /// <param name="command">Menu command context.</param>
        [MenuItem("CONTEXT/VideoPlayer/Localize")]
        public static void LocalizeVideoPlayer(MenuCommand command)
        {
            VideoPlayer target = command.context as VideoPlayer;
            SetupForLocalization(target);
        }

        /// <summary>
        /// Adds and sets up a VideoPlayer for Localization.
        /// </summary>
        /// <param name="target">VideoPlayer to add localization to.</param>
        public static void SetupForLocalization(VideoPlayer target)
        {
            LocalizeVideoClipEvent comp = Undo.AddComponent(target.gameObject, typeof(LocalizeVideoClipEvent)) as LocalizeVideoClipEvent;

            MethodInfo stopVideoMethod = target.GetType().GetMethod("Stop");
            MethodInfo setStringMethod = target.GetType().GetProperty("clip").GetSetMethod();
            MethodInfo playVideoMethod = target.GetType().GetMethod("Play");

            // Stop curring playback on the VideoPlayer and then play it afterward the clip has been changed
            UnityAction methodStopVideoDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), target, stopVideoMethod) as UnityAction;
            UnityAction<VideoClip> methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<VideoClip>), target, setStringMethod) as UnityAction<VideoClip>;
            UnityAction methodPlayVideoDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), target, playVideoMethod) as UnityAction;

            UnityEventTools.AddVoidPersistentListener(comp.OnUpdateAsset, methodStopVideoDelegate);
            UnityEventTools.AddPersistentListener(comp.OnUpdateAsset, methodDelegate);
            UnityEventTools.AddVoidPersistentListener(comp.OnUpdateAsset, methodPlayVideoDelegate);
        }
    }
}