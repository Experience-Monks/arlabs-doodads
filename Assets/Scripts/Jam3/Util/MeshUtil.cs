using UnityEngine;
using System.Text;

namespace Jam3.Util
{
    /// <summary>
    /// Mesh util.
    /// </summary>
    public class MeshUtil
    {
        private static int StartIndex = 0;

        /// <summary>
        /// Start.
        /// </summary>
        public static void Start()
        {
            StartIndex = 0;
        }

        /// <summary>
        /// End.
        /// </summary>
        public static void End()
        {
            StartIndex = 0;
        }

        /// <summary>
        /// Meshes to string.
        /// </summary>
        /// <param name="mf">The mf.</param>
        /// <param name="t">The t.</param>
        public static string MeshToString(MeshFilter mf, Transform t)
        {
            Vector3 s = t.localScale;
            Vector3 p = t.localPosition;
            Quaternion r = t.localRotation;

            int numVertices = 0;
            Mesh m = mf.sharedMesh;
            if (!m)
            {
                return "####Error####";
            }
            Material[] mats = mf.GetComponent<MeshRenderer>().sharedMaterials;

            StringBuilder sb = new StringBuilder();

            foreach (Vector3 vv in m.vertices)
            {
                Vector3 v = t.TransformPoint(vv);
                numVertices++;
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
            }
            sb.Append("\n");
            foreach (Vector3 nn in m.normals)
            {
                Vector3 v = r * nn;
                sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in m.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (int material = 0; material < m.subMeshCount; material++)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                sb.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1 + StartIndex, triangles[i + 1] + 1 + StartIndex, triangles[i + 2] + 1 + StartIndex));
                }
            }

            StartIndex += numVertices;
            return sb.ToString();
        }
    }
}
