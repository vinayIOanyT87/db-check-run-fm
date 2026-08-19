// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerLogClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Web page resonsible for managing Controller Log entries in the FuelsManager Website.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Linq;

namespace FMBusinessObjects.DataObjects
{
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Summary description for ControllersLogToTransactionMapClass.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class ControllerLogClassCollectionClass : List<ControllerLogClass> { }

	/// <summary>
	/// ControllerLogClass Data Object
	/// </summary>
	[DataContract]
	[Serializable]
	public class ControllerLogClass : BaseDataObject
	{
		#region Properties
		/// <summary>
		/// AuditLog property
		/// </summary>
		[DataMember]
		public bool AuditLog = false;

		/// <summary>
		/// EventTime property
		/// </summary>
		[DataMember]
		public DateAndTime _EventTime = new DateAndTime();

		/// <summary>
		/// Controller property
		/// </summary>
		[DataMember]
		public string _Controller;

		/// <summary>
		/// Memo property
		/// </summary>
		[DataMember]
		public string _Memo;

		/// <summary>
		/// EventTime Date property
		/// </summary>
		[QueryWriterField("Event Time", "EventTime")]
		public DateAndTime EventTimeObject { get { return _EventTime; } }

		/// <summary>
		/// EventTime string property
		/// </summary>
		public string EventTime { get { return _EventTime.ToString(); } set { SetDateAndTime("Event Time", value, ref _EventTime); } }

		/// <summary>
		/// Controller property
		/// </summary>
		[QueryWriterField("Controller")]
		public string Controller { get { return _Controller; } set { SetString("Controller", 50, value, ref _Controller); } }

		/// <summary>
		/// Memo property
		/// </summary>
		[QueryWriterField("Memo")]
		public string Memo { get { return _Memo; } set { SetString("Memo", 150, value, ref _Memo); } }

		#endregion

		#region Constructors
		/// <summary>
		/// Default ControllerLogClass constructor
		/// </summary>
		public ControllerLogClass()
		{
			this._EventTime = new DateAndTime();
			Reset();
		}

		public ControllerLogClass(SiteClass site)
		{
			this._EventTime = new DateAndTime(site);
			Reset();
		}


		#endregion

		#region Public methods
		/// <summary>
		/// This method resets the attributes of the class
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			_Controller = "";
			_Memo = "";
			_EventTime.Value = TimeConverter.Now(_EventTime.StandardName);
			Deleted = false;
		}
		#endregion

		#region Data retrieval methods
		/// <summary>
		/// Loads a DataSet with ControllerLogClass data
		/// </summary>
		/// <param name="Set">A DataSet</param>
		public void Load(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException("Set is Null");
			}

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

