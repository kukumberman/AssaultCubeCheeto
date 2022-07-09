using System;
using System.Windows.Forms;
using Cucumba.Cheeto.Native;
using Cucumba.Cheeto.Structs;

namespace Cucumba.Cheeto.Core
{
    class Overlay : IDisposable
    {
        public event Action OnTargetWindowDestroyed;
        public event Action OnTargetWindowChanged;

        public readonly Form Window;
        public readonly GraphicsContext Graphics;

        private readonly string _targetWindowName;

        private IntPtr _currentTargetHandle;
        private bool _isTargetValid;

        public Overlay(string targetWindowName)
        {
            _targetWindowName = targetWindowName;

            Window = new Form
            {
                Name = "Overlay Name",
                Text = "Overlay Text",
                MinimizeBox = false,
                MaximizeBox = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
            };

            Window.Load += Window_Load;
            Window.SizeChanged += Window_SizeChanged;

            Graphics = new GraphicsContext(this);
        }

        public void Dispose()
        {
            Graphics.Dispose();
            Window.Close();
        }

        public void UpdateSize(int width, int height)
        {
            Window.Size = new System.Drawing.Size(width, height);
        }

        public void UpdateLocation(int x, int y)
        {
            Window.Location = new System.Drawing.Point(x, y);
        }

        public bool ValidateGameWindow()
        {
            if (!User32.IsWindow(_currentTargetHandle) && _isTargetValid)
            {
                UpdateLocation(0, 0);
                UpdateSize(500, 500);

                OnTargetWindowDestroyed.Invoke();

                _isTargetValid = false;
                return false;
            }

            var hwnd = User32.FindWindow(null, _targetWindowName);

            if (!User32.GetClientRect(hwnd, out var rect))
            {
                return false;
            }

            var point = new Point
            {
                X = rect.Left,
                Y = rect.Top
            };

            if (!User32.ClientToScreen(hwnd, out point))
            {
                return false;
            }

            UpdateLocation(point.X, point.Y);
            UpdateSize(rect.Width, rect.Height);

            if (_currentTargetHandle != hwnd)
            {
                _currentTargetHandle = hwnd;
                OnTargetWindowChanged?.Invoke();
            }

            _isTargetValid = true;
            return true;
        }

        public bool IsKeyPressed(int vKey)
        {
            return (User32.GetAsyncKeyState(vKey) & 1) > 0;
        }

        public bool IsKeyDown(int vKey)
        {
            return User32.GetAsyncKeyState(vKey) > 0;
        }

        private void Window_SizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Window_SizeChanged");
            
            if (Graphics != null)
            {
                Graphics.Reset();
            }
        }

        private void Window_Load(object sender, EventArgs args)
        {
            SetExtendedStyle();
            ExtendFrameIntoClientArea();
        }

        private void SetExtendedStyle()
        {
            var exStyle = User32.GetWindowLong(Window.Handle, User32.GWL_EXSTYLE);
            exStyle |= User32.WS_EX_LAYERED;
            exStyle |= User32.WS_EX_TRANSPARENT;
            User32.SetWindowLong(Window.Handle, User32.GWL_EXSTYLE, exStyle);
            User32.SetLayeredWindowAttributes(Window.Handle, 0, 255, User32.LWA_ALPHA);
        }

        private void ExtendFrameIntoClientArea()
        {
            var margins = new Margins
            {
                Left = -1,
                Right = -1,
                Top = -1,
                Bottom = -1
            };
            Dwmapi.DwmExtendFrameIntoClientArea(Window.Handle, ref margins);
        }
    }
}
