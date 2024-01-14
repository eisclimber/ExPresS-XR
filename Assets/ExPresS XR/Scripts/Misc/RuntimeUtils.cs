using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using ExPresSXR.UI;
using ExPresSXR.Rig;
using UnityEditor;

namespace ExPresSXR.Misc
{
    public static class RuntimeUtils
    {
        /// <summary>
        /// Finds and returns the (first) <see cref="Transform"/> of a child with the specified name.
        /// </summary>
        /// <param name="parent">The transform to find the child in.</param>
        /// <param name="childName">The name of the GameObject to find.</param>
        /// <returns>Returns the first Transform of a GameObject having the specified name,
        /// or <see langword="null"/> if there is none.</returns>
        public static Transform RecursiveFindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }
                else
                {
                    Transform found = RecursiveFindChild(child, childName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Changes a scene whilst the current rig is faded out. Supports 'DontDestroyOnLoad' if enabled on the rig.
        /// If no rig is provided or it does does not have a fade Rect the Scene will change instant.
        /// </summary>
        /// <param name="rig">The rig that is will be attempted to fade. </param>
        /// <param name="sceneIdx"> The Scene index to change to (from the build settings). </param>
        /// <param name="keepRig"> Wether or not the rig should be kept after loading the new scene. </param>
        /// <param name="sceneLoadedCallback"> A callback that will be executed after the new scene loaded. </param>
        public static void ChangeSceneWithFade(ExPresSXRRig rig, int sceneIdx, bool keepRig, Action sceneLoadedCallback)
        {
            if (rig == null || rig.fadeRect == null)
            {
                // No Rig => No Fade Out
                SwitchSceneAsync(sceneIdx, sceneLoadedCallback);
            }
            else
            {
                FadeRect fadeRect = rig.fadeRect;

                if (keepRig)
                {
                    UnityEngine.Object.DontDestroyOnLoad(rig);
                }

                /*
                    Use local functions to remove the listeners on completion.
                */
                void SceneSwitcher()
                {
                    SwitchSceneAsync(sceneIdx, RigSetup);
                }

                void RigSetup()
                {
                    if (!keepRig)
                    {
                        if (TryFindExPresSXRRigReference(out ExPresSXRRig newRig))
                        {
                            fadeRect = newRig.fadeRect;
                        }
                        else
                        {
                            Debug.LogError("Could not find the new ExPresSXRRig, make sure it has the tag 'Player' and "
                                            + "it is the highest in the hierarchy with that tag. "
                                            + "Be aware this means that the callback will never be invoked!");
                        }
                    }
                    else
                    {
                        fadeRect.OnFadeToColorCompleted.RemoveListener(SceneSwitcher);
                    }

                    fadeRect.OnFadeToClearCompleted.AddListener(SwitchCleanup);
                    fadeRect.FadeToColor(true);
                    fadeRect.FadeToClear(false);

                    // Invoke Callback
                    sceneLoadedCallback.Invoke();
                }

                void SwitchCleanup()
                {
                    fadeRect.OnFadeToColorCompleted.RemoveListener(SwitchCleanup);
                }


                // Fade out and switch scene
                fadeRect.FadeToColor(false);
                fadeRect.OnFadeToColorCompleted.AddListener(SceneSwitcher);
            }
        }

        /// <summary>
        /// Switches the scene to the given index (if possible) and invokes a callback after completion.
        /// </summary>
        /// <param name="sceneIdx">The scene's index. Must be added to the BuildSetting to receive an index.</param>
        /// <param name="callback">The callback invoked after completing the AsyncLoad.</param>
        public static void SwitchSceneAsync(int sceneIdx, Action callback)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneIdx, LoadSceneMode.Single);
            op.completed += (_) => { callback.Invoke(); };
        }


