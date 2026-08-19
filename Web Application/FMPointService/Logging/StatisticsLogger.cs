namespace FMPointService.Logging
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	using FMBusinessObjects.DataObjects;


	internal class StatisticsLogger
	{
		private class StatTimer
		{
			public Stopwatch Timer;

			public string StatName;
		}

		private static readonly object StatisticLock = new object();

		private static readonly ConcurrentDictionary<string, Statistic> Statistics = new ConcurrentDictionary<string, Statistic>();

		private static readonly ConcurrentDictionary<Guid, StatTimer> Timers = new ConcurrentDictionary<Guid, StatTimer>();

		private static readonly EventLogger EventLogger = new EventLogger();

		public void ResetStatistics()
		{
			lock (StatisticLock)
			{
				var stats = Statistics.Keys.ToList();

				Statistics.Clear();
				Timers.Clear();

				stats.ForEach(CreateNewStat);
			}
		}

		public Guid Start(string statName)
		{
			if (string.IsNullOrEmpty(statName))
			{
				throw new ArgumentNullException("statName");
			}

			// Initialize if we do not already have a stat of this type
			Statistic stat;
			if (Statistics.TryGetValue(statName, out stat) == false)
			{
				CreateNewStat(statName);
			}

			// Create a new timer
			var statTimer = new StatTimer { Timer = Stopwatch.StartNew(), StatName = statName };
			var statTimerGuid = Guid.NewGuid();
			Timers[statTimerGuid] = statTimer;

			return statTimerGuid;
		}

		private static void CreateNewStat(string statName)
		{
			Statistics[statName] = new Statistic { Name = statName };
		}

		public void Stop(Guid statTimerGuid)
		{ 
			// Find the timer
			StatTimer statTimer;
			if (Timers.TryRemove(statTimerGuid, out statTimer) == false)
			{
				EventLogger.Error( "StatisticsLogger: Timer was not found to stop: " + statTimerGuid );
				return;
			}

			lock (StatisticLock)
			{
				// Get the stat record to update
				Statistic stat;
				if ( Statistics.TryGetValue( statTimer.StatName, out stat ) == false )
				{
					EventLogger.Error("StatisticsLogger: Could not find associated stat: " + statTimer.StatName);
					return;
				}

				// Stop the timer
				statTimer.Timer.Stop();

				// Record the updated count
				stat.Count += 1;

				// Record the updated total time
				stat.TotalMillisconds += statTimer.Timer.ElapsedMilliseconds;

				stat.Max = Math.Max( stat.Max, statTimer.Timer.ElapsedMilliseconds );

				// If this is the first one, ensure we get a good min number;
				if (stat.Count == 1)
				{
					stat.Min = statTimer.Timer.ElapsedMilliseconds;
				}
				else
				{
					stat.Min = Math.Min( stat.Min, statTimer.Timer.ElapsedMilliseconds );
				}

				var average = 0.0;
				if ( stat.Count > 0 )
				{
					average = stat.TotalMillisconds / (double) stat.Count;
				}

				stat.Average = average;
			}
		}

		public List<Statistic> GetStatistics()
		{
			return Statistics.Values.ToList();
		}

		public Statistic GetStat(string statName)
		{
			if ( string.IsNullOrEmpty( statName ) )
			{
				throw new ArgumentNullException("statName");
			}

			Statistic stat;
			if ( Statistics.TryGetValue( statName, out stat ) == false )
			{
				return null;
			}

			return stat;
		}
	}
}
