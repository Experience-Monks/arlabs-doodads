using UnityEngine;
using TMPro;

namespace Jam3
{
    public class HelpControllerUI : MonoBehaviour
    {
        public GameObject ObjectContainer = null;
        public GameObject UIMenu = null;
        public GameObject UIPopUps = null;
        public TMP_Text MinText = null;
        public TMP_Text MaxText = null;

        public void Open()
        {
            AudioManager.Instance.PlayAudioClip("Help");

            gameObject.SetActive(true);

            if (ObjectContainer == null)
                ObjectContainer = GameObject.Find("/ObjectContaier");

            ObjectContainer.SetActive(false);
            UIMenu.SetActive(false);
            UIPopUps.SetActive(false);

            MinText.text = PlacementManager.Instance.MinObjectsInScene.ToString();
            MaxText.text = PlacementManager.Instance.MaxObjectsInScene.ToString() + ".";
        }

        public void Close()
        {
            AudioManager.Instance.PlayAudioClip("HelpClose");

            gameObject.SetActive(false);

            ObjectContainer.SetActive(true);
            UIMenu.SetActive(true);
            UIPopUps.SetActive(true);
        }
    }
}
