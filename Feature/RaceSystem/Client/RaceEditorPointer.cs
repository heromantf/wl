using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client
{
    public class RaceEditorPointer
    {
        private static int placementHandle;
        //private static int errPlacementHandle;
        private static int ringHandle;

        public static async Task Create()
        {
            string placementName = "prop_mp_placement";
            //string errPlacementName = "prop_mp_placement_red";
            string ringName = "prop_mp_pointer_ring";

            Vector3 playerPos = Game.PlayerPed.Position;
            placementHandle = await CreateLocalObject(placementName, playerPos);
            //errPlacementHandle = await CreateLocalObject(errPlacementName, playerPos);
            ringHandle = await CreateLocalObject(ringName, playerPos);
        }

        public static void Update(Vector3 hitPos)
        {
            UpdateObjects(hitPos, API.GetEntityRotation(placementHandle, 2));
            float height = API.GetEntityHeightAboveGround(placementHandle);

            if (API.PlaceObjectOnGroundProperly(placementHandle) &&
                API.PlaceObjectOnGroundProperly(ringHandle))
            {
                if (height >= 0.1f)
                    UpdateObjects(hitPos, new Vector3(0.0f,0.0f,0.0f));
            }
        }

        public static void Destory()
        {
            API.SetEntityAsMissionEntity(placementHandle, true, true);
            //API.SetEntityAsMissionEntity(errPlacementHandle, true, true);
            API.SetEntityAsMissionEntity(ringHandle, true, true);
            
            API.DeleteObject(ref placementHandle);
            //API.DeleteObject(ref errPlacementHandle);
            API.DeleteObject(ref ringHandle);
        }
        
        private static async Task<int> CreateLocalObject(string name, Vector3 pos)
        {
            int hash = API.GetHashKey(name);
            API.RequestModel((uint) hash);
            while (!API.HasModelLoaded((uint) hash))
                await BaseScript.Delay(1);
            return API.CreateObject(hash, pos.X, pos.Y, pos.Z, false, true, false);
        }
        
        private static void UpdateObjects(Vector3 pos, Vector3 rot)
        {
            API.SetEntityCoords(placementHandle, pos.X, pos.Y, pos.Z, true, false, false, true);
            API.SetEntityCoords(ringHandle, pos.X, pos.Y, pos.Z, true, false, false, true);
            
            API.SetEntityRotation(placementHandle, rot.X, rot.Y, rot.Z, 2, true);
            API.SetEntityRotation(ringHandle, rot.X, rot.Y, rot.Z, 2, true);
        }

        public static int GetHandle()
        {
            return placementHandle;
        }
    }
}