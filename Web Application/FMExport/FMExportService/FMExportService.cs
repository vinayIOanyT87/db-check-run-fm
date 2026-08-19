// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMExportService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMExport service which periodically exports data using custom aviation interfaces
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.ServiceModel;
	using System.ServiceProcess;
	using System.Threading;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;
	using DTO;

	/// <summary>
	/// Defines the FMExport service which periodically exports data using custom aviation interfaces
	/// </summary>
	public partial class FMExportService : ServiceBase
	{
		#region Private Members

		/// <summary>
		/// The well-known Guid which identifies the Administrative Site in FuelsManager.
		/// The Site Admin Guid is required to log in to FuelsManager.
		/// </summary>
		private readonly Guid siteAdminGuid = new Guid("00000000-0000-0000-0000-000000000001");

		private ManualResetEvent mainStopEvent;
		private ManualResetEvent ftpThreadHandlerStopAckEvent;
		private ManualResetEvent queueThreadHandlerStopAckEvent;
		private ManualResetEvent objWebServiceThreadStockAckEvent;

		private SortedList mainRequestList;

		/// <summary>
		/// Will hold a collection of ftp thread generated events with timestamp.
		/// </summary>
		private SynchronizedCollection<FtpThreadLog> ftpLog;

		private object lockObject = new object();

		private Thread queueThread;
		private Thread ftpThread;
		private Thread objWebServiceThread;
		private Thread ftpThreadMonitor;

		/// <summary>
		/// The FuelsManager security object
		/// </summary>
		private SecurityClass security;

		/// <summary>
		/// The FMExport service event logger.
		/// </summary>
		private readonly FMExportServiceLogger logger;

		/// <summary>
		/// A host for the WCF service which receives communication from the configuration utility
		/// </summary>
		private ServiceHost fmExportServiceCommunicationHost = null;

		public static Type[] SupportedInterfaceTypes;

		#endregion

		/// <summary>
		/// Initializes a new instance of the FMExportService class.
		/// </summary>
		public FMExportService()
		{
			this.InitializeComponent();
			this.mainStopEvent = new ManualResetEvent(false);
			this.ftpThreadHandlerStopAckEvent = new ManualResetEvent(true);
			this.queueThreadHandlerStopAckEvent = new ManualResetEvent(true);
			this.objWebServiceThreadStockAckEvent = new ManualResetEvent(true);

			this.mainRequestList = SortedList.Synchronized(new SortedList());
			this.logger = FMExportServiceLogger.Instance;

			this.ftpLog = new SynchronizedCollection<FtpThreadLog>();
		}

		static FMExportService()
		{
			SupportedInterfaceTypes = GetTypesImplementingInterface("IDataRetriever");
		}

		/// <summary>
		/// Get a data retriever from the interface matching the provided ID
		/// </summary>
		/// <param name="interfaceID">Identifies the data retriever to get</param>
		/// <returns>A data retriever from the interface matching the provided ID.
		/// This method will throw if the data retriever is not found</returns>
		public static IDataRetriever GetDataRetriever(string interfaceID)
		{
			foreach (Type objType in SupportedInterfaceTypes)
			{
				IDataRetriever dataRetriever = Activator.CreateInstance(objType) as IDataRetriever;
				if (dataRetriever != null && dataRetriever.InterfaceId == interfaceID)
					return dataRetriever;
			}

			throw new Exception("Could not locate a data retriever for interface id " + interfaceID);
		}

		/// <summary>
		/// Calls the OnStart() method.  Used to simulate the behavior of the
		/// service when it is not actually installed or running.
		/// </summary>
		public void ProxyStart()
		{
			this.OnStart(null);
		}

		/// <summary>
		/// Calls the OnStop() method.  Used to simulate the behavior of the
		/// service when it is not actually installed or running.
		/// </summary>
		public void ProxyStop()
		{
			this.OnStop();
		}

		#region Protected Overrides

		/// <summary>
		/// Executes when a start command is sent to the service
		/// </summary>
		/// <param name="args">
		/// Data Passed by the start command
		/// </param>
		protected override void OnStart(string[] args)
		{
			try
			{
				if (System.Configuration.ConfigurationManager.AppSettings["WaitForDebuggerOnStart"] != null && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["WaitForDebuggerOnStart"]))
				{
					while (!Debugger.IsAttached)
					{
						Thread.Sleep(100);
						this.RequestAdditionalTime(100);
					}
				}
				if (!this.logger.Initialized)
				{
					var log = new EventLog("Application");
					log.WriteEntry("Unable to initialize logger for FMExport service. Stopping service.", EventLogEntryType.Error, 999);
					this.mainStopEvent.Set();
					this.queueThreadHandlerStopAckEvent.Set();
					this.ftpThreadHandlerStopAckEvent.Set();
					this.objWebServiceThreadStockAckEvent.Set();
					this.ProxyStop();
					return;
				}

                FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

				// Start the WCF communication host to receive requests from the configuration utility
				//if (RoleEnvironment.IsAvailable)
				//{
				//    this.fmExportServiceCommunicationHost = new ServiceHost(typeof(FMExportServiceCommunication), new Uri(GetEndpointAddress("FMExportServiceEndpoint")));
				//}
				//else
				//{
				this.fmExportServiceCommunicationHost = new ServiceHost(typeof(FMExportServiceCommunication));
				//}

				this.fmExportServiceCommunicationHost.Open();

				// Keep trying to login until the login is successful
				var retryCount = 0;
				while (!this.LoginToFuelsManager())
				{
					if (++retryCount > 9)
					{
						throw new ApplicationException("Login to FuelsManager failed after 10 retries.");
					}

					Thread.Sleep(3000);
				}

				this.mainStopEvent.Reset();
				this.ftpThreadHandlerStopAckEvent.Reset();
				this.queueThreadHandlerStopAckEvent.Reset();
				this.objWebServiceThreadStockAckEvent.Reset();

				this.CheckForConfigChanges();

				this.queueThread = new Thread(this.QueueThreadHandler);
				this.logger.LogInfo("Request Queue Thread Handler Started", 1001);
				this.queueThread.Start();

				this.ftpThread = new Thread(this.FTPThreadHandler);
				this.logger.LogInfo("FTP Queue Thread Handler Started", 1002);
				this.ftpThread.Start();

				objWebServiceThread = new Thread(WebServiceThreadHandler);
				logger.LogInfo("Web Service Thread Handler Started", 1003);
				objWebServiceThread.Start();

				this.ftpThreadMonitor = new Thread(FTPThreadWatcher);
				this.logger.LogInfo("FTP Watcher Thread Handler Started", 1003);
				this.ftpThreadMonitor.Start();

			}
			catch (FMExportServiceInterfaceFolderNotFoundException e)
			{
				this.logger.LogError(e.ToString(), 444);
				this.ProxyStop();
			}
			catch (Exception e)
			{
				this.logger.LogError(e.ToString(), 555);
				this.ProxyStop();
			}
		}

		/// <summary>
		/// Executes when a stop command is sent to the service
		/// </summary>
		protected override void OnStop()
		{
			try
			{
				// Shut down the WCF service 
				if (this.fmExportServiceCommunicationHost != null)
				{
					this.fmExportServiceCommunicationHost.Close();
				}

				this.mainStopEvent.Set();
				this.queueThreadHandlerStopAckEvent.WaitOne(10000);
				this.ftpThreadHandlerStopAckEvent.WaitOne(10000);
				objWebServiceThreadStockAckEvent.WaitOne(10000);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex.ToString());

				if (this.fmExportServiceCommunicationHost != null)
				{
					this.fmExportServiceCommunicationHost.Abort();
				}
			}
		}

		#endregion

		#region Private Methods

		public static Type[] GetTypesImplementingInterface(string InterfaceName)
		{
			string strPath, strDir;

			strPath = Assembly.GetExecutingAssembly().Location;
			strDir = Path.GetDirectoryName(strPath);
			return GetTypesImplementingInterface(InterfaceName, strDir);
		}

		public static Type[] GetTypesImplementingInterface(string InterfaceName, string PathName)
		{
			List<Type> objRetTypes = new List<Type>();
			string[] strFiles;
			Type[] objTypes;

			strFiles = Directory.GetFiles(PathName, "*.dll");
			foreach (string strFile in strFiles)
			{
				Assembly objAssembly = null;
				if (!AssemblyDictionary.ContainsKey(strFile.ToLower()))
				{
					try
					{
						objAssembly = Assembly.LoadFile(strFile);
					}
					catch (Exception)
					{
					}

					if (objAssembly != null)
					{
						AssemblyDictionary.Add(strFile.ToLower(), objAssembly);
					}
				}
				else
				{
					objAssembly = AssemblyDictionary.Get(strFile.ToLower());
				}

				if(objAssembly != null)
				{
					try
					{
						objTypes = objAssembly.GetTypes();
						foreach (Type objType in objTypes)
						{
							if (objType.GetInterface(InterfaceName) != null)
							{
								objRetTypes.Add(objType);
								break;
							}
						}
					}
					catch { }
				}
			}

			return objRetTypes.ToArray();
		}

		///// <summary>
		///// Get the address for the specified endpoint
		///// </summary>
		///// <param name="endpointName">The endpoint to get the address for</param>
		///// <returns>The address for the specified endpoint</returns>
		//private static string GetEndpointAddress(string endpointName)
		//{
		//    RoleInstanceEndpoint roleEndpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[endpointName];

		//    string endpointAddress = roleEndpoint.IPEndpoint.ToString();

		//    return "net.tcp://" + endpointAddress + "/";
		//}

		/// <summary>
		/// Login to FuelsManager
		/// </summary>
		/// <returns>True if the login was successful</returns>
		private bool LoginToFuelsManager()
		{
			try
			{
				var loginSecurity = new SecurityClass
				{
					UserGuid = Guid.Empty,
					LoginSiteGuid = Guids.SiteAdminGuid,
					SiteGuid = Guids.SiteAdminGuid
				};

				loginSecurity.UserID = FMChannelHelper.MakeCall<IDBAccess, string>(
					fuelsManagerDatabaseAccess => fuelsManagerDatabaseAccess.ServiceLogin(loginSecurity));

				loginSecurity.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
				loginSecurity.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
				loginSecurity.AddRight(RIGHT.CONFIGURE_AVIATION_EXPORT);
				loginSecurity.AddRight(RIGHT.VIEW_TRANSACTION_DATA);

				this.security = loginSecurity;
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex.ToString());
				return false;
			}

			return true;
		}

		private void CheckForConfigChanges()
		{
			List<ExportRequestClass> exportRequestList = FMChannelHelper.MakeCall<IExportRequests, List<ExportRequestClass>>(
										exportRequests => exportRequests.GetRequests(this.security));

			this.mainRequestList.Clear();
			foreach (ExportRequestClass request in exportRequestList)
			{
				this.mainRequestList.Add(request.RequestId, request);
			}
		}

		private void QueueThreadHandler()
		{
			try
			{
				while (!this.mainStopEvent.WaitOne(3000))
				{
					try
					{
						this.CheckForConfigChanges();

						foreach (ExportRequestClass request in this.mainRequestList.Values)
						{
							if (request.InitiateSend)
							{
								this.InvokeProcessor(request);
							}
						}
					}
					catch (Exception e)
					{
						this.logger.LogError(e.ToString(), 111);
					}
				}
			}
			catch (Exception e)
			{
				this.logger.LogError(e.ToString(), 222);
			}

			this.queueThreadHandlerStopAckEvent.Set();
		}

		/// <summary>
		/// Monitors the FTPThreadHandler to see if it gets stuck, then restarts it.
		/// </summary>
		private void FTPThreadWatcher()
		{
			while (!this.mainStopEvent.WaitOne(5000))
			{
				try
				{
					CleanUpFTPLogCollection();

					//lets get the last published event
					var lastEvent = this.ftpLog.OrderByDescending(x => x.Published).FirstOrDefault();

					if (!this.ftpThread.IsAlive)
					{
						//it is dead and I do not know why, lets restart it
						this.logger.LogError("FTP thread is dead without any logs");
						RestartFTPThread();
					}
					if (lastEvent == null)
					{
						//no events just yet
						continue;
					}
					//is it older than an hour?
					var timePassed = DateTime.UtcNow - lastEvent.Published;
					if (timePassed.TotalMinutes > 60)
					{
						RestartFTPThread();
					}
				}
				catch (Exception e)
				{
					this.logger.LogError(e.ToString(), 333);
				}
			}
		}

		/// <summary>
		/// Tries to kill the ftpThread and then starts up a new instance
		/// </summary>
		private void RestartFTPThread()
		{
			var log = string.Join("\n",
				 this.ftpLog.Select(x => string.Format("{0} - {1}", x.Published, x.Description)));
			this.logger.LogError("ftp thread LockedUp - " + log);
			this.ftpLog.Clear();
			while (this.ftpThread.IsAlive)
			{
				//try to get the thread to abort
				this.ftpThread.Abort();
				Thread.Sleep(1000);
			}
			this.ftpThread = new Thread(this.FTPThreadHandler);
			this.ftpThread.Start();
			this.logger.LogError("ftp thread has restarted the ftp thread");
		}

		/// <summary>
		/// Clears the ftp log collection as long as it completes its passes
		/// </summary>
		private void CleanUpFTPLogCollection()
		{
			lock (lockObject)
			{
				//clear out the list if there is a completed flag set as true
				var didCompleteList = this.ftpLog.Where(x => x.LastEventBeforeLoop).ToList();
				var completedLogItems = new List<FtpThreadLog>();
				foreach (var completedItem in didCompleteList)
				{
					var partialCompletedLogItems = this.ftpLog.Where(x => x.CorrelationId == completedItem.CorrelationId).ToList();
					foreach (var toRemove in partialCompletedLogItems)
					{
						completedLogItems.Add(toRemove);
					}
				}

				foreach (var completedLogItem in completedLogItems)
				{
					this.ftpLog.Remove(completedLogItem);
				}
			}
		}

		private void FTPThreadHandler()
		{
			try
			{
				while (!this.mainStopEvent.WaitOne(5000))
				{
					var correlationId = Guid.NewGuid();
					ftpLog.Add(new FtpThreadLog()
					{
						Published = DateTime.UtcNow,
						LastEventBeforeLoop = false,
						Description = "Starting ftp attempt",
						CorrelationId = correlationId
					});
					List<ExportRequestClass> exportRequestList =
				 FMChannelHelper.MakeCall<IExportRequests, List<ExportRequestClass>>(
					 exportRequests => exportRequests.GetRequests(this.security));
					foreach (ExportRequestClass request in exportRequestList)
					{
						if (request.SendMethod == FMBusinessObjects.Constants.FileSendMethodEnum.FTP || request.SendMethod == FMBusinessObjects.Constants.FileSendMethodEnum.FTPS)
						{
							ftpLog.Add(new FtpThreadLog()
							{
								Published = DateTime.UtcNow,
								LastEventBeforeLoop = false,
								Description = string.Format("ConnectionInfo: {0} , SendingCompanyCode: {1}", request.ConnectionInfo, request.SendingCompanyCode),
								CorrelationId = correlationId
							});
							FTPConnectionClass ftp = (FTPConnectionClass)XmlObjConverter.FromXml(request.ConnectionInfo, typeof(FTPConnectionClass));
#if DEBUG
							ftp.DebugMode = true;
#else
                            ftp.DebugMode = false;
#endif
							ftp.UploadFiles(request);

							ftpLog.Add(new FtpThreadLog()
							{
								Published = DateTime.UtcNow,
								LastEventBeforeLoop = true,
								Description = string.Format("Sending Complete"),
								CorrelationId = correlationId
							});
						}
					}
				}
			}
			catch (Exception e)
			{
				this.logger.LogError(e.ToString(), 333);
			}

			this.ftpThreadHandlerStopAckEvent.Set();
		}

		/// <summary>
		/// This method is used for the web service plug-in thread.
		/// </summary>
		private void WebServiceThreadHandler()
		{
			Thread.Sleep(5000);
			do
			{
				string strCurrentRequestID = null;
				try
				{
					List<ExportRequestClass> objRequests;
					List<IWebServicePlugin> objPlugins = null;
					IWebServicePlugin objPlugin;

					objRequests = FMChannelHelper.MakeCall<IExportRequests, List<ExportRequestClass>>(
							  objER => objER.GetRequests(this.security)
					);
					if (objRequests.Count(objER => objER.SendMethod == FMBusinessObjects.Constants.FileSendMethodEnum.WebService) > 0)
						objPlugins = GetWebServicePlugins();
					foreach (ExportRequestClass objRequest in objRequests)
					{
						strCurrentRequestID = objRequest.RequestId;
						if (objRequest.SendMethod == FMBusinessObjects.Constants.FileSendMethodEnum.WebService)
						{
							objPlugin = objPlugins.Find(objP => objP.WebServicePluginID == objRequest.WebServicePluginType);
							if (objPlugin != null)
								SendExportUsingWebServicePlugin(objRequest, objPlugin);
							else
								throw new Exception("An export request is using a web service plug-in ID of \"" + objRequest.WebServicePluginType + "\" and that plug-in could not be found.");
						}
					}
				}
				catch (Exception objEx)
				{
					if (strCurrentRequestID != null)
						this.logger.LogError("An error was encountered while processing the web service plug-in for the request, " + strCurrentRequestID + ".\n" + objEx.ToString(), 666);
					else
						this.logger.LogError(objEx.ToString(), 666);
				}
			} while (!this.mainStopEvent.WaitOne(5000));
			objWebServiceThreadStockAckEvent.Set();
		}

		/// <summary>
		/// This method checks for a file to send for the export request passed in and send the file using the web service plug-in.
		/// </summary>
		/// <param name="Request">The FM Export request to process.</param>
		/// <param name="Plugin">An instantiated web service plug-in to send the export.</param>
		private void SendExportUsingWebServicePlugin(ExportRequestClass Request, IWebServicePlugin Plugin)
		{
			string strPath, strDir;
			FileInfo[] objFiles;
			FileStream objFStream;

			strPath = FMConvert.GetAssemblyDirectory();
			strDir = Path.Combine(strPath, Request.UploadStagingFolder);
			if (Directory.Exists(strDir))
			{
				objFiles = new DirectoryInfo(strDir).GetFiles();
				foreach (FileInfo objFile in objFiles)
				{
					try
					{
						objFStream = new FileStream(objFile.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
						objFStream.Close();
					}
					catch (IOException objEx)
					{
						this.logger.LogWarning(objEx.ToString(), 1008);
						continue;
					}
					this.logger.LogInfo("Uploading " + objFile.FullName + " using the web service plug-in, " + Plugin.WebServicePluginID + "...");
					Plugin.SetConfiguration(Request.WebServiceConfiguration);
					Plugin.Send(objFile.FullName);
					objFile.Delete();
				}
			}
			else
				this.logger.LogWarning("Staging Folder: " + strDir + " does not exist", 1005);
		}

		/// <summary>
		/// Gets a List of objects that implement the IWebServicePlugin interface for use with sending files via web service plug-ins.
		/// </summary>
		/// <returns>A List of IWebServicePlugin objects that exist in the WebServicePlugins folder.</returns>
		private List<IWebServicePlugin> GetWebServicePlugins()
		{
			Type[] objTypes;
			string strPath, strDir;
			IWebServicePlugin objPlugin;
			List<IWebServicePlugin> objPlugins = new List<IWebServicePlugin>();

			strPath = Assembly.GetExecutingAssembly().Location;
			strDir = Path.GetDirectoryName(strPath);
			strDir = Path.Combine(strDir, Constants.WEBSERVICE_PLUGIN_FOLDER);
			objTypes = GetTypesImplementingInterface("IWebServicePlugin", strDir);
			foreach (Type objType in objTypes)
			{
				objPlugin = (IWebServicePlugin)Activator.CreateInstance(objType, false);
				objPlugins.Add(objPlugin);
			}
			return objPlugins;
		}


		private void InvokeProcessor(ExportRequestClass request)
		{
			IDataRetriever dataRetriever = GetDataRetriever(request.InterfaceId);
			DataResultClass result = dataRetriever.GetData(request.RequestId, this.security);
			this.ProcessResult(request, result);
		}

		private void ProcessResult(ExportRequestClass request, DataResultClass result)
		{
			string assemblyPath = FMConvert.GetAssemblyDirectory();
			for (int i = 0; i < result.Xml.Count; i++)
			{
				string archivePath = assemblyPath + request.ArchiveFolder;
				string stagingPath = assemblyPath + request.UploadStagingFolder;

				string archiveFilePath = archivePath;
				string stagingFilePath = stagingPath;
				if (!result.UseRawResultFileName)
				{
					archiveFilePath += request.SendingCompanyCode + "_DataExport_" + result.Xml.Keys[i] + "_" + DateTime.UtcNow.ToString("yyyy-MM-dd HH+mm+ss") + ".xml";
					stagingFilePath += request.SendingCompanyCode + "_DataExport_" + result.Xml.Keys[i] + "_" + DateTime.UtcNow.ToString("yyyy-MM-dd HH+mm+ss") + ".xml";
				}
				else
				{
					archiveFilePath += result.Xml.Keys[i];
					stagingFilePath += result.Xml.Keys[i];
				}

				if (!Directory.Exists(archivePath))
				{
					Directory.CreateDirectory(archivePath);
				}

				if (!Directory.Exists(stagingPath))
				{
					Directory.CreateDirectory(stagingPath);
				}

				if (!(request.ExcludeEmptyFiles && result.GetTransCountValue(result.Xml.Keys[i]) == 0))
				{
					// Ensures that file is only accessible by current thread.
					FileStream stagingFile = File.Open(stagingFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

					var writerStaging = new StreamWriter(stagingFile);
					writerStaging.Write(result.Xml.Values[i]);
					writerStaging.Close();
					this.logger.LogInfo("Staging file: \"" + stagingFilePath + "\" written to disk", 888);
				}

				// Ensures that archive file is only accessible by current thread.
				FileStream archiveFile = File.Open(archiveFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

				var writerArchive = new StreamWriter(archiveFile);
				writerArchive.Write(result.Xml.Values[i]);
				writerArchive.Close();
				this.logger.LogInfo("Archive file: \"" + stagingFilePath + "\" written to disk", 899);
			}

			request.LatestRowVersion = result.LargestRowVersion;
			request.LastExportTime = DateTimeOffset.Now;

			if (request.UseTimeOfDay)
			{
				request.SetNextExportTime();
			}

			FMChannelHelper.MakeCall<IExportRequests>(
				exportRequests => exportRequests.Update(this.security, request));
		}

		#endregion
	}
}
