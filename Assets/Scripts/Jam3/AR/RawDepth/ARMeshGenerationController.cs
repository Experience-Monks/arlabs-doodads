using System;
using System.Collections.Generic;

using UnityEngine;

namespace Jam3.AR
{
    public class ARMeshGenerationController : MonoBehaviour
    {
        public float DepthConfidenceAverage { get { return m_averageColor; } }
        public bool IsOverMesh { get { return m_isOverMesh; } }
        public bool IsReady { get { return m_isReady; } }
        public Action OnScanComplete = null;
        public Action OnMeshScanCompleted = null;

        //Public Variables
        [Header("Config")]
        public Layers MeshLayer = Layers.BackgroundMesh;
        public bool CalculateNormals = false;
        public bool AutomaticSpawn = true;

        [Range(0.0f, 1.0f)]
        public float DepthThreshold = 0.8f;

        [Range(1, 20)]
        public int MaxSteps = 4;

        [Range(0.1f, 10f)]
        public float CreateTime = 1f;

        [Header("Camera")]
        public GameObject SceneCamera = null;
        public CameraCollision CameraCollision = null;

        [Header("Mesh")]
        public bool ShowMesh = false;
        public GameObject MeshObject = null;
        public bool CleanMesh = false;
        public bool FullClean = false;
        public bool LevelCleanedMesh = false;

        [Header("Depth Shader")]
        public ComputeShader DepthProcessingCS = null;

        [Header("Bit Options")]
        public RenderTexture BlitRenderTexture = null;
        public Material BlitMaterial = null;
        public float BlitTime = 1f;

        //Private Variables
        private const int k_NumThreadsX = 8;
        private const int k_NumThreadsY = 8;
        private const int k_NormalSamplingOffset = 1;

        private GameObject m_Root = null;
        private List<GameObject> m_MeshObjects = new List<GameObject>();

        private int m_DepthPixelSkippingX = 6;
        private int m_DepthPixelSkippingY = 6;

        private bool m_Initialized = false;
        private int m_MeshWidth;
        private int m_MeshHeight;
        private int m_NumElements;

        private int m_VertexFromDepthHandle;
        private int m_NormalFromVertexHandle;

        private ComputeBuffer m_VertexBuffer;
        private ComputeBuffer m_NormalBuffer;

        private Texture2D m_tempTexture = null;
        private float m_blitTimeCount = 0.0f;
        private float m_averageColor = 0;
        private int m_meshCount = 0;
        private bool m_isReady = false;
        private bool m_isOverMesh = false;

        private float m_createTimeCount = 0.0f;
        private bool m_scanning = false;

        private Vector3 cameraPosition = Vector3.zero;

        private void Start()
        {
            if (SceneCamera == null)
                SceneCamera = Camera.main.gameObject;

            m_Root = new GameObject();
            m_Root.name = "BackgroundMeshContainer";

            if (BlitRenderTexture != null)
                m_tempTexture = new Texture2D(BlitRenderTexture.width, BlitRenderTexture.height, TextureFormat.RGBA32, false);
        }

        void Update()
        {
            if (ARCameraInfo.Initialized && !m_Initialized)
            {
                m_MeshWidth = ARCameraInfo.DepthWidth / m_DepthPixelSkippingX;
                m_MeshHeight = ARCameraInfo.DepthHeight / m_DepthPixelSkippingY;
                m_NumElements = m_MeshWidth * m_MeshHeight;

                InitializeComputeShader();
                m_Initialized = true;
                m_blitTimeCount = BlitTime;
            }

            if (m_scanning && m_Initialized && AutomaticSpawn && m_meshCount < MaxSteps)
            {
                if (CameraCollision != null)
                {
                    UpdateDepthData();
                    m_isReady = false;
                    m_isOverMesh = CameraCollision.IsOverMesh;

                    if (!m_isOverMesh && m_averageColor >= DepthThreshold)
                    {
                        m_isReady = true;

                        m_createTimeCount += Time.deltaTime;
                        if (m_createTimeCount >= CreateTime)
                        {
                            m_createTimeCount = 0f;
                            SpawnMesh();
                        }
                    }
                }
            }
        }

        public void StartScanning()
        {
            if (CameraCollision != null)
                CameraCollision.Reset();

            m_createTimeCount = 0f;

            ClearMeshes();
            m_scanning = true;
        }

        public void SpawnMesh()
        {
            if (MeshObject != null)
            {
                Vector3 rotation = Vector3.zero;
                GameObject meshObject = Instantiate(MeshObject, new Vector3(0f, 0f, 0f), Quaternion.Euler(rotation), m_Root.transform);
                meshObject.layer = (int)MeshLayer;
                m_MeshObjects.Add(meshObject);

                if (!ShowMesh)
                    meshObject.GetComponent<MeshRenderer>().enabled = false;

                InitializeMesh(meshObject);

                if (OnMeshScanCompleted != null)
                    OnMeshScanCompleted();

                m_meshCount++;

                if (m_meshCount >= MaxSteps)
                {
                    m_scanning = false;

                    if (OnScanComplete != null)
                        OnScanComplete();
                }
            }
        }

