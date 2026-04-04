using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class EyeTrackingMenuCreations
    {
        // Eye Tracking
        [MenuItem("GameObject/ExPresS XR/Eye Tracking/Area Of Interest")]
        static void CreateAOI(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Eye Tracking/Area Of Interest");
        }

        [MenuItem("GameObject/ExPresS XR/Eye Tracking/Area Of Interest Ray")]
        static void CreateAOIRay(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Eye Tracking/Area Of Interest Ray");
        }
    }
}