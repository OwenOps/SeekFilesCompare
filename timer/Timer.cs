namespace SeekFilesCompare.timer
{
    using System.Diagnostics;

    public class TimerSeek
    {
        private static Stopwatch _stopwatch = new Stopwatch();

        public static void Start()
        {
            _stopwatch.Start();
        }

        public static void Stop()
        {
            _stopwatch.Stop();
        }

        public static string GetElapsedTime()
        {
            TimeSpan elapsed = _stopwatch.Elapsed;
            return $"{elapsed.Days:00}:{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds:000}";
        }
    }
}
