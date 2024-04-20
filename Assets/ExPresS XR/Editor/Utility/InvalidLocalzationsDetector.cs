using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using ExPresSXR.Localization;
using System.Reflection;
using ExPresSXR.Experimentation.DataGathering;
using System.Collections;
using Codice.CM.SEIDInfo;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using ExPresSXR.Presentation;
namespace ExPresSXR.Editor.Utility
{
    public static class InvalidLocalzationsDetector
    {
        [MenuItem("ExPresS XR/Tools.../Localization/Find Invalid Localization Events", false, 6)]
        private static void FindInvalidLocalizations()
        {
            GameObject[] gameObjects = Object.FindObjectsOfType<GameObject>();
            int missingLocalizations = 0;

            foreach (GameObject go in gameObjects)
            {
                // Check Audio
                foreach (MonoBehaviour localizedMb in go.GetComponents<MonoBehaviour>())
                {
                    bool isMissing = IsLocalizerWithoutListeners(localizedMb);
                    if (isMissing)
                    {
                        Debug.LogWarning($"The localizer {localizedMb} is not localizing anything!");
                        missingLocalizations++;
                    }
                }
            }

            if (missingLocalizations <= 0)
            {
                Debug.Log("Found no missing localizations in this scene. You are all good!");
            }
            else
            {
                Debug.LogError($"Found {missingLocalizations} missing localizations. Be sure to fix them!");
            }
        }

        /// <summary>
        /// Searches for Texts, TMP_Text, VideoPlayers and ExhibitionDisplays that are missing LocalizationEvents.
        /// Any missing pairings will be printed to the console.
        /// Does NOT check AudioSources!  
        /// </summary>
        [MenuItem("ExPresS XR/Tools.../Localization/Find Not Localized (Common)", false, 6)]
        public static void FindNotLocalizedDefaultComponents()
        {
            Debug.Log("Searching for not localized Texts, TMP_Texts, VideoPlayers and ExhibitionDisplays");
            FindNotLocalizedTexts();
            FindNotLocalizedVideoPlayers();
            FindNotLocalizedExhibitionDisplays();
        }

        /// <summary>
        /// Searches for Text and TMP_Text that are missing a LocalizeAudioClipEvent.
        /// Any missing pairings will be printed to the console.
        /// </summary>
        [MenuItem("ExPresS XR/Tools.../Localization/Find Not Localized (Texts)", false, 6)]
        public static void FindNotLocalizedTexts()
        {
            FindNotLocalizedComponents<Text, LocalizeStringEvent>();
            FindNotLocalizedComponents<TMP_Text, LocalizeStringEvent>();
        }

        /// <summary>
        /// Searches for VideoPlayers that are missing a LocalizeVideoClipEvent.
        /// Any missing pairings will be printed to the console.
        /// </summary>
        [MenuItem("ExPresS XR/Tools.../Localization/Find Not Localized (VideoPlayers)", false, 6)]
        public static void FindNotLocalizedVideoPlayers() => FindNotLocalizedComponents<VideoPlayer, LocalizeVideoClipEvent>();

        /// <summary>
        /// Searches for AudioSources that are missing a LocalizeAudioClipEvent.
        /// Any missing pairings will be printed to the console.
        /// As buttons and others objects use AudioSources that might not need to be localized, there may loads false-positives.
        /// </summary>
        [MenuItem("ExPresS XR/Tools.../Localization/Find Not Localized (AudioSources)", false, 6)]
        public static void FindNotLocalizedAudioSources() => FindNotLocalizedComponents<AudioSource, LocalizeAudioClipEvent>();

        /// <summary>
        /// Searches for ExhibitionDisplays are missing LocalizeStringEvents and VideoClipEvents if the variables were set.
        /// Any missing pairings will be printed to the console.
        /// </summary>
        [MenuItem("ExPresS XR/Tools.../Localization/Find Not Localized (ExhibitionDisplays)", false, 6)]
        public static void FindNotLocalizedExhibitionDisplays()
        {
            Debug.Log("Checking ExhibitionDisplays for missing localizers.");
            Component[] components = Object.FindObjectsOfType<Component>();
            int numNotLocalized = 0;

            foreach (Component component in components)
            {
                if (component is ExhibitionDisplay @display)
                {
                    // Check texts (if exists)
                    int requiredTextLocalizers = (@display.infoText != "" ? 1 : 0)
                                                    + (@display.labelText != "" ? 1 : 0);

                    int stringLocalizations = component.GetComponents<LocalizeStringEvent>().Length;
                    if (stringLocalizations < requiredTextLocalizers)
                    {
                        Debug.LogWarning($"Found not localized ExhibitionDisplay {component}. You might want to add a LocalizeStringEvent for its infoText and labelText.");
                        numNotLocalized++;
                    }

                    // Check video
                    if (@display.infoVideoClip != null && !component.TryGetComponent<LocalizeVideoClipEvent>(out _))
                    {
                        Debug.LogWarning($"Found not localized ExhibitionDisplay {component}. You might want to add a LocalizeVideoClipEvent to localize its audio clip.");
                    }
                }
            }

            if (numNotLocalized <= 0)
            {
                Debug.Log("Found no components that could be localized in this scene. You still might want to check if other values need to be localized!");
            }
            else
            {
                Debug.LogError($"Found {numNotLocalized} Components that could be localized. You might want to check them out "
                    + "but also check if there are other things to be localized (we could have missed some)!");
            }
        }

