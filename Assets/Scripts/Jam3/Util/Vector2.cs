using System.Collections.Generic;

using UnityEngine;

namespace Jam3.Util
{
    /// <summary>
    /// Vector2 extensions.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Angles between.
        /// </summary>
        /// <param name="vec1">The vec1.</param>
        /// <param name="vec2">The vec2.</param>
        public static float AngleBetween(Vector2 vec1, Vector2 vec2)
        {
            Vector2 diference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
            return Vector2.Angle(Vector2.right, diference) * sign;
        }

        /// <summary>
        /// Angles between linear.
        /// </summary>
        /// <param name="vec1">The vec1.</param>
        /// <param name="vec2">The vec2.</param>
        public static float AngleBetweenLinear(Vector2 vec1, Vector2 vec2)
        {
            Vector2 diference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
            return (Vector2.Angle(Vector2.right, diference) * sign) + 180f;
        }

        /// <summary>
        /// Area.
        /// </summary>
        /// <param name="v">The v.</param>
        public static float Area(this Vector2 v)
        {
            return v.x * v.y;
        }

        /// <summary>
        /// Are contained in polygon.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="polygon">The polygon.</param>
        public static bool IsContainedInPolygon(this Vector2 p, Vector2[] polygon)
        {
            var j = polygon.Length - 1;
            var inside = false;

            for (int i = 0; i < polygon.Length; j = i++)
            {
                var pi = polygon[i];
                var pj = polygon[j];
                if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                    (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                    inside = !inside;
            }

            return inside;
        }

        /// <summary>
        /// Lerp.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="t">The t.</param>
        public static float Lerp(this Vector2 v, float t)
        {
            return Mathf.Lerp(v.x, v.y, t);
        }

        /// <summary>
        /// Contains.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="val">The val.</param>
        public static bool Contains(this Vector2 v, float val)
        {
            return val >= v.x && val <= v.y;
        }

        /// <summary>
        /// Adds y component.
        /// </summary>
        /// <param name="vs">The vs.</param>
        /// <param name="y">The y.</param>
        public static Vector3[] AddYComponent(this Vector2[] vs, float y)
        {
            List<Vector3> list = new List<Vector3>();
            foreach (Vector2 v in vs)
            {
                list.Add(new Vector3(v.x, y, v.y));
            }
            return list.ToArray();
        }

        /// <summary>
        /// To world space.
        /// </summary>
        /// <param name="vs">The vs.</param>
        /// <param name="parentTransform">The parent transform.</param>
        public static Vector2[] ToWorldSpace(this Vector2[] vs, Transform parentTransform)
        {
            List<Vector2> list = new List<Vector2>();
            foreach (Vector2 v in vs)
            {
                list.Add(parentTransform.TransformPoint(v.x, 0f, v.y).xz());
            }
            return list.ToArray();
        }
    }
}
