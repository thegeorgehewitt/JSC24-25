using UnityEngine;

namespace Custom.Attribute
{
    public class LimitCharacterAttribute : PropertyAttribute
    {
        public int charLimit;

        public LimitCharacterAttribute()
            : this(30) { }

        public LimitCharacterAttribute(int _limit)
        {
            charLimit = _limit;
        }
    }
}
