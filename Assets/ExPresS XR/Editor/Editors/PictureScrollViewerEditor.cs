using UnityEngine;
using UnityEditor;
using ExPresSXR.Presentation.Pictures;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(PictureScrollViewer))]
    [CanEditMultipleObjects]
    public class PictureScrollViewerEditor : UnityEditor.Editor
    {
        PictureScrollViewer scrollViewer;

        protected static bool _showEvents = false;
        protected static bool _showObjectRefs = false;

        protected virtual void OnEnable()
        {
            scrollViewer = (PictureScrollViewer)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            EditorGUILayout.Space();
            DrawBaseProperties();
            EditorGUILayout.Space();
            DrawEventsFoldout();
            EditorGUILayout.Space();
            DrawObjectRefsFoldout();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawBaseProperties()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pictureData"), true);
            if (EditorGUI.EndChangeCheck())
            {
                // Update displayed prefab only when necessary
                serializedObject.ApplyModifiedProperties();
                scrollViewer.InternalUpdatePictureData();
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_scrollBehavior"), true);
            EditorGUI.indentLevel++;
            if (scrollViewer.ScrollBehavior == ScrollType.ConstantDuration)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoScrollDuration"), true);
            }
            else if (scrollViewer.ScrollBehavior == ScrollType.ConstantSpeed)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoScrollSpeed"), true);
            }
            else if (scrollViewer.ScrollBehavior == ScrollType.PictureSnap)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_pictureSnapDuration"), true);
            }
            EditorGUI.indentLevel--;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_scrollValue"), true);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoScrollRight"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoScrollLeft"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_noPicturesTitle"), true);
        }

        protected virtual void DrawEventsFoldout()
        {
            _showEvents = EditorGUILayout.BeginFoldoutHeaderGroup(_showEvents, "Events");
            if (_showEvents)
            {
                DrawEvents();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected virtual void DrawEvents()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPictureDataChanged"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPictureDataAdded"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPictureDataRemoved"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAutoScrollActive"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAutoScrollInactive"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPicturesStartReached"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPicturesMidReached"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPicturesEndReached"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawObjectRefsFoldout()
        {
            _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

            if (_showObjectRefs)
            {
                DrawObjectRefs();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected virtual void DrawObjectRefs()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Do not change these! Thank you:)");

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_scrollRect"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_picturesContainer"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_slider"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_titleText"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoTextDisplay"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pictureDataSocket"), true);

            EditorGUI.indentLevel--;
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