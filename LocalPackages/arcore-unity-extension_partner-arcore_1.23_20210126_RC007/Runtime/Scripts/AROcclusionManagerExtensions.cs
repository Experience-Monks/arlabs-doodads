//-----------------------------------------------------------------------
// <copyright file="AROcclusionManagerExtensions.cs" company="Google LLC">
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

namespace Google.XR.ARCoreExtensions
{
    using System;
    using Google.XR.ARCoreExtensions.Internal;
    using UnityEngine;

    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;

    /// <summary>
    /// Extensions to AR Foundation's AROcclusionManager class.
    /// </summary>
    public static class AROcclusionManagerExtensions
    {
        /// <summary>
        /// Get the latest person mask texture that corresponds to the current frame in
        /// <a href="https://docs.unity3d.com/ScriptReference/TextureFormat.Alpha8.html">
        /// TextureFormat.Alpha8</a> format where each pixel represents the confidence of
        /// the segmentation result: 255 represents high confidence that the pixel is part of
        /// a person, 0 represents high confidence that the pixel is not part of a person.
        /// </summary>
        /// <param name="occlusionManager">The AROcclusionManager instance.</param>
        /// <returns>If available, the texture containing the person mask.
        /// Otherwise, returns null and logs the failure reason,
        /// e.g. Segmentation Mode is Disabled, or current camera configuration
        /// is incompatible with <c><see cref="SegmentationMode"/></c>.<c>People</c>.</returns>
        public static Texture2D GetPersonMaskTexture(this AROcclusionManager occlusionManager)
        {
            if (ARCoreExtensions._instance.ARCoreExtensionsConfig.SegmentationMode !=
                SegmentationMode.People)
            {
                Debug.LogWarning(
                    "Person mask texture is not available when SegmentationMode is not People.");
                return null;
            }

            if (!TryGetLastFrameFromExtensions(out XRCameraFrame frame))
            {
                return null;
            }

            if (CachedData.TryGetCachedData(
                    CachedData.PersonMaskTexture, out Texture2D texture) &&
                CachedData.TryGetCachedData(
                    CachedData.PersonMaskTextureTimestamp, out long timestamp) &&
                texture != null && timestamp == frame.timestampNs)
            {
                return texture;
            }

            IntPtr sessionHandle = ARCoreExtensions._instance.currentARCoreSessionHandle;
            IntPtr imageHandle = FrameApi.AcquirePersonMaskImage(
                sessionHandle, frame.FrameHandle());
            if (imageHandle != IntPtr.Zero)
            {
                ImageApi.UpdateTexture(
                    sessionHandle, imageHandle, TextureFormat.Alpha8, ref texture);
                ImageApi.Release(imageHandle);
                CachedData.SetCachedData(CachedData.PersonMaskTexture, texture);
                CachedData.SetCachedData(
                    CachedData.PersonMaskTextureTimestamp, frame.timestampNs);
            }

            return texture;
        }

        /// <summary>
        /// Attempt to get the latest person mask data that corresponds to the current frame
        /// for CPU access. Each pixel represents the confidence of the segmentation result:
        /// 255 represents high confidence that the pixel is part of a person, 0 represents
        /// high confidence that the pixel is not part of a person. If the data is available,
        /// it is copied into the output image buffer.
        /// </summary>
        /// <param name="occlusionManager">The AROcclusionManager instance.</param>
        /// <param name="outputBuffer">
        /// The output image buffer to be filled with raw image data.</param>
        /// <returns>If available, returns a Vector2Int which represents the size of the image.
        /// Otherwise, returns <c>Vector2Int.zero</c> and logs the failure reason, e.g.
        /// Segmentation Mode is Disabled, or current camera configuration s incompatible with
        /// <c><see cref="SegmentationMode"/></c>.<c>People</c>.</returns>
        public static Vector2Int TryAcquirePersonMaskRawData(
            this AROcclusionManager occlusionManager, ref byte[] outputBuffer)
        {
            if (ARCoreExtensions._instance.ARCoreExtensionsConfig.SegmentationMode !=
                SegmentationMode.People)
            {
                Debug.LogWarning(
                    "Person mask data is not available when SegmentationMode is not People.");
                return Vector2Int.zero;
            }

            if (!TryGetLastFrameFromExtensions(out XRCameraFrame frame))
            {
                return Vector2Int.zero;
            }

            Vector2Int imageSize = Vector2Int.zero;
            IntPtr sessionHandle = ARCoreExtensions._instance.currentARCoreSessionHandle;
            IntPtr imageHandle = FrameApi.AcquirePersonMaskImage(
                sessionHandle, frame.FrameHandle());
            if (imageHandle != IntPtr.Zero)
            {
                imageSize = ImageApi.UpdateRawData(
                    sessionHandle, imageHandle, ref outputBuffer);
                ImageApi.Release(imageHandle);
            }

            return imageSize;
        }

