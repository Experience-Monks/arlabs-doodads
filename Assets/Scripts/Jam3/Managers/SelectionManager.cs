using System;
using UnityEngine;
using Jam3.Util;

namespace Jam3
{
    public class SelectionManager : Singleton<SelectionManager>
    {
        #region Exposed fields

        [SerializeField]
        private MenuControllerUI menuControllerUI;

        [SerializeField]
        private Layers objectLayer = Layers.Draggable;

        [SerializeField]
        private Layers objectLayerSecundary = Layers.DraggableAction;

        // Runtime
        private ARObject selectedObject = null;

        #endregion Exposed fields

        #region Non Exposed fields

        private float lastTapTime;

        private int objectLayerMask = -1;
        private int objectLayerSecondaryMask = -1;

        #endregion Non Exposed fields

        #region Properties

        public MenuControllerUI MenuControllerUI
        {
            get => menuControllerUI;
            set => menuControllerUI = value;
        }

        public Layers ObjectLayer
        {
            get => objectLayer;
            set => objectLayer = value;
        }

        public Layers ObjectLayerSecundary
        {
            get => objectLayerSecundary;
            set => objectLayerSecundary = value;
        }

        public ARObject SelectedObject
        {
            get => selectedObject;
            private set => selectedObject = value;
        }

        #endregion Properties

        #region Custom Events

        public delegate void SelectionEvent(bool isSelecting, ARObject selectedObject);
        public event SelectionEvent OnSelection;

        #endregion Custom Events

        #region Events methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            // Layers
            objectLayerMask = 1 << (int)ObjectLayer;
            objectLayerSecondaryMask = 1 << (int)ObjectLayerSecundary;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            bool tapEnabled = GameManager.Instance.TapEnabled;
            var shouldSelect = false;
            var doubleTap = TouchManager.Instance.DoubleTap;

            if (tapEnabled && Input.GetMouseButtonDown((int)MouseButtons.Left))
                shouldSelect = true;

            if (SelectedObject == null && shouldSelect)
            {
                var targetObj = GetObjectHit();
                if (targetObj != null)
                    SelectObject(targetObj);
            }
        }

        #endregion Events methods

        #region Public Methods

        #endregion Public Methods

        #region Non Public Methods

        #endregion Non Public Methods


        public void SelectionActive(bool isSelecting)
        {
            OnSelection?.Invoke(isSelecting, SelectedObject);
        }

        /// <summary>
        /// Gets the game object from the selected AR object.
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            if (selectedObject != null)
            {
                return selectedObject.gameObject;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Destroys the selected object.
        /// </summary>
        public void DestroyObject()
        {
            SelectionActive(false);

            if (SelectedObject != null)
            {
                Destroy(SelectedObject.gameObject);
                SelectedObject = null;
            }
        }

        /// <summary>
        /// Selects the given object.
        /// </summary>
        /// <param name="arObject">The selected AR object.</param>
        /// <param name="setMenu">if set to <c>true</c> menu interaction will be enabled.</param>
        public void SelectObject(ARObject arObject = null, bool setMenu = true)
        {
            if (!GameManager.Instance.IsPlaying)
            {
                // Cache selected object
                SelectedObject = arObject;

                // Event trigger
                SelectionActive(true);

                // UI
                //TODO: Should be invoked via event callback
                if (SelectedObject != null)
                {
                    if (setMenu && MenuControllerUI != null)
                        MenuControllerUI.SetMenuTransform(SelectedObject.ID);
                }
            }
        }

        /// <summary>
        /// Uns the select object.
        /// </summary>
        public void UnselectObject()
        {
            SelectionActive(false);
            SelectedObject = null;
        }

        /// <summary>
        /// Gets the object hit.
        /// </summary>
        /// <returns></returns>
        public ARObject GetObjectHit()
        {
            ARObject hitobj = null;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitObject, 100.0f, objectLayerMask | objectLayerSecondaryMask))
            {
                hitobj = hitObject.collider.gameObject.GetComponent<ARObject>();
                if (hitobj == null)
                {
                    var collisionObject = hitObject.collider.gameObject.GetComponent<CollisionDetect>();
                    if (collisionObject != null)
                        hitobj = collisionObject.Controller;
                }
            }
            return hitobj;
        }
    }
}
