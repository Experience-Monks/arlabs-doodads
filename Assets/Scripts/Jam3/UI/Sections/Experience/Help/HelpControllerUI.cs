//-----------------------------------------------------------------------
// <copyright file="HelpControllerUI.cs" company="Jam3 Inc">
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
using TMPro;

namespace Jam3
{
    public class HelpControllerUI : MonoBehaviour
    {
        public GameObject ObjectContainer = null;
        public GameObject UIMenu = null;
        public GameObject UIPopUps = null;
        public TMP_Text MinText = null;
        public TMP_Text MaxText = null;

        /// <summary>
        /// Opens the Help screen
        /// </summary>
        public void Open()
        {
            AudioManager.Instance.PlayAudioClip("Help");

            gameObject.SetActive(true);

            if (ObjectContainer == null)
                ObjectContainer = GameObject.Find("/ObjectContaier");

            ObjectContainer.SetActive(false);
            UIMenu.SetActive(false);
            UIPopUps.SetActive(false);

            MinText.text = PlacementManager.Instance.MinObjectsInScene.ToString();
            MaxText.text = PlacementManager.Instance.MaxObjectsInScene.ToString() + ".";
        }

        /// <summary>
        /// Closes the Help screen
        /// </summary>
        public void Close()
        {
            AudioManager.Instance.PlayAudioClip("HelpClose");

            gameObject.SetActive(false);

            ObjectContainer.SetActive(true);
            UIMenu.SetActive(true);
            UIPopUps.SetActive(true);
        }
    }
}
