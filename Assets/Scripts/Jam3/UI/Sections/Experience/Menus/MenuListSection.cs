//-----------------------------------------------------------------------
// <copyright file="MenuListSection.cs" company="Jam3 Inc">
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
using UnityEngine.EventSystems;
using TMPro;

using DG.Tweening;

namespace Jam3
{
    /// <summary>
    /// Available play button states
    /// </summary>
    public enum PlayButtonState
    {
        None = 0,
        Placement = 1,
        Play = 2,
        Replay = 3
    }

    [Serializable]
    public class MenuItem
    {
        public GameObject Item = null;
        public bool IsOut = false;
    }

    public class MenuListSection : MenuSectionUI
    {
        public float TimeToOpen = 2f;

        public GameObject ListContainer = null;
        public GameObject ElementPrefab = null;
        public GameObject ItemList = null;
        public GameObject VerticalLineSeparatorPrefab = null;
        public GameObject TitleLines = null;
        public GameObject ResultsTable = null;

        public Button PlayButton = null;
        public Button MenuButton = null;

        public Sprite PlayButtonPlacementSprite = null;
        public Sprite PlayButtonPlaySprite = null;
        public Sprite PlayButtonReplaySprite = null;

        public Image RoundProgress = null;

        public TMP_Text TitleText;
        public TMP_Text MaxSpeedText;
        public TMP_Text TotalDistanceText;
        public TMP_Text ObjectsUsedText;

        public string ClosedTitleString = null;
        public string OpenTitleString = null;

        private bool init = false;
        public bool IsBallPlaced { get; set; }

        private PlayButtonState currentPlayButtonState = PlayButtonState.None;
        private MenuItem[] itensList = null;
        private EventTrigger trigger = null;

        void Awake()
        {
            if (!init)
            {
                trigger = PlayButton.gameObject.AddComponent<EventTrigger>();

                if (ListContainer != null && ElementPrefab != null)
                {
                    //add interactive elements to the menu
                    int items = ObjectManager.Instance.InteractiveElements.Length;
                    itensList = new MenuItem[items];

                    for (int i = 0; i < items; i++)
                    {
                        var element = ObjectManager.Instance.InteractiveElements[i];
                        var newObject = Instantiate(ElementPrefab, Vector3.zero, Quaternion.identity);
                        newObject.transform.SetParent(ListContainer.transform, false);

                        itensList[i] = new MenuItem();
                        itensList[i].Item = newObject;
                        itensList[i].IsOut = false;

                        if (i < items - 1)
                        {
                            var verticalLine = Instantiate(VerticalLineSeparatorPrefab, Vector3.zero, Quaternion.identity);
                            verticalLine.transform.SetParent(ListContainer.transform, false);
                        }

                        Image imageComponent = newObject.GetComponent<Image>();
                        if (imageComponent != null && element.Icon != null)
                        {
                            imageComponent.sprite = element.Icon;
                        }

                        int id = i;
                        Button buttonComponent = newObject.GetComponent<Button>();
                        if (buttonComponent != null)
                        {
                            buttonComponent.onClick.RemoveAllListeners();
                            buttonComponent.onClick.AddListener(() => OnItemClick(id));
                        }
                    }
                }

                RoundProgress.gameObject.SetActive(false);
                SetPlayButtonState(PlayButtonState.Placement);
                init = true;
            }
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            GameManager.Instance.OnGameOver += HandleGameOver;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            GameManager.Instance.OnGameOver -= HandleGameOver;
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        private void OnDestroy()
        {
            GameManager.Instance.OnGameOver -= HandleGameOver;
        }

        private void Update()
        {
            if (ObjectManager.Instance.BallObject != null && ObjectManager.Instance.BallObject.SpringController != null)
            {
                if (ObjectManager.Instance.BallObject.SpringController.IsHolding)
                {
                    RoundProgress.fillAmount = ObjectManager.Instance.BallObject.SpringController.SpringTension;
                }
            }
        }

        /// <summary>
        /// Detects the list scrolling
        /// </summary>
        public void OnScroll()
        {
            for (int i = 0; i < itensList.Length; i++)
            {
                if (!itensList[i].IsOut && itensList[i].Item.transform.position.x < 0)
                {
                    AudioManager.Instance.PlayAudioClip("Caroulsel");
                    itensList[i].IsOut = true;
                }

                if (itensList[i].IsOut && itensList[i].Item.transform.position.x > 0)
                {
                    AudioManager.Instance.PlayAudioClip("Caroulsel");
                    itensList[i].IsOut = false;
                }
            }
        }

        /// <summary>
        /// List menu item selection
        /// </summary>
        public void OnItemClick(int id)
        {
            if (PlacementManager.Instance.HasHitPoint && PlacementManager.Instance.InSceneObjectsCount < PlacementManager.Instance.MaxObjectsInScene)
            {
                ObjectManager.Instance.SpawnObject(id);

                if (MenuControllerUI != null)
                    MenuControllerUI.SetMenuPlacement(id);

                PlacementManager.Instance.InSceneObjectsCount++;
                PopUpManager.Instance.CloseAll();

                DOTween.Kill("timer");
            }
            else
            {
                AudioManager.Instance.PlayAudioClip("HelpClose");
                Debug.Log("No Target Position");
            }
        }

        /// <summary>
        /// The ball spawn button press
        /// </summary>
        public void OnSpawnBallClick(int id)
        {
            if (PlacementManager.Instance.HasHitPoint)
            {
                ObjectManager.Instance.SpawnBall(0);

                if (MenuControllerUI != null)
                    MenuControllerUI.SetMenuPlacement(-1);

                SetPlayButtonState(PlayButtonState.Play);

                PopUpManager.Instance.CloseAll();
            }
            else
            {
                AudioManager.Instance.PlayAudioClip("HelpClose");
                Debug.Log("No Target Position");
            }
        }

        /// <summary>
        /// The play button press
        /// </summary>
        private void OnPlayClick()
        {
            AudioManager.Instance.PlayAudioClip("Play", MixerType.UI, false, true);

            GameManager.Instance.Play();
            SetPlayButtonState(PlayButtonState.Replay);

            RoundProgress.gameObject.SetActive(false);

            PlayButton.GetComponent<RectTransform>().DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.3f).SetEase(Ease.OutBounce);
        }

