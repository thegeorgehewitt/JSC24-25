using UnityEngine;
using UnityEditor;

namespace Custom.Editor
{
    public static class CustomSceneViewUtil
    {
        /// <summary>
        /// Draws a solid triangular arrowhead in the Scene view.
        /// </summary>
        /// <param name="_tip">         The position of the arrow tip. </param>
        /// <param name="_direction">   The direction the arrow is pointing. </param>
        /// <param name="_size">        Size of the arrowhead. </param>
        /// <param name="_color">       Color of the arrowhead. </param>
        public static void DrawSolidArrowhead(Vector3 _tip, Vector3 _direction, float _size = 0.2f, Color? _color = null)
        {
            if (_direction == Vector3.zero) return;

            _direction.Normalize();

            Color originalColor = Handles.color;
            Handles.color = _color ?? originalColor;

            Vector3 baseCenter = _tip - _direction.normalized * _size;                          // Base center
            Vector3 right = Quaternion.AngleAxis(30, Vector3.forward) * (baseCenter - _tip);    // Right side
            Vector3 left = Quaternion.AngleAxis(-30, Vector3.forward) * (baseCenter - _tip);    // Left side

            Handles.DrawAAConvexPolygon(new Vector3[] { _tip, _tip + right, _tip + left });

            Handles.color = originalColor;
        }

        /// <summary>
        /// Draws a gradient-colored line in the Scene view between two points.
        /// </summary>
        /// <param name="_start">   The starting position of the line. </param>
        /// <param name="_end">     The ending position of the line. </param>
        /// <param name="_colorA">  The color at the start of the line. </param>
        /// <param name="_colorB">  The color at the end of the line. </param>
        public static void DrawGradientLine(Vector3 _start, Vector3 _end, Color? _colorA = null, Color? _colorB = null)
        {
            Color startColor = _colorA ?? Color.white;
            Color endColor = _colorB ?? Color.white;

            // Use Unity's built-in line rendering material.
            Material lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);

            lineMaterial.SetPass(0);

            // Since Handles does not allow per vertex coloring, we have to use GL instead.
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            GL.Color(startColor);
            GL.Vertex(_start);
            GL.Color(endColor);
            GL.Vertex(_end);
            GL.End();
            GL.PopMatrix();
        }
    }
}
