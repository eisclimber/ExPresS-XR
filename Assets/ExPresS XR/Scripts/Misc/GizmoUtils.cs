using UnityEditor;
using UnityEngine;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// An utility class to draw a few more Gizmo shapes.
    /// 
    /// If you want your gizmos to use local coordinates and scale and rotate with your object,
    /// be sure to pass it's Transform as <see cref="atTransform"/>.
    /// </summary>
    public static class GizmoUtils
    {
        /// <summary>
        /// Length of the end marker when drawing a MinMaxLine or arc.
        /// </summary>
        public const float END_LINE_LENGTH = 0.015f;

        /// <summary>
        /// Length of the value marker when marking (Value)MinMaxLine or arc.
        /// </summary>
        public const float VALUE_LINE_LENGTH = 0.01f;

        /// <summary>
        /// Radius of the arch when drawing a DrawMinMaxArc.
        /// </summary>
        public const float ARCH_RADIUS = 0.15f;

        /// <summary>
        /// Line length when drawing the end marker of a MinMaxRotationSpan.
        /// </summary>
        public const float ROTATION_SPAN_END_LENGTH = 0.25f;

        /// <summary>
        /// Line length when drawing the value marker of a MinMaxRotationSpan.
        /// </summary>
        public const float ROTATION_SPAN_VALUE_LENGTH = 0.2f;

        /// <summary>
        /// Line length when drawing the arc of a MinMaxRotationSpan.
        /// </summary>
        public const float ROTATION_SPAN_RADIUS = 0.1f;

        /// <summary>
        /// Default size for labels (=Unity's default).
        /// </summary>
        public const int DEFAULT_LABEL_SIZE = 11;

        /// <summary>
        /// Draws a marker line with the provided properties.
        /// </summary>
        /// <param name="pos">Center position of the marker.</param>
        /// <param name="length">Length of the marker.</param>
        /// <param name="color">Color of the marker.</param>
        /// <param name="upVector">Determines the direction of the end markers.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawMarkerLineAt(Vector3 pos, float length, Color color, Vector3 upVector, Transform atTransform = null)
        {
            Gizmos.matrix = atTransform != null ? atTransform.localToWorldMatrix : Matrix4x4.identity;
            Gizmos.color = color;
            Gizmos.DrawLine(pos + upVector * length, pos - upVector * length);
        }

        /// <summary>
        /// Draws a line between two points (minimum and maximum) with a marker at each end.
        /// </summary>
        /// <param name="minPos">Minimal position.</param>
        /// <param name="maxPos">Maximal position.</param>
        /// <param name="minColor">Color of the minimum marker.</param>
        /// <param name="maxColor">Color of the maximum marker.</param>
        /// <param name="lineColor">Color of the line between the points.</param>
        /// <param name="upVector">Determines the direction of the end markers.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawMinMaxLine(Vector3 minPos, Vector3 maxPos, Color minColor, Color maxColor, Color lineColor, Vector3 upVector, Transform atTransform = null)
        {
            // Apply transform if provided
            Gizmos.matrix = atTransform != null ? atTransform.localToWorldMatrix : Matrix4x4.identity;

            DrawMarkerLineAt(minPos, END_LINE_LENGTH, minColor, upVector, atTransform);
            DrawMarkerLineAt(maxPos, END_LINE_LENGTH, maxColor, upVector, atTransform);

            // Draw Middle Lines
            Gizmos.color = lineColor;
            Gizmos.DrawLine(minPos, maxPos);
        }

        /// <summary>
        /// Draws a line between two points (minimum and maximum) with a marker at each end and a marker highlighting between these two values.
        /// </summary>
        /// <param name="minPos">Minimal position.</param>
        /// <param name="maxPos">Maximal position.</param>
        /// <param name="value">Value in the range of 0.0f to 1.0f.</param>
        /// <param name="minColor">Color of the minimum marker.</param>
        /// <param name="maxColor">Color of the maximum marker.</param>
        /// <param name="lineColor">Color of the line between the points.</param>
        /// <param name="valueColor">Color of the value marker.</param>
        /// <param name="upVector">Determines the direction of the end markers.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawMinMaxValueLine(Vector3 minPos, Vector3 maxPos, float value, Color minColor, Color maxColor,
                                                    Color lineColor, Color valueColor, Vector3 upVector, Transform atTransform = null)
        {
            DrawMinMaxLine(minPos, maxPos, minColor, maxColor, lineColor, upVector, atTransform);
            Vector3 valuePos = Vector3.Lerp(minPos, maxPos, value);
            DrawMarkerLineAt(valuePos, VALUE_LINE_LENGTH, valueColor, upVector, atTransform);
        }

        /// <summary>
        /// Draws a grid with the given up-vector and the specified number of intersecting lines.
        /// </summary>
        /// <param name="center">Position of the gird.</param>
        /// <param name="size">Size of the grid.</param>
        /// <param name="numTiles">Number of tiles in the grid.</param>
        /// <param name="outlineColor">Color of the outline.</param>
        /// <param name="gridColor">Color of the grid lines.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawGrid(Vector3 center, Vector2 extents, Vector2 numTiles, Color outlineColor, Color gridColor, Transform atTransform = null)
        {
#if UNITY_EDITOR
            Gizmos.matrix = atTransform != null ? atTransform.localToWorldMatrix : Matrix4x4.identity;

            Vector3 blPos = new(center.x - extents.x, center.y, center.z - extents.y);
            Vector3 brPos = new(center.x + extents.x, center.y, center.z - extents.y);
            Vector3 tlPos = new(center.x - extents.x, center.y, center.z + extents.y);
            Vector3 trPos = new(center.x + extents.x, center.y, center.z + extents.y);

            // Draw outline
            Gizmos.color = outlineColor;
            Gizmos.DrawLine(blPos, brPos);
            Gizmos.DrawLine(blPos, tlPos);
            Gizmos.DrawLine(trPos, tlPos);
            Gizmos.DrawLine(trPos, brPos);

            // Draw grid
            Gizmos.color = gridColor;
            Vector2 gridStepSize = 2.0f * extents / numTiles;
            // Draw vertical grid
            for (int i = 1; i < numTiles.x; i++)
            {
                float delta = i * gridStepSize.x;
                Vector3 offset = new(delta, 0.0f, 0.0f);
                Gizmos.DrawLine(blPos + offset, tlPos + offset);
            }

            // Draw horizontal lines
            for (int i = 1; i < numTiles.y; i++)
            {
                float delta = i * gridStepSize.y;
                Vector3 offset = new(0.0f, 0.0f, delta);
                Gizmos.DrawLine(blPos + offset, brPos + offset);
            }
#endif
        }

        /// <summary>
        /// Draws an arc between two angles (minimum and maximum) relative to the normal plane defined by <see cref="localNormal"/> 
        /// and origin <see cref="localOffset"/>.
        /// </summary>
        /// <param name="minAngle">Minimum angle.</param>
        /// <param name="maxAngle">Maximum angle.</param>
        /// <param name="minColor">Color of the minimum marker.</param>
        /// <param name="maxColor">Color of the maximum marker.</param>
        /// <param name="arcColor">Color of the arc between the angels.</param>
        /// <param name="localOffset">Pivot offset of the angle.</param>
        /// <param name="localNormal">Normal vector defining the plane of the angles.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawMinMaxArc(float minAngle, float maxAngle, Color minColor, Color maxColor, Color arcColor,
                                            Vector3 localOffset, Vector3 localNormal, Transform atTransform = null)
        {
#if UNITY_EDITOR
            Gizmos.matrix = atTransform != null ? atTransform.localToWorldMatrix : Matrix4x4.identity;
            Handles.matrix = atTransform != null ? atTransform.localToWorldMatrix : Matrix4x4.identity;

            Vector3 minPos = localOffset + Quaternion.AngleAxis(minAngle, localNormal) * Vector3.forward * ARCH_RADIUS;
            Vector3 maxPos = localOffset + Quaternion.AngleAxis(maxAngle, localNormal) * Vector3.forward * ARCH_RADIUS;

            DrawMarkerLineAt(minPos, END_LINE_LENGTH, minColor, localNormal, atTransform);
            DrawMarkerLineAt(maxPos, END_LINE_LENGTH, maxColor, localNormal, atTransform);

            Handles.color = arcColor;
            Handles.DrawWireArc(localOffset, Vector3.up, minPos.normalized, maxAngle - minAngle, ARCH_RADIUS);
#endif
        }

        /// <summary>
        /// Draws an arc between two angles (minimum and maximum) relative to the normal plane defined by <see cref="localNormal"/>.
        /// and origin <see cref="localOffset"/>.
        /// </summary>
        /// <param name="minAngle">Minimum angle.</param>
        /// <param name="maxAngle">Maximum angle.</param>
        /// <param name="value">Value in the range of 0.0f to 1.0f.</param>
        /// <param name="minColor">Color of the minimum marker.</param>
        /// <param name="maxColor">Color of the maximum marker.</param>
        /// <param name="arcColor">Color of the arc between the angels.</param>
        /// <param name="valueColor">Color of the value marker.</param>
        /// <param name="localOffset">Pivot offset of the angle.</param>
        /// <param name="localNormal">Normal vector defining the plane of the angles.</param>
        public static void DrawMinMaxValueArc(float minAngle, float maxAngle, float value, Color minColor, Color maxColor,
                                                Color arcColor, Color valueColor, Vector3 localOffset, Vector3 localNormal, Transform atTransform = null)
        {
            DrawMinMaxArc(minAngle, maxAngle, minColor, maxColor, arcColor, localOffset, localNormal, atTransform);

            float valueAngle = Mathf.Lerp(minAngle, maxAngle, value);
            Vector3 maxPos = localOffset + Quaternion.AngleAxis(valueAngle, localNormal) * Vector3.forward * ARCH_RADIUS;
            DrawMarkerLineAt(maxPos, VALUE_LINE_LENGTH, valueColor, localNormal, atTransform);
        }

        /// <summary>
        /// Draws an span between two rotations (minimum and maximum) with a marker at each end.
        /// </summary>
        /// <param name="minRotation">Minimum rotation.</param>
        /// <param name="maxRotation">Minimum rotation.</param>
        /// <param name="minColor">Color of the minimum marker.</param>
        /// <param name="maxColor">Color of the maximum marker.</param>
        /// <param name="spanColor">Color of the span between min and max.</param>
        /// <param name="localOffset">Pivot offset of the angle.</param>
        /// <param name="localNormal">Normal of the plane where the markers are placed.</param>
        /// <param name="localForward">Direction in the which the angle arc is oriented.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawMinMaxRotationSpan(Quaternion minRotation, Quaternion maxRotation, Color minColor, Color maxColor, Color spanColor,
                                                    Vector3 localOffset, Vector3 localNormal, Vector3 localForward, Transform atTransform = null)
        {
#if UNITY_EDITOR
            Gizmos.matrix = atTransform != null ? atTransform.localToWorldMatrix : Matrix4x4.identity;
            Handles.matrix = atTransform != null ? atTransform.localToWorldMatrix : Matrix4x4.identity;

            float angleRange = Quaternion.Angle(minRotation, maxRotation);

            Vector3 angleMinDir = minRotation * localNormal;
            Vector3 angleMaxDir = maxRotation * localNormal;

            Vector3 angleMinPoint = localOffset + angleMinDir * ROTATION_SPAN_END_LENGTH;
            Vector3 angleMaxPoint = localOffset + angleMaxDir * ROTATION_SPAN_END_LENGTH;

            Gizmos.color = minColor;
            Gizmos.DrawLine(localOffset, angleMinPoint);

            Gizmos.color = maxColor;
            Gizmos.DrawLine(localOffset, angleMaxPoint);

            Handles.color = spanColor;
            Handles.DrawWireArc(localOffset, localForward, angleMinDir, angleRange, ROTATION_SPAN_RADIUS);
#endif
        }

        /// <summary>
        /// Draws a span between two rotations (minimum and maximum) with a marker at each end and another one for the current value.
        /// </summary>
        /// <param name="minRotation">Minimum rotation.</param>
        /// <param name="maxRotation">Minimum rotation.</param>
        /// <param name="value">Value in the range of 0.0f to 1.0f.</param>
        /// <param name="minColor">Color of the minimum marker.</param>
        /// <param name="maxColor">Color of the maximum marker.</param>
        /// <param name="spanColor">Color of the span between min and max.</param>
        /// <param name="valueColor">Color of the value marker.</param>
        /// <param name="localOffset">Pivot offset of the angle.</param>
        /// <param name="localNormal">Normal of the plane where the markers are placed.</param>
        /// <param name="localForward">Direction in the which the angle arc is oriented.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawMinMaxValueRotationSpan(Quaternion minRotation, Quaternion maxRotation, float value, Color minColor, Color maxColor, Color spanColor,
                                                        Color valueColor, Vector3 localOffset, Vector3 localNormal, Vector3 localForward, Transform atTransform = null)
        {
            DrawMinMaxRotationSpan(minRotation, maxRotation, minColor, maxColor, spanColor, localOffset, localNormal, localForward, atTransform);

            Quaternion valueRotation = Quaternion.Lerp(minRotation, maxRotation, value);

            Vector3 angleValueDir = valueRotation * localNormal;

            Vector3 angleValuePoint = localOffset + angleValueDir * ROTATION_SPAN_VALUE_LENGTH;

            Gizmos.color = valueColor;
            Gizmos.DrawLine(localOffset, angleValuePoint);
        }
        /// <summary>
        /// Draws a text at a position.
        /// Defaults to white text with size 11 and text anchor at the top left.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <param name="position">Position to draw at.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawLabel(string text, Vector3 position, Transform atTransform = null)
        {
            DrawLabel(text, position, Color.white, FontStyle.Normal, DEFAULT_LABEL_SIZE, TextAnchor.UpperLeft, atTransform);
        }


        /// <summary>
        /// Draws a text at a position with the given color, fontSize, fontStyle and text anchor.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <param name="position">Position to draw at.</param>
        /// <param name="color">Color of the text.</param>
        /// <param name="fontSize">Font size of the text.</param>
        /// <param name="fontStyle">Font style (normal, italic, bold).</param>
        /// <param name="alignment">Alignment of the text.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawLabel(string text, Vector3 position, Color color, FontStyle fontStyle = FontStyle.Normal, int fontSize = DEFAULT_LABEL_SIZE,
                                        TextAnchor alignment = TextAnchor.UpperLeft, Transform atTransform = null)
        {
            GUIStyle guiStyle = new()
            {
                fontSize = fontSize,
                fontStyle = fontStyle,
                alignment = alignment
            };
            guiStyle.normal.textColor = color;

            DrawLabel(text, position, guiStyle, atTransform);
        }

        /// <summary>
        /// Draws a text at a position with the provided GuiStyle.
        /// If the guiStyle is null, GuiStyle.none will be used.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <param name="position">Position to draw at.</param>
        /// <param name="guiStyle">GuiStyle for the text.</param>
        /// <param name="atTransform">Transform context to draw the gizmo.</param>
        public static void DrawLabel(string text, Vector3 position, GUIStyle guiStyle, Transform atTransform = null)
        {
#if UNITY_EDITOR
            Handles.matrix = atTransform != null ? atTransform.localToWorldMatrix : Matrix4x4.identity;
            Handles.Label(position, text, guiStyle);
#endif
        }
    }
}