using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace FMBusinessObjects.DataObjects
{
	#region Collection Class
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(BulkPaymentInvoiceMappingClass))]
	public class BulkPaymentInvoiceMappingCollectionClass : CollectionBase
	{
		public void Add(BulkPaymentInvoiceMappingClass a_invoice)
		{
			List.Add(a_invoice);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				List.RemoveAt(index);
			}
		}

		public void Remove(BulkPaymentInvoiceMappingClass a_invoice)
		{
			int index = 0;
			foreach (BulkPaymentInvoiceMappingClass Item in List)
			{
				if (Item.IdentityGuid == a_invoice.IdentityGuid)
				{
					List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public BulkPaymentInvoiceMappingClass Item(int Index)
		{
			return (BulkPaymentInvoiceMappingClass)List[Index];
		}
	}
	#endregion

	[DataContract]
   [Serializable]
	public class BulkPaymentInvoiceMappingClass : BaseDataObject
	{
		#region Properties
		[DataMember]
		public Guid LinkID { get; set; }

		public Guid BulkPaymentID
		{
			get { return base.IdentityGuid; }
			set { base.IdentityGuid = value; }
		}

		public string InvoiceTransID
		{
			get { return base.ID; }
			set { base.ID = value; }
		}

		[DataMember]
		public string RebateNumber { get; set; }
		#endregion // Properties

		#region Construction
		public BulkPaymentInvoiceMappingClass()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.LinkID = Guid.Empty;
			this.BulkPaymentID = Guid.Empty;
			this.InvoiceTransID = string.Empty;
			this.RebateNumber = string.Empty;
			this.CreatedBy = string.Empty;
			this.CreatedDate = DateTimeOffset.Now;
			this.UpdatedBy = string.Empty;
			this.UpdatedDate = CreatedDate;
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
		}
		#endregion // Construction

		#region Database Interactions
		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		public void Load(DataRow row)
		{
			if (null == row)
			{
				throw new ArgumentNullException(MethodBase.GetCurrentMethod().ToString());
			}

			this.LinkID = DataObject.getValue(row["BulkPaymentLinkGuid"], Guid.Empty);

			this.BulkPaymentID = DataObject.getValue(row["BulkPaymentGuid"], Guid.Empty);
			this.InvoiceTransID = DataObject.getValue(row["InvoiceTransID"], string.Empty);
			this.RebateNumber = DataObject.getValue(row["RebateNumber"], string.Empty);

			this.CreatedBy = DataObject.getValue(row["CreatedBy"], ADMIN);
			this.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			this.UpdatedBy = DataObject.getValue(row["UpdatedBy"], ADMIN);
			this.UpdatedDate = DataObject.getValue(row["UpdatedDate"], this.CreatedDate);
		}

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// </exception>
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

			this.Load(table.Rows[0]);
		}

		#region Enumerators
		/// <summary>
		/// The enumerate SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		static public void EnumerateSQL(SqlCommand cmd)
		{
			// gets all records
			cmd.CommandText = "SELECT * FROM tblBulkPaymentLinks";
		}

		/// <summary>
		/// The select by invoice trans ID.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="transId">
		/// The trans ID.
		/// </param>
		static public void SelectByInvoiceTransID(SqlCommand cmd, string transId)
		{
			cmd.CommandText = "SELECT * FROM tblBulkPaymentLinks WHERE InvoiceTransID = @InvoiceTransID";
			cmd.Parameters.Add("@InvoiceTransID", SqlDbType.NVarChar, 64);
			cmd.Parameters["@InvoiceTransID"].Value = transId;
		}

		/// <summary>
		/// The select by ID.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="guid">
		/// The GUID.
		/// </param>
		static public void SelectByID(SqlCommand cmd, Guid guid)
		{
			cmd.CommandText += "SELECT * FROM tblBulkPaymentLinks WHERE BulkPaymentID = @BulkPaymentID";
			cmd.Parameters.Add("@BulkPaymentID", SqlDbType.Int);
			cmd.Parameters["@BulkPaymentID"].Value = guid;
		}

		/// <summary>
		/// The insert SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblBulkPaymentLinks "
				+ "(" +
				"BulkPaymentID, " +
				"InvoiceTransID, " +
				"RebateNumber, " +
				"CreatedBy, " +
				"CreatedDate, " +
				"UpdatedBy, " +
				"UpdatedDate " +
				")"
				+ "VALUES " +
				"(" +
				"@BulkPaymentID," +
				"@InvoiceTransID," +
				"@RebateNumber," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy)";

			cmd.Parameters.Add("@BulkPaymentID", SqlDbType.Int);
			cmd.Parameters.Add("@InvoiceTransID", SqlDbType.NVarChar, 64);
			cmd.Parameters.Add("@RebateNumber", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@BulkPaymentID"].Value = BulkPaymentID;
			cmd.Parameters["@InvoiceTransID"].Value = InvoiceTransID;
			cmd.Parameters["@RebateNumber"].Value = RebateNumber;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
		}

		/// <summary>
		/// The update SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblBulkPaymentLinks SET " +
				"BulkPaymentID = @BulkPaymentID," +
				"InvoiceTransID = @InvoiceTransID," +
				"RebateNumber = @RebateNumber," +
				"CreatedBy = @CreatedBy," +
				"CreatedDate = @CreatedDate," +
				"UpdatedBy = @UpdatedBy," +
				"UpdatedDate = @UpdatedDate " +
				"WHERE BulkPaymentLinkGuid = @LinkID";

			cmd.Parameters.Add("@BulkPaymentID", SqlDbType.Int);
			cmd.Parameters.Add("@InvoiceTransID", SqlDbType.NVarChar, 64);
			cmd.Parameters.Add("@RebateNumber", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@LinkID", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@BulkPaymentID"].Value = BulkPaymentID;
			cmd.Parameters["@InvoiceTransID"].Value = InvoiceTransID;
			cmd.Parameters["@RebateNumber"].Value = RebateNumber;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@LinkID"].Value = LinkID;
		}

		/// <summary>
		/// The purge SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblBulkPaymentLinks WHERE BulkPaymentLinkGuid = @LinkID";
			cmd.Parameters.Add("@LinkID", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@LinkID"].Value = this.LinkID;
		}
		#endregion // Enumerators
		#endregion 
	}
}
