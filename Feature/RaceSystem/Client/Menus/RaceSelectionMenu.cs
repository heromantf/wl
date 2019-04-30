using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MenuAPI;

namespace Client.Menus
{
    public class RaceSelectionMenu : SubMenu
    {
        private static RaceStartMenu raceStartMenu;
        
        protected override void CreateMenu()
        {
            menu = new Menu("Race Selector");
            raceStartMenu = new RaceStartMenu(rootMenu);
            MenuController.AddSubmenu(menu, raceStartMenu.GetMenu()); 

            menu.OnItemSelect += (_menu, _item, _index) =>
            {
                string raceName = _item.Text;
                string raceAuthor = _item.Label;

                raceStartMenu.GetMenu().MenuTitle = raceName;
                raceStartMenu.GetMenu().MenuSubtitle = raceAuthor;
                raceStartMenu.GetMenu().RefreshIndex();
            };
        }

        public RaceStartMenu GetStartMenu()
        {
            return raceStartMenu;
        }
        
        public RaceSelectionMenu(MainMenu rootMenu) : base(rootMenu) { }
    }
}
