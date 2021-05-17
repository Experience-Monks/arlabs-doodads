using System;
using UnityEngine;

using Jam3.Util;

namespace Jam3
{
    public class HandlerObject : MonoBehaviour
    {
        #region Enums

        /// <summary>
        /// Possible different handler working modes.
        /// </summary>
        public enum HandlerMode
        {
            Translate,
            Rotate,
            Scale
        }

        #endregion Enums

        #region Exposed fields

        [SerializeField]
        private float handlersOffset = 0.01f;

        [SerializeField]
        private float radiusOffset = 0.6f;

        [SerializeField]
        private Layers handlersXLayer = Layers.HandlerX;
        [SerializeField]
        private Layers handlersYLayer = Layers.HandlerY;
        [SerializeField]
        private Layers handlersZLayer = Layers.HandlerZ;

        [Header("Restrictions")]

        [SerializeField]
        private Axis translateEnabledAxis = Axis.XYZ;
        [SerializeField]
        private Axis rotateEnabledAxis = Axis.XYZ;
        [SerializeField]
        private Axis scaleEnabledAxis = Axis.XYZ;

        [SerializeField]
        private Axis translateWorldAxis = Axis.None;
        [SerializeField]
        private Axis rotateWorldAxis = Axis.Y;
        [SerializeField]
        private Axis scaleWorldAxis = Axis.None;

        [SerializeField]
        private Vector3 translateMin = new Vector3(float.NegativeInfinity, 0f, float.NegativeInfinity);
        [SerializeField]
        private Vector3 translateMax = Vector3.positiveInfinity;

        [SerializeField]
        private Vector3 rotateMin = Vector3.negativeInfinity;
        [SerializeField]
        private Vector3 rotateMax = Vector3.positiveInfinity;

        [SerializeField]
        private bool uniformScale = true;
        [SerializeField]
        private Vector3 scaleMin = new Vector3(.25f, .25f, .25f);
        [SerializeField]
        private Vector3 scaleMax = new Vector3(4f, 4f, 4f);

        [Header("Handler Objects")]

        [SerializeField]
        private Transform localSpaceHandlersContainer;
        [SerializeField]
        private Transform worldSpaceHandlersContainer;

        [SerializeField]
        private Transform translateXHandler;
        [SerializeField]
        private Transform translateYHandler;
        [SerializeField]
        private Transform translateZHandler;

        [SerializeField]
        private Transform rotateXHandler;
        [SerializeField]
        private Transform rotateYHandler;
        [SerializeField]
        private Transform rotateZHandler;

        [SerializeField]
        private Transform scaleXHandler;
        [SerializeField]
        private Transform scaleYHandler;
        [SerializeField]
        private Transform scaleZHandler;

        // Runtime
        private HandlerMode mode = HandlerMode.Translate;
        private TransformableObject target;

        #endregion Exposed fields

        #region Non Exposed fields

        private int xLayerMask = -1;
        private int yLayerMask = -1;
        private int zLayerMask = -1;

        private Axis activeAxis = Axis.None;

        private Ray xLocalHandlerRay;
        private Ray yLocalHandlerRay;
        private Ray zLocalHandlerRay;
        private Ray xWorldHandlerRay;
        private Ray yWorldHandlerRay;
        private Ray zWorldHandlerRay;

        private Vector3 xHandlerHitPoint;
        private Vector3 yHandlerHitPoint;
        private Vector3 zHandlerHitPoint;
        private Vector3 xScreenRayHitPoint;
        private Vector3 yScreenRayHitPoint;
        private Vector3 zScreenRayHitPoint;
        private float xHandlerLastOffset;
        private float yHandlerLastOffset;
        private float zHandlerLastOffset;

        #endregion Non Exposed fields

        #region Properties

        /// <summary>
        /// Gets or sets the handlers offset.
        /// </summary>
        /// <value>
        /// The handlers offset.
        /// </value>
        public float HandlersOffset
        {
            get => handlersOffset;
            set => handlersOffset = value;
        }

        /// <summary>
        /// Gets or sets the rotation radius handlers offset.
        /// </summary>
        /// <value>
        /// The rotation radius handlers offset.
        /// </value>
        public float RadiusOffset
        {
            get => radiusOffset;
            set => radiusOffset = value;
        }

        /// <summary>
        /// Gets or sets the handlers x layer.
        /// </summary>
        /// <value>
        /// The handlers x layer.
        /// </value>
        public Layers HandlersXLayer
        {
            get => handlersXLayer;
            set => handlersXLayer = value;
        }

