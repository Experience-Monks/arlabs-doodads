using System.Collections.Generic;

using UnityEngine;

namespace Jam3
{
    /// <summary>
    /// Mesh optmizer.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class MeshOptmizer : MonoBehaviour
    {
        public MeshFilter MeshObject = null;
        public MeshCollider MeshCollider = null;

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            if (MeshObject != null)
            {
                List<bool> insideOrNot = new List<bool>();

                Mesh mesh = MeshObject.mesh;
                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;
                List<int> newTriangles = new List<int>();

                Vector3 diff = Vector3.zero;
                Vector3 prev = vertices[0];
                Vector3 pos = Vector3.zero;

                // vari
                float zdiff = 0;
                Vector3 posHeight = new Vector3(-1000, -1000, -1000);

                for (int i = 0; i < vertices.Length; i++)
                {
                    diff = vertices[i] - prev;
                    zdiff = vertices[i].z - prev.z;

                    pos = vertices[i];

                    Debug.Log(i + " :: " + pos.z);

                    // pos.z -= 20.24f;

                    if (Mathf.Abs(vertices[i].x - posHeight.x) > 0.05f)
                        posHeight.x = vertices[i].x;

                    if (Mathf.Abs(vertices[i].y - posHeight.y) > 0.05f)
                        posHeight.y = vertices[i].y;

                    if (Mathf.Abs(vertices[i].z - posHeight.z) > 0.1f)
                        posHeight.z = vertices[i].z;

                    if (diff.magnitude < 1.3f && pos.z < 5f && pos.z > 0.1f && zdiff < 0.5f)
                        insideOrNot.Add(true);
                    else
                        insideOrNot.Add(false);

                    prev = vertices[i];

                    vertices[i].x = posHeight.x;
                    vertices[i].y = posHeight.y;
                    vertices[i].z = posHeight.z;
                }

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    if (insideOrNot[triangles[i]] && insideOrNot[triangles[i + 1]] && insideOrNot[triangles[i + 2]])
                    {
                        newTriangles.Add(triangles[i]);
                        newTriangles.Add(triangles[i + 1]);
                        newTriangles.Add(triangles[i + 2]);
                    }
                }

                mesh.vertices = vertices;
                mesh.triangles = newTriangles.ToArray();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.UploadMeshData(false);

                if (MeshCollider != null)
                {
                    MeshCollider.sharedMesh = null;
                    MeshCollider.sharedMesh = mesh;
                }

                // BoxCollider BoxCollider = gameObject.AddComponent<BoxCollider>();
                // if (BoxCollider != null)
                // {
                //     BoxCollider.gameObject.transform.position = MeshCollider.bounds.center;
                //     BoxCollider.size = MeshCollider.bounds.size;
                // }
            }
        }
    }
}
