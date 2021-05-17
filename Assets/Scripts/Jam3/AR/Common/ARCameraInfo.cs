//-----------------------------------------------------------------------
// <copyright file="ARCameraInfo.cs" company="Jam3 Inc">
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
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using Google.XR.ARCoreExtensions;

namespace Jam3.AR
{
    /// <summary>
    /// A r camera info.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class ARCameraInfo : MonoBehaviour
    {
        public ARCameraManager CameraManager;
        public AROcclusionManager OcclusionManager;
        public Texture2D EditorDepthFallback;

        private ScreenOrientation? m_CurrentScreenOrientation;


#if UNITY_ANDROID
        private Matrix4x4 k_AndroidFlipYMatrix = Matrix4x4.identity;
#endif // UNITY_ANDROID

        const string k_DisplayRotationPerFrameName = "_DisplayRotationPerFrame";
        static readonly int k_DisplayRotationPerFrameId = Shader.PropertyToID(k_DisplayRotationPerFrameName);

        // Runtime varilables
        private static Texture2D s_ConfidenceTexture;
        private static Texture2D s_DepthTexture;
        private static Texture2D s_RawDepthTexture;
        private static Matrix4x4 s_LocalToWorldTransform = Matrix4x4.identity;
        private static Matrix4x4 s_DisplayRotationMatrix = Matrix4x4.identity;
        private static Matrix4x4 s_ScreenRotation = Matrix4x4.Rotate(Quaternion.identity);
        private static Vector2 s_FocalLength = Vector2.zero;
        private static Vector2 s_PrincipalPoint = Vector2.zero;
        private static Vector2Int s_ImageDimensions = Vector2Int.zero;
        private static XRCameraIntrinsics s_CameraIntrinsics = default;
        private static bool s_HasIntrinsics = false;

        /// <summary>
        /// Awake.
        /// </summary>
        void Awake()
        {
#if UNITY_ANDROID
            k_AndroidFlipYMatrix[1, 1] = -1.0f;
            k_AndroidFlipYMatrix[2, 1] = 1.0f;
#endif // UNITY_ANDROID
        }

        /// <summary>
        /// OnEnable.
        /// </summary>
        void OnEnable()
        {
            if (CameraManager == null || OcclusionManager == null) return;

            //OcclusionManager.frameReceived += OnOcclusionFrameEventReceived;
            CameraManager.frameReceived += OnCameraFrameEventReceived;
            s_DisplayRotationMatrix = Matrix4x4.identity;
            m_CurrentScreenOrientation = null;
        }

        /// <summary>
        /// OnDisable.
        /// </summary>
        void OnDisable()
        {
            if (CameraManager == null) return;

            //OcclusionManager.frameReceived -= OnOcclusionFrameEventReceived;
            CameraManager.frameReceived -= OnCameraFrameEventReceived;
            s_DisplayRotationMatrix = Matrix4x4.identity;
            m_CurrentScreenOrientation = null;
        }

        /// <summary>
        /// Gets the global reference to the depth texture.
        /// </summary>
        public static bool Initialized
        {
            get {
                return s_HasIntrinsics;
            }
        }

        /// <summary>
        /// Gets the camera to world matrix for transforming depth vertices.
        /// </summary>
        public static Matrix4x4 LocalToWorldMatrix
        {
            get {
                return s_LocalToWorldTransform;
            }
        }

        /// <summary>
        /// Gets the global reference to the depth texture.
        /// </summary>
        public static Texture2D RawDepthTexture
        {
            get {
                return s_RawDepthTexture;
            }
        }

        /// <summary>
        /// Gets the global reference to the depth texture.
        /// </summary>
        public static Texture2D DepthTexture
        {
            get {
                return s_DepthTexture;
            }
        }

        /// <summary>
        /// Gets the global reference to the depth confidence texture.
        /// </summary>
        public static Texture2D ConfidenceTexture
        {
            get {
                return s_ConfidenceTexture;
            }
        }

        /// <summary>
        /// Gets the global reference to the display rotation matrix.
        /// </summary>
        public static Matrix4x4 DisplayRotationMatrix
        {
            get {
                return s_DisplayRotationMatrix;
            }
        }

        /// <summary>
        /// Gets the focal length in pixels.
        /// Focal length is conventionally represented in pixels. For a detailed
        /// explanation, please see
        /// http://ksimek.github.io/2013/08/13/intrinsic.
        /// Pixels-to-meters conversion can use SENSOR_INFO_PHYSICAL_SIZE and
        /// SENSOR_INFO_PIXEL_ARRAY_SIZE in the Android CameraCharacteristics API.
        /// </summary>
        public static Vector2 FocalLength
        {
            get {
                return s_FocalLength;
            }
        }

        /// <summary>
        /// Gets the principal point in pixels.
        /// </summary>
        public static Vector2 PrincipalPoint
        {
            get {
                return s_PrincipalPoint;
            }
        }

        /// <summary>
        /// Gets the intrinsic's width and height in pixels.
        /// </summary>
        public static Vector2Int ImageDimensions
        {
            get {
                return s_ImageDimensions;
            }
        }

        /// <summary>
        /// Gets the width of the depth map.
        /// </summary>
        public static int DepthWidth
        {
            get {
                return s_ImageDimensions.x;
            }
        }

        /// <summary>
        /// Gets the height of the depth map.
        /// </summary>
        public static int DepthHeight
        {
            get {
                return s_ImageDimensions.y;
            }
        }

        /// <summary>
        /// Gets the screen rotation.
        /// </summary>
        /// <value>
        /// The screen rotation.
        /// </value>
        public static Matrix4x4 ScreenRotation
        {
            get {
                return s_ScreenRotation;
            }
        }

        /// <summary>
        /// Returns a copy of the latest depth texture. A new texture is generated unless a texture
        /// is provided.
        /// The provided texture will be resized, if the size is different.
        /// </summary>
        /// <param name="snapshot">Texture to hold the snapshot depth data.</param>
        /// <returns>Returns a texture snapshot with the latest depth data.</returns>
        public static Texture2D GetDepthTextureSnapshot(Texture2D snapshot = null)
        {
            if (s_DepthTexture != null)
            {
                if (snapshot == null)
                {
                    snapshot = new Texture2D(s_DepthTexture.width, s_DepthTexture.height, s_DepthTexture.format, false);
                    snapshot.Apply();
                }
                else if (snapshot.width != s_DepthTexture.width || snapshot.height != s_DepthTexture.height)
                {
                    snapshot.Resize(s_DepthTexture.width, s_DepthTexture.height);
                    snapshot.Apply();
                }

                Graphics.CopyTexture(s_DepthTexture, snapshot);
            }

            return snapshot;
        }

        /// <summary>
        /// Updates the data from the occlusion frame.
        /// </summary>
        void OnOcclusionFrameEventReceived(AROcclusionFrameEventArgs occlusionFrameEventArgs)
        {
            if (occlusionFrameEventArgs.textures != null && occlusionFrameEventArgs.textures.Count > 0)
            {
                for (int i = 0; i < occlusionFrameEventArgs.textures.Count; i++)
                {
                    if (i == 0)
                    {
                        s_DepthTexture = occlusionFrameEventArgs.textures[i];
                    }
                }
            }
        }

        /// <summary>
        /// Updates the data from the camera frame.
        /// </summary>
        void OnCameraFrameEventReceived(ARCameraFrameEventArgs cameraFrameEventArgs)
        {
            if (m_CurrentScreenOrientation == Screen.orientation)
            {
                return;
            }

            m_CurrentScreenOrientation = Screen.orientation;
            Matrix4x4 cameraMatrix = cameraFrameEventArgs.displayMatrix ?? Matrix4x4.identity;

            Vector2 affineBasisX = new Vector2(1.0f, 0.0f);
            Vector2 affineBasisY = new Vector2(0.0f, 1.0f);
            Vector2 affineTranslation = new Vector2(0.0f, 0.0f);

#if UNITY_ANDROID
            affineBasisX = new Vector2(cameraMatrix[0, 0], cameraMatrix[0, 1]);
            affineBasisY = new Vector2(cameraMatrix[1, 0], cameraMatrix[1, 1]);
            affineTranslation = new Vector2(cameraMatrix[0, 2], cameraMatrix[1, 2]);
#endif // UNITY_ANDROID

            affineBasisX = affineBasisX.normalized;
            affineBasisY = affineBasisY.normalized;
            s_DisplayRotationMatrix = Matrix4x4.identity;
            s_DisplayRotationMatrix[0, 0] = affineBasisX.x;
            s_DisplayRotationMatrix[0, 1] = affineBasisY.x;
            s_DisplayRotationMatrix[1, 0] = affineBasisX.y;
            s_DisplayRotationMatrix[1, 1] = affineBasisY.y;
            s_DisplayRotationMatrix[2, 0] = Mathf.Round(affineTranslation.x);
            s_DisplayRotationMatrix[2, 1] = Mathf.Round(affineTranslation.y);

#if UNITY_ANDROID
            s_DisplayRotationMatrix = k_AndroidFlipYMatrix * s_DisplayRotationMatrix;
#endif // UNITY_ANDROID

            Shader.SetGlobalMatrix(k_DisplayRotationPerFrameId, s_DisplayRotationMatrix);
        }

        /// <summary>
        /// Updates screen orientation.
        /// </summary>
        private static void UpdateScreenOrientation()
        {
            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                    s_ScreenRotation = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90));
                    break;
                case ScreenOrientation.LandscapeLeft:
                    s_ScreenRotation = Matrix4x4.Rotate(Quaternion.identity);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    s_ScreenRotation = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
                    break;
                case ScreenOrientation.LandscapeRight:
                    s_ScreenRotation = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
                    break;
            }
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            UpdateScreenOrientation();
            s_LocalToWorldTransform = Camera.main.transform.localToWorldMatrix * ARCameraInfo.ScreenRotation;

            if (!s_HasIntrinsics)
            {
                if (Application.isEditor)
                {
                    s_HasIntrinsics = true;
                    s_FocalLength = new Vector2(506.4582f, 505.9296f);
                    s_PrincipalPoint = new Vector2(319.5142f, 242.3049f);
                    s_ImageDimensions = new Vector2Int(640, 480);
                }
                else
                {
                    s_HasIntrinsics = CameraManager.TryGetIntrinsics(out s_CameraIntrinsics);
                    if (s_HasIntrinsics)
                    {
                        s_FocalLength = new Vector2(125.7424f, 125.7979f); //s_CameraIntrinsics.focalLength;
                        s_PrincipalPoint = new Vector2(79.99667f, 45.71975f); //s_CameraIntrinsics.principalPoint;
                        s_ImageDimensions = new Vector2Int(160, 90); //s_CameraIntrinsics.resolution;
                    }
                }
            }

            if (Application.isEditor)
            {
                s_DepthTexture = EditorDepthFallback;
                s_RawDepthTexture = EditorDepthFallback;
                s_ConfidenceTexture = EditorDepthFallback;
            }
            else
            {
                if (OcclusionManager == null) return;

                s_DepthTexture = OcclusionManager.environmentDepthTexture;
                s_RawDepthTexture = OcclusionManager.GetEnvironmentRawDepthTexture();
                s_ConfidenceTexture = OcclusionManager.GetEnvironmentRawDepthConfidenceTexture();
            }
        }
    }
}
