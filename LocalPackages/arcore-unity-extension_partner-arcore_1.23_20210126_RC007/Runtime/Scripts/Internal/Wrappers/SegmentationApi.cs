//-----------------------------------------------------------------------
// <copyright file="SegmentationApi.cs" company="Google LLC">
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

    internal class SegmentationApi
    {
        public static void Release(IntPtr segmentationHandle)
        {
            ExternApi.ArSegmentation_release_private(segmentationHandle);
        }

        public static bool AcquirePersonMask(
            IntPtr sessionHandle, IntPtr segmentationHandle, ref IntPtr imageHandle)
        {
            ApiArStatus status = ExternApi.ArSegmentation_acquirePersonMask_private(
                sessionHandle, segmentationHandle, ref imageHandle);
            if (status != ApiArStatus.Success)
            {
                Debug.LogErrorFormat(
                    "ARCore Extensions: failed to acquire person mask image with status {0}",
                    status);
                return false;
            }

            return true;
        }

        private struct ExternApi
        {
#pragma warning disable 626
            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArSegmentation_release_private(IntPtr segmentation);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern ApiArStatus ArSegmentation_acquirePersonMask_private(
                IntPtr session, IntPtr segmentation, ref IntPtr out_image);
#pragma warning restore 626
        }
    }
}
