
namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using System.Security.Cryptography.X509Certificates;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using System.Text;
	using System.Configuration;
	using System.Threading.Tasks;
	using System.Xml;
	using FMBusinessObjects.Constants;
	using FuelsManager.FMWebApp;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
	using Softing.Opc.Ua.Configuration;
	using System.Runtime.Caching;

	using global::FMWebApp;

	public class OpcUaEditorController : FMBaseControllerEx
	{

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult OpcUaBrowser(Guid pointTagGuid)
		{

			try
			{
				var uaApplication = GetUaApplication();

				var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
				if (model == null)
				{
					model = new OpcUaEditorModel();
					this.Session[OpcUaEditorModel.SessionKey] = model;

					if (model.SelectionMode == "Local")
					{
						model.Domain = string.Empty;
						model.Server = "localhost";
					}
				}

				model.Application = uaApplication;

				if (pointTagGuid != Guid.Empty)
				{
					var pointTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(
							  x => x.Get(this.Security, pointTagGuid));

					model.PointTagGuid = pointTagGuid;
					model.PointTagId = pointTag.ID;

					// EndpointUrl, if it is to be changed start selection from local
					if (!string.IsNullOrEmpty(pointTag.OpcUaServerEndPoint))
					{
						model.EndpointUrl = pointTag.OpcUaServerEndPoint;
						model.SelectionMode = "Local";
						model.Domain = string.Empty;
						model.Server = "localhost";
						model.OpcUaServer = string.Empty;

						MessageSecurityMode messageSecurityMode;
						if (!Enum.TryParse<MessageSecurityMode>(pointTag.OpcUaSecurityMode, out messageSecurityMode))
						{
							messageSecurityMode = MessageSecurityMode.None;
						}
						model.SecurityMode = messageSecurityMode;

						SecurityPolicy securityPolicy;
						if (!Enum.TryParse<SecurityPolicy>(pointTag.OpcUaSecurityPolicy, out securityPolicy))
						{
							securityPolicy = SecurityPolicy.None;
						}
						model.SecurityPolicy = securityPolicy;

						MessageEncoding messageEncoding;
						if (!Enum.TryParse<MessageEncoding>(pointTag.OpcUaMessageEncoding, out messageEncoding))
						{
							messageEncoding = MessageEncoding.Binary;
						}
						model.MessageEncoding = messageEncoding;

						UserTokenType userTokenType;
						if (!Enum.TryParse<UserTokenType>(pointTag.OpcUaUserIdentityMethod, out userTokenType))
						{
							userTokenType = UserTokenType.Anonymous;
						}
						model.UserTokenType = userTokenType;

						model.UserName = pointTag.OpcUaUserId;
						model.UserPassword = pointTag.OpcUaUserPassword;
						model.CertificatePath = pointTag.OpcUaUserCertificatePath;
						model.CertificatePassword = pointTag.OpcUaUserPassword;
						model.PointTagOpcUaBrowsePath = pointTag.OpcUaBrowsePath;
						model.PointTagOpcUaNodeId = pointTag.OpcUaNodeId;

					}

					if (pointTag.OpcUaPublishingInterval != null)
					{
						model.PointTagOpcUaPublishingInterval = pointTag.OpcUaPublishingInterval.Value;
					}

					model.PointTagOpcUaWriteHoldoff = pointTag.OpcUaWriteHoldoffTime;

					model.PointTagOpcUaWritePeriodicUpdateInterval = pointTag.OpcUaWritePeriodicUpdateInterval;

					model.PointTagOpcUaIsReadable = pointTag.OpcUaIsReadable;
					model.PointTagInput = pointTag.Input;
					if (pointTag.Input)
					{
						model.PointTagOpcUaIsReadable = true;
					}
					model.PointTagValueType = pointTag.ValueTypeString;
					model.PointTagOpcUaServerDataType = (pointTag.OpcUaServerDataType.HasValue) ? pointTag.OpcUaServerDataType.Value.ToString() : "0";
					if (!IsNumericDatatype(pointTag.ValueType))
					{
						// non numeric data type
						model.PointTagOPCClientDeadband = 0.0;
						model.PointTagOPCClientHoldoff = 0;
						model.PointTagOPCClientDisableFilter = true;
					}
					else
					{
						model.PointTagOPCClientDeadband = pointTag.Deadband;
						model.PointTagOPCClientHoldoff = pointTag.Holdoff;
						model.PointTagOPCClientDisableFilter = false;
					}
				}

				model.ServerNodeIdList.Clear();

				if (!string.IsNullOrEmpty(model.EndpointUrl))
				{
					var session = this.CreateOpcUaSession(model);
				   var cache = MemoryCache.Default;
					cacheSession(session, cache);

					{
						try
						{
							session.Connect(false, true);
						}
						catch (Exception ex)
						{
							this.ModelState.AddModelError("Connection", "Cannot Connect to OPC Server: " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
							FMFormBase.LogErrorMessage("Cannot Connect to OPC Server: " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
							return PartialViewWithErrorMessages("OpcUaEnhancedModalEditor", model, JsonRequestBehavior.AllowGet);
							//return PartialViewWithErrorMessages("OpcUaBrowser", model, JsonRequestBehavior.AllowGet);
						}
						var referenceDescriptons = session.Browse(null, null);

						if (referenceDescriptons != null && referenceDescriptons.Count > 0)
						{
							var readValueIdList = new List<ReadValueId>();

							foreach (var referenceDescription in referenceDescriptons)
							{
								if (referenceDescription.ReferenceTypeName == "HasTypeDefinition")
								{
									continue;
								}

								readValueIdList.Add(new ReadValueId
								{
									NodeId = (NodeId)referenceDescription.NodeId,
									AttributeId = Attributes.DataType
								});
							}

							var readValueList = session.Read(readValueIdList, 0.0, new TimestampsToReturn());

							var index = 0;

							foreach (var referenceDescription in referenceDescriptons)
							{
								if (referenceDescription.ReferenceTypeName == "HasReferenceDescription")
								{
									continue;
								}

								if (referenceDescription.ReferenceTypeName == "HasTypeDefinition")
								{
									continue;
								}


								string dataType = "0";
								if (readValueList != null
								&& readValueList.Count > index)
								{
									var readValue = readValueList[index++];
									if (readValue != null
									&& readValue.Value != null
									&& readValue.Value is NodeId)
									{
										dataType = ((NodeId)readValue.Value).Identifier.ToString();
									}
								}

								model.ServerNodeIdList.Add(new OpcUaNode(referenceDescription.BrowseName.Name, referenceDescription.DisplayName.Text, referenceDescription.NodeId.ToString(), dataType));
							}
						}
					}
				}
				return PartialViewWithErrorMessages("OpcUaEnhancedModalEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				FMFormBase.LogErrorMessage(except.Message + (except.InnerException != null ? except.InnerException.Message : ""));
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public ActionResult OpcUaBrowser()
		{
			return null;
		}

		[HttpGet]
		public ActionResult OpcUaServers()
		{
			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				this.ModelState.AddModelError("", "No Model in Session");
			}

			return PartialViewWithErrorMessages("OpcUaServers", model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SelectionModeSelectionChanged(string selectionMode)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;

			model.SelectionMode = selectionMode;
			model.Domain = string.Empty;
			model.Server = string.Empty;
			model.OpcUaServer = string.Empty;
			model.OpcUaServerSecurity = string.Empty;
			model.EndpointUrl = string.Empty;
			disposeSession();

			if (model.SelectionMode == "Local")
			{
				model.Server = "localhost";
			}

			List<string> domainList = new List<string>();
			try
			{
				domainList = model.GetDomainList();
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(domainList);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult DomainSelectionChanged(string domain)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
			model.Domain = string.Empty;
			model.Server = string.Empty;
			model.OpcUaServer = string.Empty;
			model.OpcUaServerSecurity = string.Empty;
			model.EndpointUrl = string.Empty;
		   disposeSession();


			if (domain != this.GetTranslatedText("--Select--"))
			{
				model.Domain = domain;
			}

			List<string> serverList = new List<string>();
			try
			{
				serverList = model.GetServerList();
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(serverList);
		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult ServerSelectionChanged(string server)
		{
			List<string> serverList = new List<string>();

			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
		   disposeSession();

			model.Server = string.Empty;
			model.OpcUaServer = string.Empty;
			model.OpcUaServerSecurity = string.Empty;
			model.EndpointUrl = string.Empty;
			if (server != this.GetTranslatedText("--Select--"))
			{
				model.Server = server;
			}
			//set the return as success but no need for message since we won't display it

			try
			{
				serverList = model.GetOpcUaServerList();
			}
			catch (Exception e)
			{
				this.OnError(e.Message);
			}

			return this.JsonWithErrorMessages(serverList);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult OpcUaServerSelectionChanged(string opcUaServer)
		{
			var uaApplication = GetUaApplication();
			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;

			model.OpcUaServer = string.Empty;
			model.OpcUaServerSecurity = string.Empty;
			model.EndpointUrl = string.Empty;
			if (opcUaServer != this.GetTranslatedText("--Select--"))
			{
				model.OpcUaServer = opcUaServer;
			}
			disposeSession();

			List<string> securityList = new List<string>();
			try
			{
				securityList = model.GetOpcUaServerSecurityList();
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(securityList);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult OpcUaServerSecuritySelectionChanged(string opcUaServerSecurity)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
			model.OpcUaServerSecurity = string.Empty;
			model.SecurityMode = MessageSecurityMode.None;
			model.SecurityPolicy = SecurityPolicy.None;
			model.MessageEncoding = MessageEncoding.Binary;
			disposeSession();

			if (opcUaServerSecurity != "null"
			&& opcUaServerSecurity != this.GetTranslatedText("--Select--"))
			{
				model.OpcUaServerSecurity = opcUaServerSecurity;
			}

			model.GetOpcUaServerSecurityList();

			var list = new List<object>();
			try
			{
				list.Add(model.EndpointUrl);
				list.Add(model.GetSecurityModeList());
				list.Add(model.SecurityMode.ToString());
				list.Add(model.GetSecurityPolicyList());
				list.Add(model.SecurityPolicy.ToString());
				list.Add(model.GetMessageEncodingList());
				list.Add(model.MessageEncoding.ToString());
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(list);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SecurityModeSelectionChanged(string securityMode)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				this.ModelState.AddModelError("", "No Model in Session");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			model.Application = uaApplication;
			model.SecurityMode = (MessageSecurityMode)Enum.Parse(typeof(MessageSecurityMode), securityMode, true);
			disposeSession();

			List<KeyValuePair<string, string>> securityPolicy = new List<KeyValuePair<string, string>>();
			try
			{
				securityPolicy = model.GetSecurityPolicyList();
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(securityPolicy, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SecurityPolicySelectionChanged(string securityPolicy)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
			disposeSession();

			try
			{
				model.SecurityPolicy = (SecurityPolicy)Enum.Parse(typeof(SecurityPolicy), securityPolicy, true);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult MessageEncodingSelectionChanged(string messageEncoding)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
			disposeSession();

			try
			{
				model.MessageEncoding = (MessageEncoding)Enum.Parse(typeof(MessageEncoding), messageEncoding, true);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult UserTokenTypeSelectionChanged(string userTokenType)
		{
			var uaApplication = GetUaApplication();


			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
			disposeSession();

			try
			{
				model.UserTokenType = (UserTokenType)Enum.Parse(typeof(UserTokenType), userTokenType, true);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null);

		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult UserNameChanged(string userName)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
			disposeSession();

			try
			{
				model.UserName = userName;
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null);

		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult CertificatePathChanged(string certificatePath)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
		   disposeSession();

			try
			{
				model.CertificatePath = certificatePath;

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null);

		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult UserPasswordChanged(string userPassword)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
		   disposeSession();

			try
			{
				model.UserPassword = userPassword;
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null);

		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult CertificatePasswordChanged(string certificatePassword)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
		   disposeSession();

			try
			{
				model.CertificatePassword = certificatePassword;
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult EndpointChanged(string endpointUrl)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;
			disposeSession();

         try
			{
				model.EndpointUrl = endpointUrl;
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null);
		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult ServerAddressSpaceSelectionChanged(string nodeIdText)
		{
			var uaApplication = GetUaApplication();

			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.Application = uaApplication;

			var nodeIdList = new List<OpcUaNode>();
			try
			{
				if (!string.IsNullOrEmpty(model.EndpointUrl))
				{
				var cache = MemoryCache.Default;
				var session = cache[this.Security.UserID + "OpcSession"] as ClientSession;
				if (session == null || session?.CurrentState != State.Active) 
				{
               session = this.CreateOpcUaSession(model);
				   cacheSession(session, cache);
            }
					{
						session.Connect(false, true);

						IList<ReferenceDescriptionEx> referenceDescriptons;

						if (string.IsNullOrEmpty(nodeIdText))
						{
							referenceDescriptons = session.Browse(null,null);
						}
						else
						{
							referenceDescriptons = session.Browse(new NodeId(nodeIdText));
						}

						if (referenceDescriptons != null && referenceDescriptons.Count > 0)
						{

							var readValueIdList = new List<ReadValueId>();

							foreach (var referenceDescription in referenceDescriptons)
							{
								if(referenceDescription.ReferenceTypeName == "HasTypeDefinition")
								{
									continue;
								}

								readValueIdList.Add(new ReadValueId
								{
									NodeId = (NodeId)referenceDescription.NodeId,
									AttributeId = Attributes.DataType
								});
							}

							var readValueList = session.Read(readValueIdList, 0.0, new TimestampsToReturn());

							var index = 0;

							foreach (var referenceDescription in referenceDescriptons)
							{
								if (referenceDescription.ReferenceTypeName == "HasReferenceDescription")
								{
									continue;
								}

								if (referenceDescription.ReferenceTypeName == "HasTypeDefinition")
								{
									continue;
								}

								string dataType = "0";
								if (readValueList != null
								&& readValueList.Count > index)
								{
									var readValue = readValueList[index++];
									if (readValue != null
									&& readValue.Value != null
									&& readValue.Value is NodeId)
									{
										dataType = ((NodeId)readValue.Value).Identifier.ToString();
									}
								}

								nodeIdList.Add(new OpcUaNode(referenceDescription.BrowseName.Name, referenceDescription.DisplayName.Text, referenceDescription.NodeId.ToString(), dataType));
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(nodeIdList, JsonRequestBehavior.AllowGet);
		}

		public ActionResult SaveChanges(string opcUaBrowsePath,
												string opcUaNodeId,
												string opcUaPublishingInterval,
												string opcUaWriteHoldoffTime,
												string opcUaWritePeriodicUpdateInterval,
												bool opcUaIsReadable,
												string opcUaServerDataType,
												string opcUaHoldoff,
												string opcUaDeadband)
		{
			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model == null)
			{
				this.ModelState.AddModelError("", "No Model in Session");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			try
			{
				var pointTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, model.PointTagGuid));

				pointTag.OpcUaServerEndPoint = model.EndpointUrl;
				pointTag.OpcUaSecurityMode = model.SecurityMode.ToString();
				pointTag.OpcUaSecurityPolicy = model.SecurityPolicy.ToString();
				pointTag.OpcUaMessageEncoding = model.MessageEncoding.ToString();
				pointTag.OpcUaUserIdentityMethod = model.UserTokenType.ToString();
				pointTag.OpcUaUserId = model.UserName;
				pointTag.OpcUaUserCertificatePath = model.CertificatePath;

				try
				{
					pointTag.Deadband = System.Convert.ToDouble(opcUaDeadband);
				}
				catch (Exception)
				{
					pointTag.Deadband = 0;
				}

				try
				{
					pointTag.Holdoff = System.Convert.ToInt32(opcUaHoldoff);
				}
				catch (Exception)
				{
					pointTag.Holdoff = 0;
				}

				if (model.UserTokenType == UserTokenType.UserName)
				{
					pointTag.OpcUaUserPassword = model.UserPassword;
				}
				else if (model.UserTokenType == UserTokenType.Certificate)
				{
					pointTag.OpcUaUserPassword = model.CertificatePassword;
				}

				pointTag.OpcUaBrowsePath = opcUaBrowsePath;
				pointTag.OpcUaNodeId = opcUaNodeId;
				int publishingInterval;

				if (!Int32.TryParse(opcUaPublishingInterval, out publishingInterval))
				{
					throw new Exception(this.GetTranslatedText("OpcUaEditorController|SaveChanges - Invalid Publishing Interval"));
				}

				if (!pointTag.Input)
				{


					if (!string.IsNullOrEmpty(opcUaWriteHoldoffTime))
					{
						int writeMHoldoffTime;
						if (!Int32.TryParse(opcUaWriteHoldoffTime, out writeMHoldoffTime))
						{
							throw new Exception(this.GetTranslatedText("OpcUaEditorController|SaveChanges - Invalid Write Holdoff"));
						}

						if (writeMHoldoffTime <= 0)
						{
							throw new Exception(this.GetTranslatedText("OpcUaEditorController|SaveChanges - Write Holdoff must be greater than 0"));
						}


						pointTag.OpcUaWriteHoldoffTime = writeMHoldoffTime;
					}
					else
					{
						pointTag.OpcUaWriteHoldoffTime = null;
					}


					if (!string.IsNullOrEmpty(opcUaWritePeriodicUpdateInterval))
					{
						int writePeriodicUpdateInterval;
						if (!Int32.TryParse(opcUaWritePeriodicUpdateInterval, out writePeriodicUpdateInterval))
						{
							throw new Exception(this.GetTranslatedText("OpcUaEditorController|SaveChanges - Invalid Write Periodic Update"));
						}

						if (writePeriodicUpdateInterval <= 0)
						{
							throw new Exception(this.GetTranslatedText("OpcUaEditorController|SaveChanges - Write Periodic Update must be greater than 0"));
						}


						pointTag.OpcUaWritePeriodicUpdateInterval = writePeriodicUpdateInterval;
					}
					else
					{
						pointTag.OpcUaWritePeriodicUpdateInterval = null;
					}
				}
				else
				{
					pointTag.OpcUaWriteHoldoffTime = null;
					pointTag.OpcUaWritePeriodicUpdateInterval = null;
				}

				if (pointTag.OpcUaWritePeriodicUpdateInterval.HasValue
				&& pointTag.OpcUaWriteHoldoffTime.HasValue)
				{
					if (pointTag.OpcUaWriteHoldoffTime.Value >= pointTag.OpcUaWritePeriodicUpdateInterval.Value)
					{
						throw new Exception(this.GetTranslatedText("OpcUaEditorController|SaveChanges - Write Periodic Update must be greater than Write Holdoff"));
					}
				}

				pointTag.OpcUaPublishingInterval = publishingInterval;
				pointTag.OpcUaIsReadable = opcUaIsReadable;
				pointTag.OpcUaServerDataType = Int32.Parse(opcUaServerDataType);

				if (pointTag.Input)
				{
					pointTag.InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa;
				}

				if (!pointTag.IsForced()
				&& pointTag.Value != null
				&& pointTag.Value is ValueType)
				{
					pointTag.Value = null;
					pointTag.Status = StatusCodes.Bad;
				}

				FMChannelHelper.MakeCall<IPointTags>(x => x.Modify(this.Security, pointTag));

				FMChannelHelper.MakeCall<IPoints>(x => x.UpdateRowVersion(this.Security, pointTag.PointGuid));

				model.PointTagOpcUaBrowsePath = pointTag.OpcUaBrowsePath;
				model.PointTagOpcUaNodeId = pointTag.OpcUaNodeId;

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			if (this.ModelState.IsValid)
			{
				this.AddSuccess("Save Successful");
			}
			return this.JsonWithErrorMessages(null);
		}

	   public ActionResult Close()
		{ 
			var model = this.Session[OpcUaEditorModel.SessionKey] as OpcUaEditorModel;
			if (model != null)
			{
				 disposeSession();
			}
		   return this.JsonWithErrorMessages(null);
      }


		private ClientSession CreateOpcUaSession(OpcUaEditorModel model)
		{
			UserIdentity userIdentity = null;

			switch (model.UserTokenType)
			{
				case UserTokenType.Anonymous:
					userIdentity = new UserIdentity();
					break;
				case UserTokenType.UserName:
					try
					{
						userIdentity = new UserIdentity(model.UserName, model.UserPassword);
					}
					catch (Exception ex)
					{
						this.ModelState.AddModelError("CreateOpcUaSession", "Cannot create User Name Identity: " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : " switched to Anonymous."));
						model.UserTokenType = UserTokenType.Anonymous;
						userIdentity = new UserIdentity();
					}
					break;
				case UserTokenType.Certificate:
					try
					{
						userIdentity = new UserIdentity(new X509Certificate2(model.CertificatePath, model.CertificatePassword));
					}
					catch (Exception ex)
					{
						this.ModelState.AddModelError("CreateOpcUaSession", "Cannot create Certificate Identity: " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : " switched to Anonymous."));
						model.UserTokenType = UserTokenType.Anonymous;
						userIdentity = new UserIdentity();
					}
					break;
			}

			var uaApplication = GetUaApplication();

			var opcuaSession = uaApplication.CreateSession(model.EndpointUrl, model.SecurityMode, model.SecurityPolicy, model.MessageEncoding, userIdentity, null);


			return opcuaSession;
		}

		/// <summary>
		/// Handles the certificate validation error event.
		/// This event is triggered when the certificate received from the server during connection is not trusted.
		/// </summary>
		private void Application_CertificateValidation(object sender, CertificateValidationEventArgs e)
		{
			try
			{
				CertificateValidator validator = (CertificateValidator)sender;
				this.HandleCertificateValidationError(validator, e);
			}
			catch (Exception ex)
			{
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry("OpcUaEditorController.Application_CertificateValidation: " + ex.Message, FMEventLogEntryType.Error));
			}
		}



		/// <summary>
		/// Handles a certificate validation error.
		/// </summary>
		/// <param name="validator">The validator (not used).</param>
		/// <param name="e">The <see cref="Softing.Opc.Ua.Sdk.CertificateValidationEventArgs"/> instance event arguments provided when a certificate validation error occurs.</param>
		public void HandleCertificateValidationError(CertificateValidator validator, CertificateValidationEventArgs e)
		{
			var buffer = new StringBuilder();

			buffer.AppendFormat("Certificate could not be validated\r\n\r\n");
			buffer.AppendFormat("Subject: {0}\r\n", e.Certificate.Subject);
			buffer.AppendFormat("Issuer: {0}\r\n", (e.Certificate.Subject == e.Certificate.Issuer) ? "Self-signed" : e.Certificate.Issuer);
			buffer.AppendFormat("Valid From: {0}\r\n", e.Certificate.NotBefore);
			buffer.AppendFormat("Valid To: {0}\r\n", e.Certificate.NotAfter);
			buffer.AppendFormat("Thumbprint: {0}\r\n\r\n", e.Certificate.Thumbprint);

			bool acceptNonValidateCertificates = System.Convert.ToBoolean(ConfigurationManager.AppSettings["OpcUaAcceptNonValidatedCertificates"]);
			if (acceptNonValidateCertificates)
			{
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry("OpcUaEditorController.HandleCertificateValidationError: Untrusted certificate accepted. " + buffer.ToString(), FMEventLogEntryType.Error));
			}
		}

		public bool IsNumericDatatype(System.Type valueType)
		{
			switch (System.Type.GetTypeCode(valueType))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}
	   private void cacheSession(ClientSession session, MemoryCache cache)
        {
				var cacheItemPolicy = new CacheItemPolicy()
				{
					 SlidingExpiration = TimeSpan.FromSeconds(15),
                RemovedCallback = new CacheEntryRemovedCallback(CacheRemovedCallback)
				};
				cache.Set(this.Security.UserID + "OpcSession", session, cacheItemPolicy);
		  }

		private void CacheRemovedCallback(CacheEntryRemovedArguments arguments)
		{
            ClientSession session = (ClientSession)arguments.CacheItem.Value;
				if (session !=null)
            {
					 session.Dispose();
				}
		}
		  private void disposeSession()
		{
				var cache = MemoryCache.Default;
				var session = cache[this.Security.UserID + "OpcSession"] as ClientSession;
				if (session != null) 			
				{ session.Dispose(); cache.Remove(this.Security.UserID + "OpcSession");}
		}
	}
}