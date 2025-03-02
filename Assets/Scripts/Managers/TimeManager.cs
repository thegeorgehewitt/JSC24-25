using UnityEngine;

namespace Custom.Manager
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance;

        /// <summary>
        /// Use this property instead of directly using <see cref="Time.deltaTime"/> to enable
        /// other features of <see cref="TimeManager"> like time pause or slow-mo.
        /// </summary>
        public static float DeltaTime { get { return Time.deltaTime * TimeScale; } }

        /// <summary>
        /// Use this property instead of directly using <see cref="Time.fixedDeltaTime"/> to enable
        /// other features of <see cref="TimeManager"> like time pause or slow-mo.
        /// </summary>
        public static float FixedDeltaTime { get { return Time.fixedDeltaTime * TimeScale; } }

        /// <summary>
        /// Clamped to [0..1]. Value of 1 will run at normal speed. Value of 0 will completely pause time.
        /// </summary>
        public static float TimeScale { get; set; } = 1.0f;

        private float originalTimeScale; // Do we really need this?



        private void Awake()
        {
            #region Singleton
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion

            DontDestroyOnLoad(transform.root);

            TimeScale = Time.timeScale;
            originalTimeScale = Time.timeScale;
        }
    }
}
