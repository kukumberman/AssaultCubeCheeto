using System;
using System.Drawing;
using D3D = Microsoft.DirectX.Direct3D;
using Vector2 = Microsoft.DirectX.Vector2;

namespace Cucumba.Cheeto.Core
{
    class GraphicsContext : IDisposable
    {
        private const string DEFAULT_FONT_NAME = "Verdana";
        private const int DEFAULT_FONT_SIZE = 14;

        private readonly Overlay _overlay;

        private readonly D3D.Device _device;
        private readonly D3D.Font _font;
        private readonly D3D.Line _line;
        private readonly D3D.PresentParameters _presentParameters;
        
        private Color _color;

        public GraphicsContext(Overlay overlay)
        {
            _overlay = overlay;

            _presentParameters = new D3D.PresentParameters
            {
                DeviceWindowHandle = overlay.Window.Handle,
                Windowed = true,
                SwapEffect = D3D.SwapEffect.Discard,
                MultiSample = D3D.MultiSampleType.None,
                BackBufferFormat = D3D.Format.A8R8G8B8,
                BackBufferWidth = overlay.Window.Width,
                BackBufferHeight = overlay.Window.Height,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = D3D.DepthFormat.D16,
                PresentationInterval = D3D.PresentInterval.Default,
            };

            D3D.Device.IsUsingEventHandlers = true;
            _device = new D3D.Device(0, D3D.DeviceType.Hardware, _presentParameters.DeviceWindowHandle, D3D.CreateFlags.HardwareVertexProcessing, _presentParameters);

            _font = new D3D.Font(_device, new Font(DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE, FontStyle.Regular));
            _line = new D3D.Line(_device);

            _color = Color.White;
        }

        public void Dispose()
        {
            _line.Dispose();
            _font.Dispose();
            _device.Dispose();
        }

        public void Begin()
        {
            _device.RenderState.AlphaBlendEnable = true;
            _device.RenderState.AlphaTestEnable = false;
            _device.RenderState.SourceBlend = D3D.Blend.SourceAlpha;
            _device.RenderState.DestinationBlend = D3D.Blend.InvSourceAlpha;
            _device.RenderState.Lighting = false;
            _device.RenderState.CullMode = D3D.Cull.None;
            _device.RenderState.ZBufferEnable = true;
            _device.RenderState.ZBufferFunction = D3D.Compare.Always;

            _device.Clear(D3D.ClearFlags.Target | D3D.ClearFlags.ZBuffer, 0, 1, 0);

            _device.BeginScene();
        }

        public void End()
        {
            _device.EndScene();
            _device.Present();
        }

        public void Reset()
        {
            _presentParameters.BackBufferWidth = _overlay.Window.Width;
            _presentParameters.BackBufferHeight = _overlay.Window.Height;

            _device.Reset(_presentParameters);
        }

        public void UseColor(Color color)
        {
            _color = color;
        }

        public void Text(string text, float x, float y)
        {
            _font.DrawText(default, text, (int)x, (int)y, _color);
        }

        public void OutlinedText(string text, float x, float y, int outline)
        {
            _font.DrawText(default, text, (int)x + outline, (int)y, Color.Black);
            _font.DrawText(default, text, (int)x - outline, (int)y, Color.Black);
            _font.DrawText(default, text, (int)x, (int)y + outline, Color.Black);
            _font.DrawText(default, text, (int)x, (int)y - outline, Color.Black);
            _font.DrawText(default, text, (int)x, (int)y, _color);
        }

        public void CenteredText(string text, float x, float y)
        {
            var position = new Vector2(x, y);
            var size = GetTextSize(text);
            position.X -= size.X * 0.5f;
            position.Y -= size.Y * 0.5f;
            Text(text, position.X, position.Y);
        }

        public void Line(float x, float y, float xx, float yy)
        {
            var vertices = new Vector2[2];
            vertices[0].X = x;
            vertices[0].Y = y;
            vertices[1].X = xx;
            vertices[1].Y = yy;

            _line.Width = 1;
            _line.Begin();
            _line.Draw(vertices, _color);
            _line.End();
        }

        public void Box(float x, float y, float w, float h)
        {
            var vertices = new Vector2[5];
            vertices[0] = new Vector2(x, y);
            vertices[1] = new Vector2(x + w, y);
            vertices[2] = new Vector2(x + w, y + h);
            vertices[3] = new Vector2(x, y + h);
            vertices[4] = vertices[0];

            _line.Width = 1;
            _line.Begin();
            _line.Draw(vertices, _color);
            _line.End();
        }

        public void Circle(float x, float y, float radius)
        {
            var RESOLUTION = 5;
            var count = 360 / RESOLUTION;
            var step = 360f / count;

            var vertices = new Vector2[count + 1];

            for (int i = 0; i <= count; i++)
            {
                var angle = i * step * Mathf.Deg2Rad;
                vertices[i].X = x + Mathf.Sin(angle) * radius;
                vertices[i].Y = y + Mathf.Cos(angle) * radius;
            }

            _line.Width = 1;
            _line.Begin();
            _line.Draw(vertices, _color);
            _line.End();
        }

        public void CenteredBox(float x, float y, float w, float h)
        {
            var offset = new Vector2(w, h) * 0.5f;
            Box(x - offset.X, y - offset.Y, w, h);
        }

        private Vector2 GetTextSize(string text)
        {
            var rect = new Rectangle();
            _font.DrawText(default, text, ref rect, D3D.DrawTextFormat.CalculateRect, _color);
            return new Vector2(rect.Width, rect.Height);
        }
    }
}
