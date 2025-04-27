using System.Collections.Generic;
using ExPresSXR.Minigames.Excavation;
using UnityEditor;
using UnityEngine;

namespace ExPresSXR.Editor.Editors
{
    [CustomPropertyDrawer(typeof(ExcavationZone))]
    public class ExcavationZoneDrawer : PropertyDrawer
    {
        private const int PROPERTY_SPACING = 2;
        private const int NUM_REGULAR_LINES = 5;
        private const int MATRIX_TOGGLE_SIZE = 20;
        private const int MATRIX_MAX_GRANULARITY = 4;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            Rect positionRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (property.isExpanded = EditorGUI.Foldout(positionRect, property.isExpanded, label))
            {
                positionRect.x += EditorGUIUtility.singleLineHeight; // = Indentation
                positionRect.width -= EditorGUIUtility.singleLineHeight; // = Indentation
                positionRect = DrawNextProperty("_description", positionRect, property);
                positionRect = DrawNextProperty("_channel", positionRect, property);
                positionRect = DrawNextProperty("_completionValue", positionRect, property);
                EditorGUI.BeginDisabledGroup(true);
                positionRect = DrawNextProperty("_completed", positionRect, property);
                EditorGUI.EndDisabledGroup();
                positionRect = DrawPositions(positionRect, property);
                positionRect.y += GetMatrixPropertyHeight(property); // Idk why this is necessary but the rect won't update...
                _ = DrawCompletionEvent(positionRect, property);
            }
            EditorGUI.EndProperty();
        }

        private Rect DrawPositions(Rect currentRect, SerializedProperty property)
        {
            int granularity = property.FindPropertyRelative("_granularity").intValue;
            if (ShouldDisplayGrid(granularity))
            {
                DrawPositionsMatrix(currentRect, property);
            }
            else
            {
                if (ShouldDisplayGridWarning(granularity))
                {
                    currentRect = GetNextPropertyRect(currentRect, 2 * EditorGUIUtility.singleLineHeight);
                    EditorGUI.HelpBox(currentRect, "Granularity too high to be displayed, consider a smaller granularity!", MessageType.Warning);
                }
                // Fallback if granularity was not set (not controlled by an ExcavationGame) or is too large > 2^5
                DrawNextProperty("_positions", currentRect, property);
            }
            return currentRect;
        }

        private Rect DrawPositionsMatrix(Rect currentRect, SerializedProperty property)
        {
            currentRect = GetNextPropertyRect(currentRect);
            EditorGUI.LabelField(currentRect, "Positions");
            
            SerializedProperty positionsProperty = property.FindPropertyRelative("_positions");
            Vector2Int[] positions = ReadPositionsAsArray(positionsProperty);
            int granularity = property.FindPropertyRelative("_granularity").intValue;
            int matrixSize = (int)Mathf.Pow(2, granularity);
            bool[,] matrix = ConvertPositionsToBoolMatrix(granularity, positions);

            EditorGUI.BeginChangeCheck();
            currentRect = GetNextPropertyRect(currentRect, GetRawMatrixHeight(granularity));
            for (int y = 0; y < matrixSize; y++)
            {
                if (y == matrixSize - 1)
                {
                    Rect arrowRect = new(
                        currentRect.x,
                        currentRect.y + y * MATRIX_TOGGLE_SIZE - EditorGUIUtility.singleLineHeight / 4,
                        EditorGUIUtility.singleLineHeight,
                        2 * EditorGUIUtility.singleLineHeight
                    );
                    EditorGUI.LabelField(arrowRect, " Y\n ↑");
                }

                for (int x = 0; x < matrixSize; x++)
                {
                    Rect toggleRect = new(
                        currentRect.x + x * MATRIX_TOGGLE_SIZE + EditorGUIUtility.singleLineHeight,
                        currentRect.y + y * MATRIX_TOGGLE_SIZE,
                        MATRIX_TOGGLE_SIZE,
                        MATRIX_TOGGLE_SIZE
                    );
                    // Bottom left is (0, 0) so we invert y coordinate
                    matrix[matrixSize - y - 1, x] = EditorGUI.Toggle(toggleRect, GUIContent.none, matrix[matrixSize - y - 1, x]);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                SetPositionsFromMatrix(matrix, positionsProperty);
            }

            currentRect = GetNextPropertyRect(currentRect);
            EditorGUI.LabelField(new(currentRect.x - PROPERTY_SPACING, currentRect.y - PROPERTY_SPACING, currentRect.width, currentRect.height), "  └→ X");
            return currentRect;
        }

        private Rect DrawCompletionEvent(Rect currentRect, SerializedProperty property)
        {
            SerializedProperty eventProperty = property.FindPropertyRelative("OnZoneCompleted");
            Rect nextRect = GetNextPropertyRect(currentRect, EditorGUI.GetPropertyHeight(eventProperty));
            EditorGUI.PropertyField(nextRect, eventProperty);
            return nextRect;
        }

        private Rect DrawNextProperty(string propertyName, Rect currentRect, SerializedProperty property)
        {
            Rect nextRect = GetNextPropertyRect(currentRect);
            EditorGUI.PropertyField(nextRect, property.FindPropertyRelative(propertyName));
            return nextRect;
        }

        private Rect GetNextPropertyRect(Rect currentRect, float height = -1.0f)
        {
            float actualHeight = height > 0.0f ? height : EditorGUIUtility.singleLineHeight;
            return new Rect(currentRect.x,
                            currentRect.y + currentRect.height + PROPERTY_SPACING,
                            currentRect.width,
                            actualHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return NUM_REGULAR_LINES * (EditorGUIUtility.singleLineHeight + PROPERTY_SPACING)
                    + GetMatrixPropertyHeight(property)
                    + 2 * PROPERTY_SPACING
                    + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("OnZoneCompleted"));
            }
            return EditorGUIUtility.singleLineHeight;
        }

