//-----------------------------------------------------------------------
// <copyright file="DebugController.cs" company="Jam3 Inc">
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
    /// Debug controller.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class DebugController : MonoBehaviour
    {
        public GameObject DebugUI = null;

        /// <summary>
        /// Shows debug.
        /// </summary>
        public void ShowDebug()
        {
            if (DebugUI != null)
                DebugUI.SetActive(true);
        }

        /// <summary>
        /// Hides debug.
        /// </summary>
        public void HideDebug()
        {
            if (DebugUI != null)
                DebugUI.SetActive(false);
        }
    }
}
