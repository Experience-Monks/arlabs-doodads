using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using Jam3.Util;

namespace Jam3.AR
{
    /// <summary>
    /// A layer of abstraction above ARPlaneManager, provides higher level information on planes.
    /// </summary>
    public class PlaneManager : Singleton<PlaneManager>
    {
        // events
        public Action PlanesUpdated;        // called when the planes are updated

        [SerializeField] private ARPlaneManager planeManager = default;

        /// <summary>
        /// The primary ARPlane where the user is standing on.
        /// </summary>
        /// <value></value>
        public ARPlane UserPlane { get; private set; }

        /// <summary>
        /// The largest horizontal ARPlane.
        /// </summary>
        /// <value></value>
        public ARPlane LargestGroundPlane { get; private set; }

        // keep a reference of all planes
        public List<ARPlane> HorizontalPlanes { get; private set; } = new List<ARPlane>();  // Surfaces plus UserPlane
        public List<ARPlane> Surfaces { get; private set; } = new List<ARPlane>();
        public List<ARPlane> Walls { get; private set; } = new List<ARPlane>();

        private void OnEnable()
        {
            if (planeManager != null) planeManager.planesChanged += OnPlanesUpdated;
        }

        private void OnDisable()
        {
            if (planeManager != null) planeManager.planesChanged -= OnPlanesUpdated;
        }

        /// <summary>
        /// Keeps track of all planes available.
        /// </summary>
        /// <param name="args"></param>
        private void OnPlanesUpdated(ARPlanesChangedEventArgs args)
        {
            // remove
            foreach (ARPlane plane in args.removed)
            {
                HorizontalPlanes.Remove(plane);
                Walls.Remove(plane);
            }

            // update
            // do nothing

            // add
            foreach (ARPlane plane in args.added)
            {
                if (plane.alignment == PlaneAlignment.Vertical)
                {
                    Walls.Add(plane);
                }
                else
                {
                    HorizontalPlanes.Add(plane);
                }
            }

            // recalculate core parameters
            Recalculate();
        }

        /// <summary>
        /// Calculates core parameters such as which plane is the UserPlane.
        /// </summary>
        private void Recalculate()
        {
            // calculate user plane
            UserPlane = GetUserPlane();

            // calculate largest plane
            LargestGroundPlane = GetLargestGroundPlane();

            // update the Surfaces and metrics
            Surfaces = new List<ARPlane>(HorizontalPlanes);
            Surfaces.Remove(UserPlane);

            // raise event
            // TODO: maybe don't raise the event if the calculated planes did not changes
            PlanesUpdated?.Invoke();
        }

        /// <summary>
        /// Helper method to get the UserPlane.
        /// </summary>
        /// <returns></returns>
        private ARPlane GetUserPlane()
        {
#if UNITY_EDITOR
            // in the Editor, return the pre-assigned variable
            return UserPlane;
#else
            // check if we have a main camera, which should be where the user is
            Camera cam = Camera.main;
            if (cam == null)
            {
                // if there is no camera
                // TODO: here we should create and return that fake plane @fabio was talking about.
                Debug.LogWarning("UserPlane can not be calculated without a main Camera.");
                return null;
            }
            else
            {
                NativeArray<XRRaycastHit> hits = planeManager.Raycast(new Ray(cam.transform.position, Vector3.down), TrackableType.Planes, Allocator.Temp);
                if (hits != null && hits.Length > 0)
                {
                    // get the largest plane we hit
                    ARPlane result = null;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        ARPlane plane = planeManager.GetPlane(hits[i].trackableId);
                        // get the largest horizontal plane
                        if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
                        {
                            if (result == null || plane.size.Area() > result.size.Area())
                            {
                                result = plane;
                            }
                        }
                    }
                    if (result != null)
                    {
                        return result;
                    }
                }

                // if nothing was returned...
                Debug.LogWarning("No UserPlane found.");
                return null;
            }
#endif
        }

        /// <summary>
        /// Helper method that returns the largest horizontal plane.
        /// </summary>
        /// <value></value>
        private ARPlane GetLargestGroundPlane()
        {
#if UNITY_EDITOR
            // in the Editor, return the pre-assigned variable
            return LargestGroundPlane;
#else
            ARPlane result = null;
            foreach (ARPlane plane in HorizontalPlanes)
            {
                if (result == null || plane.size.Area() > result.size.Area())
                {
                    result = plane;
                }
            }
            return result;
#endif
        }
    }

    /// <summary>
    /// Describes the core plane types for the game.
    /// </summary>
    public enum PlaneType
    {
        UserPlane,
        Surface,
        Wall
    }
}
