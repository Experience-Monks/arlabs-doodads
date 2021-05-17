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

        public void UpdateDepthImage(Texture2D texture)
        {
            if (DepthImage != null)
                DepthImage.texture = texture;
        }

        public void UpdateConfidenceImage(Texture2D texture)
        {
            if (ConfidenceImage != null)
                ConfidenceImage.texture = texture;
        }

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

        public void EndScan()
        {
            ShowDebugPanel(false);
            HideScanTip();

            if (!UseAutomaticFlowEnd)
                ShowCompleteButton(true);
            else
                Complete();
        }

        public void Complete()
        {
            ShowDebugPanel(false);
            HideScanTip();
            ShowCompleteButton(false);
            SetScanningStatusComplete();

            StartCoroutine(WaitAndComplete());
        }

        private IEnumerator WaitAndComplete()
        {
            yield return new WaitForSeconds(2f);

            if (OnComplete != null)
                OnComplete();
        }

        public void ShowDebugPanel(bool show)
        {
            if (DebugUI != null)
                DebugUI.SetActive(show);
        }

        public void SetDebugText(string text)
        {
            if (ValueUI != null)
                ValueUI.text = text;
        }

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

        public void Reset()
        {
            ShowStartTip();
            HideScanTip();
            ResetScanningSteps();

            ShowCompleteButton(false);
            ShowStartButton(!UseAutomaticFlowStart);
            ShowDebugPanel(false);
        }

        public void ShowStartButton(bool show)
        {
            if (StartButton != null)
                StartButton.gameObject.SetActive(show);
        }

        public void ShowCompleteButton(bool show)
        {
            if (CompleteButton != null)
                CompleteButton.gameObject.SetActive(show);

            if (RestartButton != null)
                RestartButton.gameObject.SetActive(show);

            SetScanningStatusComplete();
        }

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

        private void AnimateScanTip()
        {
            arrowsTransform.DOKill();
            arrowsTransform.DOAnchorPosX(30.0f, 1.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

            phoneTransform.DOKill();
            phoneTransform.DOAnchorPosX(20.0f, 1.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(0.25f);
        }

        private void HideScanTip()
        {
            scanTip.SetActive(false);
        }

        private void ShowStartTip()
        {
            startTip.SetActive(true);

            startTipTarget.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            startTipTarget.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 1.7f);

            startTipTitleText.color = new Color(1, 1, 1, 0);
            startTipTitleText.DOColor(new Color(1, 1, 1, 1), 1.7f);

            AnimateStartTip();
        }

        private void AnimateStartTip()
        {
            startTipTarget.DOKill();
            startTipTarget.DOAnchorPosX(30.0f, 1.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        private void HideStartTip()
        {
            startTip.SetActive(false);
        }

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

        public void SetScanningStatusComplete()
        {
            ScanningText.DOKill();
            ScanningText.color = new Color(1, 1, 1, 1);
        }

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