        /// <summary>
        /// Gets or sets the handlers y layer.
        /// </summary>
        /// <value>
        /// The handlers y layer.
        /// </value>
        public Layers HandlersYLayer
        {
            get => handlersYLayer;
            set => handlersYLayer = value;
        }

        /// <summary>
        /// Gets or sets the handlers z layer.
        /// </summary>
        /// <value>
        /// The handlers z layer.
        /// </value>
        public Layers HandlersZLayer
        {
            get => handlersZLayer;
            set => handlersZLayer = value;
        }


        /// <summary>
        /// Gets or sets the translation enabled axis.
        /// </summary>
        /// <value>
        /// The translate axis.
        /// </value>
        public Axis TranslateEnabledAxis
        {
            get => translateEnabledAxis;
            set => translateEnabledAxis = value;
        }

        /// <summary>
        /// Gets or sets the rotation enabled axis.
        /// </summary>
        /// <value>
        /// The rotate axis.
        /// </value>
        public Axis RotateEnabledAxis
        {
            get => rotateEnabledAxis;
            set => rotateEnabledAxis = value;
        }

        /// <summary>
        /// Gets or sets the scaled enable axis.
        /// </summary>
        /// <value>
        /// The scale axis.
        /// </value>
        public Axis ScaleEnabledAxis
        {
            get => scaleEnabledAxis;
            set => scaleEnabledAxis = value;
        }

        /// <summary>
        /// Gets or sets the translate world axis.
        /// </summary>
        /// <value>
        /// The translate world axis.
        /// </value>
        public Axis TranslateWorldAxis
        {
            get => translateWorldAxis;
            set => translateWorldAxis = value;
        }

        /// <summary>
        /// Gets or sets the rotate world axis.
        /// </summary>
        /// <value>
        /// The rotate world axis.
        /// </value>
        public Axis RotateWorldAxis
        {
            get => rotateWorldAxis;
            set => rotateWorldAxis = value;
        }

        /// <summary>
        /// Gets or sets the scale world axis.
        /// </summary>
        /// <value>
        /// The scale world axis.
        /// </value>
        public Axis ScaleWorldAxis
        {
            get => scaleWorldAxis;
            set => scaleWorldAxis = value;
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
            set => translateMin = value;
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
            set => translateMax = value;
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
            set => rotateMin = value;
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
            set => rotateMax = value;
        }

        /// <summary>
        /// Gets or sets the scale minimum (local space).
        /// </summary>
        /// <value>
        /// The scale minimum.
        /// </value>
        public bool UniformScale
        {
            get => uniformScale;
            set => uniformScale = value;
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
            set => scaleMin = value;
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
            set => scaleMax = value;
        }

        /// <summary>
        /// Gets or sets the local space handlers container.
        /// </summary>
        /// <value>
        /// The local space handlers container.
        /// </value>
        public Transform LocalSpaceHandlersContainer
        {
            get => localSpaceHandlersContainer;
            set => localSpaceHandlersContainer = value;
        }

        /// <summary>
        /// Gets or sets the world space handlers container.
        /// </summary>
        /// <value>
        /// The world space handlers container.
        /// </value>
        public Transform WorldSpaceHandlersContainer
        {
            get => worldSpaceHandlersContainer;
            set => worldSpaceHandlersContainer = value;
        }

        /// <summary>
        /// Gets the translate x handler.
        /// </summary>
        /// <value>
        /// The translate x handler.
        /// </value>
        public Transform TranslateXHandler => translateXHandler;

        /// <summary>
        /// Gets the translate y handler.
        /// </summary>
        /// <value>
        /// The translate y handler.
        /// </value>
        public Transform TranslateYHandler => translateYHandler;

        /// <summary>
        /// Gets the translate z handler.
        /// </summary>
        /// <value>
        /// The translate z handler.
        /// </value>
        public Transform TranslateZHandler => translateZHandler;


        /// <summary>
        /// Gets the rotate x handler.
        /// </summary>
        /// <value>
        /// The rotate x handler.
        /// </value>
        public Transform RotateXHandler => rotateXHandler;

        /// <summary>
        /// Gets the rotate y handler.
        /// </summary>
        /// <value>
        /// The rotate y handler.
        /// </value>
        public Transform RotateYHandler => rotateYHandler;

        /// <summary>
        /// Gets the rotate z handler.
        /// </summary>
        /// <value>
        /// The rotate z handler.
        /// </value>
        public Transform RotateZHandler => rotateZHandler;


        /// <summary>
        /// Gets the scale x handler.
        /// </summary>
        /// <value>
        /// The scale x handler.
        /// </value>
        public Transform ScaleXHandler => scaleXHandler;

