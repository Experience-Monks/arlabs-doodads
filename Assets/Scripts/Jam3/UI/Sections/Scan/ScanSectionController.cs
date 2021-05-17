//-----------------------------------------------------------------------
// <copyright file="ScanSectionController.cs" company="Jam3 Inc">
//
// Copyright 2021 Jam3 Inc.
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

using UnityEngine;
using Jam3.AR;

namespace Jam3
{
    public class ScanSectionController : SectionController
    {
        public bool SkipScan = false;

        [Header("UI")]
        public ScanSectionUI ScanSectionUI = default;

        [Header("AR Scan Scripts")]
        public ARCameraEffect ARCameraEffect = null;
        public ARDepthMeshPreview ARDepthMesh = null;
        public ARMeshGenerationController ARMeshGenerationController = null;

        private bool scanEnabled = false;

        /// <summary>
        /// Returns the section type
        /// </summary>
        public override SectionType GetSectionType()
        {
            return SectionType.Scan;
        }

        /// <summary>
        /// Overrides the StartSection method and adds some functionalities
        /// </summary>
        public override void StartSection()
        {
            base.StartSection();
            Reset();

            if (SkipScan)
            {
                CompleteSection();
            }
            else
            {
                scanEnabled = false;
                gameObject.SetActive(true);

                if (ARDepthMesh != null)
                    ARDepthMesh.gameObject.SetActive(true);
                else if (ARCameraEffect != null)
                    ARCameraEffect.enabled = true;

                if (ScanSectionUI != null)
                    ScanSectionUI.StartFlow(EnableScan, CompleteSection);

                PlacementManager.Instance.ShowTarget(false);
            }
        }

        /// <summary>
        /// Completes the section
        /// </summary>
        public override void CompleteSection()
        {
            if (ARDepthMesh != null)
                ARDepthMesh.gameObject.SetActive(false);

            if (ARCameraEffect != null)
                ARCameraEffect.enabled = false;

            gameObject.SetActive(false);
            DisableScan();

            base.CompleteSection();
        }

        /// <summary>
        /// Spawns a depth mesh
        /// </summary>
        public void SpawnMesh()
        {
            if (ARMeshGenerationController != null)
                ARMeshGenerationController.SpawnMesh();
        }

        /// <summary>
        /// Resets the section
        /// </summary>
        public void Reset()
        {
            if (ARMeshGenerationController != null)
                ARMeshGenerationController.Reset();

            if (ScanSectionUI != null)
                ScanSectionUI.Reset();
        }

        void Update()
        {
            if (scanEnabled && ScanSectionUI != null)
            {
                if (ScanSectionUI.ShowDebugUI)
                {
                    ScanSectionUI.UpdateDepthImage(ARCameraInfo.RawDepthTexture);
                    ScanSectionUI.UpdateConfidenceImage(ARCameraInfo.ConfidenceTexture);

                    if (ARMeshGenerationController != null)
                        ScanSectionUI.SetDebugText(ARMeshGenerationController.DepthConfidenceAverage.ToString());
                }

                //Gets scan data to show on the UI
                if (ARMeshGenerationController != null)
                {
                    ScanSectionUI.SetReady(ARMeshGenerationController.IsReady);
                    ScanSectionUI.DepthConfidenceStatus(ARMeshGenerationController.DepthConfidenceAverage, ARMeshGenerationController.DepthThreshold);
                }
            }

            if (ARCameraEffect != null && ARCameraEffect.enabled)
            {
                ARCameraEffect.EffectReady = ARMeshGenerationController.IsReady;
                ARCameraEffect.IsOverMesh = ARMeshGenerationController.IsOverMesh;
                ARCameraEffect.DepthConfidenceAverage = ARMeshGenerationController.DepthConfidenceAverage;
            }
        }

        /// <summary>
        /// Enables the scan process
        /// </summary>
        private void EnableScan()
        {
            if (ARMeshGenerationController != null)
            {
                if (!scanEnabled)
                {
                    ARMeshGenerationController.OnScanComplete = ScanComplete;
                    ARMeshGenerationController.gameObject.SetActive(true);

                    ARMeshGenerationController.OnMeshScanCompleted = MeshScanCompleted;

                    scanEnabled = true;
                }

                ARMeshGenerationController.StartScanning();
            }
        }

        /// <summary>
        /// Disables the scan process
        /// </summary>
        private void DisableScan()
        {
            scanEnabled = false;

            if (ARMeshGenerationController != null)
                ARMeshGenerationController.gameObject.SetActive(false);
        }

        /// <summary>
        /// Completes the scan process
        /// </summary>
        private void ScanComplete()
        {
            if (ScanSectionUI != null)
                ScanSectionUI.EndScan();
        }

        /// <summary>
        /// Completes the depth mesh scan process
        /// </summary>
        private void MeshScanCompleted()
        {
            if (ScanSectionUI != null)
                ScanSectionUI.AddCompleteScanningStep();
        }
    }
}
