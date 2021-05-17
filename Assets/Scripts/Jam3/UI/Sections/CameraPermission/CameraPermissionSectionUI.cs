//-----------------------------------------------------------------------
// <copyright file="CameraPermissionSectionUI.cs" company="Jam3 Inc">
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

        /// <summary>
        /// Shows the camera permission screen
        /// </summary>
        public void ShowPermissionScreen(Action onComplete)
        {
            Bottom.SetActive(false);
            OnComplete = onComplete;

            StartCoroutine(WaitAndShowBottom());
        }

        /// <summary>
        /// Asks the camera permission by pressing the button
        /// </summary>
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

        /// <summary>
        /// Wait and shows the UI permission button
        /// </summary>
        private IEnumerator WaitAndShowBottom()
        {
            yield return new WaitForSeconds(2f);

            Bottom.SetActive(true);
        }

        /// <summary>
        /// Redirects user to app config 
        /// </summary>
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
