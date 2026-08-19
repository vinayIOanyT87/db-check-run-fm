using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;

namespace FMBusinessObjects.ChannelFactories
{
	using System.Web.Services.Description;

	using FMCore;

	using FMBusinessObjects.UtilityObjects;

	using Binding = System.ServiceModel.Channels.Binding;

	public class FMChannelFactoryConfigInfo
	{
		#region Private data members
		private enum BindingTypes { WSHttp, BasicHttp, TCPIP, NetNamedPipe, Custom, NONE };
		private bool configFileRead;
		private BindingTypes bindingType;
		//private string endPointAddressStr;
		//private string bindingName;
		private string serviceName;

		#endregion Private data members

		public bool EndPointAddressSetByConfigFile { get; set; }
		public bool EndPointBindingTypeSetByConfigFile { get; set; }
		public bool EndPointConfigurationSetByConfigFile { get; set; }
		public bool EndPointAddressContainsServiceName { get; set; }

		public string EndpointAddressConfigKey { get; set; }
		public string EndPointBindingTypeConfigKey { get; set; }
		public string EndPointConfigurationConfigKey { get; set; }
		public string EndPointBehaviorNameConfigKey { get; set; }

		public string EndPointAddress { get; set; }
		public string EndPointBindingType { get; set; }
		public string EndPointConfiguration { get; set; }

		public string EndPointBehaviorName { get; set; }


		#region Constructors
		/// <summary>
		/// This is the default constructor for the channel factory configuration information.
		/// </summary>
		//public FMChannelFactoryConfigInfo(String serviceName): this("endPointAddress", "bindingName", serviceName)
		//{
		//	this.Init();
		//}

		public FMChannelFactoryConfigInfo(string serviceName)
		{
			this.serviceName = serviceName;
			this.Init();
		}
		#endregion Constructors

		//#region Public properties
		//public string EndPointAddress { get { return this.endPointAddressStr; } set { this.endPointAddressStr = value; } }
		//#endregion Public properties

		#region Public methods

		/////// <summary>
		/////// This method will return a binding based on the configuration and the contract.
		/////// </summary>
		/////// <returns></returns>
		////public Binding GetBinding(EndpointAddress endpoint)
		////{
		////	this.ReadApplicationConfigFile();
		////	this.UpdateBindingTypeFromEndpoint(endpoint);
		////	return this.CreateBindings();
		////}

		/////// <summary>
		/////// This method will return a binding based on the configuration and the contract.
		/////// </summary>
		/////// <returns></returns>
		////public  Binding GetBinding(string bindingConfigName)
		////{
		////	return (CreateBindings(GetBindingType(EndPointBindingType), bindingConfigName));
		////}

		/// <summary>
		/// This method will return the endpoint address based on the configuration and the
		/// contract.
		/// </summary>
		/// <returns>The endpoint address</returns>
		public EndpointAddress GetEndPointAddress()
		{
			if (EndPointAddressSetByConfigFile)
			{
				this.ReadApplicationConfigFile();
			}

			return CreateEndPointAddress(EndPointAddressContainsServiceName);
		}

		public Binding GetBinding()
		{

			BindingTypes tmpBinding = BindingTypes.WSHttp;

			if (EndPointBindingTypeSetByConfigFile || EndPointConfigurationSetByConfigFile)
			{
				this.ReadApplicationConfigFile();
			}


			if (!EndPointBindingTypeSetByConfigFile)
			{
				tmpBinding = ParseBindingTypeFromEndpoint(GetEndPointAddress(), tmpBinding);
			}
			else
			{
				tmpBinding = GetBindingType(EndPointBindingType);
			}



			return CreateBindings(tmpBinding, EndPointConfiguration);

		}

		///////// <summary>
		///////// This method will return the endpoint address based on supplied uri
		///////// contract.
		///////// </summary>
		///////// <returns>The endpoint address</returns>
		//////public EndpointAddress GetEndPointAddress(string uri)
		//////{
		//////	return (CreateEndPointAddress(uri));
		//////}
		#endregion Public methods

