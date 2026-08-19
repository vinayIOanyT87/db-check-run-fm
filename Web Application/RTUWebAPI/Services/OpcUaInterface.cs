using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RTUWebAPI.Models;
using Softing.Opc.Ua.Toolkit;

namespace RTUWebAPI.Services
{
	public class OpcUaInterface : IDisposable
	{
		public enum PathId {
			CpuModuleConfiguration = 1,
			CpuModuleDynamic = 2,
			CpuModuleChannelConfiguration = 3,
			CpuModuleChannelDynamic = 4,
			InterfaceModuleConfiguration = 5,
			InterfaceModuleDynamic = 6,
			InterfaceModuleChannelConfiguration = 7,
			InterfaceModuleChannelDynamic = 8
		};

		private Softing.Opc.Ua.Toolkit.Client.Session Session { get; set; }

		bool disposed = false;

		private static NodeId rootNodeId = new NodeId(85, 0);
		private static QualifiedName RtuQualifiedName = new QualifiedName("8810 RTU", 1);
		private static QualifiedName ChassisQualifiedName = new QualifiedName("Chassis", 1);
		private static QualifiedName CpuModuleQualifiedName = new QualifiedName("CPU Module", 1);
		private static QualifiedName ConfigurationQualifiedName = new QualifiedName("Configuration", 1);
		private static QualifiedName DynamicQualifiedName = new QualifiedName("Dynamic/Command", 1);
		private static QualifiedName ModConfiguredQualifiedName = new QualifiedName("ModConfigured", 1);
		private static QualifiedName ProtocolQualifiedName = new QualifiedName("Protocol", 1);

		public OpcUaInterface(RTUConnection connectionInfo)
		{
			this.LoadApplicationConfiguration();
			while (true)
			{
				try
				{
					lock (Application.CurrentSessions)
					{
						this.Session = this.CreateOpcUaSession(connectionInfo);
						this.Session.Timeout = 2500;
						this.Session.Connect(false, false);
					}
					break;
				}
				catch (Exception e)
				{
					if (this.Session != null)
					{
						lock (Application.CurrentSessions)
						{
							if (this.Session.CurrentState == Softing.Opc.Ua.Toolkit.Client.State.Connected)
							{
								this.Session.Disconnect(false);
							}
							this.Session.Dispose();
						}
						this.Session = null;
					}
					var inner = e;
					while(inner.InnerException != null)
					{
						inner = inner.InnerException;
					}
					throw inner;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			// Suppress finalization.
			GC.SuppressFinalize(this);
		}

		// Protected implementation of Dispose pattern.
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing)
			{

				// Dispose of unmanaged resources.
				if (this.Session != null)
				{
					lock (Application.CurrentSessions)
					{
						try
						{
							if (this.Session.CurrentState == Softing.Opc.Ua.Toolkit.Client.State.Connected)
							{
								this.Session.Disconnect(false);
							}

							this.Session.Dispose();
							this.Session = null;
						}
						catch (Exception e)
						{
							if (this.Session != null)
							{
								this.Session.Dispose();
								this.Session = null;
							}
						}
					}
				}
			}

			disposed = true;
		}

		public IList<DataValue> ReadChassisConfigurationData()
		{
			try
			{
				var browsePathList = new List<BrowsePath>()
				{
					new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module 1", 1), ConfigurationQualifiedName, ModConfiguredQualifiedName } },
					new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module 2", 1), ConfigurationQualifiedName, ModConfiguredQualifiedName } },
					new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module 3", 1), ConfigurationQualifiedName, ModConfiguredQualifiedName } },
					new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module 4", 1), ConfigurationQualifiedName, ModConfiguredQualifiedName } },
					new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module 5", 1), ConfigurationQualifiedName, ModConfiguredQualifiedName } },
					new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module 6", 1), ConfigurationQualifiedName, ModConfiguredQualifiedName } }
				};

				var browsePathResultList = this.Session.TranslateBrowsePathsToNodeIds(browsePathList);
				var readValueIdList = browsePathResultList.Select(s => new ReadValueId() { AttributeId = AttributeId.Value, NodeId = s.TargetIds[0] }).ToList();
				return this.Session.Read(readValueIdList, 0, TimestampsToReturn.Neither);
			}
			catch (Exception except)
			{
			}

			return null;
		}

		public List<BrowsePath> GetChassisBrowsePathList(PathId pathId, int module = 0, int channel = 0)
		{
			switch (pathId)
			{
				case PathId.CpuModuleConfiguration:
					return new List<BrowsePath>()
					{
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, CpuModuleQualifiedName, ConfigurationQualifiedName } },
					};