        private float GetMatrixPropertyHeight(SerializedProperty property)
        {
            int granularity = property.FindPropertyRelative("_granularity").intValue;
            if (ShouldDisplayGrid(granularity))
            {
                // Header line + bottom line + matrix
                return 2 * (EditorGUIUtility.singleLineHeight + PROPERTY_SPACING) + GetRawMatrixHeight(granularity);
            }
            else
            {
                float warningHeight = ShouldDisplayGridWarning(granularity) ? 2 * EditorGUIUtility.singleLineHeight : 0.0f;
                // Display "raw" positions
                return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_positions"), true) + warningHeight;
            }
        }

        private float GetRawMatrixHeight(float granularity) => Mathf.Pow(2, granularity) * MATRIX_TOGGLE_SIZE;

        private bool ShouldDisplayGrid(int granularity) => granularity >= 0 && granularity <= MATRIX_MAX_GRANULARITY;
        private bool ShouldDisplayGridWarning(int granularity) => granularity > MATRIX_MAX_GRANULARITY;

        private Vector2Int[] ReadPositionsAsArray(SerializedProperty positionsProperty)
        {
            Vector2Int[] positions = new Vector2Int[positionsProperty.arraySize];
            for (int i = 0; i < positionsProperty.arraySize; i++)
            {
                positions[i] = positionsProperty.GetArrayElementAtIndex(i).vector2IntValue;
            }
            return positions;
        }

        private bool[,] ConvertPositionsToBoolMatrix(int granularity, Vector2Int[] positions)
        {
            if (!ShouldDisplayGrid(granularity))
            {
                // No granularity -> no value to be calculated.
                Debug.LogWarning("Can't calculate a proper matrix of the excavation zone since the granularity is less than zero.");
                return new bool[0, 0];
            }

            int matrixSize = (int)Mathf.Pow(2, granularity);
            bool[,] matrix = new bool[matrixSize, matrixSize];

            foreach (Vector2Int pos in positions)
            {
                if (pos.x < 0 || pos.x >= matrixSize || pos.y < 0 || pos.y >= matrixSize)
                {
                    // No warning to avoid spamming if granularity get changed for testing/setup/...
                    // Debug.LogWarning($"Invalid position found, skipping position '{pos}'.");
                    continue;
                }
                matrix[pos.y, pos.x] = true;
            }
            return matrix;
        }

        private Vector2Int[] CondenseMatrix(bool[,] matrix)
        {
            List<Vector2Int> newPositions = new();
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (matrix[y, x])
                    {
                        newPositions.Add(new(x, y));
                    }
                }
            }
            return newPositions.ToArray();
        }

        private void SetPositionsFromMatrix(bool[,] matrix, SerializedProperty positionsProperty)
        {
            if (!positionsProperty.isArray)
            {
                Debug.LogError("Invalid positionProperty provided. Must be an array type!");
                return;
            }

            Vector2Int[] positions = CondenseMatrix(matrix);
            positionsProperty.arraySize = positions.Length;
            for (int i = 0; i < positions.Length; i++)
            {
                positionsProperty.GetArrayElementAtIndex(i).vector2IntValue = positions[i];
            }
        }
    }
}