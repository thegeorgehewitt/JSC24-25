using UnityEngine;

namespace Custom.FSM
{
    [System.Serializable]
    public class StateMachineParameter
    {
        public string name;
        public StateMachineParameterType type;

        public bool defaultBool = false;
        public string defaultString = "";
        public int defaultInt = 0;
        public float defaultFloat = 0.0f;
        public Vector2 defaultVector2 = Vector2.zero;
        public Vector3 defaultVector3 = Vector3.zero;
        public Vector4 defaultVector4 = Vector4.zero;
        public Texture2D defaultTexture2D = null;
        public Sprite defaultSprite = null;
        public GameObject defaultGameObject = null;

        public int NameHash => name.GetHashCode();
    }


    
    /// <summary>
    /// All supported parameter types for state machine.
    /// </summary>
    public enum StateMachineParameterType
    {
        Bool,

        String,

        Int,

        Float,

        Vector2,

        Vector3,

        Vector4,

        Texture2D,

        Sprite,

        GameObject,
    }
}
