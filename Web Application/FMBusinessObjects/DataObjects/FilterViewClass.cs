using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Reflection;

namespace FMBusinessObjects.DataObjects
{
	public class FilterViewsCollectionClass : List<FilterViewClass> { }

	[DataContract]
   [Serializable]
	[KnownType(typeof(GregorianCalendar))]
	public class FilterViewClass : BaseDataObject
	{
		#region Constants

		public enum FilterFields : int
		{
			NONE = 0, // placeholder only
			MANAGER = 1,
			OWNER = 2,
			SUPPLIER = 3,
			PONUMBER = 4,
			SHIPTO = 5,
			BILLTO = 6,
			DOCUMENTNUMBER = 7,
			PRODUCT = 8
		}
		#endregion // Constants

		#region Properties
		[DataMember]
		public Guid FilterViewGuid
		{
			get
			{
				return base.IdentityGuid;
			}
			set
			{
				base.IdentityGuid = value;
			}
		}
		[DataMember]
		public TransactionTypes TransTypeID { get; set; }
		[DataMember]
		public FilterFields FilterFieldID { get; set; }
		#endregion // Properties

		#region Constructors
		public FilterViewClass()
			: base()
		{
			this.Reset();
		}
		#endregion // Constructors

		#region Overrides
		public override void Reset()
		{
			base.Reset();

			this.FilterViewGuid = Guid.Empty;
			this.TransTypeID = TransactionTypes.T_Maximum;
			this.FilterFieldID = FilterFields.NONE;
		}

		#endregion // Overrides

		#region Database Interactions
		public void Load(DataRow a_row)
		{
			if (null == a_row)
			{
				throw new ArgumentNullException(MethodBase.GetCurrentMethod().ToString());
			}

			this.FilterViewGuid = DataObject.getValue<Guid>(a_row["FilterViewGuid"], Guid.Empty);
			this.TransTypeID = DataObject.getValue<TransactionTypes>(a_row["LookupTransTypeIndex"], TransactionTypes.T_Maximum);
			this.FilterFieldID = DataObject.getValue<FilterFields>(a_row["LookupFilterFieldIndex"], FilterFields.NONE);
			base.CreatedBy = DataObject.getValue<string>(a_row["CreatedBy"], ADMIN);
			base.CreatedDate = DataObject.getValue<DateTimeOffset>(a_row["CreatedDate"], DateTimeOffset.Now);
			base.UpdatedBy = DataObject.getValue<string>(a_row["UpdatedBy"], ADMIN);
			base.UpdatedDate = DataObject.getValue<DateTimeOffset>(a_row["UpdatedDate"], CreatedDate);
		}

		public void Load(DataSet dataSet)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException(MethodBase.GetCurrentMethod().ToString());
			}

			this.Reset();

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			Load(table.Rows[0]);
		}

		#region Enumerators
		static public void EnumerateSQL(SqlCommand cmd)
		{
			// gets all records
			cmd.CommandText = "SELECT * FROM tblFilterViews";
		}

		static public void EnumerateByTransTypeID(SqlCommand cmd, TransactionTypes a_type)
		{
			cmd.CommandText = "SELECT * FROM tblFilterViews WHERE LookupTransTypeIndex = @LookupTransTypeIndex ";

			cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);

			cmd.Parameters["@LookupTransTypeIndex"].Value = (short)a_type;
		}

		static public void SelectByIdentityGuid(SqlCommand cmd, Guid filterViewGuid)
		{
			cmd.CommandText = "SELECT * FROM tblFilterViews WHERE FilterViewGuid = @FilterViewGuid";
			cmd.Parameters.Add("@FilterViewGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@FilterViewGuid"].Value = filterViewGuid;
		}

		public void InsertSQL(SqlCommand cmd)
		{
			string sql = "INSERT INTO tblFilterViews ";

			// check if we're actually manually manually overriding

			sql += "(" +
					"LookupTransTypeIndex, " +
					"LookupFilterFieldIndex, " +
					"CreatedBy, " +
					"CreatedDate, " +
					"UpdatedBy, " +
					"UpdatedDate " +
					")";

			sql += "VALUES " +
					"(" +
					"@LookupTransTypeIndex," +
					"@LookupFilterFieldIndex," +
					"@CreatedBy," +
					"@CreatedDate," +
					"@UpdatedBy," +
					"@UpdatedDate" +
					")";

			cmd.CommandText = sql;

			cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);
			cmd.Parameters.Add("@LookupFilterFieldIndex", SqlDbType.Int);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@LookupTransTypeIndex"].Value = (short)TransTypeID;
			cmd.Parameters["@LookupFilterFieldIndex"].Value = (int)FilterFieldID;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblFilterViews SET " +
					"LookupTransTypeIndex = @LookupTransTypeIndex," +
					"LookupFilterFieldIndex = @LookupFilterFieldIndex," +
					"CreatedBy = @CreatedBy," +
					"CreatedDate = @CreatedDate," +
					"UpdatedBy = @UpdatedBy," +
					"UpdatedDate = @UpdatedDate" +
					"WHERE FilterViewGuid = @FilterViewGuid";

			cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);
			cmd.Parameters.Add("@LookupFilterFieldIndex", SqlDbType.Int);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@FilterViewGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@LookupTransTypeIndex"].Value = (short)TransTypeID;
			cmd.Parameters["@LookupFilterFieldIndex"].Value = (int)FilterFieldID;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@FilterViewGuid"].Value = FilterViewGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblFilterViews WHERE FilterViewGuid = @FilterViewGuid";
			cmd.Parameters.Add("@FilterViewGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@FilterViewGuid"].Value = FilterViewGuid;
		}
		#endregion // Enumerators

		#endregion // Database Interactions
	}
}
