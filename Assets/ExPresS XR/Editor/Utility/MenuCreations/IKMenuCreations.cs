using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class IKMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Inverse Kinematics/Sample - Empty")]
        static void CreateIKSampleEmpty(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "IK/IK Sample - Empty");
        }


        [MenuItem("GameObject/ExPresS XR/Inverse Kinematics/Sample - Character")]
        static void CreateIKSampleCharacter(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "IK/IK Sample - Character");
        }
    }
}