//-----------------------------------------------------------------------
// <copyright file="ScanSectionUI.cs" company="Jam3 Inc">
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

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using TMPro;

namespace Jam3
{
    public class ScanSectionUI : MonoBehaviour
    {
        public Action OnStart = null;
        public Action OnComplete = null;

        [Header("Config")]
        public bool UseAutomaticFlowStart = false;
        public bool UseAutomaticFlowEnd = false;
        public float EnterScanDelay = 5f;

        [Header("Debug UI")]
        public bool ShowDebugUI = true;
        public GameObject DebugUI = null;
        public Button CreateButton = null;
        public RawImage DepthImage = default;
        public RawImage ConfidenceImage = default;
        public Text ValueUI = null;
        public Image ReadyUI = null;

        [Header("Buttons")]
        public Button StartButton = null;
        public Button CompleteButton = null;
        public Button RestartButton = null;

        [Header("Scan Icon")]
        public GameObject scanTip;
        public RectTransform arrowsTransform;
        public RectTransform phoneTransform;
        public TextMeshProUGUI titleText;

        [Header("Scan Start Icon")]
        public GameObject startTip;
        public RectTransform startTipTarget;
        public TextMeshProUGUI startTipTitleText;

        [Header("Scan Status")]
        public GameObject ScanningStatus = null;
        public TextMeshProUGUI ScanningText = null;

        [Header("Scan Progress")]
        public float StatusSpeed = 1f;
        public GameObject ScanningProgressBar = null;
        public Sprite ScanProgScanning = null;
        public Sprite ScanProgScanned = null;
        public GameObject Step1 = null;
        public GameObject Step2 = null;
        public GameObject Step3 = null;

        private Image step1RealtimeBar = null;
        private Image step1TotalAmountBar = null;

        private Image step2RealtimeBar = null;
        private Image step2TotalAmountBar = null;

        private Image step3RealtimeBar = null;
        private Image step3TotalAmountBar = null;

        private float statusScanTotalAmount = 0;
        private int scanProgCompletedStepsCount = 0;
        private bool scanReady = false;

        private float step1RealtimeAmount = 0.0f;
        private float step1TotalAmount = 0.0f;

        private float step2RealtimeAmount = 0.0f;
        private float step2TotalAmount = 0.0f;

        private float step3RealtimeAmount = 0.0f;
        private float step3TotalAmount = 0.0f;

        // private
        private Image[] arrowsImages;

        /// <summary>
        /// Starts the section flow
        /// </summary>
        public void StartFlow(Action onStart, Action onComplete)
        {
            Reset();

            if (DebugUI != null)
                DebugUI.SetActive(false);

            OnStart = onStart;
            OnComplete = onComplete;
            gameObject.SetActive(true);

            if (UseAutomaticFlowStart)
                StartCoroutine("Flow");

            ShowScanningStatus(false);

            ResetScanningSteps();
        }

        private IEnumerator Flow()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(EnterScanDelay);

            StartScan();
        }

        /// <summary>
        /// Updates the depth image texture
        /// </summary>
        public void UpdateDepthImage(Texture2D texture)
        {
            if (DepthImage != null)
                DepthImage.texture = texture;
        }

        /// <summary>
        /// Updates the confidence image texture
        /// </summary>
        public void UpdateConfidenceImage(Texture2D texture)
        {
            if (ConfidenceImage != null)
                ConfidenceImage.texture = texture;
        }

        /// <summary>
        /// Starts the scan process
        /// </summary>
        public void StartScan()
        {
            ShowStartButton(false);
            HideStartTip();
            ShowScanTip();
            ShowScanningStatus(true);

            ShowDebugPanel(ShowDebugUI);

            if (OnStart != null)
                OnStart();
        }

        /// <summary>
        /// Ends the scan process
        /// </summary>
        public void EndScan()
        {
            ShowDebugPanel(false);
            HideScanTip();

            if (!UseAutomaticFlowEnd)
                ShowCompleteButton(true);
            else
                Complete();
        }

        /// <summary>
        /// Completes the scan process
        /// </summary>
        public void Complete()
        {
            ShowDebugPanel(false);
            HideScanTip();
            ShowCompleteButton(false);
            SetScanningStatusComplete();

            StartCoroutine(WaitAndComplete());
        }

        /// <summary>
        /// Wait a little bit to show some info and completes the section
        /// </summary>
        private IEnumerator WaitAndComplete()
        {
            yield return new WaitForSeconds(2f);

            if (OnComplete != null)
                OnComplete();
        }

