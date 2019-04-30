using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using CitizenFX.Core.UI;
using Client.Menus;
using Newtonsoft.Json;
using MenuAPI;

namespace Client
{
    public class Main : BaseScript
    {
        private int camHandle = -1;
        private static MainMenu mainMenu;

        private bool isReceived;
        private string response;

        private double timerLocal = 0.0;

        public Main()
        {
            // Add task to every tick
            Tick += OnTick;;
            
            // Initialize instance variables
            mainMenu = new MainMenu();
            RaceEditor.Init();
            
            // Register command and events
            API.RegisterCommand("race", new Action<int, List<object>, string>(RaceCommand), false);
            EventHandlers["rs:Received"] += new Action<string>(Received);
            EventHandlers["rs:SetRaces"] += new Action<List<Dictionary<string, string>>>(SetRaces);
            EventHandlers["rs:SetRacesJson"] += new Action<string>(SetRacesJson);
        }

        private void SetRaces(List<Dictionary<string, string>> races)
        {
            mainMenu.SetRaces(races);
        }
        
        private void SetRacesJson(string racesAsJson)
        {
            mainMenu.SetRacesJson(racesAsJson);
        }

        private void Received(string resp)
        {
            response = resp;
            isReceived = true;
        }

        private void RaceCommand(int source, List<object> args, string raw)
        {
            // Open the menu
            mainMenu.GetMenu().OpenMenu();
        }

        private async Task CheckSave()
        {
            if (mainMenu.requestSave)
            {
                mainMenu.requestSave = false;
                //TriggerServerEvent("rs:SaveRace", mainMenu.raceName, RaceEditor.GetCheckpointPos());
                TriggerServerEvent("rs:SaveRaceJson", mainMenu.raceName, RaceEditor.GetCheckpointPosJson());
                while (!isReceived)
                {
                    await Delay(100);
                    timerLocal += 100;

                    if (timerLocal >= 1500)
                    {
                        timerLocal = 0;
                        break;
                    }
                }
                
                // Notification above mini map
                API.SetNotificationTextEntry("STRING");
                API.AddTextComponentString(response == "OK" ? "Saved Successfully!" : "Save Failed!");
                API.DrawNotification(false, true);
            }
        }

        private void CheckCleanUp()
        {
            if (mainMenu.requestCleanUp)
            {
                mainMenu.requestCleanUp = false;
                RaceEditor.CreatorCleanUp();
            }
        }

        private async Task OnTick()
        {
            if (mainMenu.isPlacingCP)
            {
                if (camHandle == -1)
                {
                    FreeCam.Start(ref camHandle);
                    await RaceEditor.Start();
                }
                else
                {
                    FreeCam.Update(ref camHandle);
                    RaceEditor.Update(ref camHandle);
                }
            }
            else if (camHandle != 1)
            {
                FreeCam.Stop(ref camHandle);
                RaceEditor.Stop();
            }

            CheckCleanUp();
            await CheckSave();
            await Delay(0);
        }
    }
}
