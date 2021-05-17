//-----------------------------------------------------------------------
// <copyright file="TouchManager.cs" company="Jam3 Inc">
//
// Copyright 2021 Jam3 Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;

using Jam3.Util;

namespace Jam3
{
    public class TouchManager : Singleton<TouchManager>
    {
        #region Subclasses

        /// <summary>
        /// Data about single screen touch.
        /// </summary>
        public class TouchElement
        {
            public Vector2 position = Vector2.zero;
            public Vector2 lastPosition = Vector2.zero;
            public GameObject cursorObject = null;
            public bool active = false;
            public bool moving = false;
        }

        #endregion Subclasses

        #region Exposed fields

        [SerializeField]
        private float doubleTapTime = 0.3f;

        [SerializeField]
        private int maxTouchs = 2;

        [Header("Debug")]

        [SerializeField]
        private bool showTouchPosition = true;

        [SerializeField]
        private Canvas uIDebugCanvas = null;

        [SerializeField]
        private GameObject uICursorObject = null;

        // Runtime
        private TouchElement[] touchElements = null;
        private int touchsActive = 0;
        private int touchsMoving = 0;

        #endregion Exposed fields

        #region Non Exposed fields

        // Runtime variables
        private readonly Vector2 outPosition = new Vector2(-1000f, -1000f);
        private bool simulatingSecondTouch = false;
        private bool simulatingTwofingersMoving = false;
        private int mouseTargetTouchIndex = 0;
        private bool doubleTap = false;
        private float lastTapTime = 0.0f;

        #endregion Non Exposed fields

        #region Properties

        /// <summary>
        /// Gets or sets the double tap flag.
        /// </summary>
        /// <value>
        /// If the user double tap.
        /// </value>
        public bool DoubleTap
        {
            get => doubleTap;
            private set => doubleTap = value;
        }

        /// <summary>
        /// Gets or sets the double tap time.
        /// </summary>
        /// <value>
        /// the time to double tap.
        /// </value>
        public float DoubleTapTime
        {
            get => doubleTapTime;
            set => doubleTapTime = value;
        }

        /// <summary>
        /// Gets or sets the maximum touchs.
        /// </summary>
        /// <value>
        /// The maximum touchs.
        /// </value>
        public int MaxTouchs
        {
            get => maxTouchs;
            set => maxTouchs = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether every touch should be shown in the screen.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show touch position]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowTouchPosition
        {
            get => showTouchPosition;
            set => showTouchPosition = value;
        }

        /// <summary>
        /// Gets or sets the UI debug canvas.
        /// </summary>
        /// <value>
        /// The UI debug canvas.
        /// </value>
        public Canvas UIDebugCanvas
        {
            get => uIDebugCanvas;
            set => uIDebugCanvas = value;
        }

        /// <summary>
        /// Gets or sets the UI cursor object prefab.
        /// </summary>
        /// <value>
        /// The UI cursor object.
        /// </value>
        public GameObject UICursorObject
        {
            get => uICursorObject;
            set => uICursorObject = value;
        }


        /// <summary>
        /// Gets the touch elements.
        /// </summary>
        /// <value>
        /// The touch elements.
        /// </value>
        public TouchElement[] TouchElements
        {
            get => touchElements;
            private set => touchElements = value;
        }

        /// <summary>
        /// Gets the touchs active.
        /// </summary>
        /// <value>
        /// The touchs active.
        /// </value>
        public int TouchsActive
        {
            get => touchsActive;
            private set => touchsActive = value;
        }

        /// <summary>
        /// Gets the touchs moving.
        /// </summary>
        /// <value>
        /// The touchs moving.
        /// </value>
        public int TouchsMoving
        {
            get => touchsMoving;
            private set => touchsMoving = value;
        }

        #endregion Properties

        #region Custom Events

        public delegate void TouchEvent(int index, TouchElement touchData);
        public event TouchEvent TouchDownEvent;
        public event TouchEvent TouchUpEvent;
        public event TouchEvent TouchMovedEvent;

        #endregion Custom Events

        #region Events methods

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            // Initialize the Touch Elements
            CreateTouchs();

            // Initialize the cursors
            CreateCursors();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            bool tapEnabled = GameManager.Instance.TapEnabled;

            // Input update
            if (Input.touchSupported && Input.touchCount > 0)
                TouchInteraction(tapEnabled);
            else
                MouseInteraction(tapEnabled);

            // Visual reference
            SetCursors();
        }

        #endregion Events methods

        #region Public Methods

        #endregion Public Methods

        #region Non Public Methods

        /// <summary>
        /// Creates the touch elements for future use.
        /// </summary>
        private void CreateTouchs()
        {
            TouchElements = new TouchElement[MaxTouchs];

            for (int i = 0; i < TouchElements.Length; i++)
            {
                TouchElements[i] = new TouchElement();
            }
        }

