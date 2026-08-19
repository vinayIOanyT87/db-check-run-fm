
namespace FMPointService.OpcClient
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using InProcLogging;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
	using System;
   using System.Net;
   using System.Security.Cryptography.X509Certificates;

   public class EnterpriseVisibilityConnectionInformation
	{
		public bool IsEnterprise { get; set; }
		public string EnterpriseVisibilityOpcUaServerUrl { get; set; }
		public string EnterpriseVisibilityOpcUaSecurityMode { get; set; }
		public string EnterpriseVisibilityOpcUaSecurityPolicy { get; set; }
		public string EnterpriseVisibilityOpcUaMessageEncoding { get; set; }
		public string EnterpriseVisibilityOpcUaUserIdentityMethod { get; set; }
		public string EnterpriseVisibilityOpcUaUserId { get; set; }
		public string EnterpriseVisibilityOpcUaUserPassword { get; set; }
		public string EnterpriseVisibilityOpcUaUserCertificatePath { get; set; }
		public double EnterpriseVisibilityOpcUaPushPeriodInMinutes { get; set; }
		public bool EnterpriseVisibilityOpcUaEnabled { get; set; }
		public int EnterpriseVisibilityOpcUaNumTagsPerSend { get; set; }

		static public string ErrorMessage { get; private set; }

		public MessageSecurityMode SecurityMode
		{
			get
			{
				if (EnterpriseVisibilityOpcUaSecurityMode != null)
				{
					switch (EnterpriseVisibilityOpcUaSecurityMode.ToLower())
					{
							case "none":
								return MessageSecurityMode.None;
							case "signandencrypt":
								return MessageSecurityMode.SignAndEncrypt;
							default:
								throw new Exception("EnterpriseVisibilityConnectionInformation.SecurityMode : Invalid MessageSecurityMode " + EnterpriseVisibilityOpcUaSecurityMode);
					}
				}
				else
				{
					return MessageSecurityMode.None;
				}
			}
		}

		public SecurityPolicy SecurityPolicy
		{
			get
			{
				if (EnterpriseVisibilityOpcUaSecurityPolicy != null)
				{
					switch (EnterpriseVisibilityOpcUaSecurityPolicy.ToLower())
					{
						case "none":
							return SecurityPolicy.None;
						case "basic256":
							return SecurityPolicy.Basic256;
						case "basic128rsa15":
							return SecurityPolicy.Basic128Rsa15;
						case "basic256sha256":
							return SecurityPolicy.Basic256Sha256;
						case "aes128_sha256_rsaoaep":
							return SecurityPolicy.Aes128_Sha256_RsaOaep;
						case "aes256_sha256_rsapss":
							return SecurityPolicy.Aes256_Sha256_RsaPss;
						default:
							throw new Exception("EnterpriseVisibilityConnectionInformation.SecurityPolicy : Invalid SecurityPolicy " + EnterpriseVisibilityOpcUaSecurityPolicy);
					}
				}
				else
				{
					return SecurityPolicy.None;
				}
			}
		}

		public MessageEncoding MessageEncoding
		{
			get
			{
				if (EnterpriseVisibilityOpcUaMessageEncoding != null)
				{
					switch (EnterpriseVisibilityOpcUaMessageEncoding.ToLower())
					{
							case "binary":
								return MessageEncoding.Binary;
							case "xml":
								return MessageEncoding.Xml;
							default:
								throw new Exception("EnterpriseVisibilityConnectionInformation.MessageEncoding : Invalid MessageEncoding " + EnterpriseVisibilityOpcUaMessageEncoding);
					}
				}
				else
				{
					return MessageEncoding.Binary;
				}
			}
		}

		public UserIdentity UserIdentity
		{
			get
			{
				if (EnterpriseVisibilityOpcUaUserIdentityMethod != null)
				{
					switch (EnterpriseVisibilityOpcUaUserIdentityMethod.ToLower())
					{
							case "anonymous":
								return new UserIdentity();
							case "username":
								return new UserIdentity(EnterpriseVisibilityOpcUaUserId, EnterpriseVisibilityOpcUaUserPassword);
							case "certificate":
								return new UserIdentity(new X509Certificate2(EnterpriseVisibilityOpcUaUserCertificatePath, EnterpriseVisibilityOpcUaUserPassword));
							default:
								throw new Exception("EnterpriseVisibilityConnectionInformation.UserIdentity : Invalid UserIdentityMethod " + EnterpriseVisibilityOpcUaUserIdentityMethod);
					}
				}
				else
				{
					return new UserIdentity();
				}
			}
		}

		public EnterpriseVisibilityConnectionInformation()
		{

		}

		public static bool GetIsEnterprise(SecurityClass security)
		{
			ErrorMessage = string.Empty;
			try
			{
				string isEnterpriseStr =
					FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						configSettingsChannel =>
							configSettingsChannel.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_IsEnterprise));

				if (isEnterpriseStr == "1")
				{
					return true;
				}
				else if (isEnterpriseStr == "0")
				{
					return false;
				}

				return Convert.ToBoolean(isEnterpriseStr);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message; 
				Logger.LogError("EnterpriseVisibilityConnectionInformation.GetIsEnterprise " + ex.Message);
				return false;
			}
		}
		private static bool sameHostnameError = false;
      public static string GetServerUrl(SecurityClass security)
		{
			bool newSameHostnameError = false;
			try
			{
				ErrorMessage= string.Empty;
				string evIP = FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security, 
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityOpcUaServerUrl));
				if (string.IsNullOrWhiteSpace(evIP) == false && GetIsEnterprise(security) == false)
				{
					Uri evUri = new Uri(evIP);
					string localHostname = Dns.GetHostName();
               IPHostEntry evHost = Dns.GetHostEntry(evUri.DnsSafeHost);
               IPHostEntry localHost = Dns.GetHostEntry(localHostname);

               foreach (var evHostAddress in evHost.AddressList)
               {
                  if (evHostAddress.ToString() == "::1" || evHostAddress.ToString() == "127.0.0.1")
                  {
                     newSameHostnameError = true;
                     ErrorMessage = $"In order for Enterprise Visibility to work correctly, EnterpriseVisibilityOpcUaServerUrl in the configuration settings should be set to the Enterprise Server hostname. Current EnterpriseVisibilityOpcUaServerUrl is {evIP}. ";
                     return string.Empty;
                  }
                  foreach (var localHostAddress in localHost.AddressList)
						{
							if (localHostAddress.Equals(evHostAddress))
							{
								newSameHostnameError = true;
                        ErrorMessage = $"In order for Enterprise Visibility to work correctly, EnterpriseVisibilityOpcUaServerUrl in the configuration settings should be set to the Enterprise Server hostname. Current EnterpriseVisibilityOpcUaServerUrl is {evIP}. ";
                        return string.Empty;
							}
						}
					}
            }
				if (string.IsNullOrWhiteSpace(evIP))
				{
               ErrorMessage = "Error: EnterpriseVisibilityOpcUaServerUrl not specified in the configuration settings.";
            }
            return evIP;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetServerUrl " + ErrorMessage);
				return string.Empty;//Don't point to OpcUaServer running on local system "http://127.0.0.1:40002/FuelsManager/OpcUaServer";
			}
			finally
			{

            if (newSameHostnameError && sameHostnameError != newSameHostnameError)
            {
               Logger.LogError("EnterpriseVisibilityConnectionInformation.GetServerUrl: " + ErrorMessage);
					FMPointService.EventLogger.Warning(FMPointService.EventLogger.ServiceName + " : " + EnterpriseVisibilityConnectionInformation.ErrorMessage);
            }
            sameHostnameError = newSameHostnameError;
         }
		}

		public static string GetSecurityMode(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            return FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security, 
					ConfigurationSettingDOClass.Key_EnterpriseVisibilitySecurityMode));
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetSecurityMode " + ex.Message);
				return "none";
			}
		}

		public static string GetSecurityPolicy(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            return FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security, 
					ConfigurationSettingDOClass.Key_EnterpriseVisibilitySecurityPolicy));
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetSecurityPolicy " + ex.Message);
				return "none";
			}
		}

		public static string GetMessageEncoding(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            return FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityMessageEncoding));
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetMessageEncoding " + ex.Message);
				return "xml";
			}
		}

		public static string GetUserIdentityMethod(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            return FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityUserIdentity));
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetUserIdentityMethod " + ex.Message);
				return "certificate";
			}
		}

		public static string GetUserId(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            return FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityUserName));
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetUserId " + ex.Message);
				return "";
			}
		}

		public static string GetPassword(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            return FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityUserPassword));
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetPassword " + ex.Message);
				return "varec";
			}
		}

		public static string GetCertificatePath(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            return FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityCertificatePath));
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetCertificatePath " + ex.Message);
				return @"C:\Users\smarlin\Documents\Luczern\F090XR1.pfx";
			}
		}

		public static double GetPushPeriod(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            string periodStr = FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityPushPeriod));
				return Convert.ToDouble(periodStr);
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetPushPeriod " + ex.Message);
				return 1.00;
			}
		}

		public static bool GetEnabled(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            string enableStr = FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityPushEnabled));

				if (enableStr == "1")
				{
					return true;
				}
				else if (enableStr == "0")
				{
					return false;
				}

				return Convert.ToBoolean(enableStr);
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetEnabled " + ex.Message);
				return false;
			}
		}

		public static int GetNumTagsPerSend(SecurityClass security)
		{
			try
			{
            ErrorMessage = string.Empty;
            string numTagsPerSendStr = FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityTagsPerCall));
				return Convert.ToInt32(numTagsPerSendStr);
			}
			catch (Exception ex)
			{
            ErrorMessage = ex.Message;
            Logger.LogError("EnterpriseVisibilityConnectionInformation.GetNumTagsPerSend " + ex.Message);
				return 1000;
			}
		}

		public EnterpriseVisibilityConnectionInformation(SecurityClass security)
		{
			this.IsEnterprise = GetIsEnterprise(security);
			if (this.IsEnterprise)
			{
				return;
			}

			this.EnterpriseVisibilityOpcUaEnabled = GetEnabled(security);
			if (!this.EnterpriseVisibilityOpcUaEnabled)
			{
				return;
			}

			this.EnterpriseVisibilityOpcUaServerUrl = GetServerUrl(security);
			this.EnterpriseVisibilityOpcUaSecurityMode = GetSecurityMode(security);
			this.EnterpriseVisibilityOpcUaSecurityPolicy = GetSecurityPolicy(security);
			this.EnterpriseVisibilityOpcUaMessageEncoding = GetMessageEncoding(security);
			this.EnterpriseVisibilityOpcUaUserIdentityMethod = GetUserIdentityMethod(security);
			this.EnterpriseVisibilityOpcUaUserId = GetUserId(security);
			this.EnterpriseVisibilityOpcUaUserPassword = GetPassword(security);
			this.EnterpriseVisibilityOpcUaUserCertificatePath = GetCertificatePath(security);
			this.EnterpriseVisibilityOpcUaPushPeriodInMinutes = GetPushPeriod(security);
			this.EnterpriseVisibilityOpcUaNumTagsPerSend = GetNumTagsPerSend(security);
		}


		public bool SessionInfoEqual(EnterpriseVisibilityConnectionInformation e)
		{
			if(e.EnterpriseVisibilityOpcUaServerUrl != this.EnterpriseVisibilityOpcUaServerUrl)
			{
				return false;
			}
			if (e.EnterpriseVisibilityOpcUaSecurityMode != this.EnterpriseVisibilityOpcUaSecurityMode)
			{
				return false;
			}
			if (e.EnterpriseVisibilityOpcUaSecurityPolicy != this.EnterpriseVisibilityOpcUaSecurityPolicy)
			{
				return false;
			}
			if (e.EnterpriseVisibilityOpcUaMessageEncoding != this.EnterpriseVisibilityOpcUaMessageEncoding)
			{
				return false;
			}
			if (e.EnterpriseVisibilityOpcUaUserIdentityMethod != this.EnterpriseVisibilityOpcUaUserIdentityMethod)
			{
				return false;
			}
			if (e.EnterpriseVisibilityOpcUaUserId != this.EnterpriseVisibilityOpcUaUserId)
			{
				return false;
			}
			if (e.EnterpriseVisibilityOpcUaUserPassword != this.EnterpriseVisibilityOpcUaUserPassword)
			{
				return false;
			}
			if (e.EnterpriseVisibilityOpcUaUserCertificatePath != this.EnterpriseVisibilityOpcUaUserCertificatePath)
			{
				return false;
			}

			return true;
		}
	}
}
