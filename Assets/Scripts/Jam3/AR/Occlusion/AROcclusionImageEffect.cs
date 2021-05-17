//-----------------------------------------------------------------------
// <copyright file="OcclusionImageEffect.cs" company="Google LLC">
//
// Copyright 2020 Google LLC
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
    /// A component that controls the full-screen occlusion effect.
    /// Exposes parameters to control the occlusion effect, which get applied every time update
    /// gets called.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class AROcclusionImageEffect : MonoBehaviour
    {
        [Header("Editor Background")]
        public bool UseEditorBackground = true;
        public Texture2D EditorBackgroundTextureTexture = null;
        public Material EditorBackgroundMaterial = null;

        [Space]
        public AROcclusionManager occlusionManager;
        public AROcclusionImageConfig config;

        /// <summary>
        /// The image effect shader to blit every frame with.
        /// </summary>
        [Space]
        public Shader OcclusionShader;

        private static readonly string DepthTexturePropertyName = "_CurrentDepthTexture";
        private static readonly string BackgroundTexturePropertyName = "_BackgroundTexture";

        private Camera _camera;
        private Material _occlusionMaterial;
        private CommandBuffer _commandBuffer;

        private AROcclusionImageConfig _currentConfig = null;
        private AROcclusionImageConfig _defaultConfig = null;

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
        }

        private void Awake()
        {
            Debug.Assert(OcclusionShader != null, "Occlusion Shader parameter must be set.");
            _occlusionMaterial = new Material(OcclusionShader);

            if (config != null)
                _defaultConfig = config;
            else
                _defaultConfig = gameObject.AddComponent(typeof(AROcclusionImageConfig)) as AROcclusionImageConfig;

            _currentConfig = _defaultConfig;
            UpdateOcclusionProperties();

            _camera = GetComponent<Camera>();
            _camera.depthTextureMode |= DepthTextureMode.Depth;

            _commandBuffer = new CommandBuffer { name = "Camera texture" };
        }

        void Start()
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
                        _commandBuffer.Blit(null, BuiltinRenderTextureType.CameraTarget, EditorBackgroundMaterial);
                    }

                    _commandBuffer.SetGlobalTexture(BackgroundTexturePropertyName, EditorBackgroundTextureTexture);
                }
            }
            else
            {
                int backgroundTextureID = Shader.PropertyToID(BackgroundTexturePropertyName);
                _commandBuffer.GetTemporaryRT(backgroundTextureID, -1, -1, 0, FilterMode.Bilinear);

                // Alternatively, can blit from BuiltinRenderTextureType.CameraTarget into
                // m_BackgroundTextureID, but make sure this is executed after the renderer is initialized.
                var backgroundMaterial = backgroundRenderer.material;

                _commandBuffer.Blit(backgroundMaterial.mainTexture, backgroundTextureID, backgroundMaterial);
                _commandBuffer.SetGlobalTexture(BackgroundTexturePropertyName, backgroundTextureID);
            }

            // Creates the occlusion map.
            int occlusionMapTextureID = Shader.PropertyToID("_OcclusionMap");
            _commandBuffer.GetTemporaryRT(occlusionMapTextureID, -1, -1, 0, FilterMode.Bilinear);

            // Pass #0 renders an auxiliary buffer - occlusion map that indicates the
            // regions of virtual objects that are behind real geometry.
            _commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, occlusionMapTextureID, _occlusionMaterial, 0);
            _commandBuffer.SetGlobalTexture("_OcclusionMap", occlusionMapTextureID);

            if (_occlusionMaterial != null)
            {
                if (Application.isEditor)
                {
                    _occlusionMaterial.SetFloat("_CorrectUV", 1.0f);
                    SetOcclusionEnabled(false);
                }
                else
                {
                    _occlusionMaterial.SetFloat("_CorrectUV", 0.0f);
                    SetOcclusionEnabled(true);
                }
            }
        }

        public void SetOcclusionEnabled(bool value = true)
        {
            if (_occlusionMaterial != null)
                _occlusionMaterial.SetFloat("_OcclusionDisabled", value ? 0.0f : 1.0f);

            isEnabled = value;
        }

        public void SetOcclusionConfig(AROcclusionImageConfig value = null)
        {
            if (value == null)
                _currentConfig = _defaultConfig;
            else
                _currentConfig = value;

            UpdateOcclusionProperties();
        }

        public void UpdateOcclusionTransparency(float value)
        {
            _currentConfig.OcclusionTransparency = value;
            UpdateOcclusionProperties();
        }

        public void UpdateOcclusionOffset(float value)
        {
            _currentConfig.OcclusionOffset = value;
            UpdateOcclusionProperties();
        }

        public void UpdateOcclusionFadeVelocity(float value)
        {
            _currentConfig.OcclusionFadeVelocity = value;
        }

        public void UpdateTransitionSize(float value)
        {
            _currentConfig.TransitionSize = value;
            UpdateOcclusionProperties();
        }

        public void UpdateMaximumOcclusionDistance(float value)
        {
            _currentConfig.MaximumOcclusionDistance = value;
            UpdateOcclusionProperties();
        }

        public void UpdateMaximumOcclusionDistanceTransition(float value)
        {
            _currentConfig.MaximumOcclusionDistanceTransition = value;
            UpdateOcclusionProperties();
        }

        private void UpdateOcclusionProperties()
        {
            if (_currentConfig != null && _occlusionMaterial != null)
            {
                _occlusionMaterial.SetFloat("_OcclusionTransparency", _currentConfig.OcclusionTransparency);
                _occlusionMaterial.SetFloat("_OcclusionOffsetMeters", _currentConfig.OcclusionOffset);
                _occlusionMaterial.SetFloat("_TransitionSize", _currentConfig.TransitionSize);
                _occlusionMaterial.SetFloat("_MaximumOcclusionDistance", _currentConfig.MaximumOcclusionDistance);
                _occlusionMaterial.SetFloat("_MaximumOcclusionDistanceTransition", _currentConfig.MaximumOcclusionDistanceTransition);
            }
        }

        private void Update()
        {
            _occlusionMaterial.SetTexture(DepthTexturePropertyName, occlusionManager.environmentDepthTexture);
        }

        private void OnEnable()
        {
            if (_commandBuffer != null)
            {
                _camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _commandBuffer);
            }
        }

        private void OnDisable()
        {
            if (_commandBuffer != null)
            {
                _camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _commandBuffer);
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_commandBuffer != null)
            {
                // Pass #1 combines virtual and real cameras based on the occlusion map.
                Graphics.Blit(source, destination, _occlusionMaterial, /*pass=*/ 1);
            }
        }
    }
}
