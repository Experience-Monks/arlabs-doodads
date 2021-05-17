//-----------------------------------------------------------------------
// <copyright file="EarthLocalizationState.cs" company="Google LLC">
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
    /// Indicates the state of localization against the global Earth map.
    /// </summary>
    public enum EarthLocalizationState
    {
        /// <summary>
        /// The device is not currently localized relative to the global Earth map.
        /// </summary>
        NotLocalized = 0,

        /// <summary>
        /// The device is currently localized relative to the global Earth map.
        /// </summary>
        Localized = 1,

        /// <summary>
        /// Earth localization is not available in the current location. Differs
        /// from NotLocalized in that this will not change unless the user
        /// physically moves to a location where Earth localization is available.
        /// </summary>
        Unavailable = 2,
    }
}
