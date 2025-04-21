using UnityEngine;

namespace Custom.Attribute
{
    public class MultilineAttribute : PropertyAttribute
    {
        public int minLines, maxLines;

        public MultilineAttribute()
            : this(0, 10) { }

        public MultilineAttribute(int _maxLines)
            : this(0, _maxLines) { }

        public MultilineAttribute(int _minLines, int _maxLines)
        {
            minLines = _minLines;
            maxLines = _maxLines;
        }
    }
}
