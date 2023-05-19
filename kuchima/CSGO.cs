using System;
using System.Diagnostics;
using System.Collections.Generic;
using GameMath;

using static kuchima.Offsets;

namespace kuchima
{
    public class CSGO
    {
        public static Memory memory;
        public static IntPtr client;
        public static IntPtr engine;

        public static bool Load()
        {
            Process[] procs = Process.GetProcessesByName("csgo");
            if (procs.Length < 1) return false;
            memory = new Memory(procs[0]);
            client = memory.GetModuleBaseAddress("client.dll");
            engine = memory.GetModuleBaseAddress("engine.dll");
            if (client == IntPtr.Zero || engine == IntPtr.Zero) return false;
            return true;
        }

        public static IntPtr GetClientState() => memory.Read<IntPtr>(engine + (int)hazedumper.signatures.dwClientState);
        public static IntPtr GetLocalPlayer() => memory.Read<IntPtr>(client + (int)hazedumper.signatures.dwLocalPlayer);
        public static int GetTeamId(IntPtr entity) => memory.Read<int>(entity + (int)hazedumper.netvars.m_iTeamNum);
        public static IntPtr GetEntity(int idx) => memory.Read<IntPtr>(client + (int)hazedumper.signatures.dwEntityList + idx * 0x10);
        public static int GetHealth(IntPtr entity) => memory.Read<int>(entity + (int)hazedumper.netvars.m_iHealth);
        public static Matrix4x4 GetViewMatrix() => memory.Read<Matrix4x4>(client + (int)hazedumper.signatures.dwViewMatrix);
        public static bool IsInGame() => memory.Read<int>(GetClientState() + (int)hazedumper.signatures.dwClientState_State) == 6;
        public static bool IsDormant(IntPtr entity) => memory.Read<bool>(entity + (int)hazedumper.signatures.m_bDormant);

        public static IntPtr[] GetEntityList(IntPtr localplayer)
        {
            int maxPlayers = memory.Read<int>(GetClientState() + (int)hazedumper.signatures.dwClientState_MaxPlayer);
            List<IntPtr> entities = new();
            for (int i = 0; i < maxPlayers; i++)
            {
                IntPtr ent = GetEntity(i);
                if (ent != localplayer)
                {
                    entities.Add(ent);
                }
            }
            return entities.ToArray();
        }

        public static bool IsVisible(IntPtr entity)
        {
            int localPlayer = memory.Read<int>(GetClientState() + (int)hazedumper.signatures.dwClientState_GetLocalPlayer);
            int spotted = memory.Read<int>(entity + (int)hazedumper.netvars.m_bSpottedByMask);
            return Convert.ToBoolean(spotted & (1 << localPlayer));
        }

        public static Vector3 GetBonePos(IntPtr entity, int bone)
        {
            IntPtr pointer = memory.Read<IntPtr>(entity + (int)hazedumper.netvars.m_dwBoneMatrix);
            Matrix3x4 boneMatrix = memory.Read<Matrix3x4>(pointer + 0x30 * bone);
            return new Vector3(boneMatrix.M14, boneMatrix.M24, boneMatrix.M34);
        }
    }
}
