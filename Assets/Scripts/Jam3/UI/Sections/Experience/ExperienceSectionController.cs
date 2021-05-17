using UnityEngine;

namespace Jam3
{
    public class ExperienceSectionController : SectionController
    {
        public MenuControllerUI MenuControllerUI = null;

        public override SectionType GetSectionType()
        {
            return SectionType.Experience;
        }

        public override void StartSection()
        {
            base.StartSection();

            PlacementManager.Instance.ShowTarget(true);

            gameObject.SetActive(true);

            if (MenuControllerUI != null)
                MenuControllerUI.SetMenuList();
        }

        public override void CompleteSection()
        {
            gameObject.SetActive(false);
            base.CompleteSection();
        }
    }
}
