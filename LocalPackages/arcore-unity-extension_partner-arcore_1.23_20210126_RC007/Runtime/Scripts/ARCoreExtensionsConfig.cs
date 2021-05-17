//-----------------------------------------------------------------------
// <copyright file="ARCoreExtensionsConfig.cs" company="Google LLC">
//
// Copyright 2019 Google LLC
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
    using Google.XR.ARCoreExtensions.Internal;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Holds settings that are used to configure the ARCore Extensions.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ARCoreExtensionsConfig",
        menuName = "ARCore Extensions/ARCore Extensions Config",
        order = 1)]
    public class ARCoreExtensionsConfig : ScriptableObject
    {
        [Header("Cloud Anchors")]

        /// <summary>
        /// Gets or sets the <c><see cref="CloudAnchorMode"/></c> to use.
        /// </summary>
        [Tooltip("Chooses which Cloud Anchors mode will be used in ARCore Extensions session.")]
        [FormerlySerializedAs("EnableCloudAnchors")]
        public CloudAnchorMode CloudAnchorMode = CloudAnchorMode.Disabled;

        [Header("Segmentation")]

        /// <summary>
        /// Choose which segmentation mode to use in the session.
        /// </summary>
        [Tooltip("Choose which segmentation mode to use in the session.")]
        [DynamicHelp("GetSegmentationModeHelpInfo")]
        public SegmentationMode SegmentationMode = SegmentationMode.Disabled;

        [Header("Depth")]

        /// <summary>
        /// Overrides the DepthMode set by the AROcclusionManager. To choose an ARCore supported
        /// <c>DepthMode</c> not available through AR Foundation, select from the
        /// <c><see cref="DepthModeOverride"/></c>.
        /// </summary>
        [Tooltip("Choose an ARCore-specific DepthMode.")]
        [DynamicHelp("GetDepthModeOverrideHelpInfo")]
        public DepthModeOverride DepthModeOverride = DepthModeOverride.DoNotOverride;

        [Header("Low Feature Vertical Plane Detection")]

        /// <summary>
        /// Replaces the current <c><see cref="ARPlaneManager"/></c> plane finding mode with
        /// ARCore's HorizontalAndVerticalLowFeatureGrowth plane finding mode.
        /// </summary>
        [Tooltip("Choose whether to enable low feature vertical plane detection in the session.")]
        public bool UseHorizontalAndVerticalLowFeatureGrowth = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Cloud Anchors are enabled.
        /// </summary>
        /// @deprecated Please use CloudAnchorMode instead.
        [System.Obsolete(
            "This field has been replaced by ARCoreExtensionsConfig.CloudAnchorMode. See " +
            "https://github.com/google-ar/arcore-unity-extensions/releases/tag/v1.20.0")]
        public bool EnableCloudAnchors
        {
            get
            {
                return CloudAnchorMode != CloudAnchorMode.Disabled;
            }

            set
            {
                CloudAnchorMode = value ? CloudAnchorMode.Enabled : CloudAnchorMode.Disabled;
            }
        }

        /// <summary>
        /// Reflection function used by 'DynamicHelp' for property 'DepthModeOverride'.
        /// </summary>
        /// <returns>The help attribute of depth mode information.</returns>
        public HelpAttribute GetDepthModeOverrideHelpInfo()
        {
            return new HelpAttribute(
                "To select an ARCore <c>DepthMode</c> not available through AR Foundation," +
                  " use this option to override the value set by the <c>AROcclusionManager</c>.",
                HelpAttribute.HelpMessageType.Info);
        }

        /// <summary>
        /// Reflection function used by 'DynamicHelp' for property 'SegmentationMode'.
        /// </summary>
        /// <returns>The help attribute of segmentation mode information.</returns>
        public HelpAttribute GetSegmentationModeHelpInfo()
        {
            if (SegmentationMode == SegmentationMode.People)
            {
                return new HelpAttribute(
                    "People segmentation mode is only supported on the rear-facing (world) camera" +
                    ", and is limited to 30 FPS target camera framerate.",
                    HelpAttribute.HelpMessageType.Info);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ValueType check if two ARCoreExtensionsConfig objects are equal.
        /// </summary>
        /// <param name="other">The other ARCoreExtensionsConfig.</param>
        /// <returns>True if the two ARCoreExtensionsConfig objects are value-type equal,
        /// otherwise false.</returns>
        public override bool Equals(object other)
        {
            ARCoreExtensionsConfig otherConfig = other as ARCoreExtensionsConfig;
            if (otherConfig == null ||
                SegmentationMode != otherConfig.SegmentationMode ||
                UseHorizontalAndVerticalLowFeatureGrowth !=
                    otherConfig.UseHorizontalAndVerticalLowFeatureGrowth ||
                CloudAnchorMode != otherConfig.CloudAnchorMode)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return a hash code for this object.
        /// </summary>
        /// <returns>A hash code value.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// ValueType copy from another ARCoreExtensionsConfig object into this one.
        /// </summary>
        /// <param name="otherConfig">The ARCoreExtensionsConfig to copy from.</param>
        public void CopyFrom(ARCoreExtensionsConfig otherConfig)
        {
            CloudAnchorMode = otherConfig.CloudAnchorMode;
            SegmentationMode = otherConfig.SegmentationMode;
            DepthModeOverride = otherConfig.DepthModeOverride;
            UseHorizontalAndVerticalLowFeatureGrowth =
                otherConfig.UseHorizontalAndVerticalLowFeatureGrowth;
        }
    }
}