        /// <summary>
        /// Searches for Components that are missing a localizer-Component. Any missing pairings will be printed to the console.
        /// </summary>
        /// <typeparam name="TComponent">Component to be checked.</typeparam>
        /// <typeparam name="TLocalizer">Should be a localization event (no type restriction as it somehow does not work:/).</typeparam>
        public static void FindNotLocalizedComponents<TComponent, TLocalizer>() where TComponent : Component where TLocalizer : Object
        {
            Debug.Log($"Checking {typeof(TComponent).Name}s for missing {typeof(TLocalizer).Name}-localizers.");
            Component[] components = Object.FindObjectsOfType<Component>();
            int numNotLocalized = 0;

            foreach (Component component in components)
            {
                if (component is TComponent && !component.TryGetComponent<TLocalizer>(out _))
                {
                    Debug.LogWarning($"Found not localized {component}. You might want to add a {typeof(TLocalizer).Name} to localize it.");
                    numNotLocalized++;
                }
            }

            if (numNotLocalized <= 0)
            {
                Debug.Log("Found no components that could be localized in this scene. You still might want to check if other values need to be localized!");
            }
            else
            {
                Debug.LogError($"Found {numNotLocalized} Components that could be localized. You might want to check them out "
                    + "but also check if there are other things to be localized (we could have missed some)!");
            }
        }

        /// <summary>
        /// Check if the MonoBehavior is one of the known LocalizationEvents and if it does not localize something.
        /// This should be the case, when the OnUpdateAsset-/OnUpdateString-Events have no valid(=persistent) call.
        /// </summary>
        /// <param name="localizedMb">MonoBehavior that to be checked.</param>
        /// <returns>If it is a localizer and does not localize a value.</returns>
        public static bool IsLocalizerWithoutListeners(MonoBehaviour localizedMb)
        {
            if (localizedMb is LocalizeAudioClipEvent @audioEvent)
            {
                return @audioEvent.OnUpdateAsset.GetPersistentEventCount() <= 0;
            }
            else if (localizedMb is LocalizeSpriteEvent @spriteEvent)
            {
                return @spriteEvent.OnUpdateAsset.GetPersistentEventCount() <= 0;
            }
            else if (localizedMb is LocalizeStringEvent @stringEvent)
            {
                return @stringEvent.OnUpdateString.GetPersistentEventCount() <= 0;
            }
            else if (localizedMb is LocalizeTextureEvent @textureEvent)
            {
                return @textureEvent.OnUpdateAsset.GetPersistentEventCount() <= 0;
            }
            else if (localizedMb is LocalizedGameObjectEvent @goEvent)
            {
                return @goEvent.OnUpdateAsset.GetPersistentEventCount() <= 0;
            }
            else if (localizedMb is LocalizeFontEvent @fontEvent)
            {
                return @fontEvent.OnUpdateAsset.GetPersistentEventCount() <= 0;
            }
            else if (localizedMb is LocalizeVideoClipEvent @videoEvent)
            {
                return @videoEvent.OnUpdateAsset.GetPersistentEventCount() <= 0;
            }
            return false;
        }

        /// <summary>
        /// Checks if a MonoBehaviour is a known LocalizeEvent.
        /// </summary>
        /// <param name="localizedMb">MonoBehavior to be checked.</param>
        /// <returns>If it is a known LocalizeEvent.</returns>
        public static bool IsLocalizer(MonoBehaviour localizedMb)
        {
            return localizedMb is LocalizeAudioClipEvent
                || localizedMb is LocalizeSpriteEvent
                || localizedMb is LocalizeStringEvent
                || localizedMb is LocalizeTextureEvent
                || localizedMb is LocalizedGameObjectEvent
                || localizedMb is LocalizeFontEvent
                || localizedMb is LocalizeVideoClipEvent;
        }
    }
}