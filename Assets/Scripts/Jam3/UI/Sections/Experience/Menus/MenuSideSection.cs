//-----------------------------------------------------------------------
// <copyright file="MenuSideSection.cs" company="Jam3 Inc">
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace Jam3
{
    public class MenuSideSection : MenuSectionUI
    {
        [Header("Buttons")]
        public Button TranslateButton = null;
        public Button RotateButton = null;
        public Button ScaleButton = null;
        public Button CompassButton = null;

        [Header("Alpha Config")]
        [Range(0f, 1f)]
        public float AlphaEnabled = 1.0f;
        [Range(0f, 1f)]
        public float AlphaDisabled = 0.3f;

        private ARObject selectedObject = null;

        /// <summary>
        /// Overrides the Open method adding extra functionalities
        /// </summary>
        public override void Open()
        {
            base.Open();

            SetButtonsVisibilityUI();
            SetButtonsStateUI(true);
        }

        /// <summary>
        /// Sets side buttons initial visibility
        /// </summary>
        private void SetButtonsVisibilityUI()
        {
            ARObject selectedObject = SelectionManager.Instance.SelectedObject;
            if (selectedObject != null)
            {
                TranslateButton.gameObject.SetActive(false);
                RotateButton.gameObject.SetActive(false);
                ScaleButton.gameObject.SetActive(false);
                CompassButton.gameObject.SetActive(false);

                if (selectedObject.Transformation.TranslateEnabledAxis != Axis.None)
                    TranslateButton.gameObject.SetActive(true);

                if (selectedObject.Transformation.RotateEnabledAxis != Axis.None)
                {
                    if (selectedObject.IsPhysic)
                        CompassButton.gameObject.SetActive(true);
                    else
                        RotateButton.gameObject.SetActive(true);
                }

                if (selectedObject.Transformation.ScaleEnabledAxis != Axis.None)
                    ScaleButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Sets the translate mode
        /// </summary>
        public void SetHandlerModeTranslate()
        {
            AudioManager.Instance.PlayAudioClip("Caroulsel");

            ARObject selectedObject = SelectionManager.Instance.SelectedObject;
            if (selectedObject != null)
                selectedObject.Transformation.Handler.SetMode(HandlerObject.HandlerMode.Translate);

            TransformManager.Instance.SetMode(HandlerObject.HandlerMode.Translate);

            SetButtonsStateUI();
        }

        /// <summary>
        /// Sets the rotation mode
        /// </summary>
        public void SetHandlerModeRotate()
        {
            AudioManager.Instance.PlayAudioClip("Caroulsel");

            ARObject selectedObject = SelectionManager.Instance.SelectedObject;
            if (selectedObject != null)
                selectedObject.Transformation.Handler.SetMode(HandlerObject.HandlerMode.Rotate);

            TransformManager.Instance.SetMode(HandlerObject.HandlerMode.Rotate);

            SetButtonsStateUI();
        }

        /// <summary>
        /// Sets the scale mode
        /// </summary>
        public void SetHandlerModeScale()
        {
            AudioManager.Instance.PlayAudioClip("Caroulsel");

            ARObject selectedObject = SelectionManager.Instance.SelectedObject;
            if (selectedObject != null)
                selectedObject.Transformation.Handler.SetMode(HandlerObject.HandlerMode.Scale);

            TransformManager.Instance.SetMode(HandlerObject.HandlerMode.Scale);

            SetButtonsStateUI();
        }

        /// <summary>
        /// Sets the side menu buttons state
        /// </summary>
        private void SetButtonsStateUI(bool clear = false)
        {
            ARObject selectedObject = SelectionManager.Instance.SelectedObject;
            if (selectedObject != null)
            {
                if (clear)
                {
                    TranslateButton.GetComponent<Image>().color = (new Color(1, 1, 1, AlphaDisabled));
                    RotateButton.GetComponent<Image>().color = (new Color(1, 1, 1, AlphaDisabled));
                    ScaleButton.GetComponent<Image>().color = (new Color(1, 1, 1, AlphaDisabled));
                    CompassButton.GetComponent<Image>().color = (new Color(1, 1, 1, AlphaDisabled));
                }

                HandlerObject.HandlerMode handlerMode = selectedObject.Transformation.Handler.Mode;

                switch (handlerMode)
                {
                    case HandlerObject.HandlerMode.Translate:
                        TranslateButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaEnabled), 0.2f);
                        RotateButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        ScaleButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        CompassButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        break;

                    case HandlerObject.HandlerMode.Rotate:
                        TranslateButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        RotateButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaEnabled), 0.2f);
                        ScaleButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        CompassButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaEnabled), 0.2f);
                        break;

                    case HandlerObject.HandlerMode.Scale:
                        TranslateButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        RotateButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        ScaleButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaEnabled), 0.2f);
                        CompassButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        break;

                    default:
                        TranslateButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaEnabled), 0.2f);
                        RotateButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        ScaleButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        CompassButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, AlphaDisabled), 0.2f);
                        break;
                }
            }
        }
    }
}
