//-----------------------------------------------------------------------
// <copyright file="CustomizableObject.cs" company="Jam3 Inc">
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

namespace Jam3
{
    /// <summary>
    /// Allows an ARObject to be customized by changing its material properties.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="Jam3.ARObject" />
    [RequireComponent(typeof(ARObject))]
    public class CustomizableObject : MonoBehaviour
    {
        #region Exposed fields

        #endregion Exposed fields

        #region Non Exposed fields

        private ARObject cachedArObjectComponent;

        private int colorId = -1;
        private Material objectMaterial;

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
        /// Gets the color identifier of the current selected texture.
        /// </summary>
        /// <value>
        /// The color identifier.
        /// </value>
        public int ColorId
        {
            get => colorId;
            private set => colorId = value;
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

            // Instantiate a material
            if (ARBase.Renderer != null)
            {
                objectMaterial = new Material(ARBase.Renderer.material);
                ARBase.Renderer.material = objectMaterial;
            }
        }

        #endregion Events methods

        #region Public Methods

        /// <summary>
        /// Changes the texture using the index of the texture inside the array of possible textures.
        /// </summary>
        /// <param name="texId">The tex identifier.</param>
        public void ChangeTexture(int texId)
        {
            var objId = ARBase.ID;

            if (objectMaterial != null && objId > -1)
            {
                if (objId < ObjectManager.Instance.InteractiveElements.Length)
                {
                    int items = ObjectManager.Instance.InteractiveElements[objId].ObjectTextures.Length;

                    Texture2D diffuse = ObjectManager.Instance.InteractiveElements[objId].ObjectTextures[texId].Diffuse;
                    if (diffuse != null)
                    {
                        objectMaterial.SetTexture("_MainTex", diffuse);

                        Texture2D specular = ObjectManager.Instance.InteractiveElements[objId].ObjectTextures[texId].Specular;
                        if (specular != null)
                            objectMaterial.SetTexture("_SpecularTex", specular);

                        Texture2D glossiness = ObjectManager.Instance.InteractiveElements[objId].ObjectTextures[texId].Glosiness;
                        if (glossiness != null)
                            objectMaterial.SetTexture("_RoughnessTex", glossiness);

                        Texture2D normal = ObjectManager.Instance.InteractiveElements[objId].ObjectTextures[texId].Normal;
                        if (normal != null)
                        {
                            objectMaterial.SetTexture("_NormalTex", normal);
                            objectMaterial.SetFloat("_Normal", 0.5f);
                        }
                        else
                        {
                            objectMaterial.SetFloat("_Normal", 0.0f);
                        }
                    }
                    ColorId = texId;
                }
            }
        }

        /// <summary>
        /// Changes the color amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        public void ChangeColorAmount(float amount)
        {
            objectMaterial.SetFloat("_Amount", amount);
        }

        #endregion Public Methods

        #region Non Public Methods

        #endregion Non Public Methods
    }
}
