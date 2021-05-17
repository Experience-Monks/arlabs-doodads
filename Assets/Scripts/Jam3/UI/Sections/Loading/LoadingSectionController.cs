using UnityEngine;

namespace Jam3
{
    public class LoadingSectionController : SectionController
    {
        public LoadingSectionUI LoadingSectionUI = default;

        public override SectionType GetSectionType()
        {
            return SectionType.Loading;
        }

        public override void StartSection()
        {
            base.StartSection();
            gameObject.SetActive(true);

            if (LoadingSectionUI != null)
                LoadingSectionUI.StartFlow(CompleteSection);
        }

        public override void CompleteSection()
        {
            gameObject.SetActive(false);
            base.CompleteSection();
        }
    }
}
