using System;
using GameMath;
using GameOverlay.Drawing;
using GameOverlay.Windows;

using static kuchima.Native;

namespace kuchima.Hacks
{
    public class ESP
    {
        private static StickyWindow overlay;
        public static void StartThread()
        {
            var graphics = new Graphics();
            graphics.WindowHandle = IntPtr.Zero;
            graphics.VSync = false;
            graphics.PerPrimitiveAntiAliasing = false;
            graphics.UseMultiThreadedFactories = false;

            overlay = new StickyWindow(CSGO.memory.proc.MainWindowHandle, graphics);
            overlay.IsTopmost = true;
            overlay.IsVisible = true;
            overlay.FPS = 60;
            overlay.Title = "kuchima Overlay";
            overlay.DrawGraphics += DrawGraphics;
            overlay.Create();

            SetWindowDisplayAffinity(overlay.Handle, 0x11);
        }

        private static void DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            e.Graphics.ClearScene();
            IntPtr focusedWindow = GetForegroundWindow();
            if (focusedWindow == CSGO.memory.proc.MainWindowHandle && CSGO.IsInGame())
            {
                IntPtr localPlayer = CSGO.GetLocalPlayer();
                int localTeam = CSGO.GetTeamId(localPlayer);

                IntPtr[] entities = CSGO.GetEntityList(localPlayer);
                foreach (IntPtr entity in entities)
                {
                    int entityTeam = CSGO.GetTeamId(entity);
                    if (CSGO.GetHealth(entity) > 0 && !CSGO.IsDormant(entity))
                    {
                        if (entityTeam != localTeam)
                        {
                            Matrix4x4 viewmatrix = CSGO.GetViewMatrix();
                            Vector3 topPlayerPos = CSGO.GetBonePos(entity, 8);
                            Vector3 bottomPlayerPos = CSGO.GetBonePos(entity, 1);
                            topPlayerPos = new Vector3(topPlayerPos.X, topPlayerPos.Y, topPlayerPos.Z + 10);

                            Vector2 top2dpos = Math3.WorldToScreen(viewmatrix, topPlayerPos, overlay.Width, overlay.Height);
                            Vector2 bottom2dpos = Math3.WorldToScreen(viewmatrix, bottomPlayerPos, overlay.Width, overlay.Height);

                            float height = bottom2dpos.Y - top2dpos.Y;
                            float width = (height / 2) * 1.2f;
                            float x = bottom2dpos.X - (width / 2);
                            float y = top2dpos.Y;

                            SolidBrush defaultBrush = e.Graphics.CreateSolidBrush(138, 43, 226);
                            SolidBrush visibleBrush = e.Graphics.CreateSolidBrush(255, 255, 0);
                            SolidBrush fill = e.Graphics.CreateSolidBrush(0, 0, 0, 65);

                            e.Graphics.DrawBox2D(e.Graphics.CreateSolidBrush(Color.Transparent), fill, x, y, x + width, y + height, 2);
                            if (CSGO.IsVisible(entity)) e.Graphics.DrawRectangleEdges(visibleBrush, x, y, x + width, y + height, 2);
                            else e.Graphics.DrawRectangleEdges(defaultBrush, x, y, x + width, y + height, 2);
                        }
                    }
                }
            }
        }
    }
}