				case PathId.CpuModuleDynamic:
					return new List<BrowsePath>()
					{
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, CpuModuleQualifiedName, DynamicQualifiedName } },

					};

				case PathId.CpuModuleChannelConfiguration:
					return new List<BrowsePath>()
					{
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, CpuModuleQualifiedName, new QualifiedName("Channel " + channel, 1) } },
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, CpuModuleQualifiedName, new QualifiedName("Channel " + channel, 1), ConfigurationQualifiedName } },
					};

				case PathId.CpuModuleChannelDynamic:
					return new List<BrowsePath>()
					{
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, CpuModuleQualifiedName, new QualifiedName("Channel " + channel, 1), DynamicQualifiedName } },
					};

				case PathId.InterfaceModuleConfiguration:
					return new List<BrowsePath>()
					{
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module " + module, 1), ConfigurationQualifiedName } },
					};

				case PathId.InterfaceModuleDynamic:
					return new List<BrowsePath>()
					{
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module " + module, 1), DynamicQualifiedName } },

					};

				case PathId.InterfaceModuleChannelConfiguration:
					return new List<BrowsePath>()
					{
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module " + module, 1), new QualifiedName("Channel " + channel, 1) } },
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module " + module, 1), new QualifiedName("Channel " + channel, 1), ConfigurationQualifiedName } },
					};

				case PathId.InterfaceModuleChannelDynamic:
					return new List<BrowsePath>()
					{
						new BrowsePath() { StartingNode = rootNodeId, RelativePath = new List<QualifiedName>() { RtuQualifiedName, ChassisQualifiedName, new QualifiedName("Interface Module " + module, 1), new QualifiedName("Channel " + channel, 1), DynamicQualifiedName } },

					};

				default:
					return null;
			}
		}

		public IList<ReferenceDescription> ReadReferences(List<BrowsePath> browsePathList)
		{
			try
			{
				var referenceDescriptionList = new List<ReferenceDescription>();
				var browsePathResultList = this.Session.TranslateBrowsePathsToNodeIds(browsePathList);
				foreach (var browsePathResult in browsePathResultList)
				{
					var referenceDescriptionSubList = this.Session.Browse(browsePathResult.TargetIds[0], null);
					foreach(var referenceDescription in referenceDescriptionSubList)
					{
						if (referenceDescription.NodeClass == NodeClass.Variable)
						{
							referenceDescriptionList.Add(referenceDescription);
						}
					}
				}

				return referenceDescriptionList;
			}
			catch (Exception except)
			{
			}

			return null;
		}

		public IList<DataValue> ReadData(IList<ReferenceDescription> referenceDescriptionList)
		{
			if (referenceDescriptionList == null)
			{
				return null;
			}

			try
			{
				var readValueIdList = referenceDescriptionList.Select(s => new ReadValueId() { AttributeId = AttributeId.Value, NodeId = new NodeId(s.NodeId) }).ToList();
				return this.Session.Read(readValueIdList, 0, TimestampsToReturn.Server);
			}
			catch (Exception except)
			{
			}

			return null;
		}

		public IList<DataValue> ReadData(IList<UInt32> identifierList)
		{
			if (identifierList == null)
			{
				return null;
			}

			try
			{
				var readValueIdList = identifierList.Select(s => new ReadValueId() { AttributeId = AttributeId.Value, NodeId = new NodeId(s, 1) }).ToList();
				return this.Session.Read(readValueIdList, 0, TimestampsToReturn.Server);
			}
			catch (Exception except)
			{
			}

			return null;
		}

		public IList<StatusCode> WriteData(IList<ReferenceDescription> referenceDescriptionList, List<DataValue> dataValueList)
		{
			if (referenceDescriptionList == null)
			{
				return null;
			}

			try
			{
				var writeValueList = new List<WriteValue>();
				var readValueIdList = referenceDescriptionList.Select(s => new ReadValueId() { AttributeId = AttributeId.Value, NodeId = new NodeId(s.NodeId) }).ToList();
				var index = 0;
				foreach(var readValueId in readValueIdList)
				{
					var writeValue = new WriteValue();
					writeValue.AttributeId = AttributeId.Value;
					writeValue.NodeId = readValueId.NodeId;
					writeValue.Value = dataValueList[index];
					index++;
				}
				return this.Session.Write(writeValueList);
			}
			catch (Exception except)
			{
			}

			return null;
		}

		public IList<StatusCode> WriteData(List<WriteValue> writeValueList)
		{
			if (writeValueList == null)
			{
				return null;
			}

			try
			{
				var statusCodeIList = this.Session.Write(writeValueList);

				return statusCodeIList;
			}
			catch (BaseException except)
			{
				var statusCodeList = new List<StatusCode>();
				foreach(var writeValue in writeValueList)
				{
					statusCodeList.Add(except.StatusCode);
				}

				return statusCodeList;
			}
		}

		protected void LoadApplicationConfiguration()
		{
			if (!string.IsNullOrEmpty(Application.Configuration.ApplicationName))
			{
				return;
			}

			var result = Application.ActivateLicense(LicenseFeature.Client, "0fa0-00d8-b0b4-a329-439d");
			if (!result)
			{
				throw new Exception("OpcUaditorController.LoadApplicationConfiguration ActivateLicense error");
			}

			Application.Configuration.ApplicationName = "RTU 8810 OpcUaInterface";

            // security configuration
            string applicationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Softing\OpcClient");

            Application.Configuration.Security.ApplicationCertificateSubject = Application.Configuration.ApplicationName;
            Application.Configuration.Security.ApplicationCertificateStore = Path.Combine(applicationFolder, @"pki\own");
            Application.Configuration.Security.TrustedCertificateStore = Path.Combine(applicationFolder, @"pki\trusted");
            Application.Configuration.Security.TrustedIssuerCertificateStore = Path.Combine(applicationFolder, @"pki\issuer");
            Application.Configuration.Security.RejectedCertificateStore = Path.Combine(applicationFolder, @"pki\rejected");

            Application.CertificateValidation += this.Application_CertificateValidation;

			try
			{
				Application.Configuration.Validate();
			}
			catch (Exception ex)
			{
                Application.Configuration.ApplicationName = "";
                throw new Exception("OpcUaEditorController.LoadApplicationConfiguration: Application Configuration Error. " + ex);
			}

			Application.UseUaValidationForHttps();

			// trace configuration
			string opcUaConfigurationTraceFile = ".//OpcUaLogs//VeRTUeOpcUaTrace.txt";
			if (opcUaConfigurationTraceFile != null)
			{
				Application.Configuration.Trace.LogFileName = opcUaConfigurationTraceFile;
				Application.Configuration.Trace.LogFileMaxSize = 10;
				Application.Configuration.Trace.LogFileMaxRollBackups = 5;
				Application.Configuration.Trace.LogFileTracelevel = Softing.Opc.Ua.Toolkit.TraceLevels.Warning;
				//enable all masks
				Application.Configuration.Trace.LogFileTraceMask = 0x00FF00FF;
				Application.Configuration.Trace.Tracelevel = Softing.Opc.Ua.Toolkit.TraceLevels.Warning;
				//enable all masks
				Application.Configuration.Trace.TraceMask = 0x00FF00FF;
			}



			return;
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
			}
		}

		/// <summary>
		/// Handles a certificate validation error.
		/// </summary>
		/// <param name="validator">The validator (not used).</param>
		/// <param name="e">The <see cref="Softing.Opc.Ua.Sdk.CertificateValidationEventArgs"/> instance event arguments provided when a certificate validation error occurs.</param>
		public void HandleCertificateValidationError(CertificateValidator validator, CertificateValidationEventArgs e)
		{
			e.ValidationOption = CertificateValidationOption.AcceptOnce;
		}

		private Softing.Opc.Ua.Toolkit.Client.Session CreateOpcUaSession(RTUConnection connectionInfo)
		{
            UserIdentity userIdentity = null;
            switch (connectionInfo.userIdentity)
            {
                case "anonymous":
                    userIdentity = new AnonymousUserIdentity();
                    break;
                case "username":
                    userIdentity = new UserNameUserIdentity(connectionInfo.loginId, connectionInfo.loginPassword);
                    break;
                case "certificate":
                    userIdentity = new CertificateUserIdentity(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            @"Softing\OpcClient\pki\own\private",
                            connectionInfo.certificateFilename), 
                        connectionInfo.loginPassword);
                    break;
            }

            var securityMode = MessageSecurityMode.None;
            switch (connectionInfo.securityMode)
            {
                case "none":
                    securityMode = MessageSecurityMode.None;
                    break;
                case "sign":
                    securityMode = MessageSecurityMode.Sign;
                    break;
                case "signAndEncrypt":
                    securityMode = MessageSecurityMode.SignAndEncrypt;
                    break;
            }
			var securityPolicy = (connectionInfo.securityMode == "none" ? "none" : connectionInfo.securityPolicy);
			var messageEncoding = MessageEncoding.Binary;
            var session = new Softing.Opc.Ua.Toolkit.Client.Session("opc.tcp://" + connectionInfo.url + ":4840", securityMode, securityPolicy, messageEncoding, userIdentity, null)
            {
                SessionName = Guid.NewGuid().ToString()
            };
			return session;

		}
	}
}