        /// <summary>
        /// Gets the scale y handler.
        /// </summary>
        /// <value>
        /// The scale y handler.
        /// </value>
        public Transform ScaleYHandler => scaleYHandler;

        /// <summary>
        /// Gets the scale z handler.
        /// </summary>
        /// <value>
        /// The scale z handler.
        /// </value>
        public Transform ScaleZHandler => scaleZHandler;


        /// <summary>
        /// Gets or sets the working mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public HandlerMode Mode
        {
            get => mode;
            set => mode = value;
        }

        /// <summary>
        /// Gets or sets the target. This will be the object used to update the handlers positions.
        /// </summary>
        /// <value>
        /// The visual feedback.
        /// </value>
        public TransformableObject Target
        {
            get => target;
            set => target = value;
        }


        /// <summary>
        /// Gets a value indicating whether the X handler is pointing towards world X.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is world handler; otherwise, <c>false</c>.
        /// </value>
        public bool IsXWorldHandler =>
            (Mode == HandlerMode.Translate && TranslateWorldAxis.HasFlag(Axis.X)) ||
            (Mode == HandlerMode.Rotate && RotateWorldAxis.HasFlag(Axis.X)) ||
            (Mode == HandlerMode.Scale && ScaleWorldAxis.HasFlag(Axis.X));

        /// <summary>
        /// Gets a value indicating whether the X handler is pointing towards world Y.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is world handler; otherwise, <c>false</c>.
        /// </value>
        public bool IsYWorldHandler =>
            (Mode == HandlerMode.Translate && TranslateWorldAxis.HasFlag(Axis.Y)) ||
            (Mode == HandlerMode.Rotate && RotateWorldAxis.HasFlag(Axis.Y)) ||
            (Mode == HandlerMode.Scale && ScaleWorldAxis.HasFlag(Axis.Y));

        /// <summary>
        /// Gets a value indicating whether the X handler is pointing towards world Z.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is world handler; otherwise, <c>false</c>.
        /// </value>
        public bool IsZWorldHandler =>
            (Mode == HandlerMode.Translate && TranslateWorldAxis.HasFlag(Axis.Z)) ||
            (Mode == HandlerMode.Rotate && RotateWorldAxis.HasFlag(Axis.Z)) ||
            (Mode == HandlerMode.Scale && ScaleWorldAxis.HasFlag(Axis.Z));

        /// <summary>
        /// Gets the x handler ray.
        /// </summary>
        /// <value>
        /// The x handler ray.
        /// </value>
        public Ray XHandlerRay
        {
            get
            {
                return IsXWorldHandler ? xWorldHandlerRay : xLocalHandlerRay;
            }
            set
            {
                if (IsXWorldHandler)
                    xWorldHandlerRay = value;
                else
                    xLocalHandlerRay = value;
            }
        }

        /// <summary>
        /// Gets the y handler ray.
        /// </summary>
        /// <value>
        /// The x handler ray.
        /// </value>
        public Ray YHandlerRay
        {
            get
            {
                return IsYWorldHandler ? yWorldHandlerRay : yLocalHandlerRay;
            }
            set
            {
                if (IsYWorldHandler)
                    yWorldHandlerRay = value;
                else
                    yLocalHandlerRay = value;
            }
        }

        /// <summary>
        /// Gets the z handler ray.
        /// </summary>
        /// <value>
        /// The x handler ray.
        /// </value>
        public Ray ZHandlerRay
        {
            get
            {
                return IsZWorldHandler ? zWorldHandlerRay : zLocalHandlerRay;
            }
            set
            {
                if (IsZWorldHandler)
                    zWorldHandlerRay = value;
                else
                    zLocalHandlerRay = value;
            }
        }

        #endregion Properties

        #region Custom Events

        public SimpleEvent<Vector3> TranslatedEvent;
        public SimpleEvent<Vector3> RotatedEvent;
        public SimpleEvent<Vector3> ScaledEvent;

        #endregion Custom Events

        #region Events methods

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            SetupGameObjectsLayer();
            RegisterCallbacks();
        }


