using UnityEngine;
using System.Text;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jam3.Util
{
    /// <summary>
    /// Runtime obj exporter.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class RuntimeObjExporter : MonoBehaviour
    {
        public string RootObjectName = "BackgroundMeshContainer";

        /// <summary>
        /// Dos export.
        /// </summary>
        /// <param name="makeSubmeshes">The make submeshes.</param>
        public void DoExport(bool makeSubmeshes)
        {
            GameObject rootObject = GameObject.Find(RootObjectName);
            if (rootObject != null)
            {
                string meshName = rootObject.name;
                string fileName = Application.persistentDataPath + "/" + meshName + ".obj";

                MeshUtil.Start();

                StringBuilder meshString = new StringBuilder();

                meshString.Append("#" + meshName + ".obj"
                                    + "\n#" + System.DateTime.Now.ToLongDateString()
                                    + "\n#" + System.DateTime.Now.ToLongTimeString()
                                    + "\n#-------"
                                    + "\n\n");

                Transform t = rootObject.transform;
                Vector3 originalPosition = t.position;
                t.position = Vector3.zero;

                if (!makeSubmeshes)
                    meshString.Append("g ").Append(t.name).Append("\n");

                meshString.Append(processTransform(t, makeSubmeshes));

                WriteToFile(meshString.ToString(), fileName);

                t.position = originalPosition;

                MeshUtil.End();

                Debug.Log("Exported Mesh: " + fileName);
            }
        }

        /// <summary>
        /// Processes transform.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="makeSubmeshes">The make submeshes.</param>
        static string processTransform(Transform t, bool makeSubmeshes)
        {
            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + t.name
                            + "\n#-------"
                            + "\n");

            if (makeSubmeshes)
            {
                meshString.Append("g ").Append(t.name).Append("\n");
            }

            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf)
            {
                meshString.Append(MeshUtil.MeshToString(mf, t));
            }

            for (int i = 0; i < t.childCount; i++)
            {
                meshString.Append(processTransform(t.GetChild(i), makeSubmeshes));
            }

            return meshString.ToString();
        }

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="filename">The filename.</param>
        static void WriteToFile(string s, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(s);
            }
        }

#if UNITY_EDITOR
        // Debug Editor
        [CustomEditor(typeof(RuntimeObjExporter))]
        public class RuntimeObjExporterEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (!Application.isPlaying) return;

                RuntimeObjExporter t = target as RuntimeObjExporter;
                if (GUILayout.Button("Export"))
                {
                    t.DoExport(true);
                }
            }
        }
#endif
    }
}
