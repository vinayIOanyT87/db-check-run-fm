using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureFMSystem
{
	using System.Net.Mime;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using System.Runtime.InteropServices;
	using System.Configuration;
	using System.IO;

	class Program
	{
		[DllImport("msvcrt")]
		static extern int _getch();

		static SecurityClass Login(string siteID)
		{
			var security = new SecurityClass { UserGuid = Guids.UserAdminGuid, SiteGuid = Guids.SiteAdminGuid };
			security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
			security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
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
				security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
				security.UserID = "Administrator";
			}
			return security;
		}

		public enum CommandEnum
		{
			CleanSite = 1,

			ConfigurePointsForSite = 2,
			
			Exit = 255
		};

		public static CommandEnum MainMenu()
		{
			System.Console.WriteLine("-----------------------------------------------------------");
			System.Console.WriteLine("1) Clean Site");
			System.Console.WriteLine("2) Configure Site");
			System.Console.WriteLine("x) Exit");
			System.Console.WriteLine("-----------------------------------------------------------");
			int c = _getch();
			if (c.Equals((int)'x' ))
			{
				return CommandEnum.Exit;
			}
			int ret = ((int)c) - 48;
			return (CommandEnum)ret;
		}

		public static string GetSiteID()
		{
			System.Console.WriteLine("-----------------------------------------------------------");
			System.Console.WriteLine("Enter Site ID");
			System.Console.WriteLine("-----------------------------------------------------------");
			return System.Console.ReadLine();
		}

		public static CreateSitePoints SitePoints = null;

		public static CreateSitePoints GetSitePoints(SecurityClass security, string site, int numTankPoints, string opcUaServerEndPoint, string templateId)
		{
				if (SitePoints == null)
				{
					SitePoints = new CreateSitePoints(security, site, numTankPoints, opcUaServerEndPoint, templateId);
				}
				else
				{
					if (SitePoints.OpcUaServerEndPoint.CompareTo(opcUaServerEndPoint) != 0 )
					{
						SitePoints = new CreateSitePoints(security, site, numTankPoints, opcUaServerEndPoint, templateId);
					}
					else
					{
						SitePoints.Site = site;
						SitePoints.NumTankPoints = numTankPoints;
						SitePoints.Security = security;
						SitePoints.TemplateID = templateId;
					}
				}
				return SitePoints;
		}

		public static void HandleCleanSite(string siteID, int numTankPoints, string opcUaServerEndPoint, string templateId)
		{
			var security = Login(siteID);

			var sitePoints = GetSitePoints(security,siteID,numTankPoints, opcUaServerEndPoint, templateId);
			sitePoints.Delete();
		}

		public static void HandleConfigureSite(string siteID, int numTankPoints, string opcUaServerEndPoint, string templateId, CreateSitePoints.TemplateType templateType)
		{
			var security = Login(siteID);

			var sitePoints = GetSitePoints(security, siteID,numTankPoints, opcUaServerEndPoint, templateId);
			sitePoints.Create(templateType);
		}

		public static void HandleMainMenu(int numTankPoints, string opcUaServerEndPoint, string templateId, CreateSitePoints.TemplateType templateType)
		{
			var cmd = MainMenu();
			switch (cmd)
			{
				case CommandEnum.CleanSite:
					HandleCleanSite(GetSiteID(), numTankPoints, opcUaServerEndPoint, templateId);
					break;
				case CommandEnum.ConfigurePointsForSite:
					HandleConfigureSite(GetSiteID(), numTankPoints, opcUaServerEndPoint, templateId,templateType);
					break;
				case CommandEnum.Exit:
					Environment.Exit(0);
					break;
			}
		}

		public static void HandleCommandLineArguments(CommandEnum cmd, string siteID, int numTankPoints, string opcUaServerEndPoint, string templateId, CreateSitePoints.TemplateType templateType)
		{
				switch (cmd)
				{
					case CommandEnum.CleanSite:
						HandleCleanSite(siteID, numTankPoints, opcUaServerEndPoint, templateId);
						break;
					case CommandEnum.ConfigurePointsForSite:
						HandleConfigureSite(siteID, numTankPoints, opcUaServerEndPoint, templateId, templateType);
						break;
					case CommandEnum.Exit:
						Environment.Exit(0);
						break;
				}
		}

		public static void HandleScriptFile(string filename)
		{
			string line;
			using (StreamReader reader = new StreamReader(filename))
			{

				while ((line = reader.ReadLine()) != null)
				{
					if (!line.StartsWith("//") && line != string.Empty)
					{
						var parameters = line.Split(',');
						if (parameters.Count() == 5)
						{
							HandleCommandLineArguments(ConvertStringToCommandEnum(parameters[0]), parameters[1], int.Parse(parameters[2]), parameters[3], parameters[4], CreateSitePoints.TemplateType.VerticalTank);
						}
						else
						{
							HandleCommandLineArguments(ConvertStringToCommandEnum(parameters[0]), parameters[1], int.Parse(parameters[2]), parameters[3], parameters[4], (CreateSitePoints.TemplateType)int.Parse(parameters[5]));

						}
					}
				}
			}
		}

		public static void Usage()
		{
				System.Console.WriteLine("Usage: ConfigureFMSystem [-c COMMAND -s SITE_ID -t TEMPLATE_ID [-p NUM_TANK_POINTS] [-o OPC_END_POINT ]] [-f SCRIPT_FILE] [-?]");
				System.Console.WriteLine("	COMMAND: clean, configure");
		}

		public static CommandEnum ConvertStringToCommandEnum(string command)
		{
				switch (command)
				{
					case "clean":
						return CommandEnum.CleanSite;
					case "configure":
						return CommandEnum.ConfigurePointsForSite;
					default:
						return CommandEnum.Exit;
				}
		}

		[STAThreadAttribute]
		static void Main(string[] args)
		{
			bool commandGiven = false;
			CommandEnum command = CommandEnum.Exit;
			string siteId = "";
			string templateId = "";
			int numTankPoints = int.Parse(ConfigurationManager.AppSettings["NumTankPoints"]);
			string opcUaServerEndPoint = ConfigurationManager.AppSettings["OpcUaServerEndPoint"];
			CreateSitePoints.TemplateType templateType = CreateSitePoints.TemplateType.VerticalTank;

			bool useScriptFile = false;
			string fileName = "";
			for (int i = 0; i < args.Length; i++)
			{
				if(args[i].CompareTo("-?") == 0)
				{
					Usage();
					return;
				}
				if(args[i].CompareTo("-c") == 0)
				{
					i++;
					command = ConvertStringToCommandEnum(args[i]);
					commandGiven = true;
					continue;
				}
				if (args[i].CompareTo("-s") == 0)
				{
					i++;
					siteId = args[i];
					continue;
				}
				if (args[i].CompareTo("-t") == 0)
				{
					i++;
					templateId = args[i];
					continue;
				}
				if (args[i].CompareTo("-tt") == 0)
				{
					i++;
					templateType = (CreateSitePoints.TemplateType)int.Parse(args[i]);
					continue;
				}
				if (args[i].CompareTo("-p") == 0)
				{
					i++;
					numTankPoints = int.Parse(args[i]);
					continue;
				}
				if (args[i].CompareTo("-o") == 0)
				{
					i++;
					opcUaServerEndPoint = args[i];
					continue;
				}
				if (args[i].CompareTo("-f") == 0)
				{
					i++;
					fileName = args[i];
					useScriptFile = true;
					continue;
				}
			}

			if(useScriptFile)
			{
				HandleScriptFile(fileName);
				return;
			}

			if (commandGiven)
			{
				HandleCommandLineArguments(command, siteId, numTankPoints, opcUaServerEndPoint, templateId,templateType);
			}
			else
			{
				while (true)
				{
					HandleMainMenu(numTankPoints, opcUaServerEndPoint, templateId,templateType);
				}
			}
		}
	}
}
