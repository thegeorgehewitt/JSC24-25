using UnityEngine;

namespace Custom.Utility
{
    public static class Offset
    {
        public enum SideOffset
        {
            Left,
            Right,
            Down,
            Up
        }
        public enum CornerOffset
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight
        }

        public static readonly Vector2[] corners = new Vector2[]
        {
            new Vector2(-1, -1),    // Bottom Left
            new Vector2(-1, 1),     // Top Left
            new Vector2(1, 1),      // Top Right
            new Vector2(1, -1),     // Bottom Right
        };

        public static readonly Vector2[] sides = new Vector2[]
        {
            new Vector2(-1, 0),     // Left
            new Vector2(1, 0),      // Right
            new Vector2(0, -1),     // Down
            new Vector2(0, 1),      // Up
        };
    }
}