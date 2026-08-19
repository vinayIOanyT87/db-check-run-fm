// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchRequests.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchRequests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using System.Collections;
	using System.Web.Hosting;
	using System.IO;
	using System.Reflection;
	using FMBusinessObjects.Interfaces;

	/// <summary>
	/// Dispatch requests service class for Dispatch use interfacing with FuelsManager Business Services.
	/// </summary>
	[ServiceBehavior()]
	public class ReportingRequest : IReportingRequest, IAlarmAndEventDiscovery
	{
		Dictionary<string, string> ReportParameters;
		string ProcessorId = string.Empty;
		SecurityClass ReportSecurity;

		#region Dictionary defines for memory leaks
		static Dictionary<string, Assembly> AssemblyDictionary = new Dictionary<string, Assembly>();
		#endregion

		#region Public Methods and Operators
		/// <summary>
		/// Enumerates equipment entities for use in Dispatch.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="parameters">The top Version</param>
		/// <returns>A dispatch equipment data object</returns>
		public DataSet ProcessReport(SecurityClass security, Dictionary<string, string> parameters)
		{
			ReportSecurity = security;
			ReportParameters = parameters;

			if (ReportSecurity == null)
			{
				throw new ArgumentNullException("security");
			}

			//var sites = new SitesClass();
			//SiteClass site = sites.Get(ReportSecurity, ReportSecurity.SiteGuid, bGetMemberSites: false, getSchedulesAndProcessVariables: false, bGetAssociatedAliases: false);
			//var timeConverter = new SiteTimeConverter(site);

			ValidateParametersHaveValidProcessor();

			IReportingRequestProcessor reportProcessor = GetProcessor();
			if (reportProcessor == null)
			{
				throw new Exception("Unable to load report processor.");
			}

			return RunProcessor(ReportSecurity, reportProcessor, ReportParameters);
		}

		private IReportingRequestProcessor GetProcessor()
		{
			assemblyList = SortedList.Synchronized(new SortedList());
			assemblyList.Clear();

			string strPath = GetInterfacePath();
			DirectoryInfo dir = new DirectoryInfo(strPath);

			if (!dir.Exists)
			{
				throw new ApplicationException("ReportProcessors folder not found");
			}

			FileInfo[] files = dir.GetFiles(this.ProcessorId + "Processor.dll");
			foreach (FileInfo file in files)
			{
				Type type = LoadInterfaceType(file.FullName, REPORTPROCESSOR_INTERFACENAME);
				if (type != null)
				{
					try
					{
						IReportingRequestProcessor processor = (IReportingRequestProcessor)Activator.CreateInstance(type);
						return processor;
					}
					catch (Exception Ex)
					{
						Console.WriteLine(Ex.Message);
						continue;
					}
				}
			}
			return null;
		}

		private bool ValidateParametersHaveValidProcessor()
		{
			bool processorIsValid = true;
			try
			{
				this.ProcessorId = ReportParameters["ProcessorId"];
			}
			catch
			{

				processorIsValid = false;
				this.ProcessorId = string.Empty;
			}

			return processorIsValid;
		}


		SortedList assemblyList;
		private string INTERFACE_FOLDER = "ReportProcessors";
		private string REPORTPROCESSOR_INTERFACENAME = "IReportingRequestProcessor";
		private string ErrorText = string.Empty;

		private List<string> GetParameters(SecurityClass security, Dictionary<string, string> parameters)
		{
			ValidateParametersHaveValidProcessor();

			IReportingRequestProcessor reportProcessor = GetProcessor();
			if (reportProcessor == null)
			{
				throw new Exception("Unable to load report processor.");
			}

			List<string> reportParameters;
			try
			{
				reportParameters = reportProcessor.GetReportParameters(security);
			}
			catch (Exception ex)
			{
				this.ErrorText = ex.Message;
				throw;
			}

			return reportParameters;
		}

		private DataSet RunProcessor(SecurityClass security, IReportingRequestProcessor processor, Dictionary<string, string> parameters)
		{
			DataSet reportDataSet;
			try
			{
				reportDataSet = processor.GetReportData(security, parameters);
			}
			catch (Exception ex)
			{
				this.ErrorText = ex.Message;
				throw;
			}

			return reportDataSet;
		}

		private Type LoadInterfaceType(string assemblyPath, string interfaceName)
		{
			Assembly DLL = null;

			if (assemblyPath == null || !File.Exists(assemblyPath))
			{
				throw new ApplicationException("Reporting Processor " + assemblyPath + " does not exist.");
			}

			if (!AssemblyDictionary.ContainsKey(assemblyPath.ToLower()))
			{
				try
				{
					DLL = Assembly.LoadFrom(assemblyPath);
				}
				catch
				{
					try
					{
						DLL = Assembly.Load(assemblyPath);
					}
					catch (Exception e)
					{
						throw new ApplicationException("Unable to Load Assembly: " + assemblyPath, e);
					}
				}
				if (DLL != null)
					AssemblyDictionary.Add(assemblyPath.ToLower(), DLL);
			}
			else
			{
				DLL = AssemblyDictionary[assemblyPath.ToLower()];
			}

			if (DLL != null)
			{
				Type[] types = null;

				try
				{
					types = DLL.GetTypes();
				}
				catch (ReflectionTypeLoadException ex)
				{
					types = ex.Types.Where(t => t != null).ToArray();
					// before only interfaces.dll got copied to the bin folder and this won't happen
					// now there are other DLLs copiied here and their types sometimes can't get loaded
					// should be ok to ignore the errors from those DLLs.
				}

				if (types != null)
				{
					foreach (Type Module in types)
					{
						Type type = Module.GetInterface(interfaceName);
						if (type != null)
						{
							return Module;
						}
					}
				}
			}
			return null;
		}

		public List<string> GetReportParameters(SecurityClass security, Dictionary<string, string> parameters)
		{
			ReportSecurity = security;
			ReportParameters = parameters;

			if (ReportSecurity == null)
			{
				throw new ArgumentNullException("security");
			}

			ValidateParametersHaveValidProcessor();

			IReportingRequestProcessor reportProcessor = GetProcessor();
			if (reportProcessor == null)
			{
				throw new Exception("Unable to load report processor.");
			}

			return reportProcessor.GetReportParameters(ReportSecurity);
		}
		private string GetInterfacePath()
		{
			return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, INTERFACE_FOLDER);
		}


		#endregion

		#region Alarm and event descriptor
		static string ReportingWebServiceKey = "Reporting Web Service";

		static AlarmAndEventDescriptorClass ProcessorNotFoundDescriptor = new AlarmAndEventDescriptorClass(true, ReportingWebServiceKey, "Unable to locate report processor.");
		static AlarmAndEventDescriptorClass ProcessorIdParameterInvalidDescriptor = new AlarmAndEventDescriptorClass(true, ReportingWebServiceKey, "ProcessorId parameter is invalid.");
		static AlarmAndEventDescriptorClass ProcessinErrorDescriptor = new AlarmAndEventDescriptorClass(true, ReportingWebServiceKey, "Processor return errors.");

		public AlarmAndEventLogClass ProcessingError
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(ProcessinErrorDescriptor);
				AlarmAndEventLog.AssociatedData = this.ErrorText;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass ProcessorIdParameterInvalid
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(ProcessorIdParameterInvalidDescriptor);
				AlarmAndEventLog.AssociatedData = this.ProcessorId;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass ProcessorNotFound
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(ProcessorNotFoundDescriptor);
				AlarmAndEventLog.AssociatedData = this.ProcessorId;
				return AlarmAndEventLog;
			}
		}

		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] Descriptors ={  ProcessorNotFoundDescriptor
																	};
				return Descriptors;
			}
		}
		#endregion
	}
}
