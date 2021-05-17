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

        public override void Open()
        {
            base.Open();

            SetButtonsVisibilityUI();
            SetButtonsStateUI(true);
        }

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

        public void SetHandlerModeTranslate()
        {
            AudioManager.Instance.PlayAudioClip("Caroulsel");

            ARObject selectedObject = SelectionManager.Instance.SelectedObject;
            if (selectedObject != null)
                selectedObject.Transformation.Handler.SetMode(HandlerObject.HandlerMode.Translate);

            TransformManager.Instance.SetMode(HandlerObject.HandlerMode.Translate);

            SetButtonsStateUI();
        }

        public void SetHandlerModeRotate()
        {
            AudioManager.Instance.PlayAudioClip("Caroulsel");

            ARObject selectedObject = SelectionManager.Instance.SelectedObject;
            if (selectedObject != null)
                selectedObject.Transformation.Handler.SetMode(HandlerObject.HandlerMode.Rotate);

            TransformManager.Instance.SetMode(HandlerObject.HandlerMode.Rotate);

            SetButtonsStateUI();
        }

        public void SetHandlerModeScale()
        {
            AudioManager.Instance.PlayAudioClip("Caroulsel");

            ARObject selectedObject = SelectionManager.Instance.SelectedObject;
            if (selectedObject != null)
                selectedObject.Transformation.Handler.SetMode(HandlerObject.HandlerMode.Scale);

            TransformManager.Instance.SetMode(HandlerObject.HandlerMode.Scale);

            SetButtonsStateUI();
        }

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
