using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace ExPresSXR.Localization
{
    /// <summary>
    /// Defines the type of the asset to be localized.
    /// </summary>
    [Serializable]
    public class LocalizedVideoClip : LocalizedAsset<VideoClip> { }

    /// <summary>
    /// Defines the Callback-Event for Localization.
    /// </summary>
    [Serializable]
    public class UnityEventVideoClip : UnityEvent<VideoClip> { }

    /// <summary>
    /// Defines the Component for Localization.
    /// </summary>
    [AddComponentMenu("Localization/Asset/Localize Video Clip Event")]
    public class LocalizeVideoClipEvent : LocalizedAssetEvent<VideoClip, LocalizedVideoClip, UnityEventVideoClip> { }
}
