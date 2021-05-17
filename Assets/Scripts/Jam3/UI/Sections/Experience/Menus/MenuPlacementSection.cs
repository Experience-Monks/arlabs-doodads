using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

using TMPro;

namespace Jam3
{
    public class MenuPlacementSection : MenuSectionUI
    {
        public GameObject MenuContainer = null;
        public Image[] ImageOptions = null;
        public GameObject ObjectsCounter = null;
        public TMP_Text CounterText = null;

        private string counterInitialString = null;
        private bool isTexturesMenuOpen = false;

        private void Start()
        {
            counterInitialString = CounterText.text;
        }

        void Update()
        {
            if (isOpen)
            {
                if (Input.GetKeyDown("space"))
                {
                    PlaceObject();
                }
            }
        }

        public void SetEnable(bool value)
        {
            if (MenuContainer != null)
                MenuContainer.SetActive(value);

            SetTexturesMenuOpen(false, false);

            ReorderTextureMenu();
        }

        private void SetTexturesMenuOpen(bool setOpen, bool animated = true)
        {
            isTexturesMenuOpen = setOpen;

            float speed = 0;

            if (animated)
                speed = 0.3f;

            if (setOpen)
            {
                for (int i = 0; i < ImageOptions.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            ImageOptions[i].GetComponent<RectTransform>().DOAnchorPosY(33, speed).SetEase(Ease.InOutCirc);
                            break;

                        case 1:
                            ImageOptions[i].GetComponent<RectTransform>().DOAnchorPosY(240, speed).SetEase(Ease.InOutCirc);
                            break;

                        case 2:
                            ImageOptions[i].GetComponent<RectTransform>().DOAnchorPosY(449, speed).SetEase(Ease.InOutCirc);
                            break;

                        default:
                            ImageOptions[i].GetComponent<RectTransform>().DOAnchorPosY(33, speed).SetEase(Ease.InOutCirc);
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ImageOptions.Length; i++)
                {
                    ImageOptions[i].GetComponent<RectTransform>().DOAnchorPosY(33, speed).SetEase(Ease.InOutCirc);
                }
            }
        }

        private void ReorderTextureMenu()
        {
            for (int i = 0; i < ImageOptions.Length; i++)
            {
                ImageOptions[i].transform.SetAsFirstSibling();
            }

            var selectedObject = PlacementManager.Instance.SelectedObject != null ?
                PlacementManager.Instance.SelectedObject :
                SelectionManager.Instance.SelectedObject;

            if (selectedObject != null && selectedObject.IsCustomizable)
                ImageOptions[selectedObject.Customization.ColorId].transform.SetAsLastSibling();
        }

        public void SetTextures(int objectID)
        {
            if (ImageOptions != null)
            {
                if (objectID < ObjectManager.Instance.InteractiveElements.Length)
                {
                    int items = ObjectManager.Instance.InteractiveElements[objectID].ObjectTextures.Length;
                    for (int i = 0; i < items; i++)
                    {
                        Sprite sprite = ObjectManager.Instance.InteractiveElements[objectID].ObjectTextures[i].Icon;
                        if (ImageOptions[i] != null)
                            ImageOptions[i].sprite = sprite;
                    }
                }
            }
        }

        public void PlaceObject()
        {
            AudioManager.Instance.PlayAudioClip("ObjectSelection");

            PlacementManager.Instance.PlaceObject();
            SelectionManager.Instance.UnselectObject();

            if (MenuControllerUI != null)
                MenuControllerUI.SetMenuList();

            UpdateObjectsCounter();

            ReorderTextureMenu();

            GameManager.Instance.GameOver(false);
        }

        public void CancelPlacement()
        {
            AudioManager.Instance.PlayAudioClip("Reset");

            if (MenuControllerUI != null)
            {
                if (SelectionManager.Instance.SelectedObject.IsPhysic)
                    MenuControllerUI.ResetBall();

                PlacementManager.Instance.DeleteObject();

                MenuControllerUI.SetMenuList();
            }

            UpdateObjectsCounter();
            ReorderTextureMenu();

            GameManager.Instance.GameOver(false);
        }

        private void UpdateObjectsCounter()
        {
            if (PlacementManager.Instance.InSceneObjectsCount > 0)
                ObjectsCounter.SetActive(true);
            else
                ObjectsCounter.SetActive(false);

            int secondCountNumber = 0;

            if (PlacementManager.Instance.InSceneObjectsCount < PlacementManager.Instance.MinObjectsInScene)
                secondCountNumber = PlacementManager.Instance.MinObjectsInScene;
            else
                secondCountNumber = PlacementManager.Instance.MaxObjectsInScene;

            CounterText.SetText(counterInitialString, PlacementManager.Instance.InSceneObjectsCount, secondCountNumber);
        }

        public void HideObjectsCounter()
        {
            ObjectsCounter.SetActive(false);
        }

        public void ChangeTexture(int id)
        {
            AudioManager.Instance.PlayAudioClip("ObjectSelection");

            if (!isTexturesMenuOpen)
            {
                SetTexturesMenuOpen(true);
                return;
            }

            var clickedButtonTransform = EventSystem.current.currentSelectedGameObject.transform;
            clickedButtonTransform.transform.SetAsLastSibling();

            var selectedObject = PlacementManager.Instance.SelectedObject != null ?
                PlacementManager.Instance.SelectedObject :
                SelectionManager.Instance.SelectedObject;

            if (selectedObject != null && selectedObject.IsCustomizable)
                selectedObject.Customization.ChangeTexture(id);

            SetTexturesMenuOpen(false);
        }

        public void ChangeBallTriggerDirection()
        {
            if (SelectionManager.Instance.SelectedObject != null)
            {
                if (SelectionManager.Instance.SelectedObject.IsPhysic)
                    SelectionManager.Instance.SelectedObject.Physics.ChangeActionTriggerDirection();
                else
                    Debug.Log("Ball is not selected.");
            }
            else
                Debug.Log("Selected obj is null");
        }
    }
}
