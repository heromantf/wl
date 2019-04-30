using MenuAPI;

namespace Client.Menus
{
    public class RaceStartMenu : SubMenu
    {
        protected override void CreateMenu()
        {
            menu = new Menu("Race Name", "Race Author");
            menu.AddMenuItem(new MenuItem("Start"));
            

            menu.OnItemSelect += (_menu, _item, _index) =>
            {
                if (_item.Text == "Start")
                {
                    MenuController.CloseAllMenus();
                }
            };
        }

        public RaceStartMenu(MainMenu rootMenu) : base(rootMenu) { }
    }
}