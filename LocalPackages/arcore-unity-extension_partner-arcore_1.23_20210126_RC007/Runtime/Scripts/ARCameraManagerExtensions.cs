//-----------------------------------------------------------------------
// <copyright file="ARCameraManagerExtensions.cs" company="Google LLC">
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
    /// Extension methods for the AR Foundation ARCameraManager.
    /// </summary>
    public static class ARCameraManagerExtensions
    {
        /// <summary>
        /// Gets the state of localization against the global Earth map.
        /// </summary>
        /// <param name="cameraManager">The ARCameraManager instance.</param>
        /// <returns>The Earth localization state against the global Earth map.</returns>
        public static EarthLocalizationState GetEarthLocalizationState(
            this ARCameraManager cameraManager)
        {
            EarthLocalizationState state = EarthLocalizationState.NotLocalized;
            var cameraParams = new XRCameraParams
            {
                zNear = cameraManager.GetComponent<Camera>().nearClipPlane,
                zFar = cameraManager.GetComponent<Camera>().farClipPlane,
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                screenOrientation = Screen.orientation
            };

            if (!cameraManager.subsystem.TryGetLatestFrame(cameraParams, out XRCameraFrame frame))
            {
                Debug.LogWarning("Unable to determine the current EarthLocalizationState, " +
                    "the current XRCameraFrame is not available, try again later.");
                return state;
            }

            if (frame.timestampNs == 0 || frame.FrameHandle() == IntPtr.Zero)
            {
                Debug.LogWarning("Unable to determine the current EarthLocalizationState, " +
                    "the current frame is not ready, try again later.");
                return state;
            }

            return FrameApi.GetEarthLocalizationState(
                ARCoreExtensions._instance.currentARCoreSessionHandle, frame.FrameHandle());
        }
    }
}
