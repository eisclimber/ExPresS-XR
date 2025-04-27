using System.Collections;
using System.Collections.Generic;
using ExPresSXR.Minigames.Archery.TargetSpawner.Line;
using UnityEditor;
using UnityEngine;


namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(LineSpawner))]
    [CanEditMultipleObjects]
    public class LineSpawnerEditor : UnityEditor.Editor
    {
        LineSpawner targetScript;

        private static bool _showTargetsConfig = false;
        private static bool _showGoodTargetsConfig = false;
        private static bool _showBadTargetsConfig = false;
        private static bool _gameObjectRefs = false;
        private static bool _showDebugValues = false;


        protected virtual void OnEnable()
        {
            targetScript = (LineSpawner)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            EditorGUILayout.Space();
            DrawBaseProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawBaseProperties()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetPrefab"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_speed"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftToRight"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_alternateDirections"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoStart"), true);

            _showTargetsConfig = EditorGUILayout.Foldout(_showTargetsConfig, "Targets Config");
            if (_showTargetsConfig)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_generateBadTargets"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weightedTargets"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weightedBadGoodProbabilities"), true);
            }

            _showGoodTargetsConfig = EditorGUILayout.Foldout(_showGoodTargetsConfig, "Good Targets Config");
            if (_showGoodTargetsConfig)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_goodImages"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_goodImagesProbabilities"), true);
            }

            _showBadTargetsConfig = EditorGUILayout.Foldout(_showBadTargetsConfig, "Bad Targets Config");
            if (_showBadTargetsConfig)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_badImages"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_badImagesProbabilities"), true);
            }


            _gameObjectRefs = EditorGUILayout.Foldout(_gameObjectRefs, "GameObject Refs");
            if (_gameObjectRefs)
            {
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftPost"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftSpawnAnchor"), true);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_rightPost"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_rightSpawnAnchor"), true);
            }


            _showDebugValues = EditorGUILayout.Foldout(_showDebugValues, "Debug Values");
            if (_showDebugValues)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentTarget"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentDirection"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isCurrentlyLeftToRight"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isCurrentTargetBad"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentSpawnAnchor"), true);
                EditorGUI.EndDisabledGroup();
            }
        }

        protected virtual void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}