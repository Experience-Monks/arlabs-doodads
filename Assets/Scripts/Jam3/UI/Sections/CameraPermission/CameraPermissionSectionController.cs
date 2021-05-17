using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Jam3.OS;

namespace Jam3
{
    public class CameraPermissionSectionController : SectionController
    {
        [SerializeField] private CameraPermissionSectionUI cameraPermissionSectionUi = default;

        private bool isRequestingPermissions = false;

        public override SectionType GetSectionType()
        {
            return SectionType.CameraPermission;
        }

        public override void StartSection()
        {
            base.StartSection();
            gameObject.SetActive(true);

            isRequestingPermissions = false;

            // skip this section if we already have camera permissions
            if (PermissionsManager.Instance.HasPermissions(PermissionsManager.PermissionType.Camera))
            {
                EnableCamera();
                CompleteSection();
            }
            else
            {
                StartCoroutine(Flow());
            }
        }

        public override void CompleteSection()
        {
            gameObject.SetActive(false);
            base.CompleteSection();
        }

        // override the Skip method to do some essential things
        public override void SkipToSection(SectionType sectionToSkipTo)
        {
            EnableCamera();
            base.SkipToSection(sectionToSkipTo);
        }

        private bool Skip()
        {
            return Input.GetKey(KeyCode.Z);
        }

        public override bool CausedAppPause()
        {
            return isRequestingPermissions;
        }

        // -----------------

        private void OnEnable()
        {
            cameraPermissionSectionUi.TurnOnCamera += EnableCamera;
        }

        private void OnDisable()
        {
            cameraPermissionSectionUi.TurnOnCamera -= EnableCamera;
        }

        private IEnumerator Flow()
        {
            cameraPermissionSectionUi.ShowPermissionScreen(CompleteSection);
            yield return new WaitForSeconds(1f);

            isRequestingPermissions = true;

            // enable camera/AR to request permissions
            EnableCamera();
            yield return new WaitUntil(() => PermissionsManager.Instance.ShouldHaveCameraPermission && PermissionsManager.Instance.HasPermissions(PermissionsManager.PermissionType.Camera));

            isRequestingPermissions = false;

            CompleteSection();
        }

        private void EnableCamera()
        {
            PermissionsManager.Instance.ShouldHaveCameraPermission = true;
            PermissionsManager.Instance.EnableARSession();
        }

    }
}
