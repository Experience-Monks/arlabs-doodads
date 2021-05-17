//-----------------------------------------------------------------------
// <copyright file="ARFPluginManager.cs" company="Google LLC">
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
    using System.Diagnostics.CodeAnalysis;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;

    /// <summary>
    /// The manager class to handle plugin conflicts in AR Foundation and ARCore Extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Internal")]
    public class ARFPluginManager : IPreprocessBuildWithReport
    {
        private const string _arfArCoreClientAarPath =
            "Packages/com.unity.xr.arcore/Runtime/Android/arcore_client.aar";

        private const string _arfArPrestoAarPath =
            "Packages/com.unity.xr.arcore/Runtime/Android/ARPresto.aar";

        [SuppressMessage("UnityRules.UnityStyleRules",
         "US1109:PublicPropertiesMustBeUpperCamelCase", Justification = "Overriden property.")]
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            // Disable the arcore_client.aar in ARCore XR Plugin package since
            // we will use the arcore_client{_flavor}.aar shipped in ARCore Extensions package.
            var clientPluginImporter =
                AssetImporter.GetAtPath(_arfArCoreClientAarPath) as PluginImporter;
            clientPluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, false);

            // Disable the ARPresto.aar in ARCore XR Plugin package, since
            // we will use the ARPresto{_flavor}.aar shipped in ARCore Extensions package.
            var prestoPluginImporter =
                AssetImporter.GetAtPath(_arfArPrestoAarPath) as PluginImporter;
            prestoPluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, false);
        }
    }
}
