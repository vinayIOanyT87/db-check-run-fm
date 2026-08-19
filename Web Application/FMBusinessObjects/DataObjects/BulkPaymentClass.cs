using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace FMBusinessObjects.DataObjects
{
	#region Collection Class
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(BulkPaymentClass))]
	public class BulkPaymentCollectionClass : CollectionBase
	{
		public void Add(BulkPaymentClass a_payment)
		{
			List.Add(a_payment);
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

		public void Remove(BulkPaymentClass a_payment)
		{
			int index = 0;
			foreach (BulkPaymentClass Item in List)
			{
				if (Item.IdentityGuid == a_payment.IdentityGuid)
				{
					List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public BulkPaymentClass Item(int Index)
		{
			return (BulkPaymentClass)List[Index];
		}
	}
	#endregion

	[DataContract]
   [Serializable]
	public class BulkPaymentFilter
	{
		#region Properties
		[DataMember]
		public string InvoiceNumber { get; set; }
		[DataMember]
		public string AccountCode { get; set; }
		[DataMember]
		public string FuelType { get; set; }
		[DataMember]
		public string EnteredBy { get; set; }
		[DataMember]
		public int InvoiceQuery { get; set; }
		[DataMember]
		public string PaymentID { get; set; }
		[DataMember]
		public string Supplier { get; set; }
		[DataMember]
		public DateTimeOffset StartDate { get; set; }
		[DataMember]
		public DateTimeOffset EndDate { get; set; }
		#endregion // Properties

		#region Construction
		public BulkPaymentFilter()
		{
			this.InvoiceNumber = "";
			this.AccountCode = "";
			this.FuelType = "";
			this.EnteredBy = "";
			this.PaymentID = "";
			this.Supplier = "";
			this.InvoiceQuery = 0;
			this.StartDate = DateTimeOffset.Now;
			this.EndDate = DateTimeOffset.Now;
		}
		#endregion // Construction
	}

	[DataContract]
   [Serializable]
	public class BulkPaymentClass : BaseDataObject
	{
		#region Properties

		public Guid BulkPaymentID
		{
			get { return base.IdentityGuid; }
			set { base.IdentityGuid = value; }
		}

		[DataMember]
		public string Section { get; set; }
		[DataMember]
		public string PaymentType { get; set; }
		[DataMember]
		public double ForeignRate { get; set; }
		[DataMember]
		public string ForeignUnit { get; set; }
		[DataMember]
		public string RomanNumber { get; set; }
		[DataMember]
		public DateTimeOffset RomanNumberDate { get; set; }
		[DataMember]
		public double DiscountRate { get; set; }
		[DataMember]
		public DateTimeOffset PaymentDueDate { get; set; }
		[DataMember]
		public DateTimeOffset TransactionDate { get; set; }
		[DataMember]
		public string Supplier { get; set; }
		[DataMember]
		public BulkPaymentInvoiceMappingCollectionClass Mapping { get; set; }
		#endregion Properties

		#region Construction
		public BulkPaymentClass()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.BulkPaymentID = Guid.Empty;
			this.Section = "";
			this.PaymentType = "";
			this.ForeignRate = 0.0f;
			this.ForeignUnit = "AUD";
			this.RomanNumber = "";
			this.DiscountRate = 0.0f;
			this.PaymentDueDate = DateTimeOffset.Now;
			this.TransactionDate = DateTimeOffset.Now;
			this.CreatedBy = "";
			this.CreatedDate = DateTimeOffset.Now;
			this.UpdatedBy = "";
			this.UpdatedDate = DateTimeOffset.Now;
			this.Supplier = "";
			this.Mapping = new BulkPaymentInvoiceMappingCollectionClass();
			this.RomanNumberDate = DateTimeOffset.MaxValue;
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
		}
		#endregion // Construction

		#region Overrides
		#endregion // Overrides

		#region Database Interactions
		public void Load(DataRow a_row)
		{
			if (null == a_row)
			{
				throw new ArgumentNullException(MethodBase.GetCurrentMethod().ToString());
			}

			this.BulkPaymentID = DataObject.getValue<Guid>(a_row["BulkPaymentGuid"], Guid.Empty);
			this.SiteID = DataObject.getValue<string>(a_row["Site"], "");
			this.Section = DataObject.getValue<string>(a_row["Section"], "");
			this.PaymentType = DataObject.getValue<string>(a_row["PaymentType"], "");
			this.ForeignRate = DataObject.getValue<double>(a_row["ForeignRate"], 0.0);
			this.ForeignUnit = DataObject.getValue<string>(a_row["ForeignUnit"], "");
			this.RomanNumber = DataObject.getValue<string>(a_row["RomanNumber"], "");
			this.DiscountRate = DataObject.getValue<double>(a_row["DiscountRate"], 0.0);
			this.PaymentDueDate = DataObject.getValue<DateTimeOffset>(a_row["PaymentDueDate"], DateTimeOffset.Now);
			this.TransactionDate = DataObject.getValue<DateTimeOffset>(a_row["TransactionDate"], DateTimeOffset.Now);
			this.Supplier = DataObject.getValue<string>(a_row["Supplier"], "");
			this.RomanNumberDate = DataObject.getValue<DateTimeOffset>(a_row["RomanNumberDate"], DateTimeOffset.MaxValue);

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
			cmd.CommandText = "SELECT * FROM tblBulkPayments";
		}

		static public void EnumerateSQLByFilter(SqlCommand cmd, BulkPaymentFilter a_filter)
		{
			string sql = "SELECT * FROM tblBulkPayments WHERE TransactionDate BETWEEN @StartDate AND @EndDate ";

			if (a_filter.PaymentID.Length > 0)
			{
				sql += "AND BulkPaymentGuid = @BulkPaymentGuid";
				cmd.Parameters.Add("@BulkPaymentGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@BulkPaymentGuid"].Value = a_filter.PaymentID;
			}

			cmd.CommandText = sql;

			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@StartDate"].Value = a_filter.StartDate;
			cmd.Parameters["@EndDate"].Value = a_filter.EndDate;
		}

		static public void SelectByID(SqlCommand cmd, Guid a_Guid)
		{

			cmd.CommandText = "SELECT * FROM tblBulkPayment WHERE BulkPaymentGuid = @BulkPaymentGuid";

			cmd.Parameters.Add("@BulkPaymentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@BulkPaymentGuid"].Value = a_Guid;
		}

		static public void GetNextSquenceID(SqlCommand cmd)
		{
			// TODO: Convert to using GUIDs instead of the obsolete tblAccountingSequences table.
			cmd.CommandText = "SELECT SequenceValue AS seq FROM tblAccountingSequences Where SequenceName = 'BulkPaymentID'";
		}

		static public void IncrementSequenceID(SqlCommand cmd)
		{
			// TODO: Convert to using GUIDs instead of the obsolete tblAccountingSequences table.
			cmd.CommandText = "UPDATE tblAccountingSequences SET SequenceValue = " +
					"(SELECT SequenceValue FROM tblAccountingSequences WHERE SequenceName = 'BulkPaymentID') + 1 " +
					"WHERE SequenceName = 'BulkPaymentID'";
		}

		public void InsertSQL(SqlCommand cmd)
		{
			string sql = "INSERT INTO tblBulkPayments (" +
				"BulkPaymentGuid, " +
				"[Site], " +
				"Section, " +
				"PaymentType, " +
				"ForeignRate, " +
				"ForeignUnit, " +
				"RomanNumber, " +
				"DiscountRate, " +
				"PaymentDueDate, " +
				"TransactionDate, " +
				"Supplier, " +
				"CreatedBy, " +
				"CreatedDate, " +
				"UpdatedBy, " +
				"UpdatedDate ";

			if (this.RomanNumber.Length > 0)
			{
				sql += ", RomanNumberDate";
			}

			sql += ")";

			sql += "VALUES " +
				"(" +
				"@BulkPaymentGuid," +
				"@SiteID," +
				"@Section," +
				"@PaymentType," +
				"@ForeignRate," +
				"@ForeignUnit," +
				"@RomanNumber," +
				"@DiscountRate," +
				"@PaymentDate," +
				"@TransacDate," +
				"@Supplier," +
				"@CreatedBy," +
				"@CreatedDate," +
				"@UpdatedBy," +
				"@UpdatedDate";

			if (this.RomanNumber.Length > 0)
			{
				sql += ", @RomanDate";
			}

			sql += ")";

			cmd.Parameters.Add("@BulkPaymentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteID", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@Section", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@PaymentType", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@ForeignRate", SqlDbType.Float);
			cmd.Parameters.Add("@ForeignUnit", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@RomanNumber", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@DiscountRate", SqlDbType.Float);
			cmd.Parameters.Add("@PaymentDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@TransacDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Supplier", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@BulkPaymentGuid"].Value = BulkPaymentID;
			cmd.Parameters["@SiteID"].Value = SiteID;
			cmd.Parameters["@Section"].Value = Section;
			cmd.Parameters["@PaymentType"].Value = PaymentType;
			cmd.Parameters["@ForeignRate"].Value = ForeignRate;
			cmd.Parameters["@ForeignUnit"].Value = ForeignUnit;
			cmd.Parameters["@RomanNumber"].Value = RomanNumber;
			cmd.Parameters["@DiscountRate"].Value = DiscountRate;
			cmd.Parameters["@PaymentDate"].Value = PaymentDueDate;
			cmd.Parameters["@TransacDate"].Value = TransactionDate;
			cmd.Parameters["@Supplier"].Value = Supplier;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;

			if (this.RomanNumber.Length > 0)
			{
				cmd.Parameters.Add("@RomanDate", SqlDbType.DateTimeOffset);
				cmd.Parameters["@RomanDate"].Value = RomanNumberDate;
			}
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			string sql = "UPDATE tblBulkPayments SET " +
				"[Site] = @SiteID," +
				"Section = @Section," +
				"PaymentType = @PaymentType," +
				"ForeignRate = @ForeignRate," +
				"ForeignUnit = @ForeignUnit," +
				"RomanNumber = @RomanNumber," +
				"DiscountRate = @DiscountRate," +
				"PaymentDueDate = @PaymentDate," +
				"TransactionDate = @TransacDate," +
				"Supplier = @Supplier," +
				"CreatedBy = @CreatedBy," +
				"CreatedDate = @CreatedDate," +
				"UpdatedBy = @UpdatedBy," +
				"UpdatedDate = @UpdatedDate";

			if (this.RomanNumber.Length > 0)
			{
				sql += ", RomanNumberDate = @RomanDate";
			}

			sql += " WHERE BulkPaymentGuid = @BulkPaymentGuid";

			if (this.RomanNumber.Length > 0)
			{
				cmd.Parameters.Add("@RomanDate", SqlDbType.DateTimeOffset);
				cmd.Parameters["@RomanDate"].Value = RomanNumberDate;
			}

			cmd.Parameters.Add("@BulkPaymentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteID", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@Section", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@PaymentType", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@ForeignRate", SqlDbType.Float);
			cmd.Parameters.Add("@ForeignUnit", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@RomanNumber", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@DiscountRate", SqlDbType.Float);
			cmd.Parameters.Add("@PaymentDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@TransacDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Supplier", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@BulkPaymentID"].Value = BulkPaymentID;
			cmd.Parameters["@SiteID"].Value = SiteID;
			cmd.Parameters["@Section"].Value = Section;
			cmd.Parameters["@PaymentType"].Value = PaymentType;
			cmd.Parameters["@ForeignRate"].Value = ForeignRate;
			cmd.Parameters["@ForeignUnit"].Value = ForeignUnit;
			cmd.Parameters["@RomanNumber"].Value = RomanNumber;
			cmd.Parameters["@DiscountRate"].Value = DiscountRate;
			cmd.Parameters["@PaymentDate"].Value = PaymentDueDate;
			cmd.Parameters["@TransacDate"].Value = TransactionDate;
			cmd.Parameters["@Supplier"].Value = Supplier;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblBulkPayments WHERE BulkPaymentID = @BulkPaymentID";

			cmd.Parameters.Add("@BulkPaymentID", SqlDbType.Int);
			cmd.Parameters["@BulkPaymentID"].Value = BulkPaymentID;
		}

		#endregion // Enumerators

		#endregion // Database Interactions
	}
}
