using System;
using UnityEngine;
using Jam3.Util;

namespace Jam3
{
    public class TransformManager : Singleton<TransformManager>
    {
        #region Enums

        /// <summary>
        /// Possible different rotation modes.
        /// </summary>
        public enum RotationMode
        {
            Angle,
            OneFinger,
            TwoFingers
        }

        /// <summary>
        /// Possible different translation modes.
        /// </summary>
        public enum TranslationMode
        {
            Grab,
            Tap,
            DoubleTap
        }

        #endregion Enums

        [Header("Config")]
        public bool AutoSetTranformMode = false;

        [Header("Move")]
        public bool CanMove = true;
        public Axis TranslateEnabledAxis = Axis.XZ;
        public TranslationMode TranslationType = TranslationMode.Tap;
        public Layers SurfaceLayer = Layers.Surface;
        public Layers BackgroundLayer = Layers.BackgroundMesh;
        [Range(0.1f, 10.0f)]
        public float MoveSpeed = 3f;

        [Header("Rotation")]
        public bool CanRotate = true;
        public RotationMode RotationType = RotationMode.OneFinger;
        [Range(100f, 500f)]
        public float FingerDistanceThreshold = 250f;
        public float RotationStep = 30f;
        [Range(0.1f, 10.0f)]
        public float RotationSpeed = 1f;

        [Header("Scale")]
        public bool CanScale = true;
        [Range(0.1f, 5f)]
        public float ScaleSpeed = 1.0f;
        [Range(0.1f, 1.0f)]
        public float MinScale = 0.1f;
        [Range(1.5f, 5.0f)]
        public float MaxScale = 5f;

        //Runtime
        private float scaleSize = 700f;
        private int hitLayerMask = -1;
        private int hitLayerMaskSecundary = -1;

        private ARObject selectedObject = null;

        private Vector3 objectScale = Vector3.zero;
        private Vector3 initialScale = Vector3.zero;

        private Vector3 objectRotation = Vector3.zero;

        private Vector3 initialRotation = Vector3.zero;
        private Vector3 objectPosition = Vector3.zero;

        private float touchScale = 0.0f;

        private float touchRotationY = 0.0f;
        private float touchRotationX = 0.0f;

        private bool isRotating = false;
        private bool isScaling = false;
        private bool isMoving = false;

        private HandlerObject.HandlerMode mode = HandlerObject.HandlerMode.Translate;
        private RaycastHit[] rayHits;

