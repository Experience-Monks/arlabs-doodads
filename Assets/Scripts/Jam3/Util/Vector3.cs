using System.Collections.Generic;

using UnityEngine;

namespace Jam3.Util
{
    public static class Vector3Extensions
    {
        public static Vector3 Spring(Vector3 from, Vector3 to, float time)
        {
            return new Vector3(MathUtil.Spring(from.x, to.x, time), MathUtil.Spring(from.y, to.y, time), MathUtil.Spring(from.z, to.z, time));
        }

        public static Vector2 xz(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3[] Offset(this Vector3[] vs, Vector3 offset)
        {
            List<Vector3> list = new List<Vector3>();
            foreach (Vector3 v in vs)
            {
                list.Add(v + offset);
            }
            return list.ToArray();
        }

        public static Vector2[] xz(this Vector3[] vs)
        {
            List<Vector2> list = new List<Vector2>();
            foreach (Vector3 v in vs)
            {
                list.Add(v.xz());
            }
            return list.ToArray();
        }
    }
}
