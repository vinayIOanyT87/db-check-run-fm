using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[CollectionDataContract]
	public class TankStatusColorsCollectionClass : List<TankStatusColorClass> { }

	[DataContract]
	[Serializable]
	public class TankStatusColorClass 
	{
		[DataMember]
		public string Enabled { get; set; }
		[DataMember]
		public string EmptyingBackground { get; set; }
		[DataMember]
		public string EmptyingText { get; set; }
		[DataMember]
		public string FillingBackground { get; set; }
		[DataMember]
		public string FillingText { get; set; }
		[DataMember]
		public string RunningBackground { get; set; }
		[DataMember]
		public string RunningText { get; set; }
		[DataMember]
		public string StoppedBackground { get; set; }
		[DataMember]
		public string StoppedText { get; set; }
		[DataMember]
		public string TestingBackground { get; set; }
		[DataMember]
		public string TestingText { get; set; }
		[DataMember]
		public string BadBackground { get; set; }
		[DataMember]
		public string BadText { get; set; }
		[DataMember]
		public string PointGroupHdrBackground { get; set; }
		[DataMember]
		public string PointGroupHdrText { get; set; }
		[DataMember]
		public string PointGroupDefaultCellBackground { get; set; }
		[DataMember]
		public string PointGroupDefaultCellText { get; set; }

		[DataMember]
		public string PointGroupDefaultCellGridLineColor { get; set; }

		[DataMember]
		public string PointCellText { get; set; }

		[DataMember]
		public string PointCellBackground { get; set; }
		public TankStatusColorClass()
		{
			Initialize();
		}

		private void Initialize()
		{
			// load the data from the web config file and store for return

			Enabled = "1";// System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Enabled"];

			EmptyingText = "black";// System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Emptying_TextColor"];

			EmptyingBackground = "#D6BEAD";// System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Emptying_BackColor"];

			FillingText = "black";// System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Filling_TextColor"];

			FillingBackground = "#ABEFFF";// System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Filling_BackColor"];

			RunningText = "black";// System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Running_TextColor"];

			RunningBackground = "#FFCC98";//System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Running_BackColor"];

			StoppedText = "";//System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Stopped_TextColor"];

			StoppedBackground = "";//System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Stopped_BackColor"];

			TestingText = "";//System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Testing_TextColor"];

			TestingBackground = "";//System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Testing_BackColor"];

			BadText = "";//System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Bad_TextColor"];

			BadBackground = "";//System.Configuration.ConfigurationManager.AppSettings["TankStatusColor_Bad_BackColor"];

			PointGroupHdrText = "";//System.Configuration.ConfigurationManager.AppSettings["PointGroup_Header_TextColor"];

			PointGroupHdrBackground = "";//System.Configuration.ConfigurationManager.AppSettings["PointGroup_Header_BackColor"];

			PointGroupDefaultCellText = "";//System.Configuration.ConfigurationManager.AppSettings["Default_cell_TextColor"];

			PointGroupDefaultCellBackground = "";//System.Configuration.ConfigurationManager.AppSettings["Default_cell_BackColor"];

			PointGroupDefaultCellGridLineColor = "";//System.Configuration.ConfigurationManager.AppSettings["Default_cell_Gridline_Color"];

			PointCellBackground = "";//System.Configuration.ConfigurationManager.AppSettings["Default_pointcell_BackColor"];

			PointCellText = "";//System.Configuration.ConfigurationManager.AppSettings["Default_pointcell_Color"];
		}

	}
}
