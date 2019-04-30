using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace Client
{
    public class RaceEditor
    {
        private static List<Vector3> creatorCheckpointPos;
        private static List<int> creatorBlips;
        private static List<int> creatorCheckpoints;
        private static int creatorIndex = -1;

        public static void Init()
        {
            creatorCheckpointPos = new List<Vector3>();
            creatorBlips = new List<int>();
            creatorCheckpoints = new List<int>();
        }

        public static async Task Start()
        {
            creatorIndex = 0;
            await RaceEditorPointer.Create();
        }

        public static void Update(ref int camHandle)
        {
            Camera currCam = new Camera(camHandle);
            Entity placementEntity = Entity.FromHandle(RaceEditorPointer.GetHandle());
            
            RaycastResult result = World.Raycast(currCam.Position, currCam.GetOffsetPosition(new Vector3(0.0f, 1000.0f, 0.0f)),
                IntersectOptions.Everything, placementEntity);
            Vector3 hitPos = result.HitPosition;
            
            RaceEditorPointer.Update(hitPos);
            
            if (API.IsInputDisabled(0))
            {  
                // Key "F"
                if (API.IsDisabledControlJustReleased(1, 23))
                {
                    creatorCheckpointPos.Add(new Vector3(hitPos.X, hitPos.Y, hitPos.Z));
                    int cp = API.CreateCheckpoint(42, hitPos.X, hitPos.Y, hitPos.Z, hitPos.X, hitPos.Y, hitPos.Z - 1.0f, 8.0f,
                        204, 204, 1, 100, creatorIndex);
                    creatorCheckpoints.Add(cp);
                    creatorIndex++;
                    
                    int blip = API.AddBlipForCoord(hitPos.X, hitPos.Y, hitPos.Z);
                    // Sprite Circle
                    API.SetBlipSprite(blip, 1);
                    // Display on both mini map and main map (Not Selectable)
                    API.SetBlipDisplay(blip, 8);
                    // Normal size
                    API.SetBlipScale(blip, 1.0f);
                    // Dark Taxi Yellow
                    API.SetBlipColour(blip, 28);
                    // Long range Blip
                    API.SetBlipAsShortRange(blip, false);
                    creatorBlips.Add(blip);
                }
            }
            
            API.SetBigmapActive(true, false);
        }

        public static void Stop()
        {
            creatorIndex = -1;
            RaceEditorPointer.Destory();
            API.SetBigmapActive(false, false);
        }
        
        public static void CreatorCleanUp()
        {
            creatorBlips.ForEach(blip => API.RemoveBlip(ref blip));
            creatorCheckpointPos.Clear();
            creatorCheckpoints.ForEach(API.DeleteCheckpoint);
        }

        public static List<Vector3> GetCheckpointPos()
        {
            return creatorCheckpointPos;
        }
        
        public static string GetCheckpointPosJson()
        {
            return JsonConvert.SerializeObject(creatorCheckpointPos);
        }
    }
}