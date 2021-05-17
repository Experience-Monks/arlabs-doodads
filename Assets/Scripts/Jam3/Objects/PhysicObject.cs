using UnityEngine;
using DG.Tweening;

namespace Jam3
{
    [RequireComponent(typeof(ARObject))]
    public class PhysicObject : MonoBehaviour
    {
        #region Exposed fields

        [Header("Components")]

        [SerializeField]
        public Rigidbody objectRigidbody = null;

        [SerializeField]
        private SpringController springController = null;

        [SerializeField]
        private AudioController audioController = null;

        [Header("Action Trigger")]

        [SerializeField]
        private GameObject actionTrigger;

        [SerializeField]
        private Vector3[] actionTriggerDirections;

        [SerializeField]
        private float timeToFinish;

        [SerializeField]
        private float velocityThreshold;

        // Runtime assigned
        private float maxSpeed;
        private float traveledDistance;

        #endregion Exposed fields

        #region Non Exposed fields

        private ARObject cachedArObjectComponent;

        private bool moving = false;
        private bool playing = false;

        private Vector3 initialPosition = Vector3.zero;
        private Vector3 lastPosition = Vector3.zero;

        private int currentDirectionId = 0;
        private float finishTimeCount = 0.0f;

        #endregion Non Exposed fields

        #region Properties

        /// <summary>
        /// Gets or sets the base.
        /// </summary>
        /// <value>
        /// The base.
        /// </value>
        public ARObject ARBase =>
            cachedArObjectComponent;

        /// <summary>
        /// Gets or sets the object rigidbody.
        /// </summary>
        /// <value>
        /// The object rigidbody.
        /// </value>
        public Rigidbody ObjectRigidbody
        {
            get => objectRigidbody;
            set => objectRigidbody = value;
        }

        /// <summary>
        /// Gets or sets the audio controller.
        /// </summary>
        /// <value>
        /// The audio controller.
        /// </value>
        public AudioController AudioController
        {
            get => audioController;
            set => audioController = value;
        }

        /// <summary>
        /// Gets or sets the spring controller.
        /// </summary>
        /// <value>
        /// The spring controller.
        /// </value>
        public SpringController SpringController
        {
            get => springController;
            set => springController = value;
        }


        /// <summary>
        /// Gets or sets the action trigger.
        /// </summary>
        /// <value>
        /// The action trigger.
        /// </value>
        public GameObject ActionTrigger
        {
            get => actionTrigger;
            set => actionTrigger = value;
        }

        /// <summary>
        /// Gets or sets the action trigger directions.
        /// </summary>
        /// <value>
        /// The action trigger directions.
        /// </value>
        public Vector3[] ActionTriggerDirections
        {
            get => actionTriggerDirections;
            set => actionTriggerDirections = value;
        }

        /// <summary>
        /// Gets or sets the traveled distance.
        /// </summary>
        /// <value>
        /// The traveled distance.
        /// </value>
        public float TraveledDistance
        {
            get => traveledDistance;
            private set => traveledDistance = value;
        }

        /// <summary>
        /// Gets or sets the maximum speed.
        /// </summary>
        /// <value>
        /// The maximum speed.
        /// </value>
        public float MaxSpeed
        {
            get => maxSpeed;
            private set => maxSpeed = value;
        }

        /// <summary>
        /// Gets or sets the fime To finish after the velocity is small than the threshold.
        /// </summary>
        /// <value>
        /// The time to finish.
        /// </value>
        public float TimeToFinish
        {
            get => timeToFinish;
            private set => timeToFinish = value;
        }

