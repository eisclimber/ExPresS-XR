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

            List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

            // Populate new Options
            for (int i = 0; i < Enum.GetNames(enumType).Length; i++)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(Enum.GetName(enumType, i)));
            }

            // Clear old and add new options
            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
        }
    }
}