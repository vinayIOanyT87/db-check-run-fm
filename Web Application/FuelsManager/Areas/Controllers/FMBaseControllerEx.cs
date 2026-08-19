
namespace FuelsManager.Areas.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using System.Web.Mvc;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
	using Softing.Opc.Ua.Configuration;
	using System.Security.Cryptography.X509Certificates;
	using System.Xml;
	using System.Xml.Serialization;


	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMPointCommon;


	public class FMBaseControllerEx : FMBaseController
	{
		private static UaApplication application = null;
		private static object applicationLock = new object();
	

		public ResponseMessageClass Results = new ResponseMessageClass();
  
		public void OnError(Exception e)
		{
				Results.ErrorMessage.Add(Guid.NewGuid().ToString(), new List<string> { GetTranslatedText(e.Message) });
		}

		public void OnError(string errMsg)
		{
				Results.ErrorMessage.Add(Guid.NewGuid().ToString(), new List<string> { GetTranslatedText(errMsg) });
		}

		protected static UaApplication GetUaApplication()
		{
			lock(applicationLock)
			{
				if(application != null)
				{
					return application;
				}

				ApplicationConfigurationBuilderEx configuration = LoadApplicationConfiguration().Result;
				application = UaApplication.Create(configuration).Result;

				return application;
			}
		}

		protected bool IsValidationErrors()
		{
			return (ModelState.Count(ms => ms.Value.Errors.Any()) != 0);
		}

		public void AddSuccess(string successMsg, bool addIfErrorsArePresent = false)
		{
			Results.SuccessMessage.Add(Guid.NewGuid().ToString(), new List<string> { GetTranslatedText(successMsg) });
		}

		protected void PopulateResultsValidations()
		{
			if (IsValidationErrors())
			{
				var erroneousFields = ModelState.Where(ms => ms.Value.Errors.Any())
							.Select(x => new { x.Key, x.Value.Errors });

					foreach (var erroneousField in erroneousFields)
					{
						if (Results.ErrorMessage.ContainsKey(erroneousField.Key) == false)
						{
							Results.ErrorMessage.Add(erroneousField.Key, erroneousField.Errors.Select(error => this.GetTranslatedText(error.ErrorMessage)).ToList());
						}

					}
				}
		}


		protected JsonResult JsonWithErrorMessages(object data, JsonRequestBehavior reqBehavior = JsonRequestBehavior.DenyGet)
		{
				PopulateResultsValidations();
				Results.Data = data;
				var ret = this.Json(Results, reqBehavior);
				return ret;
		}

		protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new JsonResult()
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior,
				MaxJsonLength = Int32.MaxValue
			};
		}

		protected void HandleModelResultsPopulation(object model)
		{
			if (model != null && model.GetType().IsSubclassOf(typeof(FMBaseModel)))
			{
				PopulateResultsValidations();
				var mod = (FMBaseModel)model;
				mod.Results = this.Results;
			}
		}

		protected new ViewResult View(string viewName, object model)
		{
			HandleModelResultsPopulation(model);
			return base.View(viewName, model);
		}

		protected new ViewResult View(object model)
		{
			HandleModelResultsPopulation(model);
			return base.View(model);
		}

		protected override ViewResult View(System.Web.Mvc.IView view, object model)
		{
			HandleModelResultsPopulation(model);
			return base.View(view, model);
		}

		protected override ViewResult View(string viewName, string masterName, object model)
		{
			HandleModelResultsPopulation(model);
			return base.View(viewName, masterName, model);
		}

		protected string RenderRazorViewToString(string viewName, object model)
		{
			ViewData.Model = model;
			using (var sw = new StringWriter())
			{
				var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
																							viewName);
				var viewContext = new ViewContext(ControllerContext, viewResult.View,
														ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);
				viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
				return sw.GetStringBuilder().ToString();
			}
		}

		protected JsonResult PartialViewWithErrorMessages(string viewName, object model, JsonRequestBehavior reqBehavior = JsonRequestBehavior.DenyGet)
		{
			return this.JsonWithErrorMessages(RenderRazorViewToString(viewName, model), reqBehavior);
		}

		private static void GetOPCUAEndPointInfoForGivenSite(SiteClass site, out MessageSecurityMode securityMode, out SecurityPolicy securityPolicy, out MessageEncoding messageEncoding, out UserIdentity userIdentity)
		{
			switch (site.SecurityMode.ToLower())
			{
				case "none":
					securityMode = MessageSecurityMode.None;
					break;
				case "signandencrypt":
					securityMode = MessageSecurityMode.SignAndEncrypt;
					break;
				default:
					throw new Exception("EnterpriseVisibilityConnectionInformation.SecurityMode : Invalid MessageSecurityMode " + site.SecurityMode);
			}

			switch (site.SecurityPolicy.ToLower())
			{
				case "none":
					securityPolicy = SecurityPolicy.None;
					break;
				case "basic256":
					securityPolicy = SecurityPolicy.Basic256;
					break;
				case "basic128rsa15":
					securityPolicy = SecurityPolicy.Basic128Rsa15;
					break;
				case "basic256sha256":
					securityPolicy = SecurityPolicy.Basic256Sha256;
					break;
				case "aes128_sha256_rsaoaep":
					securityPolicy = SecurityPolicy.Aes128_Sha256_RsaOaep;
					break;
				case "aes256_sha256_rsapss":
					securityPolicy = SecurityPolicy.Aes256_Sha256_RsaPss;
					break;
				default:
					throw new Exception("EnterpriseVisibilityConnectionInformation.SecurityPolicy : Invalid SecurityPolicy " + site.SecurityPolicy);
			}

			switch (site.MessageEncoding.ToLower())
			{
				case "binary":
					messageEncoding = MessageEncoding.Binary;
					break;
				case "xml":
					messageEncoding = MessageEncoding.Xml;
					break;
				default:
					throw new Exception("EnterpriseVisibilityConnectionInformation.MessageEncoding : Invalid MessageEncoding " + site.MessageEncoding);
			}

			switch (site.UserIdentityMethod.ToLower())
			{
				case "anonymous":
					userIdentity = new UserIdentity();
					break;
				case "username":
					userIdentity = new UserIdentity(site.UserId, site.UserPassword);
					break;
				case "certificate":
					userIdentity = new UserIdentity(new X509Certificate2(site.UserCertificatePath, site.UserPassword));
					break;
				default:
					throw new Exception("EnterpriseVisibilityConnectionInformation.UserIdentity : Invalid UserIdentityMethod " + site.UserIdentityMethod);
			}
		}

		/// <summary>
		/// This static method will update values for a list of point Tags at the Terminal using OPC UA and Point Service Manager.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="site">The current site.</param>
		/// <param name="pointValueList">The list of Point Tag Values to update.</param>
		public static void SetPointValues(SecurityClass security, SiteClass site, List<PointValue> pointValueList)
		{
			ClientSession opcuaSession = null;
			bool isEnterprise = false;

			try
			{
				isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsEnterpriseKey());

				if (isEnterprise
				&& !site.Enterprise
				&& !string.IsNullOrEmpty(site.ServerEndPoint))
				{
					string ErrorMessage = string.Empty;

					if (!string.IsNullOrWhiteSpace(site.ServerEndPoint) && isEnterprise)
					{
						Uri evUri = new Uri(site.ServerEndPoint);
						string localHostname = Dns.GetHostName();
						IPHostEntry evHost = Dns.GetHostEntry(evUri.DnsSafeHost);
						IPHostEntry localHost = Dns.GetHostEntry(localHostname);

						foreach (var evHostAddress in evHost.AddressList)
						{
							if (evHostAddress.ToString() == "::1" || evHostAddress.ToString() == "127.0.0.1")
							{
								ErrorMessage = $"In order for Enterprise Visibility to work correctly, Server End Point in the Site Opc Ua settings should be set to the Site Server hostname. Current Server End Point is {site.ServerEndPoint}. ";
							}
							foreach (var localHostAddress in localHost.AddressList)
							{
								if (localHostAddress.Equals(evHostAddress))
								{
									ErrorMessage = $"In order for Enterprise Visibility to work correctly, Server End Point in the Site Opc Ua settings should be set to the Site Server hostname. Current Server End Point is {site.ServerEndPoint}. ";
								}
							}
						}

						if (!string.IsNullOrEmpty(ErrorMessage))
						{
							throw new Exception(ErrorMessage);
						}
					}

					var uaApplication = GetUaApplication();

					GetOPCUAEndPointInfoForGivenSite(site, out MessageSecurityMode securityMode, out SecurityPolicy securityPolicy, out MessageEncoding messageEncoding, out UserIdentity userIdentity);

					opcuaSession = uaApplication.CreateSession(site.ServerEndPoint, securityMode, securityPolicy, messageEncoding, userIdentity);
					opcuaSession.Connect(false, true);
					List<WriteValue> writeValueList = new List<WriteValue>();

					// Update each point value in the list.
					foreach (PointValue pointValue in pointValueList)
					{
						var writeValue = new WriteValue { AttributeId = Opc.Ua.Attributes.Value };

						writeValue.NodeId = new NodeId(PointManager.CreateTagNodeID(pointValue.PointValueIdentifier.IdentityGuid), 2);

						if (pointValue.Value is PointCommandStatusListReference)
						{
							writeValue.Value.Value = pointValue.ValueXml;
						}
						else if (pointValue.Value is DeviceAlarmMapReference)
						{
							writeValue.Value.Value = pointValue.ValueXml;
						}
						else if (pointValue.Value is DateTimeOffset)
						{
							writeValue.Value.Value = ((DateTimeOffset)pointValue.Value).DateTime;
						}
						else if (pointValue.Value is TimeSpan)
						{
							writeValue.Value.Value = ((TimeSpan)pointValue.Value).Ticks;
						}
						else if (pointValue.Value is double || pointValue.Value is float)
						{
							var enterpriseVisibilityData = new EnterpriseVisibilityData(pointValue.EngineeringUnitsType
																						, pointValue.Units
																						, pointValue.Value
																						, pointValue.DecimalPlaces
																						, pointValue.Maximum
																						, pointValue.Minimum);

							var xmlserializer = CachingXmlSerializerFactory.Create(enterpriseVisibilityData.GetType());
							var stringWriter = new StringWriter();
							var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

							using (var writer = XmlWriter.Create(stringWriter, settings))
							{
								var emptyNameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
								xmlserializer.Serialize(writer, enterpriseVisibilityData, emptyNameSpaces);
								writeValue.Value.Value = stringWriter.ToString();
							}
						}
						else
						{
							writeValue.Value.Value = pointValue.Value;
						}

						writeValue.Value.SourceTimestamp = new DateTime(pointValue.SourceTimeStamp.Ticks, DateTimeKind.Utc);
						writeValue.Value.ServerTimestamp = writeValue.Value.SourceTimestamp;
						writeValue.Value.StatusCode = new StatusCode((uint)pointValue.Status);

						writeValueList.Add(writeValue);
					}

					opcuaSession.Write(writeValueList);
					opcuaSession.Dispose();

					// Write successful, send to database with Enterprise Visibility true to disable change tracking
					FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(security, pointValueList, true));
				}
				else
				{
					// Send to database with Enterprise Visibility false to enable change tracking
					FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(security, pointValueList, false));
				}
			}
			catch(Opc.Ua.ServiceResultException e)
			{
				EventLog eventLog = new EventLog("Application", ".", "FuelsManager");
				eventLog.WriteEntry("Error Setting Point Value : " + e.Message, EventLogEntryType.Information);

				throw (e);
			}
			catch (Exception e)
			{
				EventLog eventLog = new EventLog("Application", ".", "FuelsManager");
				eventLog.WriteEntry(e.Message, EventLogEntryType.Information);

				throw new Exception("Server Endpoint");
			}
			finally
			{
				if (opcuaSession != null)
				{
					opcuaSession.Dispose();
				}
			}
		}

		/// <summary>
		/// This static method will update point property value at the Terminal using OPC UA and DB using Point Properties 
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="site">The current site.</param>
		/// <param name="pointProperty">Point Property Value to update.</param>
		public static void SetPointPropertyValue(SecurityClass security, SiteClass site, PointProperty pointProperty)
		{
			ClientSession opcuaSession = null;

			try
			{
				var isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsEnterpriseKey());

				if (isEnterprise
					&& site.Enterprise == false
					&& string.IsNullOrEmpty(site.ServerEndPoint) == false)
				{

					string ErrorMessage = string.Empty;

					if (!string.IsNullOrWhiteSpace(site.ServerEndPoint) && isEnterprise)
					{
						Uri evUri = new Uri(site.ServerEndPoint);
						string localHostname = Dns.GetHostName();
						IPHostEntry evHost = Dns.GetHostEntry(evUri.DnsSafeHost);
						IPHostEntry localHost = Dns.GetHostEntry(localHostname);

						foreach (var evHostAddress in evHost.AddressList)
						{
							if (evHostAddress.ToString() == "::1" || evHostAddress.ToString() == "127.0.0.1")
							{
								ErrorMessage = $"In order for Enterprise Visibility to work correctly, Server End Point in the Site Opc Ua settings should be set to the Site Server hostname. Current Server End Point is {site.ServerEndPoint}. ";
							}
							foreach (var localHostAddress in localHost.AddressList)
							{
								if (localHostAddress.Equals(evHostAddress))
								{
									ErrorMessage = $"In order for Enterprise Visibility to work correctly, Server End Point in the Site Opc Ua settings should be set to the Site Server hostname. Current Server End Point is {site.ServerEndPoint}. ";
								}
							}
						}

						if (!string.IsNullOrEmpty(ErrorMessage))
						{
							throw new Exception(ErrorMessage);
						}
					}

					var uaApplication = GetUaApplication();

					GetOPCUAEndPointInfoForGivenSite(site, out MessageSecurityMode securityMode, out SecurityPolicy securityPolicy, out MessageEncoding messageEncoding, out UserIdentity userIdentity);

					opcuaSession = uaApplication.CreateSession(site.ServerEndPoint, securityMode, securityPolicy, messageEncoding, userIdentity);
					opcuaSession.Connect(false, true);
					List<WriteValue> writeValueList = new List<WriteValue>();

					var writeValue = new WriteValue { AttributeId = Opc.Ua.Attributes.Value };

					writeValue.NodeId = new NodeId(PointManager.CreateSettingNodeID(pointProperty.PointGuid, pointProperty.IdentityGuid, pointProperty.ID), 2);
					writeValue.Value.StatusCode = StatusCodes.Good;
					writeValue.Value.Value = pointProperty.ValueXml;
					writeValue.Value.ServerTimestamp = new DateTime(pointProperty.UpdatedDate.Ticks, DateTimeKind.Utc);
					writeValue.Value.SourceTimestamp = new DateTime(pointProperty.UpdatedDate.Ticks, DateTimeKind.Utc);

					writeValueList.Add(writeValue);

					opcuaSession.Write(writeValueList);
					opcuaSession.Dispose();

					// Write successful, send to database with bypassUpdatePointRowVersion (,,true,) to disable sync from seeing the changes
					FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(security, pointProperty, true, true));
				}
				else
				{
					// Send to database with bypassUpdatePointRowVersion (,,false,) to allow sync to pick-up the changes
					FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(security, pointProperty, false, true));
				}
			}
			catch (Opc.Ua.ServiceResultException e)
			{
				EventLog eventLog = new EventLog("Application", ".", "FuelsManager");
				eventLog.WriteEntry("Error Setting Point Property Value : " + e.Message, EventLogEntryType.Information);

				throw (e);
			}
			catch (Exception e)
			{
				EventLog eventLog = new EventLog("Application", ".", "FuelsManager");
				eventLog.WriteEntry(e.Message, EventLogEntryType.Information);

				throw new Exception("Server Endpoint");
			}
			finally
			{
				if (opcuaSession != null)
				{
					opcuaSession.Dispose();
				}
			}
		}

		/// <summary>
		/// Loads the OPC UA Client Configuration
		/// </summary>
		/// <returns></returns>
		public static async Task<ApplicationConfigurationBuilderEx> LoadApplicationConfiguration()
		{
			ApplicationConfigurationBuilderEx applicationConfigurationBuilder = new ApplicationConfigurationBuilderEx(ApplicationType.Client);

			bool opcUaAutoAcceptUntrustedCertificates;
			if (!bool.TryParse(ConfigurationManager.AppSettings["OpcUaAutoAcceptUntrustedCertificates"], out opcUaAutoAcceptUntrustedCertificates))
			{
				opcUaAutoAcceptUntrustedCertificates = true;
			}

			int opcUaTraceMasks;
			if (!int.TryParse(ConfigurationManager.AppSettings["OpcUaTraceMasks"], out opcUaTraceMasks))
			{
				opcUaTraceMasks = 1;
			}

			ushort opcUaCertificateLifeTime;
			if (!UInt16.TryParse(ConfigurationManager.AppSettings["OpcUaCertificateLifeTime"], out opcUaCertificateLifeTime))
			{
				opcUaCertificateLifeTime = 12;
			}

			await applicationConfigurationBuilder
				.Initialize("http://Varec.com/FuelsManager/OpcUaEditorController",
						"http://Varec.com/FuelsManager/OpcUaEditorController")
				.SetApplicationName("Opc Ua Editor Controller Client")
				.DisableHiResClock(true)
				.SetTransportQuotas(new Opc.Ua.TransportQuotas()
				{
					OperationTimeout = 120000,
					MaxStringLength = 1048576,
					MaxByteStringLength = 4194304,
					MaxArrayLength = 65535,
					MaxMessageSize = 4194304,
					MaxBufferSize = 65535,
					ChannelLifetime = 300000,
					SecurityTokenLifetime = 3600000
				})
				.AsClient()
					.SetDefaultSessionTimeout(610000)
					.SetMinSubscriptionLifetime(11000)
					.AddWellKnownDiscoveryUrls("opc.tcp://{0}:4840/UADiscovery")
				.AddSecurityConfigurationExt(
					"FuelsManager Opc Ua Editor Controller",
					"%CommonApplicationData%/Varec/FuelsManager/OpcUaEditorController/pki",
					"%CommonApplicationData%/Varec/FuelsManager/OpcUaEditorController/pki",
					"%CommonApplicationData%/Varec/FuelsManager/OpcUaEditorController/pki")
					.SetRejectSHA1SignedCertificates(false)
					.SetUserRoleDirectory("%CommonApplicationData%/Varec/FuelsManager/OpcUaEditorController/userRoles")
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
				.SetOutputFilePath("%CommonApplicationData%/Varec/FuelsManager/OpcUaEditorController/logs/OpcUaEditorController.log")
				.SetDeleteOnLoad(true)
				.Create().ConfigureAwait(false);


			applicationConfigurationBuilder.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates = opcUaAutoAcceptUntrustedCertificates;
			applicationConfigurationBuilder.ApplicationConfiguration.SecurityConfiguration.AddAppCertToTrustedStore = true;
			applicationConfigurationBuilder.ApplicationConfiguration.CertificateValidator.AutoAcceptUntrustedCertificates = opcUaAutoAcceptUntrustedCertificates;

			return applicationConfigurationBuilder;
		}
	}
}
