using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jam3.Util
{
    /// <summary>
    /// Multitouch scroll rect.
    /// </summary>
    /// <seealso cref="ScrollRect" />
    public class MultitouchScrollRect : ScrollRect
    {
        private bool isEnabled = true;

        /// <summary>
        /// On begin drag.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            GetSingleTouchPosition(eventData);
            base.OnBeginDrag(eventData);
        }


        /// <summary>
        /// On end drag.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            GetSingleTouchPosition(eventData);
            base.OnEndDrag(eventData);
        }


        /// <summary>
        /// On drag.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnDrag(PointerEventData eventData)
        {
            if (isEnabled)
            {
                GetSingleTouchPosition(eventData);
                base.OnDrag(eventData);
            }
        }


        /// <summary>
        /// Gets single touch position.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        private void GetSingleTouchPosition(PointerEventData eventData)
        {
            bool isRemoteConnected = false;
#if UNITY_EDITOR
            isRemoteConnected = UnityEditor.EditorApplication.isRemoteConnected;
#endif
            if ((SystemInfo.deviceType == DeviceType.Handheld || isRemoteConnected) && Input.touchCount > 0)
            {
                eventData.position = Input.touches[0].position;
            }
        }

        public void SetEnabled(bool enabled)
        {
            isEnabled = enabled;
        }
    }

}
