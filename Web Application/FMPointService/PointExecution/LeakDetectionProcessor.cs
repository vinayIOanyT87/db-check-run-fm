namespace FMPointService.PointExecution
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ReportSvr2005;
	using FMBusinessObjects.UtilityObjects;

	using InProcLogging;

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;

	using ThreadSupport;

	public class LeakDetectionProcessor : SrmThread
	{
		protected static LeakDetectionProcessor inst = null;

		protected readonly AutoResetEvent pointChangeEvent = new AutoResetEvent(false);

		protected SecurityClass security;

		private readonly Dictionary<Guid, Point> leakDetectionPointsDic = new Dictionary<Guid, Point>();

		public static LeakDetectionProcessor Instance()
		{
			if (inst == null)
			{
				inst = new LeakDetectionProcessor();
			}
			return inst;
		}

		/// <summary>
		/// Initial Movement Processor
		/// </summary>
		protected void Initialize()
		{
			var threadSharedData = ThreadSharedData.Instance();
			this.security = threadSharedData.Login("SiteAdmin");
		}

		public void SignalPointChanges()
		{
			pointChangeEvent.Set();
		}


		// We process point at same time of day as we would print leak rate if it was Auto Print
		private DateTimeOffset TodaysPrintTime(LeakDetectionSettings settings)
		{
			DateTimeOffset nextProccessTime = DateTimeOffset.Now.Date;
			nextProccessTime = nextProccessTime.AddHours(settings.PrintTime.Hour);
			nextProccessTime = nextProccessTime.AddMinutes(settings.PrintTime.Minute);
			nextProccessTime = nextProccessTime.AddSeconds(settings.PrintTime.Second);
			return nextProccessTime;
		}

		// We process point at 12am each day
		private DateTimeOffset TodaysStandardProccesTime()
		{
			DateTimeOffset nextProccessTime = DateTimeOffset.Now.Date;
			return nextProccessTime;
		}

		private DateTimeOffset PrintDateThisMonth(DateTimeOffset eom, LeakDetectionSettings settings)
		{
			DateTimeOffset nextPrintTime = eom.AddDays(-settings.PrintDaysBeforeEOM);
			return nextPrintTime;
		}

		private DateTimeOffset GetEndOfMonthDate()
		{
			return new DateTimeOffset(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
		}

		private void ProcessRealTimePoints()
		{
			if (this.leakDetectionPointsDic == null)
			{
				return;
			}
			DateTimeOffset eom = GetEndOfMonthDate();
			DateTimeOffset currentProcessDateTime = DateTimeOffset.Now;
			DateTimeOffset todaysStandardProccesTime = TodaysStandardProccesTime();

			foreach (Point point in this.leakDetectionPointsDic.Values)
			{
				if (this.mShutdown)
				{
					break;
				}
				bool processed = false;
				DateTimeOffset currentTime = DateTimeOffset.Now;
				var settings = GetLeakDetectionSettings(point);

				DateTimeOffset todaysPrintTime = TodaysPrintTime(settings);

				var lastRunTag = point.Tags.Values.FirstOrDefault(u => u.WellKnownIdentityGuid == Guids.LeakDetectionLastRunGuid);

				GaugeTypeClass gaugeType = FMChannelHelper.MakeCall<IGaugeTypes, GaugeTypeClass>(x => x.GetByID(this.security, settings.GaugeType));
				if (gaugeType == null)
				{
					Logger.LogError($"Gauge Type {settings.GaugeType} not found for point {point.ID}. Cannot run leak test");
					continue;
				}

				if (lastRunTag != null)
				{
					// new point not seen before
					if (lastRunTag.Value == null)
					{
						// So we process for first time today at scheduled print time
						lastRunTag.Value = DateTimeOffset.MinValue;
					}

					DateTimeOffset lastSuccesfullRunTime = (DateTimeOffset)lastRunTag.Value;

					// We have not proccessed today and todays process time has passed (Due now)
					bool shouldProcess = (todaysStandardProccesTime > lastSuccesfullRunTime && todaysStandardProccesTime < currentProcessDateTime);


					DateTimeOffset printDate = PrintDateThisMonth(eom, settings);

					// It is AutoPrint and scheduled to print today and time has passed.
					// May fail to print for month if service is down for whole of day after scheduled print time
					// Should be ok if service process point at least once after print time on the scheduled day.
					bool shouldPrint = settings.AutoPrint && printDate == DateTimeOffset.Now.Date
										 && (todaysPrintTime > lastSuccesfullRunTime && todaysPrintTime < currentProcessDateTime);

					if (shouldProcess || shouldPrint)
					{
						DateTimeOffset start = currentProcessDateTime + TimeSpan.FromDays(-30);
						LeakAnalysisResult leakAnalysisResult = new LeakAnalysisResult
						{
							GaugeType = gaugeType.IdentityGuid,
							CertRate = gaugeType.CertificationLeakRate.GetValueOrDefault(),
							LeakThreshold = gaugeType.Threshold.GetValueOrDefault(),
							MaxTemperature = 200,
							MinTemperature = -25,
							DeltaTemperature = gaugeType.DeltaTemp.GetValueOrDefault(),
							MinValue = point.Tags.Values.FirstOrDefault(u => u.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid).GetMinimum(point),
							MaxValue = point.Tags.Values.FirstOrDefault(u => u.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid).GetMaximum(point),
							MinimumFillPercentage = settings.MinimumFillPercentage,
							MinGaugeTestTime = (ushort)gaugeType.MinHours.GetValueOrDefault()
						}; 

						LeakDetectionError leakError = FMChannelHelper.MakeCall<ILeakTests, LeakDetectionError>(x => x.Run(this.security, point, settings.AnalysisType, settings.AnalysisMethod, start, currentProcessDateTime, ref leakAnalysisResult));

						if (shouldPrint)
						{
							var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.security, this.security.SiteGuid, false, false, false));
							EventLog eventLog = new EventLog("Application", ".", "FuelsManager"); ;

							if (leakError != LeakDetectionError.None)
							{
								// We have errors
								Logger.LogError("Error calculating leak rate:" + leakError.ToString());
							}
							else
							{
								if (!string.IsNullOrEmpty(site.LeakDetectionReport))
								{
									SystemSettingClass systemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(systemSettings => systemSettings.Get(this.security));

									ParameterValue[] parameterValues = new ParameterValue[1];

									parameterValues[0] = new ParameterValue { Name = "LeakReportId", Value = leakAnalysisResult.LeakRecordId.ToString() };

									string rptDir = FMChannelHelper.MakeCall<ISites, string>(
																										  x =>
																										  x.GetReportDirectory(this.security, site.LeakDetectionReport)
																									);
									ReportServicePrintService printService =
										 new ReportServicePrintService(eventLog)
										 {
											 ReportingServiceUrl = systemSetting.ReportServerUrl,
											 ReportName = rptDir + "/" + site.LeakDetectionReport,
											 ParameterValues = parameterValues,
											 Security = this.security,
											 EnableBOLPDFArchiving = false,
										 };

									printService.PrintReport();
								}
							}
						}

						Logger.LogDebug("leak Detection Thread placeholer call Leak Detection Service DELETE ANAYLISIS DATA " + point.PointId);
						lastRunTag.Value = currentProcessDateTime;
						processed = true;
					}

					if (processed)
					{
						ThreadSharedData.Instance().SetPointTag(lastRunTag);

						List<PointTag> tagList = new List<PointTag>
						{
							lastRunTag
						};
						FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, true));
					}
				}
				else
				{
					Logger.LogCritical("LeakDetectionProcessor next run time tag not found for point: " + point.PointId);
				}

			}
		}

		private LeakDetectionSettings GetLeakDetectionSettings(Point point)
		{
			LeakDetectionSettings settings = null;
			PointProperty leakDetectionSettingsProperty = point.Properties.Values.SingleOrDefault(u => u.ValueTypeString == LeakDetectionSettings.LeakDetectionSettingsIdentifier);
			if (leakDetectionSettingsProperty != null)
			{
				settings = (LeakDetectionSettings)leakDetectionSettingsProperty.Value;
			}
			return settings;
		}

		private void GetLeakDetectionSettings()
		{
			leakDetectionPointsDic.Clear();

			var sharedDataPoints = ThreadSharedData.Instance().GetRealTimeLeakDetectionPoints();

			foreach (var point in sharedDataPoints.Values)
			{
				LeakDetectionSettings settings = GetLeakDetectionSettings(point);
				if (settings != null && settings.AnalysisType.Equals(LeakAnalysisType.RealTime))
				{
					if (settings.AnalysisType.Equals(LeakAnalysisType.RealTime))
					{
						leakDetectionPointsDic.Add(point.IdentityGuid, point);
					}
				}
			}
		}

		public override void Run()
		{
			try
			{
				this.Initialize();

				WaitHandle[] events = { this.pointChangeEvent };

				while (this.mShutdown != true)
				{
					try
					{
						var eventThatSignaled = WaitHandle.WaitAny(events, 1000);
						if (eventThatSignaled == 0)
						{
							this.GetLeakDetectionSettings();
						}

						this.ProcessRealTimePoints();
					}
					catch (Exception ex)
					{
						Logger.LogError("LeakDetectionProcessor Inner Loop Exception: " + ex);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError("LeakDetectionProcessor exception: " + ex);
			}
		}
	}
}
