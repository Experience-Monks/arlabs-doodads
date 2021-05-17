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

            //StartCoroutine("Flow");
        }

        /*
        private IEnumerator Flow()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(1);
            OnComplete();
        }
        */

        Vector3 startMousePosition;

        bool isFading = false;

        private void Update()
        {
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

        public void FadeToNextPage()
        {
            isFading = true;

            FadeWall.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            FadeWall.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 0.33f).OnComplete(NextPage);
            FadeWall.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.33f).SetDelay(0.33f);

            PagesList[actualPageId].transform.Find("Middle").GetComponent<RectTransform>().DOAnchorPosX(-120, 0.33f);
        }

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

        public void FadeToPreviousPage()
        {
            isFading = true;

            FadeWall.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            FadeWall.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 0.33f).OnComplete(PreviousPage);
            FadeWall.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.33f).SetDelay(0.33f);

            PagesList[actualPageId].transform.Find("Middle").GetComponent<RectTransform>().DOAnchorPosX(120, 0.33f);
        }

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

        public void Skip()
        {
            actualPageId = 0;

            OnComplete();
        }

    }
}
