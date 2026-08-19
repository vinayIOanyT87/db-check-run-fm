
namespace ConfigureFMSystem
{
	using System;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	class CreateOpcUAServers
	{
		public SecurityClass Security;

		public string OpcUaServerEndPoint;

        public CreateOpcUAServers(SecurityClass security, string opcUaServerEndPoint)
		{
			Security = security;
			OpcUaServerEndPoint = opcUaServerEndPoint;
		}

		public Guid AddOpcUaServer()
		{
			var servers = this.GetServers();
			foreach (var svr in servers)
			{
				if (svr.ServerEndPoint == OpcUaServerEndPoint)
				{
					//Uncomment When running as service that restarts itself
					//this.ResetServer();
					//System.Threading.Thread.Sleep(10000);
					return svr.IdentityGuid;
				}
			}
			var server = new OpcUAServerClass{
					IdentityGuid = Guid.NewGuid(),
					ServerEndPoint = OpcUaServerEndPoint,
					SecurityMode = "None",
					SecurityPolicy = "None",
					MessageEncoding  = "Binary",
					UserIdentityMethod = "anonymous",
					UserId = null,
					UserPassword = null,
					UserCertificatePath = null
			};
			return FMChannelHelper.MakeCall<IOpcUAServer, Guid>(x => x.Add(this.Security, server));
		}

		public OpcUAServerCollectionClass GetServers()
		{
			return FMChannelHelper.MakeCall<IOpcUAServer, OpcUAServerCollectionClass>(x => x.GetAll(this.Security));
		}

	}
}