        /// <summary>
        /// The play button press and hold
        /// </summary>
        private void OnPlayDown()
        {
            AudioManager.Instance.PlayAudioClip("Hold", MixerType.UI, true);

            GameManager.Instance.Prepare();
            RoundProgress.gameObject.SetActive(true);

            PlayButton.GetComponent<RectTransform>().DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f).SetEase(Ease.OutBounce);
        }

        /// <summary>
        /// The play button release
        /// </summary>
        private void OnPlayUp()
        {
            // Debug.Log("up");
            // OnPlayClick();
        }

        /// <summary>
        /// The play again button press
        /// </summary>
        private void OnReplayClick()
        {
            AudioManager.Instance.PlayAudioClip("Replay");

            if (GameManager.Instance.IsGameOver)
            {
                CloseWithAnimation();
            }

            GameManager.Instance.Replay();
            SetPlayButtonState(PlayButtonState.Play);
        }

        /// <summary>
        /// Sets all states for the play button
        /// </summary>
        public void SetPlayButtonState(PlayButtonState state)
        {
            currentPlayButtonState = state;
            if (PlayButton != null)
            {
                trigger.triggers.Clear();
                PlayButton.onClick.RemoveAllListeners();
                switch (state)
                {
                    case PlayButtonState.Placement:
                        PlayButton.onClick.AddListener(() => OnSpawnBallClick(0));
                        PlayButton.image.sprite = PlayButtonPlacementSprite;
                        break;

                    case PlayButtonState.Play:
                        PlayButton.onClick.AddListener(() => OnPlayClick());
                        PlayButton.image.sprite = PlayButtonPlaySprite;

                        if (trigger != null)
                        {
                            var pointerDown = new EventTrigger.Entry();
                            pointerDown.eventID = EventTriggerType.PointerDown;
                            pointerDown.callback.AddListener((e) => OnPlayDown());

                            trigger.triggers.Add(pointerDown);

                            var pointerUP = new EventTrigger.Entry();
                            pointerUP.eventID = EventTriggerType.PointerUp;
                            pointerUP.callback.AddListener((e) => OnPlayUp());

                            trigger.triggers.Add(pointerUP);
                        }

                        break;

                    case PlayButtonState.Replay:
                        PlayButton.onClick.AddListener(() => OnReplayClick());
                        PlayButton.image.sprite = PlayButtonReplaySprite;
                        break;

                    default:
                        break;
                }
            }
        }

        bool canClick = true;

        /// <summary>
        /// The open menu button press
        /// </summary>
        public void OnOpenClick()
        {
            AudioManager.Instance.PlayAudioClip("Caroulsel");

            if (canClick)
            {
                canClick = false;

                if (!isOpen)
                    Open();
                else
                    CloseWithAnimation();
            }
        }

        /// <summary>
        /// Called by the delegate when it's game over
        /// </summary>
        private void HandleGameOver(bool isGameOver)
        {
            if (isGameOver)
            {
                ShowResults();
            }
            else
            {
                HideResults();
            }
        }

