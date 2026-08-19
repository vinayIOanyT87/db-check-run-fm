using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.DataObjects.CodedVariables;


using FuelsManager.Areas.Controllers;
using FuelsManager.Areas.InventoryManagement.ViewModels;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System.ComponentModel;
	using System.Globalization;
	using System.Runtime.InteropServices;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMPointCommon;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Web;

	public class StrapTableEditorController : FMBaseControllerEx
	{

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult StrapTableEditor(bool isTemplatePoint, Guid pointGuid, Guid pointPropertyGuid)
		{
			StrapTable strapTable = null;
			BasePoint basePoint = null;
			SiteClass site = null;
			string pointPropertyID = string.Empty;

			try
			{
				if (isTemplatePoint)
				{
					basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, pointGuid));
					var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointPropertyGuid));
					pointPropertyID = pointTemplateProperty.ID;
					strapTable = pointTemplateProperty.Value as StrapTable;
				}
				else
				{
					basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointGuid));
					var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropertyGuid));
					pointPropertyID = pointProperty.ID;
					strapTable = pointProperty.Value as StrapTable;
				}

				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, basePoint.SiteGuid, false, false, false));

				if (strapTable ==  null)
				{
					throw new InvalidOperationException("Strap Table not found in the Point.");
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.PartialViewWithErrorMessages("StrapTableEditor", new StrapTableEditorModel(isTemplatePoint, pointPropertyID, pointPropertyGuid, basePoint, site, strapTable, 0), JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SaveStrapTable(string editorEntries, StrapTableEditorModel strapTableModelParam)
		{

			var strapTable = new StrapTable();

			// During this process, the strap table array is adjusted to compensate for any deletions and the table selections are adjusted as well.
			strapTable.SelectedTableForStrap = strapTableModelParam.StrapTable.SelectedTableForStrap;
			strapTable.SelectedTableForWaterVolume = strapTableModelParam.StrapTable.SelectedTableForWaterVolume;
			strapTable.SelectedTableForSolidsVolume = strapTableModelParam.StrapTable.SelectedTableForSolidsVolume;

			var pointProperty = new PointProperty();
			BasePoint basePoint = null;
			SiteClass site = null;

			try
			{
				if (strapTableModelParam.IsTemplatePoint)
				{
					basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, strapTableModelParam.PointGuid));
				}
				else
				{
					basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, strapTableModelParam.PointGuid));
				}

				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, basePoint.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
					NumberDecimalDigits = 10
				};

				var strapTables = new List<IndividualStrapTable>();


				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(editorEntries))
				{
					var editorRawStrapTables = jss.Deserialize<List<EditorStrapTableEntry>[]>(editorEntries);

					// remove duplicates for the same Level (use the last value) and then sort by level entry
					for (int strapTableIndex = 0; strapTableIndex < editorRawStrapTables.Length; strapTableIndex++)
					{
						var editorRawStrapTable = editorRawStrapTables[strapTableIndex];

						var editorRawStrapTableEntries = editorRawStrapTable.GroupBy(x => x.LevelEntry).Select(x => x.Last()).OrderBy(x => x.LevelEntry).ToList();

						var individualStrapTable = new IndividualStrapTable();

						strapTables.Add(individualStrapTable);

						// convert the list of entries into a strap table
						foreach (var editorRawRow in editorRawStrapTable)
						{
							double level = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.LevelUnit, numberFormatInfo, editorRawRow.LevelEntry);
							double volume = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.VolumeUnit, numberFormatInfo, editorRawRow.VolumeEntry);

							if(strapTableModelParam.LevelUnit == EngineeringUnit.FmlFtIn16Th
							|| strapTableModelParam.LevelUnit == EngineeringUnit.FmlFtIn8Th)
                     {
								level = Math.Round(level, 12, MidpointRounding.AwayFromZero);
                     }

							individualStrapTable.table.Add(new StrapTableEntry(level, volume));
						}

						var editorStrapTableSettings = strapTableModelParam.EditorStrapTableSettings[strapTableIndex];

						individualStrapTable.StrapTableDescription = strapTableModelParam.StrapTable.StrapTables[strapTableIndex].StrapTableDescription;

						if(string.IsNullOrEmpty(individualStrapTable.StrapTableDescription))
						{
							throw new Exception(this.GetTranslatedText("Strap Table Description must not be null or empty."));
						}

						// make sure there are at least 4 entries in each table
						if(individualStrapTable.table.Count < 4)
						{
							throw new Exception(this.GetTranslatedText("Strap Tables must have a minimum of 4 entries."));
						}

						this.UpdateStrapTableSettings(individualStrapTable, editorStrapTableSettings, strapTableModelParam, numberFormatInfo, strapTableIndex);
					}
				}

				strapTable.StrapTables = strapTables.ToArray();

				this.SaveStrapTable(strapTableModelParam.IsTemplatePoint, strapTable, basePoint, numberFormatInfo, strapTableModelParam.PointPropertyGuid);
			}
			catch (CommunicationException e)
			{
				this.OnError(new Exception(this.GetTranslatedText(e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}


			return this.JsonWithErrorMessages(null);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddStrapTable(string editorEntries, StrapTableEditorModel strapTableModelParam)
		{

			var strapTable = new StrapTable();

			// During this process, the strap table array is adjusted to compensate for any deletions and the table selections are adjusted as well.
			strapTable.SelectedTableForStrap = strapTableModelParam.StrapTable.SelectedTableForStrap;
			strapTable.SelectedTableForWaterVolume = strapTableModelParam.StrapTable.SelectedTableForWaterVolume;
			strapTable.SelectedTableForSolidsVolume = strapTableModelParam.StrapTable.SelectedTableForSolidsVolume;

			var pointProperty = new PointProperty();
			Point point = null;
			SiteClass site = null;

			try
			{
				if (strapTableModelParam.IsTemplatePoint)
				{
					var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, strapTableModelParam.PointGuid));
					point = new Point(pointTemplate) { ID = pointTemplate.ID, SiteGuid = pointTemplate.SiteGuid, SiteID = pointTemplate.SiteID };
				}
				else
				{
					point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, strapTableModelParam.PointGuid));
				}

				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, point.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
					NumberDecimalDigits = 10
				};
				var strapTables = new List<IndividualStrapTable>();


				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(editorEntries))
				{
					var editorRawStrapTables = jss.Deserialize<List<EditorStrapTableEntry>[]>(editorEntries);

					// remove duplicates for the same Level (use the last value) and then sort by level entry
					for (int strapTableIndex = 0; strapTableIndex < editorRawStrapTables.Length; strapTableIndex++)
					{
						var editorRawStrapTable = editorRawStrapTables[strapTableIndex];

						var editorRawStrapTableEntries = editorRawStrapTable.GroupBy(x => x.LevelEntry).Select(x => x.Last()).OrderBy(x => x.LevelEntry).ToList();

						var individualStrapTable = new IndividualStrapTable();

						strapTables.Add(individualStrapTable);

						// convert the list of entries into a strap table
						foreach (var editorRawRow in editorRawStrapTable)
						{
							double level = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.LevelUnit, numberFormatInfo, editorRawRow.LevelEntry);
							double volume = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.VolumeUnit, numberFormatInfo, editorRawRow.VolumeEntry);

							if (strapTableModelParam.LevelUnit == EngineeringUnit.FmlFtIn16Th
							|| strapTableModelParam.LevelUnit == EngineeringUnit.FmlFtIn8Th)
							{
								level = Math.Round(level, 12, MidpointRounding.AwayFromZero);
							}

							individualStrapTable.table.Add(new StrapTableEntry(level, volume));
						}

						var editorStrapTableSettings = strapTableModelParam.EditorStrapTableSettings[strapTableIndex];

						individualStrapTable.StrapTableDescription = strapTableModelParam.StrapTable.StrapTables[strapTableIndex].StrapTableDescription;

						this.UpdateStrapTableSettings(individualStrapTable, editorStrapTableSettings, strapTableModelParam, numberFormatInfo, strapTableIndex);
					}
				}


				strapTables.Add(new IndividualStrapTable()
				{
					StrapTableDescription = "Strap Table " + (strapTables.Count + 1).ToString()
				});


				strapTable.StrapTables = strapTables.ToArray();

				this.ModelState.Remove("ActiveTab");
				this.ModelState.Remove("StrapTable.StrapTables[" + (strapTables.Count - 1).ToString() + "].StrapTableDescription");
				this.ModelState.Remove("EditorStrapTableSettings[" + (strapTables.Count - 1).ToString() + "]");

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}


			// reload the strap table
			return this.PartialViewWithErrorMessages("StrapTableEditor", new StrapTableEditorModel(strapTableModelParam.IsTemplatePoint, pointProperty.ID, pointProperty.PointPropertyGuid, point, site, strapTable, strapTable.StrapTables.Length - 1));
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteStrapTable(string editorEntries, StrapTableEditorModel strapTableModelParam, int deleteTableIndex)
		{

			var strapTable = new StrapTable();

			// During this process, the strap table array is adjusted to compensate for any deletions and the table selections are adjusted as well.
			strapTable.SelectedTableForStrap = strapTableModelParam.StrapTable.SelectedTableForStrap;
			strapTable.SelectedTableForWaterVolume = strapTableModelParam.StrapTable.SelectedTableForWaterVolume;
			strapTable.SelectedTableForSolidsVolume = strapTableModelParam.StrapTable.SelectedTableForSolidsVolume;

			var pointProperty = new PointProperty();
			Point point = null;
			SiteClass site = null;
			try
			{
				if (strapTableModelParam.IsTemplatePoint)
				{
					var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, strapTableModelParam.PointGuid));
					point = new Point(pointTemplate) { ID = pointTemplate.ID, SiteGuid = pointTemplate.SiteGuid, SiteID = pointTemplate.SiteID };
				}
				else
				{
					point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, strapTableModelParam.PointGuid));
				}

				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, point.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
					NumberDecimalDigits = 10
				};

				var strapTables = new List<IndividualStrapTable>();


				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(editorEntries))
				{
					var editorRawStrapTables = jss.Deserialize<List<EditorStrapTableEntry>[]>(editorEntries);

					// remove duplicates for the same Level (use the last value) and then sort by level entry
					for (int strapTableIndex = 0; strapTableIndex < editorRawStrapTables.Length; strapTableIndex++)
					{
						if (strapTableIndex == deleteTableIndex)
						{
							continue;
						}

						var editorRawStrapTable = editorRawStrapTables[strapTableIndex];

						var editorRawStrapTableEntries = editorRawStrapTable.GroupBy(x => x.LevelEntry).Select(x => x.Last()).OrderBy(x => x.LevelEntry).ToList();

						var individualStrapTable = new IndividualStrapTable();

						strapTables.Add(individualStrapTable);

						// Fix the index for the table selections
						if (strapTable.SelectedTableForStrap == strapTableIndex)
						{
							strapTable.SelectedTableForStrap = strapTables.Count - 1;
						}

						if (strapTable.SelectedTableForWaterVolume == strapTableIndex)
						{
							strapTable.SelectedTableForWaterVolume = strapTables.Count - 1;
						}

						if (strapTable.SelectedTableForSolidsVolume == strapTableIndex)
						{
							strapTable.SelectedTableForSolidsVolume = strapTables.Count - 1;
						}


						// convert the list of entries into a strap table
						foreach (var editorRawRow in editorRawStrapTable)
						{
							individualStrapTable.table.Add(
									new StrapTableEntry(
										(double)PointManager.ParseValue(typeof(double), strapTableModelParam.LevelUnit, NumberFormatInfo.InvariantInfo, editorRawRow.LevelEntry),
										Convert.ToDouble(editorRawRow.VolumeEntry)));
						}

						var editorStrapTableSettings = strapTableModelParam.EditorStrapTableSettings[strapTableIndex];

						if (strapTableModelParam.StrapTable.StrapTables[strapTableIndex].StrapTableDescription == "Strap Table " + (strapTableIndex + 1).ToString())
						{
							individualStrapTable.StrapTableDescription = "Strap Table " + (strapTables.Count).ToString();
						}
						else
						{
							individualStrapTable.StrapTableDescription = strapTableModelParam.StrapTable.StrapTables[strapTableIndex].StrapTableDescription;
						}

						this.UpdateStrapTableSettings(individualStrapTable, editorStrapTableSettings, strapTableModelParam, numberFormatInfo, strapTableIndex);
					}
				}

				strapTable.StrapTables = strapTables.ToArray();
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

			ModelState.Remove("StrapTable.SelectedTableForStrap");
			ModelState.Remove("StrapTable.SelectedTableForWaterVolume");
			ModelState.Remove("StrapTable.SelectedTableForSolidsVolume");
			ModelState.Remove("ActiveTab");
			for (int strapTableIndex = 0; strapTableIndex < 6; strapTableIndex++)
			{
				ModelState.Remove("StrapTable.StrapTables[" + strapTableIndex.ToString() + "].StrapTableDescription");
			}

			foreach (var ms in ModelState.ToArray())
			{
				if (ms.Key.StartsWith("EditorStrapTableSettings"))
				{
					ModelState.Remove(ms);
				}
			}

			if (deleteTableIndex >= strapTable.StrapTables.Length)
			{
				deleteTableIndex--;
			}

			// reload the strap table
			return this.PartialViewWithErrorMessages("StrapTableEditor", new StrapTableEditorModel(strapTableModelParam.IsTemplatePoint, pointProperty.ID, pointProperty.PointPropertyGuid, point, site, strapTable, deleteTableIndex));
		}



		protected void SaveStrapTable(bool isTemplatePoint, StrapTable strapTable, BasePoint basePoint, NumberFormatInfo numberFormatInfo, Guid pointPropertyGuid)
		{
			ValidateStrapTable(this.ModelState, numberFormatInfo, basePoint, strapTable, null);

			if (this.ModelState.IsValid)
			{
				if (isTemplatePoint)
				{
					var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointPropertyGuid));
					pointTemplateProperty.Value = strapTable;
					FMChannelHelper.MakeCall<IPointTemplateProperties>(x => x.ModifyPointTemplatePropertyValue(this.Security, pointTemplateProperty));
				}
				else
				{
					var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropertyGuid));
					pointProperty.Value = strapTable;
					FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.Security, pointProperty, false, false));
				}

				this.AddSuccess("Save Successful");
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult Import(string editorEntries, StrapTableEditorModel strapTableModelParam, int importTableIndex)
		{
			var strapTable = new StrapTable();
			int numberFoundInFile = -1;
			int dataPrecision = -1;
			bool precisionIsLessThanFile = false;

			// During this process, the strap table array is adjusted to compensate for any deletions and the table selections are adjusted as well.
			strapTable.SelectedTableForStrap = strapTableModelParam.StrapTable.SelectedTableForStrap;
			strapTable.SelectedTableForWaterVolume = strapTableModelParam.StrapTable.SelectedTableForWaterVolume;
			strapTable.SelectedTableForSolidsVolume = strapTableModelParam.StrapTable.SelectedTableForSolidsVolume;

			BasePoint basePoint = null;
			SiteClass site = null;
			bool IsFM12OrHigherStrapTable = false;

			try
			{
				if (strapTableModelParam.IsTemplatePoint)
				{
					basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, strapTableModelParam.PointGuid));
				}
				else
				{
					basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, strapTableModelParam.PointGuid));
				}

				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, basePoint.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
					NumberDecimalDigits = 10
				};

				var strapTables = new List<IndividualStrapTable>();


				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(editorEntries))
				{
					var editorRawStrapTables = jss.Deserialize<List<EditorStrapTableEntry>[]>(editorEntries);

					// remove duplicates for the same Level (use the last value) and then sort by level entry
					for (int strapTableIndex = 0; strapTableIndex < editorRawStrapTables.Length; strapTableIndex++)
					{
						var editorRawStrapTable = editorRawStrapTables[strapTableIndex];

						var editorRawStrapTableEntries = editorRawStrapTable.GroupBy(x => x.LevelEntry).Select(x => x.Last()).OrderBy(x => x.LevelEntry).ToList();

						var individualStrapTable = new IndividualStrapTable();

						strapTables.Add(individualStrapTable);
						
						// convert the list of entries into a strap table
						foreach (var editorRawRow in editorRawStrapTable)
						{
							individualStrapTable.table.Add(
									new StrapTableEntry(
										(double)PointManager.ParseValue(typeof(double), strapTableModelParam.LevelUnit, numberFormatInfo, editorRawRow.LevelEntry),
										(double)PointManager.ParseValue(typeof(double), strapTableModelParam.VolumeUnit, numberFormatInfo, editorRawRow.VolumeEntry)));
						}

						var editorStrapTableSettings = strapTableModelParam.EditorStrapTableSettings[strapTableIndex];

						individualStrapTable.StrapTableDescription = strapTableModelParam.StrapTable.StrapTables[strapTableIndex].StrapTableDescription;

						this.UpdateStrapTableSettings(individualStrapTable, editorStrapTableSettings, strapTableModelParam, numberFormatInfo, strapTableIndex);
						
					}
				}
				
				strapTable.StrapTables = strapTables.ToArray();

				// we are going to assume we are going to receive only 1 file
				if (this.Request.Files.Count > 0)
				{
					HttpPostedFileBase file = this.Request.Files[0];
					//int fileSize = file.ContentLength;
					string fileName = file.FileName;
					//string mimeType = file.ContentType;
					System.IO.Stream fileContent = file.InputStream;
					//To save file, use SaveAs method

					TSTFileOperations tst = new TSTFileOperations();
					IsFM12OrHigherStrapTable = 
										tst.ReadStrapFile(fileContent,
										basePoint.LevelUnit,
										basePoint.VolumeUnit,
										basePoint.MassUnit,
										strapTable,
										importTableIndex,
										fileName,
										strapTableModelParam.LevelDecimalPlaces,
										strapTableModelParam.VolumeDecimalPlaces,
										strapTableModelParam.DensityDecimalPlaces,
										strapTableModelParam.TemperatureDecimalPlaces,
										strapTableModelParam.MassDecimalPlaces,
										ref numberFoundInFile,
										ref dataPrecision,
										ref precisionIsLessThanFile);

				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

			foreach (var ms in ModelState.ToArray())
			{
				if (ms.Key.StartsWith("EditorStrapTableSettings[" + importTableIndex.ToString() + "]"))
				{
					// If >= FM12 Strap Table then refresh all of the Strap Table Settings
					if (IsFM12OrHigherStrapTable)
					{
						ModelState.Remove(ms);
					}
					// Refresh all Strap Table Settings but RoofType, DatumHeight, and TankShellReferenceTemperature 
					else
					{
						if (ms.Key != "EditorStrapTableSettings[" + importTableIndex.ToString() + "].RoofType" &&
							ms.Key != "EditorStrapTableSettings[" + importTableIndex.ToString() + "].DatumHeight" &&
							ms.Key != "EditorStrapTableSettings[" + importTableIndex.ToString() + "].TankShellReferenceTemperature")
							ModelState.Remove(ms);
					}
				}
			}
			ModelState.Remove("StrapTable.StrapTables[" + importTableIndex.ToString() + "].StrapTableDescription");

			if (ModelState.IsValid)
			{
				string successMessage = "File successfully loaded!";
				// store the strap table entries versus actual found if they differ
				// stored as configured - actual found
				// we do this here because it only applies if the strap table was successfully loaded
				if (numberFoundInFile > 0)	// this should only be true if we successfully imported a strap file
				{
					if(numberFoundInFile != strapTable.StrapTables[importTableIndex].table.Count)
					{
						successMessage += "-" + numberFoundInFile.ToString() + "-" + strapTable.StrapTables[importTableIndex].table.Count.ToString();
					}
					if(precisionIsLessThanFile == true)
					{
						successMessage += "-P-" + "Precision-" + dataPrecision.ToString();
					}
				}
				this.AddSuccess(successMessage);
			}

			return this.PartialViewWithErrorMessages("StrapTableEditor", new StrapTableEditorModel(strapTableModelParam.IsTemplatePoint, strapTableModelParam.PointPropertyId, strapTableModelParam.PointPropertyGuid, basePoint, site, strapTable, 0));
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult ValidateRegionalSettings()
		{

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(this.Security, this.Security.SiteID, false));

			// we need to validate the regional settings for the Strap table export
			// the decimal separator, number group separator and list separator have to be different.
			//the number group separator only come in place when they select to use the number grouping

			// the easiest way to check is creating a list or array of the values and than check to see if there are duplicates.
			List<string> separators = new List<string>();

			separators.Add(site.NumberDecimalSeparator);
			if (site.NumberGroupSizesType != NUMBER_GROUP_SIZES_TYPE.ZERO)  // if we are not using the the group separator no need to compare
			{
				separators.Add(site.NumberGroupSeparator);
			}
			separators.Add(site.ListSeparator);

			Dictionary<string, string> results = new Dictionary<string, string>();
			// check if duplicates exist
			if (separators.Count != separators.Distinct().Count())
			{
				ModelState.AddModelError(string.Empty, "Cannot generate export file. The Decimal Symbol, Digit Grouping Symbol and List Separator have to be different. Please correct the delimiters in the Regional Settings.");
			}
			return this.JsonWithErrorMessages(null);
		}

		[HttpGet]
		public FileContentResult Export(Guid pointGuid, Guid pointPropertyGuid, int exportTableIndex, bool isTemplatePoint)
		{
			TSTFileOperations tst = new TSTFileOperations();

			BasePoint basePoint = null;

			SiteClass site = null;

			StrapTable strapTable = null;

			try
			{
				if (isTemplatePoint)
				{
					basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, pointGuid));
					var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointPropertyGuid));
					strapTable = pointTemplateProperty.Value as StrapTable;
				}
				else
				{
					basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointGuid));
					var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropertyGuid));
					strapTable = pointProperty.Value as StrapTable;
				}

				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, basePoint.SiteGuid, false, false, false));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			Response.AddHeader("Content-Disposition", "attachment; filename=" + basePoint.ID + " " + strapTable.StrapTables[exportTableIndex].StrapTableDescription + ".tst");
			return new FileContentResult(System.Text.Encoding.UTF8.GetBytes(tst.WriteStrapFile(strapTable, basePoint, site, exportTableIndex)), System.Net.Mime.MediaTypeNames.Application.Octet);
		}


		[NonAction]
		public static IEnumerable<SelectListItem> GetRoofTypes()
		{
			return GetEnumSelectList<RoofTypeEnum>();
		}

		private void UpdateStrapTableSettings(IndividualStrapTable individualStrapTable, EditorStrapTableSettings editorStrapTableSettings, StrapTableEditorModel strapTableModelParam, NumberFormatInfo numberFormatInfo, int tableIndex)
		{
			individualStrapTable.RoofType = (RoofTypeEnum)Convert.ToInt32(editorStrapTableSettings.RoofType);
			individualStrapTable.RoofMass.Value = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.MassUnit, numberFormatInfo, editorStrapTableSettings.RoofMass);
			individualStrapTable.RoofLandingHeight.Value = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.LevelUnit, numberFormatInfo, editorStrapTableSettings.RoofLandingHeight);
			individualStrapTable.RoofFloatingHeight.Value = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.LevelUnit, numberFormatInfo, editorStrapTableSettings.RoofFloatingHeight);
			individualStrapTable.DatumHeight.Value = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.LevelUnit, numberFormatInfo, editorStrapTableSettings.DatumHeight);
			individualStrapTable.StrapDensity.Value = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.DensityUnit, numberFormatInfo, editorStrapTableSettings.StrapDensity);
			individualStrapTable.StrapTemperature.Value = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.TemperatureUnit, numberFormatInfo, editorStrapTableSettings.StrapTemperature);
			individualStrapTable.TankShellReferenceTemperature.Value = (double)PointManager.ParseValue(typeof(double), strapTableModelParam.TemperatureUnit, numberFormatInfo, editorStrapTableSettings.TankShellReferenceTemperature);
		}




		public static void ValidateStrapTable(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, BasePoint basePoint, StrapTable strapTable, List<PointDefaultUnitChangeHistory> defaultUnitConversionHistory)
		{
			// for the strap table we do not update values in the point editor however we can update the data types and will require conversion

			int tableIndex = 0;
			foreach (var individualStrapTable in strapTable.StrapTables)
			{
				if (defaultUnitConversionHistory != null)
				{
					foreach (var historyEntry in defaultUnitConversionHistory)
					{
						if (historyEntry.PerformConversion)
						{
							switch (historyEntry.UnitType)
							{
								case "LevelUnit":
									var newRoofLandingHeightDbl = individualStrapTable.RoofLandingHeight.Value;
									individualStrapTable.RoofLandingHeight.Value = EngineeringUnits.Convert(newRoofLandingHeightDbl, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, newRoofLandingHeightDbl);
									var newRoofFloatingHeightDbl = individualStrapTable.RoofFloatingHeight.Value;
									individualStrapTable.RoofFloatingHeight.Value = EngineeringUnits.Convert(newRoofFloatingHeightDbl, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, newRoofFloatingHeightDbl);
									var newDatumHeightDbl = individualStrapTable.DatumHeight.Value;
									individualStrapTable.DatumHeight.Value = EngineeringUnits.Convert(newDatumHeightDbl, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, newDatumHeightDbl);
									// convert each of the entries in the strap table
									foreach (var strapTableEntry in individualStrapTable.table)
									{
										var newLevel = strapTableEntry.Level;
										strapTableEntry.Level = EngineeringUnits.Convert(newLevel, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, newLevel);
									}
									break;
								case "TemperatureUnit":
									var newStrapTemperatureDbl = individualStrapTable.StrapTemperature.Value;
									individualStrapTable.StrapTemperature.Value = EngineeringUnits.Convert(newStrapTemperatureDbl, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, newStrapTemperatureDbl);
									var newTankShellReferenceTemperatureDbl = individualStrapTable.TankShellReferenceTemperature.Value;
									individualStrapTable.TankShellReferenceTemperature.Value = EngineeringUnits.Convert(newTankShellReferenceTemperatureDbl, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, newTankShellReferenceTemperatureDbl);
									break;
								case "VolumeUnit":
									// convert each of the entries in the strap table
									foreach (var strapTableEntry in individualStrapTable.table)
									{
										var newVolume = strapTableEntry.Volume;
										strapTableEntry.Volume = EngineeringUnits.Convert(newVolume, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, newVolume);
									}
									break;
								case "DensityUnit":
									var newStrapDensityDbl = individualStrapTable.StrapDensity.Value;
									individualStrapTable.StrapDensity.Value = EngineeringUnits.Convert(newStrapDensityDbl, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 15.555);
									break;
								case "MassUnit":
									var newRoofMassDbl = individualStrapTable.RoofMass.Value;
									individualStrapTable.RoofMass.Value = EngineeringUnits.Convert(newRoofMassDbl, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, newRoofMassDbl);
									break;
							}
						}
					}
				}

				// we need to verify the roof settings are valid for the selected roof type
				// the only one that we care about is roof not in strap
				if (individualStrapTable.RoofType == RoofTypeEnum.RoofMassNotInStrap)
				{
					// make sure we have a roof mass
					if (individualStrapTable.RoofMass.Value == 0.0)
					{
						modelState.AddModelError(string.Empty, "Roof Mass required for selected Roof Type.");
					}
					else if (individualStrapTable.RoofMass.Value < 0.0)
					{
						modelState.AddModelError(string.Empty, "Roof Mass cannot be a negative value.");
					}
					// make sure that the floating height is greater than the landed height
					if (individualStrapTable.RoofFloatingHeight.Value <= individualStrapTable.RoofLandingHeight.Value)
					{
						modelState.AddModelError(string.Empty, "Roof Floating Height must be greater than Roof Landing Height.");
					}
				}
				tableIndex++;
			}
		}
	}
}