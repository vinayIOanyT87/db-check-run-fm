
namespace ConfigureFMSystem
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;


	using NodeId = Softing.Opc.Ua.Toolkit.NodeId;

	class CreateSitePoints
	{
		public SecurityClass Security;

		public int NumTankPoints;

		public string OpcUaServerEndPoint;

		public string Site;

		public string TemplateID;

		protected ushort NamespaceIndex;

		protected string OpcUaServerFolderNodeIdForPoints;

		public string OpcUaNamespaceUri;

		public int OpcUaPublishingInterval;


		public CreateSitePoints(SecurityClass security, string site, int numTankPoints, string opcUaServerEndPoint, string templateId)
		{
			Security = security;
			NumTankPoints = numTankPoints;
			OpcUaServerEndPoint = opcUaServerEndPoint;
			Site = site;
			TemplateID = templateId;
			NamespaceIndex = ushort.Parse(ConfigurationManager.AppSettings["NamespaceIndex"]);
			OpcUaServerFolderNodeIdForPoints = ConfigurationManager.AppSettings["OpcUaServerFolderNodeIdForPoints"];
			OpcUaNamespaceUri = ConfigurationManager.AppSettings["OpcUaNamespaceUri"];
			OpcUaPublishingInterval = int.Parse(ConfigurationManager.AppSettings["OpcUaPublishingInterval"]);
		}

		public enum TemplateType { VerticalTank = 0, SimpleHighAlarm = 1, SimpleHighAlarmOpc = 2 }

		public void Create(TemplateType type)
		{
			switch (type)
			{
				case TemplateType.VerticalTank:
					this.CreateVerticalTank();
					break;
				case TemplateType.SimpleHighAlarm:
					this.CreateSimpleHighAlarm();
					break;
				case TemplateType.SimpleHighAlarmOpc:
					this.CreateSimpleHighAlarmOpc();
					break;
			}
		}

		protected CreateOpcUAServers OpcServer = null;

		protected CreateOpcUAServers GetOpcServer()
		{
			if (OpcServer == null)
			{
				OpcServer = new CreateOpcUAServers(Security, OpcUaServerEndPoint);
			}
			else
			{
				if (OpcServer.OpcUaServerEndPoint.CompareTo(OpcUaServerEndPoint) != 0)
				{
					OpcServer = new CreateOpcUAServers(Security, OpcUaServerEndPoint);
				}
				OpcServer.Security = Security;
			}
			return OpcServer;
		}

		public void CreateSignalSelector()
		{
			//Delete();
			var pt = new CreatePointTemplates(Security, Site, TemplateID);
			pt.CreateSignalSelector();
			var template = pt.TankPointTemplate;
			FMChannelHelper.MakeCall<IPoints>(x => x.CreatePoints(this.Security, TemplateID, this.NumTankPoints, template.PointTemplateGuid));
			var opcServer = GetOpcServer();
			Guid opcServerGuid = Guid.Empty;
			opcServerGuid = opcServer.AddOpcUaServer();
			var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateByPointTemplate(this.Security, template.PointTemplateGuid));
			foreach (var point in points)
			{
				foreach (var tag in point.Tags.Values)
				{
					if (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa)
					{
						tag.OpcUaServerGuid = opcServerGuid;
						tag.OpcUaServerEndPoint = OpcUaServerEndPoint;
						tag.OpcUaSecurityMode = "None";
						tag.OpcUaSecurityPolicy = "None";
						tag.OpcUaMessageEncoding = "Binary";
						tag.OpcUaNamespaceUri = OpcUaNamespaceUri;
						tag.InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa;
						tag.OpcUaPublishingInterval = OpcUaPublishingInterval;
						tag.OpcUaNodeId = new NodeId(tag.PointTagGuid, NamespaceIndex).ToString();
						tag.OpcUaUserIdentityMethod = "anonymous";
						tag.OpcUaBrowsePath = string.Empty;
					}
				}
				FMChannelHelper.MakeCall<IPoints>(x => x.Modify(this.Security, point));
			}
		}

		public void CreateVerticalTank()
		{
			//Delete();
			var pt = new CreatePointTemplates(Security, Site, TemplateID);
			pt.CreateVerticalTank();
			var template = pt.TankPointTemplate;
			FMChannelHelper.MakeCall<IPoints>(x => x.CreatePoints(this.Security, TemplateID, this.NumTankPoints, template.PointTemplateGuid));
			var opcServer = GetOpcServer();
			Guid opcServerGuid = Guid.Empty;
			opcServerGuid = opcServer.AddOpcUaServer();
			var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateByPointTemplate(this.Security, template.PointTemplateGuid));
			foreach (var point in points)
			{
				foreach (var tag in point.Tags.Values)
				{
					if (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa)
					{
						tag.OpcUaServerGuid = opcServerGuid;
						tag.OpcUaServerEndPoint = OpcUaServerEndPoint;
						tag.OpcUaSecurityMode = "None";
						tag.OpcUaSecurityPolicy = "None";
						tag.OpcUaMessageEncoding = "Binary";
						tag.OpcUaNamespaceUri = OpcUaNamespaceUri;
						tag.InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa;
						tag.OpcUaPublishingInterval = OpcUaPublishingInterval;
						tag.OpcUaNodeId = new NodeId(tag.PointTagGuid, NamespaceIndex).ToString();
						tag.OpcUaUserIdentityMethod = "anonymous";
						tag.OpcUaBrowsePath = string.Empty;
					}
				}
				FMChannelHelper.MakeCall<IPoints>(x => x.Modify(this.Security, point));
			}
		}

		public void CreateSimpleHighAlarmOpc()
		{
			var pt = new CreatePointTemplates(Security, Site, TemplateID);
			pt.CreateSimpleHighAlarmOpc();
			var template = pt.TankPointTemplate;
			FMChannelHelper.MakeCall<IPoints>(x => x.CreatePoints(this.Security, TemplateID, this.NumTankPoints, template.PointTemplateGuid));
			var opcServer = this.GetOpcServer();
			Guid opcServerGuid = Guid.Empty;
			opcServerGuid = opcServer.AddOpcUaServer();
			var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateByPointTemplate(this.Security, template.PointTemplateGuid));
			foreach (var point in points)
			{
				foreach (var tag in point.Tags.Values)
				{
					if (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa)
					{
						tag.OpcUaServerGuid = opcServerGuid;
						tag.OpcUaServerEndPoint = this.OpcUaServerEndPoint;
						tag.OpcUaSecurityMode = "None";
						tag.OpcUaSecurityPolicy = "None";
						tag.OpcUaMessageEncoding = "Binary";
						tag.OpcUaNamespaceUri = this.OpcUaNamespaceUri;
						tag.InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa;
						tag.OpcUaPublishingInterval = this.OpcUaPublishingInterval;
						tag.OpcUaNodeId = new NodeId(tag.PointTagGuid, this.NamespaceIndex).ToString();
						tag.OpcUaUserIdentityMethod = "anonymous";
						tag.OpcUaBrowsePath = string.Empty;
					}
				}
				FMChannelHelper.MakeCall<IPoints>(x => x.Modify(this.Security, point));
			}
		}

		public void CreateSimpleHighAlarm()
		{
			var pt = new CreatePointTemplates(Security, Site, TemplateID);
			pt.CreateSimpleHighAlarm();
			var template = pt.TankPointTemplate;
			FMChannelHelper.MakeCall<IPoints>(x => x.CreatePoints(this.Security, TemplateID, this.NumTankPoints, template.PointTemplateGuid));
		}

		public void Delete()
		{
			var points = GetAllPoints();
			foreach (var point in points)
			{
				FMChannelHelper.MakeCall<IPoints>(x => x.Purge(this.Security, point.PointGuid));
			}
			var pt = new CreatePointTemplates(Security, Site, TemplateID);
			pt.Delete();
		}

		public PointCollection GetAllPoints()
		{
			var pt = new CreatePointTemplates(Security, Site, TemplateID);
			var template = pt.TankPointTemplate;
			var points = new PointCollection();
			if (template != null)
			{
				points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateByPointTemplate(this.Security, template.PointTemplateGuid));
			}
			return points;
		}

		public PointTagCollection GetAllTags()
		{
			var ret = new PointTagCollection();
			var tankPoints = GetAllPoints();
			foreach (var point in tankPoints)
			{
				foreach (var tag in point.Tags)
				{
					ret.Add(tag.Value);
				}
			}
			return ret;
		}
	}
}
