//-----------------------------------------------------------------------
// <copyright file="ExperienceSectionController.cs" company="Jam3 Inc">
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
    public class ExperienceSectionController : SectionController
    {
        public MenuControllerUI MenuControllerUI = null;

        /// <summary>
        /// Returns the section type
        /// </summary>
        public override SectionType GetSectionType()
        {
            return SectionType.Experience;
        }

        /// <summary>
        /// Starts the section
        /// </summary>
        public override void StartSection()
        {
            base.StartSection();

            PlacementManager.Instance.ShowTarget(true);

            gameObject.SetActive(true);

            if (MenuControllerUI != null)
                MenuControllerUI.SetMenuList();
        }

        /// <summary>
        /// Completes the section
        /// </summary>
        public override void CompleteSection()
        {
            gameObject.SetActive(false);
            base.CompleteSection();
        }
    }
}
