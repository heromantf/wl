using System;
using System.Collections.Generic;
using ServiceStack.OrmLite.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using CitizenFX.Core;

namespace Server
{
    
    
    public class Main : BaseScript
    {
        private Database db;
        
        public Main()
        {
            db = new Database();
            EventHandlers["rs:SaveRace"] += new Action<Player, string, List<Vector3>>(SaveRace);
            EventHandlers["rs:SaveRaceJson"] += new Action<Player, string, string>(SaveRaceJson);
            EventHandlers["rs:GetRaces"] += new Action<Player>(GetRaces);
            EventHandlers["rs:GetRacesJson"] += new Action<Player>(GetRacesJson);
        }
        
        private void SaveRace([FromSource] Player source, string name, List<Vector3> pos)
        {
            bool result = db.InsertRace(name, source.Name, pos);
            string response = result ? "OK" : "Bad";
            TriggerClientEvent(source, "rs:Received", response);
        }

        private void GetRaces([FromSource] Player source)
        {
            TriggerClientEvent(source, "rs:SetRaces", db.GetRaces());
        }
        
        private void SaveRaceJson([FromSource] Player source, string name, string posAsJson)
        {
            bool result = db.InsertRaceJson(name, source.Name, posAsJson);
            string response = result ? "OK" : "Bad";
            TriggerClientEvent(source, "rs:Received", response);
        }
        
        private void GetRacesJson([FromSource]Player source)
        {
            TriggerClientEvent(source, "rs:SetRacesJson", db.GetRacesJson());
        }
    }
    
}
