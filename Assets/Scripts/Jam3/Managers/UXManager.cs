//-----------------------------------------------------------------------
// <copyright file="UXManager.cs" company="Jam3 Inc">
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
using UnityEngine;
using Jam3.Util;

namespace Jam3
{
    /// <summary>
    /// U x manager.
    /// </summary>
    /// <seealso cref="Singleton<UXManager>" />
    public class UXManager : Singleton<UXManager>
    {
        public Action<SectionType> SectionChanged;
        [SerializeField] private SectionController[] sectionControllers = null;

        public SectionController CurrentSectionController => currentSectionController;

        [Header("Editor Only")]
        [SerializeField] bool jumpToTestSection = false;
        [SerializeField] private SectionType testSection = SectionType.Loading;
        public bool isDebugging = false;

        // Runtime variables
        private SectionController currentSectionController = null;

        /// <summary>
        /// Gets or sets the current section.
        /// </summary>
        /// <value>
        /// The current section.
        /// </value>
        public SectionType CurrentSection { get; private set; }

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            SectionType firstSection = SectionType.Loading;

            if (Application.isEditor && jumpToTestSection && testSection != firstSection)
            {
                SectionController firstSectionController = ControllerForSection(firstSection);
                AddListeners(firstSectionController);
                firstSectionController.SkipToSection(testSection);
            }
            else
            {
                GoToSection(firstSection);
            }
        }

        /// <summary>
        /// Ons application pause.
        /// </summary>
        /// <param name="pauseStatus">The pause status.</param>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (currentSectionController == null || currentSectionController.CausedAppPause())
                return;

            // if (pauseStatus)
            // {
            //     if (CurrentSection != SectionType.Loading && CurrentSection != SectionType.Start)
            //         GoToSection(SectionType.Experience);
            // }
            // else
            // {
            //     ARSessionToggle.Instance.Toggle(false);
            // }
        }

        /// <summary>
        /// Jumps to section.
        /// </summary>
        /// <param name="section">The section.</param>
        public void JumpToSection(SectionType section)
        {
            GoToSection(section);
        }

        /// <summary>
        /// Gos to section.
        /// </summary>
        /// <param name="type">The type.</param>
        public void GoToSection(SectionType type)
        {
            if (currentSectionController != null && currentSectionController.GetSectionType() == type)
            {
                Debug.LogWarning("UXManager is already at section: " + type + ", skipping.");
                return;
            }

            CurrentSection = type;
            SectionChanged?.Invoke(type);

            if (currentSectionController != null && currentSectionController.IsSectionActive)
            {
                ClearListeners();
                currentSectionController.CompleteSection();
            }

            currentSectionController = ControllerForSection(CurrentSection);
            StartCurrentSection();
        }

        /// <summary>
        /// Ons section complete.
        /// </summary>
        /// <param name="type">The type.</param>
        private void OnSectionComplete(SectionType type)
        {
            ClearListeners();
            GoToSection(NextSection(type));
        }

        /// <summary>
        /// Ons section skipped.
        /// </summary>
        /// <param name="skippedSection">The skipped section.</param>
        /// <param name="sectionToSkipTo">The section to skip to.</param>
        private void OnSectionSkipped(SectionType skippedSection, SectionType sectionToSkipTo)
        {
            ClearListeners();
            SectionType nextSection = NextSection(skippedSection);
            if (nextSection == sectionToSkipTo)
            {
                UnityEngine.Debug.Log("Successfully skipped to section: " + nextSection);
                GoToSection(nextSection);
            }
            else
            {
                SectionController nextSectionController = ControllerForSection(nextSection);
                AddListeners(nextSectionController);
                nextSectionController.SkipToSection(sectionToSkipTo);
            }
        }

        /// <summary>
        /// Starts current section.
        /// </summary>
        private void StartCurrentSection()
        {
            if (currentSectionController)
            {
                AddListeners(currentSectionController);
                currentSectionController.StartSection();
            }
        }

        /// <summary>
        /// Adds listeners.
        /// </summary>
        /// <param name="controller">The controller.</param>
        private void AddListeners(SectionController controller)
        {
            controller.OnSectionCompleted += OnSectionComplete;
            controller.OnSectionSkipped += OnSectionSkipped;
        }

        /// <summary>
        /// Clears listeners.
        /// </summary>
        private void ClearListeners()
        {
            if (currentSectionController)
            {
                currentSectionController.OnSectionCompleted -= OnSectionComplete;
                currentSectionController.OnSectionSkipped -= OnSectionSkipped;
            }
        }

        /// <summary>
        /// Nexts section.
        /// </summary>
        /// <param name="fromSection">The from section.</param>
        private SectionType NextSection(SectionType fromSection)
        {
            switch (fromSection)
            {
                case SectionType.Loading: return SectionType.CameraPermission;
                case SectionType.CameraPermission: return SectionType.Home;
                case SectionType.Home: return SectionType.Scan;
                case SectionType.Scan: return SectionType.Experience;
                case SectionType.Experience: return SectionType.Home;
                default:
                    UnityEngine.Debug.LogWarningFormat("Section {0} has no Next Section.", fromSection);
                    return SectionType.Loading;
            }
        }

        /// <summary>
        /// Controllers for section.
        /// </summary>
        /// <param name="section">The section.</param>
        private SectionController ControllerForSection(SectionType section)
        {
            foreach (SectionController controller in sectionControllers)
            {
                if (controller != null)
                {
                    if (controller.GetSectionType() == section)
                        return controller;
                }
            }

            UnityEngine.Debug.LogWarningFormat("Section {0} has no Controller.", section);
            return null;
        }
    }
}
