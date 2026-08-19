// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllersLogToTransactionMapClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Data object representing a tblControllersLogToTransaction record.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for ControllersLogToTransactionCollectionClass.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class ControllersLogToTransactionCollectionClass : List<ControllersLogToTransactionMapClass> { }

	#region ControllersLogToTransactionMapClass
	/// <summary>
	/// Summary description for ControllersLogToTransactionMapClass.
	/// </summary>
	[Serializable()]
	[DataContract]
	public class ControllersLogToTransactionMapClass : BaseDataObject
	{
		private const string SCHEMANAME = "map" + ".";

		#region Private data members
		[DataMember]
		private Guid controllersLogToTransactionGuid;
		[DataMember]
		private Guid controllersLogGuid;
		[DataMember]
		private Guid transactionGuid;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Controllers Log To TransactionMapClass
		/// </summary>
		public ControllersLogToTransactionMapClass()
		{
			this.Reset();
		}
		#endregion

		#region Properties

		/// <summary>
		/// The related TransactionGuid.
		/// </summary>
		public Guid TransactionGuid
		{
			get { return transactionGuid; }
			set { transactionGuid = value; }
		}

		/// <summary>
		/// The related ControllersLogGuid
		/// </summary>
		public Guid ControllersLogGuid
		{
			get { return controllersLogGuid; }
			set { controllersLogGuid = value; }
		}

		/// <summary>
		/// The resulting database key for an instance of ControllersLogToTransactionMapClass.
		/// </summary>
		public Guid ControllersLogToTransactionGuid
		{
			get { return controllersLogToTransactionGuid; }
			set { controllersLogToTransactionGuid = value; }
		}

		#endregion

		#region SQL Commands and Data-related Methods

		/// <summary>
		/// Selects an instance of the ControllersLogToTransactionMapClass
		/// </summary>
		/// <param  param name="cmd">A SqlCommand instance.</param>
		public void SelectByTransactionGuidSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * " +
					" FROM " + SCHEMANAME + "tblControllersLogToTransaction " +
					" WHERE TransactionGuid = @TransactionGuid";

			cmd.Parameters.AddWithValue("@TransactionGuid", this.TransactionGuid);
		}

		/// <summary>
		/// Selects an instance of the ControllersLogToTransactionMapClass
		/// </summary>
		/// <param  param name="cmd">A SqlCommand instance.</param>
		public void SelectById(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * " +
					" FROM " + SCHEMANAME + "tblControllersLogToTransaction " +
					" WHERE ControllersLogToTransactionGuid = @ControllersLogToTransactionGuid";

			cmd.Parameters.AddWithValue("@ControllersLogToTransactionGuid", this.ControllersLogToTransactionGuid);
		}

		/// <summary>
		/// Inserts an instance of the ControllersLogToTransactionMapClass
		/// </summary>
		/// <param  param name="cmd">A SqlCommand instance.</param>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO " + SCHEMANAME + "tblControllersLogToTransaction " +
						 "(ControllersLogGuid, TransactionGuid, CreatedBy, CreatedDate,ControllersLogToTransactionGuid) " +
						 " VALUES (@ControllersLogGuid, @TransactionGuid, @CreatedBy, @CreatedDate, @ControllersLogToTransactionGuid)";

			cmd.Parameters.AddWithValue("@ControllersLogGuid", this.ControllersLogGuid);
			cmd.Parameters.AddWithValue("@TransactionGuid", this.TransactionGuid);
			cmd.Parameters.AddWithValue("@CreatedBy", this.CreatedBy);
			cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
			cmd.Parameters.AddWithValue("@ControllersLogToTransactionGuid", this._IdentityGuid);

		}

		/// <summary>
		/// Updates an instance of the ControllersLogToTransactionMapClass
		/// </summary>
		/// <param  param name="cmd">A SqlCommand instance.</param>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE " + SCHEMANAME + "tblControllersLogToTransaction SET " +
			  "UpdatedDate = @UpdatedDate," +
			  "UpdatedBy = @UpdatedBy" +
			  " WHERE ControllersLogToTransactionGuid = @ControllersLogToTransactionGuid";

			cmd.Parameters.Add("@ControllersLogToTransactionGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@ControllersLogToTransactionGuid"].Value = this.ControllersLogGuid;
		}

		/// <summary>
		/// Purges an instance of the ControllersLogToTransactionMapClass
		/// </summary>
		/// <param  param name="cmd">A SqlCommand instance.</param>
		public void Purge(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM " + SCHEMANAME + "tblControllersLogToTransaction " +
				" WHERE ControllersLogToTransactionGuid = @ControllersLogToTransactionGuid";
			cmd.Parameters.Add("@ControllersLogToTransactionGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ControllersLogToTransactionGuid"].Value = this.ControllersLogToTransactionGuid;
		}

		/// <summary>
		/// Loads an instance of the ControllersLogToTransactionMapClass.
		/// </summary>
		/// <param name="O">An object instance.</param>
		public override void Load(Object o)
		{
			this.Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				this.controllersLogToTransactionGuid = DataObject.getValue<Guid>(Row["ControllersLogToTransactionGuid"], Guid.Empty);
				this.controllersLogGuid = DataObject.getValue<Guid>(Row["ControllersLogGuid"], Guid.Empty);
				this.transactionGuid = DataObject.getValue<Guid>(Row["TransactionGuid"], Guid.Empty);
				base.CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				base.CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				base.UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], DateTimeOffset.Now);
				base.UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			}
			else
			{
				base.Load(o);
			}
		}

		/// <summary>
		/// Stores an instance of the ControllersLogToTransactionMapClass.
		/// </summary>
		/// <param name="o">An object instance.</param>
		public override void Store(Object o)
		{
			base.Store(o);
		}

		/// <summary>
		/// Enumerates the ControllerLogClass records
		/// </summary>
		/// <param name="cmd">A SqlCommand instance</param>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="bInTransaction">A bool representing if this method is wrapped in a transaction</param>
		/// <param name="StartDate">A Start Date</param>
		/// <param name="EndDate">The end date the ControllerLogClass record was stored in the database</param>
		/// <param name="Deleted">A bool indicating whether or not to retrieve logically deleted records</param>
		public void EnumerateByStartStopDatesSQL(SqlCommand cmd, SecurityClass security, DateTimeOffset StartDate, 
			DateTimeOffset EndDate, bool Deleted, Guid transactionGuid)
		{
			var transactionGuidStr = (transactionGuid != Guid.Empty && transactionGuid != null) ?
				"cltt.TransactionGuid = @TransactionGuid" : string.Empty;
			cmd.CommandText = "SELECT * " +
				" FROM tblControllersLog cl inner join " + SCHEMANAME + "tblControllersLogToTransaction cltt" +
				" on cl.ControllersLogGuid = cltt.ControllersLogGuid " +
				" WHERE " + transactionGuidStr +
				" AND cl.SiteGuid = @SiteGuid " +
				" AND cl.EventTime >= @StartDate " +
				" AND cl.EventTime < @EndDate " +
				" AND cl.Deleted = @Deleted ";

			if (!string.IsNullOrEmpty(transactionGuidStr))
				cmd.Parameters.AddWithValue("@TransactionGuid", transactionGuid);
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@StartDate", StartDate);
			cmd.Parameters.AddWithValue("@EndDate", EndDate.AddDays(1));

			if (Deleted)
			{
				cmd.Parameters.AddWithValue("@Deleted", 1);
			}
			else
			{
				cmd.Parameters.AddWithValue("@Deleted", 0);
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Resets all member variables of the class.
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			this.controllersLogToTransactionGuid = Guid.Empty;
			this.controllersLogGuid = Guid.Empty;
			this.transactionGuid = Guid.Empty;
		}

		#endregion

	}
	#endregion
}
