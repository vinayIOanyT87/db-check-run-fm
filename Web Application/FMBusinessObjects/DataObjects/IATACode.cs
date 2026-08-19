using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(IATACodeClass))]
	public class IATACodeCollectionClass : List<IATACodeClass> { }

	/// <summary>
	/// Summary description for IATACodeClass.
	/// </summary>
	[DataContract]
	[Serializable]
	[EntityImportExportWorksheetAttribute("IATACODES")]
    public class IATACodeClass : FMBaseDataObjectWithUserData
    {
		#region constants
		private const string ParamNameSiteGuid = "@SiteGuid";
		private const SqlDbType ParamTypeSiteGuid = SqlDbType.UniqueIdentifier;

		private const string ParamNameIataGuid = "@IATAGuid";
		private const SqlDbType ParamTypeIataGuid = SqlDbType.UniqueIdentifier;

		private const string ParamNameIataId = "@IATAID";
		private const string ParamNameIataIdWhere = "@WhereIATAID";
		private const SqlDbType ParamTypeIataId = SqlDbType.NVarChar;
		private const int ParamSizeIataId = 50;

		private const string ParamNameName = "@Name";
		private const SqlDbType ParamTypeName = SqlDbType.NVarChar;
		private const int ParamSizeName = 50;

		private const string ParamNameCountryId = "@CountryID";
		private const SqlDbType ParamTypeCoutnryId = SqlDbType.NVarChar;
		private const int ParamSizeCountryId = 3;

        const string PARAM_NAME_TIMEZONE = "@TimeZone";
        const SqlDbType PARAM_TYPE_TIMEZONE = SqlDbType.NVarChar;
        const int PARAM_SIZE_TIMEZONE = 10;

        const string PARAM_NAME_USERDATA1 = "@UserData1";
        const string PARAM_NAME_USERDATA2 = "@UserData2";
        const string PARAM_NAME_USERDATA3 = "@UserData3";
        const string PARAM_NAME_USERDATA4 = "@UserData4";
        const string PARAM_NAME_USERDATA5 = "@UserData5";
        const string PARAM_NAME_USERDATA6 = "@UserData6";
        const string PARAM_NAME_USERDATA7 = "@UserData7";
        const string PARAM_NAME_USERDATA8 = "@UserData8";
        const SqlDbType PARAM_TYPE_USERDATA = SqlDbType.NVarChar;
        const int PARAM_SIZE_USERDATA = 60;

        private const string ParamNameCreatedDate = "@CreatedDate";
		private const SqlDbType ParamTypeCreatedDate = SqlDbType.DateTimeOffset;

		private const string ParamNameCreatedBy = "@CreatedBy";
		private const SqlDbType ParamTypeCreatedBy = SqlDbType.NVarChar;
		private const int ParamSizeCreatedBy = 100;

		private const string ParamNameUpdatedDate = "@UpdatedDate";
		private const SqlDbType ParamTypeUpdatedDate = SqlDbType.DateTimeOffset;

		private const string ParamNameUpdatedBy = "@UpdatedBy";
		private const SqlDbType ParamTypeUpdatedBy = SqlDbType.NVarChar;
		private const int ParamSizeUpdatedBy = 100;

		private const string ParamNameLatitude = "@Latitude";
		private const SqlDbType ParamTypeLatitude = SqlDbType.Float;

		private const string ParamNameLongitude = "@Longitude";
		private const SqlDbType ParamTypeLongitude = SqlDbType.Float;

		private const string ParamNameZoom = "@Zoom";
		private const SqlDbType ParamTypeZoom = SqlDbType.Int;
		#endregion constants

		#region Private data members
		[DataMember] private string name;
		[DataMember] private string country;
		[DataMember] private double? latitude;
		[DataMember] private double? longitude;
		[DataMember] private int? zoom;
        [DataMember] private string _TimeZone;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public IATACodeClass()
		{
			this.Init();
		}
		#endregion


		#region Properties
		[EntityImportExportAttribute("SITE*", 105, "SITEGUID")]
		new public Guid SiteGuid
		{
			get { return this._SiteGuid; }
			set { this._SiteGuid = value; }
		}

		[EntityImportExportAttribute("IATACODEID*", 110, "ID")]
		public override string ID
		{
			get { return this._ID; }
			set { this.SetString("ID", 50, value, ref this._ID); }
		}

		[EntityImportExportAttribute("NAME", 110, "Name")]
		public string Name
		{
			get { return this.name; }
			set { this.SetString("Name", 200, value, ref this.name); }
		}

		[EntityImportExportAttribute("COUNTRY", 110, "Country")]
		public string Country
		{
			get { return this.country; }
			set { this.SetString("Country", 50, value, ref this.country); }
		}

        [EntityImportExportAttribute("TIMEZONE", 110, "TimeZone")]
        public string TimeZone
        {
            get { return _TimeZone; }
            set { SetString("TimeZone", 50, value, ref _TimeZone); }
        }

        [EntityImportExportAttribute("USERDATA1", 110, "UserData1")]
        public string UserData1
        {
            get { return UserData[0]; }
            set { this.UserData[0] = value; }
        }

        [EntityImportExportAttribute("USERDATA2", 110, "UserData2")]
        public string UserData2
        {
            get { return UserData[1]; }
            set { this.UserData[1] = value; }
        }

        [EntityImportExportAttribute("USERDATA3", 110, "UserData3")]
        public string UserData3
        {
            get { return UserData[2]; }
            set { this.UserData[2] = value; }
        }

        [EntityImportExportAttribute("USERDATA4", 110, "UserData4")]
        public string UserData4
        {
            get { return UserData[3]; }
            set { this.UserData[3] = value; }
        }

        [EntityImportExportAttribute("USERDATA5", 110, "UserData5")]
        public string UserData5
        {
            get { return UserData[4]; }
            set { this.UserData[4] = value; }
        }

        [EntityImportExportAttribute("USERDATA6", 110, "UserData6")]
        public string UserData6
        {
            get { return UserData[5]; }
            set { this.UserData[5] = value; }
        }

        [EntityImportExportAttribute("USERDATA7", 110, "UserData7")]
        public string UserData7
        {
            get { return UserData[6]; }
            set { this.UserData[6] = value; }
        }

        [EntityImportExportAttribute("USERDATA8", 110, "UserData8")]
        public string UserData8
        {
            get { return UserData[7]; }
            set { this.UserData[7] = value; }
        }

        [EntityImportExportAttribute("LATITUDE", 110, "Latitude")]
		public double? Latitude
		{
			get { return this.latitude; }
			set { this.latitude = value; }
		}

		[EntityImportExportAttribute("LONGITUDE", 110, "Longitude")]
		public double? Longitude
		{
			get { return this.longitude; }
			set { this.longitude = value; }
		}

		[EntityImportExportAttribute("ZOOM", 110, "Zoom")]
		public int? Zoom
		{
			get { return this.zoom; }
			set { this.zoom = value; }
		}

		public string LatitudeStr
		{
			get { return this.latitude.ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.latitude = null;
					return;
				}

				double lat;

				if (double.TryParse(value, out lat))
				{
					if (lat < -90 || lat > 90)
					{
						throw new Exception("Latitude value must be between -90.0 and 90.0");
					}

					this.latitude = lat;
				}
				else
				{
					throw new Exception("Latitude must be numeric.");
				}
			}
		}

		public string LongitudeStr
		{
			get { return this.longitude.ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.longitude = null;
					return;
				}

				double lng;

				if (double.TryParse(value, out lng))
				{
					if (lng < -180 || lng > 180)
					{
						throw new Exception("Longitude value must be between -180.0 and 180.0");
					}

					this.longitude = lng;
				}
				else
				{
					throw new Exception("Longitude must be numeric.");
				}
			}
		}

		public string ZoomStr
		{
			get { return this.zoom.ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.zoom = null;
					return;
				}

				int localZoom;

				if (int.TryParse(value, out localZoom))
				{
					if (localZoom < 0 || localZoom > 25)
					{
						throw new Exception("Zoom value must be between 0 and 25");
					}

					this.zoom = localZoom;
				}
				else
				{
					throw new Exception("Zoom must be numeric.");
				}

			}
		}

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.IATA_CODE; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public SqlCommand UpdateSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = 
						"UPDATE tblIATA SET "
						+ DataObject.AddParameter(cmd, false, "SiteGuid", ParamNameSiteGuid, ParamTypeSiteGuid, this._SiteGuid)
						+ ", " + DataObject.AddParameter(cmd, false, "IATAID", ParamNameIataId, ParamTypeIataId, ParamSizeIataId, this.ID) 
						+ ", " + DataObject.AddParameter(cmd, false, "Name", ParamNameName, ParamTypeName, ParamSizeName, this.Name) 
						+ ", " + DataObject.AddParameter(cmd, false, "CountryID", ParamNameCountryId, ParamTypeCoutnryId, ParamSizeCountryId, this.Country) 
						+ ", " + DataObject.AddParameter(cmd, false, "UpdatedDate", ParamNameUpdatedDate, ParamTypeUpdatedDate, this._UpdatedDate)
						+ ", " + DataObject.AddParameter(cmd, false, "UpdatedBy", ParamNameUpdatedBy, ParamTypeUpdatedBy, ParamSizeUpdatedBy, this._UpdatedBy)
                        + ", " + DataObject.AddParameter(cmd, false, "TimeZone", PARAM_NAME_TIMEZONE, PARAM_TYPE_TIMEZONE, PARAM_SIZE_TIMEZONE, TimeZone)
                        + ", " + DataObject.AddParameter(cmd, false, "UserData1", PARAM_NAME_USERDATA1, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData1)
                        + ", " + DataObject.AddParameter(cmd, false, "UserData2", PARAM_NAME_USERDATA2, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData2)
                        + ", " + DataObject.AddParameter(cmd, false, "UserData3", PARAM_NAME_USERDATA3, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData3)
                        + ", " + DataObject.AddParameter(cmd, false, "UserData4", PARAM_NAME_USERDATA4, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData4)
                        + ", " + DataObject.AddParameter(cmd, false, "UserData5", PARAM_NAME_USERDATA5, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData5)
                        + ", " + DataObject.AddParameter(cmd, false, "UserData6", PARAM_NAME_USERDATA6, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData6)
                        + ", " + DataObject.AddParameter(cmd, false, "UserData7", PARAM_NAME_USERDATA7, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData7)
                        + ", " + DataObject.AddParameter(cmd, false, "UserData8", PARAM_NAME_USERDATA8, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData8);

                if (this.latitude != null)
				{
					cmd.CommandText = cmd.CommandText + ", "
									  + DataObject.AddParameter(cmd, false, "Latitude", ParamNameLatitude, ParamTypeLatitude, this.latitude);
				}
				else
				{
					cmd.CommandText = cmd.CommandText + ", "
									  + DataObject.AddParameter(cmd, false, "Latitude", ParamNameLatitude, ParamTypeLatitude, DBNull.Value);
				}

				if (this.longitude != null)
				{
					cmd.CommandText = cmd.CommandText + ", "
									  + DataObject.AddParameter(cmd, false, "Longitude", ParamNameLongitude, ParamTypeLongitude, this.longitude);
				}
				else
				{
					cmd.CommandText = cmd.CommandText + ", "
									  + DataObject.AddParameter(cmd, false, "Longitude", ParamNameLongitude, ParamTypeLongitude, DBNull.Value);
				}

				if (this.zoom != null)
				{
					cmd.CommandText = cmd.CommandText + ", "
									  + DataObject.AddParameter(cmd, false, "Zoom", ParamNameZoom, ParamTypeZoom, this.zoom);
				}
				else
				{
					cmd.CommandText = cmd.CommandText + ", "
									  + DataObject.AddParameter(cmd, false, "Zoom", ParamNameZoom, ParamTypeZoom, DBNull.Value);
				}

				cmd.CommandText = cmd.CommandText + " WHERE "
									+ DataObject.AddParameter(cmd, false, "IATAGuid", ParamNameIataGuid, ParamTypeIataGuid, this._IdentityGuid);


				return cmd;
			}
		}

		public SqlCommand PurgeSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "DELETE FROM tblIATA WHERE " +
										DataObject.AddParameter(cmd, false, "IATAGuid", ParamNameIataGuid, ParamTypeIataGuid, this._IdentityGuid);

				return cmd;
			}
		}
		#endregion

		#region Public and internal methods
		public override void Reset()
		{
			this.Init();
		}

		/// <summary>
		/// This method will populate the SQL Insert Command.
		/// </summary>
		/// <param name="cmd">SQL command to populate.</param>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = 
							"INSERT INTO tblIATA "
							+ "(SiteGuid, "
							+ "IATAID, "
							+ "Name, "
							+ "CountryID, "
							+ "CreatedDate, "
							+ "CreatedBy, "
							+ "UpdatedDate, "
							+ "UpdatedBy, "
							+ "IATAGuid, "
                            + "UserData1, "
                            + "UserData2, "
                            + "UserData3, "
                            + "UserData4, "
                            + "UserData5, "
                            + "UserData6, "
                            + "UserData7, "
                            + "UserData8, "
                            + "TimeZone, "
                            + "Latitude, "
                            + "Longitude, "
                            + "Zoom "
                            + ") VALUES (" 
							+ DataObject.AddGuidParameter(cmd, string.Empty, ParamNameSiteGuid, this._SiteGuid) 
							+ DataObject.AddParameter(cmd, ",", ParamNameIataId, ParamTypeIataId, ParamSizeIataId, this.ID) 
							+ DataObject.AddParameter(cmd, ",", ParamNameName, ParamTypeName, ParamSizeName, this.Name) 
							+ DataObject.AddParameter(cmd, ",", ParamNameCountryId, ParamTypeCoutnryId, ParamSizeCountryId, this.Country) 
							+ DataObject.AddParameter(cmd, ",", ParamNameCreatedDate, ParamTypeCreatedDate, this._CreatedDate) 
							+ DataObject.AddParameter(cmd, ",", ParamNameCreatedBy, ParamTypeCreatedBy, ParamSizeCreatedBy, this._CreatedBy) 
							+ DataObject.AddParameter(cmd, ",", ParamNameUpdatedDate, ParamTypeUpdatedDate, this._UpdatedDate) 
							+ DataObject.AddParameter(cmd, ",", ParamNameUpdatedBy, ParamTypeUpdatedBy, ParamSizeUpdatedBy, this._UpdatedBy)
                            + DataObject.AddParameter(cmd, ",", "@IATAGuid", SqlDbType.UniqueIdentifier, this._IdentityGuid)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_USERDATA1, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData1)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_USERDATA2, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData2)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_USERDATA3, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData3)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_USERDATA4, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData4)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_USERDATA5, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData5)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_USERDATA6, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData6)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_USERDATA7, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData7)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_USERDATA8, PARAM_TYPE_USERDATA, PARAM_SIZE_USERDATA, UserData8)
                            + DataObject.AddParameter(cmd, ",", PARAM_NAME_TIMEZONE, PARAM_TYPE_TIMEZONE, PARAM_SIZE_TIMEZONE, TimeZone);



            if (this.latitude != null)
			{
				cmd.CommandText = cmd.CommandText
				                  + DataObject.AddParameter(cmd, ",", ParamNameLatitude, ParamTypeLatitude, this.latitude);
			}
			else
			{
				cmd.CommandText = cmd.CommandText
								  + DataObject.AddParameter(cmd, ",", ParamNameLatitude, ParamTypeLatitude, DBNull.Value);
			}

			if (this.longitude != null)
			{
				cmd.CommandText += DataObject.AddParameter(cmd, ",", ParamNameLongitude, ParamTypeLongitude, this.longitude);
			}
			else
			{
				cmd.CommandText += DataObject.AddParameter(cmd, ",", ParamNameLongitude, ParamTypeLongitude, DBNull.Value);
			}

			if (this.zoom != null)
			{
				cmd.CommandText += DataObject.AddParameter(cmd, ",", ParamNameZoom, ParamTypeZoom, this.zoom);
			}
			else
			{
				cmd.CommandText += DataObject.AddParameter(cmd, ",", ParamNameZoom, ParamTypeZoom, DBNull.Value);
			}

			cmd.CommandText += ")";
		}

		/// <summary>
		/// This method will load the object from a dataset.
		/// </summary>
		/// <param name="dataSet">The dataset to load.</param>
		public void Load(DataSet dataSet)
		{
			if (dataSet == null || dataSet.Tables.Count == 0)
			{
				throw new ArgumentNullException("dataSet");
			}

			this.Reset();
			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			this._IdentityGuid	= DataObject.getValue<Guid>(row["IATAGuid"], Guid.Empty);
			this._SiteGuid		= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this._ID			= DataObject.getValue<string>(row["IATAID"], "");
			this.Name			= DataObject.getValue<string>(row["Name"], "");
			this.Country		= DataObject.getValue<string>(row["CountryID"], "");
			this._CreatedDate	= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy		= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this._UpdatedDate	= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy		= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			this.latitude		= DataObject.getValue<double?>(row["Latitude"], null);
			this.longitude		= DataObject.getValue<double?>(row["Longitude"], null);
			this.zoom			= DataObject.getValue<int?>(row["Zoom"], null);
            this.TimeZone       = DataObject.getValue<string>(row["TimeZone"], string.Empty);
            this.UserData1      = DataObject.getValue<string>(row["UserData1"], string.Empty);
            this.UserData2      = DataObject.getValue<string>(row["UserData2"], string.Empty);
            this.UserData3      = DataObject.getValue<string>(row["UserData3"], string.Empty);
            this.UserData4      = DataObject.getValue<string>(row["UserData4"], string.Empty);
            this.UserData5      = DataObject.getValue<string>(row["UserData5"], string.Empty);
            this.UserData6      = DataObject.getValue<string>(row["UserData6"], string.Empty);
            this.UserData7      = DataObject.getValue<string>(row["UserData7"], string.Empty);
            this.UserData8      = DataObject.getValue<string>(row["UserData8"], string.Empty);

        }

        public SqlCommand SelectSQL(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT tblIATA.* FROM tblIATA " + SQLUpdateLock(bInTransaction) + " WHERE " +
									DataObject.AddParameter(cmd, false, "IATAGuid", ParamNameIataGuid, ParamTypeIataGuid, this._IdentityGuid);

			return cmd;
		}

		public SqlCommand SelectByIDSQL(SecurityClass security, bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT tblIATA.* FROM tblIATA " + SQLUpdateLock(bInTransaction) +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblIATA", "IATAGuid") +
					DataObject.AddParameter(cmd, true, "IATAID", ParamNameIataIdWhere, ParamTypeIataId, ParamSizeIataId, this.ID);

			return cmd;
		}

		public SqlCommand EnumerateSQL(SecurityClass security, string filterString)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT tblIATA.*" +
					" FROM tblIATA" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblIATA", "IATAGuid") +
                    this.AppendFilterClause(cmd, filterString) +
                    " ORDER BY IATAID";

			return cmd;
        }

		public SqlCommand EnumerateWhereCoordinateSQL(SecurityClass security)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT tblIATA.*" +
					" FROM tblIATA" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblIATA", "IATAGuid") +
					" AND (Latitude IS NOT NULL OR Latitude <> '') AND (Longitude IS NOT NULL OR Longitude <> '') " +
					" ORDER BY IATAID";

			return cmd;
		}

		public SqlCommand EnumerateByPrefixSQL(SecurityClass security, string Prefix)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT tblIATA.*" +
					" FROM tblIATA" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblIATA", "IATAGuid") +
					DataObject.AddParameter(cmd, "AND IATAID LIKE ", ParamNameIataIdWhere, ParamTypeIataId, Prefix + "%") +
					" ORDER BY IATAID";

			return cmd;
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			base.Reset();
			this.Name       = string.Empty;
			this.Country    = string.Empty;
			this.latitude   = null;
			this.longitude  = null;
			this.zoom       = null;
            this.UserData   = new UserDataClass();
            this.UserData1  = string.Empty;
            this.UserData2  = string.Empty;
            this.UserData3  = string.Empty;
            this.UserData4  = string.Empty;
            this.UserData5  = string.Empty;
            this.UserData6  = string.Empty;
            this.UserData7  = string.Empty;
            this.UserData8  = string.Empty;
        }

        private string AppendFilterClause(SqlCommand cmd, string filterString)
        {
            string filterClause = string.Empty;
            if (String.IsNullOrEmpty(filterString) == false && filterString.Length > 0)
            {
                filterClause =
                    " AND (IATAID LIKE @FilterParam OR CountryID LIKE @FilterParam OR NAME LIKE @FilterParam "
                    + " OR UserData1 LIKE @FilterParam OR UserData2 LIKE @FilterParam OR UserData3 LIKE @FilterParam OR UserData4 LIKE @FilterParam "
                    + " OR UserData5 LIKE @FilterParam OR UserData6 LIKE @FilterParam OR UserData7 LIKE @FilterParam OR UserData8 LIKE @FilterParam) ";

                const SqlDbType ParamTypeFilterString = SqlDbType.NVarChar;
                string likeFilter = string.Format("%{0}%", filterString);
                DataObject.AddParameter(cmd, "@FilterParam", ParamTypeFilterString, likeFilter);
            }

            return filterClause;
        }
        #endregion
    }
}