        /// <summary>
        /// Tracks the mouse interaction as a fallback for Editor.
        /// </summary>
        /// <param name="tapEnabled">if set to <c>true</c> [tap enabled].</param>
        private void MouseInteraction(bool tapEnabled)
        {
            TouchsMoving = 0;
            doubleTap = false;

            var realTouchsActive = simulatingSecondTouch ? TouchsActive - 1 : TouchsActive;

            // Simulate second touch only if a touch was not already active
            if (realTouchsActive == 0 &&
                Input.GetKeyDown(KeyCode.LeftControl))
            {
                simulatingSecondTouch = true;
                TouchDown(0, Input.mousePosition);
            }

            // Terminate second touch only if it was being simulated
            if (simulatingSecondTouch &&
                Input.GetKeyUp(KeyCode.LeftControl))
            {
                simulatingSecondTouch = false;
                TouchUp(0, Input.mousePosition);
            }

            if (tapEnabled && Input.GetMouseButtonDown((int)MouseButtons.Right))
                simulatingTwofingersMoving = true;

            if (tapEnabled && Input.GetMouseButtonUp((int)MouseButtons.Right))
                simulatingTwofingersMoving = false;


            // Detect mouse input for simulating touch
            if (tapEnabled && Input.GetMouseButtonDown((int)MouseButtons.Left))
            {
                SetDoubleTap();

                mouseTargetTouchIndex = simulatingSecondTouch ? 1 : 0;
                TouchDown(mouseTargetTouchIndex, Input.mousePosition);
            }

            // Terminate touch previously started (may be first or second touch)
            if (Input.GetMouseButtonUp((int)MouseButtons.Left))
            {
                TouchUp(mouseTargetTouchIndex, Input.mousePosition);
            }

            // Update real touchs count since it may have changed if the modifier key was pressed
            realTouchsActive = simulatingSecondTouch ? TouchsActive - 1 : TouchsActive;
            if (realTouchsActive > 0)
            {
                var moving = (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
                if (moving)
                {
                    var movingTouchIndex = TouchElements[1].active ? 1 : 0;
                    SetTouchPosition(movingTouchIndex, Input.mousePosition, true);

                    TouchsMoving = simulatingTwofingersMoving ? 2 : 1;
                }
            }
        }

        /// <summary>
        /// Tracks the touchs interation.
        /// </summary>
        /// <param name="tapEnabled">if set to <c>true</c> [tap enabled].</param>
        private void TouchInteraction(bool tapEnabled)
        {
            TouchsMoving = 0;
            doubleTap = false;

            for (int i = 0; i < MaxTouchs; i++)
            {
                if (i < Input.touchCount)
                {
                    var touch = Input.GetTouch(i);

                    // Touch down
                    if (tapEnabled && touch.phase == TouchPhase.Began)
                    {
                        if (i == 0)
                            SetDoubleTap();

                        TouchDown(i, touch.position);
                    }

                    // Touch moved
                    else if (tapEnabled && touch.phase == TouchPhase.Moved)
                    {
                        // Update touch position
                        SetTouchPosition(i, touch.position, true);

                        // Counter
                        TouchsMoving++;
                    }

                    // Touch up
                    else if (touch.phase == TouchPhase.Ended)
                        TouchUp(i, touch.position);
                }
            }
        }

        /// <summary>
        /// Indicates a particular touch has started.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="position">The position.</param>
        private void TouchDown(int index, Vector2 position)
        {
            // Counter
            TouchsActive++;
            TouchsActive = Mathf.Clamp(TouchsActive, 0, MaxTouchs);

            // Set as active
            TouchElements[index].active = true;

            // Update touch position
            SetTouchPosition(index, position, false);

            // Trigger event
            TouchDownEvent?.Invoke(index, TouchElements[index]);
        }

        /// <summary>
        /// Indicates a particular touch has ended.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="position">The position.</param>
        private void TouchUp(int index, Vector2 position)
        {
            // Counter
            TouchsActive--;
            TouchsActive = Mathf.Clamp(TouchsActive, 0, MaxTouchs);

            // Set as inactive
            TouchElements[index].active = false;

            // Update touch position
            SetTouchPosition(index, position, false);

            // Trigger event
            TouchUpEvent?.Invoke(index, TouchElements[index]);
        }

        /// <summary>
        /// Checks if the user DoubleTap.
        /// </summary>
        private void SetDoubleTap()
        {
            if (Time.time < lastTapTime + DoubleTapTime)
                doubleTap = true;

            lastTapTime = Time.time;
        }


        /// <summary>
        /// Sets the touch position for a given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="position">The position.</param>
        /// <param name="triggerEvent">if set to <c>true</c> will trigger touch movement event.</param>
        private void SetTouchPosition(int index, Vector2 position, bool triggerEvent)
        {
            if (TouchElements != null && index < TouchElements.Length)
            {
                TouchElements[index].position.x = position.x;
                TouchElements[index].position.y = position.y;

                // Trigger event
                if (triggerEvent)
                    TouchMovedEvent?.Invoke(index, TouchElements[index]);
            }
        }

        /// <summary>
        /// Creates the visual cursors.
        /// </summary>
        private void CreateCursors()
        {
            if (UIDebugCanvas != null && UICursorObject != null)
            {
                for (int i = 0; i < TouchElements.Length; i++)
                {
                    var UICursor = Instantiate(UICursorObject, new Vector3(outPosition.x, outPosition.y, 0.0f), Quaternion.identity);
                    UICursor.transform.SetParent(UIDebugCanvas.transform, false);
                    TouchElements[i].cursorObject = UICursor;
                }
            }
        }

        /// <summary>
        /// Sets the visual cursors.
        /// </summary>
        private void SetCursors()
        {
            if (TouchElements != null)
            {
                int i = 0;
                foreach (var touchElement in TouchElements)
                {
                    if (touchElement.cursorObject != null)
                    {
                        if (touchElement.active)
                        {
                            touchElement.cursorObject.SetActive(ShowTouchPosition);
                        }
                        else
                        {
                            touchElement.position.x = outPosition.x;
                            touchElement.position.y = outPosition.y;
                            touchElement.cursorObject.SetActive(false);
                        }
                        touchElement.cursorObject.transform.position = touchElement.position;
                    }
                    i++;
                }
            }
        }

        #endregion Non Public Methods
    }
}
