///***************************************************************************
/// Module Name:  AutoDistributionReasonCodeClass
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// A list of AutoDistributionReasonCodeClass
	/// </summary>
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(AutoDistributionReasonCodeClass))]
	public class AutoDistributionReasonCodeCollectionClass : List<AutoDistributionReasonCodeClass>
	{
		/// <summary>
		/// Use this only if you know your list has unique Guid value
		/// Empty Guid is ok.  It typically happens for a short moment when we are adding a new one.
		/// </summary>
		/// <param name="targetGuid">Reason Code Guid</param>
		/// <returns>Returns the reason code with the given Guid</returns>
		public AutoDistributionReasonCodeClass this[Guid targetGuid]
		{
			get
			{
				return this.Single<AutoDistributionReasonCodeClass>(reasonCode => reasonCode.IdentityGuid == targetGuid);
			}
		}
	}

	/// <summary>
	/// Reason Code used in Auto Distribution Transactions
	/// This is the Data Object class which holds the data and prepares SqlCommands.
	/// </summary>
	[DataContract]
   [Serializable]
	public class AutoDistributionReasonCodeClass : BaseDataObject
	{
		#region constants
		public const string TransactionFieldID = "ReasonCode";
		private const string SqlSPPrefix = "dbo.usp_tblAutoDistributionReasonCodes_";
		private const string SqlInsertSP = SqlSPPrefix + "ApplicationInsert";
		private const string SqlDeleteSP = SqlSPPrefix + "ApplicationDelete";
		private const string SqlUpdateSP = SqlSPPrefix + "ApplicationUpdate";
		private const string SqlSelectSP = SqlSPPrefix + "Select";
		#endregion


		#region Ctors and initialization
		/// <summary>
		/// Default Constructor
		/// </summary>
		public AutoDistributionReasonCodeClass()
		{
			ResetInternal();
		}

		/// <summary>
		/// Resets all properties
		/// </summary>
		public override void Reset()
		{
			ResetInternal();
		}
		#endregion Ctors and initialization


		#region Public Data Members

		public string Code
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		[DataMember]
		public string Description { get; set; }

		#endregion Public Data Members

		#region override properties

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE; }
			set { }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#endregion override properties

		#region public methods
		/// <summary>
		/// Loads from the given DataRow
		/// </summary>
		/// <param name="srcRow">DataRow which has the Reason Code record</param>
		/// <returns>Returns the loaded reason code</returns>
		public static AutoDistributionReasonCodeClass Load(DataRow srcRow)
		{
			AutoDistributionReasonCodeClass reasonCode = new AutoDistributionReasonCodeClass();
			reasonCode.IdentityGuid = DataObject.getValue<Guid>(srcRow["AutoDistributionReasonCodeGuid"], Guid.Empty);
			reasonCode.SiteGuid = DataObject.getValue<Guid>(srcRow["SiteGuid"], Guid.Empty);
			reasonCode.Code = DataObject.getValue<string>(srcRow["ReasonCode"], string.Empty);
			reasonCode.Description = DataObject.getValue<string>(srcRow["Description"], string.Empty);
			return reasonCode;
		}

		/// <summary>
		/// Loads from the given srcObject
		/// </summary>
		/// <param name="o">object which has Reason Code Data</param>
		public override void Load(Object o)
		{
			if (typeof(DataRow).IsInstanceOfType(o))
			{
				Load((DataRow)o);
			}
		}
		#endregion public methods

		#region private methods

		/// <summary>
		/// This is basically reset.  
		/// This is created to follow the rule: CA2214: Do not call overridable methods in constructors
		/// </summary>
		private void ResetInternal()
		{
			_ID = string.Empty;
			Description = string.Empty;
			base.Reset();
		}
		#endregion private methods
		#region Methods with SQLs

		/// <summary>
		/// Prepares a SqlCommand to select all records
		/// </summary>
		/// <param name="cmd">SqlCommand to be configured</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="bInTransaction">are we in transaction?</param>
		public static void PrepareSelectSqlCommand(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			AutoDistributionReasonCodeClass reasonCode = new AutoDistributionReasonCodeClass();
			reasonCode.SiteGuid = security.SiteGuid;
			reasonCode.PrepareSelectSqlCommand(cmd, security, bInTransaction, false, false);
		}

		/// <summary>
		/// Prepares a SqlCommand to select the record with the given Guid
		/// </summary>
		/// <param name="cmd">SqlCommand to be configured</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="bInTransaction">are we in transaction?</param>
		/// <param name="reasonCodeGuid">Reason Code Guid to be filtered by</param>
		public static void PrepareSelectByGuidSqlCommand(SqlCommand cmd, SecurityClass security, bool bInTransaction, Guid reasonCodeGuid)
		{
			AutoDistributionReasonCodeClass reasonCode = new AutoDistributionReasonCodeClass();
			reasonCode.SiteGuid = security.SiteGuid;
			reasonCode.IdentityGuid = reasonCodeGuid;
			reasonCode.PrepareSelectSqlCommand(cmd, security, bInTransaction, true, false);
		}

		/// <summary>
		/// Prepares a SqlCommand to select the record with the given Code
		/// </summary>
		/// <param name="cmd">SqlCommand to be configured</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="bInTransaction">are we in transaction?</param>
		/// <param name="code">code/ID to be filtered by</param>
		public static void PrepareSelectByIDSqlCommand(SqlCommand cmd, SecurityClass security, bool bInTransaction, string code)
		{
			AutoDistributionReasonCodeClass reasonCode = new AutoDistributionReasonCodeClass();
			reasonCode.SiteGuid = security.SiteGuid;
			reasonCode.Code = code;
			reasonCode.PrepareSelectSqlCommand(cmd, security, bInTransaction, false, true);
		}
		
		/// <summary>
		/// Prepares a SqlCommand to select from the Reason Code table.
		/// Filtering criteria will be included if specified.
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="bInTransaction">Are we in Transaction?</param>
		/// <param name="byGuid">Select by Guid</param>
		/// <param name="byCode">Select by ReasonCode(ID)</param>
		private void PrepareSelectSqlCommand(SqlCommand cmd, SecurityClass security, bool bInTransaction, bool byGuid, bool byCode)
		{
			cmd.CommandText = SqlSelectSP;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@LoginSiteGuid", security.LoginSiteGuid);
			cmd.Parameters.AddWithValue("@ReasonCode", byCode ? this.Code : (object)DBNull.Value);
			cmd.Parameters.AddWithValue("@ReasonCodeGuid", byGuid ? this.IdentityGuid : (object)DBNull.Value);			
		}

		/// <summary>
		/// Creates common parameters for Insert and Update operations
		/// </summary>
		/// <param name="cmd">SqlCommand to be configured</param>
		private void AddCommonParameters(SqlCommand cmd)
		{
			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@ReasonCode", this.Code);
			cmd.Parameters.AddWithValue("@Description", this.Description);
			cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
		}

		/// <summary>
		/// Prepares a SqlCommand for upate
		/// </summary>
		/// <param name="cmd">SqlCommand to be configured</param>
		/// <param name="security">Security object used by FM</param>
		public void PrepareUpdateSqlCommand(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = SqlUpdateSP;
			AddCommonParameters(cmd);
			cmd.Parameters.AddWithValue("@ReasonCodeGuid", this.IdentityGuid);
		}

		/// <summary>
		/// Prepares a SqlCommand for Insert
		/// </summary>
		/// <param name="cmd">SqlCommand to be configured</param>
		/// <param name="security">Security object used by FM</param>
		public void PrepareInsertSqlCommand(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = SqlInsertSP;
			AddCommonParameters(cmd);
			cmd.Parameters.AddWithValue("@CreatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this.UpdatedBy);
			DataObject.AddGuidOutputParameter(cmd, "@ReasonCodeGuid");
		}

		/// <summary>
		/// Prepares a SqlCommand for Delete
		/// </summary>
		/// <param name="cmd">SqlCommand to be configured</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="reasonCodeGuid">Guid of the Reason code to be deleted</param>		
		public static void PrepareDeleteSqlCommand(SqlCommand cmd, SecurityClass security, Guid reasonCodeGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = SqlDeleteSP;
			cmd.Parameters.AddWithValue("@ReasonCodeGuid", reasonCodeGuid);
		}
		#endregion
	}
}
