
namespace FMDataExchange
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Security;
	using System.Web.Hosting;
	using System.Web.Services.Protocols;
	using System.Xml;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;


	// NOTE: If you change the class name "ExchangeService" here, you must also update the reference to "ExchangeService" in Web.config.
	public class ExchangeService : IExchangeService
	{

		#region Internal Attributes
		private SortedList assemblyList;

		private const string DataexchangeInterfacename = "IFMDataExchangeProcessor";
		private const string InterfaceFolder = "Interfaces";

		#endregion

		#region Dictionary defines for memory leaks
		static Dictionary<string, Assembly> AssemblyDictionary = new Dictionary<string, Assembly>();
		#endregion

		#region Constructors
		public ExchangeService()
		{
			this.assemblyList = null;
		}
		#endregion

		/// <summary>
		/// Please see MessageInspector.FaultExceptionActionName for comment.
		/// </summary>
		public void NotUsed()
		{
		}

		public string Exchange(string userID, string password, bool bCAC, string siteID, string interfaceID, string xmlData)
		{
			string retValue = null;

			var security = new SecurityClass();

			this.InitializeAssemblyList();

			Type interfaceType = this.LoadSpecificInterfaceType(interfaceID);
			if (interfaceType != null)
			{
				Object lateBoundObj = Activator.CreateInstance(interfaceType);
				var processor = lateBoundObj as IFMDataExchangeProcessor;
				if (processor != null)
				{
					if (processor.Authenticate)
					{
						try
						{
							bool changePassword = false;
							int daysUntilExpiration = 0;

							var loginRequest = new SecurityLoginRequest
							{
								UserID = userID,
								Password = password,
								SiteID = siteID,
								CACEnabled = bCAC
							};

							string result = FMChannelHelper.MakeCall<ISites, string>(
								x => x.Login(out changePassword, out daysUntilExpiration, out security, loginRequest));
							SecurityLoginResponse loginResponse =
								FMChannelHelper.MakeCall<ISites, SecurityLoginResponse>(
									x => x.Login2(loginRequest));

							//string result = (loginResponse == null) ? null : loginResponse.Result;

							if (result != null)
							{
								throw new SecurityException("User \"" + userID + "\" " + result);

							}
						}
						catch (Exception ex)
						{
							throw new SoapException(ex.Message, new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"));
						}

						if (!security.HasRight(RIGHT.INTERFACE_IMPORT))
						{
							throw new SecurityException(
								"User \"" + userID + "\" " + "not authorized to perform import or export operations.");
						}
					}
					else
					{
						//Setup security
						security.SiteGuid = Guids.SiteAdminGuid;
						security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
						security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
						security.AddRight(RIGHT.VIEW_USERS);
						security.AddRight(RIGHT.MODIFY_USERS);
						security.AddRight(RIGHT.VIEW_USER_GROUPS);
						security.AddRight(RIGHT.MODIFY_USER_GROUPS);
						security.AddRight(RIGHT.VIEW_EQUIPMENT_DATA);
						security.AddRight(RIGHT.MODIFY_EQUIPMENT_DATA);


						string serviceLogin = FMChannelHelper.MakeCall<IDBAccess, string>(x => x.ServiceLogin(security));


						// For now, create a security object that has sufficient rights to import ADC transactions.  more later.
						security.UserID = serviceLogin;
						var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(security, siteID, true));
						if (site.IdentityGuid != Guid.Empty)
						{
							security.SiteID = site.ID;
							security.SiteGuid = site.IdentityGuid;
							AddSecurityRights(security);
						}
						else
						{
							throw new Exception("Site '" + siteID + "'is not configured.");
						}
					}

					processor.InterfacePath = this.GetInterfacePath();
					try
					{
						var eventLog = new EventLog("Application", ".", "FuelsManager");
						eventLog.WriteEntry("FMDataExchange Processor Starting: " + interfaceType.Assembly.Location, EventLogEntryType.Information);
						retValue = processor.ProcessData(security, xmlData);
						eventLog.WriteEntry("FMDataExchange Processor Completed: " + interfaceType.Assembly.Location, EventLogEntryType.Information);
					}
					catch (Exception ex)
					{
						var eventLog = new EventLog("Application", ".", "FuelsManager");
						eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
						eventLog.WriteEntry(ex.StackTrace, EventLogEntryType.Error);

						var soapExc = new SoapException(
					ex.Message,
					new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"),
					ex.InnerException);

						throw soapExc;
					}

					if (processor.Authenticate)
					{
						FMChannelHelper.MakeCall<ISites>(x => x.Logout(security));
					}
				}

				if (string.IsNullOrEmpty(retValue))
				{
					throw new FMDataExchangeNullDataException("Null Object Returned");
				}
			}

			return retValue;
		}

		public string ExchangeCompressed(string user, string password, bool bCAC, string site, string interfaceID, string compressedXmlData)
		{
			var decompressor = new DecompressionProcessor();
			string xmlData = decompressor.DecompressToString(Convert.FromBase64String(compressedXmlData));

			string retValue = this.Exchange(user, password, bCAC, site, interfaceID, xmlData);

			var compressor = new CompressionProcessor();

			return Convert.ToBase64String(compressor.Compress(retValue));
		}




		#region Internal Methods

		private static void AddSecurityRights(SecurityClass security)
		{
			security.AddRight(RIGHT.EXECUTE_IMPORT_EXPORT);
			security.AddRight(RIGHT.IMPORT_ENTERPRISE_DATA);
			security.AddRight(RIGHT.INTERFACE_IMPORT);
			security.AddRight(RIGHT.MODIFY_DISPATCH);
			security.AddRight(RIGHT.MODIFY_FINANCIAL_DATA);
			security.AddRight(RIGHT.MODIFY_PRODUCTS);
			security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
			security.AddRight(RIGHT.MODIFY_STANDING_OFFERS);
			security.AddRight(RIGHT.MODIFY_TRANSACTION_ALIASES);
			security.AddRight(RIGHT.MODIFY_TRANSACTION_DATA);
			security.AddRight(RIGHT.PERFORM_CLOSEOUT);
			security.AddRight(RIGHT.VIEW_BILLS_OF_LADING);
			security.AddRight(RIGHT.VIEW_DISPATCH);
			security.AddRight(RIGHT.VIEW_PRODUCTS);
			security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
			security.AddRight(RIGHT.VIEW_TRANSACTION_DATA);
		}

		private string GetInterfacePath()
		{
			return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, InterfaceFolder);
		}

		private void InitializeAssemblyList()
		{
			this.assemblyList = SortedList.Synchronized(new SortedList());
			this.assemblyList.Clear();

			string strPath = this.GetInterfacePath();
			var dir = new DirectoryInfo(strPath);
			if (!dir.Exists)
			{
				throw new FMDataExchangeInterfaceFolderNotFoundException("Interfaces folder not found");
			}

			FileInfo[] files = dir.GetFiles("*.dll");
			foreach (FileInfo file in files)
			{
				Type type = this.LoadInterfaceType(file.FullName, DataexchangeInterfacename);
				if (type != null)
				{
					try
					{
						Object lateBoundObj = Activator.CreateInstance(type);
						var obj = (IFMDataExchangeProcessor)lateBoundObj;
						if (!this.assemblyList.ContainsKey(obj.InterfaceID.ToUpper()))
						{
							this.assemblyList.Add(obj.InterfaceID.ToUpper(), file.FullName);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
		}

		private Type LoadInterfaceType(string assemblyPath, string interfaceName)
		{
			Assembly dll = null;

			if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
			{
				throw new FMDataExchangeInterfaceAssemblyNotFoundException(assemblyPath + " does not exists");
			}

			if (!AssemblyDictionary.ContainsKey(assemblyPath.ToLower()))
			{
				try
				{
					dll = Assembly.LoadFrom(assemblyPath);
				}
				catch (BadImageFormatException)
				{
					// that's ok, it is not a .Net file
				}
				catch
				{
					try
					{
						dll = Assembly.Load(assemblyPath);
					}
					catch (BadImageFormatException)
					{
						// that's ok, it is not a .Net file
					}
					catch (Exception e)
					{
						throw new FMDataExchangeInterfaceAssemblyNotFoundException("Unable to Load Assembly: " + assemblyPath, e);
					}
				}
				if (dll != null)
					AssemblyDictionary.Add(assemblyPath.ToLower(), dll);
			}
			else
			{
				dll = AssemblyDictionary[assemblyPath.ToLower()];
			}

			if (dll != null)
			{
				Type[] types = null;

				try
				{
					types = dll.GetTypes();
				}
				// ReSharper disable once EmptyGeneralCatchClause
				catch
				{
					// before only interfaces.dll got copied to the bin folder and this won't happen
					// now there are other DLLs copiied here and their types sometimes can't get loaded
					// should be ok to ignore the errors from those DLLs.
				}

				if (types != null)
				{
					foreach (Type module in types)
					{
						Type type = module.GetInterface(interfaceName);
						if (type != null)
						{
							return module;
						}
					}
				}
			}
			return null;
		}

		private Type LoadSpecificInterfaceType(string interfaceID)
		{
			interfaceID = interfaceID.ToUpper();
			if (this.assemblyList.Contains(interfaceID))
			{
				string assy = this.assemblyList[interfaceID].ToString();
				Type type = this.LoadInterfaceType(assy, DataexchangeInterfacename);

				if (type != null)
				{
					try
					{
						var lateBoundObj = Activator.CreateInstance(type);
						var obj = (IFMDataExchangeProcessor)lateBoundObj;
						if (String.Compare(obj.InterfaceID, interfaceID, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return type;
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			return null;
		}
		#endregion
	}
}
