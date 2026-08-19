using CqlSharp;
using FMPointTagArchive.Core.InternalClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static FMPointTagArchive.Core.InternalClasses.CurrentTankInventoryHelper;

namespace FMPointTagArchive.Core
{
    public class TankInventoryProcessorBase
    {
        public TankInventoryProcessorBase()
        {
            this.tagGuidToPointID = new Dictionary<Guid, string>();
            this.pointIDToTagDataDictionary = new SortedDictionary<string, Dictionary<Guid, TagData>>();
        }

        internal readonly Dictionary<Guid, string> tagGuidToPointID;

        internal readonly SortedDictionary<string, Dictionary<Guid, TagData>> pointIDToTagDataDictionary;
        internal Dictionary<PointValueIdentifier, PointValueAccess> EnumerateRestrictedAccessByPointValueIdenfierList(System.Data.SqlTypes.SqlGuid SiteGuid,
                                                                                                                    System.Data.SqlTypes.SqlGuid UserGuid,
                                                                                                                    List<PointValueIdentifier> pointValueIdentifierList)
        {
            DataSet set;

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = "[dbo].[usp_EnumerateRestrictedAccessByPointValueIdentifiers]";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
                cmd.Parameters.AddWithValue("@UserGuid", UserGuid);

                var pointValueIdentifiers = new DataTable();
                pointValueIdentifiers.Columns.Add("Guid", typeof(Guid));
                pointValueIdentifiers.Columns.Add("PropertyId", typeof(string));
                pointValueIdentifiers.Columns.Add("ValueType", typeof(byte));

                foreach (var pointValueIdentifier in pointValueIdentifierList)
                {
                    var row = pointValueIdentifiers.NewRow();
                    row[0] = pointValueIdentifier.IdentityGuid;
                    row[1] = pointValueIdentifier.PropertyID;
                    row[2] = pointValueIdentifier.PointValueType;

                    pointValueIdentifiers.Rows.Add(row);
                }
                SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointValueIdentifiers", SqlDbType.Structured);
                tableValuedParameter.Value = pointValueIdentifiers;
                tableValuedParameter.TypeName = "dbo.utt_PointValueIdentifier";

                try
                {
                    set = this.GetDataSet(cmd);
                }
                catch
                {
                    return null;
                }

            }
            Dictionary<PointValueIdentifier, PointValueAccess> pointValueAccessDictionary = new Dictionary<PointValueIdentifier, PointValueAccess>();

            if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
            {
                return pointValueAccessDictionary;
            }

