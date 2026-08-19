

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using FMBusinessObjects.DataObjects;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Web.Mvc;
	using System.Runtime.InteropServices;
	using FMBusinessObjects.DataObjects.CodedVariables;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMPointCommon;
	using System.Web.ModelBinding;
	using System.Runtime.Serialization;


	[Serializable]
	public class EditorStrapTableEntry
	{
		public EditorStrapTableEntry()
		{
		}

		public EditorStrapTableEntry(string level, string volume)
		{
				this.LevelEntry = level;
				this.VolumeEntry = volume;
		}

		public string LevelEntry { get; set; }

		public string VolumeEntry { get; set; }
	}

	[Serializable]
	public class EditorStrapTableSettings
	{
		public EditorStrapTableSettings()
		{
  		}

		public string StrapDensity { get; set; }

		public string StrapTemperature { get; set; }

		public string TankShellReferenceTemperature { get; set; }

		public string DatumHeight { get; set; }

		public string RoofFloatingHeight { get; set; }

		public string RoofLandingHeight { get; set; }

		public string RoofType { get; set; }

		public string RoofMass { get; set; }

	}


	[Serializable]
	[Bind(Exclude = "EditorEntries")]
	public class StrapTableEditorModel
	{
		public int ActiveTab { get; set; }
		public string PointId { get; set; }
		public string PointPropertyId { get; set; }
		public StrapTable StrapTable { get; set; }
		public List<EditorStrapTableEntry>[] EditorEntries { get; set; }
		public Guid PointGuid { get; set; }
		public Guid PointPropertyGuid { get; set; }
		public EngineeringUnit LevelUnit { get; set; }
		public int LevelDecimalPlaces { get; set; }
		public EngineeringUnit VolumeUnit { get; set; }
		public int VolumeDecimalPlaces { get; set; }
		public EngineeringUnit DensityUnit { get; set; }
		public int DensityDecimalPlaces { get; set; }
		public EngineeringUnit TemperatureUnit { get; set; }
		public int TemperatureDecimalPlaces { get; set; }
		public EngineeringUnit MassUnit { get; set; }
		public int MassDecimalPlaces { get; set; }
		public EditorStrapTableSettings[] EditorStrapTableSettings { get; set; }
		public List<KeyValuePair<int, string>> StrapTableList { get; set; }
		public SiteClass Site { get; set; }
		public bool IsTemplatePoint { get; set; }

		public StrapTableEditorModel()
		{
			this.EditorEntries = new List<EditorStrapTableEntry>[6];
			this.StrapTable = new StrapTable();
			this.StrapTable.StrapTables = null;
			this.StrapTableList = new List<KeyValuePair<int, string>>();
		}

		public StrapTableEditorModel( bool isTemplatePoint, string pointPropertyID, Guid pointPropertyGuid, BasePoint basePoint, SiteClass site, StrapTable strapTable, int activeTab)
		{
			this.IsTemplatePoint = isTemplatePoint;
			this.Site = site;
			this.ActiveTab = activeTab;
			this.StrapTable = strapTable;
			if (this.StrapTable == null)
			{
				this.StrapTable = new StrapTable();
				this.StrapTable.StrapTables = null;
			}
			this.StrapTableList = new List<KeyValuePair<int, string>>();
			this.EditorEntries= new List<EditorStrapTableEntry>[6];
			this.EditorStrapTableSettings = new EditorStrapTableSettings[6];

			this.PointId = basePoint.ID;
			this.PointGuid = basePoint.IdentityGuid;
			this.PointPropertyId = pointPropertyID;
			this.PointPropertyGuid = pointPropertyGuid;
			this.LevelUnit = basePoint.LevelUnit;
			this.LevelDecimalPlaces = 9;//basePoint.LevelDecimalPlaces;
			this.VolumeUnit = basePoint.VolumeUnit;
			this.VolumeDecimalPlaces = 9;//basePoint.VolumeDecimalPlaces;
			this.DensityUnit = basePoint.DensityUnit;
			this.DensityDecimalPlaces = 9;//basePoint.DensityDecimalPlaces;
			this.TemperatureUnit = basePoint.TemperatureUnit;
			this.TemperatureDecimalPlaces = 9;//basePoint.TemperatureDecimalPlaces;
			this.MassUnit = basePoint.MassUnit;
			this.MassDecimalPlaces = 9;//basePoint.MassDecimalPlaces;

			var levelFormat = new CultureInfo( "", true).NumberFormat;
			levelFormat.NumberDecimalDigits = 9;// basePoint.LevelDecimalPlaces;
			levelFormat.NumberDecimalSeparator = site.NumberDecimalSeparator;
			levelFormat.NumberGroupSeparator = site.NumberGroupSeparator;
			levelFormat.NumberGroupSizes = site.GetNumberGroupSizes();

			var volumeFormat = new CultureInfo("", true).NumberFormat;
			volumeFormat.NumberDecimalDigits = 9;//basePoint.VolumeDecimalPlaces;
			volumeFormat.NumberDecimalSeparator = site.NumberDecimalSeparator;
			volumeFormat.NumberGroupSeparator = site.NumberGroupSeparator;
			volumeFormat.NumberGroupSizes = site.GetNumberGroupSizes();

			var temperatureFormat = new CultureInfo("", true).NumberFormat;
			temperatureFormat.NumberDecimalDigits = 9;//basePoint.TemperatureDecimalPlaces;
			temperatureFormat.NumberDecimalSeparator = site.NumberDecimalSeparator;
			temperatureFormat.NumberGroupSeparator = site.NumberGroupSeparator;
			temperatureFormat.NumberGroupSizes = site.GetNumberGroupSizes();


			var densityFormat = new CultureInfo("", true).NumberFormat;
			densityFormat.NumberDecimalDigits = 9;//basePoint.DensityDecimalPlaces;
			densityFormat.NumberDecimalSeparator = site.NumberDecimalSeparator;
			densityFormat.NumberGroupSeparator = site.NumberGroupSeparator;
			densityFormat.NumberGroupSizes = site.GetNumberGroupSizes();


			var massFormat = new CultureInfo("", true).NumberFormat;
			massFormat.NumberDecimalDigits = 9;//basePoint.MassDecimalPlaces;
			massFormat.NumberDecimalSeparator = site.NumberDecimalSeparator;
			massFormat.NumberGroupSeparator = site.NumberGroupSeparator;
			massFormat.NumberGroupSizes = site.GetNumberGroupSizes();

			for (int index=0; index < this.StrapTable.StrapTables.Length;index++)
			{
				if (this.StrapTable.StrapTables[index] != null)
				{
					var individualStrapTable = this.StrapTable.StrapTables[index];

					StrapTableList.Add(new KeyValuePair<int, string>(index, individualStrapTable.StrapTableDescription));

					this.EditorEntries[index] = new List<EditorStrapTableEntry>();
					this.EditorStrapTableSettings[index] = new EditorStrapTableSettings();
					this.StrapTable.StrapTables[index].table.ToList().ForEach(
											s => this.EditorEntries[index].Add(new EditorStrapTableEntry(
																		PointManager.FormatValueFullPrecision(typeof(decimal), basePoint.LevelUnit, levelFormat, s.Level),
																		PointManager.FormatValueFullPrecision(typeof(decimal), basePoint.VolumeUnit, volumeFormat, s.Volume)))
											);

					this.EditorStrapTableSettings[index].RoofType = ((int) individualStrapTable.RoofType).ToString();
					this.EditorStrapTableSettings[index].StrapDensity = individualStrapTable.StrapDensity.Value.ToString("N", densityFormat);
					this.EditorStrapTableSettings[index].StrapDensity = formatNumricStringDataforStrapTable(this.EditorStrapTableSettings[index].StrapDensity, densityFormat);


					this.EditorStrapTableSettings[index].StrapTemperature = individualStrapTable.StrapTemperature.Value.ToString("N", temperatureFormat);
					this.EditorStrapTableSettings[index].StrapTemperature = formatNumricStringDataforStrapTable(this.EditorStrapTableSettings[index].StrapTemperature, temperatureFormat);

					this.EditorStrapTableSettings[index].TankShellReferenceTemperature = individualStrapTable.TankShellReferenceTemperature.Value.ToString("N", temperatureFormat);
					this.EditorStrapTableSettings[index].TankShellReferenceTemperature = formatNumricStringDataforStrapTable(this.EditorStrapTableSettings[index].TankShellReferenceTemperature, temperatureFormat);

					this.EditorStrapTableSettings[index].DatumHeight = PointManager.FormatValueFullPrecision(typeof(decimal), basePoint.LevelUnit, levelFormat, individualStrapTable.DatumHeight.Value);
					this.EditorStrapTableSettings[index].RoofLandingHeight = PointManager.FormatValueFullPrecision(typeof(decimal), basePoint.LevelUnit, levelFormat, individualStrapTable.RoofLandingHeight.Value);
					this.EditorStrapTableSettings[index].RoofFloatingHeight = PointManager.FormatValueFullPrecision(typeof(decimal), basePoint.LevelUnit, levelFormat, individualStrapTable.RoofFloatingHeight.Value);
					this.EditorStrapTableSettings[index].RoofMass = PointManager.FormatValueFullPrecision(typeof(decimal), basePoint.MassUnit, massFormat, individualStrapTable.RoofMass.Value);
				}
			}
		}
		public string formatNumricStringDataforStrapTable(string stringToFormat, NumberFormatInfo numberInfo)
		{
			string returnString = string.Empty;

			// trim the trialing 0's
			if (stringToFormat.IndexOf(numberInfo.NumberDecimalSeparator) >= 0)
			{
				returnString = stringToFormat.Trim('0');
				// check and see if we need to add a 0 to the end so it looks proper
				if (returnString.IndexOf(numberInfo.NumberDecimalSeparator) == returnString.Length - 1)
				{
					returnString += "0";
				}
			}
			else 
			{
				returnString = stringToFormat;
			}

			return returnString;
		}
	}
}
