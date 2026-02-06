using UnityEditor;
using ExPresSXR.Minigames.TileGame;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(TileRespawnSocket))]
    [CanEditMultipleObjects]
    public class TileRespawnSocketEditor : PutBackSocketInteractorEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawBeforeProperties();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Respawn Variants", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_respawnVariants"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_scoreReferenceTransform"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_areas"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentVisuals"), true);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            DrawPutBackProperties();
            EditorGUILayout.Space();
            DrawHighlightingProperties();
            EditorGUILayout.Space();
            DrawBaseSocketProperties();

            serializedObject.ApplyModifiedProperties();
        }
    }
}