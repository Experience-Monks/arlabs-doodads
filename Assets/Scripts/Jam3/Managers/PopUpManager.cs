using System;
using UnityEngine;
using UnityEngine.UI;
using Jam3.Util;

using TMPro;

namespace Jam3
{
    [Serializable]
    public class PopUpElement
    {
        public const int Id = 0;
        public GameObject PopUpPrefab = null;
        public Sprite Icon = null;
        public string Message = null;
    }

    public class PopUpManager : Singleton<PopUpManager>
    {
        public PopUpElement[] PopUps = null;

        public GameObject ClearConfirmation = null;
        public GameObject ResetConfirmation = null;

        public bool IsShowing { get => isShowing; }

        private bool isShowing;

        private void Start()
        {
            CloseAll();
        }

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

        public void ShowClearConfirmation()
        {
            CloseAll();

            ClearConfirmation.SetActive(true);
        }

        public void ShowResetConfirmation()
        {
            CloseAll();

            ResetConfirmation.SetActive(true);
        }

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
