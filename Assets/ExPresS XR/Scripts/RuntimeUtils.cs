using UnityEngine;


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
    }
}