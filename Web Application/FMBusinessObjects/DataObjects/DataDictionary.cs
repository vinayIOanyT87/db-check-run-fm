namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Runtime.Serialization;
	using System.Xml;

	using FMBusinessObjects.UtilityObjects;
    using System.Collections.Generic;

    /// <summary>
    /// Summary description for CompanyCollectionClass.
    /// </summary>
    [Serializable]
	[CollectionDataContract]
	public class DataDictionaryCollectionClass : DictionaryBase
	{
		public DataDictionaryCollectionClass()
		{
			this.LatestUpdatedDateTime = TimeConverter.MinFMDate;
			this.RowVersion = 0;
			this.DeletedRowVersion = 0;
		}

		public string this[string key]
		{
			get
			{
				if (string.IsNullOrEmpty(key))
				{
					return key;
				}

				string conversionResult = string.Empty;
				char[] componentSeperators = { '[', ']' };
				string[] components = key.Split(componentSeperators);

				foreach (string component in components)
				{
					// skip " ", ",", and ", "
					if (component == " " || component == "," || component == ", ")
					{
						conversionResult += component;
						continue;
					}

					// skip numbers
					double result;
					if (double.TryParse(component, NumberStyles.Any, null, out result))
					{
						conversionResult += component;
						continue;
					}

					string dictionaryValue = (string) this.Dictionary[component];

					if (string.IsNullOrEmpty(dictionaryValue))
					{
						if (component.Length == 0)
						{
							continue;
						}

						// If the Component has a prefix such as Site|ID then return ID
						char[] seperators = { '|' };
						string[] keys = component.Split(seperators);

						if (keys.Length > 1)
						{
							conversionResult += keys[1];
						}
						else
						{
							conversionResult += keys[0];
						}
					}
					else
					{
						conversionResult += dictionaryValue;
					}
				}

				return conversionResult;
			}
			set
			{
				this.Dictionary[key] = value;
			}
		}

		public ICollection Keys
		{
			get { return (this.Dictionary.Keys); }
		}

		public ICollection Values
		{
			get { return (this.Dictionary.Values); }
		}

		public void Add(String key, String value)
		{
			this.Dictionary.Add(key, value);
		}

		public bool Contains(String key)
		{
			return (this.Dictionary.Contains(key));
		}

		public DateTimeOffset LatestUpdatedDateTime { get; set; }

		public long RowVersion { get; set; }

		public long DeletedRowVersion { get; set; }

		public void Add(String key, String value, DateTimeOffset updatedDateTime, long rowVersion)
		{
			// set the latest updated date time
			if (updatedDateTime > this.LatestUpdatedDateTime)
			{
				this.LatestUpdatedDateTime = updatedDateTime;
			}

			// Set the most current row version.
			if(rowVersion > this.RowVersion)
         {
				this.RowVersion = rowVersion;
         }

			if (this.Contains(key) == false)
			{
				this.Dictionary.Add(key, value);
			}
			else
			{
				this.Dictionary[key] = value;
			}
		}

		public void Remove(String key)
		{
			this.Dictionary.Remove(key);
		}
	}

    #region Data Dictionary Class
    /// <summary>
    /// Summary description for DataDictionaryClass.
    /// </summary>
    [DataContract]
	[Serializable]
	public class DataDictionaryClass : BaseDataObject
	{
		#region Data Members
		[DataMember] public string _Key;
		[DataMember] public string _Value;
		[DataMember] public Guid _DataDictionaryGuid;
		[DataMember] private int nMaxLengthOfDataDictionary;
		#endregion

        #region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public DataDictionaryClass()
		{
			this.Init();
		}
		#endregion

		#region Properties

		public override string ID
		{
			get { return this._Key; }
			set { ; }
		}

		public string Key
		{
			get { return this._Key; }
			set
			{
				if (value.Length > 100)
					throw new Exception("[Key] " + value + ", [maximum length of] 100 [exceeded]");

				this._Key = value;
			}
		}

		public string Value
		{
			get { return this._Value; }
			set
			{
				if (value.Length > this.nMaxLengthOfDataDictionary)
				{
					string strExceptionMessage = String.Format("[Value] {0}, [maximum length of] {1} [exceeded]", value, this.nMaxLengthOfDataDictionary);
					throw new Exception(strExceptionMessage);

					//throw new Exception("[Value] "+value+", [maximum length of] 50 [exceeded]");
				}

				this._Value = value;
			}
		}

		public Guid DataDictionaryGuid
		{
			get { return this._DataDictionaryGuid; }
			set { this._DataDictionaryGuid = value; }
		}



		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.DATA_DICTIONARY; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}
        #endregion

      #region Public methods
      public override void Reset()
		{
			this.Init();
		}

		public override void Load(Object o)
		{
			var dataSet = o as DataSet;

			if (dataSet != null)
			{
				this.Reset();

				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				this.SiteGuid				= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
				this.Key						= DataObject.getValue<string>(row["Key"], "");
				this.Value					= DataObject.getValue<string>(row["Value"], "");
				this.CreatedDate			= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this.CreatedBy				= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
				this.UpdatedDate			= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this.CreatedDate);
				this.UpdatedBy				= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
				this.DataDictionaryGuid = DataObject.getValue<Guid>(row["DataDictionaryGuid"], Guid.Empty);
				base.RowVersion			= DataObject.getValue<byte[]>(row["_RowVersion"], null);
			}
			else
			{
				base.Load(o);
			}
		}

		/// <summary>
		/// This method will load the data dictionary based on a data row.
		/// </summary>
		/// <param name="row">Record to load.</param>
		public void Load(DataRow row)
		{
			this.SiteGuid				= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this.Key						= DataObject.getValue<string>(row["Key"], "");
			this.Value					= DataObject.getValue<string>(row["Value"], "");
			this.CreatedDate			= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this.CreatedBy				= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this.UpdatedDate			= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this.CreatedDate);
			this.UpdatedBy				= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			this.DataDictionaryGuid = DataObject.getValue<Guid>(row["DataDictionaryGuid"], Guid.Empty);
			base.RowVersion			= DataObject.getValue<byte[]>(row["_RowVersion"], null);
		}

        public override void Store(Object dataObject)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (dataObject is XmlNode)
			{
				XmlElement dataDictionaryEntryElement = (XmlElement) dataObject;

				if (dataDictionaryEntryElement.Name != "DataDictionaryEntry")
				{
					throw new Exception("Load Error - Invalid Element Name : " + dataDictionaryEntryElement.Name);
				}

				if (dataDictionaryEntryElement.OwnerDocument != null)
				{
					XmlAttribute keyAttribute = dataDictionaryEntryElement.OwnerDocument.CreateAttribute("Key");
					keyAttribute.Value = this.Key;
					dataDictionaryEntryElement.Attributes.Append(keyAttribute);
				}

				if (dataDictionaryEntryElement.OwnerDocument != null)
				{
					XmlAttribute valueAttribute = dataDictionaryEntryElement.OwnerDocument.CreateAttribute("Value");
					valueAttribute.Value = this.Value;
					dataDictionaryEntryElement.Attributes.Append(valueAttribute);
				}

				return;
			}

			throw new Exception("Load Error - Invalid Object Type : " + dataObject.GetType());
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblDataDictionaries " +
				"(SiteGuid," +
				"[Key]," +
				"[Value]," +
				"DataDictionaryGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy" +
				") VALUES (" +
				"@SiteGuid," +
				"@Key," +
				"@Value," +
				"@DataDictionaryGuid, " +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy" +
				")";

			cmd.CommandText = cmd.CommandText + " SELECT _RowVersion FROM tblDataDictionaries WHERE DataDictionaryGuid = @DataDictionaryGuid ";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@Value", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@DataDictionaryGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@SiteGuid"].Value			= this.SiteGuid;
			cmd.Parameters["@Key"].Value				= this.Key;
			cmd.Parameters["@Value"].Value				= this.Value;
			cmd.Parameters["@DataDictionaryGuid"].Value = this.DataDictionaryGuid;
			cmd.Parameters["@CreatedDate"].Value		= this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value			= this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value		= this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value			= this.UpdatedBy;
		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE tblDataDictionaries " +
				"SET SiteGuid = @SiteGuid," +
				"[Value] = @Value," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE SiteGuid = @SiteGuid" +
				" AND [Key] = @Key";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@Value", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@Key"].Value = this.Key;
			cmd.Parameters["@Value"].Value = this.Value;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblDataDictionaries" +
					" WHERE SiteGuid = @SiteGuid" +
					" AND [Key] = @Key";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 100);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@Key"].Value = this.Key;
		}

		public void SelectSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblDataDictionaries" +
				" WHERE SiteGuid = @SiteGuid" +
				" AND [Key] = @Key";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 100);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@Key"].Value = this.Key;
		}

		/// <summary>
		/// This method will populate the SQL command with a SQL to retrieve the data
		/// dictionary based on the row version and site.
		/// </summary>
		/// <param name="cmd"></param>
		public void EnumerateSQLFromRowVersion(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblDataDictionaries" +
					" WHERE ((SiteGuid = @SiteGuid" +
					" OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityDataDictionaryToSite" +
					" WHERE MapToSiteGuid = @SiteGuid))" +
					" AND _RowVersion > @RowVersion)" +
					" ORDER BY [Key]";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@RowVersion", SqlDbType.VarBinary);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@RowVersion"].Value = base.RowVersion;
		}

		/// <summary>
		/// This method will populate the SQL command with a SQL to retrieve the data
		/// dictionary based on the row version and site.
		/// </summary>
		/// <param name="cmd"></param>
		public void EnumerateSQLDeletedFromRowVersion(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT PK_DataDictionaryGuid, _RowVersion as RowVersion FROM track.tblDataDictionaries" +
					" WHERE ((CurrentSiteGuid = @SiteGuid" +
					" OR CurrentSiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityDataDictionaryToSite" +
					" WHERE MapToSiteGuid = @SiteGuid))" +
					" AND DeletedDate IS NOT NULL" +
					" AND DeletedContext IS NOT NULL" +
					" AND _RowVersion > @RowVersion)" +
					" ORDER BY _RowVersion";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@RowVersion", SqlDbType.VarBinary);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@RowVersion"].Value = base.RowVersion;
		}


		public void EnumerateSQLFromUpdatedDateTime(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblDataDictionaries" +
					" WHERE ((SiteGuid = @SiteGuid" +
					" OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityDataDictionaryToSite" +
					" WHERE MapToSiteGuid = @SiteGuid))" +
					" AND UpdatedDate > @UpdatedDate)" +
					" ORDER BY [Key]";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
		}

		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblDataDictionaries" +
					" WHERE (SiteGuid = @SiteGuid" +
					" OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityDataDictionaryToSite" +
					" WHERE MapToSiteGuid = @SiteGuid))" +
					" ORDER BY [Key]";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		public void EnumerateBySiteSQL(SqlCommand cmd)
		{

			cmd.CommandText = "SELECT * FROM tblDataDictionaries" +
				" WHERE SiteGuid = @SiteGuid" +
				" ORDER BY [Key]";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}


        /// <summary>
        /// This method will created a SQL command to import the data dictionary records.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="cmd">The SQL Command to be populated</param>
        /// <param name="addList">The list of add items.</param>
        /// <param name="modList">The list of modify items.</param>
        /// <param name="delList">The list of delete items.</param>
        public void ImportDataSql(SecurityClass security, SqlCommand cmd, List<DataDictionaryClass> addList, List<DataDictionaryClass> modList, List<DataDictionaryClass> delList)
        {
            DataTable addTable = this.CreateImportTable(security, addList);
            DataTable modTable = this.CreateImportTable(security, modList);
            DataTable delTable = this.CreateImportTable(security, delList);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_DataDictionaryImport";

            SqlParameter tableValuedParameter = cmd.Parameters.Add("@AddListTempTable", SqlDbType.Structured);
            tableValuedParameter.Value = addTable;
            tableValuedParameter.TypeName = "dbo.DataDictionaryDataType";

            tableValuedParameter = cmd.Parameters.Add("@ModListTempTable", SqlDbType.Structured);
            tableValuedParameter.Value = modTable;
            tableValuedParameter.TypeName = "dbo.DataDictionaryDataType";

            tableValuedParameter = cmd.Parameters.Add("@DelListTempTable", SqlDbType.Structured);
            tableValuedParameter.Value = delTable;
            tableValuedParameter.TypeName = "dbo.DataDictionaryDataType";

            var parm = new SqlParameter("@ImportSiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
            cmd.Parameters.Add(parm);
        }

		public static long DataDictionaryRowVersion(byte[] rowVersion)
		{
			if (rowVersion == null)
			{
				return 0;
			}

			// SQL Server returns the bytes in a reverse order than the bit converter is 
			// looking for. Therefore, swap the bytes.
			var swappedBytes = new byte[8];

			if (rowVersion.Length == 8)
			{
				swappedBytes[0] = rowVersion[7];
				swappedBytes[1] = rowVersion[6];
				swappedBytes[2] = rowVersion[5];
				swappedBytes[3] = rowVersion[4];
				swappedBytes[4] = rowVersion[3];
				swappedBytes[5] = rowVersion[2];
				swappedBytes[6] = rowVersion[1];
				swappedBytes[7] = rowVersion[0];
			}
			else if (rowVersion.Length == 4)
			{
				swappedBytes[0] = rowVersion[3];
				swappedBytes[1] = rowVersion[2];
				swappedBytes[2] = rowVersion[1];
				swappedBytes[3] = rowVersion[0];
				swappedBytes[4] = 0;
				swappedBytes[5] = 0;
				swappedBytes[6] = 0;
				swappedBytes[7] = 0;
			}
			else
			{
				return 0;
			}

			return BitConverter.ToInt64(swappedBytes, 0);
		}

		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			base.Reset();
			this.Key = string.Empty;
			this.Value = string.Empty;
			this.nMaxLengthOfDataDictionary = 100;
		}

		/// <summary>
		/// This method will create a Data Table that contains the data dictionary records that will be
		/// either inserted, modified, or deleted.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="itemList">The items to build the table.</param>
		/// <returns>Return a data table.</returns>
		private DataTable CreateImportTable(SecurityClass security, List<DataDictionaryClass> itemList)
        {
            var table = new DataTable();
            table.Columns.Add("[Key]", typeof(string));
            table.Columns.Add("[Value]", typeof(string));
            table.Columns.Add("SiteGuid", typeof(Guid));
            table.Columns.Add("CreatedDate", typeof(DateTimeOffset));
            table.Columns.Add("CreatedBy", typeof(string));
            table.Columns.Add("UpdatedDate", typeof(DateTimeOffset));
            table.Columns.Add("UpdatedBy", typeof(string));

            foreach (var dictionary in itemList)
            {
                var row = table.NewRow();

                row["[Key]"]		= dictionary.Key;
                row["[Value]"]		= dictionary.Value;
                row["SiteGuid"]		= dictionary.SiteGuid;
                row["CreatedDate"]	= dictionary.CreatedDate;
                row["CreatedBy"]	= security.UserID;
                row["UpdatedDate"]	= dictionary.CreatedDate;
                row["UpdatedBy"]	= security.UserID;

				table.Rows.Add(row);
            }

            return table;
        }
        #endregion
    }
    #endregion
}
