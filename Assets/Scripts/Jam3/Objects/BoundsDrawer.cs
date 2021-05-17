//-----------------------------------------------------------------------
// <copyright file="BoundsDrawer.cs" company="Jam3 Inc">
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
using UnityEngine.Rendering;
using TMPro;
using DG.Tweening;

using Jam3.Util;

namespace Jam3
{
    /// <summary>
    /// Responsible for drawing the bounds of an ARObject.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="Jam3.ARObject" />
    [RequireComponent(typeof(ARObject))]
    public class BoundsDrawer : MonoBehaviour
    {
        #region Exposed fields

        [Header("Materials")]
        public Material LineMaterial = null;

        [Header("Labels")]
        public Transform LabelContainer = null;
        public Transform WiresContainer = null;
        public TMP_Text LabelWidth = null;
        public TMP_Text LabelHeight = null;
        public TMP_Text LabelDepth = null;
        public float TextLineWidth = 0;
        public float TextOffset = 0.1f;

        [Header("Lines")]
        public float LineWidth = 0.01f;
        public float LineOffset = 0.01f;

        #endregion Exposed fields

        #region Non Exposed fields

        private ARObject cachedArObjectComponent;

        private Material instancedLineMaterial = null;
        private LineRenderer[] wireRenderer = new LineRenderer[4];

        private Vector3 withPosition = Vector3.zero;
        private Vector3 heightPosition = Vector3.zero;
        private Vector3 depthPosition = Vector3.zero;

        private Camera mainCamera = null;

        #endregion Non Exposed fields

        #region Properties

        /// <summary>
        /// Gets the base AR component.
        /// </summary>
        /// <value>
        /// The base.
        /// </value>
        public ARObject ARBase =>
            cachedArObjectComponent;

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

            // Get the main camera
            mainCamera = Camera.main;

            // Setup labels
            SetupLabels();

            // Create lines
            CreateLineMaterial();
            CreateLineRenderers();

            // Manual update
            UpdateLabelsPosition();
            UpdateWiresPosition();

            // Register events callbacks
            RegisterCallbacks();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            //Set the label orientation look at the camera
            UpdateLabelsOrientation();
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterCallbacks();
        }


        /// <summary>
        /// Called when this object is selected.
        /// </summary>
        private void OnSelected()
        {
            AnimateIn(0.3f);
        }

        /// <summary>
        /// Called when this object is deselected.
        /// </summary>
        private void OnDeselected()
        {
            AnimateOut(0.15f);
        }

        /// <summary>
        /// Called when [position is set].
        /// </summary>
        /// <param name="position">The position.</param>
        private void OnPositionSet(Vector3 position)
        {
            UpdateLabelsPosition();
            UpdateWiresPosition();
        }

        /// <summary>
        /// Called when [rotation is set].
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        private void OnRotationSet(Vector3 rotation)
        {
            UpdateLabelsPosition();
            UpdateWiresPosition();
        }

        /// <summary>
        /// Called when [scale is set].
        /// </summary>
        /// <param name="scale">The scale.</param>
        private void OnScaleSet(Vector3 scale)
        {
            UpdateLabelsPosition();
            UpdateWiresPosition();
        }

        #endregion Events methods

        #region Public Methods

        /// <summary>
        /// Animates the in.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="delay">The delay.</param>
        public void AnimateIn(float time = 0.5f, float delay = 0.0f)
        {
            if (instancedLineMaterial != null)
            {
                for (int i = 0; i < wireRenderer.Length; i++)
                {
                    wireRenderer[i].enabled = true;
                }

                instancedLineMaterial.DOKill();
                instancedLineMaterial.DOFloat(1.0f, "_Alpha", time).SetEase(Ease.InOutSine).SetDelay(delay);

                LabelWidth.DOFade(1.0f, time).SetEase(Ease.InOutSine).SetDelay(delay);
                LabelHeight.DOFade(1.0f, time).SetEase(Ease.InOutSine).SetDelay(delay);
                LabelDepth.DOFade(1.0f, time).SetEase(Ease.InOutSine).SetDelay(delay);
            }
        }

