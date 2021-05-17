using UnityEngine;

namespace Jam3
{
    public enum MenuType
    {
        MenuList,
        MenuPlacement,
        MenuTop,
        MenuSide
    }

    public class MenuControllerUI : MonoBehaviour
    {
        public MenuSectionUI MenuList = null;
        public MenuSectionUI MenuPlacement = null;
        public MenuSectionUI MenuTop = null;
        public MenuSectionUI MenuSide = null;

        private bool isFirstOpen = true;

        public void ResetBall()
        {
            if (MenuList != null)
            {
                MenuListSection menuListSection = MenuList as MenuListSection;
                menuListSection.SetPlayButtonState(PlayButtonState.Placement);

                ObjectManager.Instance.Reset();
            }
        }

        public void SetMenuPlacementTextures(int objectID)
        {
            if (MenuPlacement != null)
            {
                MenuPlacementSection menuPlacement = MenuPlacement as MenuPlacementSection;
                menuPlacement.SetTextures(objectID);
                menuPlacement.SetEnable(true);
            }
        }

        public void DisablePlacementTextures()
        {
            if (MenuPlacement != null)
            {
                MenuPlacementSection menuPlacement = MenuPlacement as MenuPlacementSection;
                menuPlacement.SetEnable(false);
            }
        }

        public void SetMenuList()
        {
            SetMenu(MenuType.MenuList, true);
        }

        public void SetMenuPlacement(int id)
        {
            if (id == -1)
                DisablePlacementTextures();
            else
                SetMenuPlacementTextures(id);

            SetMenu(MenuType.MenuPlacement, true, true);
        }

        public void SetMenuTransform(int id)
        {
            if (id == -1)
                DisablePlacementTextures();
            else
                SetMenuPlacementTextures(id);

            SetMenu(MenuType.MenuPlacement, true, true);
        }

        public void SetMenuSide()
        {
            SetMenu(MenuType.MenuSide, false);
        }

        private void SetMenu(MenuType type, bool useTopMenu = false, bool useSideMenu = false)
        {
            if (MenuList != null)
            {
                if (type == MenuType.MenuList)
                {
                    if (isFirstOpen)
                    {
                        MenuList.Close();
                        isFirstOpen = false;
                    }
                    else
                        MenuList.Open();
                }
                else
                    MenuList.Hide();
            }

            if (MenuPlacement != null)
            {
                if (type == MenuType.MenuPlacement)
                    MenuPlacement.Open();
                else
                    MenuPlacement.Close();
            }

            if (MenuSide != null)
            {
                if (useSideMenu)
                    MenuSide.Open();
                else
                    MenuSide.Close();
            }

            if (MenuTop != null)
            {
                if (useTopMenu)
                    MenuTop.Open();
                else
                    MenuTop.Close();
            }
        }
    }
}
