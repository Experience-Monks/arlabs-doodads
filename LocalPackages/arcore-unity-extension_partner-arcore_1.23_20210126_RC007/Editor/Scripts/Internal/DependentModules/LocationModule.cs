//-----------------------------------------------------------------------
// <copyright file="LocationModule.cs" company="Google LLC">
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

namespace Google.XR.ARCoreExtensions.Editor.Internal
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;

    /// <summary>
    /// The implemented class of location module.
    /// </summary>
    public class LocationModule : DependentModuleBase
    {
        /// <summary>
        /// Checking whether it needs to be included in the customized AndroidManifest.
        /// The default values for new fields in ARCoreProjectSettings should cause the
        /// associated module to return false.
        /// </summary>
        /// <param name="settings">ARCore Extensions Project Settings.</param>
        /// <returns>The boolean shows whether the module is enabled.</returns>
        public override bool IsEnabled(ARCoreExtensionsProjectSettings settings)
        {
            return settings.EarthCloudAnchorEnabled;
        }

        /// <summary>
        /// Return the XML snippet needs to be included if location module is enabled.
        /// The string output would be added as a child node of in the ‘manifest’ node
        /// of the customized AndroidManifest.xml. The android namespace would be provided
        /// and feature developers could use it directly.
        /// </summary>
        /// <param name="settings">ARCore Extensions Project Settings.</param>
        /// <returns>The XML string snippet to add as a child of node 'manifest'.</returns>
        public override string GetAndroidManifestSnippet(ARCoreExtensionsProjectSettings settings)
        {
            return @"
                <uses-permission android:name=""android.permission.ACCESS_FINE_LOCATION""/>
                <uses-feature
                    android:name=""android.hardware.location.gps"" android:required=""true""/>
                <uses-feature
                    android:name=""android.hardware.location"" android:required=""true""/>";
        }

        /// <summary>
        /// Checking whether location module is compatible with given Project Settings.
        /// If it returns false, the preprocessbuild will throw a general Build Failure Error
        /// with detailed error messages.
        /// </summary>
        /// <param name="settings">ARCore Extensions Project Settings.</param>
        /// <param name="sessionConfig">ARCore Extensions Config.</param>
        /// <returns>The boolean shows whether the ARCoreExtensionsProjectSettings is compatible
        /// with the ARCoreExtensionsConfig.</returns>
        public override bool IsCompatibleWithSessionConfig(ARCoreExtensionsProjectSettings settings,
            ARCoreExtensionsConfig sessionConfig)
        {
            CloudAnchorMode cloudAnchorMode = sessionConfig.CloudAnchorMode;
            bool isRequired =
                cloudAnchorMode == CloudAnchorMode.EnabledWithEarthLocalization;
            if (isRequired && !IsEnabled(settings))
            {
                Debug.LogErrorFormat(
                    "LocationModule is required by CloudAnchorMode {0}. Navigate to " +
                    "'Project Settings > XR > ARCore Extensions' and select " +
                    "Earth Cloud Anchor Enabled.", cloudAnchorMode);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Return the Proguard to include if this module is enabled. The string output will be
        /// added into "proguard-user.txt" directly.
        /// </summary>
        /// <param name="settings">ARCore Extensions Project Settings.</param>
        /// <returns>The proguard rule string snippet.</returns>
        public override string GetProguardSnippet(ARCoreExtensionsProjectSettings settings)
        {
            return @"-keep class com.google.android.gms.common.** { *; }
                    -keep class com.google.android.gms.location.** { *; }
                    -keep class com.google.android.gms.tasks.** { *; }";
        }
    }
}
