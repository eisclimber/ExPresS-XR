using System;
using System.Collections.Generic;
using System.Linq;
using ExPresSXR.Rig;
using ExPresSXR.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        /// Finds the first ExPresSXRRig in the scene (if exists).
        /// Returns true if the rig was found and set to the out parameter and false otherwise.
        /// 
        /// 
        /// Be aware that the rig must be tagged as "Player"!
        /// !!This operation is expensive, call it sparingly and rather use direct References to the rig whenever possible!!
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
        /// Helper class to calculate the positive modulo for integers.
        /// It differs from the remainder function (%) as it will return only positive values including zero.
        /// </summary>
        /// <param name="a">Dividend of the modulo operation.</param>
        /// <param name="n">Divider of the modulo operation.</param>
        /// <returns>The positive modulo of 'a mod n'.</returns>
        public static int PosMod(int a, int n)
        {
            if (n < 0)
            {
                n = -n;
            }

            int r = a % n;
            return r < 0 ? r + n : r;
        }

        /// <summary>
        /// Steps a value on a range between [0.0f, 1.0f] to the closest of even <param name="numSteps"> intervals 
        /// including the borders 0.0f and 1.0f.
        /// If <param name="numSteps"> is less than 1, the value will only be clamped between 0.0f and 1.0f.
        /// </summary>
        /// <param name="value">Value to be stepped.</param>
        /// <param name="numSteps">Number of intermediate steps.</param>
        /// <returns>Value in range [0.0f, 1.0f] stepped to the closest value.</returns>
        public static float GetValue01Stepped(float value, int numSteps)
        {
            float valueClamped = Mathf.Clamp01(value);
            if (numSteps > 0)
            {
                return Mathf.Round(valueClamped * numSteps) / numSteps;
            }
            return valueClamped;
        }

        #region Random
        /// <summary>
        /// Returns a random weighted index provided by an array of probabilities that should add up to 1.0f.
        /// </summary>
        /// <param name="probabilities">List of probabilities for each index. Should add up to 1.0f.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>A random index in the range of _probabilities or -1 if empty.</returns>
        public static int GetWeightedRandomIdx(float[] probabilities)
        {
            float sum = probabilities.Sum();
            if (sum != 1.0f)
            {
                Debug.LogWarning($"Probabilities for weighted idx did not add to 1.0f but instead to {sum}.");
            }

            if (probabilities == null || probabilities.Length <= 0)
            {
                Debug.LogWarning($"No probabilities provided. Can't generate random idx.");
                return -1;
            }

            int length = probabilities.Length;
            float random = UnityEngine.Random.Range(0.0f, 1.0f);
            float acc = 0.0f;

            for (int i = 0; i < length; i++)
            {
                acc += probabilities[i];
                if (random > (1.0 - acc))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Return a random element from the first array given the probabilities for each object.
        /// If probabilities is null, a linear distribution is used.
        /// </summary>
        /// <param name="objects">Objects to draw from.</param>
        /// <param name="probabilities">Probabilities for each object.</param>
        /// <typeparam name="T">Type of the object to draw.</typeparam>
        /// <returns>A random object from the array.</returns>
        public static T GetRandomArrayElement<T>(T[] objects, float[] probabilities = null)
        {
            bool useWeighted = probabilities != null && probabilities.Length >= 0;
            return GetRandomArrayElement(objects, useWeighted, probabilities);
        }

        /// <summary>
        /// Return a random element from the first array given the probabilities for each object.
        /// If a linear or weighted distribution should be used, is decided by the useWeighted value.
        /// </summary>
        /// <param name="objects">Objects to draw from.</param>
        /// <param name="useWeighted">Whether or not weighted random should be used.</param>
        /// /// <param name="probabilities">Probabilities for each object.</param>
        /// <typeparam name="T">Type of the object to draw.</typeparam>
        /// <returns>A random object from the array.</returns>
        public static T GetRandomArrayElement<T>(T[] objects, bool useWeighted, float[] probabilities = null)
        {
            return useWeighted ? GetRandomArrayElementWeighted(objects, probabilities) : GetRandomArrayElementUnweighted(objects);
        }

        /// <summary>
        /// Returns a random element from an array using linear distribution.
        /// </summary>
        /// <param name="objects">Objects to draw from.</param>
        /// <typeparam name="T">Type of the object to draw.</typeparam>
        /// <returns>A random object from the array.</returns>
        public static T GetRandomArrayElementUnweighted<T>(T[] objects)
        {
            if (objects.Length <= 0)
            {
                Debug.LogError("Can retrieve random element from an empty array.");
                return default;
            }

            int randomIdx = UnityEngine.Random.Range(0, objects.Length);
            return objects[randomIdx];
        }

        /// <summary>
        /// Returns a random element from an array using weighted distribution.
        /// </summary>
        /// <param name="objects">Objects to draw from.</param>
        /// <param name="probabilities">Probabilities for each object.</param>
        /// <typeparam name="T">Type of the object to draw.</typeparam>
        /// <returns>A random object from the array.</returns>
        public static T GetRandomArrayElementWeighted<T>(T[] objects, float[] probabilities)
        {
            if (objects.Length <= 0 || objects.Length != probabilities.Length)
            {
                Debug.LogError("Invalid array size for generating random ");
                return default;
            }
            int randomIdx = GetWeightedRandomIdx(probabilities);
            return randomIdx >= 0 ? objects[randomIdx] : default;
        }

        #endregion

        #region Scene Switching
        /// <summary>
        /// Changes a scene whilst the current rig is faded out. Supports 'DontDestroyOnLoad' if enabled on the rig.
        /// If no rig is provided or it does does not have a fade Rect the Scene will change instant.
        /// </summary>
        /// <param name="rig">The rig that is will be attempted to fade. </param>
        /// <param name="sceneIdx"> The Scene index to change to (from the build settings). </param>
        /// <param name="keepRig"> Wether or not the rig should be kept after loading the new scene. </param>
        /// <param name="sceneLoadedCallback"> A callback that will be executed after the new scene loaded. Can be null. </param>
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

                    // Invoke Callback if provided
                    sceneLoadedCallback?.Invoke();
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
        /// <param name="callback">The callback invoked after completing the AsyncLoad. Can be null. </param>
        public static void SwitchSceneAsync(int sceneIdx, Action callback)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneIdx, LoadSceneMode.Single);
            op.completed += (_) => { callback?.Invoke(); };
        }
        #endregion

        # region Dropdown Helper
        /// <summary>
        /// Populates a <see cref="Dropdown"/> with the names of a given <see cref="Enum"/>.
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
        /// Populates a <see cref="TMP_Dropdown"/> with the names of a given <see cref="Enum"/>.
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
        /// Populates a <see cref="TMP_Dropdown"/> with the names of a given <see cref="Enum"/>.
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
        /// Populates a <see cref="TMP_Dropdown"/> with the names proved by stringOptions.
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
        #endregion
    }
}