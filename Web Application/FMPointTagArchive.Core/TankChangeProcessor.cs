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

	using static FMPointTagArchive.Core.InternalClasses.TankChangeHelper;
	using System.Data.SqlClient;

	public class TankChangeProcessor : TankInventoryProcessorBase, ITankChangeProcessor
	{

		private readonly ICassandraConnectionBuilder ConnectionBuilder = new CassandraConnectionBuilder();

		public TankChangeProcessor()
		{
		}

		public DataSet Process(TankChangeProcessorSR request)
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

		internal void ExtractArchiveData(DataSet dataSet, TankChangeProcessorSR request)
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
				Dictionary<string, int> pointRowDictionary = new Dictionary<string, int>();
				Dictionary<string, double> startLevelValues = new Dictionary<string, double>();

				connection.Open();

				this.QueryCassandraForArchiveRecords(
					connection,
					request.BeginDate.DateTime,
					172800, // this is 2 days to ensure we go back to the UDT midnight record
					request.UseSmallFieldNames
					);

				this.PopulateResultsIntoDataTable(dataSet, request, true, ref pointRowDictionary, ref startLevelValues);

				this.QueryCassandraForArchiveRecords(
					connection,
					request.EndDate.DateTime,
					172800, // this is 2 days to ensure we go back to the UDT midnight record
					request.UseSmallFieldNames
					);

				this.PopulateResultsIntoDataTable(dataSet, request, false, ref pointRowDictionary, ref startLevelValues);
			}
		}

		private void QueryCassandraForArchiveRecords(CqlConnection connection, DateTime cuttoffDateTime, int rangeDuration, bool useSmallFieldNames)
		{

			DateTime cuttoffDateTimeBeginDate = cuttoffDateTime - new TimeSpan(0, 0, 0, rangeDuration, 0);
			int yearMonth = cuttoffDateTime.Year * 100 + cuttoffDateTime.Month;

			//cuttoffDateTime = new DateTime(cuttoffDateTime.Year, cuttoffDateTime.Month, cuttoffDateTime.Day, 23, 59, 59, 999);

			string QueryFormat = (useSmallFieldNames) ?
				"SELECT a,f,d,o,b FROM \"FMArchive_Data\".valuearchivedata WHERE a = {0} AND b = '{1}' AND c = {2} AND f > '{3}' and f <= '{4}' ORDER BY f DESC LIMIT 1" :
				"SELECT pointvalueguid, valuetimestamp, value FROM \"FMArchive_Data\".valuearchivedata WHERE pointvalueguid = {0} AND propertyid = '{1}' AND yearmonth = {2} AND valuetimestamp > '{3}' and valuetimestamp <= '{4}' ORDER BY valuetimestamp DESC LIMIT 1";

			const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff+0000";

			var cmd = new CqlCommand(connection, "", CqlConsistency.One);

			// Using buffering will cause the rows of each execution to be read into memory
			// rather than leaving them on the network connection.  If you choose to stop
			// using buffering, you will also need to arrange to displose of the readers created 
			// by the ExecuteReaderAsync call below to be disposed sooner than later.
			cmd.UseBuffering = true;

			// Build all the queries
			var taskList = new List<Task>();

			this.pointIDToTagDataDictionary.Values.AsEnumerable().All(tagDataDictionary =>
			{
				tagDataDictionary.Values.AsEnumerable().All(tagData =>
				{
					if (IsInterestingTag(tagData.ID))
					{
						string propertyid = string.Empty;

						propertyid = getPropertyid(tagData.ID);

						string query = string.Format(QueryFormat, tagData.PointTagGuid.ToString(), propertyid, yearMonth, cuttoffDateTimeBeginDate.ToString(dateTimeFormat), cuttoffDateTime.ToString(dateTimeFormat));

						cmd.CommandText = query;
						var part = new TankChangePartitionKeyClass() { A = tagData.PointTagGuid, B = propertyid, C = yearMonth };
						cmd.PartitionKey.Set(part);
						var task = cmd.ExecuteReaderAsync().ContinueWith(t => UpdateResult(t.Result));
						taskList.Add(task);
					}

					return true;
				});

				return true;
			});

			// Wait on the queries to complete.
			Task.WaitAll(taskList.ToArray());
		}

		private void UpdateResult(CqlDataReader reader)
		{
			if (reader != null)
			{
				while (reader.Read())
				{
					var archiveData = new ArchiveTagData();
					archiveData.PointTagGuid = new Guid(reader[0].ToString());
					archiveData.ValueTimeStamp = DateTimeOffset.Parse(reader[1].ToString());
					archiveData.Value = reader[2];
					archiveData.QualityString = (string)reader[3];
					archiveData.PropertyID = (string)reader[4];


					if (tagGuidToPointID.ContainsKey(archiveData.PointTagGuid))
					{
						string pointID = tagGuidToPointID[archiveData.PointTagGuid];
						if (pointIDToTagDataDictionary.ContainsKey(pointID))
						{
							Dictionary<Guid, TagData> tags = pointIDToTagDataDictionary[pointID];

							if (tags.ContainsKey(archiveData.PointTagGuid))
							{
								TagData t = tags[archiveData.PointTagGuid];
								t.Value = archiveData.Value;
								t.ValueTimeStamp = archiveData.ValueTimeStamp;
								t.QualityString = archiveData.QualityString;
								t.PropertyID = archiveData.PropertyID;
							}
						}
					}
				}
			}
		}

		internal void PopulateResultsIntoDataTable(DataSet dataset,
													TankChangeProcessorSR request,
													bool populateStartdata,
													ref Dictionary<string, int> pointRowDictionary,
													ref Dictionary<string, double> startLevelValues)
		{
			SortedDictionary<string, Dictionary<Guid, TagData>> pointIDToTagDataDictionaryAccess;

			pointIDToTagDataDictionaryAccess = GetAndSetDictionaryBasedOnAccessRights(pointIDToTagDataDictionary,
																						request.SiteGuid,request.UserGuid);
			int rowIndex = 0;
			foreach (string pointID in this.pointIDToTagDataDictionary.Keys)
			{
				DataRow row = null;
				Dictionary<Guid, TagData> tags = this.pointIDToTagDataDictionary[pointID];
				double tempLevelValue = 0.0;
				string tempLevelString = string.Empty;
				var levelfield = StartLevelProductFieldName;
				var levelfieldStatus = StartLevelProductStatusFieldName;
				var levelfieldtime = StartLevelProductTimestampFieldName;

				var TemperatureProductFieldName = StartTemperatureProductFieldName;
				var VolumeGrossObservedFieldName = StartVolumeGrossObservedFieldName;
				var VolumeNetStandardFieldName = StartVolumeNetStandardFieldName;
				var NetRemainingFieldName = StartNetRemainingFieldName;
				var NetAvailableFieldName = StartNetAvailableFieldName;

				var TemperatureProductStatusFieldName = StartTemperatureProductStatusFieldName;
				var VolumeGrossObservedStatusFieldName = StartVolumeGrossObservedStatusFieldName;
				var VolumeNetStandardStatusFieldName = StartVolumeNetStandardStatusFieldName;
				var NetRemainingStatusFieldName = StartNetRemainingStatusFieldName;
				var NetAvailableStatusFieldName = StartNetAvailableStatusFieldName;

				if (populateStartdata)
				{
					row = dataset.Tables[TableName].NewRow();
					dataset.Tables[TableName].Rows.Add(row);
					row[SiteNameFieldName] = request.SiteID;
					row[PointNameFieldName] = pointID;
					pointRowDictionary.Add(pointID, rowIndex);
					++rowIndex;
				}
				else
				{
					pointRowDictionary.TryGetValue(pointID, out rowIndex);
					row = dataset.Tables[TableName].Rows[rowIndex];
					levelfield = EndLevelProductFieldName;
					levelfieldStatus = EndLevelProductStatusFieldName;
					levelfieldtime = EndLevelProductTimestampFieldName;
					TemperatureProductFieldName = EndTemperatureProductFieldName;
					VolumeGrossObservedFieldName = EndVolumeGrossObservedFieldName;
					VolumeNetStandardFieldName = EndVolumeNetStandardFieldName;
					NetRemainingFieldName = EndNetRemainingFieldName;
					NetAvailableFieldName = EndNetAvailableFieldName;

					TemperatureProductStatusFieldName = EndTemperatureProductStatusFieldName;
					VolumeGrossObservedStatusFieldName = EndVolumeGrossObservedStatusFieldName;
					VolumeNetStandardStatusFieldName = EndVolumeNetStandardStatusFieldName;
					NetRemainingStatusFieldName = EndNetRemainingStatusFieldName;
					NetAvailableStatusFieldName = EndNetAvailableStatusFieldName;
				}


				foreach (TagData tagData in tags.Values)
				{
					switch (tagData.ID)
					{
						case "Level Product":
							{
								try
								{
									if (tagData.Enabled == 0)
									{
										row[levelfieldStatus] = "Out of Service";
										row[LevelProductUnitFieldName] = DBNull.Value;
										row[levelfield] = DBNull.Value;
									}
									else if (tagData.QualityString == "Restricted")
									{
										row[levelfieldStatus] = "Restricted";
										row[LevelProductUnitFieldName] = DBNull.Value;
										row[levelfield] = DBNull.Value;
									}
									else
									{
										var tempp = GetDBValue(tagData.Value);
										row[levelfieldStatus] = tagData.QualityString;
										if (tempp == DBNull.Value)
										{
											row[levelfield] = DBNull.Value;
											row[LevelProductUnitFieldName] = DBNull.Value;
											row[levelfieldtime] = DBNull.Value;
											row[levelfieldStatus] = "No Data";

											if (populateStartdata)
												startLevelValues.Add(pointID, 0.0);
										}
										else
										{
											tempLevelValue = System.Convert.ToDouble(tagData.Value);

											if (populateStartdata)
											{
												startLevelValues.Add(pointID, tempLevelValue);
											}
											else
											{
												// calculate the difference and store
												double startValue = 0.0;
												startLevelValues.TryGetValue(pointID, out startValue);
												// diff is end - start
												if (tagData.EngrUnitIndex == 27)
												{
													tempLevelString = EngUnitsHelper.EncodeFtInFractionasString(16, tempLevelValue - startValue);
												}
												else if (tagData.EngrUnitIndex == 19)
												{
													tempLevelString = EngUnitsHelper.EncodeFtInFractionasString(8, tempLevelValue - startValue);
												}
												else
												{
													tempLevelString = string.Format("{0:0.00}", (tempLevelValue - startValue));
												}
												row[ChangeLevelProductFieldName] = GetDBValue(tempLevelString);
											}
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
											row[levelfield] = GetDBValue(tempLevelString);
											row[LevelProductUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
											row[levelfieldtime] = GetDBValue(tagData.ValueTimeStamp);
											row[levelfieldStatus] = tagData.QualityString;
										}
									}
								}
								catch
								{
									row[levelfield] = GetDBValue(tagData.Value);
									row[LevelProductUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
									row[levelfieldtime] = GetDBValue(tagData.ValueTimeStamp);
									row[levelfieldStatus] = tagData.QualityString;
								}
								break;
							}
						case "Temperature Product":
							{
								if (tagData.Enabled == 0)
								{
									row[TemperatureProductStatusFieldName] = "Out of Service";
									row[TemperatureUnitFieldName] = DBNull.Value;
									row[TemperatureProductFieldName] = DBNull.Value;
								}
								else if (tagData.QualityString == "Restricted")
								{
									row[TemperatureProductStatusFieldName] = "Restricted";
									row[TemperatureUnitFieldName] = DBNull.Value;
									row[TemperatureProductFieldName] = DBNull.Value;
								}
								else
								{
									row[TemperatureProductFieldName] = GetDBValue(tagData.Value);
									row[TemperatureProductStatusFieldName] = tagData.QualityString;
									if (row[TemperatureProductFieldName] != DBNull.Value)
									{
										row[TemperatureUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
									}
									else
									{
										row[TemperatureUnitFieldName] = DBNull.Value;
										row[TemperatureProductStatusFieldName] = "No Data";
									}
								}
								break;
							}
						case "Volume Gross Observed":
							{
								if (tagData.Enabled == 0)
								{
									row[VolumeGrossObservedStatusFieldName] = "Out of Service";
									row[GrossVolUnitFieldName] = DBNull.Value;
									row[VolumeGrossObservedFieldName] = DBNull.Value;
								}
								else if (tagData.QualityString == "Restricted")
								{
									row[VolumeGrossObservedStatusFieldName] = "Restricted";
									row[GrossVolUnitFieldName] = DBNull.Value;
									row[VolumeGrossObservedFieldName] = DBNull.Value;
								}
								else
								{
									row[VolumeGrossObservedFieldName] = GetDBValue(tagData.Value);
									row[VolumeGrossObservedStatusFieldName] = tagData.QualityString;
									if (row[VolumeGrossObservedFieldName] != DBNull.Value)
									{
										row[GrossVolUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
									}
									else
									{
										row[GrossVolUnitFieldName] = DBNull.Value;
										row[VolumeGrossObservedStatusFieldName] = "No Data";
									}
								}
								break;
							}
						case "Volume Net Standard":
							{
								if (tagData.Enabled == 0)
								{
									row[VolumeNetStandardStatusFieldName] = "Out of Service";
									row[NetVolUnitFieldName] = DBNull.Value;
									row[VolumeNetStandardFieldName] = DBNull.Value;
								}
								else if (tagData.QualityString == "Restricted")
								{
									row[VolumeNetStandardStatusFieldName] = "Restricted";
									row[NetVolUnitFieldName] = DBNull.Value;
									row[VolumeNetStandardFieldName] = DBNull.Value;
								}
								else
								{
									row[VolumeNetStandardFieldName] = GetDBValue(tagData.Value);
									row[VolumeNetStandardStatusFieldName] = tagData.QualityString;
									if (row[VolumeNetStandardFieldName] != DBNull.Value)
									{
										row[NetVolUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
									}
									else
									{
										row[NetVolUnitFieldName] = DBNull.Value;
										row[VolumeNetStandardStatusFieldName] = "No Data";
									}
								}
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
								if (tagData.Enabled == 0)
								{
									row[NetRemainingStatusFieldName] = "Out of Service";
									row[NetVolUnitFieldName] = DBNull.Value;
									row[NetRemainingFieldName] = DBNull.Value;
								}
								else if (tagData.QualityString == "Restricted")
								{
									row[NetRemainingStatusFieldName] = "Restricted";
									row[NetVolUnitFieldName] = DBNull.Value;
									row[NetRemainingFieldName] = DBNull.Value;
								}
								else
								{
									row[NetRemainingFieldName] = GetDBValue(tagData.Value);
									row[NetRemainingStatusFieldName] = tagData.QualityString;
									if (row[NetRemainingFieldName] != DBNull.Value)
									{
										row[NetVolUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
									}
									else
									{
										row[NetVolUnitFieldName] = DBNull.Value;
										row[NetRemainingStatusFieldName] = "No Data";
									}
								}
								break;
							}
						case "Volume Net Standard Available":
							{
								if (tagData.Enabled == 0)
								{
									row[NetAvailableStatusFieldName] = "Out of Service";
									row[NetVolUnitFieldName] = DBNull.Value;
									row[NetAvailableFieldName] = DBNull.Value;
								}
								else if (tagData.QualityString == "Restricted")
								{
									row[NetAvailableStatusFieldName] = "Restricted";
									row[NetVolUnitFieldName] = DBNull.Value;
									row[NetAvailableFieldName] = DBNull.Value;
								}
								else
								{
									row[NetAvailableFieldName] = GetDBValue(tagData.Value);
									row[NetAvailableStatusFieldName] = tagData.QualityString;
									if (row[NetAvailableFieldName] != DBNull.Value)
									{
										row[NetVolUnitFieldName] = EngUnitsHelper.GetUnitString(tagData.EngrUnitIndex);
									}
									else
									{
										row[NetVolUnitFieldName] = DBNull.Value;
										row[NetAvailableStatusFieldName] = "No Data";
									}
								}
								break;
							}
					}
				}
			}
		}

	
	}
}
