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

        protected virtual void Init()
        {
            isInit = true;
        }

        public virtual void StartSection()
        {
            if (!isInit)
                Init();

            isSectionActive = true;
        }

        public virtual void CompleteSection()
        {
            isSectionActive = false;
            OnSectionCompleted(GetSectionType());
        }

        public virtual void SkipToSection(SectionType sectionToSkipTo)
        {
            OnSectionSkipped?.Invoke(GetSectionType(), sectionToSkipTo);
        }

        public virtual bool CausedAppPause()
        {
            return false;
        }

        private IEnumerator SkipCoroutine(SectionType sectionToSkipTo)
        {
            yield return new WaitForEndOfFrame();
            OnSectionSkipped?.Invoke(GetSectionType(), sectionToSkipTo);
        }
    }
}

