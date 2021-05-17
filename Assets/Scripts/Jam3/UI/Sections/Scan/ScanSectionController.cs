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

        public override SectionType GetSectionType()
        {
            return SectionType.Scan;
        }

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

        public void SpawnMesh()
        {
            if (ARMeshGenerationController != null)
                ARMeshGenerationController.SpawnMesh();
        }

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

        private void DisableScan()
        {
            scanEnabled = false;

            if (ARMeshGenerationController != null)
                ARMeshGenerationController.gameObject.SetActive(false);
        }

        private void ScanComplete()
        {
            if (ScanSectionUI != null)
                ScanSectionUI.EndScan();
        }

        private void MeshScanCompleted()
        {
            if (ScanSectionUI != null)
                ScanSectionUI.AddCompleteScanningStep();
        }
    }
}