		#region Private methods

		/////// <summary>
		/////// Given the endpoint to be used, update the binding type
		/////// </summary>
		/////// <param name="endpoint"></param>
		////private void UpdateBindingTypeFromEndpoint(EndpointAddress endpoint)
		////{
		////	this.bindingType = ParseBindingTypeFromEndpoint(endpoint, this.bindingType);
		////}

		/// <summary>
		/// This method will create a Net TCP binding using the binding configuration provided.
		/// </summary>
		/// <param name="bindingConfigName">Configuration to use for the NetTcpBinding instance.</param>
		/// <returns></returns>
		private static NetTcpBinding CreateNetTcpBinding(string bindingConfigName)
		{
			NetTcpBinding binding = null;

			if (string.IsNullOrEmpty(bindingConfigName))
				binding = CreateNetTcpBinding();
			else
				binding = new NetTcpBinding(bindingConfigName);

			return (binding);
		}

		/// <summary>
		/// This method will create a default Net TCP binding.
		/// </summary>
		/// <returns></returns>
		private static NetTcpBinding CreateNetTcpBinding()
		{
			NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
			return (binding);
		}

		/// <summary>
		/// This method will create a WSHttp binding using the binding configuration provided.
		/// </summary>
		/// <param name="bindingConfigName">Configuration to use for the WSHttpBinding instance.</param>
		/// <returns></returns>
		private static WSHttpBinding CreateWSHttpBinding(string bindingConfigName)
		{
			WSHttpBinding binding = null;

			if (string.IsNullOrEmpty(bindingConfigName))
				binding = CreateWSHttpBinding();
			else
				binding = new WSHttpBinding(bindingConfigName);

			return (binding);
		}

		/// <summary>
		/// This method will create a WS HTTP binding.
		/// </summary>
		/// <returns></returns>
		private static WSHttpBinding CreateWSHttpBinding()
		{
			WSHttpBinding binding = new WSHttpBinding();
			return (binding);
		}

		/// <summary>
		/// This method will create a Basic HTPP Binding using the binding configuration provided.
		/// </summary>
		/// <param name="bindingConfigName">Configuration to use for the BasicHttpBinding instance.</param>
		/// <returns></returns>
		private static BasicHttpBinding CreateBasicHttpBinding(string bindingConfigName)
		{
			BasicHttpBinding binding = null;

			if (string.IsNullOrEmpty(bindingConfigName))
				binding = CreateBasicHttpBinding();
			else
				binding = new BasicHttpBinding(bindingConfigName);

			return (binding);
		}

		/// <summary>
		/// This method will create a Basic HTTP binding.
		/// </summary>
		/// <returns></returns>
		private static BasicHttpBinding CreateBasicHttpBinding()
		{
			BasicHttpBinding binding = new BasicHttpBinding();
			return (binding);
		}

		/// <summary>
		/// This method will create a Named Pipe Binding using the binding configuration provided.
		/// </summary>
		/// <param name="bindingConfigName">Configuration to use for the NetNamedPipeBinding instance.</param>
		/// <returns></returns>
		private static NetNamedPipeBinding CreateNetNamedPipeBinding(string bindingConfigName)
		{
			NetNamedPipeBinding binding = null;

			if (string.IsNullOrEmpty(bindingConfigName))
				binding = CreateNetNamedPipeBinding();
			else
				binding = new NetNamedPipeBinding(bindingConfigName);

			return (binding);
		}

		/// <summary>
		/// This method will create a Basic HTTP binding.
		/// </summary>
		/// <returns></returns>
		private static NetNamedPipeBinding CreateNetNamedPipeBinding()
		{
			NetNamedPipeBinding binding = new NetNamedPipeBinding();
			return (binding);
		}

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.configFileRead = false;

			this.EndPointAddress = "http://localhost:9500/";
			this.bindingType = BindingTypes.WSHttp;
		}

