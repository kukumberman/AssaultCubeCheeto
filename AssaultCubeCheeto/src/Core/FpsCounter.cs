using System;
using System.Diagnostics;

namespace Cucumba.Cheeto.Core
{
    public class FpsCounter
    {
        private static readonly TimeSpan TimeSpanFpsUpdate = new TimeSpan(0, 0, 0, 0, 500);

        private readonly Stopwatch _stopwatch;

        private int _frameCount;

        public double Fps { get; private set; }

        public FpsCounter()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void Update()
        {
            var fpsTimerElapsed = _stopwatch.Elapsed;

            if (fpsTimerElapsed > TimeSpanFpsUpdate)
            {
                Fps = _frameCount / fpsTimerElapsed.TotalSeconds;
                _stopwatch.Restart();
                _frameCount = 0;
            }
            
            _frameCount++;
        }
    }
}