using System;
using System.Drawing;
using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace ConsoleApp1
{
    class Program
    {
        private const string TITLE = "Silk.NET example";

        private IWindow _window = null;
        private ImGuiController _controller = null;
        private GL _gl = null;
        private IInputContext _inputContext = null;
        private IKeyboard _keyboard = null;

        private bool _showDemoWindow = false;
        private int _someNumber = 0;
        private float _myFloat = 1;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            var options = WindowOptions.Default;
            options.Title = TITLE;
            options.Size = new Vector2D<int>(1280, 720);

            _window = Window.Create(options);

            _window.Load += Window_Load;
            _window.FramebufferResize += Window_Resize;
            _window.Render += Window_Render;
            _window.Closing += Window_Closing;

            _window.Run();
        }

        private void Window_Load()
        {
            _gl = _window.CreateOpenGL();
            _inputContext = _window.CreateInput();

            _keyboard = _inputContext.Keyboards[0];
            _keyboard.KeyDown += Keyboard_KeyDown;

            _controller = new ImGuiController(_gl, _window, _inputContext);

            //! use this handle to manipulate ex-style and dwmapi
            var nativeHandle = _window.Native.Win32.Value.Hwnd;
        }

        private void Window_Resize(Vector2D<int> size)
        {
            _gl.Viewport(size);
        }

        private void Window_Render(double deltaTime)
        {
            _controller.Update((float)deltaTime);

            _gl.ClearColor(Color.FromArgb(255, (int)(.45f * 255), (int)(.55f * 255), (int)(.60f * 255)));
            _gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            Render();

            _controller.Render();
        }

        private void Window_Closing()
        {
            _controller.Dispose();
            _controller = null;

            _inputContext.Dispose();
            _inputContext = null;

            _gl.Dispose();
            _gl = null;
        }

        private void Render()
        {
            if (_showDemoWindow)
            {
                ImGui.ShowDemoWindow(ref _showDemoWindow);
            }

            ImGui.Begin(TITLE);

            var io = ImGui.GetIO();
            var deltaTime = io.DeltaTime;
            var fps = io.Framerate;
            var ms = deltaTime * 1000;

            ImGui.Text($"{fps:0.0} {deltaTime:0.000}");
            ImGui.Text(string.Format("{0:0.0} {1:0.000} {2:0.0}ms", fps, deltaTime, ms));

            if (ImGui.Checkbox("Show Demo Window", ref _showDemoWindow))
            {
                Console.WriteLine(_showDemoWindow);
            }

            if (ImGui.SliderInt("#slider1", ref _someNumber, 0, 100, $"SliderInt {_someNumber}"))
            {
                Console.WriteLine(_someNumber);
            }

            ImGui.SliderFloat("#slider2", ref _myFloat, 0, 1);

            var draw = ImGui.GetBackgroundDrawList();

            var red = ImGui.GetColorU32(new Vector4(1, 0, 0, 1));
            var green = ImGui.ColorConvertFloat4ToU32(new Vector4(0, 1, 0, 1));
            var blue = ImGui.GetColorU32(new Vector4(0, 0, 1, 1));

            draw.AddLine(new Vector2(0, 0), new Vector2(_window.Size.X, _window.Size.Y), red);
            draw.AddLine(new Vector2(_window.Size.X, 0), new Vector2(0, _window.Size.Y), red);

            var center = new Vector2(_window.Size.X * 0.5f, _window.Size.Y * 0.5f);
            var radius = 100;
            draw.AddCircle(center, radius, green);

            var offset = new Vector2(radius, radius);
            var min = center - offset;
            var max = center + offset;
            draw.AddRect(min, max, blue);
        }

        private void Keyboard_KeyDown(IKeyboard kb, Key key, int arg3)
        {
            if (key == Key.Keypad5)
            {
                _showDemoWindow = !_showDemoWindow;
            }
        }
    }
}
