using UnityEngine;
using UnityEngine.UI;

namespace Custom.UI.General
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image fillMaskImage;

        [Header("SETTINGS")]
        [SerializeField] private FillMode fillMode;

        /// <summary>
        /// Clamped to [0..1].
        /// </summary>
        public float FillPercentage
        {
            get => fillMaskImage.fillAmount;
            set
            {
                fillMaskImage.fillAmount = Mathf.Clamp01(value);
            }
        }

        public Color FillColor
        {
            get => fillImage.color;
            set
            {
                fillImage.color = value;
            }
        }

        public Color BackgroundColor
        {
            get => backgroundImage.color;
            set
            {
                backgroundImage.color = value;
            }
        }

        public FillMode FillMode
        {
            get => fillMode;
            set
            {
                fillMode = value;
                switch (value)
                {
                    case FillMode.FromLeft:
                        fillMaskImage.fillMethod = Image.FillMethod.Horizontal;
                        fillMaskImage.fillOrigin = 0;
                        break;

                    case FillMode.FromRight:
                        fillMaskImage.fillMethod = Image.FillMethod.Horizontal;
                        fillMaskImage.fillOrigin = 1;
                        break;

                    case FillMode.FromTop:
                        fillMaskImage.fillMethod = Image.FillMethod.Vertical;
                        fillMaskImage.fillOrigin = 1;
                        break;

                    case FillMode.FromBottom:
                        fillMaskImage.fillMethod = Image.FillMethod.Vertical;
                        fillMaskImage.fillOrigin = 0;
                        break;

                    default: break;
                }
            }
        }



        private void Awake()
        {
            if (!backgroundImage || !fillImage || !fillMaskImage)
                enabled = false;

            FillMode = fillMode;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FillMode = fillMode;
        }
#endif
    }



    /// <summary>
    /// Fill mode of progress bar.
    /// </summary>
    public enum FillMode
    {
        /// <summary>
        /// Fill from left to right.
        /// </summary>
        FromLeft,

        /// <summary>
        /// Fill from right to left.
        /// </summary>
        FromRight,

        /// <summary>
        /// Fill from top to bottom.
        /// </summary>
        FromTop,

        /// <summary>
        /// Fill from bottom to top.
        /// </summary>
        FromBottom,
    }
}
