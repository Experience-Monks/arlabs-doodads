//-----------------------------------------------------------------------
// <copyright file="SyncObjectController.cs" company="Jam3 Inc">
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
    /// Sync object controller.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    [ExecuteInEditMode]
    public class SyncObjectController : MonoBehaviour
    {
        public GameObject ObjectToSync = null;
        public Vector3 Offset = Vector3.zero;

        /// <summary>
        /// Lates update.
        /// </summary>
        private void LateUpdate()
        {
            if (ObjectToSync != null)
            {
                gameObject.transform.position = ObjectToSync.transform.position + Offset;
            }
        }
    }
}