        private void UpdateDepthData()
        {
            if (m_Initialized)
            {
                m_blitTimeCount += Time.deltaTime;

                if (m_blitTimeCount >= BlitTime)
                {
                    m_blitTimeCount = 0.0f;

                    if (BlitRenderTexture != null)
                    {
                        if (BlitMaterial != null)
                            Graphics.Blit(ARCameraInfo.RawDepthTexture, BlitRenderTexture, BlitMaterial);
                        else
                            Graphics.Blit(ARCameraInfo.RawDepthTexture, BlitRenderTexture);

                        if (m_tempTexture != null)
                        {
                            RenderTexture.active = BlitRenderTexture;
                            m_tempTexture.ReadPixels(new Rect(0, 0, m_tempTexture.width, m_tempTexture.height), 0, 0);
                            RenderTexture.active = null;

                            m_averageColor = 0;
                            int count = 0;

                            for (int x = 0; x < m_tempTexture.width; x++)
                            {
                                for (int y = 0; y < m_tempTexture.height; y++)
                                {
                                    float value = m_tempTexture.GetPixel(x, y).grayscale;
                                    m_averageColor += value;
                                    count++;
                                }
                            }

                            m_averageColor /= count;
                        }
                    }
                }
            }
        }

        private void InitializeMesh(GameObject meshObject)
        {
            // Creates template vertices.
            Vector3[] m_Vertices = new Vector3[m_NumElements];
            Vector3[] m_Normals = new Vector3[m_NumElements];

            // Creates template vertices for the mesh object.
            for (int y = 0; y < m_MeshHeight; y++)
            {
                for (int x = 0; x < m_MeshWidth; x++)
                {
                    int index = (y * m_MeshWidth) + x;
                    Vector3 v = new Vector3(x * 0.01f, -y * 0.01f, 0);
                    m_Vertices[index] = v;
                    m_Normals[index] = Vector3.back;
                }
            }

            // Creates template triangle list.
            int[] triangles = GenerateTriangles(m_MeshWidth, m_MeshHeight);

            // Creates the mesh object and set all template data.
            Mesh mesh = new Mesh();
            mesh.MarkDynamic();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(m_Vertices);
            mesh.SetNormals(m_Normals);
            mesh.SetTriangles(triangles, 0);
            mesh.bounds = new UnityEngine.Bounds(Vector3.zero, new Vector3(50, 50, 50));
            mesh.UploadMeshData(false);

            MeshFilter mf = meshObject.GetComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            MeshCollider meshCollider = meshObject.GetComponent<MeshCollider>();

            UpdateMesh(mesh, m_Vertices, m_Normals, meshCollider);
        }

        private void UpdateMesh(Mesh mesh, Vector3[] vertices, Vector3[] normals, MeshCollider meshCollider)
        {
            if (DepthProcessingCS != null)
            {
                m_VertexBuffer = new ComputeBuffer(m_NumElements, sizeof(float) * 3);
                m_NormalBuffer = new ComputeBuffer(m_NumElements, sizeof(float) * 3);

                Texture2D depthTexture = ARCameraInfo.RawDepthTexture;

                if (depthTexture != null)
                    DepthProcessingCS.SetTexture(m_VertexFromDepthHandle, "depthTex", depthTexture);

                DepthProcessingCS.SetBuffer(m_VertexFromDepthHandle, "vertexBuffer", m_VertexBuffer);
                DepthProcessingCS.SetMatrix("ModelTransform", ARCameraInfo.LocalToWorldMatrix);
                DepthProcessingCS.Dispatch(m_VertexFromDepthHandle, m_MeshWidth / k_NumThreadsX, (m_MeshHeight / k_NumThreadsY) + 1, 1);

                m_VertexBuffer.GetData(vertices);
                mesh.SetVertices(vertices);

                if (CalculateNormals)
                {
                    DepthProcessingCS.SetBuffer(m_NormalFromVertexHandle, "vertexBuffer", m_VertexBuffer);
                    DepthProcessingCS.SetBuffer(m_NormalFromVertexHandle, "normalBuffer", m_NormalBuffer);
                    DepthProcessingCS.Dispatch(m_NormalFromVertexHandle, m_MeshWidth / k_NumThreadsX, (m_MeshHeight / k_NumThreadsY) + 1, 1);

                    m_NormalBuffer.GetData(vertices);
                    mesh.SetNormals(normals);
                }

                cameraPosition = SceneCamera.transform.position;

                if (CleanMesh)
                    mesh = OptimzeMesh(mesh);

                mesh.RecalculateNormals();
                mesh.UploadMeshData(false);

                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
            }
        }

