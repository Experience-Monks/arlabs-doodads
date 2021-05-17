using UnityEngine;

namespace Jam3
{
    public class OnboardingSectionController : SectionController
    {
        public OnboardingSectionUI OnboardingSectionUI = default;

        public override SectionType GetSectionType()
        {
            return SectionType.Home;
        }

        public override void StartSection()
        {
            base.StartSection();
            gameObject.SetActive(true);

            if (OnboardingSectionUI != null)
                OnboardingSectionUI.StartFlow(CompleteSection);
        }

        public override void CompleteSection()
        {
            gameObject.SetActive(false);
            base.CompleteSection();
        }
    }
}
