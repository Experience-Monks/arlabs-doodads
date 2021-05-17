using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Jam3.OS;

namespace Jam3
{
    public class CameraPermissionSectionUI : MonoBehaviour
    {
        public Action OnComplete = null;
        public Action TurnOnCamera = null;

        public GameObject Bottom;

        public void ShowPermissionScreen(Action onComplete)
        {
            Bottom.SetActive(false);
            OnComplete = onComplete;

            StartCoroutine(WaitAndShowBottom());
        }

        // called from button
        public void TurnOnCameraPressed()
        {
            if (Application.isEditor)
            {
                OnComplete();
            }
            else
            {
                if (!PermissionsManager.Instance.HasPermissions(PermissionsManager.PermissionType.Camera))
                {
                    if (!PermissionsManager.Instance.ShouldHaveCameraPermission)
                    {
                        TurnOnCamera?.Invoke();
                    }
                    else
                    {
                        RedirectToAppConfig();
                    }
                }
            }
        }

        private IEnumerator WaitAndShowBottom()
        {
            yield return new WaitForSeconds(2f);

            Bottom.SetActive(true);
        }

        private void RedirectToAppConfig()
        {
            try
            {
#if UNITY_ANDROID
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    string packageName = currentActivityObject.Call<string>("getPackageName");

                    using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                    using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                    using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                    {
                        intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                        intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                        currentActivityObject.Call("startActivity", intentObject);
                    }
                }

                Application.Quit();
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

    }
}
