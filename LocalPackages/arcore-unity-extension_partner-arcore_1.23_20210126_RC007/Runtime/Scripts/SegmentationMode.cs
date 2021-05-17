//-----------------------------------------------------------------------
// <copyright file="SegmentationMode.cs" company="Google LLC">
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
    /// <summary>
    /// The segmentation mode determines the type, quality, and speed of the segmentation mask
    /// that is generated.
    /// </summary>
    public enum SegmentationMode
    {
        /// <summary>
        /// Disable segmentation.
        /// </summary>
        Disabled,

        /// <summary>
        /// Segment one or more front-facing human subjects from the background with a neural
        /// network running on the CPU. People, the clothing they're wearing, and sometimes the
        /// objects they are holding will all be included in the segmentation mask.
        /// The segmentation mask image is in
        /// <a href="https://docs.unity3d.com/ScriptReference/TextureFormat.Alpha8.html">
        /// TextureFormat.Alpha8</a> format.
        ///
        /// Runs at a maximum of 30 fps target camera framerate. Quality may
        /// decline as CPU load increases.
        /// </summary>
        People,
    }
}
