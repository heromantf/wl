using System;
using System.Collections.Generic;
using CitizenFX.Core;
using Newtonsoft.Json;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.Text.Common;

namespace Server
{
    public class Database
    {
        private OrmLiteConnectionFactory raceDbFactory;
        private OrmLiteConnectionFactory cpDbFactory;
        
        private class RaceTable
        {
            [AutoIncrement]
            public int id { get; set; }
            
            [Required]
            public string name { get; set; }
            
            [Required]
            public string author { get; set; }
        }

        private class CheckpointTable
        {
            [Required]
            public int race_id { get; set; }
            
            [Required]
            public int cp_idx { get; set; }
            
            [Required]
            public float pos_x { get; set; }
            
            [Required]
            public float pos_y { get; set; }
            
            [Required]
            public float pos_z { get; set; }
        }

        public Database()
        {
            string racePath = "wl/Races.db";
            string cpPath = "wl/RaceCheckpoints.db";
            
            raceDbFactory = new OrmLiteConnectionFactory($"Data Source = {racePath};Version=3;",SqliteDialect.Provider);
            cpDbFactory = new OrmLiteConnectionFactory($"Data Source = {cpPath};Version=3;",SqliteDialect.Provider);
            
            CreateTableIfNotExists();
        }
        
        private void CreateTableIfNotExists()
        {
            // Races
            using (var db = raceDbFactory.Open())
            {
                db.CreateTableIfNotExists<RaceTable>();
            }
            
            // Checkpoints
            using (var db = cpDbFactory.Open())
            {
                db.CreateTableIfNotExists<CheckpointTable>();
            }
        }

        public bool InsertRace(string _name, string _author, List<Vector3> pos)
        {
            int id = -1;
            using (var db = raceDbFactory.Open())
            {
                if (!db.Exists<RaceTable>(new {name = _name, author = _author}))
                {
                    RaceTable newRecord = new RaceTable
                    {
                        name = _name,
                        author = _author
                    };
                    
                    db.Save(newRecord, true);
                    id = newRecord.id;
                }
            }

            if (id != -1)
            {
                using (var db = cpDbFactory.Open())
                {
                    for (int i = 0; i < pos.Count; i++)
                    {
                        CheckpointTable newCp = new CheckpointTable
                        {
                            race_id = id,
                            cp_idx = i,
                            pos_x = pos[i].X,
                            pos_y = pos[i].Y,
                            pos_z = pos[i].Z
                        };

                        db.Save(newCp);
                    }
                }

                return true;
            }

            return false;
        }
        
        public List<Dictionary<string, string>> GetRaces()
        {
            List<RaceTable> races;
            List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            using (var db = raceDbFactory.Open())
            {
                races = db.Select<RaceTable>();
            }
            
            races.ForEach(race =>
            {
                Dictionary<string, string> record = new Dictionary<string, string>
                {
                    {"id", race.id.ToString()},
                    {"name", race.name},
                    {"author", race.author}
                };
                
                results.Add(record);
            });

            return results;
        }
        
        public bool InsertRaceJson(string _name, string _author, string posAsJson)
        {
            var pos = JsonConvert.DeserializeObject<List<Vector3>>(posAsJson);
            return InsertRace(_name, _author, pos);
        }
        
        public string GetRacesJson()
        {
            return JsonConvert.SerializeObject(GetRaces());
        }
    }
}