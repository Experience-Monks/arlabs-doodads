using UnityEngine;

namespace Jam3.Util
{
    public class UnitUtil
    {
        public static string UnitToInches(float value)
        {
            //0.3048 ft
            //0.0254 in
            string inchesValue = value + " \"";
            inchesValue = Round(value / 0.0254f, 1) + " \"";
            return inchesValue;
        }

        public static string UnitToMeters(float value)
        {
            string metersValue = value + " m";
            if (value < 1.0f)
            {
                value *= 100f;
                metersValue = Round(value, 1) + " cm";
            }
            else
            {
                metersValue = Round(value, 1) + " m";
            }

            return metersValue;
        }

        public static float Round(float value, int digits)
        {
            float mult = Mathf.Pow(10.0f, (float)digits);
            return Mathf.Round(value * mult) / mult;
        }
    }
}
