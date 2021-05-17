using UnityEngine;

namespace Jam3.Util
{
    /// <summary>
    /// Math util.
    /// </summary>
    public class MathUtil
    {
        /// <summary>
        /// Spring.
        /// </summary>
        /// <param name="from">The from.</param>
        /// <param name="to">The to.</param>
        /// <param name="time">The time.</param>
        public static float Spring(float from, float to, float time)
        {
            time = Mathf.Clamp01(time);
            time = (Mathf.Sin(time * Mathf.PI * (.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + (1.2f * (1f - time)));
            return from + (to - from) * time;
        }


        /// <summary>
        /// Gets the coin flip.
        /// </summary>
        /// <value>
        /// The coin flip.
        /// </value>
        public static bool CoinFlip
        {
            get {
                return UnityEngine.Random.value < 0.5f;
            }
        }
    }
}
