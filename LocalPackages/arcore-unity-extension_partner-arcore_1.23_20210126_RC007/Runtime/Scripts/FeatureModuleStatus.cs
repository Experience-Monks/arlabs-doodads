//-----------------------------------------------------------------------
// <copyright file="FeatureModuleStatus.cs" company="Google LLC">
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
    /// Describes the current state of a FeatureModule's availability on the
    /// device.
    /// </summary>
    public enum FeatureModuleStatus
    {
        /// <summary>
        /// An internal error occurred while determining feature module
        /// availability. Check logcat for more details.
        /// </summary>
        UnknownError,

        /// <summary>
        /// The feature module is supported, installed, and available to use.
        /// </summary>
        SupportedInstalled,

        /// <summary>
        /// The feature module was requested with
        /// <c><see cref="Session.RequestModuleInstallImmediate"/></c> and is still downloading or
        /// installing.
        /// </summary>
        SupportedPendingImmediateInstall,

        /// <summary>
        /// The feature module is supported but not installed.
        /// </summary>
        SupportedNotInstalled,

        /// <summary>
        /// The feature module was requested with
        /// <c><see cref="Session.RequestModuleInstallDeferred"/></c> and will be downloaded and
        /// installed at some point in the future.
        /// </summary>
        SupportedPendingDeferredInstall,
    }
}
