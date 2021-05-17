//-----------------------------------------------------------------------
// <copyright file="MenuTopSection.cs" company="Jam3 Inc">
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
using UnityEngine.XR.ARFoundation;

namespace Jam3
{
    public class MenuTopSection : MenuSectionUI
    {
        [SerializeField]
        private ARSession arSession = null;

        public GameObject clearButton = null;
        public GameObject resetButton = null;

        private bool clearButtonLastState;

        private void Awake() { }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            RegisterCallbacks();

            clearButton.SetActive(false);
            resetButton.SetActive(true);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterCallbacks();
        }

        /// <summary>
        /// Called by the delegate when selecting an object
        /// </summary>mmary>
        private void HandleSelectionActive(bool isSelecting, ARObject selectedObject)
        {
            if (PlacementManager.Instance.InSceneObjectsCount < 1)
            {
                clearButton.SetActive(false);
            }
            else
            {
                clearButton.SetActive(!isSelecting);
                resetButton.SetActive(true);
            }

        }

        /// <summary>
        /// Called by the delegate when it's in play mode
        /// </summary>
        private void HandlePlay(bool isPlaying)
        {
            if (PlacementManager.Instance.InSceneObjectsCount < 1)
            {
                clearButton.SetActive(false);
            }
            else
            {
                clearButton.SetActive(!isPlaying);
            }

            if (isPlaying)
                PopUpManager.Instance.CloseAll();
        }

        /// <summary>
        /// Called by the delegate when it's game over
        /// </summary>
        private void HandleGameOver(bool isGameOver)
        {
            if (PlacementManager.Instance.InSceneObjectsCount < 1)
            {
                clearButton.SetActive(false);
            }
            else
            {
                clearButton.SetActive(!isGameOver);
            }
        }

        /// <summary>
        /// Prompts the clear scene confirmation menu
        /// </summary>
        public void PromptClearConfirmation()
        {
            AudioManager.Instance.PlayAudioClip("ObjectSelection");

            clearButtonLastState = clearButton.activeSelf;

            PopUpManager.Instance.ShowClearConfirmation();
            clearButton.SetActive(false);
            resetButton.SetActive(false);
        }

        /// <summary>
        /// Prompts the reset confirmation menu
        /// </summary>
        public void PromptResetConfirmation()
        {
            AudioManager.Instance.PlayAudioClip("ObjectSelection");

            clearButtonLastState = clearButton.activeSelf;

            PopUpManager.Instance.ShowResetConfirmation();
            clearButton.SetActive(false);
            resetButton.SetActive(false);
        }

        /// <summary>
        /// Button press for clear scene confirmation
        /// </summary>
        public void OnClearGameClick()
        {
            AudioManager.Instance.PlayAudioClip("Reset");

            GameManager.Instance.RestartGame();

            if (MenuControllerUI != null)
                MenuControllerUI.SetMenuList();

            clearButton.SetActive(false);
            resetButton.SetActive(true);
        }

        /// <summary>
        /// Button press for restart cancelation
        /// </summary>
        public void CancelRestart()
        {
            AudioManager.Instance.PlayAudioClip("HelpClose");

            PopUpManager.Instance.CloseAll();

            if (PlacementManager.Instance.InSceneObjectsCount > 0)
            {
                clearButton.SetActive(clearButtonLastState);
                resetButton.SetActive(true);
            }

        }

        /// <summary>
        /// Button press for reset confirmation
        /// </summary>
        public void OnRestartGameClick()
        {
            AudioManager.Instance.PlayAudioClip("Reset");

            GameManager.Instance.RestartGame();
            arSession.Reset();

            UXManager.Instance.GoToSection(SectionType.Scan);
        }

        /// <summary>
        /// Registers the callbacks.
        /// </summary>
        private void RegisterCallbacks()
        {
            SelectionManager.Instance.OnSelection += HandleSelectionActive;
            GameManager.Instance.OnPlay += HandlePlay;
            GameManager.Instance.OnGameOver += HandleGameOver;
        }

        /// <summary>
        /// Unregisters the callbacks.
        /// </summary>
        private void UnregisterCallbacks()
        {
            SelectionManager.Instance.OnSelection -= HandleSelectionActive;
            GameManager.Instance.OnPlay -= HandlePlay;
            GameManager.Instance.OnGameOver -= HandleGameOver;
        }

    }
}
