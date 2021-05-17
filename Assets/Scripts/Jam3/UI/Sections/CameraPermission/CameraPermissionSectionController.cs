//-----------------------------------------------------------------------
// <copyright file="CameraPermissionSectionController.cs" company="Jam3 Inc">
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

        /// <summary>
        /// Returns this section type
        /// </summary>
        public override SectionType GetSectionType()
        {
            return SectionType.CameraPermission;
        }

        /// <summary>
        /// Starts this section
        /// </summary>
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

        /// <summary>
        /// Completes this section and goes to the next
        /// </summary>
        public override void CompleteSection()
        {
            gameObject.SetActive(false);
            base.CompleteSection();
        }

        /// <summary>
        /// Overrides the Skip method to do some essential things
        /// </summary>
        public override void SkipToSection(SectionType sectionToSkipTo)
        {
            EnableCamera();
            base.SkipToSection(sectionToSkipTo);
        }

        private bool Skip()
        {
            return Input.GetKey(KeyCode.Z);
        }

        /// <summary>
        /// Can return the app pause cause, in this case the camera permission request
        /// </summary>
        public override bool CausedAppPause()
        {
            return isRequestingPermissions;
        }

        private void OnEnable()
        {
            cameraPermissionSectionUi.TurnOnCamera += EnableCamera;
        }

        private void OnDisable()
        {
            cameraPermissionSectionUi.TurnOnCamera -= EnableCamera;
        }

        /// <summary>
        /// This section flow
        /// </summary>
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

        /// <summary>
        /// Enables the camera and shows the camera permission request
        /// </summary>
        private void EnableCamera()
        {
            PermissionsManager.Instance.ShouldHaveCameraPermission = true;
            PermissionsManager.Instance.EnableARSession();
        }

    }
}
