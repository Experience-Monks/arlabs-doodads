//-----------------------------------------------------------------------
// <copyright file="ARObject.cs" company="Jam3 Inc">
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

using System;
using UnityEngine;

namespace Jam3
{
    /// <summary>
    /// Base component for any AR object. Further features will be enabled depending on which other components are added to the same object. For example TransformableObject component.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    [RequireComponent(typeof(ObjectID))]
    public class ARObject : MonoBehaviour
    {
        #region Exposed fields

        [SerializeField]
        private GameObject meshObject;

        [SerializeField]
        private Transform snapObject;

        [SerializeField]
        private Transform pivotObject;

        [SerializeField]
        private Transform translateObject;

        [SerializeField]
        private Transform scaleObject;

        [SerializeField]
        private Transform rotateObject;

        [SerializeField]
        private Transform boundsLinkedObject;

        // Runtime
        private ObjectID cachedIdComponent;
        private Renderer cachedRendererComponent;
        private PhysicObject cachedPhysicComponent;
        private CustomizableObject cachedCustomizableComponent;
        private TransformableObject cachedTransformableComponent;
        private BoundsDrawer cachedBoundsDrawerComponent;

        private Bounds bounds;
        private bool selected;
        private Vector3 targetPosition;
        private float moveSpeed = 0.0f;

        #endregion Exposed fields

        #region Non Exposed fields

