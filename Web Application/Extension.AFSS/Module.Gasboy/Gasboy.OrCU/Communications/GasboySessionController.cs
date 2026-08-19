namespace FuelsManager.Afss.Module.Gasboy.OrCU.Communications
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.Threading;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.OrCU.GasboyBOS;

	internal class GasboySessionController
	{
		#region Attributes
		/// <summary>
		/// The is disposed.
		/// </summary>
		private bool isDisposed = false;

		private Dictionary<Guid, GasboySession> SessionList = new Dictionary<Guid, GasboySession>();

		#endregion Attributes

		#region Constructors/Destructors

		public GasboySessionController()
		{
			this.isDisposed = false;
		}

		#endregion Constructors/Destructors

		public GasboySession GetGasboySession(SecurityClass security, GasboyStation externalStation)
		{
			bool lockTaken = false;

			GasboySession gasboySession = null;

			try
			{
				Monitor.Enter(this.SessionList, ref lockTaken);

				gasboySession = this.CheckForExistingSession(externalStation);

				if (null != gasboySession)
				{
					return gasboySession;
				}

				gasboySession = this.CreateGasboySession(security, externalStation);

				this.SessionList.Add(externalStation.IdentityGuid, gasboySession);
			}
			finally
			{
				if (lockTaken) Monitor.Exit(this.SessionList);
			}

			return gasboySession;
		}

		public void CloseGasboySession(GasboySession session)
		{
			try
			{
				GasboySession sessionEntry = (from t in this.SessionList.Values
											 where t.SessionID.Equals(session.SessionID)
											 select t).FirstOrDefault();
				if (null != sessionEntry)
				{
					sessionEntry.SessionID = string.Empty;
					sessionEntry.Service = null;
					this.SessionList.Remove(sessionEntry.StationGuid);
				}
			}
			catch
			{
				// ignored
			}

			return;
		}

		private GasboySession CheckForExistingSession(GasboyStation externalStation)
		{
			if (this.SessionList.ContainsKey(externalStation.IdentityGuid))
			{
				return this.SessionList[externalStation.IdentityGuid];
			}

			return null;
		}

		private GasboySession CreateGasboySession(SecurityClass security, GasboyStation externalStation)
		{
			GasboySession newSession = new GasboySession(externalStation);
			
			ServicePointManager.ServerCertificateValidationCallback +=
					new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

			newSession.Service = this.CreateGasboyBOSServiceEndpoint(
				externalStation.IpAddress,
				"http://orpak.com/SiteOmatServices/");

			LoginResponse loginResponse = newSession.Service.SOLogin(
				externalStation.UserName,
				externalStation.Password);

			if (loginResponse.rc != 0)
			{
				// HOCOMM
				// 123456
				switch (loginResponse.rc)
				{
					// Service bad user or password
					case 15:
						var logEntry = new GasboyStationLog()
											{
												IdentityGuid = Guid.NewGuid(),
												LogType = ExternalStationLogType.ValidationFailure,
												CreatedBy = security.UserID,
												CreatedDate = DateTimeOffset.Now,
												ExternalStationGuid = externalStation.IdentityGuid,
												SiteGuid = security.SiteGuid
											};

						GasboyChannelHelper.MakeCall<IGasboyStations>(
							service => service.AddExternalStationLog(security, logEntry));

						break;
				}
			}
			else
			{
				newSession.SessionID = loginResponse.SessionID;
				newSession.SiteCode = externalStation.SiteCode ?? 0;
			}

			return newSession;
		}

		private SiteOmatClassSoap CreateGasboyBOSServiceEndpoint(string hostName, string endpointNamespace)
		{
			// strangely, these two are equivalent
			var binding = new CustomBinding("SiteOmatClassSoap");
			//WSHttpBinding binding = new WSHttpBinding("SiteOmatClassSoap");

			var remoteAddress = new EndpointAddress(new Uri(string.Format("https://{0}/SiteOmatService/SiteOmatService.asmx", hostName)), new UpnEndpointIdentity(endpointNamespace));

			return (SiteOmatClassSoap)(new SiteOmatClassSoapClient(binding, remoteAddress));
		}

		private static bool ValidateRemoteCertificate(
			object sender,
			X509Certificate cert,
			X509Chain chain,
			SslPolicyErrors policyErrors)
		{
			bool result = cert.Subject.ToUpper().Equals(@"E=A@B.COM, CN=LOCALHOST, OU=A, O=LOCALHOST, L=LOCAL, S=LOCALHOST, C=AA"); //Islander prime
			if (!result)
				result = cert.Subject.ToUpper().Equals(@"E=HELPDESK@GASBOY.COM, CN=GASBOY, OU=GASBOY, O=GASBOY, L=GREENSBORO, S=NORTH CAROLINA, C=US"); //Islander plus

			return result;
		}

		#region Disposable Pattern Implementation
		/// <summary>
		/// Disposes this Client Sync Provider instance 
		/// </summary>
		/// <param name="disposing">True if explicit finalization, false if through GC</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.isDisposed)
			{
				return;
			}

			try
			{
				if (disposing)
				{
					this.SessionList.Clear();
				}
			}
			finally
			{
				this.isDisposed = true;
			}
		}

		/// <summary>
		/// Disposes this Client Sync Provider instance 
		/// </summary>
		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion Disposable Pattern Implementation
	}
}