            DataTable table = set.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = (Guid)row["PointValueGuid"], PropertyID = (string)row["PointValuePropertyId"], PointValueType = (PointValueType)Convert.ToInt32(row["PointValueType"]) };
                var pointValueAccess = new PointValueAccess() { View = (bool)row["View"], Modify = (bool)row["Modify"], ExceedRange = (bool)row["ExceedRange"], Override = (bool)row["Override"] };
                pointValueAccessDictionary.Add(pointValueIdentifier, pointValueAccess);
            }

            return pointValueAccessDictionary;
        }

        internal DataSet GetDataSet(SqlCommand command)
        {
            DataSet ResultDataSet = new DataSet();
            SqlConnection Connection = null;
            //string ConnectionString = "Data Source=(local);Initial Catalog=EmptyDB4;Integrated Security=True";
            string ConnectionString = "context connection=true";

            try
            {
                int nRetryCount = 1;
                do
                {
                    try
                    {
                        SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

                        Connection = new SqlConnection(connectionBuilder.ConnectionString);
                        command.Connection = Connection;
                        var expirationTime = DateTime.Now.AddSeconds(command.CommandTimeout);
                        Connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            string tableNameSuffix = "";
                            int tableNumber = 0;

                            do
                            {
                                var schema = reader.GetSchemaTable();


                                if (schema != null && schema.Rows.Count > 0)
                                {
                                    var table = new DataTable();
                                    table.TableName = "TableName" + tableNameSuffix;
                                    tableNumber++;
                                    tableNameSuffix = tableNumber.ToString();
                                    ResultDataSet.Tables.Add(table);

                                    foreach (DataRow schemaRow in schema.Rows)
                                    {
                                        var column = new DataColumn
                                        {
                                            ColumnName = schemaRow["ColumnName"] as string,
                                            DataType = schemaRow["DataType"] as Type,
                                            AllowDBNull = (bool)schemaRow["AllowDBNull"]
                                        };

                                        if (column.DataType == typeof(string))
                                        {
                                            column.MaxLength = (int)schemaRow["ColumnSize"];
                                        }

                                        string columnNameSuffix = "";
                                        int columnNameSuffixInt = 1;

                                        while (table.Columns.Contains(column.ColumnName + columnNameSuffix))
                                        {
                                            columnNameSuffix = columnNameSuffixInt.ToString();
                                            columnNameSuffixInt++;
                                        }

                                        column.ColumnName += columnNameSuffix;
                                        table.Columns.Add(column);
                                    }

                                    if (command.CommandTimeout != 0 && DateTime.Now > expirationTime)
                                    {
                                        throw new Exception("Operation Timed Out");
                                    }

                                    var objects = new Object[table.Columns.Count];

                                    while (reader.Read())
                                    {
                                        var row = table.NewRow();
                                        ((IDataRecord)reader).GetValues(objects);
                                        row.ItemArray = objects;
                                        table.Rows.Add(row);

                                        if (command.CommandTimeout != 0 && DateTime.Now > expirationTime)
                                        {
                                            throw new Exception("Operation Timed Out");
                                        }
                                    }
                                }
                            } while (reader.NextResult());
                        }

                        return ResultDataSet;
                    }
                    catch (SqlException exception)
                    {
                        // Transport error most likely due to SQL Server failover
                        // Clear out pools if the following error occurs: "New request is not allowed to 
                        // start because it should come with valid transaction descriptor" (3989).
                        if ((exception.Number == 10054) || (exception.Number == 3989))
                        {
                            SqlConnection.ClearAllPools();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        if (Connection != null && Connection.State != ConnectionState.Closed)
                        {
                            Connection.Close();
                        }
                    }
                }
                while (--nRetryCount > 0);
            }
            catch (SqlException se)
            {
                throw se;
            }
            catch (Exception e)
            {
                throw e;
            }

            return ResultDataSet;
        }

        internal SortedDictionary<string, Dictionary<Guid, TagData>> GetAndSetDictionaryBasedOnAccessRights(SortedDictionary<string, Dictionary<Guid, TagData>> pointIDToTagData,
                                                                                                            System.Data.SqlTypes.SqlGuid SiteGuid, System.Data.SqlTypes.SqlGuid UserGuid)
        {
            SortedDictionary<string, Dictionary<Guid, TagData>> pointIDToTagDataDictionaryAccessReturn = new SortedDictionary<string, Dictionary<Guid, TagData>>();
            List<PointValueIdentifier> pointValueIdentifierList = new List<PointValueIdentifier>();
            PointValueIdentifier pointValueIdentifier = null;
            Dictionary<PointValueIdentifier, PointValueAccess> pointAccesList = new Dictionary<PointValueIdentifier, PointValueAccess>();

            foreach (string pointID in pointIDToTagData.Keys)
            {
                Dictionary<Guid, TagData> tags = pointIDToTagData[pointID];
                foreach (TagData tagData in tags.Values)
                {
                    switch (tagData.ID)
                    {
                        case "Level Product":
                        case "Temperature Product":
                        case "Volume Gross Observed":
                        case "Volume Net Standard":
                        case "Volume Net Standard Remaining":
                        case "Volume Net Standard Available":
                        case "Volume Water":
                        case "Level Water":
                        case "Density Product Standard":
                        case "Volume Correction Factor":
                     {
                        pointValueIdentifier = new PointValueIdentifier();
                                pointValueIdentifier.IdentityGuid = tagData.PointTagGuid;
                                pointValueIdentifier.PropertyID = tagData.PropertyID;
                                pointValueIdentifier.PointValueType = PointValueType.Tag;
                                pointValueIdentifierList.Add(pointValueIdentifier);
                                break;
                            }
                        case "ProductID":
                            {
                                pointValueIdentifier = new PointValueIdentifier();
                                pointValueIdentifier.IdentityGuid = tagData.PointTagGuid;
                                pointValueIdentifier.PropertyID = tagData.PropertyID;
                                pointValueIdentifier.PointValueType = PointValueType.Point;
                                pointValueIdentifierList.Add(pointValueIdentifier);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }

            pointAccesList = EnumerateRestrictedAccessByPointValueIdenfierList(SiteGuid, UserGuid, pointValueIdentifierList);

            if (pointAccesList == null)
            {
                return pointIDToTagDataDictionaryAccessReturn;
            }
            else if (pointAccesList.Count == 0)
            {
                pointIDToTagDataDictionaryAccessReturn = pointIDToTagData;
            }
            else
            {
                // go through the list and set any Restricted variable to "Restricted"
                pointIDToTagDataDictionaryAccessReturn = pointIDToTagData;
                foreach (var pointAccess in pointAccesList.Keys)
                {
                    foreach (string pointID in pointIDToTagData.Keys)
                    {
                        Dictionary<Guid, TagData> tags = pointIDToTagDataDictionaryAccessReturn[pointID];
                        foreach (TagData tagData in tags.Values)
                        {
                            if (tagData.PointTagGuid == pointAccess.IdentityGuid &&
                                tagData.PropertyID == pointAccess.PropertyID)
                            {
                                // we only care anout view
                                if (pointAccesList[pointAccess].View == false)
                                {
                                    tagData.QualityString = "Restricted";
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return pointIDToTagDataDictionaryAccessReturn;
        }

      /*
       *  
       *  cuttoffDateTime must be in UTC time zone
       * 
       */
        internal void QueryCassandraForArchiveRecords(CqlConnection connection, DateTime cuttoffDateTime, int rangeDuration, bool useSmallFieldNames, bool useDateOnly)
        {

            DateTime cuttoffDateTimeBeginDate = cuttoffDateTime - new TimeSpan(0, 0, 0, rangeDuration, 0);
            int yearMonth = cuttoffDateTime.Year * 100 + cuttoffDateTime.Month;

            if (useDateOnly == true)
            {
                cuttoffDateTime = new DateTime(cuttoffDateTime.Year, cuttoffDateTime.Month, cuttoffDateTime.Day, 23, 59, 59, 999);
            }

            string QueryFormat = (useSmallFieldNames) ?
                "SELECT a,f,d,o,b,h FROM \"FMArchive_Data\".valuearchivedata WHERE a = {0} AND b = '{1}' AND c = {2} AND f > '{3}' and f <= '{4}' ORDER BY f DESC LIMIT 1" :
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
                        var part = new CurrentTankInventoryPartitionKeyClass() { A = tagData.PointTagGuid, B = propertyid, C = yearMonth };
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

        internal string getPropertyid(string tagID)
        {
            string returnValue = string.Empty;

            if (tagID == "ProductID")
                returnValue = tagID;

            return returnValue;
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
                    archiveData.DataType = (string)reader[5];

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
                                t.DataType = archiveData.DataType;
                            }
                        }
                    }
                }
            }
        }
    }
}