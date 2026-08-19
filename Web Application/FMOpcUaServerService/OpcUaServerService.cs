// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMOpcUaServerService.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	A service responsible for the OpcUaServer interface to FuelsManager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMOpcUaServerService
{
	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.Management;
	using System.ServiceProcess;
	using System.IO;
	using System.Threading.Tasks;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using System.Xml;
	using Opc.Ua;
	using Softing.Opc.Ua.Configuration;
	using Softing.Opc.Ua.Server;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.BusinessInterfaces;

    public partial class OpcUaServerService : ServiceBase
	{
		private readonly EventLog eventLog;

		private OpcUaServer opcUaServer;


		/// <summary>
		/// A security object used to interact with FMBusinessServices
		/// </summary>
		private SecurityClass security;

		public OpcUaServerService()
		{
			this.AutoLog = false;
			this.CanShutdown = true;
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			this.InitializeComponent();
			this.eventLog = new EventLog("Application", ".", this.ServiceName);
		}

		protected override void OnStart(string[] args)
		{
			this.Start();
		}

		public void Start()
		{
			try
			{

				this.security = new SecurityClass { UserID = "Administrator", UserGuid = Guids.UserAdminGuid, SiteGuid = Guids.SiteAdminGuid, SiteID = "SiteAdmin" };
				this.security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);

				this.opcUaServer = new OpcUaServer(security);


                FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

				ApplicationConfigurationBuilderEx defaultConfiguration = LoadDefaultConfiguration().Result;

				opcUaServer.Start(defaultConfiguration).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				this.eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
				OnStop();
				Environment.Exit(1);
			}

			this.eventLog.WriteEntry(this.GetServiceName() + " Started", EventLogEntryType.Information);
		}

		protected override void OnStop()
		{
			opcUaServer.Stop();
			this.eventLog.WriteEntry(this.GetServiceName() + " Stopped", EventLogEntryType.Information);
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

			foreach (var obj in searcher.Get())
			{
				var queryObj = obj as ManagementObject;
				if (queryObj != null)
				{
					return queryObj["Name"].ToString();
				}
			}

			return "FuelsManager OpcUaServer Service";
		}

		/// <summary>
		/// Load default configuration
		/// </summary>
		/// <returns></returns>
		private static async Task<ApplicationConfigurationBuilderEx> LoadDefaultConfiguration()
		{
			bool autoAcceptUntrustedCertificates;
			if (!bool.TryParse(ConfigurationManager.AppSettings["AutoAcceptUntrustedCertificates"], out autoAcceptUntrustedCertificates))
			{
				autoAcceptUntrustedCertificates = true;
			}

			bool rejectUnknownRevocationStatus;
			if (!bool.TryParse(ConfigurationManager.AppSettings["RejectUnknownRevocationStatus"], out rejectUnknownRevocationStatus))
			{
				rejectUnknownRevocationStatus = false;
			}

			string opcTcpPort = ConfigurationManager.AppSettings["OpcTcpPort"];
			if (string.IsNullOrEmpty(opcTcpPort))
			{
				opcTcpPort = "40003";
			}

			string httpsPort = ConfigurationManager.AppSettings["HttpsPort"];
			if (string.IsNullOrEmpty(httpsPort))
			{
				httpsPort = "40002";
			}

			string reverseConnectURL = ConfigurationManager.AppSettings["ReverseConnectURL"];
			if (string.IsNullOrEmpty(reverseConnectURL))
			{
				reverseConnectURL = "localhost:40004";
			}


			int traceMasks;
			if (!int.TryParse(ConfigurationManager.AppSettings["OpcUaTraceMasks"], out traceMasks))
			{
				traceMasks = 1;
			}

			bool deleteOnLoad;
			if (!Boolean.TryParse(ConfigurationManager.AppSettings["OpcUaDeleteOnLoad"], out deleteOnLoad))
			{
				deleteOnLoad = true;
			}

			ushort opcUaCertificateLifeTime;
			if (!UInt16.TryParse(ConfigurationManager.AppSettings["OpcUaCertificateLifeTime"], out opcUaCertificateLifeTime))
			{
				opcUaCertificateLifeTime = 12;
			}


			ApplicationConfigurationBuilderEx applicationConfigurationBuilder = new ApplicationConfigurationBuilderEx(ApplicationType.Server);

			await applicationConfigurationBuilder
				.Initialize("http://Varec.com/FMOpcUaServer",
						"http://Varec.com/FMOpcUaServer")
				.SetApplicationName("Varec FuelsManager Opc Ua Server")
				.DisableHiResClock(true)
				.SetTransportQuotas(new Opc.Ua.TransportQuotas()
				{
					OperationTimeout = 600000,
					MaxStringLength = 1048576,
					MaxByteStringLength = 1048576,
					MaxMessageSize = 4194304,
					ChannelLifetime = 300000,
				})
				.AsServer(new string[] { "opc.tcp://localhost:" + opcTcpPort + "/FMOpcUaServer"
				, "https://localhost:" + httpsPort + "/FMOpcUaServer" })
					.AddUnsecurePolicyNone()
					.AddSignAndEncryptPolicies()
					.AddPolicy(Opc.Ua.MessageSecurityMode.Sign, "http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256")
					.AddPolicy(Opc.Ua.MessageSecurityMode.SignAndEncrypt, "http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256")
					.AddPolicy(Opc.Ua.MessageSecurityMode.Sign, "http://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep")
					.AddPolicy(Opc.Ua.MessageSecurityMode.SignAndEncrypt, "http://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep")
					.AddPolicy(Opc.Ua.MessageSecurityMode.Sign, "http://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss")
					.AddPolicy(Opc.Ua.MessageSecurityMode.SignAndEncrypt, "http://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss")
					.AddUserTokenPolicy(new Opc.Ua.UserTokenPolicy() { TokenType = Opc.Ua.UserTokenType.Anonymous, SecurityPolicyUri = "http://opcfoundation.org/UA/SecurityPolicy#None" })
					.AddUserTokenPolicy(new Opc.Ua.UserTokenPolicy() { TokenType = Opc.Ua.UserTokenType.UserName, SecurityPolicyUri = "http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256" })
					.AddUserTokenPolicy(new Opc.Ua.UserTokenPolicy() { TokenType = Opc.Ua.UserTokenType.Certificate, SecurityPolicyUri = "http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256" })
					.SetDiagnosticsEnabled(true)
					.SetPublishingResolution(500)
					.SetMaxMessageQueueSize(100)
					.SetMaxNotificationsPerPublish(1000)
					.SetAvailableSamplingRates(new Opc.Ua.SamplingRateGroupCollection() {
						new Opc.Ua.SamplingRateGroup(){Start=5, Increment=5, Count=20},
						new Opc.Ua.SamplingRateGroup(){Start=100, Increment=100, Count=4},
						new Opc.Ua.SamplingRateGroup(){Start=500, Increment=250, Count=2},
						new Opc.Ua.SamplingRateGroup(){Start=100, Increment=500, Count=20},
					})
					.SetNodeManagerSaveFile("")
					.SetMaxPublishRequestCount(100)
					.SetMaxSubscriptionCount(200)
					.AddServerProfile("http://opcfoundation.org/UA-Profile/Server/StandardUA2017")
					.AddServerProfile("http://opcfoundation.org/UA-Profile/Server/DataAccess")
					.AddServerProfile("http://opcfoundation.org/UA-Profile/Server/Methods")
					.AddServerProfile("http://opcfoundation.org/UA-Profile/Server/ReverseConnect")
					.SetReverseConnect(new Opc.Ua.ReverseConnectServerConfiguration()
					{
						Clients = new Opc.Ua.ReverseConnectClientCollection()
						{
							new Opc.Ua.ReverseConnectClient() { EndpointUrl="opc.tcp://" + reverseConnectURL, Timeout=30000, MaxSessionCount=0, Enabled=true}
						},
						ConnectInterval = 10000,
						RejectTimeout = 20000
					})
				.AddSecurityConfigurationExt(
					"Varec FuelsManager Opc Ua Server",
					"%CommonApplicationData%/Varec/FuelsManager/FMOpcUaServer/pki",
					"%CommonApplicationData%/Varec/FuelsManager/FMOpcUaServer/pki",
					"%CommonApplicationData%/Varec/FuelsManager/FMOpcUaServer/pki")
					.SetRejectSHA1SignedCertificates(false)
					.SetUserRoleDirectory("%CommonApplicationData%/Varec/FuelsManager/FMOpcUaServer/userRoles")
				.AddExtension<OpcUaServerConfiguration>(new XmlQualifiedName("OpcUaServerConfiguration"),
					new OpcUaServerConfiguration() { TimerInterval = 1000, ClearCachedCertificatesInterval = 30000 })
				.AddExtension<ServerToolkitConfiguration>(new XmlQualifiedName("ServerToolkitConfiguration"),
					new ServerToolkitConfiguration() { ServerCertificateLifeTime = opcUaCertificateLifeTime })
				.SetTraceMasks(traceMasks)
				.SetOutputFilePath("%CommonApplicationData%/Varec/FuelsManager/FMOpcUaServer/logs/FMOpcUaServer.log")
				.SetDeleteOnLoad(deleteOnLoad)
				.Create();


			applicationConfigurationBuilder.ApplicationConfiguration.ServerConfiguration.MaxRegistrationInterval = 10000;
			applicationConfigurationBuilder.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates = autoAcceptUntrustedCertificates;
			applicationConfigurationBuilder.ApplicationConfiguration.SecurityConfiguration.RejectUnknownRevocationStatus = rejectUnknownRevocationStatus;
			applicationConfigurationBuilder.ApplicationConfiguration.SecurityConfiguration.AddAppCertToTrustedStore = true;
			applicationConfigurationBuilder.ApplicationConfiguration.CertificateValidator.AutoAcceptUntrustedCertificates = autoAcceptUntrustedCertificates;
			applicationConfigurationBuilder.ApplicationConfiguration.CertificateValidator.RejectUnknownRevocationStatus = rejectUnknownRevocationStatus;

         return applicationConfigurationBuilder;
		}

	}
}
