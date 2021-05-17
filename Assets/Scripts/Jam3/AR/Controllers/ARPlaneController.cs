using System;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Jam3.AR
{
    public class ARPlaneController : MonoBehaviour
    {
        public GameObject PlaneObject = null;
        public ARPlaneManager planeManager = default;

        private Transform m_OriginTransform = null;
        private bool planeSet = false;

        private void OnEnable()
        {
            if (!Application.isEditor)
            {
                if (PlaneObject != null)
                {
                    PlaneObject.transform.position = new Vector3(0, 1000, 0);
                    PlaneObject.SetActive(false);
                }

                if (planeManager != null) planeManager.planesChanged += OnPlanesUpdated;
            }
        }

        private void OnDisable()
        {
            if (!Application.isEditor)
                if (planeManager != null) planeManager.planesChanged -= OnPlanesUpdated;
        }

        private void OnPlanesUpdated(ARPlanesChangedEventArgs args)
        {
            if (!Application.isEditor)
            {
                foreach (ARPlane plane in args.added)
                {
                    if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
                    {
                        Vector3 center = plane.center;
                        if (center.y <= PlaneObject.transform.position.y)
                        {
                            PlaneObject.transform.position = center;
                            PlaneObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}
