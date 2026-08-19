namespace FMPointService.WcfPointService
{
	using System;
	using System.Configuration;
	using System.ServiceModel;
	using System.Threading;

	using FMBusinessObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using Logging;
	using ThreadSupport;
	using System.Linq;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	using InProcLogging;

	public class PingProcessor : SrmThread
	{
		private static PingProcessor inst = null;

		private string host;

		private string port;

		private int MaxPointsToProcess;

		private readonly AutoResetEvent ShutdownEvent = new AutoResetEvent(false);

		private readonly AutoResetEvent PointChangedEvent = new AutoResetEvent(false);

		private static readonly EventLogger EventLogger = new EventLogger();

		private static readonly StatisticsLogger StatisticsLogger = new StatisticsLogger();

		public PingProcessor(string host, string port)
		{
			this.host = host;
			this.port = port;

			hostname = this.host + ":" + this.port;

			pingIntervalInSeconds = int.Parse(ConfigurationManager.AppSettings["PingIntervalInSeconds"]);
			percentCpuUtilizationThrottleLevel = int.Parse(ConfigurationManager.AppSettings["PercentCpuUtilizationThrottleLevel"]);
			percentMemoryUtilizationThrottleLevel = int.Parse(ConfigurationManager.AppSettings["PercentMemoryUtilizationThrottleLevel"]);
			MaxPointsToProcess = int.Parse(ConfigurationManager.AppSettings["MaxPointsToProcess"]);
			thrShrData = ThreadSharedData.Instance();
			security = thrShrData.Login("SiteAdmin");
			thrShrData.Clear();
			previousPointCheckSumDictionary = new Dictionary<Guid, PointChecksum>();

		}

		public static PingProcessor Instance(string host, string port)
		{
			if (inst == null)
			{
				inst = new PingProcessor(host, port);
			}
			return inst;
		}

		public static PingProcessor Instance()
		{
			if (inst == null)
			{
				throw new Exception("PingProcessor not initialized");
			}
			return inst;
		}



		public void SignalShutdown()
		{
			this.mShutdown = true;
			this.ShutdownEvent.Set();
		}

		public void SignalPointChanged()
		{
			this.PointChangedEvent.Set();
		}


		protected void Ping()
		{
			FMChannelHelper.MakeCall<IPointServiceManager>(
				x =>	x.Ping(security, hostname,
				PointServiceHealthStatus.Good,
				pingIntervalInSeconds,
				0,
				percentCpuUtilizationThrottleLevel,
				0,
				percentMemoryUtilizationThrottleLevel,
				MaxPointsToProcess)
			);
		}


		protected SchedulePointsResponse SchedulePoints()
		{
			return FMChannelHelper.MakeCall<IPointServiceManager, SchedulePointsResponse>(


			x =>
			{
				((IClientChannel)x).OperationTimeout = new TimeSpan(0, 10, 0);
				return x.SchedulePoints(security, hostname);
			}
			);
		}


		protected PointCollection GetPointsEx(List<Guid> pointsToRetrieve)
		{
			if (pointsToRetrieve != null && pointsToRetrieve.Count > 0)
			{
				if (pointsToRetrieve.Count <= numPointsPerCall)
				{
					return FMChannelHelper.MakeCall<IPointServiceManager, PointCollection>(x => x.GetPointsForHostnameEx(security, hostname, pointsToRetrieve));
				}
				else
				{
					PointCollection points = new PointCollection();
					for (int i = 0; i < pointsToRetrieve.Count; i = i + numPointsPerCall)
					{
							if (mShutdown)
							{
								return new PointCollection();
							}
							var numPointsToGet = pointsToRetrieve.Count - i > numPointsPerCall ? numPointsPerCall : pointsToRetrieve.Count - i;
							var subPoints = FMChannelHelper.MakeCall<IPointServiceManager, PointCollection>(x => x.GetPointsForHostnameEx(security, hostname, pointsToRetrieve.GetRange(i, numPointsToGet)));
							points.AddRange(subPoints);
					}
					return points;
				}
			}
			else
			{
				return new PointCollection();
			}
		}

		protected SecurityClass security;

		private ThreadSharedData thrShrData;

		protected int pingIntervalInSeconds;

		protected int percentCpuUtilizationThrottleLevel;

		protected int percentMemoryUtilizationThrottleLevel;

		protected string hostname;

		protected int numPointsPerCall = 1000;

		protected Dictionary<Guid, PointChecksum> previousPointCheckSumDictionary = new Dictionary<Guid, PointChecksum>();

		protected long lastPingTime = 0;

		protected int GetSleepTimeForNextPing()
		{
			int millisecondsToSleep = 0;
			long currentTime = HighPerformanceTimer.Now;
			if (this.lastPingTime != 0)
			{
				long pingIntervalTicks = HighPerformanceTimer.convertToTicks((double)(this.pingIntervalInSeconds));
				long nextPingTime = this.lastPingTime + pingIntervalTicks;
				var ticksToSleep = nextPingTime - currentTime;
				double sleepTimeDouble = HighPerformanceTimer.convertToSeconds(ticksToSleep) * 1000.00;
				if (sleepTimeDouble > 100.00)
				{
					millisecondsToSleep = (int)sleepTimeDouble;
				}
				else
				{
					this.lastPingTime = currentTime;
					millisecondsToSleep = 0;
				}
			}
			else
			{
				this.lastPingTime = currentTime;
				millisecondsToSleep = 0;
			}
			return millisecondsToSleep;
		}

		protected void DelayForNextPing()
		{
			WaitHandle[] events = { this.ShutdownEvent, this.PointChangedEvent };

			while (true && !this.mShutdown)
			{
				int sleepTime = this.GetSleepTimeForNextPing();
				if (sleepTime > 0)
				{
					WaitHandle.WaitAny(events, sleepTime);
					return;
					
				}
				else
				{
					return;
				}
			}
		}

		protected void UpdateShelvedAlarmInfo()
		{
			ThreadSharedData.Instance().UpdateShelvedAlarmInfo();
		}

		protected void SchedulePointsLogicEx()
		{
			long getPointsStart = 0, getPointsStop = 0, mergePointsStart = 0, mergePointsStop = 0;
			long startStamp = HighPerformanceTimer.Now;
			var schedulePointsResponse = this.SchedulePoints();
			long pingStop = HighPerformanceTimer.Now;
			var pointsToRetrieve = new List<Guid>();


			if (schedulePointsResponse.Status == SchedulePointsStatus.Good
			|| schedulePointsResponse.Status == SchedulePointsStatus.NoPointsAssigned)
			{
				getPointsStart = HighPerformanceTimer.Now;
				var pointCheckSumDictionary = new Dictionary<Guid, PointChecksum>();
				foreach (var pointCheckSum in schedulePointsResponse.PointCheckSums)
				{
					pointCheckSumDictionary.Add(pointCheckSum.PointGuid, pointCheckSum);
				}
				var deletedPoints = new List<Guid>();
				foreach (var prevPointChecksum in this.previousPointCheckSumDictionary.Values)
				{
					PointChecksum pointCheckSum;
					if (pointCheckSumDictionary.TryGetValue(prevPointChecksum.PointGuid, out pointCheckSum))
					{
						if(prevPointChecksum.MaxRowVersion != pointCheckSum.MaxRowVersion)
						{
							pointsToRetrieve.Add(pointCheckSum.PointGuid);
						}
					}
					else
					{
						deletedPoints.Add(prevPointChecksum.PointGuid);
					}
				}

				foreach(var pointChecksumGuid in pointCheckSumDictionary.Keys)
				{
					if(!this.previousPointCheckSumDictionary.ContainsKey(pointChecksumGuid))
					{
						pointsToRetrieve.Add(pointChecksumGuid);
					}
				}

				Guid statTimer = Guid.Empty;
				PointCollection points = null;
				try
				{
					statTimer = StatisticsLogger.Start("Ping Get Points");
					points = this.GetPointsEx(pointsToRetrieve);
				}
				finally
				{
					StatisticsLogger.Stop(statTimer);
				}
				mergePointsStart = HighPerformanceTimer.Now;
				ThreadSharedData.Instance().MergePoints(points, deletedPoints);
				mergePointsStop = HighPerformanceTimer.Now;

				if (pointsToRetrieve.Count > 0)
				{
					var pointDictionary = new Dictionary<Guid, Point>();
					foreach (var point in points)
					{
						pointDictionary.Add(point.PointGuid, point);
					}

					foreach (var pointGuid in pointsToRetrieve)
					{
						// point didn't load, set checksum to prior checksum if available else zero to force attempt reload at next ping
						if (!pointDictionary.ContainsKey(pointGuid))
						{
							if (this.previousPointCheckSumDictionary.ContainsKey(pointGuid))
							{
								pointCheckSumDictionary[pointGuid].MaxRowVersion = this.previousPointCheckSumDictionary[pointGuid].MaxRowVersion;
							}
							else
							{
								pointCheckSumDictionary[pointGuid].MaxRowVersion = 0;
							}
						}
					}
				}

				this.previousPointCheckSumDictionary = pointCheckSumDictionary;
				getPointsStop = HighPerformanceTimer.Now;
			}

			long stopStamp = HighPerformanceTimer.Now;
			if (pointsToRetrieve.Count != 0)
			{
				Logger.LogDebug("PingHandlerThread.SchedulePointsLogicEx SchedulePoints Result " + schedulePointsResponse.Status.ToString() + " Took " + HighPerformanceTimer.convertToSeconds(stopStamp - startStamp) + " seconds");
				Logger.LogDebug("PingHandlerThread.SchedulePointsLogicEx SchedulePoints call took " + HighPerformanceTimer.convertToSeconds(pingStop - startStamp) + " seconds, GetPointsForHostname took " + HighPerformanceTimer.convertToSeconds(getPointsStop - getPointsStart) + " seconds for " + pointsToRetrieve.Count + " points, MergePoints took " + HighPerformanceTimer.convertToSeconds(mergePointsStop - mergePointsStart) + " seconds.");
			}
			// Excess log entries that cause bloat in the Point_Service Log
			//Logger.LogDebug("PingHandlerThread.SchedulePointsLogicEx PointCount " + this.thrShrData.PointCount + " TagCount " + this.thrShrData.TagCount);
			Logger.LogDebug("PingHandlerThread.SchedulePointsLogicEx NumNonGoodOpcTags " + this.thrShrData.NumNonGoodOpcTags);
		}

		public override void Run()
		{

			if (this.SetThreadPrioirty(ThreadPriority.Highest) == false)
			{
				Logger.LogError("OpcUaClientProcessor.Run Error setting thread priority");
			}
			while (this.mShutdown == false)
			{
				try
				{
					//TBD: Need to do a rowversion check and continually check in case there are site changes
					this.thrShrData.Clear();
					this.previousPointCheckSumDictionary = new Dictionary<Guid, PointChecksum>();
					int exceptionCount = 0;
					while (this.mShutdown == false)
					{
						try
						{
							this.Ping();
							this.SchedulePointsLogicEx();
							this.UpdateShelvedAlarmInfo();

							this.DelayForNextPing();
							exceptionCount = 0;
						}
						catch (Exception ex)
						{
							Logger.LogCritical("PingHandlerThread.Run Inner Run Loop Exception: " + ex.Message);
							Logger.Flush();
							exceptionCount++;
							if(exceptionCount > 2)
							{
								throw ex;
							}

						}
					}
				}
				catch (Exception ex)
				{
					Logger.LogCritical("PingHandlerThread.Run Exception: " + ex.Message);
					Logger.Flush();

					EventLogger.Error("PingHandler: " + ex);
				}
			}

			this.thrShrData.Clear();
			this.previousPointCheckSumDictionary = new Dictionary<Guid, PointChecksum>();
		}
	}
}
