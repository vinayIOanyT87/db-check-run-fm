using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
    using System.Data;

    [Serializable]
   [DataContract]
	public class TransactionPIDXDO : DataObject
	{
		#region Private data members
		private const string TRANS_PIDX_TBL_NAME = "tblTransactionPIDX";

		[DataMember]
		private string transID;
		[DataMember]
		private Guid transactionPIDXGuid;
		[DataMember]
		private Guid transactionGuid;
		[DataMember]
		private string authorizationNumber;
		[DataMember]
		private Guid pidxProfileGuid;
		[DataMember]
		private Guid companyPersonnelToShipToBillToGuid;
		[DataMember]
		private bool sentFlag;
		[DataMember]
		private DateTimeOffset dateSent;
		[DataMember]
		private string createdBy;
		[DataMember]
		private DateTimeOffset createDate;
		[DataMember]
		private string updatedBy;
		[DataMember]
		private DateTimeOffset updatedDate;
		[DataMember]
		private Guid siteGuid;
		[DataMember]
		private bool brokenBlend;
		[DataMember]
		private int bolVersion;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction PIDX data object.
		/// </summary>
		public TransactionPIDXDO()
		{
			this.Initialize();
		}
		#endregion

		#region Properties

		public Guid TransactionPIDXGuid
		{
			get { return this.transactionPIDXGuid; }
			private set { ; }
		}

		public string TransID
		{
			get { return this.transID; }
			set { this.transID = value; }
		}

		public Guid TransactionGuid
		{
			get { return this.transactionGuid; }
			set { this.transactionGuid = value; }
		}

		public string AuthorizationNumber
		{
			get { return this.authorizationNumber; }
			set { this.authorizationNumber = value; }
		}

		public Guid PIDXProfileGuid
		{
			get { return this.pidxProfileGuid; }
			set { this.pidxProfileGuid = value; }
		}

		public Guid CompanyPersonnelToShipToBillToGuid
		{
			get { return this.companyPersonnelToShipToBillToGuid; }
			set { this.companyPersonnelToShipToBillToGuid = value; }
		}

		public bool SentFlag
		{
			get { return this.sentFlag; }
			set
			{
				this.sentFlag = value;
				this.dateSent = DateTimeOffset.Now;
			}
		}

		public DateTimeOffset DateSent
		{
			get { return this.dateSent; }
			private set { ; }
		}

		public string CreatedBy
		{
			get { return this.createdBy; }
			set
			{
				this.createdBy = value;
				this.createDate = DateTimeOffset.Now;
			}
		}

		public DateTimeOffset CreatedDate
		{
			get { return this.createDate; }
			private set { ; }
		}

		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set
			{
				this.updatedBy = value;
				this.updatedDate = DateTimeOffset.Now;
			}
		}

		public DateTimeOffset UpdatedDate
		{
			get { return this.updatedDate; }
			private set { ; }
		}

		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		public bool BrokenBlend
		{
			get { return this.brokenBlend; }
			set { this.brokenBlend = value; }
		}

		public int BOLVersion
		{
			get { return this.bolVersion; }
			set { this.bolVersion = value; }
		}

		#endregion

		#region public methods

        /// <summary>
        /// This method will return a SQL select command for all the records 
        /// that have not been sent.
        /// </summary>
        /// <returns></returns>
        public void GetNonSentRecordsSqlCmd(SqlCommand cmd)
        {
            cmd.CommandText = "usp_GetUnsentPidxRecordBols";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SiteGuid", this.siteGuid));
            cmd.Parameters.Add(new SqlParameter("@SendTime", DateTimeOffset.Now));
        }

        /// <summary>
        /// This method will return a SQL select statement for all the records 
        /// associated with the TransactionGuid.
        /// </summary>
        /// <returns></returns>
        public void GetTransRecordsSQL(SqlCommand cmd)
		{
		    cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_TransactionPIDXGet";

			cmd.Parameters.AddWithValue("@TransactionGuid", this.transactionGuid);
			cmd.Parameters.AddWithValue("@SiteGuid", this.siteGuid);
		}

		/// <summary>
		/// This method will return an Update SQL statement that updates the
		/// sent flag.
		/// </summary>
		/// <returns></returns>
		public void UpdateSentStatusSQL(SqlCommand cmd)
		{
			this.UpdateSentStatusSQL(cmd, this.transactionPIDXGuid);
		}

		/// <summary>
		/// This method will return an Update SQL statement that updates the
		/// sent flag for a given index.
		/// </summary>
		/// <param name="authorizationIndex"></param>
		/// <returns></returns>
		public void UpdateSentStatusSQL(SqlCommand cmd, Guid transactionPIDXGuid)
		{
			if (transactionPIDXGuid != Guid.Empty)
			{
				cmd.CommandText = "UPDATE " + TransactionPIDXDO.TRANS_PIDX_TBL_NAME + " " +
					  "SET SentFlag = 1, " +
					  "UpdatedBy = @UpdatedBy, " +
					  "UpdatedDate = @UpdatedDate, " +
					  "DateSent = @DateSent " +
					  "WHERE TransactionPIDXGuid = @TransactionPIDXGuid";

				cmd.Parameters.AddWithValue("@UpdatedBy", this.updatedBy);
				cmd.Parameters.AddWithValue("@UpdatedDate", this.updatedDate);
				cmd.Parameters.AddWithValue("@DateSent", this.dateSent);
				cmd.Parameters.AddWithValue("@TransactionPIDXGuid", this.transactionPIDXGuid);
			}
		}

		/// <summary>
		/// This method will return the SQL to delete the current object's information
		/// from the database. Deletion is based on authorization index being set.
		/// </summary>
		/// <returns></returns>
		public void DeletePIDXSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM " + TransactionPIDXDO.TRANS_PIDX_TBL_NAME + " " +
				"WHERE TransactionPIDXGuid = @TransactionPIDXGuid";

			cmd.Parameters.AddWithValue("@TransactionPIDXGuid", this.transactionPIDXGuid);
		}

		/// <summary>
		/// This method will load one row of data into the object.
		/// </summary>
		/// <param name="row"></param>
		public void LoadNonSentRecordsSQL(System.Data.DataRow row)
		{
			if (row == null)
			{
				this.Initialize();
			}
			else
			{
				this.transactionPIDXGuid = DataObject.getValue<Guid>(row["TransactionPIDXGuid"], Guid.Empty);
				this.transactionGuid = DataObject.getValue<Guid>(row["TransactionGuid"], Guid.Empty);
				this.authorizationNumber = DataObject.getValue<string>(row["AuthorizationNumber"], "");
				this.pidxProfileGuid = DataObject.getValue<Guid>(row["PIDXProfileGuid"], Guid.Empty);
				this.companyPersonnelToShipToBillToGuid = DataObject.getValue<Guid>(row["CompanyPersonnelToShipToBillToGuid"], Guid.Empty);
				this.sentFlag = DataObject.getValue<bool>(row["SentFlag"], false);
                this.dateSent = DataObject.getValue<DateTimeOffset>(row["DateSent"], this.DateSent);
				this.createdBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
				this.createDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this.updatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
				this.updatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this.createDate);
				this.brokenBlend = DataObject.getValue<bool>(row["BrokenBlend"], false);
				this.bolVersion = (row.IsNull("BOLVersion")) ? 0 : (int) row["BOLVersion"];
				this.siteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);

				if (row["DateSent"] != System.DBNull.Value)
				{
					this.dateSent = DataObject.getValue<DateTimeOffset>(row["DateSent"], DateTimeOffset.Now);
				}
			}
		}
		#endregion

		#region private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.transactionPIDXGuid = Guid.Empty;
			this.transactionGuid = Guid.Empty;
			this.authorizationNumber = "";
			this.pidxProfileGuid = Guid.Empty;
			this.sentFlag = false;
			this.createdBy = ""; ;
			this.updatedBy = "";
			this.brokenBlend = false;
			this.bolVersion = 0;
		}
		#endregion

		#region Abstract implementation
		public override string getSelectCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion
	}
}