		/// <summary>
		/// This method will read the application configuration file for the binding and 
		/// endpoint address configuration settings.  There is a flag indicating whether
		/// the configuration file has been read. It only reads the file once.
		/// </summary>
		private void ReadApplicationConfigFile()
		{
			if (this.configFileRead == false)
			{
				if (EndPointAddressSetByConfigFile)
				{
					EndPointAddress = ConfigurationManager.AppSettings[EndpointAddressConfigKey];
					//EndPointAddress = this.GetEndPointAddress() 
				}

				if (EndPointBindingTypeSetByConfigFile)
				{
					EndPointBindingType = ConfigurationManager.AppSettings[EndPointBindingTypeConfigKey];
				}


				if (EndPointConfigurationSetByConfigFile)
				{
					EndPointConfiguration = ConfigurationManager.AppSettings[EndPointConfigurationConfigKey];
				}

				if (!EndPointBehaviorNameConfigKey.DefaultIfNullOrEmpty(string.Empty).Equals(string.Empty))
				{
					EndPointBehaviorName = ConfigurationManager.AppSettings[EndPointBehaviorNameConfigKey];
				}


				// Set the flag that the configuration file was read.
				this.configFileRead = true;
			}
		}

		/// <summary>
		/// This method will create an endpoint address based on the configured
		/// endpoint address string.
		/// </summary>
		/// <returns>The endpoint address</returns>
		private EndpointAddress CreateEndPointAddress(bool alreadyHasServiceName)
		{
			EndpointAddress endPoint = null;

			string uri = EndPointAddress;
			string tmpServiceName = "";

			if (!alreadyHasServiceName)
			{
				if (uri[uri.Length - 1] != '/')
				{
					tmpServiceName += "/";
				}
				tmpServiceName = string.Format("{0}{1}.svc", tmpServiceName, serviceName);
			}

			if (string.IsNullOrEmpty(uri) == true)
			{
				throw new Exception("Must have an end point address!");
			}

			try
			{
				endPoint = new EndpointAddress(string.Format("{0}{1}", uri, tmpServiceName));
			}
			catch (Exception ex)
			{
				string errMsg = "Invalid endpoint address: '" + uri + "'. ";
				throw new Exception(errMsg + ex.Message);
			}

			return endPoint;
		}

		/////// <summary>
		/////// This method will create an endpoint address based on the configured
		/////// endpoint address string.
		/////// </summary>
		/////// <returns>The endpoint address</returns>
		////private EndpointAddress CreateEndPointAddressWithoutServiceName(string uri)
		////{
		////	EndpointAddress endPoint = null;

		////	if (string.IsNullOrEmpty(uri) == true)
		////	{
		////		throw new Exception("Must have an end point address!");
		////	}

		////	try
		////	{
		////		endPoint = new EndpointAddress(uri);
		////	}
		////	catch (Exception ex)
		////	{
		////		string errMsg = "Invalid endpoint address: '" + uri + "'. ";
		////		throw new Exception(errMsg + ex.Message);
		////	}

		////	return endPoint;
		////}

