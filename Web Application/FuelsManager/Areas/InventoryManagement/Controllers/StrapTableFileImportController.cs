namespace FuelsManager.Areas.InventoryManagement.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Web;
    using System.Web.Mvc;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FuelsManager.Areas.Controllers;
    using FuelsManager.Areas.InventoryManagement.ViewModels;
    using FMBusinessObjects.Constants;

    using FuelsManager.FMWebApp;
    using System.IO;
    //  using Microsoft.Web.Services2.Security.Utility;

    using System.Linq;
    using FMPointCommon;
    using System.Net;
    using System.Text;
    using System.Windows.Forms.VisualStyles;
    using System.Globalization;
    using System.Security.Policy;

    public class StrapTableFileImportController : FMBaseControllerEx
    {
        public ActionResult StrapTableFileImportView(string logText = "")
        {
            var model = new StrapTableFileImportModel(this.Security.SiteGuid, logText);
            return this.View(model);
        }

        [HttpPost , ValidateJsonAntiForgeryToken]
        public ActionResult Import(List<HttpPostedFileBase> tstFileImportBtn, HttpPostedFileBase csvFileImportBtn)
        {
            try
            {
                var model = new StrapTableFileImportModel(this.Security.SiteGuid);
                // Beginning import process
                model.logText = "Beginning Import...";
                int numberFoundInFile = -1;
                int dataPrecision = -1;
                bool precisionIsLessThanFile = false;

                // Check for file validity
                var csvExtension = Path.GetExtension(csvFileImportBtn.FileName);
                if (csvExtension != ".csv")
                {
                    model.logText += "\nCsv expected, Stopping Import";
                }
                foreach (HttpPostedFileBase f in tstFileImportBtn)
                {
                    var tstExtension = Path.GetExtension(f.FileName);
                    if (tstExtension != ".tst")
                    {
                        model.logText += "\nTst expected, Stopping Import";
                    }
                }
                //read in the csv file to get access to point_id for building the points
                List<string[]> readPoints = new List<string[]>(); // each string array represents a row in the CSV file
                using (var reader = new StreamReader(csvFileImportBtn.InputStream))
                {
                    string header = reader.ReadLine();
                    if(header == null)
                    {
                        return new HttpStatusCodeResult(400, "Invalid CSV");
                    }
                    
                    while (!reader.EndOfStream)
                    {
                    
                        var line = reader.ReadLine();
                        string [] entry = line.Split(',');
                        readPoints.Add(entry);
                    }
                }

                var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
                var numberFormatInfo = new NumberFormatInfo
                {
                    NumberGroupSizes = site.GetNumberGroupSizes(),
                    NumberGroupSeparator = site.NumberGroupSeparator,
                    NumberDecimalSeparator = site.NumberDecimalSeparator
                };
                //Create point, straptable, and read tst files

                foreach (string[] rawPoint in readPoints)
                {
                    try
                    {
                        // Build a point
                        string pointID = rawPoint[0];
                        Guid pointGuid = FMChannelHelper.MakeCall<IPoints, Guid>(x => x.GetIdentityGuid(this.Security, pointID));
                        if (pointGuid == Guid.Empty)
                        {
                            model.logText += "\n" + rawPoint[0] + ": " + rawPoint[1] + " import failed. " + pointID + " was not found at " + site.ID + ".";
                            continue;
                        }
                        BasePoint point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointGuid));

                        

                        // Import tst files and build individual strap table
                        string tstFileName = rawPoint[1]; //holds tst file name
                        HttpPostedFileBase tstFile = null;
                        for (int i = 0; i < tstFileImportBtn.Count(); i++)
                        {
                            if (tstFileImportBtn.ElementAt(i).FileName.Equals(tstFileName))
                            {
                                tstFile = tstFileImportBtn.ElementAt(i);
                            }
                        }

                        // Read in tst and fill out strap table
                        MemoryStream fileContent = new MemoryStream();
                        tstFile.InputStream.CopyTo(fileContent);
                        tstFile.InputStream.Position = fileContent.Position = 0;
                        TSTFileOperations tst = new TSTFileOperations();
                        int importTableIndex = Int32.Parse(rawPoint[2]) - 1; // 0 based index

                        // Load the strapTable from the point
                        StrapTable strapTable;
                        var strapTableGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, pointGuid, "Strap Table"));
                        var pointProperty = FMChannelHelper.MakeCall<IPointProperties, FMBusinessObjects.DataObjects.PointProperty>(x => x.Get(this.Security, strapTableGuid));
                        strapTable = pointProperty.Value as StrapTable;

                        // if the number of the table is higher than the size of the individualStrapTables[]
                        // resize the array to accomodate this number and put default individualStrapTables if needed
                        if (strapTable.StrapTables.Length - 1 < importTableIndex)
                        {
                            var currentArray = strapTable.StrapTables;
                            Array.Resize(ref currentArray, importTableIndex + 1);
                            for (int i = 0; i < currentArray.Length; i++)
                            {
                                if (currentArray[i] == null)
                                {
                                    currentArray[i] = new IndividualStrapTable()
                                    {
                                        StrapTableDescription = "Strap Table " + (i + 1).ToString()
                                    };
                                    currentArray[i].table.Add(new StrapTableEntry(0.0, 0.0));
                                    currentArray[i].table.Add(new StrapTableEntry(10.0, 2500.0));
                                    currentArray[i].table.Add(new StrapTableEntry(20.0, 5000.0));
                                    currentArray[i].table.Add(new StrapTableEntry(40.0, 10000.0));
                                }
                            }
                            strapTable.StrapTables = currentArray;
                        }

                        bool strapImportRead =
                                        tst.ReadStrapFile(
                                            fileContent,
                                            point.LevelUnit, // going to throw null pointer exception
                                            point.VolumeUnit, // going to throw null pointer exception
                                            point.MassUnit, // going to throw null pointer exception
                                            strapTable,
                                            importTableIndex,
                                            tstFile.FileName,
                                            model.LevelDecimalPlaces,
                                            model.VolumeDecimalPlaces,
                                            model.DensityDecimalPlaces,
                                            model.TemperatureDecimalPlaces,
                                            model.MassDecimalPlaces,
                                            ref numberFoundInFile,
                                            ref dataPrecision,
                                            ref precisionIsLessThanFile);

                        // Validate the strap table.
                        StrapTableEditorController.ValidateStrapTable(this.ModelState, numberFormatInfo, point, strapTable, null);
                        // If validation has failed, don't import this line.
                        // The Point property will rollback because we don't call ModifyPointPropertyValue on it
                        if (!this.ModelState.IsValid)
                        {
                            model.logText += "\n" + rawPoint[0] + ": " + rawPoint[1] + " import failed. " + this.ModelState[""].Errors[0].ErrorMessage;
                            continue;
                        }

                        // Do the assignment to Product or Bottoms if necessary
                        if (rawPoint[3] == "Product")
                            strapTable.SelectedTableForStrap = importTableIndex;
                        else if (rawPoint[3] == "Bottoms")
                            strapTable.SelectedTableForWaterVolume = importTableIndex;
                        else if (!string.IsNullOrWhiteSpace(rawPoint[3]))
                        {
                            model.logText += "\n" + rawPoint[0] + ": " + rawPoint[1] + " import failed. " + rawPoint[3] + " assignment is not a valid Table Assignment.";
                            continue;
                        }

                        // Save
                        FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.Security, pointProperty, false, false));
                        model.logText += "\n" + pointID + ": " + tstFile.FileName + " imported successfully.";
                    }

                    catch (Exception e)
                    {
                        if (!(int.TryParse(rawPoint[2], out _)))
                        {
                            model.logText += "\n" + rawPoint[0] + ": Strap Table Number was not in a correct format." + rawPoint[2];
                        }
                        else if(e is NullReferenceException)
                        {
                            model.logText += "\n" + "Error corresponding to point: " + rawPoint[0] +  ", Invalid data from selected file: " + rawPoint[1];
                        }
                        else
                        {
                            model.logText += "\n" + rawPoint[0] + ": " + rawPoint[1] + " import failed. " + e.Message;
                        }
                    }
                }
                model.logText += "\nImport complete.";
                return Content(model.logText);
                
            }
            catch(Exception e)
            {
                var model = new StrapTableFileImportModel(this.Security.SiteGuid);
                model.logText = "The import encountered an exception!";
                model.logText = model.logText + "\n" + e.Message;
                return View("StrapTableFileImportView", model);
            }
        }

  

    }
}