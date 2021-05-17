//-----------------------------------------------------------------------
// <copyright file="FrameApi.cs" company="Google LLC">
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

namespace Google.XR.ARCoreExtensions.Internal
{
    using System;
    using UnityEngine;

#if UNITY_IOS && ARCORE_EXTENSIONS_IOS_SUPPORT
    using AndroidImport = Google.XR.ARCoreExtensions.Internal.DllImportNoop;
    using IOSImport = System.Runtime.InteropServices.DllImportAttribute;
#else
    using AndroidImport = System.Runtime.InteropServices.DllImportAttribute;
    using IOSImport = Google.XR.ARCoreExtensions.Internal.DllImportNoop;
#endif

    internal class FrameApi
    {
        public static IntPtr AcquireCamera(IntPtr sessionHandle, IntPtr frameHandle)
        {
            IntPtr cameraHandle = IntPtr.Zero;
            ExternApi.ArFrame_acquireCamera(sessionHandle, frameHandle, ref cameraHandle);
            return cameraHandle;
        }

        public static void ReleaseCamera(IntPtr cameraHandle)
        {
            ExternApi.ArCamera_release(cameraHandle);
        }

        public static EarthLocalizationState GetEarthLocalizationState(
            IntPtr sessionHandle, IntPtr frameHandle)
        {
            int state = (int)EarthLocalizationState.NotLocalized;
#if UNITY_IOS && ARCORE_EXTENSIONS_IOS_SUPPORT
            ExternApi.ArFrame_getEarthLocalizationState(
                sessionHandle, frameHandle, ref state);
#else
            var cameraHandle = AcquireCamera(sessionHandle, frameHandle);
            ExternApi.ArCamera_getEarthLocalizationState_private(
                sessionHandle, cameraHandle, ref state);
            ReleaseCamera(cameraHandle);
#endif // UNITY_IOS && ARCORE_EXTENSIONS_IOS_SUPPORT
            return (EarthLocalizationState)state;
        }

        public static IntPtr AcquireRawDepthImage(IntPtr sessionHandle, IntPtr frameHandle)
        {
            IntPtr depthImageHandle = IntPtr.Zero;

            // Get the current depth image.
            ApiArStatus status = ExternApi.ArFrame_acquireRawDepthImage(
                sessionHandle, frameHandle, ref depthImageHandle);

            if (status != ApiArStatus.Success)
            {
                Debug.LogErrorFormat(
                    "ARCore Extensions: failed to acquire raw depth image with status {0}",
                    status.ToString());
                return IntPtr.Zero;
            }

            return depthImageHandle;
        }

        public static IntPtr AcquireRawDepthConfidenceImage(
            IntPtr sessionHandle, IntPtr frameHandle)
        {
            IntPtr confidenceImageHandle = IntPtr.Zero;

            // Get the current confidence depth image.
            ApiArStatus status = ExternApi.ArFrame_acquireRawDepthConfidenceImage(
                sessionHandle, frameHandle, ref confidenceImageHandle);

            if (status != ApiArStatus.Success)
            {
                Debug.LogErrorFormat(
                    "ARCore Extensions: failed to acquire raw depth confidence image with" +
                    " status {0}",
                    status.ToString());
                return IntPtr.Zero;
            }

            return confidenceImageHandle;
        }

        public static IntPtr AcquirePersonMaskImage(IntPtr sessionHandle, IntPtr frameHandle)
        {
            IntPtr segmentationHandle = IntPtr.Zero;
            ApiArStatus status = ExternApi.ArFrame_acquireSegmentation_private(
                sessionHandle, frameHandle, ref segmentationHandle);
            if (status != ApiArStatus.Success)
            {
                Debug.LogErrorFormat(
                    "ARCore Extensions: failed to acquire person mask segmentation with status {0}",
                    status);
                return IntPtr.Zero;
            }

            IntPtr maskImageHandle = IntPtr.Zero;
            if (!SegmentationApi.AcquirePersonMask(
                sessionHandle, segmentationHandle, ref maskImageHandle))
            {
                SegmentationApi.Release(segmentationHandle);
                return IntPtr.Zero;
            }

            SegmentationApi.Release(segmentationHandle);
            return maskImageHandle;
        }

        public static Vector2 TransformCoordinates2d(IntPtr sessionHandle, IntPtr frameHandle,
            ApiCoordinates2dType inputType, ApiCoordinates2dType outputType, ref Vector2 uvIn)
        {
            Vector2 uvOut = new Vector2(uvIn.x, uvIn.y);
            ExternApi.ArFrame_transformCoordinates2d(
                sessionHandle, frameHandle, inputType, 1, ref uvIn, outputType, ref uvOut);
            return uvOut;
        }

        private struct ExternApi
        {
#pragma warning disable 626
            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArFrame_acquireCamera(
                IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr cameraHandle);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArCamera_release(IntPtr cameraHandle);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArCamera_getEarthLocalizationState_private(
                IntPtr sessionHandle, IntPtr cameraHandle, ref int outEarthLocalizationState);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern ApiArStatus ArFrame_acquireRawDepthImage(
                IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr imageHandle);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern ApiArStatus ArFrame_acquireRawDepthConfidenceImage(
                IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr imageHandle);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern ApiArStatus ArFrame_acquireSegmentation_private(IntPtr session,
                IntPtr frame, ref IntPtr out_segmentation);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArFrame_transformCoordinates2d(IntPtr session, IntPtr frame,
                ApiCoordinates2dType inputType, int numVertices, ref Vector2 uvsIn,
                ApiCoordinates2dType outputType, ref Vector2 uvsOut);
#pragma warning restore 626

            [IOSImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArFrame_getEarthLocalizationState(
                IntPtr sessionHandle, IntPtr frameHandle, ref int outEarthLocalizationState);
        }
    }
}
