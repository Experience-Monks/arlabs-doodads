//-----------------------------------------------------------------------
// <copyright file="SpringController.cs" company="Jam3 Inc">
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
using DG.Tweening;

namespace Jam3
{

    /// <summary>
    /// Spring controller.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    [RequireComponent(typeof(BoxCollider))]
    public class SpringController : MonoBehaviour
    {
        public bool UseSpringAsset = true;

        [Header("Ground Refference")]
        public GameObject GroundObject = null;

        [Header("Target")]
        public Collider TargetCollider = null;

        [Header("Spring Components")]
        public GameObject SpringTop = null;
        public GameObject SpringBottom = null;

        [Header("Spring Force")]
        public float SpringForce = 200.0f;

        [Header("Spring Animation")]
        [Range(0.0f, 2f)]
        public float SpringOffset = 0.0f;

        [Range(-2, 0f)]
        public float SpringMinOffset = -0.04f;
        [Range(0.0f, 2f)]
        public float SpringMaxOffset = 0.5f;

        [Range(0f, 2f)]
        public float SpringHoldStep = 1f;

        [Range(0f, 10f)]
        public float SpringHoldSpeed = 3f;

        [Range(0.01f, 5f)]
        public float ReleaseTime = 1.0f;

        [Range(0.01f, 5f)]
        public float ReturnTime = 0.5f;

        [Header("Debug")]
        [Range(0.0f, 1f)]
        public float SpringTension = 0.0f;

        // Runtime varilables
        private BoxCollider springCollider = null;
        private float topIniPositionX = 0.0f;

        private Vector3 objectSize = Vector3.zero;
        private Vector3 objectCenter = Vector3.zero;

        private Vector3 springTopPosition = Vector3.zero;

        private bool holding = false;
        private bool animating = false;

        private bool isColliding = false;
        private float springTension = 0.0f;

        private GameObject ballObject = null;

        /// <summary>
        /// Gets if the spring is holding.
        /// </summary>
        public bool IsHolding { get => holding; }

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            springCollider = GetComponent<BoxCollider>();

            if (UseSpringAsset && SpringTop != null)
            {
                springTopPosition = SpringTop.transform.localPosition;
                springTopPosition.x += -SpringOffset;
                topIniPositionX = springTopPosition.x;

                SpringTop.transform.localPosition = springTopPosition;
            }

            gameObject.layer = 0;

            animating = false;
            holding = false;
            SetColliderSize();
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            if (holding)
            {
                SpringTension += SpringHoldStep * Time.deltaTime;
                SpringTension = SpringTension > 1.0f ? 1.0f : SpringTension;

                if (UseSpringAsset)
                    springTopPosition.x = Mathf.Lerp(springTopPosition.x, topIniPositionX + (-SpringMinOffset * SpringTension), Time.deltaTime * SpringHoldSpeed);
            }

            if (UseSpringAsset)
            {
                SpringTop.transform.localPosition = springTopPosition;
                SetColliderSize();
            }
        }

        /// <summary>
        /// Lates update.
        /// </summary>
        private void LateUpdate()
        {
            if (UseSpringAsset)
            {
                if (isColliding && ballObject != null)
                {
                    Vector3 offset = (-gameObject.transform.right * 0.08f);
                    ballObject.transform.position = SpringTop.transform.position + offset;
                }
            }
        }

        /// <summary>
        /// Ons trigger enter.
        /// </summary>
        /// <param name="other">The other.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (UseSpringAsset)
            {
                if (other.gameObject.tag == "Player")
                {
                    Vector3 forceAdd = (-gameObject.transform.right * (SpringForce * springTension));

                    other.attachedRigidbody.isKinematic = false;
                    other.attachedRigidbody.WakeUp();
                    other.attachedRigidbody.AddForce(forceAdd);

                    ballObject = other.gameObject;

                    isColliding = true;
                }
            }
        }

        /// <summary>
        /// Ons trigger exit.
        /// </summary>
        /// <param name="other">The other.</param>
        private void OnTriggerExit(Collider other)
        {
            if (UseSpringAsset)
            {
                if (isColliding && other.gameObject.tag == "Player")
                {
                    isColliding = false;
                    ballObject = null;
                }
            }
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public void Reset()
        {
            springTension = 0.0f;
            holding = false;
            animating = false;

            if (UseSpringAsset)
            {
                isColliding = false;

                DOTween.Kill("spring_release");
                DOTween.Kill("spring_return");

                springTopPosition.x = topIniPositionX;
            }
        }

        /// <summary>
        /// Hold.
        /// </summary>
        public void Hold()
        {
            if (!holding)
            {
                springTension = 0.0f;
                holding = true;
            }
        }

        /// <summary>
        /// Release.
        /// </summary>
        public void Release()
        {
            holding = false;
            springTension = SpringTension;

            if (UseSpringAsset)
                AnimateRelease();
            else
                SetTargetForce(TargetCollider);

            SpringTension = 0.0f;
        }

        /// <summary>
        /// Animates release.
        /// </summary>
        public void AnimateRelease()
        {
            if (!animating)
            {
                float finalValue = topIniPositionX + (-SpringMaxOffset * SpringTension);
                animating = true;
                float currentX = springTopPosition.x;
                DOTween.To(() => currentX, x => currentX = x, finalValue, ReleaseTime).SetId("spring_release").SetEase(Ease.OutElastic).OnUpdate(() => {
                    float dist = Vector3.Distance(GroundObject.transform.position, SpringTop.transform.position);
                    if (dist > Mathf.Abs(-gameObject.transform.right.y * 0.16f))
                        springTopPosition.x = currentX;
                }).OnComplete(() => {
                    DOTween.To(() => springTopPosition.x, x => springTopPosition.x = x, topIniPositionX, ReturnTime).SetId("spring_return").SetEase(Ease.OutElastic, 1.3f, 1.2f).OnComplete(() => {
                        animating = false;
                        springTension = 0.0f;
                    });
                });
            }
        }

        /// <summary>
        /// Sets target force.
        /// </summary>
        /// <param name="target">The target.</param>
        private void SetTargetForce(Collider target)
        {
            if (target != null)
            {
                Vector3 forceAdd = (-gameObject.transform.right * (SpringForce * springTension));

                target.attachedRigidbody.isKinematic = false;
                target.attachedRigidbody.WakeUp();
                target.attachedRigidbody.AddForce(forceAdd);
            }
        }

        /// <summary>
        /// Sets collider size.
        /// </summary>
        private void SetColliderSize()
        {
            if (UseSpringAsset && SpringTop != null && SpringBottom != null)
            {
                objectSize = SpringTop.transform.localPosition - SpringBottom.transform.localPosition;
                objectSize.y = 0.12f;
                objectSize.z = 0.12f;

                objectCenter.x = objectSize.x / 2;

                springCollider.center = objectCenter;
                springCollider.size = objectSize;
            }
        }
    }
}


