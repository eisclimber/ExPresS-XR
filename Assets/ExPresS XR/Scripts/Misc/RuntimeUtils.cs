using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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

            List<Dropdown.OptionData> newOptions = new List<Dropdown.OptionData>();

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