		/// <summary>
		/// This method will return a boolean value. The default is the give
		/// default value;
		/// </summary>
		/// <param name="inValue"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static bool GetBoolean(string inValue, bool defaultValue)
		{
			bool returnValue = defaultValue;

			if (string.IsNullOrEmpty(inValue) == false)
			{
				try
				{
					returnValue = Convert.ToBoolean(inValue);
				}
				catch (Exception)
				{
					// Do nothing.
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the time span if configured.  Otherwise,
		/// it will return the given default.
		/// </summary>
		/// <param name="inTimeSpan"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static TimeSpan GetTimeSpan(string inTimeSpan, TimeSpan defaultValue)
		{
			TimeSpan returnValue = defaultValue;

			if (string.IsNullOrEmpty(inTimeSpan) == false)
			{
				char[] splitValue = { ':' };
				int maxSubstrings = 3;

				string[] timeSpanValues = inTimeSpan.Split(splitValue, maxSubstrings);

				if (timeSpanValues.Length == 3)
				{
					try
					{
						int hour = Convert.ToInt32(timeSpanValues[0]);
						int minute = Convert.ToInt32(timeSpanValues[1]);
						int second = Convert.ToInt32(timeSpanValues[2]);

						returnValue = new TimeSpan(hour, minute, second);
					}
					catch (Exception)
					{
						// Do nothing
					}
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the bind type value.  The default is Basic Http binding.
		/// </summary>
		/// <param name="configBindingType"></param>
		/// <returns></returns>
		private static BindingTypes GetBindingType(string configBindingType)
		{
			BindingTypes returnValue = BindingTypes.BasicHttp;

			if (string.IsNullOrEmpty(configBindingType) == false)
			{
				switch (configBindingType.ToUpper())
				{
					case "NETNAMEDPIPE":
						returnValue = BindingTypes.NetNamedPipe;
						break;
					case "BASICHTTP":
					case "BASICHTTPBINDING":
						returnValue = BindingTypes.BasicHttp;
						break;
					case "TCPIP":
					case "NETTCPBINDING":
						returnValue = BindingTypes.TCPIP;
						break;
					case "WSHTTP":
					case "WSHTTPBINDING":
						returnValue = BindingTypes.WSHttp;
						break;
					case "NONE":
						returnValue = BindingTypes.NONE;
						break;
				}
			}

			return (returnValue);
		}

		///////// <summary>
		///////// This method will create and return a WS Http, TCP/IP, Basic Http or Named Pipe
		///////// binding. The binding will be created based on the configuration file settings.
		///////// </summary>
		///////// <returns>A new WCF Binding binding instance.</returns>
		//////private Binding CreateBindings()
		//////{
		//////	return (CreateBindings(this.bindingType, EndPointBindingType));
		//////}

		/// <summary>
		/// This method will create and return a WSHttpBinding, BasicHttpBinding, NetTcpBinding or NamedPipeBinding instance
		/// based on the requested bindingType and the bindingConfig name provided.
		/// </summary>
		/// <param name="bindingType">WSHttp, BasicHttp or TCPIP</param>
		/// <param name="bindingConfigName">A binding configuration name that is appropriate for the specified bindingType.</param>
		/// <returns></returns>
		private static Binding CreateBindings(BindingTypes bindingType, string bindingConfigName)
		{
			Binding newBinding = null;

			switch (bindingType)
			{
				case BindingTypes.NetNamedPipe:
					newBinding = (Binding)CreateNetNamedPipeBinding(bindingConfigName);
					break;
				case BindingTypes.WSHttp:
					newBinding = (Binding)CreateWSHttpBinding(bindingConfigName);
					break;
				case BindingTypes.BasicHttp:
					newBinding = (Binding)CreateBasicHttpBinding(bindingConfigName);
					break;
				case BindingTypes.TCPIP:
					newBinding = (Binding)CreateNetTcpBinding(bindingConfigName);
					break;
			}

			return (newBinding);
		}

		/// <summary>
		/// This method will return the Security Algorithm Suite value.  The
		/// default is "default".
		/// </summary>
		/// <param name="securityAlgorithm"></param>
		/// <returns></returns>
		private static SecurityAlgorithmSuite GetSecurityAlgorithmSuite(string securityAlgorithm)
		{
			SecurityAlgorithmSuite returnValue = SecurityAlgorithmSuite.Default;

			if (string.IsNullOrEmpty(securityAlgorithm) == false)
			{
				switch (securityAlgorithm.ToUpper())
				{
					case "BASIC128":
						returnValue = SecurityAlgorithmSuite.Basic128;
						break;
					case "BASIC128RSA15":
						returnValue = SecurityAlgorithmSuite.Basic128Rsa15;
						break;
					case "BASIC128SHA256":
						returnValue = SecurityAlgorithmSuite.Basic128Sha256;
						break;
					case "BASIC128SHA256RSA15":
						returnValue = SecurityAlgorithmSuite.Basic128Sha256Rsa15;
						break;
					case "BASIC192":
						returnValue = SecurityAlgorithmSuite.Basic192;
						break;
					case "BASIC192RSA15":
						returnValue = SecurityAlgorithmSuite.Basic192Rsa15;
						break;
					case "BASIC192SHA256":
						returnValue = SecurityAlgorithmSuite.Basic192Sha256;
						break;
					case "BASIC192SHA256RSA15":
						returnValue = SecurityAlgorithmSuite.Basic192Sha256Rsa15;
						break;
					case "BASIC256":
						returnValue = SecurityAlgorithmSuite.Basic256;
						break;
					case "BASIC256RSA15":
						returnValue = SecurityAlgorithmSuite.Basic256Rsa15;
						break;
					case "BASIC256SHA256":
						returnValue = SecurityAlgorithmSuite.Basic256Sha256;
						break;
					case "BASIC256SHA256RSA15":
						returnValue = SecurityAlgorithmSuite.Basic256Sha256Rsa15;
						break;
					case "DEFAULT":
						returnValue = SecurityAlgorithmSuite.Default;
						break;
					case "TRIPLEDES":
						returnValue = SecurityAlgorithmSuite.TripleDes;
						break;
					case "TRIPLEDESRSA15":
						returnValue = SecurityAlgorithmSuite.TripleDesRsa15;
						break;
					case "TRIPLEDESSHA256":
						returnValue = SecurityAlgorithmSuite.TripleDesSha256;
						break;
					case "TRIPLEDESSHA256RSA15":
						returnValue = SecurityAlgorithmSuite.TripleDesSha256Rsa15;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the Basic Http Message Credential Type value.  The
		/// default is "UserName".
		/// </summary>
		/// <param name="msgCredentialType"></param>
		/// <returns></returns>
		private static BasicHttpMessageCredentialType GetBasicHttpMessageCredentialType(string msgCredentialType)
		{
			BasicHttpMessageCredentialType returnValue = BasicHttpMessageCredentialType.UserName;

			if (string.IsNullOrEmpty(msgCredentialType) == false)
			{
				switch (msgCredentialType.ToUpper())
				{
					case "CERTIFICATE":
						returnValue = BasicHttpMessageCredentialType.Certificate;
						break;
					case "USERNAME":
						returnValue = BasicHttpMessageCredentialType.UserName;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the Message Credential Type value.  The
		/// default is "UserName".
		/// </summary>
		/// <param name="msgCredentialType"></param>
		/// <returns></returns>
		private static MessageCredentialType GetMessageCredentialType(string msgCredentialType)
		{
			if (string.IsNullOrEmpty(msgCredentialType))
				return MessageCredentialType.UserName;

			try
			{
				return (MessageCredentialType)Enum.Parse(typeof(MessageCredentialType), msgCredentialType, true);
			}
			catch (InvalidOperationException)
			{
				return MessageCredentialType.UserName;
			}
		}

		/// <summary>
		/// This method will return the HTTP Proxy Credential Type value.  The
		/// default is "none".
		/// </summary>
		/// <param name="proxyCredentialType"></param>
		/// <returns></returns>
		private static HttpProxyCredentialType GetHttpProxyCredentialType(string proxyCredentialType)
		{
			HttpProxyCredentialType returnValue = HttpProxyCredentialType.None;

			if (string.IsNullOrEmpty(proxyCredentialType) == false)
			{
				switch (proxyCredentialType.ToUpper())
				{
					case "BASIC":
						returnValue = HttpProxyCredentialType.Basic;
						break;
					case "DIGEST":
						returnValue = HttpProxyCredentialType.Digest;
						break;
					case "NONE":
						returnValue = HttpProxyCredentialType.None;
						break;
					case "NTLM":
						returnValue = HttpProxyCredentialType.Ntlm;
						break;
					case "WINDOWS":
						returnValue = HttpProxyCredentialType.Windows;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the HTTP Client Credential Type value.  The
		/// default is "none";
		/// </summary>
		/// <param name="credentialType"></param>
		/// <returns></returns>
		private static HttpClientCredentialType GetHttpClientCredentialType(string credentialType)
		{
			HttpClientCredentialType returnValue = HttpClientCredentialType.None;

			if (string.IsNullOrEmpty(credentialType) == false)
			{
				switch (credentialType.ToUpper())
				{
					case "BASIC":
						returnValue = HttpClientCredentialType.Basic;
						break;
					case "CERTIFICATE":
						returnValue = HttpClientCredentialType.Certificate;
						break;
					case "DIGEST":
						returnValue = HttpClientCredentialType.Digest;
						break;
					case "NONE":
						returnValue = HttpClientCredentialType.None;
						break;
					case "NTLM":
						returnValue = HttpClientCredentialType.Ntlm;
						break;
					case "WINDOWS":
						returnValue = HttpClientCredentialType.Windows;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the TCP Client Credential Type value.  The
		/// default is "None";
		/// </summary>
		/// <param name="credentialType"></param>
		/// <returns></returns>
		private static TcpClientCredentialType GetTcpClientCredentialType(string credentialType)
		{
			if (string.IsNullOrEmpty(credentialType))
				return TcpClientCredentialType.None;

			try
			{
				return (TcpClientCredentialType)Enum.Parse(typeof(TcpClientCredentialType), credentialType, true);
			}
			catch (InvalidOperationException)
			{
				return TcpClientCredentialType.None;
			}
		}

		/// <summary>
		/// This method will return the Basic HTTP Security Mode value.  The default
		/// is "None".
		/// </summary>
		/// <param name="securityMode"></param>
		/// <returns></returns>
		private static BasicHttpSecurityMode GetBasicHttpSecurityMode(string securityMode)
		{
			BasicHttpSecurityMode returnValue = BasicHttpSecurityMode.None;

			if (string.IsNullOrEmpty(securityMode) == false)
			{
				switch (securityMode.ToUpper())
				{
					case "MESSAGE":
						returnValue = BasicHttpSecurityMode.Message;
						break;
					case "NONE":
						returnValue = BasicHttpSecurityMode.None;
						break;
					case "TRANSPORT":
						returnValue = BasicHttpSecurityMode.Transport;
						break;
					case "TRANSPORTCREDENTIALONLY":
						returnValue = BasicHttpSecurityMode.TransportCredentialOnly;
						break;
					case "TRANSPORTWITHMESSAGECREDENTIAL":
						returnValue = BasicHttpSecurityMode.TransportWithMessageCredential;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the Security Mode value.  The default
		/// is "None".
		/// </summary>
		/// <param name="securityMode"></param>
		/// <returns></returns>
		private static SecurityMode GetSecurityMode(string securityMode)
		{
			if (string.IsNullOrEmpty(securityMode))
				return SecurityMode.None;

			try
			{
				return (SecurityMode)Enum.Parse(typeof(SecurityMode), securityMode, true);
			}
			catch (InvalidOperationException)
			{
				return SecurityMode.None;
			}
		}

		/// <summary>
		/// This method will return the Transfer Mode value.  The default
		/// is "Buffered".
		/// </summary>
		/// <param name="transMode"></param>
		/// <returns></returns>
		private static TransferMode GetTransferMode(string transMode)
		{
			TransferMode returnValue = TransferMode.Buffered;

			if (string.IsNullOrEmpty(transMode) == false)
			{
				switch (transMode.ToUpper())
				{
					case "BUFFERED":
						returnValue = TransferMode.Buffered;
						break;
					case "STREAMED":
						returnValue = TransferMode.Streamed;
						break;
					case "STREAMEDREQUEST":
						returnValue = TransferMode.StreamedRequest;
						break;
					case "STREAMEDRESPONSE":
						returnValue = TransferMode.StreamedResponse;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the Encoding value.  The default
		/// is "utf-8".
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		private static Encoding GetEncoding(string encoding)
		{
			Encoding returnValue = Encoding.UTF8;

			if (string.IsNullOrEmpty(encoding) == false)
			{
				switch (encoding.ToUpper())
				{
					case "ASCII":
						returnValue = Encoding.ASCII;
						break;
					case "BIGENDIANUNICODE":
						returnValue = Encoding.BigEndianUnicode;
						break;
					case "DEFAULT":
						returnValue = Encoding.Default;
						break;
					case "UNICODE":
						returnValue = Encoding.Unicode;
						break;
					case "UTF-32":
						returnValue = Encoding.UTF32;
						break;
					case "UTF-7":
						returnValue = Encoding.UTF7;
						break;
					case "UTF-8":
						returnValue = Encoding.UTF8;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the WS Message Encoding value.  The
		/// default is "Text".
		/// </summary>
		/// <param name="msgEncoding"></param>
		/// <returns></returns>
		private static WSMessageEncoding GetWSMessageEncoding(string msgEncoding)
		{
			WSMessageEncoding returnValue = WSMessageEncoding.Text;

			if (string.IsNullOrEmpty(msgEncoding) == false)
			{
				switch (msgEncoding.ToUpper())
				{
					case "MTOM":
						returnValue = WSMessageEncoding.Mtom;
						break;
					case "TEXT":
						returnValue = WSMessageEncoding.Text;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will return the host name comparison mode.  The default
		/// is "StrongWildcard".
		/// </summary>
		/// <param name="hostName"></param>
		/// <returns></returns>
		private static HostNameComparisonMode GetHostNameComparisonMode(string hostName)
		{
			HostNameComparisonMode returnValue = HostNameComparisonMode.StrongWildcard;

			if (string.IsNullOrEmpty(hostName) == false)
			{
				switch (hostName.ToUpper())
				{
					case "STRONGWILDCARD":
						returnValue = HostNameComparisonMode.StrongWildcard;
						break;
					case "EXACT":
						returnValue = HostNameComparisonMode.Exact;
						break;
					case "WEAKWILDCARD":
						returnValue = HostNameComparisonMode.WeakWildcard;
						break;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will convert a string into an integer value.  If the 
		/// string cannot be converted the default value will be returned.
		/// </summary>
		/// <param name="newValue"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private int ConvertToInt(string newValue, int defaultValue)
		{
			int returnValue = defaultValue;

			if (string.IsNullOrEmpty(newValue) == false)
			{
				try
				{
					returnValue = Convert.ToInt32(newValue);
				}
				catch (Exception)
				{
					// Do nothing
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Given the endpoint to be used, determine the binding type
		/// </summary>
		/// <param name="endpoint"></param>
		private static BindingTypes ParseBindingTypeFromEndpoint(EndpointAddress endpoint, BindingTypes defaultBindingType)
		{
			BindingTypes newBindingType = defaultBindingType;

			if (endpoint.Uri.AbsoluteUri.Substring(0, 8).ToLower() == "net.pipe")
			{
				newBindingType = BindingTypes.NetNamedPipe;
			}
			else if (endpoint.Uri.AbsoluteUri.Substring(0, 7).ToLower() == "net.tcp")
			{
				newBindingType = BindingTypes.TCPIP;
			}
			else if (endpoint.Uri.AbsoluteUri.Substring(0, 5).ToLower() == "https")
			{
				newBindingType = BindingTypes.WSHttp;
			}
			else if (endpoint.Uri.AbsoluteUri.Substring(0, 4).ToLower() == "http")
			{
				newBindingType = BindingTypes.BasicHttp;
			}

			return (newBindingType);
		}

		#endregion Private methods

		public void RefreshConfiguration()
		{
			this.configFileRead = false;
		}
	}
}
