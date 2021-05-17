//-----------------------------------------------------------------------
// <copyright file="TransformableObject.cs" company="Jam3 Inc">
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
    /// <summary>
    /// Allows an ARObject to be transformed and/or manipulated.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="Jam3.ARObject" />
    [RequireComponent(typeof(ARObject))]
    public class TransformableObject : MonoBehaviour
    {
        #region Exposed fields

        [Header("Config")]

        [SerializeField]
        public Layers dragLayer = Layers.Draggable;

        [SerializeField]
        private HandlerObject handlerPrefab;

        [SerializeField]
        private bool canInteract = false;

        [Header("Restrictions (Local Space)")]

        [SerializeField]
        private bool showTranslateHandler = true;
        [SerializeField]
        private Axis translateEnabledAxis = Axis.XYZ;

        [SerializeField]
        private bool showRotateHandler = true;
        [SerializeField]
        private GameObject alternativeRotateHandler = null;
        [SerializeField]
        private Axis rotateEnabledAxis = Axis.Y;

        [SerializeField]
        private bool showScaleHandler = true;
        [SerializeField]
        private Axis scaleEnabledAxis = Axis.XYZ;

        [SerializeField]
        private Vector3 translateMin = new Vector3(float.NegativeInfinity, 0f, float.NegativeInfinity);
        [SerializeField]
        private Vector3 translateMax = Vector3.positiveInfinity;

        [SerializeField]
        private Vector3 rotateMin = Vector3.negativeInfinity;
        [SerializeField]
        private Vector3 rotateMax = Vector3.positiveInfinity;

        [SerializeField]
        private Vector3 scaleMin = new Vector3(.25f, .25f, .25f);
        [SerializeField]
        private Vector3 scaleMax = new Vector3(4f, 4f, 4f);

        // Runtime
        private bool selected = false;

        #endregion Exposed fields

        #region Non Exposed fields

        private ARObject cachedArObjectComponent;
        private HandlerObject instancedHandler;

        private int layerMask = -1;

        #endregion Non Exposed fields

        #region Properties

        /// <summary>
        /// Gets or sets the base AR component.
        /// </summary>
        /// <value>
        /// The base AR component.
        /// </value>
        public ARObject ARBase =>
            cachedArObjectComponent;

        /// <summary>
        /// Gets the pivot object.
        /// </summary>
        /// <value>
        /// The pivot.
        /// </value>
        public Transform Pivot =>
            ARBase.PivotObject;

        /// <summary>
        /// Gets the target bounds.
        /// </summary>
        /// <value>
        /// The target bounds.
        /// </value>
        public Bounds TargetBounds =>
            ARBase.Bounds;


        /// <summary>
        /// Gets or sets the handler prefab.
        /// </summary>
        /// <value>
        /// The handler prefab.
        /// </value>
        public HandlerObject HandlerPrefab
        {
            get => handlerPrefab;
            set => handlerPrefab = value;
        }


        /// <summary>
        /// Gets a value indicating whether this instance can be interacted with.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can be interacted; otherwise, <c>false</c>.
        /// </value>
        public bool CanInteract
        {
            get => canInteract;
            set => canInteract = value;
        }

        /// <summary>
        /// Gets or sets if the objests shows the translate handler.
        /// </summary>
        /// <value>
        /// boolean to show translate handler.
        /// </value>
        public bool ShowTranslateHandler
        {
            get => showTranslateHandler;
            set => showTranslateHandler = value;
        }

        /// <summary>
        /// Gets or sets if the objests shows the rotate handler.
        /// </summary>
        /// <value>
        /// boolean to show rotate handler.
        /// </value>
        public bool ShowRotateHandler
        {
            get => showRotateHandler;
            set => showRotateHandler = value;
        }

        /// <summary>
        /// Gets or sets if the objests shows the scale handler.
        /// </summary>
        /// <value>
        /// boolean to show scale handler.
        /// </value>
        public bool ShowScaleHandler
        {
            get => showScaleHandler;
            set => showScaleHandler = value;
        }

        /// <summary>
        /// Gets or sets the translate enabled axis.
        /// </summary>
        /// <value>
        /// The translate enabled axis.
        /// </value>
        public Axis TranslateEnabledAxis
        {
            get => translateEnabledAxis;
            set
            {
                translateEnabledAxis = value;
                SetupHandler();
            }
        }

        /// <summary>
        /// Gets or sets the rotate enabled axis.
        /// </summary>
        /// <value>
        /// The rotation enabled axis.
        /// </value>
        public Axis RotateEnabledAxis
        {
            get => rotateEnabledAxis;
            set
            {
                rotateEnabledAxis = value;
                SetupHandler();
            }
        }

        /// <summary>
        /// Gets or sets the scale enabled axis.
        /// </summary>
        /// <value>
        /// The scale enabled axis.
        /// </value>
        public Axis ScaleEnabledAxis
        {
            get => scaleEnabledAxis;
            set
            {
                scaleEnabledAxis = value;
                SetupHandler();
            }
        }


        /// <summary>
        /// Gets or sets the alternative rotate handler.
        /// </summary>
        /// <value>
        /// The alternative rotate handler.
        /// </value>
        public GameObject AlternativeRotateHandler
        {
            get => alternativeRotateHandler;
            set => alternativeRotateHandler = value;
        }


        /// <summary>
        /// Gets or sets the translate minimum (local space).
        /// </summary>
        /// <value>
        /// The translate minimum.
        /// </value>
        public Vector3 TranslateMin
        {
            get => translateMin;
            set
            {
                translateMin = value;
                SetupHandler();
            }
        }

        /// <summary>
        /// Gets or sets the translate maximum (local space).
        /// </summary>
        /// <value>
        /// The translate maximum.
        /// </value>
        public Vector3 TranslateMax
        {
            get => translateMax;
            set
            {
                translateMax = value;
                SetupHandler();
            }
        }

        /// <summary>
        /// Gets or sets the rotate minimum (local space).
        /// </summary>
        /// <value>
        /// The rotate minimum.
        /// </value>
        public Vector3 RotateMin
        {
            get => rotateMin;
            set
            {
                rotateMin = value;
                SetupHandler();
            }
        }

        /// <summary>
        /// Gets or sets the rotate maximum (local space).
        /// </summary>
        /// <value>
        /// The rotate maximum.
        /// </value>
        public Vector3 RotateMax
        {
            get => rotateMax;
            set
            {
                rotateMax = value;
                SetupHandler();
            }
        }

        /// <summary>
        /// Gets or sets the scale minimum (local space).
        /// </summary>
        /// <value>
        /// The scale minimum.
        /// </value>
        public Vector3 ScaleMin
        {
            get => scaleMin;
            set
            {
                scaleMin = value;
                SetupHandler();
            }
        }

        /// <summary>
        /// Gets or sets the scale maximum (local space).
        /// </summary>
        /// <value>
        /// The scale maximum.
        /// </value>
        public Vector3 ScaleMax
        {
            get => scaleMax;
            set
            {
                scaleMax = value;
                SetupHandler();
            }
        }


        /// <summary>
        /// Gets a value indicating whether this <see cref="TransformableObject"/> is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if selected; otherwise, <c>false</c>.
        /// </value>
        public bool Selected
        {
            get => selected;
            private set => selected = value;
        }

        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public HandlerObject Handler
        {
            get => instancedHandler;
            private set => instancedHandler = value;
        }

        #endregion Properties

        #region Custom Events

        #endregion Custom Events

        #region Events methods

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            Reset();

            // Get required components
            cachedArObjectComponent = GetComponent<ARObject>();

            // Set layermask
            layerMask = 1 << (int)dragLayer;
            ObjectUtil.SetObjectLayer(gameObject, (int)dragLayer, true);

            // Create and setup the handler
            CreateHandler();

            // Events registration
            RegisterCallbacks();
        }

        /// <summary>
        /// Called upon destruction.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterCallbacks();
            DestoyHandler();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (ARBase.Selected && Handler != null)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    Handler.SetMode(HandlerObject.HandlerMode.Translate);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    Handler.SetMode(HandlerObject.HandlerMode.Rotate);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    Handler.SetMode(HandlerObject.HandlerMode.Scale);
                }
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        private void Reset()
        {
            DestoyHandler();
            CanInteract = true;
        }


        /// <summary>
        /// Called when the object is [placed].
        /// </summary>
        private void OnPlaced()
        {
            CanInteract = true;
        }

        /// <summary>
        /// Called when this object is selected.
        /// </summary>
        private void OnSelected()
        {
            AudioManager.Instance.PlayAudioClip("ObjectSelection");

            if (CanInteract && Handler != null)
                Handler.Show();
        }

        /// <summary>
        /// Called when this object is deselected.
        /// </summary>
        private void OnDeselected()
        {
            if (Handler != null)
                Handler.Hide();
        }

        /// <summary>
        /// Called when [position is set].
        /// </summary>
        /// <param name="position">The position.</param>
        private void OnPositionSet(Vector3 position)
        {
            if (Handler != null)
                Handler.UpdateHandlerPositions();
        }

        /// <summary>
        /// Called when [rotation is set].
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        private void OnRotationSet(Vector3 rotation)
        {
            if (Handler != null)
                Handler.UpdateHandlerPositions();
        }

        /// <summary>
        /// Called when [scale is set].
        /// </summary>
        /// <param name="scale">The scale.</param>
        private void OnScaleSet(Vector3 scale)
        {
            if (Handler != null)
                Handler.UpdateHandlerPositions();
        }

        #endregion Events methods

        #region Public Methods

        #endregion Public Methods

        #region Non Public Methods

        /// <summary>
        /// Registers the callbacks.
        /// </summary>
        private void RegisterCallbacks()
        {
            ARBase.SelectedEvent += OnSelected;
            ARBase.DeselectedEvent += OnDeselected;
            ARBase.PlacedEvent += OnPlaced;
            ARBase.PositionSetEvent += OnPositionSet;
            ARBase.RotationSetEvent += OnRotationSet;
            ARBase.ScaleSetEvent += OnScaleSet;
        }

        /// <summary>
        /// Unregisters the callbacks.
        /// </summary>
        private void UnregisterCallbacks()
        {
            ARBase.SelectedEvent -= OnSelected;
            ARBase.DeselectedEvent -= OnDeselected;
            ARBase.PlacedEvent -= OnPlaced;
            ARBase.PositionSetEvent -= OnPositionSet;
            ARBase.RotationSetEvent -= OnRotationSet;
            ARBase.ScaleSetEvent -= OnScaleSet;
        }


        /// <summary>
        /// Creates the handler.
        /// </summary>
        private void CreateHandler()
        {
            // Instantiate a new handler inside this object
            Handler = Instantiate(HandlerPrefab);
            Handler.transform.SetParent(transform, false);
            Handler.Target = this;

            // Initial setup
            SetupHandler();
        }

        /// <summary>
        /// Destoys the handler.
        /// </summary>
        private void DestoyHandler()
        {
            if (Handler != null)
            {
                Handler.Target = null;
                Destroy(Handler);
                Handler = null;
            }
        }

        /// <summary>
        /// Setups the handler.
        /// </summary>
        private void SetupHandler()
        {
            if (Handler != null)
            {
                Handler.TranslateEnabledAxis = TranslateEnabledAxis;
                Handler.ScaleEnabledAxis = ScaleEnabledAxis;
                Handler.RotateEnabledAxis = RotateEnabledAxis;
                Handler.TranslateMin = TranslateMin;
                Handler.TranslateMax = TranslateMax;
                Handler.RotateMin = RotateMin;
                Handler.RotateMax = RotateMax;
                Handler.ScaleMin = ScaleMin;
                Handler.ScaleMax = ScaleMax;
            }
        }

        #endregion Non Public Methods
    }
}
