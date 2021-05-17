//-----------------------------------------------------------------------
// <copyright file="SectionController.cs" company="Jam3 Inc">
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

using System;
using System.Collections;

using UnityEngine;

namespace Jam3
{
    public enum SectionType
    {
        Loading,
        CameraPermission,
        Home,
        Scan,
        Experience,
        Record,
        Share
    }

    public abstract class SectionController : MonoBehaviour
    {
        public bool IsSectionActive { get => isSectionActive; }

        public abstract SectionType GetSectionType();
        public Action<SectionType> OnSectionCompleted = delegate { };
        public Action<SectionType, SectionType> OnSectionSkipped = delegate { };

        private bool isInit = false;
        private bool isSectionActive = false;

        /// <summary>
        /// Section initialization
        /// </summary>
        protected virtual void Init()
        {
            isInit = true;
        }

        /// <summary>
        /// Starts the section
        /// </summary>
        public virtual void StartSection()
        {
            if (!isInit)
                Init();

            isSectionActive = true;
        }

        /// <summary>
        /// Completes the section
        /// </summary>
        public virtual void CompleteSection()
        {
            isSectionActive = false;
            OnSectionCompleted(GetSectionType());
        }

        /// <summary>
        /// Skips to section
        /// </summary>
        public virtual void SkipToSection(SectionType sectionToSkipTo)
        {
            OnSectionSkipped?.Invoke(GetSectionType(), sectionToSkipTo);
        }

        /// <summary>
        /// Can return the app pause cause, in this case the camera permission request
        /// </summary>
        public virtual bool CausedAppPause()
        {
            return false;
        }

        /// <summary>
        /// Skips coroutine
        /// </summary>
        private IEnumerator SkipCoroutine(SectionType sectionToSkipTo)
        {
            yield return new WaitForEndOfFrame();
            OnSectionSkipped?.Invoke(GetSectionType(), sectionToSkipTo);
        }
    }
}

