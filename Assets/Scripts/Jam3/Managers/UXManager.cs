using System;
using UnityEngine;
using Jam3.Util;

namespace Jam3
{
    public class UXManager : Singleton<UXManager>
    {
        public Action<SectionType> SectionChanged;
        [SerializeField] private SectionController[] sectionControllers = null;

        public SectionController CurrentSectionController => currentSectionController;
        public SectionType CurrentSection { get; private set; }

        private SectionController currentSectionController = null;

        [Header("Editor Only")]
        [SerializeField] bool jumpToTestSection = false;
        [SerializeField] private SectionType testSection = SectionType.Loading;
        public bool isDebugging = false;

        void Start()
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

        public void JumpToSection(SectionType section)
        {
            GoToSection(section);
        }

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

        private void OnSectionComplete(SectionType type)
        {
            ClearListeners();
            GoToSection(NextSection(type));
        }

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

        void StartCurrentSection()
        {
            if (currentSectionController)
            {
                AddListeners(currentSectionController);
                currentSectionController.StartSection();
            }
        }

        private void AddListeners(SectionController controller)
        {
            controller.OnSectionCompleted += OnSectionComplete;
            controller.OnSectionSkipped += OnSectionSkipped;
        }

        private void ClearListeners()
        {
            if (currentSectionController)
            {
                currentSectionController.OnSectionCompleted -= OnSectionComplete;
                currentSectionController.OnSectionSkipped -= OnSectionSkipped;
            }
        }

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
