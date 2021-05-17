using UnityEngine;
// using DG.Tweening;

namespace Jam3
{
    public abstract class MenuSectionUI : MonoBehaviour
    {
        public MenuControllerUI MenuControllerUI = null;

        [HideInInspector]
        public bool isOpen = false;

        public virtual void Open()
        {
            gameObject.SetActive(true);
            isOpen = true;
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            isOpen = false;
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
