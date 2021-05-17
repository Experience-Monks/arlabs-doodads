//-----------------------------------------------------------------------
// <copyright file="ARCameraEffect.cs" company="Jam3 Inc">
//
// Copyright 2021 Jam3 Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

namespace Jam3.AR
{
    /// <summary>
    /// A r camera effect.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class ARCameraEffect : MonoBehaviour
    {
        public bool EffectReady { get { return m_effectReady; } set { m_effectReady = value; } }
        public bool IsOverMesh { get { return m_isOverMesh; } set { m_isOverMesh = value; } }
        public float DepthConfidenceAverage { get { return m_depthConfidenceAverage; } set { m_depthConfidenceAverage = value; } }

        [Header("Editor Background")]
        public bool UseEditorBackground = true;
        public Texture2D EditorBackgroundTextureTexture = null;
        public Material EditorBackgroundMaterial = null;

        [Header("Effect")]
        public Color ColorIdle = Color.red;
        public Color ColorReady = Color.green;

        public bool RenderEffect = true;
        public Shader EffectShader = null;
        public float TriangleConnectivityCutOff = 5.0f;
        public Texture2D MaterialTexture = null;

        // Runtime static names
        private static readonly string k_RawDepthTexturePropertyName = "_RawDepthTexture";
        private static readonly string k_DepthTexturePropertyName = "_DepthTexture";
        private static readonly string k_ConfidenceTexturePropertyName = "_ConfidenceTexture";

        private static readonly string k_BackgroundTexturePropertyName = "_BackgroundTexture";
        private static readonly string k_VertexModelTransformPropertyName = "_VertexModelTransform";

        // Runtime Variables
        private Camera m_camera;
        private Material m_effectMaterial;
        private CommandBuffer m_commandBuffer;

        private bool m_Initialized = false;
        private bool m_effectReady = false;
        private bool m_isOverMesh = false;
        private float m_depthConfidenceAverage = 0.0f;

        /// <summary>
        /// Awake.
        /// </summary>
        private void Awake()
        {
            if (EffectShader != null)
                m_effectMaterial = new Material(EffectShader);

            m_camera = GetComponent<Camera>();
            m_camera.depthTextureMode |= DepthTextureMode.Depth;
            m_commandBuffer = new CommandBuffer { name = "Camera texture" };
        }

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            var backgroundRenderer = FindObjectOfType<ARCameraBackground>();
            if (backgroundRenderer == null)
            {
                Debug.LogError("OcclusionImageEffect requires ARCameraBackground anywhere in the scene.");
                return;
            }

            if (Application.isEditor)
            {
                if (UseEditorBackground)
                {
                    if (EditorBackgroundMaterial != null)
                    {
                        EditorBackgroundMaterial.SetTexture("_MainTex", EditorBackgroundTextureTexture);
                        m_commandBuffer.Blit(null, BuiltinRenderTextureType.CameraTarget, EditorBackgroundMaterial);
                    }

                    m_commandBuffer.SetGlobalTexture(k_BackgroundTexturePropertyName, EditorBackgroundTextureTexture);
                }
            }
            else
            {
                int backgroundTextureID = Shader.PropertyToID(k_BackgroundTexturePropertyName);
                m_commandBuffer.GetTemporaryRT(backgroundTextureID, -1, -1, 0, FilterMode.Bilinear);

                var backgroundMaterial = backgroundRenderer.material;

                m_commandBuffer.Blit(backgroundMaterial.mainTexture, backgroundTextureID, backgroundMaterial);
                m_commandBuffer.SetGlobalTexture(k_BackgroundTexturePropertyName, backgroundTextureID);
            }
        }

        /// <summary>
        /// Initializes effect.
        /// </summary>
        private void InitializeEffect()
        {
            if (m_effectMaterial != null)
            {
                m_effectMaterial.SetTexture(k_RawDepthTexturePropertyName, ARCameraInfo.RawDepthTexture);
                m_effectMaterial.SetTexture(k_DepthTexturePropertyName, ARCameraInfo.DepthTexture);
                m_effectMaterial.SetTexture(k_ConfidenceTexturePropertyName, ARCameraInfo.ConfidenceTexture);

                m_effectMaterial.SetVector("_Color", ColorIdle);
                m_effectMaterial.SetVector("_ColorReady", ColorReady);

                m_effectMaterial.SetFloat("_FocalLengthX", ARCameraInfo.FocalLength.x);
                m_effectMaterial.SetFloat("_FocalLengthY", ARCameraInfo.FocalLength.y);
                m_effectMaterial.SetFloat("_PrincipalPointX", ARCameraInfo.PrincipalPoint.x);
                m_effectMaterial.SetFloat("_PrincipalPointY", ARCameraInfo.PrincipalPoint.y);
                m_effectMaterial.SetInt("_ImageDimensionsX", ARCameraInfo.ImageDimensions.x);
                m_effectMaterial.SetInt("_ImageDimensionsY", ARCameraInfo.ImageDimensions.y);

                if (MaterialTexture != null)
                    m_effectMaterial.SetTexture("_EffectTexture", MaterialTexture);
            }

            m_Initialized = true;
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            if (m_Initialized)
            {
                m_effectMaterial.SetFloat("_TriangleConnectivityCutOff", TriangleConnectivityCutOff);

                m_effectMaterial.SetMatrix(k_VertexModelTransformPropertyName, ARCameraInfo.LocalToWorldMatrix);
                m_effectMaterial.SetTexture(k_RawDepthTexturePropertyName, ARCameraInfo.RawDepthTexture);
                m_effectMaterial.SetTexture(k_DepthTexturePropertyName, ARCameraInfo.DepthTexture);
                m_effectMaterial.SetTexture(k_ConfidenceTexturePropertyName, ARCameraInfo.ConfidenceTexture);

                m_effectMaterial.SetFloat("_isOver", IsOverMesh ? 1.0f : 0.0f);
                m_effectMaterial.SetFloat("_Amount", DepthConfidenceAverage);
            }

            if (!m_Initialized && ARCameraInfo.Initialized)
            {
                InitializeEffect();
            }
        }

        /// <summary>
        /// On enable.
        /// </summary>
        private void OnEnable()
        {
            if (m_commandBuffer != null)
                m_camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_commandBuffer);
        }

        /// <summary>
        /// On disable.
        /// </summary>
        private void OnDisable()
        {
            if (m_commandBuffer != null)
                m_camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_commandBuffer);
        }

        /// <summary>
        /// On render image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (RenderEffect && m_Initialized && m_commandBuffer != null && m_effectMaterial != null)
                Graphics.Blit(source, destination, m_effectMaterial);
        }
    }
}
