using System.Collections.Generic;

using UnityEngine;

namespace Jam3.Util
{
    public static class Vector2Extensions
    {
        public static float AngleBetween(Vector2 vec1, Vector2 vec2)
        {
            Vector2 diference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
            return Vector2.Angle(Vector2.right, diference) * sign;
        }

        public static float AngleBetweenLinear(Vector2 vec1, Vector2 vec2)
        {
            Vector2 diference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
            return (Vector2.Angle(Vector2.right, diference) * sign) + 180f;
        }

        public static float Area(this Vector2 v)
        {
            return v.x * v.y;
        }

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

        public static float Lerp(this Vector2 v, float t)
        {
            return Mathf.Lerp(v.x, v.y, t);
        }

        public static bool Contains(this Vector2 v, float val)
        {
            return val >= v.x && val <= v.y;
        }

        public static Vector3[] AddYComponent(this Vector2[] vs, float y)
        {
            List<Vector3> list = new List<Vector3>();
            foreach (Vector2 v in vs)
            {
                list.Add(new Vector3(v.x, y, v.y));
            }
            return list.ToArray();
        }

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