			_IdentityGuid = DataObject.getValue<Guid>(Row["ControllersLogGuid"], Guid.Empty);
			_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);

			_Controller = DataObject.getValue<string>(Row["Controller"], "");
			_Memo = DataObject.getValue<string>(Row["Memo"], "");
			_EventTime.Value = DataObject.getValue<DateTimeOffset>(Row["EventTime"], TimeConverter.Today(_EventTime.StandardName));
			Deleted = DataObject.getValue<bool>(Row["Deleted"], false);

			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);

		}

		/// <summary>
		/// Inserts a ControllerLogClass record
		/// </summary>
		/// <param name="cmd">A SqlCommand instance</param>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblControllersLog (" +
			  "SiteGuid," +
			  "EventTime," +
			  "Controller," +
			  "Memo," +
			  "Deleted," +
			  "CreatedDate," +
			  "CreatedBy," +
			  "UpdatedDate," +
			  "UpdatedBy," +
			  "ControllersLogGuid" +
			  ") VALUES (" +
			  "@SiteGuid," +
			  "@EventTime," +
			  "@Controller," +
			  "@Memo," +
			  "@Deleted," +
			  "@CreatedDate," +
			  "@CreatedBy," +
			  "@UpdatedDate," +
			  "@UpdatedBy," +
			  "@ControllersLogGuid" +
			  ")";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EventTime", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Controller", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Memo", SqlDbType.NVarChar, 150);
			cmd.Parameters.Add("@Deleted", SqlDbType.Bit);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ControllersLogGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@EventTime"].Value = _EventTime.Value;
			cmd.Parameters["@Controller"].Value = Controller;
			cmd.Parameters["@Memo"].Value = Memo;

			if (Deleted)
			{
				cmd.Parameters["@Deleted"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Deleted"].Value = 0;
			}

			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@ControllersLogGuid"].Value = _IdentityGuid;
		}

		/// <summary>
		/// Updates a ControllerLogClass record
		/// </summary>
		/// <param name="cmd">A SqlCommand instance</param>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblControllersLog SET " +
			  "SiteGuid = @SiteGuid," +
			  "EventTime = @EventTime," +
			  "Controller = @Controller," +
			  "Memo = @Memo," +
			  "Deleted = @Deleted," +
			  "CreatedDate = @CreatedDate," +
			  "CreatedBy = @CreatedBy," +
			  "UpdatedDate = @UpdatedDate," +
			  "UpdatedBy = @UpdatedBy" +
			  " WHERE ControllersLogGuid = @ControllersLogGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EventTime", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Controller", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Memo", SqlDbType.NVarChar, 150);
			cmd.Parameters.Add("@Deleted", SqlDbType.Bit);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ControllersLogGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@EventTime"].Value = _EventTime.Value;
			cmd.Parameters["@Controller"].Value = Controller;
			cmd.Parameters["@Memo"].Value = Memo;

			if (Deleted)
			{
				cmd.Parameters["@Deleted"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Deleted"].Value = 0;
			}

			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@ControllersLogGuid"].Value = IdentityGuid;
		}

		/// <summary>
		/// Deletes a ControllerLogClass record
		/// </summary>
		/// <param name="cmd">A SqlCommand instance</param>
		public void DeleteSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblControllersLog SET " +
				"SiteGuid = @SiteGuid," +
				"EventTime = @EventTime," +
				"Controller = @Controller," +
				"Memo = @Memo," +
				"Deleted = @Deleted," +
				"CreatedDate = @CreatedDate," +
				"CreatedBy = @CreatedBy," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE ControllersLogGuid = @ControllersLogGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EventTime", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Controller", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Memo", SqlDbType.NVarChar, 150);
			cmd.Parameters.Add("@Deleted", SqlDbType.Bit);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ControllersLogGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@EventTime"].Value = _EventTime.Value;
			cmd.Parameters["@Controller"].Value = Controller;
			cmd.Parameters["@Memo"].Value = Memo;

			if (Deleted)
			{
				cmd.Parameters["@Deleted"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Deleted"].Value = 0;
			}

			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@ControllersLogGuid"].Value = IdentityGuid;
		}

		/// <summary>
		/// Purges a ControllerLogClass record
		/// </summary>
		/// <param name="cmd">A SqlCommand instance</param>
		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblControllersLog SET DELETED = @Deleted WHERE ControllersLogGuid = @ControllersLogGuid";

			cmd.Parameters.Add("@ControllersLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Deleted", SqlDbType.Bit);
			cmd.Parameters["@ControllersLogGuid"].Value = IdentityGuid;

			if (Deleted)
			{
				cmd.Parameters["@Deleted"].Value = 0;
			}
			else
			{
				cmd.Parameters["@Deleted"].Value = 1;
			}
		}

		/// <summary>
		/// Retrieves a ControllerLogClass record
		/// </summary>
		/// <param name="cmd">A SqlCommand instance</param>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="site">The current site.</param>
		/// <param name="bInTransaction">A bool representing if this method is wrapped in a transaction</param>
		public void SelectSQL(SqlCommand cmd, SecurityClass security, SiteClass site, bool bInTransaction)
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblControllersLog " + SQLUpdateLock(bInTransaction) + " WHERE ControllersLogGuid = @ControllersLogGuid";

			this.AddSiteFilterCondition(cmd, security, site);

			cmd.Parameters.Add("@ControllersLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ControllersLogGuid"].Value = IdentityGuid;
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
		/// <param name="site">The current site.</param>
		public void EnumerateByStartStopDates(SqlCommand cmd, SecurityClass security, bool bInTransaction, DateTimeOffset StartDate, DateTimeOffset EndDate, bool Deleted, Guid transactionGuid, SiteClass site)
		{
			var transactionGuidStr = (transactionGuid != Guid.Empty && transactionGuid != null) ?
				"cltt.TransactionGuid = @TransactionGuid" : string.Empty;

			cmd.CommandText = "SELECT * " +
				" FROM tblControllersLog INNER JOIN map.tblControllersLogToTransaction cltt" +
				" ON tblControllersLog.ControllersLogGuid = cltt.ControllersLogGuid " +
				" WHERE " + transactionGuidStr +
				" AND tblControllersLog.EventTime >= @StartDate " +
				" AND tblControllersLog.EventTime < @EndDate " +
				" AND tblControllersLog.Deleted = @Deleted ";

			this.AddSiteFilterCondition(cmd, security, site);

			if (!string.IsNullOrEmpty(transactionGuidStr))
			{
				cmd.Parameters.AddWithValue("@TransactionGuid", transactionGuid);
			}
		
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


		/// </summary>
		/// <param name="cmd">A SqlCommand instance</param>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="StartDate">A Start Date</param>
		/// <param name="EndDate">The end date the ControllerLogClass record was stored in the database</param>
		/// <param name="Deleted">A bool indicating whether or not to retrieve logically deleted records</param>
		/// <param name="site">The current site.</param>
		public void EnumerateByStartStopDateAndDeletedSQL(SqlCommand cmd, SecurityClass security,  
			DateTimeOffset StartDate, DateTimeOffset EndDate, bool Deleted, SiteClass site)
		{
			cmd.CommandText = "SELECT * " +
				" FROM tblControllersLog " +
				" WHERE tblControllersLog.EventTime >= @StartDate " +
				" AND tblControllersLog.EventTime < @EndDate " +
				" AND tblControllersLog.Deleted = @Deleted ";

			this.AddSiteFilterCondition(cmd, security, site);

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

		/// <summary>
		/// Enumerates the ControllerLogClass records
		/// </summary>
		/// <param name="cmd">A SqlCommand instance</param>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="bInTransaction">A bool representing if this method is wrapped in a transaction</param>
		/// <param name="selectedGuid">A Guid</param>
		/// <param name="site">The current site.</param>
		public void EnumerateByIdentityGuid(SqlCommand cmd, SecurityClass security, bool bInTransaction, Guid selectedGuid, SiteClass site)
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblControllersLog WHERE ControllersLogGuid = @ControllersLogGuid";

			this.AddSiteFilterCondition(cmd, security, site);

			cmd.Parameters.Add("@ControllersLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ControllersLogGuid"].Value = selectedGuid;
		}

		/// <summary>
		/// Adds the site filter condition.  It includes the ability to add member sites to a group site 
		/// query so as to include member site control log records.
		/// </summary>
		/// <param name="cmd">The SQL command being built.</param>
		/// <param name="cmd">Contains Security Information</param>
		/// <param name="site">The current site.</param>
		private void AddSiteFilterCondition(SqlCommand cmd, SecurityClass security, SiteClass site)
		{
			var siteGuids = site.SiteToSiteMapCollection.Select((parentSite) => parentSite.ChildSiteGuid.ToString()).ToArray();

			string memberSites = String.Empty;

			if (siteGuids.Length > 0)
			{
				memberSites += ",'" + String.Join("','", siteGuids) + "'";
			}

			cmd.CommandText += String.Format(" AND tblControllersLog.[SiteGuid] IN (@SiteGuidParam{0}) ", memberSites);

			cmd.Parameters.Add("@SiteGuidParam", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
		}

		#endregion

	}
}
