using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	#region Public enumeration
	public enum PIDXType
	{
		Tds = 0,
		Dtn = 1,
		MaxPIDX = 2
	};
	
	public enum PIDXVersion
	{
		OneDotZeroTwo = 0,
		FourDotZeroOne = 1,
		MaxVersion = 2
	};
    #endregion

    #region PIDX Profile Collection Class
    [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(PIDXProfileClass))]
	public class PIDXProfileCollectionClass : CollectionBase
	{
		public void Add(PIDXProfileClass pidxProfile)
		{
		    this.List.Add(pidxProfile);
		}

		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
			    this.List.RemoveAt(index);
			}
		}

		public void Remove(PIDXProfileClass pidxProfile)
		{
			int index = 0;

			foreach (PIDXProfileClass item in this.List)
			{
				if (item.IdentityGuid == pidxProfile.IdentityGuid)
				{
				    this.List.RemoveAt(index);
					return;
				}

				index++;
			}
		}

		public PIDXProfileClass Item(int index)
		{
			return (PIDXProfileClass)this.List[index];
		}

		public PIDXProfileClass Find(Guid profileGuid)
		{
			foreach (PIDXProfileClass pidxProfile in this.List)
			{
				if (pidxProfile.IdentityGuid == profileGuid)
				{
					return pidxProfile;
				}
			}

			return null;
		}
	}
	#endregion

	/// <summary>
	/// Summary description for PIDXProfileClass.
	/// </summary>
	[Serializable()]
	[DataContract]
	public class PIDXProfileClass : BaseDataObject
	{
		#region Private data members
		[DataMember]
		private PIDXType _Type;
		[DataMember]
		private string _IPAddress;
		[DataMember]
		private int _Port;
		[DataMember]
		private string _TerminalID;
		[DataMember]
		private string _UserID;
		[DataMember]
		private string _Password;
		[DataMember]
		private bool _Enabled;
		[DataMember]
		private bool _LoggingEnabled;
		[DataMember]
		private string _LogFilePath;
		[DataMember]
		private PIDXVersion _Version;
		#endregion

		#region Public data members
		[DataMember]
		public PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the PIDX Profile Class.
		/// </summary>
		public PIDXProfileClass()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		public override string ID
		{
			get { return this._ID; }
			set {
			    this.SetString("ID", 30, value, ref this._ID); }
		}

		public PIDXType Type
		{
			get { return this._Type; }
			set {
			    this._Type = value; }
		}

	    // ReSharper disable once InconsistentNaming
		public string IPAddress
		{
			get { return this._IPAddress; }
			set {
			    this.SetString("IP Address", 60, value, ref this._IPAddress); }
		}

		public int Port
		{
			get { return this._Port; }
			set {
			    this._Port = value; }
		}

		public string TerminalID
		{
			get { return this._TerminalID; }
			set {
			    this.SetString("Terminal ID", 30, value, ref this._TerminalID); }
		}

		public string UserID
		{
			get { return this._UserID; }
			set {
			    this.SetString("User ID", 30, value, ref this._UserID); }
		}

		public string Password
		{
			get { return this._Password; }
			set {
			    this.SetString("Password", 30, value, ref this._Password); }
		}

		public bool Enabled
		{
			get { return this._Enabled; }
			set {
			    this._Enabled = value; }
		}

		public bool LoggingEnabled
		{
			get { return this._LoggingEnabled; }
			set {
			    this._LoggingEnabled = value; }
		}

		public string LogFilePath
		{
			get { return this._LogFilePath; }
			set {
			    this.SetString("Log File", 255, value, ref this._LogFilePath); }
		}

	    public PIDXVersion Version
	    {
	        get
	        {
	            return this._Version;
	        }

	        set
	        {
	            this._Version = value;
	        }
	    }

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.PIDX_PROFILE; }
			set { }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

	    #endregion

		#region SqlCommand with Parameters
		
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblPIDXProfiles ( " +
					" SiteGuid, " +
					" ID, " +
					" Type, " +
					" IPAddress, " +
					" Port, " +
					" TerminalID, " +
					" UserID, " +
					" Password, " +
					" Enabled, " +
					" LoggingEnabled, " +
					" LogFilePath, " +
					" CreatedDate, " +
					" CreatedBy, " +
					" UpdatedDate, " +
					" UpdatedBy, " +
					" PIDXProfileGuid, " +
                    " Version " +
					" ) VALUES (" +
					" @SiteGuid, " +
					" @ID, " +
					" @Type, " +
					" @IPAddress, " +
					" @Port, " +
					" @TerminalID, " +
					" @UserID, " +
					" @Password, " +
					" @Enabled, " +
					" @LoggingEnabled, " +
					" @LogFilePath, " +
					" @CreatedDate, " +
					" @CreatedBy, " +
					" @UpdatedDate, " +
					" @UpdatedBy, " +
					" @PIDXProfileGuid, " +
                    " @Version)";

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ID" , SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@Type" , SqlDbType.Int);
				cmd.Parameters.Add("@IPAddress" , SqlDbType.NVarChar, 60);
				cmd.Parameters.Add("@Port" , SqlDbType.Int);
				cmd.Parameters.Add("@TerminalID" , SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@UserID" , SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@Password" , SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@Enabled" , SqlDbType.Bit);
				cmd.Parameters.Add("@LoggingEnabled" , SqlDbType.Bit);
				cmd.Parameters.Add("@LogFilePath" , SqlDbType.NVarChar, 255);
				cmd.Parameters.Add("@CreatedDate" , SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@CreatedBy" , SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@UpdatedDate" , SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@PIDXProfileGuid", SqlDbType.UniqueIdentifier);
		        cmd.Parameters.Add("@Version", SqlDbType.Int);

				cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
				cmd.Parameters["@ID"].Value = this._ID;
				cmd.Parameters["@Type"].Value = this._Type; 
				cmd.Parameters["@IPAddress"].Value = this._IPAddress; 
				cmd.Parameters["@Port"].Value = this._Port; 
				cmd.Parameters["@TerminalID"].Value = this._TerminalID; 
				cmd.Parameters["@UserID"].Value = this._UserID; 
				cmd.Parameters["@Password"].Value = this._Password; 
				cmd.Parameters["@Enabled"].Value = (this._Enabled ? 1 : 0); 
				cmd.Parameters["@LoggingEnabled"].Value = (this._LoggingEnabled ? 1 : 0); 
				cmd.Parameters["@LogFilePath"].Value = this._LogFilePath; 
				cmd.Parameters["@CreatedDate"].Value = this._CreatedDate; 
				cmd.Parameters["@CreatedBy"].Value = this._CreatedBy; 
				cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
				cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
				cmd.Parameters["@PIDXProfileGuid"].Value = this._IdentityGuid;
                cmd.Parameters["@Version"].Value = this.Version;
        }

        public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblPIDXProfiles SET " +
				" ID = @ID, " +
				" Type = @Type, " +
				" IPAddress = @IPAddress, " +
				" Port = @Port, " +
				" TerminalID = @TerminalID, " +
				" UserID = @UserID, " +
				" Password = @Password, " +
				" Enabled = @Enabled, " +
				" LoggingEnabled = @LoggingEnabled, " +
				" LogFilePath = @LogFilePath, " +
				" UpdatedDate = @UpdatedDate, " +
				" UpdatedBy = @UpdatedBy, " +
                " Version = @Version " +
				" WHERE PIDXProfileGuid = @PIDXProfileGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@Type", SqlDbType.Int);
			cmd.Parameters.Add("@IPAddress", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@Port", SqlDbType.Int);
			cmd.Parameters.Add("@TerminalID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@Enabled", SqlDbType.Bit);
			cmd.Parameters.Add("@LoggingEnabled", SqlDbType.Bit);
			cmd.Parameters.Add("@LogFilePath", SqlDbType.NVarChar, 255);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
		    cmd.Parameters.Add("@Version", SqlDbType.Int);
			cmd.Parameters.Add("@PIDXProfileGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = this._ID;
			cmd.Parameters["@Type"].Value = this._Type;
			cmd.Parameters["@IPAddress"].Value = this._IPAddress;
			cmd.Parameters["@Port"].Value = this._Port;
			cmd.Parameters["@TerminalID"].Value = this._TerminalID;
			cmd.Parameters["@UserID"].Value = this._UserID;
			cmd.Parameters["@Password"].Value = this._Password;
			cmd.Parameters["@Enabled"].Value = (this._Enabled ? 1 : 0);
			cmd.Parameters["@LoggingEnabled"].Value = (this._LoggingEnabled ? 1 : 0);
			cmd.Parameters["@LogFilePath"].Value = this._LogFilePath;
			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
		    cmd.Parameters["@Version"].Value = this.Version;
			cmd.Parameters["@PIDXProfileGuid"].Value = this._IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblPIDXProfiles WHERE PIDXProfileGuid = @PIDXProfileGuid";
			cmd.Parameters.Add("@PIDXProfileGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@PIDXProfileGuid"].Value = this._IdentityGuid;
		}

		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblPIDXProfiles" +
						" WHERE SiteGuid = @SiteGuid" +
						" ORDER BY ID";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblPIDXProfiles " + SQLUpdateLock(bInTransaction) + " WHERE PIDXProfileGuid = @PIDXProfileGuid";
			cmd.Parameters.Add("@PIDXProfileGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@PIDXProfileGuid"].Value = this._IdentityGuid;
		}

		public void SelectByIDSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblPIDXProfiles " + SQLUpdateLock(bInTransaction) +
					" WHERE SiteGuid = @SiteGuid" +
					" AND ID = @ID";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
			cmd.Parameters["@ID"].Value = this._ID;
		}


		#endregion

		#region Public methods
		public static string TypeID(PIDXType type)
		{
			switch (type)
			{
				case PIDXType.Tds:
					return "Toptech Data Services";
				case PIDXType.Dtn:
					return "DTN";
				default:
					return "Undefined";
			}
		}

		public static string VersionID(PIDXVersion version)
		{
			switch(version)
			{
				case PIDXVersion.OneDotZeroTwo:
					return "1.02";
					
				case PIDXVersion.FourDotZeroOne:
					return "4.01";
					
				default:
					return "Undefined";
			}
		}

		public override void Reset()
		{
			base.Reset();
		    this._Type=PIDXType.Tds;
		    this._IPAddress="tdshost.com";
		    this._Port=5008;
		    this._TerminalID="000";
		    this._UserID="";
		    this._Password="";
		    this._Enabled=false;
		    this._LoggingEnabled=false;
		    this._LogFilePath="";
		    this._Version=PIDXVersion.FourDotZeroOne;
		    this.PIDXProfileCompanyMapCollection=new PIDXProfileCompanyMapCollectionClass();
		}

		public void Load(DataSet set)
		{
			if (set == null)
				throw new ArgumentNullException(nameof(set));

		    this.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
				return;

			DataRow row = table.Rows[0];

		    this._IdentityGuid = DataObject.getValue<Guid>(row["PIDXProfileGuid"], Guid.Empty);
		    this._SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
		    this._Type = (PIDXType)DataObject.getValue<byte>(row["Type"], (byte)PIDXType.Tds);
		    this._ID = DataObject.getValue<string>(row["ID"], "");
		    this._IPAddress = DataObject.getValue<string>(row["IPAddress"], "tdshost.com");
		    this._Port = DataObject.getValue<int>(row["Port"], 5008);
		    this._TerminalID = DataObject.getValue<string>(row["TerminalID"], "000");
		    this._UserID = DataObject.getValue<string>(row["UserID"], "");
		    this._Password = DataObject.getValue<string>(row["Password"], "");
		    this._Enabled = DataObject.getValue<bool>(row["Enabled"], false);
		    this._LoggingEnabled = DataObject.getValue<bool>(row["LoggingEnabled"], false);
		    this._LogFilePath = DataObject.getValue<string>(row["LogFilePath"], "");
		    this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
		    this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
		    this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
		    this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
		    this._Version = DataObject.getValue<PIDXVersion>(row["Version"], PIDXVersion.OneDotZeroTwo); 
		}

		//public string SelectSQL(bool bInTransaction)
		//{
		//   string SQL;

		//   SQL = "SELECT * FROM tblPIDXProfiles " + SQLUpdateLock(bInTransaction) + " WHERE PIDXProfileGuid = '" + _IdentityGuid.ToString() + "'";

		//   return SQL;
		//}

		//public string SelectByIDSQL(bool bInTransaction)
		//{
		//   string SQL;

		//   SQL = "SELECT * FROM tblPIDXProfiles " + SQLUpdateLock(bInTransaction) +
		//         " WHERE SiteGuid = '" + _SiteGuid.ToString() + "'" +
		//         " AND ID = N'" + _ID + "'";

		//   return SQL;
		//}
		#endregion
	}
}
