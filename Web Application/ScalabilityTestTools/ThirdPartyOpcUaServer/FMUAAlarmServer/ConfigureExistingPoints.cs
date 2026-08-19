
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using FMUAAlarmPluginInterface;
using FMUAAlarmPlugins;
using InProcLogging;
using Softing.Opc.Ua.Toolkit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMUAAlarmServer
{
	public class ConfigureExistingPoints
	{
		protected List<string> OpcUaServerEndPoints = new List<string>();

		public SecurityClass Security;

		protected ushort NamespaceIndex;

		protected string OpcUaServerFolderNodeIdForPoints;

		protected double MaxRampValue;

		protected double MinRampValue;

		protected int RampUpdateRateInSeconds;

		protected double RampIncrement;

		protected bool RampIncreasing;

		public string OpcUaNamespaceUri;

		protected AddDelAlarmsCli cli = null;

		static SecurityClass Login(string siteID)
		{
			var security = new SecurityClass { UserGuid = Guids.UserAdminGuid, SiteGuid = Guids.SiteAdminGuid };
			security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
			security.UserID = "Administrator";
			if (siteID != "SiteAdmin")
			{
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(security, siteID, false));
				if (site == null || site.SiteGuid == Guid.Empty)
				{
					throw new Exception("Bad Site ID " + siteID);
				}
				security = new SecurityClass { UserGuid = Guids.UserAdminGuid, SiteGuid = site.SiteGuid };
				security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
				security.UserID = "Administrator";
			}
			return security;
		}

		protected AddDelAlarmsCli GetCli()
		{
			if (OpcUaServerEndPoints.Count < 1)
			{
				return null;
			}
			string opcUaServerEndPoint = OpcUaServerEndPoints[0];
			if (cli == null)
			{
				cli = new AddDelAlarmsCli(opcUaServerEndPoint);
			}
			else
			{
				if (cli.ServerUrl.CompareTo(opcUaServerEndPoint) != 0)
				{
					cli = new AddDelAlarmsCli(opcUaServerEndPoint);
				}
			}
			return cli;
		}
		public ConfigureExistingPoints(List<string> opcUaServerEndPoints)
		{
			OpcUaServerEndPoints = opcUaServerEndPoints;

			Security = Login("SiteAdmin");
			NamespaceIndex = ushort.Parse(ConfigurationManager.AppSettings["NamespaceIndex"]);
			OpcUaServerFolderNodeIdForPoints = ConfigurationManager.AppSettings["OpcUaServerFolderNodeIdForPoints"];
			OpcUaNamespaceUri = ConfigurationManager.AppSettings["OpcUaNamespaceUri"];
			MinRampValue = double.Parse(ConfigurationManager.AppSettings["MinRampValue"]);
			MaxRampValue = double.Parse(ConfigurationManager.AppSettings["MaxRampValue"]);
			RampUpdateRateInSeconds = int.Parse(ConfigurationManager.AppSettings["RampUpdateRateInSeconds"]);
			RampIncrement = double.Parse(ConfigurationManager.AppSettings["RampIncrement"]);
			RampIncreasing = bool.Parse(ConfigurationManager.AppSettings["RampIncreasing"]);

		}

		public void Configure()
		{
			while (true)
			{
				try
				{
					var sites = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.Enumerate(this.Security));
					var siteDictionary = new Dictionary<Guid, SiteClass>();
					foreach (var site in sites)
					{
						siteDictionary.Add(site.SiteGuid, site);
					}
					foreach (var opcUaEndPoint in OpcUaServerEndPoints)
					{
						int numPoints = FMChannelHelper.MakeCall<IPoints, int>(x => x.EnabledPointCountForSimulator(this.Security, opcUaEndPoint));
						int numPointsRead = 0;
						while (numPointsRead < numPoints)
						{
							int numPointsLeftToRead = numPoints - numPointsRead;
							int numPointsToRead = numPointsLeftToRead > 1000 ? 1000 : numPointsLeftToRead;
							List<Point> points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateEnabledForSimulator(Security, opcUaEndPoint, numPointsRead, numPointsToRead));
							if (points != null && points.Count > 0)
							{

								while (AddTagsToServerComplex(points, siteDictionary, MinRampValue, MaxRampValue, RampUpdateRateInSeconds, RampIncrement, RampIncreasing, opcUaEndPoint) == false)
								{
									System.Threading.Thread.Sleep(3000);
								}
							}
							numPointsRead += numPointsToRead;
						}
					}
					break;
				}
				catch (Exception e)
				{
					System.Console.WriteLine("ConfigureExistingPoints.Configure Exception: " + e.Message);
					Logger.LogError("ConfigureExistingPoints.Configure Exception: " + e.Message);
					System.Threading.Thread.Sleep(1000);
				}
			}
		}

		public bool AddTagsToServerComplex(List<Point> points, Dictionary<Guid, SiteClass> siteDictionary, double minRampValue, double maxRampValue, int rampUpdateRateInSeconds, double rampIncrement, bool rampIncreasing, string opcUaEndPoint)
		{
			AddDelAlarmsCli comms = null;
			try
			{
				comms = GetCli();
				foreach (var point in points)
				{
					string siteId = point.SiteGuid.ToString();
					SiteClass site;
					if (siteDictionary.TryGetValue(point.SiteGuid, out site))
					{
						siteId = site.SiteID;
					}

					var n1 = new AddNodeClass { NodeName = siteId + " " + point.ID, ParentNodeID = OpcUaServerFolderNodeIdForPoints };
					var n1Request = new AddNodeRequestClass
					{
						Sender = "ConfigureFMSystem",
						DynamicEntityType =
							  new ComplexPoint2Factory().GetDynamicEntityTypeName()
					};
					var tags = new ParameterCollection();
					foreach (var tag in point.Tags)
					{
						if (tag.Value.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa)
						{
							if (tag.Value.OpcUaServerEndPoint != null && tag.Value.OpcUaServerEndPoint.ToLower() == opcUaEndPoint.ToLower())
							{
								tags[tag.Value.OpcUaNodeId] = tag.Value.ValueTypeString;
							}
						}
					}
					var inputParams = new ParameterCollection();
					inputParams[ComplexPoint2Factory.NameKey] = siteId + " " + point.ID;
					inputParams[ComplexPoint2Factory.NodeIdKey] = new NodeId(point.PointGuid, NamespaceIndex).ToString();
					inputParams[ComplexPoint2Factory.TagsKey] = tags;
					inputParams[ComplexPoint2Factory.MinRampValueKey] = minRampValue;
					inputParams[ComplexPoint2Factory.MaxRampValueKey] = maxRampValue;
					inputParams[ComplexPoint2Factory.RampUpdateRateInSecondsKey] = rampUpdateRateInSeconds;
					inputParams[ComplexPoint2Factory.RampIncrementKey] = rampIncrement;
					inputParams[ComplexPoint2Factory.RampIncreasingKey] = rampIncreasing;
					n1Request.InputParameters = inputParams;
					n1.NodeXML = n1Request.ToXML();
					comms.AddNodes(n1);
				}
				if (comms != null)
				{
					comms.Disconnect();
				}
				return true;
			}
			catch (Exception e)
			{
				System.Console.WriteLine("ConfigureExistingPoints.AddTagsToServerComplex Exception: " + e.Message);
				Logger.LogError("ConfigureExistingPoints.AddTagsToServerComplex Exception: " + e.Message);
				if (comms != null)
				{
					comms.Disconnect();
				}
				return false;
			}
		}
	}
}
