using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace ExPresSXR.Localization
{
    /// <summary>
    /// Defines the type of the asset to be localized,
    /// </summary>
    [Serializable]
    public class LocalizedFont : LocalizedAsset<Font> { }

    /// <summary>
    /// Defines the Callback-Event for Localization.
    /// </summary>
    [Serializable]
    public class UnityEventFont : UnityEvent<Font> { }

    /// <summary>
    /// Defines the Component for Localization.
    /// </summary>
    [AddComponentMenu("Localization/Asset/Localize Font Event")]
    public class LocalizeFontEvent : LocalizedAssetEvent<Font, LocalizedFont, UnityEventFont> { }
}
