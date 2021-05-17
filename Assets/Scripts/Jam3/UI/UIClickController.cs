using UnityEngine;

namespace Jam3
{
    public class UIClickController : MonoBehaviour
    {
        private GameManager gameManager;

        public void OnPointerDown()
        {
            GameManager.Instance.TapEnabled = true;
        }

        public void OnPointerUp()
        {
            GameManager.Instance.TapEnabled = false;
        }
    }
}