        /// <summary>
        /// Triggered on touch down event.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="touchData">The touch data.</param>
        private void TouchDownEvent(int index, TouchManager.TouchElement touchData)
        {
            // Only take into account first touch
            if (index != 0)
            {
                return;
            }

            // Convert touch into world space ray
            var screenPos = touchData.position;
            var screenRay = Camera.main.ScreenPointToRay(screenPos);

            // Create handlers ray
            CreateHandlersRay();

            // Check if the touch hit any axis handler
            activeAxis = Axis.None;
            var handlerHitPoint = default(Vector3);
            var handlerRayHitPoint = default(Vector3);
            var screenRayHitPoint = default(Vector3);

            if (CheckHandlerInteraction(screenRay, XHandlerRay, xLayerMask, out handlerRayHitPoint, out screenRayHitPoint, out handlerHitPoint))
            {
                xHandlerLastOffset = 0;
                XHandlerRay = new Ray(handlerRayHitPoint, XHandlerRay.direction);
                xHandlerHitPoint = handlerHitPoint;
                xScreenRayHitPoint = screenRayHitPoint;
                activeAxis |= Axis.X;
            }

            else if (CheckHandlerInteraction(screenRay, YHandlerRay, yLayerMask, out handlerRayHitPoint, out screenRayHitPoint, out handlerHitPoint))
            {
                yHandlerLastOffset = 0;
                YHandlerRay = new Ray(handlerRayHitPoint, YHandlerRay.direction);
                yHandlerHitPoint = handlerHitPoint;
                yScreenRayHitPoint = screenRayHitPoint;
                activeAxis |= Axis.Y;
            }

            else if (CheckHandlerInteraction(screenRay, ZHandlerRay, zLayerMask, out handlerRayHitPoint, out screenRayHitPoint, out handlerHitPoint))
            {
                zHandlerLastOffset = 0;
                ZHandlerRay = new Ray(handlerRayHitPoint, ZHandlerRay.direction);
                zHandlerHitPoint = handlerHitPoint;
                zScreenRayHitPoint = screenRayHitPoint;
                activeAxis |= Axis.Z;
            }
        }

        /// <summary>
        /// Triggered on touch up event.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="touchData">The touch data.</param>
        private void TouchUpEvent(int index, TouchManager.TouchElement touchData)
        {
            // Only take into account first touch
            if (index != 0)
            {
                return;
            }

            // Stops interaction
            activeAxis = Axis.None;
        }

        /// <summary>
        /// Triggered on touch moved event.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="touchData">The touch data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void TouchMovedEvent(int index, TouchManager.TouchElement touchData)
        {
            // Only take into account first touch
            if (index != 0)
            {
                return;
            }

            // Ignore if not interacting with any handler
            if (activeAxis == Axis.None)
            {
                return;
            }

            // Manage the interaction
            DoInteraction(touchData);
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterCallbacks();
        }

        #endregion Events methods

        #region Public Methods

        /// <summary>
        /// Shows the handlers, remembering last working mode.
        /// </summary>
        public void Show() =>
            Show(Mode);

        /// <summary>
        /// Shows the handlers in the specified working mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public void Show(HandlerMode mode) =>
            SetMode(mode);

        /// <summary>
        /// Hides the handlers.
        /// </summary>
        public void Hide()
        {
            HideAllHandlers();
        }

        /// <summary>
        /// Sets the working mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public void SetMode(HandlerMode mode)
        {
            Mode = mode;
            HideAllHandlers();

            switch (mode)
            {
                case HandlerMode.Translate:
                    ShowTranslationHandlers();
                    break;
                case HandlerMode.Rotate:
                    ShowRotationHandlers();
                    break;
                case HandlerMode.Scale:
                    ShowScaleHandlers();
                    break;
            }

            UpdateHandlerPositions();
        }


        /// <summary>
        /// Updates the handler positions.
        /// </summary>
        public void UpdateHandlerPositions()
        {
            // Position the handlers container to match the object
            LocalSpaceHandlersContainer.position = Target.TargetBounds.WorldCenter;
            WorldSpaceHandlersContainer.position = Target.TargetBounds.WorldCenter;

            // Rotate only the local space handlers
            LocalSpaceHandlersContainer.rotation = Target.TargetBounds.Rotation;

            if (Target != null)
            {
                // Position the handlers in local space
                if (Mode == HandlerMode.Translate)
                {
                    TranslateXHandler.localPosition = new Vector3(Target.TargetBounds.Extents.x + HandlersOffset, 0, 0);
                    TranslateYHandler.localPosition = new Vector3(0, Target.TargetBounds.Extents.y + HandlersOffset, 0);
                    TranslateZHandler.localPosition = new Vector3(0, 0, Target.TargetBounds.Extents.z + HandlersOffset);
                }

                if (Mode == HandlerMode.Rotate)
                {
                    RotateXHandler.position = Target.TargetBounds.WorldCenter;
                    RotateYHandler.position = Target.TargetBounds.WorldCenter;
                    RotateZHandler.position = Target.TargetBounds.WorldCenter;

                    float radius = Target.TargetBounds.Size.x + RadiusOffset;
                    RotateXHandler.localScale = new Vector3(radius, radius, radius);
                    RotateYHandler.localScale = new Vector3(radius, radius, radius);
                    RotateZHandler.localScale = new Vector3(radius, radius, radius);
                }

                if (Mode == HandlerMode.Scale)
                {
                    ScaleXHandler.localPosition = new Vector3(Target.TargetBounds.Extents.x + HandlersOffset, 0, 0);
                    ScaleYHandler.localPosition = new Vector3(0, Target.TargetBounds.Extents.y + HandlersOffset, 0);
                    ScaleZHandler.localPosition = new Vector3(0, 0, Target.TargetBounds.Extents.z + HandlersOffset);
                }
            }
        }

