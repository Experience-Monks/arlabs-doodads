using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Jam3
{
    public class Bounds
    {
        #region Exposed fields

        public readonly Renderer renderer;
        public readonly Transform pivot;

        private Vector3 localCenter = Vector3.zero;
        private Vector3 worldCenter = Vector3.zero;
        private Vector3 extents = Vector3.zero;
        private Vector3 orientedExtents = Vector3.zero;
        private Vector3 size = Vector3.zero;

        // Runtime
        private Matrix4x4 tMatrix;
        private Matrix4x4 rMatrix;
        private Matrix4x4 sMatrix;
        private Matrix4x4 rsMatrix;
        private Matrix4x4 trsMatrix;
        private Matrix4x4 iTrsMatrix;

        #endregion Exposed fields

        #region Non Exposed fields

        private Vector3 initialSize = Vector3.zero;
        private Vector3 initialExtents = Vector3.zero;
        private Vector3 initialScale = Vector3.one;
        private Quaternion initialRotation = Quaternion.identity;

        #endregion Non Exposed fields

        #region Properties

        /// <summary>
        /// Gets the local center.
        /// </summary>
        /// <value>
        /// The local center.
        /// </value>
        public Vector3 LocalCenter
        {
            get => localCenter;
            private set => localCenter = value;
        }

        /// <summary>
        /// Gets or sets the center in world space.
        /// </summary>
        /// <value>
        /// The center.
        /// </value>
        public Vector3 WorldCenter
        {
            get => worldCenter;
            private set => worldCenter = value;
        }

        /// <summary>
        /// Gets the extents.
        /// </summary>
        /// <value>
        /// The extents.
        /// </value>
        public Vector3 Extents
        {
            get => extents;
            private set => extents = value;
        }

        /// <summary>
        /// Gets the oriented extents.
        /// </summary>
        /// <value>
        /// The extents.
        /// </value>
        public Vector3 OrientedExtents
        {
            get => orientedExtents;
            private set => orientedExtents = value;
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Vector3 Size
        {
            get => size;
            private set => size = value;
        }


        /// <summary>
        /// Gets the translation matrix.
        /// </summary>
        /// <value>
        /// The t matrix.
        /// </value>
        ///
        public Matrix4x4 TMatrix => tMatrix;

        /// <summary>
        /// Gets the rotation matrix.
        /// </summary>
        /// <value>
        /// The r matrix.
        /// </value>
        public Matrix4x4 RMatrix => rMatrix;

        /// <summary>
        /// Gets the scale matrix.
        /// </summary>
        /// <value>
        /// The s matrix.
        /// </value>
        public Matrix4x4 SMatrix => sMatrix;

        /// <summary>
        /// Gets the rotation-scale matrix.
        /// </summary>
        /// <value>
        /// The rs matrix.
        /// </value>
        public Matrix4x4 RsMatrix => rsMatrix;

        /// <summary>
        /// Gets the TRS matrix.
        /// </summary>
        /// <value>
        /// The TRS matrix.
        /// </value>
        public Matrix4x4 TrsMatrix => trsMatrix;

        /// <summary>
        /// Gets the inversed TRS matrix.
        /// </summary>
        /// <value>
        /// The i TRS matrix.
        /// </value>
        public Matrix4x4 ITrsMatrix => iTrsMatrix;


        /// <summary>
        /// Gets the translation.
        /// </summary>
        /// <value>
        /// The translation.
        /// </value>
        public Vector3 Translation => pivot.position;

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public Quaternion Rotation => pivot.rotation;

        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public Vector3 Scale => pivot.lossyScale;

        #endregion Properties

        #region Custom Events

        #endregion Custom Events

        #region Events methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Bounds"/> class.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public Bounds(Renderer renderer, Transform pivot)
        {
            this.renderer = renderer;
            this.pivot = pivot;

            Initialize();
        }

        #endregion Events methods

        #region Public Methods

        /// <summary>
        /// Updates the bounds.
        /// </summary>
        public void UpdateBounds()
        {
            if (renderer != null)
            {
                tMatrix = Matrix4x4.Translate(pivot.position);
                rMatrix = Matrix4x4.Rotate(pivot.rotation);
                sMatrix = Matrix4x4.Scale(pivot.lossyScale);
                rsMatrix = rMatrix * sMatrix;
                trsMatrix = tMatrix * rMatrix * sMatrix;
                iTrsMatrix = trsMatrix.inverse;

                WorldCenter = renderer.bounds.center;
                LocalCenter = iTrsMatrix.MultiplyPoint3x4(WorldCenter);

                Extents = sMatrix.MultiplyPoint3x4(initialExtents);
                OrientedExtents = rsMatrix.MultiplyPoint3x4(initialExtents);

                Size = sMatrix.MultiplyPoint3x4(initialSize);
            }
        }

        #endregion Public Methods

        #region Non Public Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            if (renderer != null)
            {
                // Cache initial values
                var inversedRotation = Quaternion.Inverse(pivot.rotation);
                initialExtents = inversedRotation * renderer.bounds.extents;
                initialSize = inversedRotation * renderer.bounds.size;

                // Manual update
                UpdateBounds();
            }
        }

        #endregion Non Public Methods
    }
}
