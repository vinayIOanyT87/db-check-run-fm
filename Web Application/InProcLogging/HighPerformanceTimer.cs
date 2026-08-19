namespace InProcLogging
{
	using System;
	using System.ComponentModel;
	using System.Runtime.InteropServices;

	public class HighPerformanceTimer
    {
        #region DLL Imports
        [DllImport("kernel32.dll")]
        protected static extern bool QueryPerformanceCounter(ref long
            lpPerformanceCount);

        [DllImport("kernel32.dll")]
        protected static extern bool QueryPerformanceFrequency(ref long
            lpFrequency);
        #endregion

        public static DateTime convertToDateTime(long aHighPerformanceTimerTicks)  //Loss of Precision
        {
            long nowHPT = Now;
            DateTime nowDT = DateTime.Now;
            double elapsedSeconds = convertToSeconds(nowHPT - aHighPerformanceTimerTicks);
            TimeSpan ts = new TimeSpan(((long)(elapsedSeconds*10000000)));
            return nowDT.Subtract(ts);
        }

        public static double convertToSeconds(long aHighPerformanceTimerTicks)
        {
            return ((double)(aHighPerformanceTimerTicks)) / ((double)(Frequency));
        }

        public static long convertToTicks(double seconds)
        {
            double ret = seconds * ((double)(Frequency));
            return (long)ret;
        }

        public static long convertToTicks(DateTime timestamp)
        {
            long nowHPT = Now;
            DateTime nowDT = DateTime.Now;
            TimeSpan ts = nowDT.Subtract(timestamp);
            double elapsedSeconds = ts.TotalSeconds;
            double nowSeconds = HighPerformanceTimer.convertToSeconds(nowHPT);
            return HighPerformanceTimer.convertToTicks(nowSeconds - elapsedSeconds);
        }

        public static long Frequency
        {
            get
            {
                long counterFrequency = 0;
                if (QueryPerformanceFrequency(ref counterFrequency) == false)
                {
                    // Frequency not supported
                    throw new Win32Exception();
                }
                return counterFrequency;
            }
        }

        public static long Now
        {
            get
            {
                long currentTime = 0;
                if(QueryPerformanceCounter(ref currentTime) == false)
                {
                    // Frequency not supported
                    throw new Win32Exception();
                }
                return (currentTime);
            }
        }
    }
}