        #endregion Public Methods

        #region Non Public Methods

        /// <summary>
        /// Registers the callback.
        /// </summary>
        private void RegisterCallbacks()
        {
            TouchManager.Instance.TouchDownEvent += TouchDownEvent;
            TouchManager.Instance.TouchUpEvent += TouchUpEvent;
            TouchManager.Instance.TouchMovedEvent += TouchMovedEvent;
        }

        /// <summary>
        /// Unregisters the callback.
        /// </summary>
        private void UnregisterCallbacks()
        {
            TouchManager.Instance.TouchDownEvent -= TouchDownEvent;
            TouchManager.Instance.TouchUpEvent -= TouchUpEvent;
            TouchManager.Instance.TouchMovedEvent -= TouchMovedEvent;
        }

        /// <summary>
        /// Setups the layers.
        /// </summary>
        private void SetupGameObjectsLayer()
        {
            ObjectUtil.SetObjectLayer(TranslateXHandler.gameObject, (int)HandlersXLayer, true);
            ObjectUtil.SetObjectLayer(TranslateYHandler.gameObject, (int)HandlersYLayer, true);
            ObjectUtil.SetObjectLayer(TranslateZHandler.gameObject, (int)HandlersZLayer, true);

            ObjectUtil.SetObjectLayer(RotateXHandler.gameObject, (int)HandlersXLayer, true);
            ObjectUtil.SetObjectLayer(RotateYHandler.gameObject, (int)HandlersYLayer, true);
            ObjectUtil.SetObjectLayer(RotateZHandler.gameObject, (int)HandlersZLayer, true);

            ObjectUtil.SetObjectLayer(ScaleXHandler.gameObject, (int)HandlersXLayer, true);
            ObjectUtil.SetObjectLayer(ScaleYHandler.gameObject, (int)HandlersYLayer, true);
            ObjectUtil.SetObjectLayer(ScaleZHandler.gameObject, (int)HandlersZLayer, true);

            xLayerMask = 1 << (int)HandlersXLayer;
            yLayerMask = 1 << (int)HandlersYLayer;
            zLayerMask = 1 << (int)HandlersZLayer;
        }


        /// <summary>
        /// Manages the interaction.
        /// </summary>
        private void DoInteraction(TouchManager.TouchElement touchData)
        {
            if (activeAxis != Axis.None)
            {
                var screenPos = touchData.position;
                var screenRay = Camera.main.ScreenPointToRay(screenPos);

                if (Mode == HandlerMode.Translate)
                    DoTranslateInteraction(screenRay);

                if (Mode == HandlerMode.Rotate)
                    DoRotateInteraction(screenRay);

                if (Mode == HandlerMode.Scale)
                    DoScaleInteraction(screenRay);
            }
        }

        /// <summary>
        /// Manages the translation iteraction.
        /// </summary>
        private void DoTranslateInteraction(Ray screenRay)
        {
            if (activeAxis.HasFlag(Axis.X))
                TranslateAxis(screenRay, XHandlerRay, xHandlerLastOffset, out xHandlerLastOffset);

            if (activeAxis.HasFlag(Axis.Y))
                TranslateAxis(screenRay, YHandlerRay, yHandlerLastOffset, out yHandlerLastOffset);

            if (activeAxis.HasFlag(Axis.Z))
                TranslateAxis(screenRay, ZHandlerRay, zHandlerLastOffset, out zHandlerLastOffset);
        }

        /// <summary>
        /// Manages the rotation iteraction.
        /// </summary>
        private void DoRotateInteraction(Ray screenRay)
        {
            if (activeAxis.HasFlag(Axis.X))
                RotateAxis(screenRay, XHandlerRay, IsXWorldHandler, xHandlerHitPoint, xHandlerLastOffset, 1f, out xHandlerLastOffset);

            if (activeAxis.HasFlag(Axis.Y))
                RotateAxis(screenRay, YHandlerRay, IsYWorldHandler, yHandlerHitPoint, yHandlerLastOffset, 1f, out yHandlerLastOffset);

            if (activeAxis.HasFlag(Axis.Z))
                RotateAxis(screenRay, ZHandlerRay, IsZWorldHandler, zHandlerHitPoint, zHandlerLastOffset, 1f, out zHandlerLastOffset);
        }