        /// <summary>
        /// Shows the debug panel
        /// </summary>
        public void ShowDebugPanel(bool show)
        {
            if (DebugUI != null)
                DebugUI.SetActive(show);
        }

        /// <summary>
        /// Sets the debug text
        /// </summary>
        public void SetDebugText(string text)
        {
            if (ValueUI != null)
                ValueUI.text = text;
        }

        /// <summary>
        /// Sets as scan ready
        /// </summary>
        public void SetReady(bool isReady)
        {
            scanReady = isReady;
            if (ReadyUI != null)
            {
                if (isReady)
                    ReadyUI.color = Color.green;
                else
                    ReadyUI.color = Color.red;
            }
        }

        /// <summary>
        /// Resets the scan process
        /// </summary>
        public void Reset()
        {
            ShowStartTip();
            HideScanTip();
            ResetScanningSteps();

            ShowCompleteButton(false);
            ShowStartButton(!UseAutomaticFlowStart);
            ShowDebugPanel(false);
        }

        /// <summary>
        /// Shows the start button
        /// </summary>
        public void ShowStartButton(bool show)
        {
            if (StartButton != null)
                StartButton.gameObject.SetActive(show);
        }

        /// <summary>
        /// Shows the complete button
        /// </summary>
        public void ShowCompleteButton(bool show)
        {
            if (CompleteButton != null)
                CompleteButton.gameObject.SetActive(show);

            if (RestartButton != null)
                RestartButton.gameObject.SetActive(show);

            SetScanningStatusComplete();
        }

        /// <summary>
        /// Shows the scan tips
        /// </summary>
        private void ShowScanTip()
        {
            scanTip.SetActive(true);

            arrowsTransform.anchoredPosition = new Vector2(-30.0f, arrowsTransform.anchoredPosition.y);
            phoneTransform.anchoredPosition = new Vector2(-20.0f, phoneTransform.anchoredPosition.y);

            Step1.SetActive(true);
            Step2.SetActive(true);
            Step3.SetActive(true);

            foreach (Transform arrow in arrowsTransform)
            {
                arrow.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                arrow.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 1.7f);
            }

