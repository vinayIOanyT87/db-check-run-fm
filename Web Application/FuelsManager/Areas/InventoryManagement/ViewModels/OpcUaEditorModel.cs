

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using Areas.Controllers;
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.DirectoryServices;
	using System.Xml;
	using System.Xml.Serialization;
	using System.DirectoryServices.ActiveDirectory;
	using System.Runtime.InteropServices;
	using System.Web.Mvc;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;


	internal class NetApi
	{
		public const int ErrorSuccess = 0;

		[DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int NetGetJoinInformation(string server, out IntPtr domain, out NetJoinStatus status);

		[DllImport("Netapi32.dll")]
		public static extern int NetApiBufferFree(IntPtr Buffer);

		[DllImport("Netapi32.dll", EntryPoint = "NetServerEnum", CharSet = CharSet.Ansi)]
		public static extern Int32 NetServerEnum(
				[MarshalAs(UnmanagedType.LPWStr)] String serverName,
				Int32 level,
				out IntPtr bufferPtr,
				UInt32 prefMaxLen,
				ref Int32 entriesRead,
				ref Int32 totalEntries,
				UInt32 serverType,
				[MarshalAs(UnmanagedType.LPWStr)] String domain,
				IntPtr handle);


		public enum NetJoinStatus
		{
			NetSetupUnknownStatus = 0,
			NetSetupUnjoined,
			NetSetupWorkgroupName,
			NetSetupDomainName
		}
	}

	public class EnumerateLanMachines
	{
		public const UInt32 SUCCESS = 0;
		public const UInt32 FAIL = 234;
		public const UInt32 MAX_PREFERRED_LENGTH = 0xFFFFFFFF;
		//public ArrayList machines = new ArrayList ( );

		enum ServerTypes : uint
		{
			WorkStation = 0x00000001,
			Server = 0x00000002
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct MachineInfo
		{
			[MarshalAs(UnmanagedType.U4)]
			public UInt32 platformId;

			[MarshalAs(UnmanagedType.LPWStr)]
			public String serverName;
		}

		public enum Platform
		{
			PLATFORM_ID_DOS = 300,
			PLATFORM_ID_OS2 = 400,
			PLATFORM_ID_NT = 500,
			PLATFORM_ID_OSF = 600,
			PLATFORM_ID_VMS = 700
		}

		public void enumerateMachines(List<string> serverList, string domain)
		{
			IntPtr buffer = new IntPtr();
			IntPtr tmpBuffer = IntPtr.Zero;
			int totalEntries = 0;
			int entriesRead = 0;
			int result;

			try
			{
				result = NetApi.NetServerEnum(null, 100, out buffer, MAX_PREFERRED_LENGTH, ref entriesRead, ref totalEntries, (uint)0xFFFFFFFF, domain, IntPtr.Zero);

				MachineInfo machineInfo;

				if (result != FAIL)
				{
					for (int i = 0; i < entriesRead; ++i)
					{
						tmpBuffer = (IntPtr)(ulong)buffer + i * Marshal.SizeOf(typeof(MachineInfo));

						machineInfo = (MachineInfo)Marshal.PtrToStructure(tmpBuffer, typeof(MachineInfo));

						serverList.Add(machineInfo.serverName);
					}
				}
			}
			finally
			{
				NetApi.NetApiBufferFree(buffer);
			}
		}
	}

	[Serializable]
	public class OpcUaNode
	{
		public string BrowseName { get; set; }
		public string DisplayName { get; set; }
		public string Id { get; set; }
		public string DataType { get; set; }

		public OpcUaNode(string browseName, string displayName, string id, string dataType)
		{
			this.BrowseName = browseName;
			this.DisplayName = displayName;
			this.Id = id;
			this.DataType = dataType;
		}
	}



	[Serializable]
	public class OpcUaEditorModel
	{
		[NonSerialized]
		public UaApplication Application;

      public string SelectionMode { get; set; }
		public string Domain { get; set; }
		public string Server { get; set; }
		public string OpcUaServer { get; set; }
		public string OpcUaServerSecurity { get; set; }
		public string EndpointUrl { get; set; }
		public MessageSecurityMode SecurityMode { get; set; }
		public SecurityPolicy SecurityPolicy { get; set; }
		public MessageEncoding MessageEncoding { get; set; }
		public UserTokenType UserTokenType { get; set; }
		public string UserName { get; set; }
		public string CertificatePath { get; set; }
		public string UserPassword { get; set; }
		public string CertificatePassword { get; set; }
		public string PointTagId { get; set; }
		public string PointTagOpcUaBrowsePath { get; set; }
		public int PointTagOpcUaPublishingInterval { get; set; }
		public bool PointTagOpcUaIsReadable { get; set; }
		public int? PointTagOpcUaWriteHoldoff { get; set; }
		public int? PointTagOpcUaWritePeriodicUpdateInterval { get; set; }
		public string PointTagOpcUaServerDataType { get; set; }
		public bool PointTagInput { get; set; }
		public string PointTagOpcUaNodeId { get; set; }
		public Guid PointTagGuid { get; set; }
		public string PointTagValueType { get; set; }
		public List<OpcUaNode> ServerNodeIdList { get; set; }

		public double PointTagOPCClientDeadband { get; set; }
		public int PointTagOPCClientHoldoff { get; set; }
		public bool PointTagOPCClientDisableFilter { get; set; }

		public const string SessionKey = "OpcUaEditorContext";


		public OpcUaEditorModel()
		{
			this.SelectionMode = "Local";
			this.SecurityMode = MessageSecurityMode.None;
			this.SecurityPolicy = SecurityPolicy.None;
			this.MessageEncoding = MessageEncoding.Binary;
			this.UserTokenType = UserTokenType.Anonymous;
			this.ServerNodeIdList = new List<OpcUaNode>();
		}

		public List<KeyValuePair<string, string>> GetSelectionModeList()
		{
			var selectionModeList = new List<KeyValuePair<string, string>>();

			selectionModeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Local"), "Local"));
			selectionModeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Remote"), "Remote"));
			selectionModeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Manual"), "Manual"));

			return selectionModeList;
		}

		public List<KeyValuePair<string, string>> GetServerDataTypeList()
		{
			var serverDataTypeList = new List<KeyValuePair<string, string>>();

			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.Null.ToString(), ((int)BuiltInType.Null).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.Boolean.ToString(), ((int)BuiltInType.Boolean).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.SByte.ToString(), ((int)BuiltInType.SByte).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.Byte.ToString(), ((int)BuiltInType.Byte).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.Int16.ToString(), ((int)BuiltInType.Int16).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.UInt16.ToString(), ((int)BuiltInType.UInt16).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.Int32.ToString(), ((int)BuiltInType.Int32).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.UInt32.ToString(), ((int)BuiltInType.UInt32).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.Int64.ToString(), ((int)BuiltInType.Int64).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.UInt64.ToString(), ((int)BuiltInType.UInt64).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.Float.ToString(), ((int)BuiltInType.Float).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.Double.ToString(), ((int)BuiltInType.Double).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.String.ToString(), ((int)BuiltInType.String).ToString()));
			serverDataTypeList.Add(new KeyValuePair<string, string>(BuiltInType.DateTime.ToString(), ((int)BuiltInType.DateTime).ToString()));

			return serverDataTypeList;
		}

		public List<KeyValuePair<string, string>> GetSecurityModeList()
		{
			var securityModeList = new List<KeyValuePair<string, string>>();

			if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
			|| this.OpcUaServerSecurity.Contains("None -"))
			{
				securityModeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("None"), "None"));
			}

			if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
			|| this.OpcUaServerSecurity.Contains("Sign -"))
			{
				securityModeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Sign"), "Sign"));
			}

			if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
			|| this.OpcUaServerSecurity.Contains("SignAndEncrypt -"))
			{
				securityModeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("SignAndEncrypt"), "SignAndEncrypt"));
			}

			if (!string.IsNullOrEmpty(this.OpcUaServerSecurity))
			{
				if (this.OpcUaServerSecurity.Contains("None -"))
				{
					this.SecurityMode = MessageSecurityMode.None;
				}

				else if (this.OpcUaServerSecurity.Contains("Sign -"))
				{
					this.SecurityMode = MessageSecurityMode.Sign;
				}

				else if (this.OpcUaServerSecurity.Contains("SignAndEncrypt -"))
				{
					this.SecurityMode = MessageSecurityMode.SignAndEncrypt;
				}
			}

			return securityModeList;
		}


		public List<KeyValuePair<string, string>> GetSecurityPolicyList()
		{
			var securityPolicyList = new List<KeyValuePair<string, string>>();

			if (this.SecurityMode == MessageSecurityMode.None)
			{
				securityPolicyList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("None"), "None"));
			}

			if (this.SecurityMode != MessageSecurityMode.None)
			{
				if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
				|| this.OpcUaServerSecurity.Contains("Basic256"))
				{
					securityPolicyList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Basic256"), "Basic256"));
				}

				if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
				|| this.OpcUaServerSecurity.Contains("Basic128Rsa15"))
				{
					securityPolicyList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Basic128Rsa15"), "Basic128Rsa15"));
				}

				if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
				|| this.OpcUaServerSecurity.Contains("Basic256Sha256"))
				{
					securityPolicyList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Basic256Sha256"), "Basic256Sha256"));
				}

				if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
				|| this.OpcUaServerSecurity.Contains("Aes128_Sha256_RsaOaep"))
				{
					securityPolicyList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Aes128_Sha256_RsaOaep"), "Aes128_Sha256_RsaOaep"));
				}

				if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
				|| this.OpcUaServerSecurity.Contains("Aes256_Sha256_RsaPss"))
				{
					securityPolicyList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Aes256_Sha256_RsaPss"), "Aes256_Sha256_RsaPss"));
				}


			}

			if (!string.IsNullOrEmpty(this.OpcUaServerSecurity))
			{
				if (this.OpcUaServerSecurity.EndsWith("None"))
				{
					this.SecurityPolicy = SecurityPolicy.None;
				}

				else if (this.OpcUaServerSecurity.EndsWith("Basic256"))
				{
					this.SecurityPolicy = SecurityPolicy.Basic256;
				}

				else if (this.OpcUaServerSecurity.EndsWith("Basic128Rsa15"))
				{
					this.SecurityPolicy = SecurityPolicy.Basic128Rsa15;
				}

				else if (this.OpcUaServerSecurity.EndsWith("Basic256Sha256"))
				{
					this.SecurityPolicy = SecurityPolicy.Basic256Sha256;
				}

				else if (this.OpcUaServerSecurity.EndsWith("Aes128_Sha256_RsaOaep"))
				{
					this.SecurityPolicy = SecurityPolicy.Aes128_Sha256_RsaOaep;
				}

				else if (this.OpcUaServerSecurity.EndsWith("Aes256_Sha256_RsaPss"))
				{
					this.SecurityPolicy = SecurityPolicy.Aes256_Sha256_RsaPss;
				}


			}


			return securityPolicyList;
		}

		public List<KeyValuePair<string, string>> GetMessageEncodingList()
		{
			var messageEncodingList = new List<KeyValuePair<string, string>>();

			messageEncodingList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Binary"), "Binary"));

			if (string.IsNullOrEmpty(this.OpcUaServerSecurity)
			|| this.OpcUaServerSecurity.StartsWith("http"))
			{
				messageEncodingList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Xml"), "Xml"));
			}

			return messageEncodingList;
		}

		public List<KeyValuePair<string, string>> GetUserTokenTypeList()
		{
			var userTokenTypeList = new List<KeyValuePair<string, string>>();

			userTokenTypeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Anonymous"), "Anonymous"));
			userTokenTypeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("UserName"), "UserName"));
			userTokenTypeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText("Certificate"), "Certificate"));

			return userTokenTypeList;
		}



		public List<string> GetDomainList()
		{
			var domainList = new List<string>();
			if (this.SelectionMode == "Local")
			{
				if (!IsInDomain())
				{
					domainList.Add("WORKGROUP");
				}
				else
				{
				}
			}
			else if (this.SelectionMode == "Remote")
			{
				var root = new DirectoryEntry("WinNT:");
				foreach (DirectoryEntry entry in root.Children)
				{
					domainList.Add(entry.Name);
				}

				if (domainList.Count > 0)
				{
					domainList.Insert(0, FMBaseController.TranslateText("--Select--"));
				}
			}

			return domainList;
		}

		public static bool IsInDomain()
		{
			NetApi.NetJoinStatus status = NetApi.NetJoinStatus.NetSetupUnknownStatus;
			IntPtr pDomain = IntPtr.Zero;
			int result = NetApi.NetGetJoinInformation(null, out pDomain, out status);
			if (pDomain != IntPtr.Zero)
			{
				NetApi.NetApiBufferFree(pDomain);
			}
			if (result == NetApi.ErrorSuccess)
			{
				return status == NetApi.NetJoinStatus.NetSetupDomainName;
			}
			else
			{
				throw new Exception("Domain Info Get Failed");
			}
		}

		public List<string> GetServerList()
		{
			var serverList = new List<string>();
			if (this.SelectionMode == "Local")
			{
				serverList.Add("localhost");
				this.Server = serverList[0];
			}
			else
			{

				EnumerateLanMachines enumerate = new EnumerateLanMachines();
				enumerate.enumerateMachines(serverList, this.Domain);
				if (serverList.Count > 0)
				{
					serverList.Insert(0, FMBaseController.TranslateText("--Select--"));
				}

				this.Server = string.Empty;
			}

			return serverList;
		}


		public List<string> GetOpcUaServerList()
		{
			IList<ApplicationDescription> servers = null;
			var opcUaServerList = new List<string>();

			if (!string.IsNullOrEmpty(this.Server))
			{
				try
				{
					servers = this.Application.DiscoverServers("opc.tcp://" + this.Server + ":4840");
				}
				catch (Exception)
				{
					this.Server = string.Empty;
				}

				if (servers != null)
				{

					foreach (var server in servers)
					{
						if (server.ApplicationType != ApplicationType.Server
						&& server.ApplicationType != ApplicationType.ClientAndServer)
						{
							continue;
						}

						opcUaServerList.Add(server.ApplicationName.Text);
					}

					if (opcUaServerList.Count > 0)
					{
						opcUaServerList.Insert(0, FMBaseController.TranslateText("--Select--"));
					}
				}
			}

			return opcUaServerList;
		}


		public List<string> GetOpcUaServerSecurityList()
		{
			var opcUaServerSecurityList = new List<string>();

			// ToDo : Enumerate available EndPoints from Server
			if (this.SelectionMode == "Manual"
			&& string.IsNullOrEmpty(this.Server))
			{

			}

			// Enumerate available Endpoints from Discovery Server
			else
			{
				if (!string.IsNullOrEmpty(this.Server))
				{
					try
					{
						var servers = this.Application.DiscoverServers("opc.tcp://" + this.Server + ":4840");

						foreach (var server in servers)
						{
							if ((server.ApplicationType != ApplicationType.Server
							&& server.ApplicationType != ApplicationType.ClientAndServer)
							|| server.ApplicationName.Text != this.OpcUaServer)
							{
								continue;
							}

							foreach (var discoveryUrl in server.DiscoveryUrls)
							{
								var endpoints = this.Application.GetEndpoints(discoveryUrl);

								foreach (var endpoint in endpoints)
								{

									if (discoveryUrl.StartsWith("opc.tcp://")
									&& endpoint.EndpointUrl.StartsWith("https"))
									{
										continue;
									}

									if (discoveryUrl.StartsWith("https://")
									&& endpoint.EndpointUrl.StartsWith("opc.tcp"))
									{
										continue;
									}


									opcUaServerSecurityList.Add(endpoint.EndpointUrl.Substring(0, endpoint.EndpointUrl.IndexOf("/")) + " - " + endpoint.SecurityMode + " - " + endpoint.SecurityPolicy);

									// set EndpointUrl to first endpoint.EndpointUrl or the selected OpcUaServerSecurity
									if (string.IsNullOrEmpty(this.EndpointUrl)
									|| this.OpcUaServerSecurity == opcUaServerSecurityList[opcUaServerSecurityList.Count - 1])
									{
										this.EndpointUrl = endpoint.EndpointUrl;
									}
								}
							}
						}

						if (opcUaServerSecurityList.Count > 0)
						{
							opcUaServerSecurityList.Insert(0, FMBaseController.TranslateText("--Select--"));
						}
					}
					catch (Exception e)
					{
						this.Server = string.Empty;
						throw new Exception(String.Format("GetOpcUaServerSecurityList Error : {0}.", e.Message));
					}
				}
			}

			return opcUaServerSecurityList;
		}
	}
}