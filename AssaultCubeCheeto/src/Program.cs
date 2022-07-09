using System;
using System.Diagnostics;
using System.Threading;
using Cucumba.Cheeto.Core;
using Cucumba.Cheeto.Structs;
using Cucumba.Cheeto.Native;
using Cucumba.Cheeto.AssaultCube;
using Color = System.Drawing.Color;

namespace Cucumba.Cheeto
{
    class Program : IDisposable
    {
        private const string PROCESS_NAME = "ac_client";
        private const string WINDOW_NAME = "AssaultCube";
        private const int TARGET_FRAMERATE = 120;

        private GameWorld _gameWorld = null;
        private Overlay _overlay = null;
        private FpsCounter _fpsCounter = null;

        private readonly int KEY_TOGGLE_RECOIL = User32.VK_NUMPAD5;
        private readonly int KEY_AIMBOT = User32.VK_KEY_Q;
        private readonly int KEY_BUNNYHOP = User32.VK_KEY_F;

        private int _frameCount;
        private readonly float _aimbotFovRadius = 100;
        private bool _isAimbotActive = false;
        private bool _aimbotHasTarget = false;
        private PlayerEntity _aimbotTarget = null;

        private readonly TimeSpan _sleep = new TimeSpan(0, 0, 0, 0, (int)(1f / TARGET_FRAMERATE * 1000));

        static void Main()
        {
            using (var p = new Program())
            {
                p.Run();
            }

            Console.ReadKey();
        }

        private static Color GetColorOverTime(float time)
        {
            Vector3 tempColor = new Vector3
            {
                X = Mathf.Cos(time + 0) * 0.5f + 0.5f,
                Y = Mathf.Cos(time + 2) * 0.5f + 0.5f,
                Z = Mathf.Cos(time + 4) * 0.5f + 0.5f,
            };

            tempColor *= byte.MaxValue;

            return Color.FromArgb((int)tempColor.X, (int)tempColor.Y, (int)tempColor.Z);
        }

        public void Dispose()
        {
            if (_gameWorld != null)
            {
                _gameWorld.Dispose();
                _gameWorld = null;
            }

            if (MemoryContext.Instance != null)
            {
                MemoryContext.Instance.Dispose();
                MemoryContext.Instance = null;
            }

            _overlay.Dispose();
        }

        private void Run()
        {
            _fpsCounter = new FpsCounter();

            _overlay = new Overlay(WINDOW_NAME);
            _overlay.OnTargetWindowChanged += Overlay_OnTargetWindowChanged;
            _overlay.OnTargetWindowDestroyed += Overlay_OnTargetWindowDestroyed;

            _overlay.Window.Show();
            _overlay.UpdateLocation(0, 0);
            _overlay.UpdateSize(500, 500);

            while (true)
            {
                Thread.Sleep(_sleep);

                Tick();

                if (_overlay.IsKeyPressed(User32.VK_END))
                {
                    break;
                }
            }
        }

        private void Overlay_OnTargetWindowChanged()
        {
            Console.WriteLine("Overlay_OnTargetWindowChanged");

            var processes = Process.GetProcessesByName(PROCESS_NAME);

            if (processes.Length > 0)
            {
                var process = processes[0];
                var clientAddress = process.MainModule.BaseAddress;
                var flags = Kernel32.PROCESS_VM_READ | Kernel32.PROCESS_VM_WRITE | Kernel32.PROCESS_VM_OPERATION;
                IntPtr handle = Kernel32.OpenProcess(flags, false, process.Id);
                MemoryContext.Instance = new MemoryContext(handle);
                _gameWorld = new GameWorld(clientAddress);
            }
        }

        private void Overlay_OnTargetWindowDestroyed()
        {
            Console.WriteLine("Overlay_OnTargetWindowDestroyed");

            MemoryContext.Instance.Dispose();
            MemoryContext.Instance = null;

            _gameWorld.Dispose();
            _gameWorld = null;
        }

        private void Tick()
        {
            _frameCount++;

            _fpsCounter.Update();

            bool valid = _overlay.ValidateGameWindow();

            var center = new Vector2(_overlay.Window.Width, _overlay.Window.Height) * 0.5f;

            _overlay.Graphics.Begin();

            _overlay.Graphics.UseColor(Color.White);
            _overlay.Graphics.Text($"{_fpsCounter.Fps:0}", 10, 10);

            string time = DateTime.Now.ToShortTimeString();
            _overlay.Graphics.CenteredText(time, center.X, 25);

            if (valid)
            {
                _gameWorld.Update();

                _aimbotHasTarget = FindAimClosestPlayer(out _aimbotTarget);

                string text = $"{_gameWorld.GameMode} - {_gameWorld.MapName} ({_gameWorld.GameVersion})";
                _overlay.Graphics.UseColor(Color.White);
                _overlay.Graphics.CenteredText(text, center.X, 50);

                DrawBorders();
                DrawPlayers();
                DrawCrosshair();

                KeyboardHandler();

                if (_isAimbotActive)
                {
                    UpdateAimbot();
                }
            }
            else
            {
                DrawExample();
            }

            _overlay.Graphics.End();
        }