        #endregion Non Exposed fields

        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID
        {
            get => cachedIdComponent.ID;
            internal set => cachedIdComponent.ID = value;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ARObject"/> is selected.
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
        /// Gets a value indicating whether this instance is physic enabled (non kinematic).
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is physic enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsPhysic =>
            cachedPhysicComponent != null;

        /// <summary>
        /// Gets a value indicating whether this instance is customizable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is customizable; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomizable =>
            cachedCustomizableComponent != null;

        /// <summary>
        /// Gets a value indicating whether this instance is transformable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is transformable; otherwise, <c>false</c>.
        /// </value>
        public bool IsTransformable =>
            cachedTransformableComponent != null;


        /// <summary>
        /// Gets or sets the mesh object.
        /// </summary>
        /// <value>
        /// The mesh object.
        /// </value>
        public GameObject MeshObject
        {
            get => meshObject;
            set => meshObject = value;
        }

        /// <summary>
        /// Gets or sets the snap object.
        /// </summary>
        /// <value>
        /// The snap object object.
        /// </value>
        public Transform SnapObject
        {
            get => snapObject;
            set => snapObject = value;
        }

        /// <summary>
        /// Gets or sets the privot object.
        /// </summary>
        /// <value>
        /// The pivot object.
        /// </value>
        public Transform PivotObject
        {
            get => pivotObject;
            set => pivotObject = value;
        }

        /// <summary>
        /// Gets or sets the translate object.
        /// </summary>
        /// <value>
        /// The translate object.
        /// </value>
        public Transform TranslateObject
        {
            get => translateObject;
            set => translateObject = value;
        }

        /// <summary>
        /// Gets or sets the scale object. This will be the object scaled instead of the main object.
        /// If not set will be the same as the main object.
        /// </summary>
        /// <value>
        /// The scale object.
        /// </value>
        public Transform ScaleObject
        {
            get => scaleObject;
            set => scaleObject = value;
        }

        /// <summary>
        /// Gets or sets the rotate object. This will be the object rotated instead of the main object.
        /// If not set will be the same as the main object.
        /// </summary>
        /// <value>
        /// The rotate object.
        /// </value>
        public Transform RotateObject
        {
            get => rotateObject;
            set => rotateObject = value;
        }

        /// <summary>
        /// Gets or sets the bounds linked object.
        /// </summary>
        /// <value>
        /// The bounds linked object.
        /// </value>
        public Transform BoundsLinkedObject
        {
            get => boundsLinkedObject;
            set => boundsLinkedObject = value;
        }


        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public Bounds Bounds
        {
            get => bounds;
            private set => bounds = value;
        }

        /// <summary>
        /// Gets or sets the mesh renderer.
        /// </summary>
        /// <value>
        /// The mesh renderer.
        /// </value>
        public Renderer Renderer
        {
            get => cachedRendererComponent;
            private set => cachedRendererComponent = value;
        }

        /// <summary>
        /// Gets the <see cref="PhysicObject"/>.
        /// </summary>
        /// <value>
        /// The <see cref="PhysicObject"/>.
        /// </value>
        public PhysicObject Physics =>
            cachedPhysicComponent;

        /// <summary>
        /// Gets the <see cref="CustomizableObject"/>.
        /// </summary>
        /// <value>
        /// The <see cref="CustomizableObject"/>.
        /// </value>
        public CustomizableObject Customization =>
            cachedCustomizableComponent;

        /// <summary>
        /// Gets the <see cref="TransformableObject"/>.
        /// </summary>
        /// <value>
        /// The <see cref="TransformableObject"/>.
        /// </value>
        public TransformableObject Transformation =>
            cachedTransformableComponent;

        /// <summary>
        /// Gets the <see cref="BoundsDrawer"/>.
        /// </summary>
        /// <value>
        /// The <see cref="BoundsDrawer"/>.
        /// </value>
        public BoundsDrawer BoundsDrawer =>
            cachedBoundsDrawerComponent;

        #endregion Properties

        #region Custom Events

        /// <summary>
        /// Occurs when [selected event].
        /// </summary>
        public event SimpleEvent SelectedEvent;

        /// <summary>
        /// Occurs when [deselected event].
        /// </summary>
        public event SimpleEvent DeselectedEvent;

        /// <summary>
        /// Occurs when [placed event].
        /// </summary>
        public event SimpleEvent PlacedEvent;

        /// <summary>
        /// Occurs when [position set event].
        /// </summary>
        public event SimpleEvent<Vector3> PositionSetEvent;

        /// <summary>
        /// Occurs when [rotation set event].
        /// </summary>
        public event SimpleEvent<Vector3> RotationSetEvent;

        /// <summary>
        /// Occurs when [scale set event].
        /// </summary>
        public event SimpleEvent<Vector3> ScaleSetEvent;

        #endregion Custom Events

        #region Events methods

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        protected virtual void Awake()
        {
            // Get required components
            cachedIdComponent = GetComponent<ObjectID>();

            // Fetch components
            cachedRendererComponent = MeshObject.GetComponent<Renderer>();
            cachedPhysicComponent = GetComponent<PhysicObject>();
            cachedCustomizableComponent = GetComponent<CustomizableObject>();
            cachedTransformableComponent = GetComponent<TransformableObject>();
            cachedBoundsDrawerComponent = GetComponent<BoundsDrawer>();

            // Ensure components
            if (PivotObject == null)
                PivotObject = transform;

            if (TranslateObject == null)
                TranslateObject = transform;

            if (ScaleObject == null)
                ScaleObject = transform;

            if (RotateObject == null)
                RotateObject = transform;

            if (BoundsLinkedObject == null)
                BoundsLinkedObject = PivotObject;

            // Create bounds
            Bounds = new Bounds(Renderer, BoundsLinkedObject);

            // Events registration
            RegisterCallbacks();
        }

        /// <summary>
        /// Called upon destruction.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterCallbacks();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (TranslateObject != null)
                TranslateObject.position = Vector3.Lerp(TranslateObject.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Bounds != null)
                Bounds.UpdateBounds();

            if (BoundsDrawer != null)
            {
                BoundsDrawer.UpdateLabelsPosition();
                BoundsDrawer.UpdateWiresPosition();
            }

            if (Transformation != null)
                Transformation.Handler.UpdateHandlerPositions();
        }


        /// <summary>
        /// Called upon selection change.
        /// </summary>
        /// <param name="isSelecting"><c>true</c> when an object is being selected.</param>
        /// <param name="selectedObj">The selected object.</param>
        private void SelectionManager_OnSelection(bool isSelecting, ARObject selectedObj)
        {
            if (isSelecting && selectedObj == this)
            {
                Selected = true;
                SelectedEvent?.Invoke();
            }
            else
            {
                Selected = false;
                DeselectedEvent?.Invoke();
            }
        }

