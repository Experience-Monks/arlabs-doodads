//-----------------------------------------------------------------------
// <copyright file="OnboardingSectionUI.cs" company="Jam3 Inc">
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
using UnityEngine.UI;

using TMPro;

using DG.Tweening;

namespace Jam3
{
    public class OnboardingSectionUI : MonoBehaviour
    {
        public Action OnComplete = null;

        public List<GameObject> PagesList = null;

        public TMP_Text MinMaxText = null;

        public GameObject FadeWall = null;

        private int actualPageId = 0;

        private Vector3 startMousePosition;

        private bool isFading = false;

        /// <summary>
        /// Starts the section flow
        /// </summary>
        public void StartFlow(Action onComplete)
        {
            int count = 0;

            OnComplete = onComplete;

            gameObject.SetActive(true);

            MinMaxText.SetText("{0} - {1}", PlacementManager.Instance.MinObjectsInScene, PlacementManager.Instance.MaxObjectsInScene);

            foreach(GameObject page in PagesList)
            {
                if (count == 0)
                    page.SetActive(true);
                else
                    page.SetActive(false);

                count++;
            }
        }

        private void Update()
        {
            //Detecting swipe for page flip

            if (Input.GetMouseButtonDown(0))
            {
                startMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3 direction = (Input.mousePosition - startMousePosition).normalized;

                if (direction.x < -0.96f && !isFading)
                    FadeToNextPage();
                else if (direction.x > 0.96f && !isFading && actualPageId > 0)
                    FadeToPreviousPage();
            }
        }

        /// <summary>
        /// Fades to the next page
        /// </summary>
        public void FadeToNextPage()
        {
            isFading = true;

            FadeWall.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            FadeWall.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 0.33f).OnComplete(NextPage);
            FadeWall.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.33f).SetDelay(0.33f);

            PagesList[actualPageId].transform.Find("Middle").GetComponent<RectTransform>().DOAnchorPosX(-120, 0.33f);
        }

        /// <summary>
        /// Goes to the next page
        /// </summary>
        private void NextPage()
        {
            if(actualPageId < PagesList.Count - 1)
            {
                actualPageId++;

                PagesList[actualPageId - 1].SetActive(false);

                PagesList[actualPageId].transform.Find("Middle").GetComponent<RectTransform>().DOAnchorPosX(0, 0);

                PagesList[actualPageId].SetActive(true);
            }
            else
            {
                actualPageId = 0;

                OnComplete();
            }

            isFading = false;
        }

        /// <summary>
        /// Fades to the previous page
        /// </summary>
        public void FadeToPreviousPage()
        {
            isFading = true;

            FadeWall.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            FadeWall.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 0.33f).OnComplete(PreviousPage);
            FadeWall.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.33f).SetDelay(0.33f);

            PagesList[actualPageId].transform.Find("Middle").GetComponent<RectTransform>().DOAnchorPosX(120, 0.33f);
        }

        /// <summary>
        /// Goes to the previous page
        /// </summary>
        private void PreviousPage()
        {
            if (actualPageId < PagesList.Count)
            {
                PagesList[actualPageId].SetActive(false);

                actualPageId--;

                PagesList[actualPageId].transform.Find("Middle").GetComponent<RectTransform>().DOAnchorPosX(0, 0);

                PagesList[actualPageId].SetActive(true);
            }
            else
            {
                actualPageId = 0;
            }

            isFading = false;
        }

        /// <summary>
        /// Skips to the next section
        /// </summary>
        public void Skip()
        {
            actualPageId = 0;

            OnComplete();
        }

    }
}