        /// <summary>
        /// Finds the first ExPresSXRRig in the scene (if exists).
        /// The rig must be tagged as "Player"!
        /// !! This operation is expensive, call it sparingly and using direct References using SerializedProperties!!
        /// </summary>
        /// <param name="rig">The rig or null.</param>
        /// <returns>If a rig was found</returns>
        public static bool TryFindExPresSXRRigReference(out ExPresSXRRig rig)
        {
            GameObject[] playerGos = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject go in playerGos)
            {
                if (go.TryGetComponent(out rig))
                {
                    return true;
                }
            }
            rig = null;
            return false;
        }

        /// <summary>
        /// Populates an <see cref="Dropdown"/> with the names of a given <see cref="Enum"/>.
        /// </summary>
        /// <param name="dropdown">The Dropdown to be populated.</param>
        /// <param name="enumType">The Type of the Enum the Dropdown should be populated with.</param>
        public static void PopulateDropDownWithEnum(Dropdown dropdown, Type enumType)
        {
            Debug.Log(dropdown == null);

            if (!enumType.IsEnum)
            {
                Debug.LogError("Parameter 'enumType' was not a Enum.");
            }

            List<Dropdown.OptionData> newOptions = new();

            // Populate new Options
            for (int i = 0; i < Enum.GetNames(enumType).Length; i++)
            {
                newOptions.Add(new Dropdown.OptionData(Enum.GetName(enumType, i)));
            }

            // Clear old and add new options
            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
        }


        /// <summary>
        /// Populates an <see cref="TMP_Dropdown"/> with the names of a given <see cref="Enum"/>.
        /// </summary>
        /// <param name="dropdown">The Dropdown to be populated.</param>
        /// <param name="enumType">The Type of the Enum the Dropdown should be populated with.</param>
        public static void PopulateTMPDropDownWithEnum(TMP_Dropdown dropdown, Type enumType)
        {
            if (!enumType.IsEnum)
            {
                Debug.LogError("Parameter 'enumType' was not a Enum.");
            }

            List<TMP_Dropdown.OptionData> newOptions = new();

            // Populate new Options
            for (int i = 0; i < Enum.GetNames(enumType).Length; i++)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(Enum.GetName(enumType, i)));
            }

            // Clear old and add new options
            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
        }

        /// <summary>
        /// Populates an <see cref="TMP_Dropdown"/> with the names of a given <see cref="Enum"/>.
        /// Be careful as this will produce an entry for every combination, meaning 2^{Enum.Length} entries.
        /// </summary>
        /// <param name="dropdown">The Dropdown to be populated.</param>
        /// <param name="enumType">The Type of the Enum the Dropdown should be populated with.</param>
        public static void PopulateTMPDropDownWithFlags(TMP_Dropdown dropdown, Type enumType)
        {
            if (!enumType.IsEnum)
            {
                Debug.LogError("Parameter 'enumType' was not a Enum.");
            }

            List<TMP_Dropdown.OptionData> newOptions = new();

            // Populate new Options
            float maxEnumValue = Mathf.Pow(2.0f, Enum.GetNames(enumType).Length);
            for (int i = 0; i < maxEnumValue; i++)
            {
                var optionName = "";
                if (i == 0)
                {
                    // No bits set
                    optionName = "None";
                }
                else if (i == maxEnumValue - 1)
                {
                    // All bits set
                    optionName = "Everything";
                }
                else
                {
                    // Parse Flags
                    for (int j = 0; j < Enum.GetNames(enumType).Length; j++)
                    {
                        // Bitmask is set for enum entry with value j
                        if ((i & (1 << j)) != 0)
                        {
                            // Add separator if necessary
                            optionName += optionName == "" ? "" : " + ";
                            // Add enum name
                            optionName += Enum.GetName(enumType, j);
                        }
                    }
                }

                newOptions.Add(new TMP_Dropdown.OptionData(optionName));
            }

            // Clear old and add new options
            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
        }


        /// <summary>
        /// Populates an <see cref="TMP_Dropdown"/> with the names proved by stringOptions.
        /// </summary>
        /// <param name="dropdown">The Dropdown to be populated.</param>
        /// <param name="enumType">The Type of the Enum the Dropdown should be populated with.</param>
        public static void PopulateTMPDropDownWithCustomValues(TMP_Dropdown dropdown, string[] stringOptions)
        {
            List<TMP_Dropdown.OptionData> newOptions = new();

            // Populate new Options
            foreach (var option in stringOptions)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(option));
            }

            // Clear old and add new options
            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
        }


        /// <summary>
        /// Enforces Unity's Flags rules on an int from a Dropdown where 'Everything' = -1 
        /// and not only 1's.
        /// </summary>
        /// <param name="value">The value to be converted to type T.</param>
        /// <returns>Returns the with a corrected value.</returns>
        public static T DropdownToUnityFlagValue<T>(int value) where T : Enum
        {
            int maxEnumValue = (int)Mathf.Pow(2.0f, Enum.GetNames(typeof(T)).Length);
            return (T)(object)(value >= 0 && value < maxEnumValue ? value : -1);
        }

        /// <summary>
        /// Changes a Unity's Flags value to an int from a Dropdown where 'Everything' is the
        /// last entry in the list.
        /// </summary>
        /// <param name="value">The value to be converted to type T.</param>
        /// <returns>Returns the with a corrected value.</returns>
        public static int UnityToDropdownFlagValue<T>(T value) where T : Enum
        {
            int intValue = (int)(object)value;
            int maxEnumValue = (int)Mathf.Pow(2.0f, Enum.GetNames(typeof(T)).Length);
            return intValue >= 0 && intValue < maxEnumValue ? intValue : maxEnumValue;
        }
    }
}