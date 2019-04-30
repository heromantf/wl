using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using MenuAPI;
using Newtonsoft.Json;

namespace Client.Menus
{
    public class MainMenu
    {
        private Menu menu;
        private static RaceSelectionMenu raceSelectionMenu;
        private static RaceCreatorTypeMenu raceCreatorMenu;

        public bool isPlacingCP;
        public bool requestSave;
        public bool requestCleanUp;
        public string raceName;

        public Menu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }

        public void SetRaces(List<Dictionary<string, string>> races)
        {
            raceSelectionMenu.GetMenu().ClearMenuItems();
            races.ForEach(record =>
            {
                MenuItem race = new MenuItem(record["name"]) {Label = record["author"]};
                raceSelectionMenu.GetMenu().AddMenuItem(race);
                MenuController.BindMenuItem(raceSelectionMenu.GetMenu(), raceSelectionMenu.GetStartMenu().GetMenu(), race);
            });
            raceSelectionMenu.GetMenu().RefreshIndex();
        }
        
        public void SetRacesJson(string racesAsJson)
        {
            var races = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(racesAsJson);
            SetRaces(races);
        }

        private void CreateMenu()
        {
            // Menu align with left side
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Left;

            // Create the main menu
            menu = new Menu("Race System") { Visible = false };
            raceCreatorMenu = new RaceCreatorTypeMenu(this);
            raceSelectionMenu = new RaceSelectionMenu(this);

            MenuController.AddMenu(menu);
            MenuController.MainMenu = menu;

            // Menu buttons
            MenuItem raceStartMenuBtn = new MenuItem("Start Race", "Start a race you want!");
            MenuItem createRaceMenuBtn = new MenuItem("Create Race", "Try to create your own race!");
            MenuItem exitMenuBtn = new MenuItem("Exit", "Enjoy the race next time!");

            menu.AddMenuItem(raceStartMenuBtn);
            menu.AddMenuItem(createRaceMenuBtn);
            menu.AddMenuItem(exitMenuBtn);

            // Add and bind buttons to submenus
            MenuController.AddSubmenu(menu, raceCreatorMenu.GetMenu());
            MenuController.AddSubmenu(menu, raceSelectionMenu.GetMenu());
            MenuController.BindMenuItem(menu, raceCreatorMenu.GetMenu(), createRaceMenuBtn);
            MenuController.BindMenuItem(menu, raceSelectionMenu.GetMenu(), raceStartMenuBtn);

            // Button handler for exit button
            menu.OnItemSelect += (_menu, _item, _index) =>
            {
                if (_item.Text == "Exit")
                    menu.CloseMenu();
                
                if(_item.Text == "Start Race")
                    //BaseScript.TriggerServerEvent("rs:GetRaces");
                    BaseScript.TriggerServerEvent("rs:GetRacesJson");
            };
        }
    }
}