        /// <summary>
        /// Animates the out.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="delay">The delay.</param>
        public void AnimateOut(float time = 0.5f, float delay = 0.0f)
        {
            if (instancedLineMaterial != null)
            {
                instancedLineMaterial.DOKill();
                instancedLineMaterial.DOFloat(0.0f, "_Alpha", time).SetEase(Ease.InOutSine).SetDelay(delay).OnComplete(() =>
                {
                    for (int i = 0; i < wireRenderer.Length; i++)
                    {
                        wireRenderer[i].enabled = false;
                    }
                });

                LabelWidth.DOFade(0.0f, time).SetEase(Ease.InOutSine).SetDelay(delay);
                LabelHeight.DOFade(0.0f, time).SetEase(Ease.InOutSine).SetDelay(delay);
                LabelDepth.DOFade(0.0f, time).SetEase(Ease.InOutSine).SetDelay(delay);
            }
        }

        /// <summary>
        /// Updates the labels position.
        /// </summary>
        public void UpdateLabelsOrientation()
        {
            SetLabelLocalXScale(LabelHeight);
            SetLabelLocalXScale(LabelWidth);
            SetLabelLocalXScale(LabelDepth);
        }

        /// <summary>
        /// Updates the labels position.
        /// </summary>
        public void UpdateLabelsPosition()
        {
            if (ARBase != null && ARBase.Bounds != null)
            {
                LabelContainer.transform.position = ARBase.Bounds.WorldCenter;
                LabelContainer.transform.rotation = ARBase.Bounds.Rotation;

                var size = ARBase.Bounds.Size;
                var extents = ARBase.Bounds.Extents;

                if (LabelWidth != null)
                {
                    LabelWidth.text = UnitUtil.UnitToInches(size.x);
                    withPosition.y = extents.y + TextOffset;
                    withPosition.z = -extents.z;
                    LabelWidth.transform.localPosition = withPosition;
                }

                if (LabelHeight != null)
                {
                    LabelHeight.text = UnitUtil.UnitToInches(size.y);
                    heightPosition.x = -extents.x - TextOffset;
                    heightPosition.z = -extents.z;
                    LabelHeight.transform.localPosition = heightPosition;
                }

                if (LabelDepth != null)
                {
                    LabelDepth.text = UnitUtil.UnitToInches(size.z);
                    depthPosition.x = extents.x;
                    depthPosition.y = extents.y + TextOffset;
                    LabelDepth.transform.localPosition = depthPosition;
                }
            }
        }

        /// <summary>
        /// Updates the wires position.
        /// </summary>
        public void UpdateWiresPosition()
        {
            if (ARBase != null && ARBase.Bounds != null)
            {
                WiresContainer.transform.position = ARBase.Bounds.WorldCenter;
                WiresContainer.transform.rotation = ARBase.Bounds.Rotation;

                var offset = Vector3.one * LineOffset;
                var localMin = -ARBase.Bounds.Extents - offset;
                var localMax = ARBase.Bounds.Extents + offset;

                //Face 0
                wireRenderer[0].SetPosition(0, new Vector3(localMax.x, localMax.y, localMax.z));
                wireRenderer[0].SetPosition(1, new Vector3(localMax.x, localMin.y, localMax.z));
                wireRenderer[0].SetPosition(2, new Vector3(localMin.x, localMin.y, localMax.z));
                wireRenderer[0].SetPosition(3, new Vector3(localMin.x, localMax.y, localMax.z));

                //Face 1
                wireRenderer[1].SetPosition(0, new Vector3(localMax.x, localMax.y, localMin.z));
                wireRenderer[1].SetPosition(1, new Vector3(localMax.x, localMin.y, localMin.z));
                wireRenderer[1].SetPosition(2, new Vector3(localMin.x, localMin.y, localMin.z));
                wireRenderer[1].SetPosition(3, new Vector3(localMin.x, localMax.y, localMin.z));

                //Face 2
                wireRenderer[2].SetPosition(0, new Vector3(localMax.x, localMax.y, localMax.z));
                wireRenderer[2].SetPosition(1, new Vector3(localMax.x, localMin.y, localMax.z));
                wireRenderer[2].SetPosition(2, new Vector3(localMax.x, localMin.y, localMin.z));
                wireRenderer[2].SetPosition(3, new Vector3(localMax.x, localMax.y, localMin.z));

                //Face 3
                wireRenderer[3].SetPosition(0, new Vector3(localMin.x, localMax.y, localMax.z));
                wireRenderer[3].SetPosition(1, new Vector3(localMin.x, localMin.y, localMax.z));
                wireRenderer[3].SetPosition(2, new Vector3(localMin.x, localMin.y, localMin.z));
                wireRenderer[3].SetPosition(3, new Vector3(localMin.x, localMax.y, localMin.z));
            }
        }

