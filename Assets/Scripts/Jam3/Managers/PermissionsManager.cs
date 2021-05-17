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
        // events
        /// <summary>
        /// Event raised when a permission has been updated.
        /// </summary>
        public Action<PermissionType, bool> PermissionsUpdated;

        private bool m_ShouldHaveCameraPermission = false;
        //private bool hasAskedForPermission = false;

        public bool ShouldHaveCameraPermission
        {
            set
            {
                m_ShouldHaveCameraPermission = value;
                SetCachedCameraPermissions(CachedCameraPermissions, true);
            }
            get
            {
                return m_ShouldHaveCameraPermission;
            }
        }

        //public bool HasAskedForPermission { get => hasAskedForPermission; }

        [SerializeField] private GameObject aRSessionGameObject;

        [SerializeField] private ARCameraManager aRCameraManager;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool cameraPermissionsInEditor = true;
#endif

        // which permissions are supported
        // for now we only support camera, but soon we could support micrphone, etc.
        public enum PermissionType
        {
            Camera
        }

        private bool CachedCameraPermissions;
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

        void Start()
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

        private void OnApplicationFocus(bool focusStatus)
        {
            // every time we return to the app, check for permissions changes
            if (focusStatus)
            {
                // force update on return to focus
                if (aRCameraManager != null) SetCachedCameraPermissions(aRCameraManager.permissionGranted, true);
            }
        }

        public void EnableARSession()
        {
            aRSessionGameObject.SetActive(true);

            //hasAskedForPermission = true;
        }

    }
}