        /// <summary>
        /// Shows the results screen
        /// </summary>
        private void ShowResults()
        {
            Open();

            ResultsTable.SetActive(true);

            MaxSpeedText.text = GameManager.Instance.GetResults()[0];
            TotalDistanceText.text = GameManager.Instance.GetResults()[1];
            ObjectsUsedText.text = GameManager.Instance.GetResults()[2];

            ItemList.SetActive(false);
            ListContainer.SetActive(false);
            TitleLines.SetActive(false);
            TitleText.gameObject.SetActive(false);
            MenuButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Hides the results screen
        /// </summary>
        private void HideResults()
        {
            ResultsTable.SetActive(false);

            ItemList.SetActive(true);
            ListContainer.SetActive(true);
            TitleLines.SetActive(true);
            TitleText.gameObject.SetActive(true);
            MenuButton.gameObject.SetActive(true);

            TitleText.text = OpenTitleString;
        }

        /// <summary>
        /// Opens the items list menu
        /// </summary>
        public override void Open()
        {
            gameObject.SetActive(true);

            PopUpManager.Instance.CloseAll();

            DOTween.Kill("timer");

            if (!GameManager.Instance.IsGameOver)
                ItemList.SetActive(true);
            else
                ItemList.SetActive(false);

            if (PlacementManager.Instance.InSceneObjectsCount < PlacementManager.Instance.MinObjectsInScene)
            {
                PlayButton.gameObject.SetActive(false);

                if (!PlacementManager.Instance.IsBallPlaced)
                    SetPlayButtonState(PlayButtonState.Placement);
            }
            else
            {
                PlayButton.gameObject.SetActive(true);

                if (PlacementManager.Instance.InSceneObjectsCount == PlacementManager.Instance.MinObjectsInScene && !PlacementManager.Instance.IsBallPlaced)
                {
                    PopUpManager.Instance.Show(1);

                    float timer = 0;
                    DOTween.To(() => timer, x => timer = x, 100, TimeToOpen).OnComplete(OpenMenuAndClosePopupAfterTimer).SetId("timer");
                }
            }

            if (!isOpen)
            {
                GetComponent<RectTransform>().DOAnchorPosY(0, 0.33f).SetEase(Ease.InOutSine).OnComplete(MenuAnimationCompleted);
                ItemList.GetComponent<RectTransform>().DOAnchorPosY(-2, 0.33f).SetEase(Ease.InOutSine);
                TitleText.DOColor(new Color(1, 1, 1, 0), 0.16f);
            }

            MenuButton.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 45), 0.33f);

            isOpen = true;
        }

        /// <summary>
        /// Closes the items list menu with no animation
        /// </summary>
        public override void Close()
        {
            gameObject.SetActive(true);
            ItemList.SetActive(true);

            if (PlacementManager.Instance.InSceneObjectsCount == 0 && !PopUpManager.Instance.IsShowing)
            {
                PopUpManager.Instance.Show(0);

                float timer = 0;
                DOTween.To(() => timer, x => timer = x, 100, TimeToOpen).OnComplete(OpenMenuAndClosePopupAfterTimer).SetId("timer");
            }
            else
                PopUpManager.Instance.CloseAll();

            if (PlacementManager.Instance.InSceneObjectsCount < PlacementManager.Instance.MinObjectsInScene)
                PlayButton.gameObject.SetActive(false);
            else
                PlayButton.gameObject.SetActive(true);

            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -360);
            ItemList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -105);

            TitleText.text = ClosedTitleString;

            MenuButton.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            isOpen = false;
        }

        /// <summary>
        /// Closes the menu with tween animation
        /// </summary>
        public void CloseWithAnimation()
        {
            GetComponent<RectTransform>().DOAnchorPosY(-360, 0.33f).SetEase(Ease.InOutSine).OnComplete(MenuAnimationCompleted);
            ItemList.GetComponent<RectTransform>().DOAnchorPosY(-105, 0.33f).SetEase(Ease.InOutSine);

            TitleText.DOColor(new Color(1, 1, 1, 0), 0.16f);

            MenuButton.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 0), 0.33f);

            isOpen = false;
        }

        /// <summary>
        /// Hides the list menu
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            ItemList.SetActive(false);
        }

        /// <summary>
        /// Setup for the list menu when it's animation tween is complete
        /// </summary>
        private void MenuAnimationCompleted()
        {
            if (isOpen)
            {
                TitleText.text = OpenTitleString;
                TitleText.DOColor(new Color(1, 1, 1, 1), 0.33f);
            }
            else
            {
                TitleText.text = ClosedTitleString;
                TitleText.DOColor(new Color(1, 1, 1, 1), 0.33f);

                ResultsTable.SetActive(false);

                ListContainer.SetActive(true);
                TitleLines.SetActive(true);
                TitleText.gameObject.SetActive(true);
                MenuButton.gameObject.SetActive(true);
            }

            canClick = true;
        }

        /// <summary>
        /// Opens the menu and closes pop-ups
        /// </summary>
        private void OpenMenuAndClosePopupAfterTimer()
        {
            PopUpManager.Instance.CloseAll(true);

            if (!isOpen)
                Open();
        }
    }
}