        /// <summary>
        /// Gets or sets the velocity threshold.
        /// </summary>
        /// <value>
        /// The velocity threshold.
        /// </value>
        public float VelocityThreshold
        {
            get => velocityThreshold;
            private set => velocityThreshold = value;
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
            // Get required components
            cachedArObjectComponent = GetComponent<ARObject>();

            finishTimeCount = 0;

            // Cache initial values
            if (ObjectRigidbody != null)
                initialPosition = ObjectRigidbody.gameObject.transform.localPosition;

            // Events registration
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
        /// Called after the regular udpate.
        /// </summary>
        private void LateUpdate()
        {
            bool canFinish = false;
            if (playing)
            {
                if (ObjectRigidbody != null && ObjectRigidbody.isKinematic == false)
                {
                    TraveledDistance += Vector3.Distance(ObjectRigidbody.transform.position, lastPosition);
                    lastPosition = ObjectRigidbody.transform.position;

                    MaxSpeed = ObjectRigidbody.velocity.magnitude > MaxSpeed ? ObjectRigidbody.velocity.magnitude : MaxSpeed;

                    if (ObjectRigidbody.velocity.magnitude > 0.0)
                        moving = true;

                    if (ObjectRigidbody.velocity.magnitude < VelocityThreshold)
                    {
                        finishTimeCount += Time.deltaTime;
                        if (finishTimeCount >= TimeToFinish)
                        {
                            canFinish = true;
                            finishTimeCount = 0;
                        }
                    }

                    if (AudioController != null)
                        AudioController.AudioAmount = ObjectRigidbody.velocity.magnitude;

                    if (canFinish || ObjectRigidbody.IsSleeping())
                    {
                        playing = false;
                        moving = false;
                        GameManager.Instance.GameOver(true);
                    }
                }
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            ResetRigidBody();
            ResetActionTrigger();

            if (SpringController != null)
                SpringController.Reset();
        }


        /// <summary>
        /// Called when the object is [placed].
        /// </summary>
        private void OnPlaced()
        {
            ShowActionTrigger();
        }

        /// <summary>
        /// Called when this object is selected.
        /// </summary>
        private void OnSelected()
        {
            if (ObjectRigidbody != null)
                ObjectRigidbody.isKinematic = true;

            ShowActionTrigger();
        }

        /// <summary>
        /// Called when this object is deselected.
        /// </summary>
        private void OnDeselected()
        {
        }

        /// <summary>
        /// Called when [position is set].
        /// </summary>
        /// <param name="position">The position.</param>
        private void OnPositionSet(Vector3 position)
        {
            ObjectManager.Instance.UpdateBallInitialPosition(position);
        }

        /// <summary>
        /// Called when [rotation is set].
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        private void OnRotationSet(Vector3 rotation)
        {
        }

        /// <summary>
        /// Called when [scale is set].
        /// </summary>
        /// <param name="scale">The scale.</param>
        private void OnScaleSet(Vector3 scale)
        {
        }

        #endregion Events methods

        #region Public Methods

        /// <summary>
        /// Replays this instance.
        /// </summary>
        public void Replay()
        {
            finishTimeCount = 0;
            ResetRigidBody();

            if (SpringController != null)
                SpringController.Reset();
        }

        /// <summary>
        /// Resets the rigid body.
        /// </summary>
        public void ResetRigidBody()
        {
            moving = false;
            playing = false;

            if (ObjectRigidbody != null)
            {
                ObjectRigidbody.isKinematic = true;
                ObjectRigidbody.gameObject.transform.localPosition = initialPosition;
                ObjectRigidbody.gameObject.transform.localEulerAngles = Vector3.zero;
            }
        }

        /// <summary>
        /// Resets the action trigger.
        /// </summary>
        public void ResetActionTrigger()
        {
            ActionTrigger.SetActive(false);

            // currentDirectionId = -1;
            // ChangeActionTriggerDirection();
        }

        /// <summary>
        /// Shows the action trigger.
        /// </summary>
        public void ShowActionTrigger()
        {
            ActionTrigger.SetActive(true);
        }

        /// <summary>
        /// Changes the action trigger direction.
        /// </summary>
        public void ChangeActionTriggerDirection()
        {
            if (currentDirectionId < ActionTriggerDirections.Length - 1)
            {
                currentDirectionId++;
                Vector3 currentRotation = ActionTriggerDirections[currentDirectionId];
                ActionTrigger.transform.DOLocalRotate(currentRotation, 0.15f);
            }
            else
            {
                currentDirectionId = -1;
                ChangeActionTriggerDirection();
            }

        }

        /// <summary>
        /// Holds the spring to add force tot he trigger.
        /// </summary>
        public void HoldSpring()
        {
            if (SpringController != null)
                SpringController.Hold();
        }

        /// <summary>
        /// Releases the spring, starting the physic gameflow (No froce added).
        /// </summary>
        public void ReleaseSpring()
        {
            if (ObjectRigidbody != null)
            {
                // Start the object counters/flags
                StartObject();
            }

            if (SpringController != null)
                SpringController.Release();
        }

        /// <summary>
        /// Drops the object, starting the physic gameflow (No froce added).
        /// </summary>
        public void DropObject()
        {
            if (ObjectRigidbody != null)
            {
                // Start the object counters/flags
                StartObject();

                // Set Physics Kinematics (Gravity)
                ObjectRigidbody.isKinematic = false;
            }
        }

        #endregion Public Methods

        #region Non Public Methods

        private void StartObject()
        {
            // Reset counters
            TraveledDistance = 0f;
            MaxSpeed = 0f;

            // Cache starting position
            lastPosition = ObjectRigidbody.transform.position;
            ObjectRigidbody.isKinematic = false;

            // Set flag
            playing = true;
        }

        /// <summary>
        /// Registers the callback.
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
        /// Registers the callback.
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

        #endregion Non Public Methods
    }
}
