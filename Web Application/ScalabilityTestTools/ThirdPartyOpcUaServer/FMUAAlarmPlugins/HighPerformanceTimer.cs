using System;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace FMUAAlarmPlugins
{
	public class HighPerformanceTimer
	{
		#region DLL Imports
		[DllImport("kernel32.dll")]
		protected static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

		[DllImport("kernel32.dll")]
		protected static extern bool QueryPerformanceFrequency(ref long lpFrequency);
		#endregion

		public static DateTime convertToDateTime(long aHighPerformanceTimerTicks)  //Loss of Precision
		{
			long nowHPT = Now;
			DateTime nowDT = DateTime.Now;
			double elapsedSeconds = convertToSeconds(nowHPT - aHighPerformanceTimerTicks);
			TimeSpan ts = new TimeSpan(((long)(elapsedSeconds * 10000000)));
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
				if (QueryPerformanceCounter(ref currentTime) == false)
				{
					// Frequency not supported
					throw new Win32Exception();
				}
				return (currentTime);
			}
		}

		public static void Wait(double durationSeconds)
		{
			long durationTicks = convertToTicks(durationSeconds);
			Wait(durationTicks);
		}

		public static void Wait(long durationTicks)
		{
			long preSleep = HighPerformanceTimer.Now;
			double msecWait = convertToSeconds(durationTicks) * 1000.00;
			if (msecWait > 10.00)
			{
				int iMsecWait = (int)(msecWait - 10.00);
				System.Threading.Thread.Sleep(iMsecWait);
			}
			long postSleep = HighPerformanceTimer.Now;
			long newDurationTics = durationTicks + preSleep - postSleep;
			long start = HighPerformanceTimer.Now;
			while (HighPerformanceTimer.Now - start < newDurationTics)
			{

			}
		}
	}
}
