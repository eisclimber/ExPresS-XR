using UnityEngine;
using UnityEditor;
using ExPresSXR.Experimentation.DataGathering;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class DataGatheringMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Data Gatherer")]
        public static DataGatherer CreateDataGatherer(MenuCommand _)
        {
            GameObject go = new("Data Gatherer");
            DataGatherer dataGatherer = go.AddComponent<DataGatherer>();
            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.RegisterCreatedObjectUndo(go, "Create Data Gatherer Game Object");
            return dataGatherer;
        }
    }
}