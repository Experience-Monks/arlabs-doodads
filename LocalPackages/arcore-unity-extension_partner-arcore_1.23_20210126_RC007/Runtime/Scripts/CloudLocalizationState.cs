//-----------------------------------------------------------------------
// <copyright file="CloudLocalizationState.cs" company="Google LLC">
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
    /// Represents the localization state of an anchor or the camera against the
    /// local map and/or global Earth map.
    /// </summary>
    public enum CloudLocalizationState
    {
        /// <summary>
        /// The anchor is not yet localized against any cloud maps. Non-cloud
        /// anchors will always be in this state.
        /// </summary>
        None = 0,

        /// <summary>
        /// The anchor is successfully localized against the global Earth map only.
        /// In this state, the anchor's pose is imprecise, as the exact pose
        /// relative to the much denser local map is not known.
        /// </summary>
        Earth = 1,

        /// <summary>
        /// The anchor is successfully localized against a local map only. During hosting,
        /// anchors created with <c><see cref="CloudAnchorMode"/></c>.<c>Enabled</c> mode will be
        /// associated with this state.
        /// </summary>
        LocalMap = 2,

        /// <summary>
        /// The anchor is successfully localized against both a local map and the
        /// global Earth map. During hosting, anchors created with
        /// <c><see cref="CloudAnchorMode"/></c>.<c>EnabledWithEarthLocalization</c> mode
        /// will be associated with this state.
        /// </summary>
        LocalMapAndEarth = 3,
    }
}