        /// <summary>
        /// Gets the <see cref="ARObject"/>.
        /// </summary>
        /// <value>
        /// The <see cref="ARObject"/>.
        /// </value>
        public ARObject SelectedObject
        {
            get { return selectedObject; }
            set {
                selectedObject = value;
                if (selectedObject != null)
                {
                    if (selectedObject.RotateObject != null)
                    {
                        objectRotation = selectedObject.GetWorldRotation();
                        initialRotation = objectRotation;
                    }

                    if (selectedObject.ScaleObject != null)
                    {
                        objectScale = selectedObject.GetLocalScale();
                        initialScale = objectScale;
                    }
                }
            }
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            rayHits = new RaycastHit[1];
            scaleSize = Screen.width;
            hitLayerMask = 1 << (int)SurfaceLayer | 1 << (int)BackgroundLayer;

            RegisterCallbacks();
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterCallbacks();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        void Update()
        {
            if (selectedObject != null)
            {
                int moving = TouchManager.Instance.TouchsMoving;
                bool doubleTap = TouchManager.Instance.DoubleTap;
                mode = selectedObject.Transformation.Handler.Mode;

                bool alowMoving = false;
                if (TranslationType == TranslationMode.Grab)
                    alowMoving = moving > 0;
                else if (TranslationType == TranslationMode.Tap)
                    alowMoving = moving > -1;
                else if (TranslationType == TranslationMode.DoubleTap)
                    alowMoving = moving > -1 && doubleTap;

                if (doubleTap)
                {
                    var targetObj = SelectionManager.Instance.GetObjectHit();
                    if (targetObj != null && !targetObj.Selected)
                    {
                        ObjectManager.Instance.MoveBallToSnap(targetObj.SnapObject.position);
                        alowMoving = false;
                    }
                }

                if ((mode == HandlerObject.HandlerMode.Translate || AutoSetTranformMode) &&
                CanMove && selectedObject.Transformation.CanInteract && isMoving && alowMoving)
                    MoveObject();

                bool alowRotation = false;
                if (RotationType == RotationMode.OneFinger)
                    alowRotation = moving > 0;
                else if (RotationType == RotationMode.TwoFingers)
                    alowRotation = moving > 1;

                if ((mode == HandlerObject.HandlerMode.Rotate || AutoSetTranformMode) &&
                CanRotate && isRotating && alowRotation)
                    RotateObject();

                if ((mode == HandlerObject.HandlerMode.Scale || AutoSetTranformMode) &&
                CanScale && isScaling && moving > 0)
                    ScaleObject();
            }
        }

        public void SetMode(HandlerObject.HandlerMode handlerMode)
        {
            mode = handlerMode;
        }

        private void OnSelect(bool isSelecting, ARObject objectSelected)
        {
            if (isSelecting)
                SelectedObject = objectSelected;
            else
                SelectedObject = null;
        }

        private void OnTouchDown(int index, TouchManager.TouchElement touchData)
        {
            int touchCount = TouchManager.Instance.TouchsActive;

            ARObject hitObject = null;
            if (TranslationType == TranslationMode.Grab)
                hitObject = SelectionManager.Instance.GetObjectHit();
            else
                hitObject = selectedObject;

            if (touchCount == 1)
            {
                if (hitObject != null)
                {
                    isRotating = false;
                    isMoving = true;
                    isScaling = false;

                    if (RotationType == RotationMode.OneFinger)
                        isRotating = true;
                }
                else
                    isMoving = false;
            }
            else if (touchCount == 2)
            {
                isRotating = false;
                isMoving = false;
                isScaling = true;

                if (RotationType == RotationMode.TwoFingers || RotationType == RotationMode.Angle)
                    isRotating = true;
            }

            if (isRotating)
                SetInitialRotation();

            if (isScaling)
                SetInitialScale();
        }

        private void OnTouchUp(int index, TouchManager.TouchElement touchData)
        {
            int touchCount = TouchManager.Instance.TouchsActive;
            if (touchCount < 1)
            {
                isMoving = false;

                if (RotationType == RotationMode.OneFinger)
                    isRotating = false;

            }
            else if (touchCount < 2)
            {
                if (RotationType == RotationMode.TwoFingers || RotationType == RotationMode.Angle)
                    isRotating = false;

                isScaling = false;
            }
        }

        private void SetInitialRotation()
        {
            TouchManager.TouchElement[] touchElements = TouchManager.Instance.TouchElements;

            initialRotation = objectRotation;

            if (RotationType == RotationMode.OneFinger)
            {
                touchRotationY = touchElements[0].position.x;
                touchRotationX = touchElements[0].position.y;
            }
            else if (RotationType == RotationMode.TwoFingers)
            {
                touchRotationY = touchElements[1].position.x;
                touchRotationX = touchElements[1].position.y;
            }
            else
            {
                touchRotationY = Vector2Extensions.AngleBetweenLinear(touchElements[0].position, touchElements[1].position);
                touchRotationX = 0;
            }
        }

        private void SetInitialScale()
        {
            TouchManager.TouchElement[] touchElements = TouchManager.Instance.TouchElements;

            if (selectedObject != null)
                objectScale = selectedObject.GetLocalScale();

            touchScale = Vector2.Distance(touchElements[0].position, touchElements[1].position);
            initialScale = objectScale;
        }

        private void MoveObject()
        {
            if (selectedObject != null)
            {
                objectPosition = selectedObject.GetWorldPosition();

                var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastNonAlloc(cameraRay, rayHits, 100.0f, hitLayerMask);
                if (hits > 0)
                {
                    var nearestHitPoint = default(Vector3);
                    var nearstPointSqrDst = float.MaxValue;

                    for (int i = 0; i < hits; i++)
                    {
                        var sqrDst = Vector3.SqrMagnitude(rayHits[i].point - cameraRay.origin);
                        if (sqrDst < nearstPointSqrDst)
                        {
                            nearestHitPoint = rayHits[i].point;
                            nearstPointSqrDst = sqrDst;
                        }
                    }

                    if (TranslateEnabledAxis.HasFlag(Axis.X))
                        objectPosition.x = nearestHitPoint.x;

                    if (TranslateEnabledAxis.HasFlag(Axis.Y))
                        objectPosition.y = nearestHitPoint.y;

                    if (TranslateEnabledAxis.HasFlag(Axis.Z))
                        objectPosition.z = nearestHitPoint.z;

                    selectedObject.SetWorldPosition(objectPosition, MoveSpeed);
                }
            }
        }

        private void ScaleObject()
        {
            if (selectedObject != null)
            {
                TouchManager.TouchElement[] touchElements = TouchManager.Instance.TouchElements;

                float currentTouchDistance = Vector2.Distance(touchElements[0].position, touchElements[1].position);
                float scale = currentTouchDistance - touchScale;

                scale = scale / scaleSize;
                scale *= ScaleSpeed;

                objectScale.x = initialScale.x + scale;
                objectScale.y = initialScale.y + scale;
                objectScale.z = initialScale.z + scale;

                objectScale.x = Mathf.Clamp(objectScale.x, MinScale, MaxScale);
                objectScale.y = Mathf.Clamp(objectScale.y, MinScale, MaxScale);
                objectScale.z = Mathf.Clamp(objectScale.z, MinScale, MaxScale);

                selectedObject.SetLocalScale(objectScale);
            }
        }

        private void RotateObject()
        {
            if (selectedObject != null)
            {
                TouchManager.TouchElement[] touchElements = TouchManager.Instance.TouchElements;

                float currentAngleY = 0;
                float currentAngleX = 0;

                if (RotationType == RotationMode.OneFinger)
                {
                    currentAngleY = touchElements[0].position.x;
                    currentAngleX = touchElements[0].position.y;
                }
                else if (RotationType == RotationMode.TwoFingers)
                {
                    currentAngleY = touchElements[1].position.x;
                    currentAngleX = touchElements[1].position.y;
                }
                else
                {
                    currentAngleY = Vector2Extensions.AngleBetweenLinear(touchElements[0].position, touchElements[1].position);
                    currentAngleX = 0;
                }

                float angleY = touchRotationY - currentAngleY;
                angleY = (RotationType == RotationMode.TwoFingers || RotationType == RotationMode.OneFinger) ? angleY * 0.4f * RotationSpeed : angleY * RotationSpeed;

                float angleX = touchRotationX - currentAngleX;
                angleX = (RotationType == RotationMode.TwoFingers || RotationType == RotationMode.OneFinger) ? angleX * 0.4f * RotationSpeed : angleX * RotationSpeed;

                if (RotationStep != -1f)
                {
                    angleY = Mathf.Floor(angleY / RotationStep) * RotationStep;
                    angleX = Mathf.Floor(angleX / RotationStep) * RotationStep;
                }

                if (selectedObject.Transformation.RotateEnabledAxis.HasFlag(Axis.Y))
                    objectRotation.y = initialRotation.y + angleY;

                if (selectedObject.Transformation.RotateEnabledAxis.HasFlag(Axis.X))
                    objectRotation.x = initialRotation.x + angleX;

                selectedObject.SetLocalRotation(objectRotation);
            }
        }

        /// <summary>
        /// Registers the callback.
        /// </summary>
        private void RegisterCallbacks()
        {
            TouchManager.Instance.TouchDownEvent += OnTouchDown;
            TouchManager.Instance.TouchUpEvent += OnTouchUp;
            SelectionManager.Instance.OnSelection += OnSelect;
        }

        /// <summary>
        /// Unregisters the callback.
        /// </summary>
        private void UnregisterCallbacks()
        {
            TouchManager.Instance.TouchDownEvent -= OnTouchDown;
            TouchManager.Instance.TouchUpEvent -= OnTouchUp;
            SelectionManager.Instance.OnSelection -= OnSelect;
        }
    }
}
