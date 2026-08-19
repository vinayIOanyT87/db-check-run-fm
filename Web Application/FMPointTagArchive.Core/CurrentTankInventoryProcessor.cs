namespace FMPointTagArchive.Core
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CqlSharp;

    using FMPointTagArchive.Core.Interfaces;
    using FMPointTagArchive.Core.Interfaces.ServiceRequests;
    using FMPointTagArchive.Core.InternalClasses;
    using FMPointTagArchive.Core.InternalInterfaces;

    using static FMPointTagArchive.Core.InternalClasses.CurrentTankInventoryHelper;
    using System.Data.SqlClient;

    public class CurrentTankInventoryProcessor : TankInventoryProcessorBase, ICurrentTankInventoryProcessor
    {



        private readonly ICassandraConnectionBuilder ConnectionBuilder = new CassandraConnectionBuilder();

        public CurrentTankInventoryProcessor()
        {


        }

        public DataSet Process(CurrentTankInventoryProcessorSR request)
        {
            DataSet dataSet = CreateEmptyDataSet();

            DataTable refDataTable = this.DeserializeRefTable(request.refDataTableAsXML);

            this.PopulateMaps(refDataTable);

            this.ExtractArchiveData(dataSet, request);

            return dataSet;
        }

        private DataTable DeserializeRefTable(string refDataTableAsXML)
        {
            DataTable table = CreateEmptyPointTagTable();
            table.ReadXml(new StringReader(refDataTableAsXML));
            return table;
        }

        internal void PopulateMaps(DataTable refDataTable)
        {
            foreach (DataRow row in refDataTable.Rows)
            {
                var pointID = "";
                //var PointGuid = Guid.Empty;

                var t = new TagData();
                pointID = (string)row[RefDataTableFieldsPointID];
                t.PointGuid = (Guid)row[RefDataTableFieldPointGuid];
                t.EngrUnitIndex = (int)row[RefDataTableFieldEngrUnitsIndex];

                t.PointTagGuid = (Guid)row[RefDataTableFieldTagGuid];
                t.ID = (string)row[RefDataTableFieldTagID];
                t.Enabled = (int)row[RefDataTableFieldsPointEnabled];

                if (!IsInterestingTag(t.ID))
                {
                    if (!this.tagGuidToPointID.ContainsKey(t.PointGuid))
                    {
                        t.ID = "ProductID";
                        t.PropertyID = "ProductID"; // needed so that the query for point access will be called correctly when there is no archive data
                        t.PointTagGuid = t.PointGuid;
                    }
                }

                //Populate PointID to Tag Guid Dictionary for Point ID lookup
                //when processing results from Cassandra Archive Result
                if (!this.tagGuidToPointID.ContainsKey(t.PointTagGuid))
                {
                    this.tagGuidToPointID.Add(t.PointTagGuid, pointID);
                }

                //Populate pointIDToTagDataDictionary so that we can iterate
                //Through the dictionaries to populate the data table
                if (!this.pointIDToTagDataDictionary.ContainsKey(pointID))
                {
                    var tagDictionary = new Dictionary<Guid, TagData>();
                    tagDictionary.Add(t.PointTagGuid, t);
                    this.pointIDToTagDataDictionary.Add(pointID, tagDictionary);
                }
                else
                {
                    var tagDictionary = this.pointIDToTagDataDictionary[pointID];
                    tagDictionary.Add(t.PointTagGuid, t);
                }
            }
        }

        internal void ExtractArchiveData(DataSet dataSet, CurrentTankInventoryProcessorSR request)
        //string siteID, DateTimeOffset reportDate, bool useSmallFieldNames, bool useDateOnly, string cassandraConfiguration)
        {
            if (this.pointIDToTagDataDictionary == null || this.pointIDToTagDataDictionary.Count == 0)
            {
                return;
            }

            if (this.tagGuidToPointID == null || this.tagGuidToPointID.Count == 0)
            {
                return;
            }

            if (request.UserGuid.ToString() == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(request.CassandraConfiguration))
            {
                return;
            }

            var connectionString = ConnectionBuilder.Build(request.CassandraConfiguration, request.CassandraUsername, request.CassandraPassword);

            using (var connection = new CqlConnection(connectionString))
            {
                connection.Open();

                this.QueryCassandraForArchiveRecords(
                    connection,
                    request.BeginDate.DateTime,
                    172800, // this is 2 days to ensure we go back to the UDT midnight record
                    request.UseSmallFieldNames,
                    request.useDateOnly
                    );

                this.PopulateResultsIntoDataTable(dataSet, request);
            }
        }



        internal void PopulateResultsIntoDataTable(DataSet dataset, CurrentTankInventoryProcessorSR request)
        {
            SortedDictionary<string, Dictionary<Guid, TagData>> pointIDToTagDataDictionaryAccess;

            pointIDToTagDataDictionaryAccess = GetAndSetDictionaryBasedOnAccessRights(pointIDToTagDataDictionary,
                                                                                        request.SiteGuid,request.UserGuid);

            foreach (string pointID in pointIDToTagDataDictionaryAccess.Keys)
            {
                DataRow row = dataset.Tables[TableName].NewRow();
                dataset.Tables[TableName].Rows.Add(row);
                row[SiteNameFieldName] = request.SiteID;
                row[PointNameFieldName] = pointID;
                row[LastUpdateFieldName] = DBNull.Value;
                Dictionary<Guid, TagData> tags = this.pointIDToTagDataDictionary[pointID];
                double tempLevelValue = 0.0;
                string tempLevelString = string.Empty;

                foreach (TagData tagData in tags.Values)
                {
                    switch (tagData.ID)
                    {
                        case "Level Product":
                            {
                                try
                                {
                                    row[LevelProductStatusFieldName] = tagData.QualityString;
                                    if (tagData.Enabled == 0)
                                    {
                                        row[LevelProductStatusFieldName] = "Out of Service";
                                        row[LevelProductUnitFieldName] = DBNull.Value;
                                        row[LevelProductFieldName] = DBNull.Value;
                                    }
                                    else if (tagData.QualityString == "Restricted")
                                    {
                                        row[LevelProductStatusFieldName] = "Restricted";
                                        row[LevelProductUnitFieldName] = DBNull.Value;
                                        row[LevelProductFieldName] = DBNull.Value;
                                    }
                                    else
                                    {
                                        var tempp = GetDBValue(tagData.Value);
                                        if (tempp == DBNull.Value)
                                        {
                                            row[LevelProductFieldName] = DBNull.Value;
                                            row[LevelProductUnitFieldName] = DBNull.Value;
                                            row[LevelProductStatusFieldName] = "No Data";
                                        }
                                        else
                                        {
                                            tempLevelValue = System.Convert.ToDouble(tagData.Value);
                                            if (tagData.EngrUnitIndex == 27)
                                            {

                                                tempLevelString = EngUnitsHelper.EncodeFtInFractionasString(16, tempLevelValue);
                                            }
                                            else if (tagData.EngrUnitIndex == 19)
                                            {
                                                tempLevelString = EngUnitsHelper.EncodeFtInFractionasString(8, tempLevelValue);
                                            }
                                            else
                                            {
                                                tempLevelString = string.Format("{0:0.00}", tempLevelValue);
                                            }
                                            row[LevelProductFieldName] = GetDBValue(tempLevelString);
                                            row[LevelProductUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
                                        }
                                    }
                                }
                                catch
                                {
                                    row[LevelProductFieldName] = GetDBValue(tagData.Value);
                                    row[LevelProductUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
                                 }
                        break;
                            }
                        case "Temperature Product":
                            {
                                SetDataRowValues(row, tagData, TemperatureProductFieldName, TemperatureProductStatusFieldName, TemperatureUnitFieldName);
                                break;
                            }
                        case "Volume Gross Observed":
                            {
                                SetDataRowValues(row, tagData, VolumeGrossObservedFieldName, VolumeGrossObservedStatusFieldName, GrossVolUnitFieldName);
                                break;
                            }
                        case "Volume Net Standard":
                            {
                                SetDataRowValues(row, tagData, VolumeNetStandardFieldName, VolumeNetStandardStatusFieldName, NetVolUnitFieldName);
                                break;
                            }
                        case "ProductID":
                            {
                                if (tagData.Enabled == 0)
                                {
                                    row[ProductIDFieldName] = "Out of Service";
                                }
                                else if (tagData.QualityString == "Restricted")
                                {
                                    row[ProductIDFieldName] = "Restricted";
                                }
                                else
                                {
                                    row[ProductIDFieldName] = GetDBValue(tagData.Value);
                                }
                                break;
                            }
                        case "Volume Net Standard Remaining":
                            {
                                SetDataRowValues(row, tagData, NetRemainingFieldName, NetRemainingStatusFieldName, NetVolUnitFieldName);
                                break;
                            }
                        case "Volume Net Standard Available":
                            {
                                SetDataRowValues(row, tagData, NetAvailableFieldName, NetAvailableStatusFieldName, NetVolUnitFieldName);
                                break;
                            }
                        case "Volume Correction Factor":
                            {
                                if (tagData.Enabled == 0)
                                {
                                    row[VCFStatusFieldName] = "Out of Service";
                                    row[VCFFieldName] = DBNull.Value;
                                }
                                else if (tagData.QualityString == "Restricted")
                                {
                                    row[VCFStatusFieldName] = "Restricted";
                                    row[VCFFieldName] = DBNull.Value;
                                }
                                else
                                {
                                    row[VCFFieldName] = GetDBValue(tagData.Value);
                                    row[VCFStatusFieldName] = tagData.QualityString;
                                    if (row[VCFFieldName] == DBNull.Value)
                                    {
                                        row[VCFStatusFieldName] = "No Data";
                                    }
                                }
                                break;
                            }
                        case "Density Product Standard":
                            {
                                SetDataRowValues(row, tagData, DensityProductStandardFieldName, DensityProductStandardStatusFieldName, DensityProductStandardUnitFieldName);
                                break;
                            }
                        case "Volume Gross Standard":
                            {
                                SetDataRowValues(row, tagData,VolumeGrossStandardFieldName,VolumeGrossStandardStatusFieldName,VolumeGrossStandardUnitFieldName);
                                break;
                            }
                        case "Temperature Density":
                            {
                                SetDataRowValues(row, tagData, TemperatureDensityFieldName, TemperatureDensityStatusFieldName, TemperatureDensityUnitFieldName);
                                break;
                            }
                        case "Level Water":
                           {

                             try
                              {

                                 row[LevelWaterStatusFieldName] = tagData.QualityString;
                                 if (tagData.Enabled == 0)
                                 {
                                    row[LevelWaterStatusFieldName] = "Out of Service";
                                    row[LevelWaterUnitFieldName] = DBNull.Value;
                                    row[LevelWaterFieldName] = DBNull.Value;
                                 }
                                 else if (tagData.QualityString == "Restricted")
                                 {
                                    row[LevelWaterStatusFieldName] = "Restricted";
                                    row[LevelWaterUnitFieldName] = DBNull.Value;
                                    row[LevelWaterFieldName] = DBNull.Value;
                                 }
                                 else
                                 {
                                    var tempp = GetDBValue(tagData.Value);

                                    if (tempp == DBNull.Value)
                                    {
                                       row[LevelWaterFieldName] = DBNull.Value;
                                       row[LevelWaterUnitFieldName] = DBNull.Value;
                                       row[LevelWaterStatusFieldName] = "No Data";
                                    }
                                    else
                                    {

                                       tempLevelValue = System.Convert.ToDouble(tagData.Value);
                                       if (tagData.EngrUnitIndex == 27)
                                       {
                                          tempLevelString = EngUnitsHelper.EncodeFtInFractionasString(16, tempLevelValue);
                                       }
                                       else if (tagData.EngrUnitIndex == 19)
                                       {
                                          tempLevelString = EngUnitsHelper.EncodeFtInFractionasString(8, tempLevelValue);
                                       }
                                       else
                                       {
                                          tempLevelString = string.Format("{0:0.00}", tempLevelValue);
                                       }
                                       row[LevelWaterFieldName] = GetDBValue(tempLevelString);
                                       row[LevelWaterUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
                                    }
                                 }
                              }
                              catch
                              {
                                 row[LevelWaterFieldName] = GetDBValue(tagData.Value);
                                 row[LevelWaterUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
                              }
                              break;
                           }
                        case "Volume Water":
                           {
                              SetDataRowValues(row, tagData, VolumeWaterFieldName, VolumeWaterStatusFieldName, VolumeWaterUnitFieldName);
                              break;
                           }
                     }
                }
            }
        }

        private void SetDataRowValues(DataRow row, TagData tagData,string fieldName, string statusFieldName, string unitFieldName)
        {
            if (tagData.Enabled == 0)
            {
                row[statusFieldName] = "Out of Service";
                row[unitFieldName] = DBNull.Value;
                row[fieldName] = DBNull.Value;
            }
         else if (tagData.QualityString == "Restricted")
         {
            row[statusFieldName] = "Restricted";
            row[unitFieldName] = DBNull.Value;
            row[fieldName] = DBNull.Value;
         }
         else
            {
                row[fieldName] = GetDBValue(tagData.Value);
                row[statusFieldName] = tagData.QualityString;
                if (row[fieldName] != DBNull.Value)
                {
                  SetLastUpdate(row, tagData);
                  row[unitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
                }
                else
                {
                    row[unitFieldName] = DBNull.Value;
                    row[statusFieldName] = "No Data";
                }
            }
        }

        private void SetLastUpdate(DataRow row, TagData tagData)
        {
            if (tagData == null || row == null) { return; }

            if (row[LastUpdateFieldName]== DBNull.Value || ((DateTime)row[LastUpdateFieldName]) < tagData.ValueTimeStamp.LocalDateTime)
            {
                row[LastUpdateFieldName] = tagData.ValueTimeStamp.LocalDateTime;
                var t = row[LastUpdateFieldName];
            }
        }
    }
}
