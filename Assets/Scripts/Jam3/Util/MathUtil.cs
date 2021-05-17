using UnityEngine;

namespace Jam3.Util
{
    public class MathUtil
    {
        public static float Spring(float from, float to, float time)
        {
            time = Mathf.Clamp01(time);
            time = (Mathf.Sin(time * Mathf.PI * (.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + (1.2f * (1f - time)));
            return from + (to - from) * time;
        }

        public static bool CoinFlip
        {
            get {
                return UnityEngine.Random.value < 0.5f;
            }
        }
    }
}
