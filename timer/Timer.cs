namespace SeekFilesCompare.timer
{
    using System.Diagnostics;

    public class TimerSeek
    {
        private static Stopwatch stopwatch = new Stopwatch();

        public static void Start()
        {
            stopwatch.Start();
        }

        public static void Stop()
        {
            stopwatch.Stop();
        }

        public static string GetElapsedTime()
        {
            TimeSpan elapsed = stopwatch.Elapsed;
            return $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds:000}";
        }
    }
}