        private void DrawExample()
        {
            var w = _overlay.Window.Width;
            var h = _overlay.Window.Height;
            var center = new Vector2(w * 0.5f, h * 0.5f);

            var time = _frameCount * 0.1f;
            _overlay.Graphics.UseColor(GetColorOverTime(time));
            _overlay.Graphics.Box(0, 0, w - 1, h - 1);

            _overlay.Graphics.UseColor(Color.White);
            _overlay.Graphics.Line(0, 0, w, h);
            _overlay.Graphics.Line(w, 0, 0, h);
            _overlay.Graphics.CenteredBox(center.X, center.Y, 200, 200);
            _overlay.Graphics.Circle(center.X, center.Y, 200);

            var text = "Hello, World!";
            _overlay.Graphics.CenteredText(text, center.X, center.Y);
        }

        private void KeyboardHandler()
        {
            if (_overlay.IsKeyPressed(KEY_TOGGLE_RECOIL))
            {
                _gameWorld.ToggleRecoil();
            }

            if (_overlay.IsKeyDown(KEY_BUNNYHOP))
            {
                _gameWorld.LocalPlayer.TryForceJump();
            }

            _isAimbotActive = _overlay.IsKeyDown(KEY_AIMBOT);
        }

        private void DrawBorders()
        {
            const int offset = 1;
            
            var time = _frameCount * 0.1f;
            var color = GetColorOverTime(time);

            _overlay.Graphics.UseColor(color);
            _overlay.Graphics.Box(offset, offset, _overlay.Window.Width - 1 - offset * 2, _overlay.Window.Height - 1 - offset * 2);
        }

        private void DrawPlayers()
        {
            var localPlayerTeam = _gameWorld.LocalPlayer.Team;
            var isTeamMode = _gameWorld.IsTeamGameMode;

            for (int i = 0, length = _gameWorld.Players.Count; i < length; i++)
            {
                var player = _gameWorld.Players[i];

                if (player.Health <= 0)
                {
                    continue;
                }

                if (!_gameWorld.Camera.WorldToScreen(player.HeadPosition, out var head))
                {
                    continue;
                }

                if (!_gameWorld.Camera.WorldToScreen(player.Origin, out var foot))
                {
                    continue;
                }

                var boxCenter = new Vector2(head.X + foot.X, head.Y + foot.Y);
                boxCenter.X *= 0.5f;
                boxCenter.Y *= 0.5f;

                var boxHeight = foot.Y - head.Y;
                var boxWidth = boxHeight * 0.5f;

                if (isTeamMode)
                {
                    if (player.Team == localPlayerTeam)
                    {
                        _overlay.Graphics.UseColor(Color.Green);
                    }
                    else
                    {
                        _overlay.Graphics.UseColor(Color.Red);
                    }
                }
                else
                {
                    _overlay.Graphics.UseColor(Color.White);
                }

                _overlay.Graphics.Circle(head.X, head.Y, 2);
                _overlay.Graphics.CenteredBox(boxCenter.X, boxCenter.Y, boxWidth, boxHeight);

                _overlay.Graphics.UseColor(Color.White);
                _overlay.Graphics.CenteredText($"{player.Health}", head.X, head.Y - 20);
            }
        }

        private void DrawCrosshair()
        {
            var center = new Vector2(_overlay.Window.Width, _overlay.Window.Height) * 0.5f;

            _overlay.Graphics.UseColor(Color.White);
            _overlay.Graphics.Circle(center.X, center.Y, _aimbotFovRadius);

            if (_aimbotHasTarget)
            {
                _gameWorld.Camera.WorldToScreen(_aimbotTarget.HeadPosition, out var w2s);

                _overlay.Graphics.UseColor(Color.Yellow);
                _overlay.Graphics.Line(center.X, center.Y, w2s.X, w2s.Y);
            }
        }

        private bool FindAimClosestPlayer(out PlayerEntity result)
        {
            result = null;
            var found = false;
            var closestDistance = float.MaxValue;

            var center = new Vector2(_overlay.Window.Width, _overlay.Window.Height) * 0.5f;

            var localPlayerTeam = _gameWorld.LocalPlayer.Team;
            var isTeamMode = _gameWorld.IsTeamGameMode;

            for (int i = 0, length = _gameWorld.Players.Count; i < length; i++)
            {
                var player = _gameWorld.Players[i];
                
                if (isTeamMode && player.Team == localPlayerTeam)
                {
                    continue;
                }

                if (player.Health <= 0)
                {
                    continue;
                }

                if (!_gameWorld.Camera.WorldToScreen(player.HeadPosition, out var w2s))
                {
                    continue;
                }

                float distanceFromCenter = Vector2.Distance(center, w2s);

                if (distanceFromCenter > _aimbotFovRadius)
                {
                    continue;
                }

                if (distanceFromCenter < closestDistance)
                {
                    result = player;
                    found = true;
                    closestDistance = distanceFromCenter;
                }
            }

            return found;
        }

        private void UpdateAimbot()
        {
            if (!_aimbotHasTarget)
            {
                return;
            }

            if (_aimbotTarget.Health <= 0)
            {
                return;
            }

            Vector3 dir = _aimbotTarget.HeadPosition - _gameWorld.LocalPlayer.HeadPosition;

            float pitch = Mathf.Asin(dir.Z / dir.Magnitude) * Mathf.Rad2Deg;
            float yaw = Mathf.Atan2(dir.Y, dir.X) * Mathf.Rad2Deg + 90;

            MemoryContext.Instance.Write(_gameWorld.LocalPlayer.BaseAddress + Offsets.PlayerEntity_Yaw, new Vector2(yaw, pitch));
        }
    }
}