        private Mesh OptimzeMesh(Mesh mesh)
        {
            List<bool> insideOrNot = new List<bool>();
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
                if (FullClean)
                {
                    diff = vertices[i] - prev;
                    zdiff = vertices[i].z - prev.z;
                    pos = vertices[i] - cameraPosition;
                }

                if (LevelCleanedMesh)
                {
                    if (Mathf.Abs(vertices[i].x - posHeight.x) > 0.05f)
                        posHeight.x = vertices[i].x;

                    if (Mathf.Abs(vertices[i].y - posHeight.y) > 0.05f)
                        posHeight.y = vertices[i].y;

                    if (Mathf.Abs(vertices[i].z - posHeight.z) > 0.1f)
                        posHeight.z = vertices[i].z;
                }

                bool validation = FullClean ? (vertices[i].magnitude > 0.0f && diff.magnitude < 3.0f && pos.z < 10f && pos.z > 0.1f && zdiff < 0.6f) : vertices[i].magnitude > 0.0f;

                if (validation)
                    insideOrNot.Add(true);
                else
                    insideOrNot.Add(false);

                if (FullClean)
                    prev = vertices[i];

                if (LevelCleanedMesh)
                {
                    vertices[i].x = posHeight.x;
                    vertices[i].y = posHeight.y;
                    vertices[i].z = posHeight.z;
                }
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

            return mesh;
        }

        private void InitializeComputeShader()
        {
            if (DepthProcessingCS != null)
            {
                m_VertexFromDepthHandle = DepthProcessingCS.FindKernel("VertexFromDepth");

                if (CalculateNormals)
                    m_NormalFromVertexHandle = DepthProcessingCS.FindKernel("NormalFromVertex");

                // Sets general compute shader variables.
                DepthProcessingCS.SetInt("DepthWidth", ARCameraInfo.ImageDimensions.x);
                DepthProcessingCS.SetInt("DepthHeight", ARCameraInfo.ImageDimensions.y);
                DepthProcessingCS.SetFloat("PrincipalX", ARCameraInfo.PrincipalPoint.x);
                DepthProcessingCS.SetFloat("PrincipalY", ARCameraInfo.PrincipalPoint.y);
                DepthProcessingCS.SetFloat("FocalLengthX", ARCameraInfo.FocalLength.x);
                DepthProcessingCS.SetFloat("FocalLengthY", ARCameraInfo.FocalLength.y);
                DepthProcessingCS.SetInt("NormalSamplingOffset", k_NormalSamplingOffset);
                DepthProcessingCS.SetInt("DepthPixelSkippingX", m_DepthPixelSkippingX);
                DepthProcessingCS.SetInt("DepthPixelSkippingY", m_DepthPixelSkippingY);
                DepthProcessingCS.SetInt("MeshWidth", m_MeshWidth);
                DepthProcessingCS.SetInt("MeshHeight", m_MeshHeight);
                DepthProcessingCS.SetBool("ExtendEdges", false);
                DepthProcessingCS.SetFloat("EdgeExtensionOffset", 0.0f);
                DepthProcessingCS.SetFloat("EdgeExtensionDepthOffset", 0.0f);
            }
        }

        private static int[] GenerateTriangles(int width, int height)
        {
            int[] indices = new int[(height - 1) * (width - 1) * 6];
            int idx = 0;
            for (int y = 0; y < (height - 1); y++)
            {
                for (int x = 0; x < (width - 1); x++)
                {
                    //// Unity has a clockwise triangle winding order.
                    //// Upper quad triangle
                    //// Top left
                    int idx0 = (y * width) + x;
                    //// Top right
                    int idx1 = idx0 + 1;
                    //// Bottom left
                    int idx2 = idx0 + width;

                    //// Lower quad triangle
                    //// Top right
                    int idx3 = idx1;
                    //// Bottom right
                    int idx4 = idx2 + 1;
                    //// Bottom left
                    int idx5 = idx2;

                    indices[idx++] = idx0;
                    indices[idx++] = idx1;
                    indices[idx++] = idx2;
                    indices[idx++] = idx3;
                    indices[idx++] = idx4;
                    indices[idx++] = idx5;
                }
            }

            return indices;
        }

        private void OnDestroy()
        {
            ClearMeshes();

            if (m_VertexBuffer != null)
                m_VertexBuffer.Dispose();

            if (m_NormalBuffer != null)
                m_NormalBuffer.Dispose();

            if (m_Root != null)
            {
                Destroy(m_Root);
            }
            m_Root = null;
        }

        public void ClearMeshes()
        {
            foreach (GameObject go in m_MeshObjects)
            {
                Destroy(go);
            }
            m_MeshObjects.Clear();

            if (m_Root != null)
            {
                foreach (Transform child in m_Root.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            m_isReady = false;
            m_meshCount = 0;
        }

        public void Reset()
        {
            ClearMeshes();

            m_blitTimeCount = 0.0f;
            m_createTimeCount = 0.0f;

            m_scanning = false;
        }
    }
}
