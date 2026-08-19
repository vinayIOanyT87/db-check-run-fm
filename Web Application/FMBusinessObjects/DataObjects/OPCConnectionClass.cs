using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for OPCConnectionCollectionClass.
	/// </summary>
	[Serializable()]
	public class OPCConnectionCollectionClass : List<OPCConnectionClass> { }

	/// <summary>
	/// Summary description for OPCConnectionClass.
	/// </summary>
	[Serializable()]
	public class OPCConnectionClass : BaseDataObject
	{
		public string _URL;
		public string _ProgID;

		public string URL { get { return _URL; } set { SetString("URL", 100, value, ref _URL); } }
		public string ProgID { get { return _ProgID; } set { SetString("ProgID", 50, value, ref _ProgID); } }

		public override string ID
		{
			get
			{
				return ProgID + " - " + URL;
			}
			set
			{
				base.ID = value;
			}
		}

		public OPCConnectionClass()
		{
			Reset();
		}

		public override void Reset()
		{
			base.Reset();
			URL = "";
			ProgID = "";
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
				throw new ArgumentNullException("Set");

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
				return;

			DataRow Row = Table.Rows[0];

			_IdentityGuid = DataObject.getValue<Guid>(Row["OPCConnectionGuid"], Guid.Empty);
			URL = DataObject.getValue<string>(Row["URL"], "");
			ProgID = DataObject.getValue<string>(Row["ProgID"], "");
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);

		}

		public SqlCommand InsertSQLCmd_
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "INSERT INTO tblOPCConnections (" +
					"URL," +
					"ProgID," +
					"CreatedDate," +
					"CreatedBy" +
					") VALUES (" +
					"@URL," +
					"@ProgID," +
					"@CreatedDate," +
					"@CreatedBy" +
					")";
				cmd.Parameters.AddWithValue("@URL", URL);
				cmd.Parameters.AddWithValue("@ProgID", ProgID);
				cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
				cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
				
				return cmd;
			}
		}

		public SqlCommand PurgeSQLCmd
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "DELETE FROM tblOPCConnections " +
										"WHERE OPCConnectionGuid = @OPCConnectionGuid";
				cmd.Parameters.AddWithValue("@OPCConnectionGuid", _IdentityGuid);

				return cmd;
			}
		}

		public SqlCommand SelectSQLCmd
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "SELECT * FROM tblOPCConnections " +
										"WHERE OPCConnectionGuid = @OPCConnectionGuid";
				cmd.Parameters.AddWithValue("@OPCConnectionGuid", _IdentityGuid);

				return cmd;
			}
		}

		public SqlCommand SelectByIDSQLCmd(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT * FROM tblOPCConnections " + SQLUpdateLock(bInTransaction) +
										" WHERE URL = @URL";
			cmd.Parameters.AddWithValue("@URL", URL);
			return cmd;
		}

		public SqlCommand EnumerateSQLCmd
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "SELECT * FROM tblOPCConnections";
				cmd.Parameters.AddWithValue("@URL", URL);
				return cmd;
			}
		}
	}
}