        #endregion Events methods

        #region Public Methods

        /// <summary>
        /// Places this instance.
        /// </summary>
        public void Place()
        {
            PlacedEvent?.Invoke();
        }

        /// <summary>
        /// Sets the global position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="speed">The speed.</param>
        public void SetWorldPosition(Vector3 position, float speed)
        {
            InternalSetWorldPosition(position, speed);

            PositionSetEvent?.Invoke(targetPosition);
        }

        /// <summary>
        /// Sets the local position.
        /// </summary>
        /// <param name="position">The local position.</param>
        /// <param name="speed">The speed.</param>
        public void SetLocalPosition(Vector3 localPosition, float speed)
        {
            var position = TranslateObject.TransformPoint(localPosition - TranslateObject.localPosition);

            InternalSetWorldPosition(position, speed);

            PositionSetEvent?.Invoke(targetPosition);
        }

        /// <summary>
        /// Sets the rotation.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        public void SetWorldRotation(Vector3 rotation)
        {
            InternalSetWorldRotation(rotation);

            RotationSetEvent?.Invoke(rotation);
        }

        /// <summary>
        /// Sets the local rotation.
        /// </summary>
        /// <param name="localRotation">The rotation.</param>
        public void SetLocalRotation(Vector3 localRotation)
        {
            InternalSetLocalRotation(localRotation);

            RotationSetEvent?.Invoke(localRotation);
        }

        /// <summary>
        /// Sets the local scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        public void SetLocalScale(Vector3 scale)
        {
            InternalSetLocalScale(scale);

            ScaleSetEvent?.Invoke(scale);
        }

        /// <summary>
        /// Gets the global position.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetWorldPosition() =>
            TranslateObject.position;

        /// <summary>
        /// Gets the local position.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocalPosition() =>
            TranslateObject.localPosition;

        /// <summary>
        /// Gets the world rotation.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetWorldRotation() =>
            RotateObject.rotation.eulerAngles;

        /// <summary>
        /// Gets the local rotation.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocalRotation() =>
            RotateObject.localRotation.eulerAngles;

        /// <summary>
        /// Gets the local scale.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocalScale() =>
            ScaleObject.localScale;

        #endregion Public Methods

        #region Non Public Methods

        /// <summary>
        /// Registers the callback.
        /// </summary>
        private void RegisterCallbacks()
        {
            SelectionManager.Instance.OnSelection += SelectionManager_OnSelection;
        }

        /// <summary>
        /// Unregisters the callback.
        /// </summary>
        private void UnregisterCallbacks()
        {
            SelectionManager.Instance.OnSelection -= SelectionManager_OnSelection;
        }


        /// <summary>
        /// Sets the global position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="speed">The speed.</param>
        internal void InternalSetWorldPosition(Vector3 position, float speed)
        {
            if (TranslateObject != null)
            {
                moveSpeed = speed > 0.0f ? speed : 1000f;
                targetPosition = position;

                //TranslateObject.position = position;
                //Bounds.UpdateBounds();
            }
        }

        /// <summary>
        /// Sets the global rotation.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        internal void InternalSetWorldRotation(Vector3 rotation)
        {
            if (RotateObject != null)
            {
                RotateObject.transform.eulerAngles = rotation;

                // Bounds.UpdateBounds();
            }
        }

        /// <summary>
        /// Sets the local rotation.
        /// </summary>
        /// <param name="localRotation">The rotation.</param>
        internal void InternalSetLocalRotation(Vector3 localRotation)
        {
            if (RotateObject != null)
            {
                RotateObject.transform.localEulerAngles = localRotation;

                // Bounds.UpdateBounds();
            }
        }

        /// <summary>
        /// Sets the local scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        internal void InternalSetLocalScale(Vector3 scale)
        {
            if (ScaleObject != null)
            {
                ScaleObject.transform.localScale = scale;

                // Bounds.UpdateBounds();
            }
        }

        #endregion Non Public Methods
    }
}