        /// <summary>
        /// A texture representing the raw depth for the current frame. See the
        /// <a href="https://developers.google.com/ar/eap/raw-depth">developer guide</a>
        /// for more information about raw depth.
        /// </summary>
        /// <param name="occlusionManager">The AROcclusionManager instance.</param>
        /// <returns>The environment raw depth texture, if any. Otherwise, null.</returns>
        public static Texture2D GetEnvironmentRawDepthTexture(
            this AROcclusionManager occlusionManager)
        {
            if (occlusionManager.currentEnvironmentDepthMode ==
                EnvironmentDepthMode.Disabled)
            {
                Debug.LogWarning(
                    "Environment raw depth texture is not available" +
                    " when EnvironmentDepthMode is Disabled.");
                return null;
            }

            if (!TryGetLastFrameFromExtensions(out XRCameraFrame frame))
            {
                return null;
            }

            if (CachedData.TryGetCachedData(
                    CachedData.RawDepthTexture, out Texture2D texture) &&
                CachedData.TryGetCachedData(
                    CachedData.RawDepthTimestamp, out long timestamp) &&
                texture != null && timestamp == frame.timestampNs)
            {
                return texture;
            }

            IntPtr imageHandle = FrameApi.AcquireRawDepthImage(
                ARCoreExtensions._instance.currentARCoreSessionHandle, frame.FrameHandle());

            if (imageHandle == IntPtr.Zero)
            {
                return null;
            }

            ImageApi.UpdateTexture(
                ARCoreExtensions._instance.currentARCoreSessionHandle, imageHandle,
                TextureFormat.RGB565, ref texture);
            ImageApi.Release(imageHandle);
            CachedData.SetCachedData(CachedData.RawDepthTexture, texture);
            CachedData.SetCachedData(CachedData.RawDepthTimestamp, frame.timestampNs);

            return texture;
        }

        /// <summary>
        /// A texture representing the confidence for each pixel in the raw depth for
        /// the current frame. See the <a href="https://developers.google.com/ar/eap/raw-depth">
        /// developer guide</a> for more information about raw depth.
        /// </summary>
        /// <param name="occlusionManager">The AROcclusionManager instance.</param>
        /// <returns>
        /// The environment raw depth confidence texture, if any. Otherwise, null.
        /// </returns>
        public static Texture2D GetEnvironmentRawDepthConfidenceTexture(
            this AROcclusionManager occlusionManager)
        {
            if (occlusionManager.currentEnvironmentDepthMode ==
                EnvironmentDepthMode.Disabled)
            {
                Debug.LogWarning(
                    "Environment raw depth confidence texture is not available" +
                    " when EnvironmentDepthMode is Disabled.");
                return null;
            }

            if (!TryGetLastFrameFromExtensions(out XRCameraFrame frame))
            {
                return null;
            }

            if (CachedData.TryGetCachedData(
                    CachedData.RawDepthConfidenceTexture, out Texture2D texture) &&
                CachedData.TryGetCachedData(
                    CachedData.RawDepthConfidenceTimestamp, out long timestamp) &&
                texture != null && timestamp == frame.timestampNs)
            {
                return texture;
            }

            IntPtr imageHandle = FrameApi.AcquireRawDepthConfidenceImage(
                ARCoreExtensions._instance.currentARCoreSessionHandle,
                frame.FrameHandle());

            if (imageHandle == IntPtr.Zero)
            {
                return null;
            }

            ImageApi.UpdateTexture(
                ARCoreExtensions._instance.currentARCoreSessionHandle, imageHandle,
                TextureFormat.Alpha8, ref texture);
            ImageApi.Release(imageHandle);
            CachedData.SetCachedData(CachedData.RawDepthConfidenceTexture, texture);
            CachedData.SetCachedData(CachedData.RawDepthConfidenceTimestamp, frame.timestampNs);

            return texture;
        }

        private static bool TryGetLastFrameFromExtensions(out XRCameraFrame frame)
        {
            ARCoreExtensions extensions = ARCoreExtensions._instance;
            ARCameraManager cameraManager = extensions.CameraManager;
            var cameraParams = new XRCameraParams
            {
                zNear = cameraManager.GetComponent<Camera>().nearClipPlane,
                zFar = cameraManager.GetComponent<Camera>().farClipPlane,
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                screenOrientation = Screen.orientation
            };

            if (!cameraManager.subsystem.TryGetLatestFrame(
                    cameraParams, out frame))
            {
                Debug.LogWarning(
                    "The current XRCameraFrame is not available, try again later.");
                return false;
            }

            if (frame.timestampNs == 0 || frame.FrameHandle() == IntPtr.Zero)
            {
                Debug.LogWarning(
                    "The current XRCameraFrame is not ready, try again later.");
                return false;
            }

            return true;
        }
    }
}
