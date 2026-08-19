///***************************************************************************
/// Module Name:  AutoDistributionReasonCodes
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// This is the business/service object for Auto Distribution Reason Codes
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AutoDistributionReasonCodes : FMBusinessObjects.BusinessInterfaces.IAutoDistributionReasonCodes
	{
		#region constants and fields
		// error messages
		private const string MessageExists = "Reason Code Exists";
		private const string MessageInvalidEntity = "Invalid Reason Code";
		private const string MessageInvalidSecurity = "Invalid Security";
		private const string MessageInvalidSite = "Invalid Site";
		private const string MessageReservedWord = "Reason code({0}) is a reserved key word.";

		// DAL object
		private FMBusinessServices.DataAccessLayer.ConsolidatedDAClass _consolidatedDA;
		#endregion

		#region ctr
		/// <summary>
		/// Default constructor
		/// </summary>
		public AutoDistributionReasonCodes()
		{
			_consolidatedDA = new FMBusinessServices.DataAccessLayer.ConsolidatedDAClass();
		}
		#endregion ctr

		#region IAutoDistributionReasonCodes interface method
		/// <summary>
		/// Selects all reason codes for the current site
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		/// <returns>Returns list of Reason Codes</returns>
		public AutoDistributionReasonCodeCollectionClass Enumerate(SecurityClass security)
		{
			SecurityCheckForRead(security);
			using (SqlCommand cmd = new SqlCommand())
			{
				AutoDistributionReasonCodeCollectionClass reasonCodeList = new AutoDistributionReasonCodeCollectionClass();
				AutoDistributionReasonCodeClass.PrepareSelectSqlCommand(cmd, security, ContextUtil.IsInTransaction);
				DataTable resultTable = _consolidatedDA.GetDataTable(cmd, security);

				if (resultTable != null)
				{
					foreach (DataRow currentRow in resultTable.Rows)
					{
						reasonCodeList.Add(AutoDistributionReasonCodeClass.Load(currentRow));
					}
				}
				
				return reasonCodeList;
			}
		}

		/// <summary>
		/// Updates the given Reason Code
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		/// <param name="reasonCode">Reason Code to be modified</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AutoDistributionReasonCodeClass reasonCode)
		{
			SecurityCheckForUpdate(security);
			Validate(security, reasonCode);
			reasonCode.UpdatedDate = DateTimeOffset.Now;
			reasonCode.UpdatedBy = security.UserID;
			AutoDistributionReasonCodeClass oldReasonCode = Get(security, reasonCode.IdentityGuid);

 
         // changing ownership
         if (reasonCode.SiteGuid != oldReasonCode.SiteGuid)
			{
				EntityToSiteMaps.RemoveAllMapsForEntity(security, ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE, reasonCode.IdentityGuid);
				EntityToSiteMaps.AddNewMap(security, reasonCode, GetType().GUID);
			}

			using (SqlCommand cmd = new SqlCommand())				
			{
				reasonCode.PrepareUpdateSqlCommand(cmd, security);
				_consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Adds the given Reason Code
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		/// <param name="reasonCode">Reason Code to be added</param>
		/// <returns>New Guid for the Reason Code</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AutoDistributionReasonCodeClass reasonCode)
		{

			SecurityCheckForUpdate(security); 
			Validate(security, reasonCode);

			reasonCode.SiteGuid = security.SiteGuid;
			reasonCode.CreatedDate = DateTimeOffset.Now;
			reasonCode.CreatedBy = security.UserID;
			reasonCode.UpdatedDate = reasonCode.CreatedDate;
			reasonCode.UpdatedBy = reasonCode.CreatedBy;
			using (SqlCommand cmd =  new SqlCommand())				
			{
				reasonCode.PrepareInsertSqlCommand(cmd, security);
				_consolidatedDA.ExecuteQuery(security, cmd);
				reasonCode.IdentityGuid = (Guid)cmd.Parameters["@ReasonCodeGuid"].Value;
			}
			EntityToSiteMaps.AddNewMap(security,  reasonCode, GetType().GUID);
			return reasonCode.IdentityGuid;
		}

		/// <summary>
		/// Deletes the Reason Code with the given Guid
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		/// <param name="reasonCodeGuid">Guid of the reason code to be deleted</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid reasonCodeGuid)
		{
			SecurityCheckForUpdate(security);
			EntityToSiteMaps.RemoveAllMapsForEntity(security, ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE, reasonCodeGuid);
			using (SqlCommand cmd =  new SqlCommand())				
			{
				AutoDistributionReasonCodeClass.PrepareDeleteSqlCommand(cmd, security, reasonCodeGuid);
				_consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Retrieves the Reason Code with the given Guid
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		/// <param name="reasonCodeGuid">Guid of the reason code to be retrieved</param>
		/// <returns>The Reason code being retrieved</returns>
		public AutoDistributionReasonCodeClass Get(SecurityClass security, Guid reasonCodeGuid)
		{
			return GetInternal(security, reasonCodeGuid);
		}

		/// <summary>
		/// Retrieves the Guid of the Reason Code with the given Code/ID
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		/// <param name="code">Reason Code to be found</param>
		/// <returns>Guid of the Given Reason Code</returns>
		public Guid GetIdentityGuid(SecurityClass security, string code)
		{
			AutoDistributionReasonCodeClass reasonCode = GetInternal(security, code);
			Guid resultGuid = Guid.Empty;
			if (reasonCode != null)
			{
				resultGuid = reasonCode.IdentityGuid;
			}
			return resultGuid;
		}
		
		#endregion IAutoDistributionReasonCodes interface method

		#region private methods
		/// <summary>
		/// Wrapper for the SecurityCheck method
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		private void SecurityCheckForRead(SecurityClass security)
		{
			SecurityCheck(security, true);
		}

		/// <summary>
		/// Wrapper for the SecurityCheck method
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		private void SecurityCheckForUpdate(SecurityClass security)
		{
			SecurityCheck(security, false);
		}

		/// <summary>
		/// Checks whether 
		///   the security object is valid
		///   the caller has rights to perform the desired operation
		/// </summary>
		/// <param name="security">caller's security</param>
		/// <param name="isViewOnly">type of operation</param>
		private void SecurityCheck(SecurityClass security, bool isViewOnly)
		{
			if (security == null)
			{
				throw new ArgumentNullException(MessageInvalidSecurity);
			}

			bool hasRight = isViewOnly ?
							security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) :
							security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION);

			if (!hasRight)
			{
				throw new FMInsufficientRightsException();
			}

		}

		/// <summary>
		/// Load an AutoDistributionReasonCodeClass object by the given key
		/// The key can either be a Guid or the ReasonCode(ID).
		/// Yes, when Guid is used, it involves boxing/unboxing.  Trade ignorable perfomance difference with shared/common code.
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		/// <param name="reasonCodeKey">key: can be Guid or Reason Code</param>
		/// <returns>Reason code with the given key</returns>
		private AutoDistributionReasonCodeClass GetInternal(SecurityClass security, object reasonCodeKey)
		{
			SecurityCheckForRead(security);
			bool byGuid = reasonCodeKey.GetType() == typeof(Guid);

			using (SqlCommand cmd = new SqlCommand())
			{

				if (byGuid)
				{
					AutoDistributionReasonCodeClass.PrepareSelectByGuidSqlCommand( cmd, security, ContextUtil.IsInTransaction, (Guid)reasonCodeKey);
				}
				else
				{
					AutoDistributionReasonCodeClass.PrepareSelectByIDSqlCommand(cmd, security, ContextUtil.IsInTransaction, (string)reasonCodeKey);
				}

				AutoDistributionReasonCodeClass reasonCode = null;
				DataTable resultTable = _consolidatedDA.GetDataTable(cmd, security);

				if (resultTable != null)
				{
					if (resultTable.Rows.Count > 0)
					{
						reasonCode = AutoDistributionReasonCodeClass.Load(resultTable.Rows[0]);
					}
				}

				return reasonCode;
			}
		}

		/// <summary>
		/// Validates the given Reason Code
		/// </summary>
		/// <param name="security">Security object used by FM</param>
		/// <param name="reasonCode">Reason Code to be validated</param>
		private void Validate(SecurityClass security, AutoDistributionReasonCodeClass reasonCode)
		{
			if (reasonCode == null)
			{
				throw new ArgumentException(MessageInvalidEntity);
			}

			if (string.IsNullOrWhiteSpace(reasonCode.Code))
			{
				throw new Exception(MessageInvalidEntity);
			}

			if (InternalClasses.Common.IsReservedWordForID(security, reasonCode.Code))
			{
				throw new Exception(string.Format(MessageReservedWord, reasonCode.Code));
			}

			// check whether there is an existing Reason Code with the same Code/ID
			Guid existingGuid = GetIdentityGuid(security, reasonCode.ID);

			if (!existingGuid.IsEmpty() &&
				existingGuid!=reasonCode.IdentityGuid)
			{
				throw new Exception(MessageExists);
			}
		}

		#endregion
	}
}