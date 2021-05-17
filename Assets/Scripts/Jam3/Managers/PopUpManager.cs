//-----------------------------------------------------------------------
// <copyright file="PopUpManager.cs" company="Jam3 Inc">
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
using UnityEngine.UI;
using Jam3.Util;

using TMPro;

namespace Jam3
{
    /// <summary>
    /// Pop up element.
    /// </summary>
    [Serializable]
    public class PopUpElement
    {
        public const int Id = 0;
        public GameObject PopUpPrefab = null;
        public Sprite Icon = null;
        public string Message = null;
    }

    /// <summary>
    /// Pop up manager.
    /// </summary>
    /// <seealso cref="Singleton<PopUpManager>" />
    public class PopUpManager : Singleton<PopUpManager>
    {
        public PopUpElement[] PopUps = null;

        public GameObject ClearConfirmation = null;
        public GameObject ResetConfirmation = null;

        // Runtime variables
        private bool isShowing;

        /// <summary>
        /// Gets the is showing.
        /// </summary>
        /// <value>
        /// The is showing.
        /// </value>
        public bool IsShowing { get => isShowing; }

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            CloseAll();
        }

        /// <summary>
        /// Show.
        /// </summary>
        /// <param name="id">The id.</param>
        public void Show(int id)
        {
            if (id > PopUps.Length)
                return;

            foreach (Transform item in PopUps[id].PopUpPrefab.transform)
            {
                if (item.GetComponent<Image>() != null)
                    item.GetComponent<Image>().sprite = PopUps[id].Icon;

                if (item.GetComponent<TMP_Text>() != null)
                {
                    if (PopUps[id].Message.Contains("{0}"))
                        item.GetComponent<TMP_Text>().SetText(PopUps[id].Message, PlacementManager.Instance.MinObjectsInScene);
                    else
                        item.GetComponent<TMP_Text>().text = PopUps[id].Message;
                }

            }

            PopUps[id].PopUpPrefab.SetActive(true);

            isShowing = true;
        }

        /// <summary>
        /// Shows clear confirmation.
        /// </summary>
        public void ShowClearConfirmation()
        {
            CloseAll();

            ClearConfirmation.SetActive(true);
        }

        /// <summary>
        /// Shows reset confirmation.
        /// </summary>
        public void ShowResetConfirmation()
        {
            CloseAll();

            ResetConfirmation.SetActive(true);
        }

        /// <summary>
        /// Closes all.
        /// </summary>
        /// <param name="onlyMessages">The only messages.</param>
        public void CloseAll(bool onlyMessages = false)
        {
            for (int i = 0; i < PopUps.Length; i++)
            {
                PopUps[i].PopUpPrefab.SetActive(false);
            }

            isShowing = false;

            if (!onlyMessages)
            {
                ResetConfirmation.SetActive(false);
                ClearConfirmation.SetActive(false);
            }
        }
    }
}