        /// <summary>
        /// Manages the scale iteraction.
        /// </summary>
        private void DoScaleInteraction(Ray screenRay)
        {
            if (activeAxis.HasFlag(Axis.X))
                ScaleAxis(screenRay, XHandlerRay, xHandlerLastOffset, 5f, out xHandlerLastOffset);

            if (activeAxis.HasFlag(Axis.Y))
                ScaleAxis(screenRay, YHandlerRay, yHandlerLastOffset, 2.5f, out yHandlerLastOffset);

            if (activeAxis.HasFlag(Axis.Z))
                ScaleAxis(screenRay, ZHandlerRay, zHandlerLastOffset, 5f, out zHandlerLastOffset);
        }


        /// <summary>
        /// Translates the target in a certain axis.
        /// </summary>
        /// <param name="screenRay">The screen ray.</param>
        /// <param name="handlerRay">The handler ray.</param>
        /// <param name="handlerLastOffset">The handler last offset.</param>
        /// <param name="newHandlerLastOffset">The new handler last offset.</param>
        private void TranslateAxis(Ray screenRay, Ray handlerRay, float handlerLastOffset, out float newHandlerLastOffset)
        {
            var handlerOffset = CalculateHandlerOffset(screenRay, handlerRay);
            var handlerDeltaOffset = handlerOffset - handlerLastOffset;

            if (Target != null)
            {
                // Note: We don't apply a direction conversion from world to local here,
                // because the translation object is not affected by the pivot object rotation
                var newLocalPosition = Target.ARBase.GetLocalPosition() + handlerRay.direction * handlerDeltaOffset;
                newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, TranslateMin.x, TranslateMax.x);
                newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, TranslateMin.y, TranslateMax.y);
                newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, TranslateMin.z, TranslateMax.z);
                Target.ARBase.SetLocalPosition(newLocalPosition, 0);
            }

            newHandlerLastOffset = handlerOffset;
        }

        /// <summary>
        /// Rotates the target in a certain axis.
        /// </summary>
        private void RotateAxis(Ray screenRay, Ray handlerRay, bool isWorldHandler, Vector3 handlerHitPoint, float handlerLastOffset, float multiplier, out float newHandlerLastOffset)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(screenRay.origin);
            var handlerProyectedPoint = ProjectPointOnPlane(handlerRay.direction, handlerRay.origin, handlerHitPoint);
            var handlerOffset = ScreenPointDistanceToLine(screenPoint, handlerRay.origin, handlerProyectedPoint);
            var handlerDeltaOffset = handlerOffset - handlerLastOffset;

            if (Target != null)
            {
                var handlerRayLocalDirection = !isWorldHandler ?
                    Quaternion.Inverse(Target.TargetBounds.Rotation) * handlerRay.direction :
                    handlerRay.direction;

                var newRotation = Target.ARBase.GetLocalRotation() + handlerRayLocalDirection * handlerDeltaOffset * multiplier * 0.01f * (180f / Mathf.PI);
                //newRotation.x = Mathf.Clamp(newRotation.x, RotateMin.x, RotateMax.x);
                //newRotation.y = Mathf.Clamp(newRotation.y, RotateMin.y, RotateMax.y);
                //newRotation.z = Mathf.Clamp(newRotation.z, RotateMin.z, RotateMax.z);

                Target.ARBase.SetLocalRotation(newRotation);
            }

            newHandlerLastOffset = handlerOffset;
        }

        /// <summary>
        /// Scales the target in a certain axis.
        /// </summary>
        private void ScaleAxis(Ray screenRay, Ray handlerRay, float handlerLastOffset, float multiplier, out float newHandlerLastOffset)
        {
            var handlerOffset = CalculateHandlerOffset(screenRay, handlerRay);
            var handlerDeltaOffset = handlerOffset - handlerLastOffset;

            if (Target != null)
            {
                var localDirection = Quaternion.Inverse(Target.TargetBounds.Rotation) * handlerRay.direction;
                var newScale = Target.ARBase.GetLocalScale();

                if (UniformScale)
                {
                    float unformValue = localDirection.magnitude * handlerDeltaOffset * multiplier;
                    newScale.x += unformValue;
                    newScale.y += unformValue;
                    newScale.z += unformValue;
                }
                else
                {
                    newScale += localDirection * handlerDeltaOffset * multiplier;
                }

                newScale.x = Mathf.Clamp(newScale.x, ScaleMin.x, ScaleMax.x);
                newScale.y = Mathf.Clamp(newScale.y, ScaleMin.y, ScaleMax.y);
                newScale.z = Mathf.Clamp(newScale.z, ScaleMin.z, ScaleMax.z);

                Target.ARBase.SetLocalScale(newScale);
            }

            newHandlerLastOffset = handlerOffset;
        }


        /// <summary>
        /// Shows the translation handlers.
        /// </summary>
        private void ShowTranslationHandlers()
        {
            HideAllHandlers();

            if (Target != null && Target.ShowTranslateHandler)
                ShowAxisHandlers(TranslateEnabledAxis, new[] { TranslateXHandler, TranslateYHandler, TranslateZHandler });
        }

        /// <summary>
        /// Shows the rotation handlers.
        /// </summary>
        private void ShowRotationHandlers()
        {
            HideAllHandlers();

            if (Target != null)
            {
                if (Target.ShowRotateHandler)
                    ShowAxisHandlers(RotateEnabledAxis, new[] { RotateXHandler, RotateYHandler, RotateZHandler });

                if (Target.AlternativeRotateHandler != null)
                    Target.AlternativeRotateHandler.SetActive(true);
            }
        }

        /// <summary>
        /// Shows the scale handlers.
        /// </summary>
        private void ShowScaleHandlers()
        {
            HideAllHandlers();

            if (Target != null && Target.ShowScaleHandler)
                ShowAxisHandlers(ScaleEnabledAxis, new[] { ScaleXHandler, ScaleYHandler, ScaleZHandler });
        }

        /// <summary>
        /// Hides all the handlers.
        /// </summary>
        private void HideAllHandlers()
        {
            if (Target != null && Target.AlternativeRotateHandler != null)
                Target.AlternativeRotateHandler.SetActive(false);

            if (translateXHandler != null)
                translateXHandler.gameObject.SetActive(false);

            if (translateYHandler != null)
                translateYHandler.gameObject.SetActive(false);

            if (translateZHandler != null)
                translateZHandler.gameObject.SetActive(false);

            if (rotateXHandler != null)
                rotateXHandler.gameObject.SetActive(false);

            if (rotateYHandler != null)
                rotateYHandler.gameObject.SetActive(false);

            if (rotateZHandler != null)
                rotateZHandler.gameObject.SetActive(false);

            if (scaleXHandler != null)
                scaleXHandler.gameObject.SetActive(false);

            if (scaleYHandler != null)
                scaleYHandler.gameObject.SetActive(false);

            if (scaleZHandler != null)
                scaleZHandler.gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the enabled axis handlers.
        /// </summary>
        /// <param name="enabledAxis">The enabled axis.</param>
        /// <param name="handlerObjects">The handler objects.</param>
        private void ShowAxisHandlers(Axis enabledAxis, Transform[] handlerObjects)
        {
            if (enabledAxis.HasFlag(Axis.X))
                handlerObjects[0].gameObject.SetActive(true);

            if (enabledAxis.HasFlag(Axis.Y))
                handlerObjects[1].gameObject.SetActive(true);

            if (enabledAxis.HasFlag(Axis.Z))
                handlerObjects[2].gameObject.SetActive(true);
        }


        /// <summary>
        /// Creates the handlers ray.
        /// </summary>
        private void CreateHandlersRay()
        {
            if (Target != null)
            {
                // Both rays are in world space, but localHandlerRays points towards object X, Y, Z directions
                xLocalHandlerRay = new Ray(Target.TargetBounds.WorldCenter, Target.TargetBounds.Rotation * Vector3.right);
                yLocalHandlerRay = new Ray(Target.TargetBounds.WorldCenter, Target.TargetBounds.Rotation * Vector3.up);
                zLocalHandlerRay = new Ray(Target.TargetBounds.WorldCenter, Target.TargetBounds.Rotation * Vector3.forward);

                xWorldHandlerRay = new Ray(Target.TargetBounds.WorldCenter, Vector3.right);
                yWorldHandlerRay = new Ray(Target.TargetBounds.WorldCenter, Vector3.up);
                zWorldHandlerRay = new Ray(Target.TargetBounds.WorldCenter, Vector3.forward);
            }
        }

        /// <summary>
        /// Checks if an input hit the specified handler.
        /// </summary>
        /// <param name="screenRay">The screen ray.</param>
        /// <param name="axisLayerMask">The axis layer mask.</param>
        /// <param name="handlerRay">The handler ray.</param>
        /// <param name="handlerOffset">The handler offset.</param>
        /// <returns></returns>
        private bool CheckHandlerInteraction(Ray screenRay, Ray handlerRay, int axisLayerMask, out Vector3 handlerRayHitPoint, out Vector3 screenRayHitPoint, out Vector3 handlerHitPoint)
        {
            var interacting = false;
            handlerRayHitPoint = default;
            screenRayHitPoint = default;
            handlerHitPoint = default;

            if (Physics.Raycast(screenRay, out RaycastHit hit, 100.0f, axisLayerMask))
            {
                // Ensure handler hit is ours
                var hitHandler = hit.transform.gameObject.GetComponentInParent<HandlerObject>();
                if (hitHandler == this)
                {
                    handlerHitPoint = hit.point;

                    // Set the handler ray hit point as origin (to be used in planar interaction)
                    if (ClosestPointsOnTwoRays(out screenRayHitPoint, out handlerRayHitPoint, screenRay, handlerRay))
                    {
                        interacting = true;
                    }
                }
            }

            return interacting;
        }

        /// <summary>
        /// Calculates the handler offset.
        /// </summary>
        private float CalculateHandlerOffset(Ray screenRay, Ray handlerRay)
        {
            var offset = default(float);

            if (ClosestPointsOnTwoRays(out Vector3 hitPoint, out _, handlerRay, screenRay))
            {
                var offsetVector = hitPoint - handlerRay.origin;
                var sign = Mathf.Sign(Vector3.Dot(offsetVector, handlerRay.direction));
                var magnitude = Vector3.Magnitude(offsetVector);
                offset = magnitude * sign;
            }

            return offset;
        }


        /// <summary>
        /// Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        /// to each other. This function finds those two points. If the lines are not parallel, the function
        /// outputs true, otherwise false.
        /// </summary>
        private bool ClosestPointsOnTwoRays(out Vector3 closestPointRay1, out Vector3 closestPointRay2, Ray ray1, Ray ray2) =>
            ClosestPointsOnTwoLines(out closestPointRay1, out closestPointRay2, ray1.origin, ray1.direction, ray2.origin, ray2.direction);

        /// <summary>
        /// Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        /// to each other. This function finds those two points. If the lines are not parallel, the function
        /// outputs true, otherwise false.
        /// </summary>
        private bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            // lines are not parallel
            if (d != 0.0f)
            {
                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;

                return true;
            }

            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the pixel distance from the screen point to a line.
        /// </summary>
        private float ScreenPointDistanceToLine(Vector3 screenPoint, Vector3 linePoint1, Vector3 linePoint2)
        {
            var camera = Camera.main;

            // Proyections to screen
            var screenLinePos1 = camera.WorldToScreenPoint(linePoint1);
            var screenLinePos2 = camera.WorldToScreenPoint(linePoint2);
            var screenLine = screenLinePos2 - screenLinePos1;

            // Flat zero depth
            screenPoint.z = 0;
            screenLinePos1.z = 0;
            screenLinePos2.z = 0;

            // Proyect screen point to screen line
            var projectedPoint = ProjectPointOnLine(screenLinePos1, screenLine.normalized, screenPoint);

            var vector = projectedPoint - screenPoint;
            var mag = vector.magnitude;
            var n = new Vector2(screenLine.y, -screenLine.x);
            var dot = Vector3.Dot(screenPoint - screenLinePos1, n);
            var sign = Mathf.Sign(dot);

            return mag * sign;
        }

        /// <summary>
        /// This function returns a point which is a projection from a point to a line.
        /// The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
        /// </summary>
        private Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
        {
            // get vector from point on line to point in space
            Vector3 linePointToPoint = point - linePoint;

            float t = Vector3.Dot(linePointToPoint, lineVec);

            return linePoint + lineVec * t;
        }

        /// <summary>
        /// This function returns a point which is a projection from a point to a plane.
        /// </summary>
        private Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            float distance;
            Vector3 translationVector;

            // First calculate the distance from the point to the plane:
            distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

            // Reverse the sign of the distance
            distance *= -1;

            // Get a translation vector
            translationVector = SetVectorLength(planeNormal, distance);

            // Translate the point to form a projection
            return point + translationVector;
        }

        /// <summary>
        /// Get the shortest distance between a point and a plane. The output is signed so it holds information
        /// as to which side of the plane normal the point is.
        /// </summary>
        private float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            return Vector3.Dot(planeNormal, (point - planePoint));
        }

        /// <summary>
        /// Create a vector of direction "vector" with length "size".
        /// </summary>
        private Vector3 SetVectorLength(Vector3 vector, float size)
        {
            // Normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(vector);

            // Scale the vector
            return vectorNormalized *= size;
        }

        #endregion Non Public Methods
    }
}
