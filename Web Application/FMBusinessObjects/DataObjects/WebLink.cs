namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
	using System.Security;

	/// <summary>
	/// Summary description for WebLinkCollectionClass.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class WebLinkCollectionClass : List<WebLink>
	{
	}

	/// <summary>
	/// Summary description for WebLink.
	/// </summary>
	[Serializable]
	[DataContract]
	[SecuritySafeCritical]
	[KnownType(typeof(WebLinkCollectionClass))]
	public class WebLink : BaseDataObject
	{
		#region Public data members
		public static readonly string ContactUsLinkName = "Contact Us";
		public static readonly string SupportLinkName = "Support";

		public enum WebLinkTypes { ContactLink, SupportLink, None }
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the web link data object.
		/// </summary>
		public WebLink()
		{
			this.Init();
		}

		public WebLink(WebLinkTypes inWebLinkType)
		{
			this.Init();
			this.WebLinkType = inWebLinkType;
		}
		#endregion

		#region Properties
		[DataMember] public string LinkName { get; set; }
		[DataMember] public string LinkAddress { get; set; }
		[DataMember] public string LinkDescription { get; set; }
		[DataMember] public WebLinkTypes WebLinkType { get; set; }
		#endregion

		public void SelectByNameSQL(SqlCommand command, bool inTransaction)
		{
			command.CommandText = "SELECT * FROM tblWebLink " + SQLUpdateLock(inTransaction) 
								  + " WHERE LinkName = @LinkName ";

			var parm = new SqlParameter("@LinkName", SqlDbType.NVarChar, 100) { Value = this.LinkName };
			command.Parameters.Add(parm);
		}

		public void EnumerateSQL(SqlCommand command, bool inTransaction)
		{
			command.CommandText = "SELECT * FROM tblWebLink " + SQLUpdateLock(inTransaction);
		}

		public void GetByKey (SqlCommand command, bool inTransaction)
		{
			command.CommandText = "SELECT * FROM tblWebLink " + SQLUpdateLock(inTransaction)
								  + " WHERE LinkName = @LinkName ";

			var parm = new SqlParameter("@LinkName", SqlDbType.NVarChar, 100)
							{
								Value = this.LinkName
							};

			command.Parameters.Add(parm);
		}

		public void GetByGuid(SqlCommand command, bool inTransaction)
		{
			command.CommandText = "SELECT * FROM tblWebLink " + SQLUpdateLock(inTransaction)
								  + " WHERE WebLinkGuid = @WebLinkGuid ";

			var parm = new SqlParameter("@WebLinkGuid", SqlDbType.UniqueIdentifier)
			{
				Value = this.IdentityGuid
			};

			command.Parameters.Add(parm);
		}

		public void PurgeSQL(SqlCommand command)
		{
			command.CommandText = "DELETE FROM dbo.tblWebLink WHERE WebLinkGuid = @WebLinkGuid";

			var parm = new SqlParameter("@WebLinkGuid", SqlDbType.UniqueIdentifier)
			{
				Value = this.IdentityGuid
			};

			command.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the command with an insert statement.
		/// </summary>
		/// <param name="command">The SQL command to populate.</param>
		public void InsertSQL(SqlCommand command)
		{
			const string InsertSQL = @"INSERT INTO dbo.tblWebLink 
									(
										WebLinkGuid,
										LinkName,
										LinkAddress,
										LinkDescription,
										CreatedDate,
										CreatedBy,
										UpdatedDate,
										Updatedby
									)
									VALUES 
									(
										@WebLinkGuid,
										@LinkName,
										@LinkAddress,
										@LinkDescription,
										@CreatedDate,
										@CreatedBy,
										@UpdatedDate,
										@UpdatedBy
									)";

			command.CommandText = InsertSQL;

			command.Parameters.Add("@WebLinkGuid", SqlDbType.UniqueIdentifier);
			command.Parameters.Add("@LinkName", SqlDbType.NVarChar, 100);
			command.Parameters.Add("@LinkAddress", SqlDbType.NVarChar, 2000);
			command.Parameters.Add("@LinkDescription", SqlDbType.NVarChar, 200);
			command.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 50);
			command.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50);

			this.IdentityGuid = Guid.NewGuid();
			command.Parameters["@WebLinkGuid"].Value		= this.IdentityGuid;
			command.Parameters["@LinkName"].Value			= this.LinkName;
			command.Parameters["@LinkAddress"].Value		= this.LinkAddress;
			command.Parameters["@LinkDescription"].Value	= this.LinkDescription;
			command.Parameters["@CreatedBy"].Value			= this.CreatedBy;
			command.Parameters["@CreatedDate"].Value		= this.CreatedDate;
			command.Parameters["@UpdatedBy"].Value			= this.UpdatedBy;
			command.Parameters["@UpdatedDate"].Value		= this.UpdatedDate;
		}

		/// <summary>
		/// This method will populate the command with a modify statement.
		/// </summary>
		/// <param name="command">The SQL command to populate.</param>
		public void ModifySQL(SqlCommand command)
		{
			const string UpdateSQL = @"UPDATE dbo.tblWebLink SET								
										LinkName = @LinkName,
										LinkAddress = @LinkAddress,
										LinkDescription = @LinkDescription,
										UpdatedDate = @UpdatedDate,
										Updatedby = @UpdatedBy	
									WHERE WebLinkGuid = @WebLinkGuid";

			command.CommandText = UpdateSQL;

			command.Parameters.Add("@WebLinkGuid", SqlDbType.UniqueIdentifier);
			command.Parameters.Add("@LinkName", SqlDbType.NVarChar, 100);
			command.Parameters.Add("@LinkAddress", SqlDbType.NVarChar, 2000);
			command.Parameters.Add("@LinkDescription", SqlDbType.NVarChar, 200);
			command.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50);

			command.Parameters["@WebLinkGuid"].Value		= this.IdentityGuid;
			command.Parameters["@LinkName"].Value			= this.LinkName;
			command.Parameters["@LinkAddress"].Value		= this.LinkAddress;
			command.Parameters["@LinkDescription"].Value	= this.LinkDescription;
			command.Parameters["@UpdatedBy"].Value			= this.UpdatedBy;
			command.Parameters["@UpdatedDate"].Value		= this.UpdatedDate;
		}

		/// <summary>
		/// This method will load a row from the database into the object.
		/// </summary>
		/// <param name="row"></param>
		public void Load(DataRow row)
		{
			if (row == null)
			{
				return;
			}

			this.IdentityGuid		= row.IsNull("WebLinkGuid") ? Guid.Empty : (Guid)row["WebLinkGuid"];
			this.LinkName			= row.IsNull("LinkName") ? string.Empty : (string)row["LinkName"];
			this.LinkAddress		= row.IsNull("LinkAddress") ? string.Empty : (string) row["LinkAddress"];
			this.LinkDescription	= row.IsNull("LinkDescription") ? string.Empty : (string) row["LinkDescription"];
			this.CreatedBy			= row.IsNull("CreatedBy") ? string.Empty : (string) row["CreatedBy"];
			this.UpdatedBy			= row.IsNull("UpdatedBy") ? string.Empty : (string) row["UpdatedBy"];
			this.CreatedDate		= row.IsNull("CreatedDate") ? DateTimeOffset.Now : (DateTimeOffset) row["CreatedDate"];
			this.UpdatedDate		= row.IsNull("UpdatedDate") ? DateTimeOffset.Now : (DateTimeOffset) row["UpdatedDate"];
		}

		#region Private methods
		/// <summary>
		/// This method will set the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Reset();
			this.LinkName			= string.Empty;
			this.LinkAddress		= string.Empty;
			this.LinkDescription	= string.Empty;
			this.WebLinkType		= WebLinkTypes.None;
		}
		#endregion
	}
}