        #endregion Public Methods

        #region Non Public Methods

        /// <summary>
        /// Registers the callback.
        /// </summary>
        private void RegisterCallbacks()
        {
            ARBase.SelectedEvent += OnSelected;
            ARBase.DeselectedEvent += OnDeselected;
            ARBase.PositionSetEvent += OnPositionSet;
            ARBase.RotationSetEvent += OnRotationSet;
            ARBase.ScaleSetEvent += OnScaleSet;
        }

        /// <summary>
        /// Unregisters the callback.
        /// </summary>
        private void UnregisterCallbacks()
        {
            ARBase.SelectedEvent -= OnSelected;
            ARBase.DeselectedEvent -= OnDeselected;
            ARBase.PositionSetEvent -= OnPositionSet;
            ARBase.RotationSetEvent -= OnRotationSet;
            ARBase.ScaleSetEvent -= OnScaleSet;
        }


        /// <summary>
        /// Sets the label local x scale.
        /// </summary>
        /// <param name="label">The label.</param>
        private void SetLabelLocalXScale(TMP_Text label)
        {
            if (label != null)
            {
                float value = Vector3.Dot(mainCamera.transform.forward, label.transform.forward);
                value = value < 0f ? -1 : 1;

                Vector3 scale = label.transform.localScale;
                scale.x = value;
                label.transform.localScale = scale;
            }
        }


        /// <summary>
        /// Setups the labels.
        /// </summary>
        private void SetupLabels()
        {
            // Ensure label container
            if (LabelContainer == null)
            {
                LabelContainer = new GameObject("Labels").transform;
                LabelContainer.SetParent(ARBase.transform, false);
            }

            // Initial setup of the labels
            if (LabelWidth != null)
            {
                LabelWidth.transform.SetParent(LabelContainer);
                LabelWidth.outlineWidth = TextLineWidth;
                LabelWidth.DOFade(0.0f, 0.0f);
            }

            if (LabelHeight != null)
            {
                LabelHeight.transform.SetParent(LabelContainer);
                LabelHeight.outlineWidth = TextLineWidth;
                LabelHeight.DOFade(0.0f, 0.0f);
            }

            if (LabelDepth != null)
            {
                LabelDepth.transform.SetParent(LabelContainer);
                LabelDepth.outlineWidth = TextLineWidth;
                LabelDepth.DOFade(0.0f, 0.0f);
            }
        }

        /// <summary>
        /// Creates the line material.
        /// </summary>
        private void CreateLineMaterial()
        {
            if (LineMaterial != null)
            {
                // Create line renderers material
                instancedLineMaterial = new Material(LineMaterial);
                instancedLineMaterial.SetFloat("_Alpha", 0.0f);
            }
        }

        /// <summary>
        /// Creates the line renderers.
        /// </summary>
        /// <param name="material">The material.</param>
        private void CreateLineRenderers()
        {
            // Ensure wires container
            if (WiresContainer == null)
            {
                WiresContainer = new GameObject("Wires").transform;
                WiresContainer.SetParent(ARBase.transform, false);
            }

            // Create each wire
            for (int i = 0; i < wireRenderer.Length; i++)
            {
                var go = new GameObject();
                go.name = "Bounding_" + i;
                go.transform.SetParent(WiresContainer, false);

                wireRenderer[i] = go.AddComponent<LineRenderer>();
                wireRenderer[i].material = instancedLineMaterial;
                wireRenderer[i].widthMultiplier = LineWidth * ObjectManager.Instance.LineWidthMultiplier;
                wireRenderer[i].positionCount = 4;
                wireRenderer[i].numCapVertices = 2; //lengthOfLineRenderer;
                wireRenderer[i].numCornerVertices = 2; //lengthOfLineRenderer;
                wireRenderer[i].loop = true;
                wireRenderer[i].receiveShadows = false;
                wireRenderer[i].shadowCastingMode = ShadowCastingMode.Off;
                wireRenderer[i].useWorldSpace = false;
            }
        }

        #endregion Non Public Methods
    }
}
