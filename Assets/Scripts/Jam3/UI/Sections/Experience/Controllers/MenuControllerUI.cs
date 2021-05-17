//-----------------------------------------------------------------------
// <copyright file="MenuControllerUI.cs" company="Jam3 Inc">
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

        /// <summary>
        /// Resets the ball
        /// </summary>
        public void ResetBall()
        {
            if (MenuList != null)
            {
                MenuListSection menuListSection = MenuList as MenuListSection;
                menuListSection.SetPlayButtonState(PlayButtonState.Placement);

                ObjectManager.Instance.Reset();
            }
        }

        /// <summary>
        /// Sets the textures menu
        /// </summary>
        public void SetMenuPlacementTextures(int objectID)
        {
            if (MenuPlacement != null)
            {
                MenuPlacementSection menuPlacement = MenuPlacement as MenuPlacementSection;
                menuPlacement.SetTextures(objectID);
                menuPlacement.SetEnable(true);
            }
        }

        /// <summary>
        /// Disables the textures menu
        /// </summary>
        public void DisablePlacementTextures()
        {
            if (MenuPlacement != null)
            {
                MenuPlacementSection menuPlacement = MenuPlacement as MenuPlacementSection;
                menuPlacement.SetEnable(false);
            }
        }

        /// <summary>
        /// Sets the items list menu
        /// </summary>
        public void SetMenuList()
        {
            SetMenu(MenuType.MenuList, true);
        }

        /// <summary>
        /// Sets the placement menu
        /// </summary>
        public void SetMenuPlacement(int id)
        {
            if (id == -1)
                DisablePlacementTextures();
            else
                SetMenuPlacementTextures(id);

            SetMenu(MenuType.MenuPlacement, true, true);
        }

        /// <summary>
        /// Sets the transform menu
        /// </summary>
        public void SetMenuTransform(int id)
        {
            if (id == -1)
                DisablePlacementTextures();
            else
                SetMenuPlacementTextures(id);

            SetMenu(MenuType.MenuPlacement, true, true);
        }

        /// <summary>
        /// Sets the side menu
        /// </summary>
        public void SetMenuSide()
        {
            SetMenu(MenuType.MenuSide, false);
        }

        /// <summary>
        /// Sets the app's menus
        /// </summary>
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
