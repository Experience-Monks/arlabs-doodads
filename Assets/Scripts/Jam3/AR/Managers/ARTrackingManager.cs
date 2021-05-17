using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Jam3.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jam3.AR
{
    // how plane tracking is going, if we have detected large enough planes
    public enum PlaneTrackingState
    {
        Disabled,
        Incomplete,
        Complete
    }

    // how tracking is going, if the phone is tracking at all
    public enum SessionState
    {
        NotTracking,
        Tracking
    }

    // a combination of both PlaneTrackingState and SessionState
    public enum TrackingState
    {
        NotTracking,
        TrackingInInsufficientSpace,
        Tracking
    }

    /// <summary>
    /// Manages everything related to AR tracking.
    /// </summary>
    public class ARTrackingManager : Singleton<ARTrackingManager>
    {
        public Action<TrackingState> TrackingUpdated;
        public Action<bool> ShouldBeTrackingUpdated;
        public Action<PlaneTrackingState> PlaneTrackingUpdated;

        private PlaneTrackingState planeTrackingState = PlaneTrackingState.Disabled;
        public PlaneTrackingState PlaneTrackingState
        {
            get {
                return planeTrackingState;
            }
            private set {
                // go through new and old state values and decide how to act for each combination of cases
                if (value == PlaneTrackingState.Disabled)
                {
                    planeTrackingState = value;
                }
                else if (value == PlaneTrackingState.Incomplete)
                {
                    // if it was complete and became incomplete, raise event
                    if (planeTrackingState == PlaneTrackingState.Complete)
                    {
                        // raise event
                        PlaneTrackingUpdated?.Invoke(value);
                    }
                    planeTrackingState = value;
                }
                else if (value == PlaneTrackingState.Complete)
                {
                    // if it was incomplete and became complete, raise event
                    if (planeTrackingState == PlaneTrackingState.Incomplete)
                    {
                        // raise event
                        PlaneTrackingUpdated?.Invoke(value);
                    }
                    planeTrackingState = value;
                }
                ReevaluateTracking();
            }
        }

        /// <summary>
        /// How much of the required space the user has scanned.
        /// </summary>
        /// <value></value>
        public float TrackingProgress
        {
            get {
                return Mathf.Clamp01(LargestGroundPlaneArea / minGroundPlaneArea);
            }
        }

        public SessionState SessionState { get; private set; } = SessionState.NotTracking;
        public TrackingState TrackingState { get; private set; } = TrackingState.NotTracking;

        /// <summary>
        /// AR Session enum for showing a reason when not tracking
        /// </summary>
        public NotTrackingReason NotTrackingReason { get; private set; } = ARSession.notTrackingReason;

        /// <summary>
        /// This is just a bool that can be set externally from any point to store whether Tracking should be established by a certain point.
        /// It is used by TrackingLostUI to tell if tracking lost screens should be displayed or not.
        /// </summary>
        public bool ShouldBeTracking
        {
            get {
                return shouldBeTracking;
            }
            set {
                shouldBeTracking = value;
                ShouldBeTrackingUpdated?.Invoke(shouldBeTracking);
            }
        }
        private bool shouldBeTracking = false;

        [Header("Calibration Parameters")]
        [SerializeField] private int minHorizontalPlanes = 1;
        [SerializeField] private int minVerticalPlanes = 0;
        [SerializeField] private float minGroundPlaneArea = 2f; // in square meters
        private bool isDebugging;
        public bool IsDebugging { get => isDebugging; }

        private PlaneManager planeManager = default;

        void Start()
        {
            // start the calibration sequence
            PlaneTrackingState = PlaneTrackingState.Incomplete;
        }

        private void OnEnable()
        {
            // cache the plane manager instance
            planeManager = PlaneManager.Instance;
            planeManager.PlanesUpdated += OnPlanesUpdated;

            ARSession.stateChanged += OnSessionStateChanged;
        }

        private void OnDisable()
        {
            planeManager.PlanesUpdated -= OnPlanesUpdated;

            ARSession.stateChanged += OnSessionStateChanged;
        }

        /// <summary>
        /// Handles ARSession State changes, to look for lost tracking.
        /// </summary>
        /// <param name="args"></param>
        private void OnSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            SessionState newState = (args.state == ARSessionState.SessionTracking) ? SessionState.Tracking : SessionState.NotTracking;

            // update session state
            SessionState = newState;

            ReevaluateTracking();
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <param name="args"></param>
        private void OnPlanesUpdated()
        {
            // ignore if not enabled
            if (PlaneTrackingState == PlaneTrackingState.Disabled)
                return;

            // recalculate calibration parameters
            RecalculatePlaneTracking();
        }

        /// <summary>
        /// Calculates copliance with tracking parameters.
        /// </summary>
        private void RecalculatePlaneTracking()
        {
            // check if the calibration parameters are fulfilled
            if (planeManager.HorizontalPlanes.Count >= minHorizontalPlanes && planeManager.Walls.Count >= minVerticalPlanes && LargestGroundPlaneArea >= minGroundPlaneArea)
            {
                PlaneTrackingState = PlaneTrackingState.Complete;
            }
            else
            {
                PlaneTrackingState = PlaneTrackingState.Incomplete;
            }
        }

        /// <summary>
        /// Check if the overall Tracking State has changed.
        /// </summary>
        private void ReevaluateTracking()
        {
            TrackingState newState = TrackingState.NotTracking;
            NotTrackingReason = ARSession.notTrackingReason;

            if (SessionState == SessionState.Tracking)
            {
                if (PlaneTrackingState == PlaneTrackingState.Incomplete)
                {
                    newState = TrackingState.TrackingInInsufficientSpace;
                }
                else if (PlaneTrackingState == PlaneTrackingState.Complete)
                {
                    newState = TrackingState.Tracking;
                }
            }

            if (newState != TrackingState)
            {
                TrackingState = newState;
                TrackingUpdated?.Invoke(TrackingState);
            }
        }

#if UNITY_EDITOR
        public void DebugTracking(NotTrackingReason reason, TrackingState state = TrackingState.NotTracking)
        {
            isDebugging = true;
            NotTrackingReason = reason;
            ARTrackingManager.Instance.TrackingUpdated.Invoke(state);
            isDebugging = false;
        }
#endif

        /// <summary>
        /// Returns the area of the largest detected ground plane.
        /// </summary>
        /// <value></value>
        private float LargestGroundPlaneArea
        {
            get {
                ARPlane largestGroundPlane = planeManager.LargestGroundPlane;
                if (largestGroundPlane == null)
                {
                    return 0f;
                }
                else
                {
                    return largestGroundPlane.size.Area();
                }
            }
        }
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(ARTrackingManager))]
    public class ObjectBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Tracking"))
            {
                ARTrackingManager.Instance.DebugTracking(NotTrackingReason.None, TrackingState.Tracking);
            }
            if (GUILayout.Button("Not Tracking: Excessive"))
            {
                ARTrackingManager.Instance.DebugTracking(NotTrackingReason.ExcessiveMotion);
            }
            if (GUILayout.Button("Not Tracking: Insufficient Features"))
            {
                ARTrackingManager.Instance.DebugTracking(NotTrackingReason.InsufficientFeatures);
            }
            if (GUILayout.Button("Not Tracking: InsufficientLight"))
            {
                ARTrackingManager.Instance.DebugTracking(NotTrackingReason.InsufficientLight);
            }
            if (GUILayout.Button("Not Tracking: Unsupported"))
            {
                ARTrackingManager.Instance.DebugTracking(NotTrackingReason.Unsupported);
            }
            if (GUILayout.Button("Not Tracking: None"))
            {
                ARTrackingManager.Instance.DebugTracking(NotTrackingReason.None);
            }
            if (GUILayout.Button("Insufficient"))
            {
                ARTrackingManager.Instance.DebugTracking(NotTrackingReason.None, TrackingState.TrackingInInsufficientSpace);
            }
        }
    }
#endif
}
