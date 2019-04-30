using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client
{
    public class FreeCam
    {
        private static float CamSpeed = 2.33f;
        private static float Precision = 2.33f;
        
        private static float offsetRotX = 0.0f;
        private static float offsetRotY = 0.0f;
        private static float offsetRotZ = 0.0f;
        
        public static void Start(ref int camHandle)
        {
            API.ClearFocus();
            
            Vector3 playerCoord = Game.PlayerPed.Position;
            SetPlayerEnterCam(Game.Player.Handle, true);
            
            camHandle = API.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", playerCoord.X, playerCoord.Y, playerCoord.Z, 0.0f, 0.0f, 0.0f,
                API.GetGameplayCamFov(), false, 2);
            
            API.SetCamActive(camHandle, true);
            API.RenderScriptCams(true, false, 0, true, false);
            API.SetCamAffectsAiming(camHandle, false);
        }
        
        public static void Update(ref int camHandle)
        {
            API.DisableFirstPersonCamThisFrame();
            API.BlockWeaponWheelThisFrame();
            
            int playerPedId = API.PlayerPedId();
            Vector3 camCoord = API.GetCamCoord(camHandle);

            Vector3 newPos = ProcessNewPosition(camCoord);
            API.SetFocusArea(newPos.X, newPos.Y, newPos.Z, 0.0f, 0.0f, 0.0f);
            API.SetCamCoord(camHandle, newPos.X, newPos.Y, newPos.Z);
            API.SetCamRot(camHandle, offsetRotX, 0.0f, offsetRotZ, 2);
        }
        
        public static void Stop(ref int camHandle)
        {
            API.ClearFocus();

            SetPlayerEnterCam(Game.Player.Handle, false);
            
            API.RenderScriptCams(false, false, 0, true, false);
            API.DestroyCam(camHandle, false);

            offsetRotX = 0;
            offsetRotY = 0;
            offsetRotZ = 0;

            camHandle = -1;
        }
        
        private static void SetPlayerEnterCam(int handle, bool flag)
        {
            API.SetEntityCollision(handle, !flag, !flag);
            API.SetEntityVisible(handle, !flag, false);
            API.SetPlayerControl(handle, !flag, 0);
            API.SetEntityInvincible(handle, flag);
            API.FreezeEntityPosition(handle, flag);
        }
        
        private static Vector3 ProcessNewPosition(Vector3 pos)
        {
            float newX = pos.X;
            float newY = pos.Y;
            float newZ = pos.Z;
            
            if (API.IsInputDisabled(0))
            {
                // Key "W"
                if (API.IsDisabledControlPressed(1, 32))
                {
                    Vector3 mult = CalculateMult();

                    newX -= 0.1f * CamSpeed * mult.X;
                    newY += 0.1f * CamSpeed * mult.Y;
                    newZ += 0.1f * CamSpeed * mult.Z;
                }
                
                // Key "S"
                if (API.IsDisabledControlPressed(1, 33))
                {
                    Vector3 mult = CalculateMult();

                    newX += 0.1f * CamSpeed * mult.X;
                    newY -= 0.1f * CamSpeed * mult.Y;
                    newZ -= 0.1f * CamSpeed * mult.Z;
                }
                
                // Key "A"
                if (API.IsDisabledControlPressed(1, 34))
                {
                    Vector3 mult = CalculateMult(true);

                    newX -= 0.1f * CamSpeed * mult.X;
                    newY += 0.1f * CamSpeed * mult.Y;
                    newZ += 0.1f * CamSpeed * mult.Z;
                }
                
                // Key "D"
                if (API.IsDisabledControlPressed(1, 35))
                {
                    Vector3 mult = CalculateMult(true);

                    newX += 0.1f * CamSpeed * mult.X;
                    newY -= 0.1f * CamSpeed * mult.Y;
                    newZ -= 0.1f * CamSpeed * mult.Z;
                }

                offsetRotX -= API.GetDisabledControlNormal(1, 2) * Precision * 8.0f;
                offsetRotZ -= API.GetDisabledControlNormal(1, 1) * Precision * 8.0f;
            }

            if (offsetRotX > 90.0f)
                offsetRotX = 90.0f;
            else if (offsetRotX < -90.0f)
                offsetRotX = -90.0f;

            if (offsetRotY > 90.0f)
                offsetRotY = 90.0f;
            else if (offsetRotY < -90.0f)
                offsetRotY = -90.0f;

            if (offsetRotZ > 360.0f)
                offsetRotZ = offsetRotZ - 360.0f;
            else if (offsetRotZ < -360.0f)
                offsetRotZ = offsetRotZ + 360.0f;

            return new Vector3(newX, newY, newZ);
        }

        private static Vector3 CalculateMult(bool flag = false)
        {
            float multX = API.Sin(offsetRotZ + (flag ? 90.0f : 0.0f));
            float multY = API.Cos(offsetRotZ + (flag ? 90.0f : 0.0f));
            float multZ = API.Sin(flag ? offsetRotY : offsetRotX);
            
            return new Vector3(multX, multY, multZ);
        }
    }
}