using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jam3.Util
{
    public class MultitouchScrollRect : ScrollRect
    {
        private bool isEnabled = true;
        public override void OnBeginDrag(PointerEventData eventData)
        {
            GetSingleTouchPosition(eventData);
            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            GetSingleTouchPosition(eventData);
            base.OnEndDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if(isEnabled) {
                GetSingleTouchPosition(eventData);
                base.OnDrag(eventData);
            }
        }

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
