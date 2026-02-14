using UnityEditor;
using ExPresSXR.Minigames.TileGame;
using UnityEngine;

[CustomEditor(typeof(TileGame))]
public class TileGameEditor : Editor
{

    protected TileGame _tileGame;

    protected void OnEnable()
    {
        _tileGame = (TileGame)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoStart"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_boardSize"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_boardSocketsParent"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_tileRespawnSockets"), true);

        EditorGUI.BeginChangeCheck();
        SerializedProperty areas = serializedObject.FindProperty("_areas");
        EditorGUILayout.PropertyField(areas, true);
        for (int i = 0; i < areas.arraySize; i++)
        {
            SerializedProperty id = areas.GetArrayElementAtIndex(i).FindPropertyRelative("Id");
            id.intValue = i;
        }
        serializedObject.ApplyModifiedProperties();


        if (EditorGUI.EndChangeCheck())
        {
            _tileGame.UpdateAreasVisuals();
        }

        EditorGUILayout.LabelField("Debug Information", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_totalScore"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_placedTiles"), true);
        EditorGUI.indentLevel--;
    }
}