//-----------------------------------------------------------------------
// <copyright file="PermissionsManager.cs" company="Jam3 Inc">
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

using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Jam3.Util;

namespace Jam3.OS
{
    /// <summary>
    /// Manages OS permissions.
    /// </summary>
    public class PermissionsManager : Singleton<PermissionsManager>
    {
        // which permissions are supported
        // for now we only support camera, but soon we could support micrphone, etc.
        /// <summary>
        /// Permission type.
        /// </summary>
        public enum PermissionType
        {
            Camera
        }

        // events
        /// <summary>
        /// Event raised when a permission has been updated.
        /// </summary>
        public Action<PermissionType, bool> PermissionsUpdated;

        // Runtime variables
        private bool m_ShouldHaveCameraPermission = false;
        private bool CachedCameraPermissions;

        [SerializeField] private GameObject aRSessionGameObject;
        [SerializeField] private ARCameraManager aRCameraManager;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool cameraPermissionsInEditor = true;
#endif

        /// <summary>
        /// Gets or sets the should have camera permission.
        /// </summary>
        /// <value>
        /// The should have camera permission.
        /// </value>
        public bool ShouldHaveCameraPermission
        {
            set {
                m_ShouldHaveCameraPermission = value;
                SetCachedCameraPermissions(CachedCameraPermissions, true);
            }
            get {
                return m_ShouldHaveCameraPermission;
            }
        }

        /// <summary>
        /// Sets cached camera permissions.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="forceUpdate">The force update.</param>
        private void SetCachedCameraPermissions(bool state, bool forceUpdate = false)
        {
            // check if there was a change and emit an event
            if (CachedCameraPermissions != state || forceUpdate)
            {
#if UNITY_EDITOR
                CachedCameraPermissions = cameraPermissionsInEditor;
#else
                CachedCameraPermissions = state;
#endif
                PermissionsUpdated?.Invoke(PermissionType.Camera, CachedCameraPermissions);
            }
        }

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            if (aRCameraManager == null) aRCameraManager = FindObjectOfType<ARCameraManager>();
            if (aRCameraManager == null)
            {
                Debug.LogError("ARCameraManager not found!");
            }
            else
            {
                // force send an event for the initial value
                SetCachedCameraPermissions(aRCameraManager.permissionGranted, true);
            }
        }

        /// <summary>
        /// Returns whether a type of permission has been granted.
        /// </summary>
        /// <param name="type">The permission type.</param>
        /// <returns></returns>
        public bool HasPermissions(PermissionType type)
        {
            switch (type)
            {
                case PermissionType.Camera:
                    SetCachedCameraPermissions(aRCameraManager.permissionGranted);
                    return CachedCameraPermissions;

                default:
                    Debug.Log("Permission type not supported!");
                    return false;
            }
        }

        /// <summary>
        /// Ons application focus.
        /// </summary>
        /// <param name="focusStatus">The focus status.</param>
        private void OnApplicationFocus(bool focusStatus)
        {
            // every time we return to the app, check for permissions changes
            if (focusStatus)
            {
                // force update on return to focus
                if (aRCameraManager != null) SetCachedCameraPermissions(aRCameraManager.permissionGranted, true);
            }
        }

        /// <summary>
        /// Enables a r session.
        /// </summary>
        public void EnableARSession()
        {
            aRSessionGameObject.SetActive(true);

            //hasAskedForPermission = true;
        }

    }
}
