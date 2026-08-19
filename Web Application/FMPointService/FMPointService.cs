// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsPointService.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	A service responsible for execution of point within FuelsManager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMPointService
{
	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using System.Management;
	using System.ServiceProcess;
	using System.ServiceModel;
	using System.Threading.Tasks;
	using System.Xml;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using Archiving;
	using AlarmAndEventArchive;
	using Logging;
	using OpcClient;
	using PointExecution;
	using WcfPointService;
	using ThreadSupport;
	using InProcLogging;
	using FMPointCommon;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using Opc.Ua;
	using Softing.Opc.Ua.Configuration;
	using Softing.Opc.Ua.Client;

   public class FMServiceHost : System.ServiceModel.ServiceHost
	{
		public FMServiceHost(string host, string port, Type svc, params Uri[] addresses) : base(svc, addresses)
		{
         if (!string.IsNullOrEmpty((host))
			&& this.BaseAddresses.Count > 0)
			{
				foreach (var endpoint in base.Description.Endpoints)
				{
               endpoint.Address = new EndpointAddress(new Uri(endpoint.Address.ToString().Replace(this.BaseAddresses[0].Host, host)));
            }
         }

			if (!string.IsNullOrEmpty(port)
			&& this.BaseAddresses.Count > 0)
			{
				foreach (var endpoint in base.Description.Endpoints)
				{
						endpoint.Address = new EndpointAddress(new Uri(endpoint.Address.ToString().Replace(this.BaseAddresses[0].Port.ToString(), port)));
            }
			}
		}
	}


	public partial class FMPointService : ServiceBase
	{
		public static object SoftingInitializationLock = new object();

		private string port;

		private string host;

		internal static readonly EventLogger EventLogger = new EventLogger();

		private PointProcessor pointProcessor= null;

		private ArchiveProcessor archiveProcessor = null;

		private FMServiceHost pointService;

		private bool HasLeakDetection;
		/// <summary>
		/// A security object used to interact with FMBusinessServices
		/// </summary>
		private SecurityClass security;


		/// <summary>
		/// Initializes a new instance of the <see cref="FMPointService"/> class.
		/// </summary>
		/// <param name="host">The host.</param>
		/// <param name="port">The port.</param>
		public FMPointService(string host, string port)
		{
			this.host = host;
			this.port = port;
			this.AutoLog = false;
			this.CanShutdown = true;
			this.InitializeComponent();
			EventLogger.ServiceName = this.ServiceName;
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
      }

      protected override void OnStart(string[] args)
		{
			this.Start();
		}

		public void Start()
		{

			try
			{
				this.security = new SecurityClass { UserGuid = Guids.UserAdminGuid, SiteGuid = Guids.SiteAdminGuid, SiteID = "SiteAdmin" };
				this.security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
				this.security.UserID = "PointService";

            FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

				// get the lastknowngood configuration
				bool enableUseLastKnownGood = false;
				try
				{
					string stenableUseLastKnownGood = FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
						ConfigurationSettingDOClass.Key_UseLastKnownGoodStatus));
					if(stenableUseLastKnownGood == "1")
					{
						enableUseLastKnownGood = true;
					}
				}
				catch (Exception ex)
				{
					Logger.LogError("EnterpriseVisibilityPushProcessor.GetNumTagsPerSend " + ex.Message);
					enableUseLastKnownGood = false;
				}

				//Start the WCF services
				this.pointService = new FMServiceHost(this.host, this.port, typeof(WcfPointService.PointService));

				if (string.IsNullOrEmpty(this.host) && this.pointService.BaseAddresses.Count > 0)
				{
					this.host = this.pointService.BaseAddresses[0].Host.ToString();
				}

            if (string.IsNullOrEmpty(this.port)
            && this.pointService.BaseAddresses.Count > 0)
            {
               this.port = this.pointService.BaseAddresses[0].Port.ToString();
				}

            Logger.InitializeFileLogger(AppDomain.CurrentDomain.BaseDirectory + "\\" + this.ServiceName + "_" + this.host + "_" + this.port + ".log", 10000, LogSeverity.Debug, System.Threading.ThreadPriority.Normal);
				Logger.LogCritical(this.ServiceName + " Started!!!");
				Logger.Flush();

				this.pointService.Open();

				int maxQueueCount;
				if (!int.TryParse(ConfigurationManager.AppSettings["TimerServiceMaxQueueCount"], out maxQueueCount))
				{
					maxQueueCount = 5000;
				}


				SRMTimerService.Initialize(new TimerSchedulePoint(), maxQueueCount);

				PingProcessor.Instance(this.host, this.port).Start();

				if (ThreadSharedData.Instance().UseOpcUaClientPolling)
				{
					OpcUaClientProcessor2.Instance(this.host, this.port, enableUseLastKnownGood).Start();
				}
				else
				{
					OpcUaClientProcessor.Instance(this.host, this.port, enableUseLastKnownGood).Start();
				}

				if (ThreadSharedData.Instance().EnableArchiveData)
				{
					// start the achive data thread
					this.archiveProcessor = new ArchiveProcessor();
					this.archiveProcessor.Start(this.security);
				}

				// start the alarm and event thread that uses Cassandra
				AlarmAndEventArchiveThread.Initialize(this.security);

				// Start the main point execution processor
				this.pointProcessor = new PointProcessor();
				this.pointProcessor.Start(this.security);

				//Start Enterprise Visibility
				EnterpriseVisibilityPushProcessor.Instance(this.host, this.port).Start();

				// Start Movement Processor
				MovementProcessor.Instance().Start();

				HasLeakDetection = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsLeakDetectionKey());
				if (HasLeakDetection)
				{
					// Start Leak Detection Processor
					LeakDetectionProcessor.Instance().Start();
				}

			}
			catch (Exception ex)
			{
				EventLogger.Error(this.ServiceName + " Error : " + ex.Message);

				Logger.LogCritical("FMPointServer.Start Exception: " + ex.Message);
				Logger.Flush();

				OnStop();
				Environment.Exit(1);
			}

			EventLogger.Info(this.GetServiceName() + " Started");
		}

		protected override void OnStop()
		{
			try
			{
				EnterpriseVisibilityPushProcessor.Instance().Terminate();

				MovementProcessor.Instance().Terminate();
				if (HasLeakDetection)
				{
					LeakDetectionProcessor.Instance().Terminate();
				}

				if (this.pointProcessor != null)
				{
					this.pointProcessor.Stop();
					this.pointProcessor = null;
				}

				PingProcessor.Instance().SignalShutdown();
				PingProcessor.Instance().Terminate();

				if (ThreadSharedData.Instance().UseOpcUaClientPolling)
				{
					OpcUaClientProcessor2.Instance().Terminate();
				}
				else
				{
					OpcUaClientProcessor.Instance().Terminate();
				}

				//Shut down the WCF service
				if (this.pointService != null)
				{
					this.pointService.Close();
				}


				// shutdown the archive thread
				if (this.archiveProcessor != null)
				{
					this.archiveProcessor.Stop();
					this.archiveProcessor = null;
				}

				// shutdown the alrm and event thread
				AlarmAndEventArchiveThread.Term();

				SRMTimerService.Term();

				Logger.LogCritical(this.ServiceName + " Stopping!!!");
				Logger.Flush();
			}
			catch (Exception ex)
			{
				Logger.LogCritical("FMPointServer.OnStop Exception: " + ex.Message);
				Logger.Flush();
			}

			Logger.Shutdown();

			EventLogger.Info(this.GetServiceName() + " Stopped");
		}

		protected String GetServiceName()
		{
			// Calling System.ServiceProcess.ServiceBase::ServiceNamea allways returns
			// an empty string,
			// see https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=387024

			// So we have to do some more work to find out our service name, this only works if
			// the process contains a single service, if there are more than one services hosted
			// in the process you will have to do something else

			int processId = Process.GetCurrentProcess().Id;
			String query = "SELECT * FROM Win32_Service where ProcessId = " + processId;
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

			foreach ( var obj in searcher.Get())
			{
				var queryObj = obj as ManagementObject;
				if (queryObj != null)
				{
					return queryObj["Name"].ToString();
				}
			}

			return "FuelsManager Point Service";
		}


		/// <summary>
		/// Loads the OPC UA Client Configuration
		/// </summary>
		/// <returns></returns>
		public static async Task<ApplicationConfigurationBuilderEx> LoadApplicationConfiguration(string applicationName)
		{
			ApplicationConfigurationBuilderEx applicationConfigurationBuilder = new ApplicationConfigurationBuilderEx(ApplicationType.Client);

			bool autoAcceptUntrustedCertificates;
			if (!Boolean.TryParse(ConfigurationManager.AppSettings["OpcUaAutoAcceptUntrustedCertificates"], out autoAcceptUntrustedCertificates))
			{
				autoAcceptUntrustedCertificates = true;
			}

			int opcUaTraceMasks;
			if (!int.TryParse(ConfigurationManager.AppSettings["OpcUaTraceMasks"], out opcUaTraceMasks))
			{
				opcUaTraceMasks = 1;
			}

			bool deleteOnLoad;
			if (!Boolean.TryParse(ConfigurationManager.AppSettings["OpcUaDeleteOnLoad"], out deleteOnLoad))
			{
				deleteOnLoad = true;
			}

			int sessionTimeout;
			if (!int.TryParse(ConfigurationManager.AppSettings["OpcUaDefaultSessionTimeout"], out sessionTimeout))
			{
				sessionTimeout = 610000;
			}

			int defaultSubscriptionLifeTimeCount;
			if (!int.TryParse(ConfigurationManager.AppSettings["OpcUaDefaultSubscriptionLifeTimeCount"], out defaultSubscriptionLifeTimeCount))
			{
				defaultSubscriptionLifeTimeCount = 11000;
			}

			int operationTimeout;
			if (!int.TryParse(ConfigurationManager.AppSettings["OpcUaOperationTimeout"], out operationTimeout))
			{
				operationTimeout = 120000;
			}

			ushort opcUaCertificateLifeTime;
			if (!UInt16.TryParse(ConfigurationManager.AppSettings["OpcUaCertificateLifeTime"], out opcUaCertificateLifeTime))
			{
				opcUaCertificateLifeTime = 12;
			}



			await applicationConfigurationBuilder
				.Initialize("http://" + System.Environment.MachineName + "/FMPointService",
						"http://" + System.Environment.MachineName + "/FMPointService")
				.SetApplicationName(applicationName)
				.DisableHiResClock(true)
				.SetTransportQuotas(new Opc.Ua.TransportQuotas()
				{
					OperationTimeout = operationTimeout,
					MaxStringLength = 1048576,
					MaxByteStringLength = 4194304,
					MaxArrayLength = 65535,
					MaxMessageSize = 4194304,
					MaxBufferSize = 65535,
					ChannelLifetime = 300000,
					SecurityTokenLifetime = 3600000
				})
				.AsClient()
					.SetDefaultSessionTimeout(sessionTimeout)
					.SetMinSubscriptionLifetime(defaultSubscriptionLifeTimeCount)
					.AddWellKnownDiscoveryUrls("opc.tcp://{0}:4840/UADiscovery")
				.AddSecurityConfigurationExt(
					"Varec Point Service Client",
					"%CommonApplicationData%/Varec/FuelsManager/FMPointService/pki",
					"%CommonApplicationData%/Varec/FuelsManager/FMPointService/pki",
					"%CommonApplicationData%/Varec/FuelsManager/FMPointService/pki")
					.SetRejectSHA1SignedCertificates(false)
					.SetUserRoleDirectory("%CommonApplicationData%/Varec/FuelsManager/FMPointService/userRoles")
				.AddExtension<OpcUaClientConfiguration>(new XmlQualifiedName("OpcUaClientConfiguration"),
					new OpcUaClientConfiguration()
					{
						TimerInterval = 1000,
						ClearCachedCertificatesInterval = 30000
					})
				.AddExtension<ClientToolkitConfiguration>(new XmlQualifiedName("ClientToolkitConfiguration"),
					new ClientToolkitConfiguration()
					{
						DiscoveryOperationTimeout = 10000,
						DecodeCustomDataTypes = true,
						DecodeDataTypeDictionaries = true,
						ClientCertificateLifeTime = opcUaCertificateLifeTime
					})
				.SetTraceMasks(opcUaTraceMasks)
				.SetOutputFilePath("%CommonApplicationData%/Varec/FuelsManager/FMPointService/logs/FMPointService.log")
				.SetDeleteOnLoad(deleteOnLoad)
				.Create().ConfigureAwait(false);


			applicationConfigurationBuilder.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates = autoAcceptUntrustedCertificates;
			applicationConfigurationBuilder.ApplicationConfiguration.SecurityConfiguration.AddAppCertToTrustedStore = true;
			applicationConfigurationBuilder.ApplicationConfiguration.CertificateValidator.AutoAcceptUntrustedCertificates = autoAcceptUntrustedCertificates;

			return applicationConfigurationBuilder;

		}
	}
}