            phoneTransform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            phoneTransform.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 1.7f);

            titleText.color = new Color(1, 1, 1, 0);
            titleText.DOColor(new Color(1, 1, 1, 1), 1.7f);

            AnimateScanTip();
        }

        /// <summary>
        /// Animates the scan tips
        /// </summary>
        private void AnimateScanTip()
        {
            arrowsTransform.DOKill();
            arrowsTransform.DOAnchorPosX(30.0f, 1.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

            phoneTransform.DOKill();
            phoneTransform.DOAnchorPosX(20.0f, 1.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(0.25f);
        }

        //// <summary>
        /// Hides the scan tips
        /// </summary>
        private void HideScanTip()
        {
            scanTip.SetActive(false);
        }

        /// <summary>
        /// Shows the scan tips
        /// </summary>
        private void ShowStartTip()
        {
            startTip.SetActive(true);

            startTipTarget.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            startTipTarget.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 1.7f);

            startTipTitleText.color = new Color(1, 1, 1, 0);
            startTipTitleText.DOColor(new Color(1, 1, 1, 1), 1.7f);

            AnimateStartTip();
        }

        /// <summary>
        /// Animates the start tips
        /// </summary>
        private void AnimateStartTip()
        {
            startTipTarget.DOKill();
            startTipTarget.DOAnchorPosX(30.0f, 1.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        /// <summary>
        /// Hides the start tips
        /// </summary>
        private void HideStartTip()
        {
            startTip.SetActive(false);
        }

        /// <summary>
        /// Shows scanning status
        /// </summary>
        public void ShowScanningStatus(bool canShow)
        {
            if (canShow)
            {
                ScanningText.text = "Keep Scanning...";
                ScanningText.DOColor(new Color(1, 1, 1, 0.4f), 0.75f).SetLoops(-1, LoopType.Yoyo);
                ScanningStatus.SetActive(true);
            }
            else
            {
                ScanningStatus.SetActive(false);
                ScanningText.DOKill();
                ScanningText.color = new Color(1, 1, 1, 1);
            }
        }

        /// <summary>
        /// Sets scanning status to complete
        /// </summary>
        public void SetScanningStatusComplete()
        {
            ScanningText.DOKill();
            ScanningText.color = new Color(1, 1, 1, 1);
        }

        /// <summary>
        /// Resets the scanning steps
        /// </summary>
        public void ResetScanningSteps()
        {
            step1RealtimeAmount = 0.0f;
            step1TotalAmount = 0.0f;

            step2RealtimeAmount = 0.0f;
            step2TotalAmount = 0.0f;

            step3RealtimeAmount = 0.0f;
            step3TotalAmount = 0.0f;

            scanProgCompletedStepsCount = 0;
            statusScanTotalAmount = 0;
            ScanningProgressBar.SetActive(true);

            Step1.SetActive(false);
            Step2.SetActive(false);
            Step3.SetActive(false);

            Step1.GetComponent<Image>().sprite = ScanProgScanning;
            Step2.GetComponent<Image>().sprite = ScanProgScanning;
            Step3.GetComponent<Image>().sprite = ScanProgScanning;

            step1RealtimeBar = Step1.transform.Find("Realtime").GetComponent<Image>();
            step1TotalAmountBar = Step1.transform.Find("Total").GetComponent<Image>();

            step2RealtimeBar = Step2.transform.Find("Realtime").GetComponent<Image>();
            step2TotalAmountBar = Step2.transform.Find("Total").GetComponent<Image>();

            step3RealtimeBar = Step3.transform.Find("Realtime").GetComponent<Image>();
            step3TotalAmountBar = Step3.transform.Find("Total").GetComponent<Image>();

            step1RealtimeBar.fillAmount = 0;
            step1TotalAmountBar.fillAmount = 0;

            step2RealtimeBar.fillAmount = 0;
            step2TotalAmountBar.fillAmount = 0;

            step3RealtimeBar.fillAmount = 0;
            step3TotalAmountBar.fillAmount = 0;
        }

        /// <summary>
        /// Adds a complete scanning step
        /// </summary>
        public void AddCompleteScanningStep()
        {
            int maxSteps = 3;
            if (scanProgCompletedStepsCount < maxSteps)
            {
                scanProgCompletedStepsCount++;
                switch (scanProgCompletedStepsCount)
                {
                    case 1:
                        ScanningText.text = "Keep Scanning...";
                        Step1.GetComponent<Image>().sprite = ScanProgScanned;
                        statusScanTotalAmount = 0;
                        step1RealtimeAmount = 1;
                        step1TotalAmount = 1;
                        break;

                    case 2:
                        ScanningText.text = "Almost There...";
                        Step2.GetComponent<Image>().sprite = ScanProgScanned;
                        statusScanTotalAmount = 0;
                        step2RealtimeAmount = 1;
                        step2TotalAmount = 1;
                        break;

                    case 3:
                        ScanningText.text = "Done!";
                        Step3.GetComponent<Image>().sprite = ScanProgScanned;
                        statusScanTotalAmount = 0;
                        step3RealtimeAmount = 1;
                        step3TotalAmount = 1;
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the depth confidence status numbers to UI
        /// </summary>
        public void DepthConfidenceStatus(float statusNumber, float threshold)
        {
            float statusRealtime = (statusNumber / threshold);
            statusRealtime = scanReady ? statusRealtime : 0.0f;

            if (statusRealtime > statusScanTotalAmount)
                statusScanTotalAmount = statusRealtime;

            int maxSteps = 3;
            if (scanProgCompletedStepsCount < maxSteps)
            {
                switch (scanProgCompletedStepsCount)
                {
                    case 0:
                        step1RealtimeAmount = Mathf.Lerp(step1RealtimeAmount, statusRealtime, Time.deltaTime * StatusSpeed);
                        step1TotalAmount = Mathf.Lerp(step1TotalAmount, statusScanTotalAmount, Time.deltaTime * StatusSpeed);
                        break;

                    case 1:
                        step2RealtimeAmount = Mathf.Lerp(step2RealtimeBar.fillAmount, statusRealtime, Time.deltaTime * StatusSpeed);
                        step2TotalAmount = Mathf.Lerp(step2TotalAmount, statusScanTotalAmount, Time.deltaTime * StatusSpeed);
                        break;

                    case 2:
                        step3RealtimeAmount = Mathf.Lerp(step3RealtimeAmount, statusRealtime, Time.deltaTime * StatusSpeed);
                        step3TotalAmount = Mathf.Lerp(step3TotalAmount, statusScanTotalAmount, Time.deltaTime * StatusSpeed);
                        break;
                }
            }

            step1RealtimeBar.fillAmount = step1RealtimeAmount;
            step1TotalAmountBar.fillAmount = step1TotalAmount;

            step2RealtimeBar.fillAmount = step2RealtimeAmount;
            step2TotalAmountBar.fillAmount = step2TotalAmount;

            step3RealtimeBar.fillAmount = step3RealtimeAmount;
            step3TotalAmountBar.fillAmount = step3TotalAmount;
        }
    }

}
