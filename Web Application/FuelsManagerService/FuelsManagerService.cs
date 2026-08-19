// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   A service responsible for background processing associated with FuelsManager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.ServiceModel;
	using System.ServiceProcess;
	using System.Text;
	using System.Threading;
	using System.Timers;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	using global::FuelsManagerService.Record_Version_Propagation;

	using Timer = System.Timers.Timer;

	public static class FuelsManagerSettings
	{
		/// <summary>
		/// The standard default number of minutes to wait
		/// </summary>
		const int DefaultIntervalMinutes = 15;
		const int DefaultRecordVersionPropagationIntervalSeconds = 60;

		private static readonly string MaxLoginAttemptsKey = "MaximumNumberOfLoginAttempts";
		private static readonly string UserAccountCleanupEnabledKey = "UserAccountCleanupEnabled";
		private static readonly string UserAccountCleanupIntervalKey = "UserAccountCleanupIntervalMinutes";
		private static readonly string AuditProcessingEnabledKey = "AuditProcessingEnabled";
		private static readonly string AuditProcessingIntervalKey = "AuditProcessingIntervalMilliseconds";
		private static readonly string AlarmAndEventProcessingEnabledKey = "AlarmAndEventProcessingEnabled";
		private static readonly string AlarmAndEventProcessingIntervalKey = "AlarmAndEventProcessingIntervalMinutes";
		private static readonly string AlarmAndEventLogCleanupEnabledKey = "AlarmAndEventLogCleanupEnabled";
		private static readonly string AlarmAndEventLogCleanupIntervalKey = "AlarmAndEventLogCleanupIntervalMinutes";
		private static readonly string FCEEMessagesCleanupEnabledKey = "FCEEMessagesCleanupEnabled";
		private static readonly string FCEEMessagesCleanupIntervalKey = "FCEEMessagesCleanupIntervalMinutes";
		private static readonly string fceHeartbeatSentryEnabledKey = "fceHeartbeatSentryEnabled";
		private static readonly string fceHeartbeatSentryIntervalKey = "fceHeartbeatSentryIntervalMinutes";
		private static readonly string SessionCleanupEnabledKey = "SessionCleanupEnabled";
		private static readonly string SessionCleanupIntervalKey = "SessionCleanupIntervalMinutes";
		private static readonly string FMaePingEnabledKey = "FMAEPingEnabled";
		private static readonly string FMaePingIntervalKey = "FMAEPingIntervalMinutes";
		private static readonly string AutoCloseoutEnabledKey = "AutoCloseoutEnabled";
		private static readonly string AutoCloseoutPollIntervalKey = "AutoCloseoutPollIntervalMinutes";
		private static readonly string AutoCloseoutRunTimeKey = "AutoCloseoutRunTime";
		private static readonly string RecordVersionPropagationEnabledKey = "RecordVersionPropagationEnabled";
		private static readonly string RecordVersionPropagationIntervalKey = "RecordVersionPropagationIntervalSeconds";		
		private static readonly string SchedulerEnabledKey = "SchedulerEnabled";
		private static readonly string PointAlarmStatusEnabledKey = "PointAlarmStatusEnabled";
		private static readonly string PointAlarmStatusIntervalKey = "PointAlarmStatusIntervalMinutes";
		private static readonly string StaleOperateSessionCleanupEnabledKey = "StaleOperateSessionCleanupEnabled";
		private static readonly string StaleOperateSessionTimeKey = "StaleOperateSessionTime";
		private static readonly string PointCalculatorRunTableCleanupEnabledKey = "PointCalculatorRunTableCleanupEnabled";
		private static readonly string PointCalculatorRunTableCleanupIntervalMinutesKey = "PointCalculatorRunTableCleanupIntervalMinutes";

		public static int MaxLoginAttempts = 10;

		public static bool UserAccountCleanupEnabled = true;
		public static int UserAccountCleanupInterval = DefaultIntervalMinutes;

		public static bool AuditProcessingEnabled = true;
		public static int AuditProcessingInterval = DefaultIntervalMinutes * 1000;

		public static bool AlarmAndEventProcessingEnabled = true;
		public static int AlarmAndEventProcessingInterval = DefaultIntervalMinutes;

		public static bool AlarmAndEventLogCleanupEnabled = true;
		public static int AlarmAndEventLogCleanupInterval = DefaultIntervalMinutes;

		public static bool FCEEMessagesCleanupEnabled = true;
		public static int FCEEMessagesCleanupInterval = DefaultIntervalMinutes;

		public static bool fceHeartbeatSentryEnabled = true;
		public static int fceHeartbeatSentryTimerInterval = DefaultIntervalMinutes;

		public static bool SessionCleanupEnabled = true;
		public static int SessionCleanupInterval = DefaultIntervalMinutes;

		public static bool FMaePingEnabled = true;
		public static int FMaePingInterval = DefaultIntervalMinutes;

		public static string AccountingAddress = "http://localhost/AccountingImportExport/ImportService.asmx";

		public static bool AutoCloseoutEnabled = false;
		public static int AutomaticCloseoutIntervalMinutes = DefaultIntervalMinutes;
		public static string AutoCloseoutRunTimeString = "1:00 AM";

		public static bool RecordVersionPropagationEnabled = false;
		public static int RecordVersionPropagationIntervalSeconds = DefaultRecordVersionPropagationIntervalSeconds;

		public static bool SchedulerEnabled = true;

		public static bool PointAlarmStatusEnabled = true;
		public static int PointAlarmStatusInterval = DefaultIntervalMinutes;

		public static bool StaleOperateSessionCleanupEnabled = true;
		public static int StaleOperateSessionTime = 60; //seconds

		public static bool PointCalculatorRunTableCleanupEnabled = true;
		public static int PointCalculatorRunTableCleanupIntervalMinutes = 1440; //Minutes

		public static void LoadConfigFile()
		{
			MaxLoginAttempts = AppSettingsHelper.GetKeyValue(MaxLoginAttemptsKey, 10);

			UserAccountCleanupEnabled = AppSettingsHelper.GetKeyValue(UserAccountCleanupEnabledKey, true);
			UserAccountCleanupInterval = AppSettingsHelper.GetKeyValue(UserAccountCleanupIntervalKey, DefaultIntervalMinutes);

			AuditProcessingEnabled = AppSettingsHelper.GetKeyValue(AuditProcessingEnabledKey, true);
			AuditProcessingInterval = AppSettingsHelper.GetKeyValue(AuditProcessingIntervalKey, DefaultIntervalMinutes);

			AlarmAndEventProcessingEnabled = AppSettingsHelper.GetKeyValue(AlarmAndEventProcessingEnabledKey, true);
			AlarmAndEventProcessingInterval = AppSettingsHelper.GetKeyValue(AlarmAndEventProcessingIntervalKey, DefaultIntervalMinutes);

			AlarmAndEventLogCleanupEnabled = AppSettingsHelper.GetKeyValue(AlarmAndEventLogCleanupEnabledKey, true);
			AlarmAndEventLogCleanupInterval = AppSettingsHelper.GetKeyValue(AlarmAndEventLogCleanupIntervalKey, DefaultIntervalMinutes);

			FCEEMessagesCleanupEnabled = AppSettingsHelper.GetKeyValue(FCEEMessagesCleanupEnabledKey, true);
			FCEEMessagesCleanupInterval = AppSettingsHelper.GetKeyValue(FCEEMessagesCleanupIntervalKey, DefaultIntervalMinutes);

			fceHeartbeatSentryEnabled = AppSettingsHelper.GetKeyValue(fceHeartbeatSentryEnabledKey, true);
			fceHeartbeatSentryTimerInterval = AppSettingsHelper.GetKeyValue(fceHeartbeatSentryIntervalKey, DefaultIntervalMinutes);

			SessionCleanupEnabled = AppSettingsHelper.GetKeyValue(SessionCleanupEnabledKey, true);
			SessionCleanupInterval = AppSettingsHelper.GetKeyValue(SessionCleanupIntervalKey, DefaultIntervalMinutes);
			
			FMaePingEnabled = AppSettingsHelper.GetKeyValue<bool>(FMaePingEnabledKey, true);
			FMaePingInterval = AppSettingsHelper.GetKeyValue(FMaePingIntervalKey, DefaultIntervalMinutes);

			AccountingAddress = AppSettingsHelper.GetKeyValue("FuelsManager_AccountingImportService_ImportService", AccountingAddress);

			AutoCloseoutEnabled = AppSettingsHelper.GetKeyValue(AutoCloseoutEnabledKey, false);
			AutomaticCloseoutIntervalMinutes = AppSettingsHelper.GetKeyValue(AutoCloseoutPollIntervalKey, DefaultIntervalMinutes);
			AutoCloseoutRunTimeString = AppSettingsHelper.GetKeyValue(AutoCloseoutRunTimeKey, "1:00 AM");

			RecordVersionPropagationEnabled = AppSettingsHelper.GetKeyValue(RecordVersionPropagationEnabledKey, false);
			RecordVersionPropagationIntervalSeconds = AppSettingsHelper.GetKeyValue(RecordVersionPropagationIntervalKey, DefaultRecordVersionPropagationIntervalSeconds);

			SchedulerEnabled = AppSettingsHelper.GetKeyValue(SchedulerEnabledKey, false);

			PointAlarmStatusEnabled = AppSettingsHelper.GetKeyValue<bool>(PointAlarmStatusEnabledKey, true);
			PointAlarmStatusInterval = AppSettingsHelper.GetKeyValue(PointAlarmStatusIntervalKey, DefaultIntervalMinutes);

			StaleOperateSessionCleanupEnabled = AppSettingsHelper.GetKeyValue(StaleOperateSessionCleanupEnabledKey, true);
			StaleOperateSessionTime = AppSettingsHelper.GetKeyValue(StaleOperateSessionTimeKey, 60);

			PointCalculatorRunTableCleanupEnabled = AppSettingsHelper.GetKeyValue(PointCalculatorRunTableCleanupEnabledKey, true);
			PointCalculatorRunTableCleanupIntervalMinutes = AppSettingsHelper.GetKeyValue(PointCalculatorRunTableCleanupIntervalMinutesKey, 1440);
		}

		public static void SaveConfigFile(string exePath)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);

			config.AppSettings.Settings.Remove(MaxLoginAttemptsKey);
			config.AppSettings.Settings.Add(MaxLoginAttemptsKey, MaxLoginAttempts.ToString());

			config.AppSettings.Settings.Remove(UserAccountCleanupEnabledKey);
			config.AppSettings.Settings.Add(UserAccountCleanupEnabledKey, UserAccountCleanupEnabled.ToString());

			config.AppSettings.Settings.Remove(UserAccountCleanupIntervalKey);
			config.AppSettings.Settings.Add(UserAccountCleanupIntervalKey, UserAccountCleanupInterval.ToString());

			config.AppSettings.Settings.Remove(AuditProcessingEnabledKey);
			config.AppSettings.Settings.Add(AuditProcessingEnabledKey, AuditProcessingEnabled.ToString());

			config.AppSettings.Settings.Remove(AuditProcessingIntervalKey);
			config.AppSettings.Settings.Add(AuditProcessingIntervalKey, AuditProcessingInterval.ToString());

			config.AppSettings.Settings.Remove(AlarmAndEventProcessingEnabledKey);
			config.AppSettings.Settings.Add(AlarmAndEventProcessingEnabledKey, AlarmAndEventProcessingEnabled.ToString());

			config.AppSettings.Settings.Remove(AlarmAndEventProcessingIntervalKey);
			config.AppSettings.Settings.Add(AlarmAndEventProcessingIntervalKey, AlarmAndEventProcessingInterval.ToString());

			config.AppSettings.Settings.Remove(AlarmAndEventLogCleanupEnabledKey);
			config.AppSettings.Settings.Add(AlarmAndEventLogCleanupEnabledKey, AlarmAndEventLogCleanupEnabled.ToString());

			config.AppSettings.Settings.Remove(AlarmAndEventLogCleanupIntervalKey);
			config.AppSettings.Settings.Add(AlarmAndEventLogCleanupIntervalKey, AlarmAndEventLogCleanupInterval.ToString());

			config.AppSettings.Settings.Remove(FCEEMessagesCleanupEnabledKey);
			config.AppSettings.Settings.Add(FCEEMessagesCleanupEnabledKey, FCEEMessagesCleanupEnabled.ToString());

			config.AppSettings.Settings.Remove(FCEEMessagesCleanupIntervalKey);
			config.AppSettings.Settings.Add(FCEEMessagesCleanupIntervalKey, FCEEMessagesCleanupInterval.ToString());

			config.AppSettings.Settings.Remove(fceHeartbeatSentryEnabledKey);
			config.AppSettings.Settings.Add(fceHeartbeatSentryEnabledKey, fceHeartbeatSentryEnabled.ToString());

			config.AppSettings.Settings.Remove(fceHeartbeatSentryIntervalKey);
			config.AppSettings.Settings.Add(fceHeartbeatSentryIntervalKey, fceHeartbeatSentryTimerInterval.ToString());

			config.AppSettings.Settings.Remove(SessionCleanupEnabledKey);
			config.AppSettings.Settings.Add(SessionCleanupEnabledKey, SessionCleanupEnabled.ToString());

			config.AppSettings.Settings.Remove(SessionCleanupIntervalKey);
			config.AppSettings.Settings.Add(SessionCleanupIntervalKey, SessionCleanupInterval.ToString());

			config.AppSettings.Settings.Remove(FMaePingEnabledKey);
			config.AppSettings.Settings.Add(FMaePingEnabledKey, FMaePingEnabled.ToString());

			config.AppSettings.Settings.Remove(FMaePingIntervalKey);
			config.AppSettings.Settings.Add(FMaePingIntervalKey, FMaePingInterval.ToString());

			config.AppSettings.Settings.Remove(AutoCloseoutEnabledKey);
			config.AppSettings.Settings.Add(AutoCloseoutEnabledKey, AutoCloseoutEnabled.ToString());

			config.AppSettings.Settings.Remove(AutoCloseoutPollIntervalKey);
			config.AppSettings.Settings.Add(AutoCloseoutPollIntervalKey, AutomaticCloseoutIntervalMinutes.ToString());

			config.AppSettings.Settings.Remove(AutoCloseoutRunTimeKey);
			config.AppSettings.Settings.Add(AutoCloseoutRunTimeKey, AutoCloseoutRunTimeString);

			config.AppSettings.Settings.Remove(RecordVersionPropagationEnabledKey);
			config.AppSettings.Settings.Add(RecordVersionPropagationEnabledKey, RecordVersionPropagationEnabled.ToString());

			config.AppSettings.Settings.Remove(RecordVersionPropagationIntervalKey);
			config.AppSettings.Settings.Add(RecordVersionPropagationIntervalKey, RecordVersionPropagationIntervalSeconds.ToString());

			config.AppSettings.Settings.Remove(PointAlarmStatusIntervalKey);
			config.AppSettings.Settings.Add(PointAlarmStatusIntervalKey, PointAlarmStatusInterval.ToString());

			config.AppSettings.Settings.Remove(StaleOperateSessionCleanupEnabledKey);
			config.AppSettings.Settings.Add(StaleOperateSessionCleanupEnabledKey, StaleOperateSessionCleanupEnabled.ToString());
			config.AppSettings.Settings.Remove(StaleOperateSessionTimeKey);
			config.AppSettings.Settings.Add(StaleOperateSessionTimeKey, StaleOperateSessionTime.ToString());

			config.AppSettings.Settings.Remove(PointCalculatorRunTableCleanupEnabledKey);
			config.AppSettings.Settings.Add(PointCalculatorRunTableCleanupEnabledKey, PointCalculatorRunTableCleanupEnabled.ToString());
			config.AppSettings.Settings.Remove(PointCalculatorRunTableCleanupIntervalMinutesKey);
			config.AppSettings.Settings.Add(PointCalculatorRunTableCleanupIntervalMinutesKey, PointCalculatorRunTableCleanupIntervalMinutes.ToString());

			config.Save(ConfigurationSaveMode.Modified);
		}
	}

	/// <summary>
	/// A service responsible for background processing associated with FuelsManager
	/// </summary>
	public partial class FuelsManagerService : ServiceBase
	{
		///// <summary>
		///// The well-known Guid which identifies the Administrative Site in FuelsManager.
		///// We have to know the Site Admin Guid to login. 
		///// </summary>
		//private readonly Guid siteAdminGuid = new Guid("00000000-0000-0000-0000-000000000001");

		/// <summary>
		/// A security object used to interact with FMBusinessServices
		/// </summary>
		private SecurityClass security;

		/// <summary>
		/// A host for the WCF service which receives communication from the FuelsManager application 
		/// </summary>
		private ServiceHost fuelsManagerServiceCommunicationHost;

		/// <summary>
		/// The time the automatic closeout process is configured to run
		/// </summary>
		private DateTime automaticCloseoutRunTime;

		/// <summary>
		/// A timer that periodically fires to cleanup expired session records.
		/// </summary>
		private Timer sessionCleanupTimer;

		/// <summary>
		/// A timer that periodically pings the FuelsManager Accounting Import / Export web service.
		/// </summary>
		private Timer fmaePingTimer;

		/// <summary>
		/// A timer that periodically checks for Point Alarm Status.
		/// </summary>
		private readonly Timer pointAlarmStatusTimer;
		/// <summary>
		/// A timer that periodically fires to cleanup old alarm and event log records.
		/// </summary>
		private Timer alarmAndEventLogCleanupTimer;

		/// <summary>
		/// A timer that periodically fires to cleanup old FCEE messages.
		/// </summary>
		private Timer fceeMessagesCleanupTimer;

		/// <summary>
		/// A timer that periodically fires to check FCE Device heartbeats.
		/// </summary>
		private Timer fceHeartbeatSentryTimer;

		/// <summary>
		/// A timer that periodically fires to perform automatic closeouts.
		/// </summary>
		private Timer automaticCloseoutTimer;
		/// <summary>
		/// A flag that indicates whether or not the database reindexing timer will operate.
		/// </summary>
		private bool reindexTimerEnabled;

		/// <summary>
		/// A timer that reindexes the base-level database at a configured time.
		/// </summary>
		private System.Threading.Timer databaseReindexingTimer;

		/// <summary>
		/// A timer that checks the reindexing configuration.
		/// </summary>
		private System.Threading.Timer checkReindexingConfigurationTimer;

		/// <summary>
		/// A flag that indicates whether or not we should propagate global changes from child records to master records.
		/// This flag should only be set at an Enterprise system, not a Terminal/Base system.
		/// </summary>
		/// <remarks>
		/// Set to false for terminal deployments.
		/// </remarks>

		/// <summary>
		/// A timer that checks for and propagates global changes in child records at Enterprise (entity record versioning).
		/// </summary>
		private Timer recordVersionPropagationTimer;

		private static readonly string GlobalFieldsProcessingInhibitTimeThresholdMinutesKey = "GlobalFieldsProcessingInhibitTimeThresholdMinutes";

		private DateTimeOffset LastGlobalFieldsInhibitEventEntryDT = DateTimeOffset.MinValue;

		private TimeSpan reindexScheduledTime;
		private static readonly AutoResetEvent EventRun = new AutoResetEvent(false);
		static bool doTerminate;
		private Thread threadReindex;
		private readonly bool doLogInfo;
		private StreamWriter debugLogger;

		private bool isReindexRunning;
		public bool IsReindexRunning
		{
			get { lock (this) { return this.isReindexRunning; } }
			set { lock (this) { this.isReindexRunning = value; } }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FuelsManagerService"/> class.
		/// </summary>
		public FuelsManagerService()
		{
			this.AutoLog = false;
			this.doLogInfo = false;
			this.CanShutdown = true;
			this.InitializeComponent();
		}

		/// <summary>
		/// Login to FuelsManager 
		/// </summary>
		/// <returns>True if the login was successful</returns>
		private bool LoginToFuelsManager()
		{
			try
			{
				SecurityClass loginSecurity = new SecurityClass
				{
					UserGuid = Guid.Empty,
					LoginSiteGuid = Guids.SiteAdminGuid,
					SiteGuid = Guids.SiteAdminGuid
				};

				loginSecurity.UserID = FMChannelHelper.MakeCall<IDBAccess, string>(fuelsManagerDatabaseAccess => fuelsManagerDatabaseAccess.ServiceLogin(loginSecurity));

				loginSecurity.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
				loginSecurity.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
				loginSecurity.AddRight(RIGHT.VIEW_USERS);

				this.security = loginSecurity;

				return true;
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
				return false;
			}
		}

		/// <summary>
		/// Executes when a start command is sent to the service
		/// </summary>
		/// <param name="args">
		/// Data Passed by the start command
		/// </param>
		protected override void OnStart(string[] args)
		{
			FuelsManagerSettings.LoadConfigFile();
			this.Start();
		}

		/// <summary>
		/// Executes when a stop command is sent to the service
		/// </summary>
		protected override void OnStop()
		{
			this.CleanupTimer(this.databaseReindexingTimer);

			this.CleanupTimer(this.checkReindexingConfigurationTimer);

			doTerminate = true;
			EventRun.Set();

			this.Exit();
		}

		/// <summary>
		/// Start the FuelsManager Service
		/// </summary>
		public void Start()
		{
			try
			{
				if (this.alarmAndEventLogCleanupTimer != null)
				{
					this.alarmAndEventLogCleanupTimer.Stop();
					this.alarmAndEventLogCleanupTimer.Close();
				}

				if (this.fceeMessagesCleanupTimer != null)
				{
					this.fceeMessagesCleanupTimer.Stop();
					this.fceeMessagesCleanupTimer.Close();
				}

				if (this.fceHeartbeatSentryTimer != null)
				{
					this.fceHeartbeatSentryTimer.Stop();
					this.fceHeartbeatSentryTimer.Close();
				}

				if (this.sessionCleanupTimer != null)
				{
					this.sessionCleanupTimer.Stop();
					this.sessionCleanupTimer.Close();
				}

				if (this.fmaePingTimer != null)
				{
					this.fmaePingTimer.Stop();
					this.fmaePingTimer.Close();
				}

				if (this.automaticCloseoutTimer != null)
				{
					this.automaticCloseoutTimer.Stop();
					this.automaticCloseoutTimer.Close();
				}

				if (this.pointAlarmStatusTimer != null)
				{
					this.pointAlarmStatusTimer.Stop();
					this.pointAlarmStatusTimer.Close();
				}

				this.fuelsManagerServiceCommunicationHost?.Close();

				FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

				// Keep trying to login until the login is successful
				int maxAttempts = FuelsManagerSettings.MaxLoginAttempts;
				var retryCount = 0;
				while (!this.LoginToFuelsManager())
				{
					if (++retryCount > maxAttempts)
					{
						throw new ApplicationException("Login to FuelsManager failed after" + maxAttempts + " retries.");
					}

					Thread.Sleep(5000);
				}

				this.fuelsManagerServiceCommunicationHost = new ServiceHost(typeof(FuelsManagerServiceCommunication));

				this.fuelsManagerServiceCommunicationHost.Open();

				try
				{
					if (FuelsManagerSettings.UserAccountCleanupEnabled)
					{
						UserAccountCleanup.StartProcessThread(this.security, TimeSpan.FromMinutes(FuelsManagerSettings.UserAccountCleanupInterval));
					}
				}
				catch (Exception ex)
				{
					FuelsManagerServiceLogger.Instance.LogError(ex);
				}

				if (FuelsManagerSettings.AuditProcessingEnabled)
				{
					AuditProcessing.StartProcessThread(this.security, TimeSpan.FromMilliseconds(FuelsManagerSettings.AuditProcessingInterval));
				}

				FMLicenseProcessing.StartProcessThread(this.security, this.EventLog);

				if (FuelsManagerSettings.AlarmAndEventProcessingEnabled)
				{
					AlarmAndEventProcessing.StartProcessThread(this.security, TimeSpan.FromMinutes(FuelsManagerSettings.AlarmAndEventProcessingInterval));
				}

				// Start the timer responsible for deleting old alarm and event log records
				if (FuelsManagerSettings.AlarmAndEventLogCleanupEnabled)
				{
					this.alarmAndEventLogCleanupTimer = new Timer(TimeSpan.FromMinutes(FuelsManagerSettings.AlarmAndEventLogCleanupInterval).TotalMilliseconds);
					this.alarmAndEventLogCleanupTimer.Elapsed += this.AlarmAndEventLogCleanupTimerElapsed;

					// No need to make an immediate call.  Let alarm and event handling proceed with configured interval.
					this.alarmAndEventLogCleanupTimer.Start();
				}

				// Start the timer responsible for deleting old FCEE messages
				if (FuelsManagerSettings.FCEEMessagesCleanupEnabled)
				{
					this.fceeMessagesCleanupTimer = new Timer(TimeSpan.FromMinutes(FuelsManagerSettings.FCEEMessagesCleanupInterval).TotalMilliseconds);
					this.fceeMessagesCleanupTimer.Elapsed += this.FCEEMessagesCleanupTimerElapsed;

					// No need to make an immediate call.	Let FCEE messages handling proceed with configured interval.
					this.fceeMessagesCleanupTimer.Start();
				}

				// Start the timer responsible for check FCE Device Heartbeats
				if (FuelsManagerSettings.fceHeartbeatSentryEnabled)
				{
					this.fceHeartbeatSentryTimer = new Timer(TimeSpan.FromMinutes(FuelsManagerSettings.fceHeartbeatSentryTimerInterval).TotalMilliseconds);
					this.fceHeartbeatSentryTimer.Elapsed += this.fceHeartbeatSentryTimerElapsed;

					// No need to make an immediate call.	Let Fthe sentry proceed with configured interval.
					this.fceHeartbeatSentryTimer.Start();
				}

				// Start the timer responsible for deleting expired session records
				if (FuelsManagerSettings.SessionCleanupEnabled)
				{

					this.sessionCleanupTimer = new Timer(TimeSpan.FromMinutes(FuelsManagerSettings.SessionCleanupInterval).TotalMilliseconds);
					this.sessionCleanupTimer.Elapsed += this.SessionCleanupTimerElapsed;

					// Make an immediate call to do cleanup on startup then start timer to follow-up regularly.
					this.SessionCleanupTimerElapsed(null, null);
					this.sessionCleanupTimer.Start();
				}

				if (FuelsManagerSettings.FMaePingEnabled)
				{
					this.StartFmaePingTimer();
				}

				if (FuelsManagerSettings.PointAlarmStatusEnabled)
				{
					PointAlarmStatusProcessing.StartProcessThread(this.security, this.EventLog);
				}

				if (FuelsManagerSettings.SchedulerEnabled)
				{
					FMScheduler.StartProcessThread(this.security);
				}

				if (FuelsManagerSettings.AutoCloseoutEnabled)
				{
					if (!DateTime.TryParse(FuelsManagerSettings.AutoCloseoutRunTimeString, out this.automaticCloseoutRunTime))
					{
						throw new Exception("AutoCloseoutRunTime application setting " + FuelsManagerSettings.AutoCloseoutRunTimeString + " is not a valid time");
					}

					this.automaticCloseoutTimer = new Timer(new TimeSpan(0, 0, FuelsManagerSettings.AutomaticCloseoutIntervalMinutes, 0).TotalMilliseconds);
					this.automaticCloseoutTimer.Elapsed += this.AutomaticCloseoutTimerElapsed;

					this.automaticCloseoutTimer.Start();
				}

				if (FuelsManagerSettings.StaleOperateSessionCleanupEnabled)
				{
					InactiveOperateSessionCleanup.StartProcessThread(this.security, TimeSpan.FromSeconds(10)); // run on a 10 second interval; this should provide enough resolution for cleanup.
				}

				if (FuelsManagerSettings.PointCalculatorRunTableCleanupEnabled)
				{
					CleanupPointCalculatorTables.StartProcessThread(this.security, TimeSpan.FromMinutes(1)); // run once a minute; this should provide enough resolution for cleanup.
				}

				this.recordVersionPropagationTimer = new Timer(new TimeSpan(0, 0, 0, FuelsManagerSettings.RecordVersionPropagationIntervalSeconds).TotalMilliseconds);
				this.recordVersionPropagationTimer.Elapsed += this.RecordVersionPropagationTimerElapsed;

				try
				{
					bool enterpriseKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(hardwareKey => hardwareKey.IsMultipleSiteKey());
					if (enterpriseKey && FuelsManagerSettings.RecordVersionPropagationEnabled)
					{
						this.recordVersionPropagationTimer.Start();
					}
				}
				catch (Exception ex)
				{
					FuelsManagerServiceLogger.Instance.LogError(ex);
				}

				this.threadReindex = null;
				this.threadReindex = new Thread(RunReindex);
				if (this.threadReindex != null)
				{
					this.ReadReindexingConfiguration();
					this.threadReindex.Start(this);
				}

				this.StartCheckReindexingConfigurationTimer();

				this.EventLog.WriteEntry("FuelsManager Service Started", EventLogEntryType.Information);
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
				Exit();
				Environment.Exit(1);
			}
		}

		/// <summary>
		/// Create a timer to perform database reindexing at a specific time using TimerProc().
		/// </summary>
		private void StartDatabaseReindexTimer()
		{
			if (this.IsReindexRunning)
			{
				return;
			}

			this.CleanupTimer(this.databaseReindexingTimer);

			var now = DateTime.Now;
			var today = DateTime.Today;
			var dtStart = today + this.reindexScheduledTime;

			if (dtStart <= now)
			{
				dtStart += TimeSpan.FromDays(1);
			}

			// Calculate the difference between the specified execution time and the current time.
			var tsWait = dtStart - now;

			this.databaseReindexingTimer = new System.Threading.Timer(this.ReindexTimerCallback, this, tsWait, TimeSpan.FromDays(1));

			var timeToExecute = now.Add(tsWait).TimeOfDay;
			var message = $"Reindexing will begin at {timeToExecute:c} in {tsWait.Hours}h {tsWait.Minutes}m {tsWait.Seconds}s";
			this.LogInfo("StartDatabaseReindexingTimer", message);
		}

		private void StartCheckReindexingConfigurationTimer()
		{
			var delay = new TimeSpan(0, 0, 1);  // 1 second
															//var interval = new TimeSpan(0,0,13);  // 13 seconds for testing
			var interval = new TimeSpan(0, 59, 0);  // 59 minutes for real

			this.checkReindexingConfigurationTimer = new System.Threading.Timer(this.CheckReindexingEnabledCallback, this, delay, interval);
		}

		private void CheckReindexingEnabledCallback(object state)
		{
			if (this.IsReindexRunning == false)
			{
				try
				{
					this.ReadReindexingConfiguration();

					if (this.IsReindexEnabled())
					{
						this.StartDatabaseReindexTimer();
					}
					else
					{
						this.CleanupTimer(this.databaseReindexingTimer);
					}
				}
				catch (Exception ex)
				{
					this.LogInfo("CheckReindexingEnabled catch", ex.Message);
					this.EventLog.WriteEntry("CheckReindexingEnabled Error: " + ex.Message, EventLogEntryType.Error);
				}
			}
		}

		private void ReadReindexingConfiguration()
		{
			try
			{
				var reindexEnabledString = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.security, ConfigurationSettingDOClass.Key_FMService_ReindexEnabled));
				this.reindexTimerEnabled = reindexEnabledString.Equals("1");

				if (this.IsReindexEnabled() == false)
				{
					var timerEnabledMessage = $"Database reindexing timer is {(this.reindexTimerEnabled ? "enabled" : "disabled")} ";
					this.LogInfo("ReadReindexingConfiguration", timerEnabledMessage);
				}

				var reindexScheduledTimeString = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.security, ConfigurationSettingDOClass.Key_FMService_ReindexScheduledTime));
				var defaultScheduledTime = new TimeSpan(3, 0, 0);     // 3:00 AM
				this.reindexScheduledTime = reindexScheduledTimeString.ToTimeSpan(defaultScheduledTime);
			}
			catch (Exception ex)
			{
				this.EventLog.WriteEntry("ReadReindexingConfiguration Error: " + ex.Message, EventLogEntryType.Error);
			}
		}

		private bool IsReindexEnabled()
		{
			return this.reindexTimerEnabled;
		}

		private void CleanupTimer(System.Threading.Timer timer)
		{
			timer?.Dispose();
		}

		// Callback method that runs on a threadpool thread.
		private void ReindexTimerCallback(object state)
		{
			FuelsManagerService service = (FuelsManagerService)state;

			if (service.IsReindexRunning)
			{
				service.LogInfo("ReindexTimerCallback", "A reindexing session is already in progress.");
				return;
			}

			EventRun.Set();
		}

		// Thread method.
		private static void RunReindex(object obj)
		{
			FuelsManagerService service = (FuelsManagerService)obj;

			while (!doTerminate)
			{
				try
				{
					// Wait here to receive run reindex request.
					EventRun.WaitOne();

					// Request to terminate thread loop may be initiated by OnStop or OnShutdown.
					if (doTerminate)
					{
						break;
					}

					if (service.IsReindexRunning)
					{
						service.LogInfo("RunReindex", "Reindexing is already in progress");
						return;
					}

					service.ReadReindexingConfiguration();

					if (service.IsReindexEnabled())
					{
						// Prevent another run while executing the current reindex process.
						service.IsReindexRunning = true;

						try
						{
							service.LogInfo("RunReindex", "Reindex initiated");
							service.EventLog.WriteEntry("RunReindex: Reindex initiated", EventLogEntryType.Information);

							FMChannelHelper.MakeCall<IDatabaseMaintenance>(
								databaseMaintenance =>
								{
									((IClientChannel)databaseMaintenance).OperationTimeout = new TimeSpan(0, 30, 0);
									databaseMaintenance.ReindexDatabase(service.security);
								});

							service.LogInfo("RunReindex", "Reindex completed");
							service.EventLog.WriteEntry("RunReindex: Reindex completed", EventLogEntryType.Information);
						}
						catch (Exception ex)
						{
							service.LogInfo("RunReindex catch 1", ex.Message);
							service.EventLog.WriteEntry("RunReindex Error: " + ex.Message, EventLogEntryType.Error);
							service.IsReindexRunning = false;
							service.CleanupTimer(service.databaseReindexingTimer);
						}
						finally
						{
							// Normal reindex operation completed.
							service.IsReindexRunning = false;
						}
					}
				}
				catch (Exception ex)
				{
					service.LogInfo("RunReindex catch 2", ex.Message);
					service.EventLog.WriteEntry("RunReindex Error: " + ex.Message, EventLogEntryType.Error);
					service.IsReindexRunning = false;
				}
			}
			service.LogInfo("RunReindex", "Terminating.");
		}

		/// <summary>
		/// Starts the FMAE ping timer.
		/// </summary>
		private void StartFmaePingTimer()
		{
			this.fmaePingTimer = new Timer(TimeSpan.FromMinutes(FuelsManagerSettings.FMaePingInterval).TotalMilliseconds);

			this.fmaePingTimer.Elapsed += (sender, args) =>
			{
				try
				{
					var theWebRequest = WebRequest.Create(FuelsManagerSettings.AccountingAddress + "/PingApplicationServer");
					theWebRequest.Method = "POST";
					theWebRequest.ContentLength = 0;
					theWebRequest.Headers.Add(HttpRequestHeader.Pragma.ToString(), "no-cache");
					theWebRequest.GetResponse();
				}
				catch (Exception except)
				{
					this.EventLog.WriteEntry("FMAE Ping Error: " + except.Message, EventLogEntryType.Error);
				}
			};

			this.fmaePingTimer.Start();
		}

		public void LogInfo(string caller, string info)
		{
			this.LogInfo(caller, info, DateTime.Now);
		}

		// Log trace info to a text file to aid development.
		public void LogInfo(string caller, string info, DateTime dateTime)
		{
			if (this.doLogInfo == false)
			{
				return;
			}

			if (this.debugLogger == null)
			{
				this.debugLogger = this.GetDebugLogger(dateTime);
			}

			const string LogDateTimeFormat = "yyyy/MM/dd HH:mm:ss";

			if (this.debugLogger != null)
			{
				try
				{
					string str;
					if (caller.Length == 0)
						str = $"{dateTime.ToString(LogDateTimeFormat)} {info}";
					else
						str = $"{dateTime.ToString(LogDateTimeFormat)} [{caller}] {info}";

					this.debugLogger.WriteLine(str);
				}
				catch (Exception ex)
				{
					this.EventLog.WriteEntry(ex.Message);
				}
			}
		}

		private StreamWriter GetDebugLogger(DateTime dateTime)
		{
			try
			{
				if (this.debugLogger == null)
				{
					// In Windows Service, must provide full path to FileStream.
					var dir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
					var fileName = $"FuelsManagerService-{dateTime.ToString("yyyyMMdd-HHmmss")}.log";
					if (dir != null)
					{
						var fullPath = Path.Combine(dir, fileName);
						var fileStream = new FileStream(fullPath, FileMode.Append, FileAccess.Write);
						this.debugLogger = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
					}
					// Make StreamWriter flush its buffer to the underlying stream after every call to StreamWriter.Write().
				}
			}
			catch (Exception ex)
			{
				this.EventLog.WriteEntry(ex.Message);
			}
			return this.debugLogger;
		}

		/// <summary>
		/// Stop the FuelsManager Service
		/// </summary>
		public void Exit()
		{
			try
			{
				UserAccountCleanup.StopProcessThread();

				AlarmAndEventProcessing.StopProcessThread();

				AuditProcessing.StopProcessThread();

				FMLicenseProcessing.StopProcessThread();

				PointAlarmStatusProcessing.StopProcessThread();

				FMScheduler.StopProcessThread();

				CleanupPointCalculatorTables.StopProcessThread();

				if (this.alarmAndEventLogCleanupTimer != null)
				{
					this.alarmAndEventLogCleanupTimer.Stop();
					this.alarmAndEventLogCleanupTimer.Close();
				}

				if (this.fceeMessagesCleanupTimer != null)
				{
					this.fceeMessagesCleanupTimer.Stop();
					this.fceeMessagesCleanupTimer.Close();
				}

				if (this.sessionCleanupTimer != null)
				{
					this.sessionCleanupTimer.Stop();
					this.sessionCleanupTimer.Close();
				}

				if (this.automaticCloseoutTimer != null)
				{
					this.automaticCloseoutTimer.Stop();
					this.automaticCloseoutTimer.Close();
				}

				this.fuelsManagerServiceCommunicationHost?.Close();

				this.EventLog.WriteEntry("FuelsManager Service Stopped", EventLogEntryType.Information);
			}
			catch (Exception ex)
			{
				this.fuelsManagerServiceCommunicationHost?.Abort();

				FuelsManagerServiceLogger.Instance.LogError(ex);
			}
		}

		/// <summary>
		/// When the alarm and event log cleanup timer ticks, run a stored procedure to delete old records
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void AlarmAndEventLogCleanupTimerElapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmsAndEvents => alarmsAndEvents.PurgeOldRecords(this.security));
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
			}
		}

		/// <summary>
		/// When the FCEE messages cleanup timer ticks, run a stored procedure to delete old records
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void FCEEMessagesCleanupTimerElapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IFCEEServiceManager>(fceeMessages => fceeMessages.PurgeOldRecords(this.security));
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
			}
		}


		/// <summary>
		/// When the FCE Heartbeat sentry cleanup timer ticks, run a stored procedure to delete old records
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void fceHeartbeatSentryTimerElapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IFCEEServiceManager>(fcee => fcee.ProcessFceHeartbeats(this.security));
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
			}
		}

		/// <summary>
		/// Delete expired session records when the session cleanup timer ticks
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void SessionCleanupTimerElapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<ISessions>(x => x.CleanupExpiredUserSessions(this.security));
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
			}
		}

		/// <summary>
		/// When the automatic closeout timer ticks, check to see if the automatic closeout 
		/// process needs to be run and run the process.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void AutomaticCloseoutTimerElapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				// Check to see if the auto closeout process should run 
				// The process should only run once a day.
				if (AutoCloseout.ShouldAutoCloseoutRun(this.automaticCloseoutRunTime))
				{
					AutoCloseout.PerformAutoCloseouts(this.security);
				}
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
			}
		}

		/// <summary>
		/// When the record version propagation timer ticks, check to see if the any 
		/// requests are present in the GlobalSpecificChanges queue and apply them. 
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void RecordVersionPropagationTimerElapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				var ervProcessSettings = FMChannelHelper.MakeCall<IFieldLevelConfigMaps, ERVProcessSettingsClass>(x => x.GetProcessSettings(this.security));

				if ((ervProcessSettings != null) && (!ervProcessSettings.InhibitGlobalFieldsProcessing))
				{
					RecordVersionPropagation.PerformRecordVersionPropagation(this.security);
				}
				else if ((ervProcessSettings != null) && (ervProcessSettings.InhibitGlobalFieldsProcessing))
				{
					int globalFieldsProcessingInhibitTimeThresholdMinutes = AppSettingsHelper.GetKeyValue(GlobalFieldsProcessingInhibitTimeThresholdMinutesKey, 0);
					int inhibitEllapsedTime = Convert.ToInt32((DateTimeOffset.Now.Subtract(ervProcessSettings.UpdatedDate)).TotalMinutes);
					if ((ervProcessSettings.UpdatedDate != null) && (inhibitEllapsedTime > globalFieldsProcessingInhibitTimeThresholdMinutes))
					{
						//Add a new entry to the Alarm and Event log every globalFieldsProcessingInhibitTimeThresholdMinutes, as long as the inhibit flag is set
						if (Convert.ToInt32((DateTimeOffset.Now.Subtract(LastGlobalFieldsInhibitEventEntryDT)).TotalMinutes) > globalFieldsProcessingInhibitTimeThresholdMinutes)
						{
							string msg = "Global Fields processing inhibited for more than " + Convert.ToString(inhibitEllapsedTime) + " minutes";
							ERVAlarmAndEventClass ervAlarmEvent = new ERVAlarmAndEventClass();
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
								alarmAndEventService => alarmAndEventService.Add(this.security, ervAlarmEvent.ERVAlarmAndEvent(ERVAlarmAndEventClass.GlobalFieldsInhibitTimeThresholdEventDescriptor, msg)));
							LastGlobalFieldsInhibitEventEntryDT = DateTimeOffset.Now;
						}
					}
				}
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
			}
		}
	}
}
