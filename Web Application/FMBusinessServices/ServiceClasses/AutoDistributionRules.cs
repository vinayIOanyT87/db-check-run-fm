///***************************************************************************
/// Module Name:  AutoDistributionRules
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// This is the business/service object for Auto Distribution Rules
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AutoDistributionRules : FMBusinessObjects.BusinessInterfaces.IAutoDistributionRules, IDependency
	{
		#region constants and fields
		// error messages
		private const string MessageExists = "Rule Exists";

		private const string MessageInvalidBaseDataObject = "Invalid BaseDataObject";
		private const string MessageInvalidEntity = "Invalid Rule";
		private const string MessageIDRequired = "ID Required";
		private const string MessageInvalidSecurity = "Invalid Security";
		private const string MessageReservedWord = "Rule ID({0}) is a reserved key word.";
		private const string MessageInvalidEntityGuid = "Invalid Guid";

		// DAL object
		private FMBusinessServices.DataAccessLayer.ConsolidatedDAClass consolidatedDA;
		#endregion

		#region ctr
		/// <summary>
		/// Default constructor
		/// </summary>
		public AutoDistributionRules()
		{
			this.consolidatedDA = new FMBusinessServices.DataAccessLayer.ConsolidatedDAClass();
		}
		#endregion ctr

		#region IAutoDistributionRules interface method

		/// <summary>
		/// Selects Rules for the current site based on the given criteria
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="managerGuid">Manager Guid to be filtered by(Empty if not used)</param>
		/// <param name="productGuid">Product Guid to be filtered by(Empty if not used)</param>
		/// <param name="searchText">String to be filtered by(Empty/null if not used)</param>
		/// <returns>All rules that match the criterias</returns>
		public AutoDistributionRuleDOCollection Enumerate(SecurityClass mySecurity, Guid managerGuid, Guid productGuid, string searchText)
		{
			SecurityCheckForRead(mySecurity);

			using (SqlCommand cmd = new SqlCommand())
			{

				// determine which method to call based on criteria
				if (managerGuid == Guid.Empty && productGuid == Guid.Empty && string.IsNullOrWhiteSpace(searchText))
				{
					AutoDistributionRuleDAC.PrepareSelectSqlCommand(cmd, mySecurity, ContextUtil.IsInTransaction, Guid.Empty, null);
				}
				else
				{
					AutoDistributionRuleDAC.PrepareSelectSqlCommandByManagerProductDescription(cmd, mySecurity, managerGuid, productGuid, searchText);
				}

				// load the data into AutoDistributionRuleDOCollection
				AutoDistributionRuleDOCollection ruleList = new AutoDistributionRuleDOCollection();
				DataTable resultTable = this.consolidatedDA.GetDataTable(cmd, mySecurity);
				if (resultTable != null)
				{
					foreach (DataRow currentRow in resultTable.Rows)
					{
						ruleList.Add(LoadWrapper(mySecurity, currentRow));
					}
				}

				return ruleList;
			}
		}

		/// <summary>
		/// Returns all Assigned with their guids and ids
		/// Passsing in Manager, owner, proudct childTYpe will include managers from manager,onwer, product groups.
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="ruleID">Rule ID</param>
		/// <param name="childType">Type of child to enumerate</param>
		/// <returns>Returns all children for the given type</returns>
		public List<BaseMapAssignedInfoDO> EnumerateAssigned(SecurityClass mySecurity, Guid ruleID, AutoDistributionRuleChildMapTypes childType)
		{
			SecurityCheckForRead(mySecurity);

			using (SqlCommand cmd = new SqlCommand())
			{
				switch(childType)
				{
					case AutoDistributionRuleChildMapTypes.Manager:
						AutoDistributionRuleDAC.PrepareSelectManagersSqlCommand(cmd, mySecurity.SiteGuid, mySecurity.LoginSiteGuid, ruleID);
						break;
					case AutoDistributionRuleChildMapTypes.Owner:
						AutoDistributionRuleDAC.PrepareSelectOwnersSqlCommand(cmd, mySecurity.SiteGuid, mySecurity.LoginSiteGuid, ruleID);
						break;
					case AutoDistributionRuleChildMapTypes.Product:
						AutoDistributionRuleDAC.PrepareSelectProductsSqlCommand(cmd, mySecurity.SiteGuid, mySecurity.LoginSiteGuid, ruleID);
						break;
				}

				// get a list of records of assigned entities
				DataTable resultTable = this.consolidatedDA.GetDataTable(cmd, mySecurity);				
				List<BaseMapAssignedInfoDO> assignedList = new List<BaseMapAssignedInfoDO>();

				if (resultTable != null)
				{
					BaseMapDAC myDAC = AutoDistributionRuleMapDACs.MapDACList[childType];

					// copy data from each row to each "mini" entity record, just Guid and ID
					foreach (DataRow currentRow in resultTable.Rows)
					{
						BaseMapAssignedInfoDO newAssigned = new BaseMapAssignedInfoDO();
						myDAC.LoadAssignedInfo<BaseMapAssignedInfoDO>(currentRow, newAssigned);
						assignedList.Add(newAssigned);
					}
				}

				return assignedList;
			}
		}

		/// <summary>
		/// Updates the given Rule
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="newRule">Rule to be modified</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass mySecurity, AutoDistributionRuleDO newRule)
		{
			SecurityCheckForUpdate(mySecurity);
			Validate(mySecurity, newRule);

			// Tracking data
			newRule.UpdatedDate = DateTimeOffset.Now;
			newRule.UpdatedBy = mySecurity.UserID;

			AutoDistributionRuleDO oldRule = Get(mySecurity, newRule.IdentityGuid);


         // Handling ownership change
         if (newRule.SiteGuid != oldRule.SiteGuid)
			{
				EntityToSiteMaps.RemoveAllMapsForEntity(mySecurity, ENTITY_TYPE.AUTODISTRIBUTION_RULE, newRule.IdentityGuid);
				EntityToSiteMaps.AddNewMap(mySecurity, newRule, GetType().GUID);
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AutoDistributionRuleDAC.PrepareUpdateSqlCommand(cmd, mySecurity, newRule);
				this.consolidatedDA.ExecuteQuery(mySecurity, cmd);
			}

			// handle all children maps
			ModifyMaps(mySecurity, newRule, oldRule);

			// Post Validate
			PostValidate(mySecurity, newRule);
		}

		/// <summary>
		/// Adds the given Rule
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="newRule">Rule to be added</param>
		/// <returns>New Guid for the rule</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass mySecurity, AutoDistributionRuleDO newRule)
		{

			SecurityCheckForUpdate(mySecurity);
			Validate(mySecurity, newRule);

			// Tracking data
			newRule.CreatedDate = DateTimeOffset.Now;
			newRule.CreatedBy = mySecurity.UserID;
			newRule.UpdatedDate = newRule.CreatedDate;
			newRule.UpdatedBy = newRule.CreatedBy;

			// let's save
			using (SqlCommand cmd = new SqlCommand())
			{
				AutoDistributionRuleDAC.PrepareInsertSqlCommand(cmd, mySecurity, newRule);
				this.consolidatedDA.ExecuteQuery(mySecurity, cmd);
				newRule.IdentityGuid = (Guid)cmd.Parameters["@AutoDistributionRuleGuid"].Value;
			}

			// handle Entity to Site map
			EntityToSiteMaps.AddNewMap(mySecurity, newRule, GetType().GUID);

			// handle all children maps
			newRule.UpdateMapGuids();
			foreach (AutoDistributionRuleChildMapTypes mapType in AutoDistributionRuleDO.AllMapTypes)
			{
				GetMapBusinessObject(mapType).AddList(mySecurity, newRule.AllMapList[mapType]);
			}

			PostValidate(mySecurity, newRule);

			return newRule.IdentityGuid;
		}

		/// <summary>
		/// Deletes the Rule with the given Guid
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="ruleToBeDeleted">Rule to be deleted</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass mySecurity, Guid ruleToBeDeleted)
		{
			SecurityCheckForUpdate(mySecurity);

			EntityToSiteMaps.RemoveAllMapsForEntity(mySecurity, ENTITY_TYPE.AUTODISTRIBUTION_RULE, ruleToBeDeleted);
			
			using (SqlCommand cmd = new SqlCommand())
			{
				AutoDistributionRuleDAC.PrepareDeleteSqlCommand(cmd, mySecurity, ruleToBeDeleted);
				this.consolidatedDA.ExecuteQuery(mySecurity, cmd);
			}

			// Other children maps are deleted in the stored procedures
		}

		/// <summary>
		/// Retrieves the Rule with the given Guid
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="ruleGuid">Guid of the rule to be retrieved</param>
		/// <returns>The rule retrieved</returns>
		public AutoDistributionRuleDO Get(SecurityClass mySecurity, Guid ruleGuid)
		{
			return GetInternal(mySecurity, ruleGuid);
		}

		/// <summary>
		/// Retrieves the Guid of the Rule with the given ID
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="ruleID">Rule ID</param>
		/// <returns>The Guid of the given rule</returns>
		public Guid GetIdentityGuid(SecurityClass mySecurity, string ruleID)
		{
			AutoDistributionRuleDO theRule = GetInternal(mySecurity, ruleID);
			Guid resultGuid = Guid.Empty;
			if (theRule != null)
			{
				resultGuid = theRule.IdentityGuid;
			}
			return resultGuid;
		}

		#endregion IAutoDistributionRules interface method

		#region IDependency
		void IDependency.Insert(SecurityClass mySecurity, BaseDataObject dependentObject, bool preOperation)
		{
			Validate(mySecurity);
			Validate(dependentObject);

			if (preOperation && typeof(EntityToSiteMapClass).IsInstanceOfType(dependentObject))
			{
				EntityToSiteMapClass ruleToSiteMap = (EntityToSiteMapClass)dependentObject;

				if (ruleToSiteMap.TypeID == ENTITY_TYPE.AUTODISTRIBUTION_RULE)
				{
					// we have to use SiteGuid from ruleToSiteMap not mySecurity, because they should be different
					ValidateDefaultEOM(mySecurity, ruleToSiteMap.SiteGuid, ruleToSiteMap.SiteGuid, ruleToSiteMap.IdentityGuid, true);
				}
			}
		}

		void IDependency.Update(SecurityClass mySecurity, BaseDataObject dependentObject)
		{
		}

		void IDependency.Purge(SecurityClass mySecurity, BaseDataObject dependentObject)
		{
		}
		#endregion IDependency
		#region private methods

		/// <summary>
		/// Loads an AutoDistributionRuleClass object by the given key
		/// The key can either be a Guid or the ID.
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="ruleKey">key: can be Guid or Rule</param>
		/// <returns>the Rule DO with the given key</returns>
		private AutoDistributionRuleDO GetInternal(SecurityClass mySecurity, object ruleKey)
		{
			SecurityCheckForRead(mySecurity);

			// setup Guid or ID based on the parameter
			bool byGuid = ruleKey.GetType() == typeof(Guid);
			Guid ruleGuid = Guid.Empty;
			string ruleID = string.Empty;
			if (byGuid)
			{
				ruleGuid = (Guid)ruleKey;
			}
			else
			{
				ruleID = (string)ruleKey;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AutoDistributionRuleDAC.PrepareSelectSqlCommand(cmd, mySecurity, ContextUtil.IsInTransaction, ruleGuid, ruleID);

				DataTable resultTable = this.consolidatedDA.GetDataTable(cmd, mySecurity);
				AutoDistributionRuleDO newRule = null;

				if ((resultTable != null) && (resultTable.Rows.Count > 0))
				{
					newRule = LoadWrapper(mySecurity, resultTable.Rows[0]);
				}

				return newRule;
			}
		}

		/// <summary>
		/// Loads the rule object and its children
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="srcRow">DataRow contains the rule record</param>
		/// <returns>Rule object with values filled from the srcRow</returns>
		private AutoDistributionRuleDO LoadWrapper(SecurityClass mySecurity, DataRow srcRow)
		{
			// load the base object
			AutoDistributionRuleDO theRule = AutoDistributionRuleDAC.Load(srcRow);

			// load simple child objects: TrxAlias, ReasonCode
			theRule.TransactionAlias = (new TransactionAliasesClass()).Get(mySecurity, theRule.TransactionAliasGuid, false);
			theRule.DefaultReasonCode = (new AutoDistributionReasonCodes()).Get(mySecurity, theRule.DefaultReasonCodeGuid);

			// load children maps
			foreach (AutoDistributionRuleChildMapTypes mapType in AutoDistributionRuleDO.AllMapTypes)
			{
				theRule.AllMapList[mapType] = GetMapBusinessObject(mapType).EnumerateByAssignee(mySecurity, theRule.IdentityGuid);
			}
			CreateDisplayText(mySecurity, theRule);

			return theRule;
		}

		/// <summary>
		/// This creates the Map Business Object
		/// </summary>
		/// <param name="mapType">Child map type</param>
		/// <returns>Returns the corresponding child map</returns>
		private static AutoDistributionRuleMaps GetMapBusinessObject(AutoDistributionRuleChildMapTypes mapType)
		{
			BaseMapDAC myDAC = AutoDistributionRuleMapDACs.MapDACList[mapType];
			AutoDistributionRuleMaps mapsBO = new AutoDistributionRuleMaps(myDAC);
			return mapsBO;
		}

		/// <summary>
		/// Merge the given lists and concatenate the IDs(separated by commas)
		/// </summary>
		/// <param name="list1">First List</param>
		/// <param name="list2">Second List</param>
		/// <returns>Combined lists separated by commas</returns>
		private static string ListToString(List<BaseMapAssignedInfoDO> list1, List<BaseMapAssignedInfoDO> list2)
		{
			List<BaseMapAssignedInfoDO> combined = new List<BaseMapAssignedInfoDO>();
			combined.AddRange(list1);
			if (list2 != null)
			{
				combined.AddRange(list2);
			}
			return string.Join(", ", combined.Select(miniObject => miniObject.ID));
		}

		/// <summary>
		/// Modify maps of the given rule
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="newRuleMaps">New Rule maps</param>
		/// <param name="oldRuleMaps">Old Rule maps</param>
		private static void ModifyMaps(SecurityClass mySecurity, AutoDistributionRuleDO newRule, AutoDistributionRuleDO oldRule)
		{
			Validate(mySecurity);
			Validate(newRule);
			Validate(oldRule);

			foreach (AutoDistributionRuleChildMapTypes mapType in AutoDistributionRuleDO.AllMapTypes)
			{
				AutoDistributionRuleMapDOCollection newMapDOList = newRule.AllMapList[mapType];
				AutoDistributionRuleMapDOCollection oldMapDOList = oldRule.AllMapList[mapType];
				AutoDistributionRuleMaps mapsBO = GetMapBusinessObject(mapType);

				// find out what have been removed
				AutoDistributionRuleMapDOCollection deleteList = new AutoDistributionRuleMapDOCollection();
				foreach (AutoDistributionRuleMapDO currentMap in oldMapDOList)
				{
					if (newMapDOList.ContainsGuid(currentMap.IdentityGuid) == false)
					{
						deleteList.Add(currentMap);
					}
				}

				// separate the new and existing maps
				AutoDistributionRuleMapDOCollection insertList = new AutoDistributionRuleMapDOCollection();
				AutoDistributionRuleMapDOCollection updateList = new AutoDistributionRuleMapDOCollection();
				foreach (AutoDistributionRuleMapDO currentMap in newMapDOList)
				{
					if (oldMapDOList.ContainsGuid(currentMap.IdentityGuid))
					{
						updateList.Add(currentMap);
					}
					else
					{
						insertList.Add(currentMap);
					}
				}

				mapsBO.ModifyList(mySecurity, deleteList, updateList, insertList);
			}
		}

		/// <summary>
		/// Create text for display for all children maps
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="theRule">Rule to create text for</param>
		public static void CreateDisplayText(SecurityClass mySecurity, AutoDistributionRuleDO theRule)
		{
			Validate(mySecurity);
			Validate(theRule);

			Dictionary<AutoDistributionRuleChildMapTypes, List<BaseMapAssignedInfoDO>> assignedDOList = new Dictionary<AutoDistributionRuleChildMapTypes, List<BaseMapAssignedInfoDO>>();

			foreach (AutoDistributionRuleChildMapTypes mapType in AutoDistributionRuleDO.AllMapTypes)
			{
				List<BaseMapAssignedInfoDO> idList;
				switch(mapType)
				{
					case AutoDistributionRuleChildMapTypes.Owner:
						using (var cmd = new SqlCommand())
						{
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.CommandText = "map.usp_OwnerToAutoDistributionRuleSelectOwner";
							cmd.Parameters.AddWithValue("@AutoDistributionRuleGuid", theRule.IdentityGuid);
							cmd.Parameters.AddWithValue("@SiteGuid", mySecurity.SiteGuid);
							idList = GetMapBusinessObject(mapType).EnumerateAssigned<BaseMapAssignedInfoDO>(mySecurity, theRule.IdentityGuid, cmd);
						}
						break;
					case AutoDistributionRuleChildMapTypes.Manager:
						using (var cmd = new SqlCommand())
						{
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.CommandText = "map.usp_ManagerToAutoDistributionRuleSelectManager";
							cmd.Parameters.AddWithValue("@AutoDistributionRuleGuid", theRule.IdentityGuid);
							cmd.Parameters.AddWithValue("@SiteGuid", mySecurity.SiteGuid);
							idList = GetMapBusinessObject(mapType).EnumerateAssigned<BaseMapAssignedInfoDO>(mySecurity, theRule.IdentityGuid, cmd);
						}
						break;
					case AutoDistributionRuleChildMapTypes.Product:
						using (var cmd = new SqlCommand())
						{
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.CommandText = "map.usp_ProductToAutoDistributionRuleSelectProduct";
							cmd.Parameters.AddWithValue("@AutoDistributionRuleGuid", theRule.IdentityGuid);
							cmd.Parameters.AddWithValue("@SiteGuid", mySecurity.SiteGuid);
							idList = GetMapBusinessObject(mapType).EnumerateAssigned<BaseMapAssignedInfoDO>(mySecurity, theRule.IdentityGuid, cmd);
						}
						break;
					default:
						idList = GetMapBusinessObject(mapType).EnumerateAssigned<BaseMapAssignedInfoDO>(mySecurity, theRule.IdentityGuid);
						break;
				}
				idList = idList.OrderBy(data => data.ID).ToList<BaseMapAssignedInfoDO>();
				assignedDOList.Add(mapType, idList);
			}

			theRule.ManagerListText = ListToString(assignedDOList[AutoDistributionRuleChildMapTypes.ManagerGroup], assignedDOList[AutoDistributionRuleChildMapTypes.Manager]);
			theRule.OwnerListText = ListToString(assignedDOList[AutoDistributionRuleChildMapTypes.OwnerGroup], assignedDOList[AutoDistributionRuleChildMapTypes.Owner]);
			theRule.ProductListText = ListToString(assignedDOList[AutoDistributionRuleChildMapTypes.ProductGroup], assignedDOList[AutoDistributionRuleChildMapTypes.Product]);
			theRule.TransactionAliasListText = ListToString(assignedDOList[AutoDistributionRuleChildMapTypes.TransactionAlias], null);
		}

		/// <summary>
		/// Wrapper for the SecurityCheck method
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		private void SecurityCheckForRead(SecurityClass mySecurity)
		{
			SecurityCheck(mySecurity, true);
		}

		/// <summary>
		/// Wrapper for the SecurityCheck method
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		private void SecurityCheckForUpdate(SecurityClass mySecurity)
		{
			SecurityCheck(mySecurity, false);
		}

		/// <summary>
		/// Checks whether 
		///   the security object is valid
		///   the caller has rights to perform the desired operation
		/// </summary>
		/// <param name="mySecurity">caller's security</param>
		/// <param name="isViewOnly">type of operation</param>
		private void SecurityCheck(SecurityClass mySecurity, bool isViewOnly)
		{
			Validate(mySecurity);

			bool hasRight = isViewOnly ?
							mySecurity.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) :
							mySecurity.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION);

			if (hasRight == false)
			{
				throw new FMInsufficientRightsException();
			}

		}

		/// <summary>
		/// Validate SecurityClass object
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		private static void Validate(SecurityClass mySecurity)
		{
			if (mySecurity == null)
			{
				throw new ArgumentNullException(MessageInvalidSecurity);
			}
		}
		/// <summary>
		/// Validate SecurityClass object
		/// </summary>
		/// <param name="bdObject">Object to be validated</param>
		private static void Validate(BaseDataObject bdObject)
		{
			if (bdObject == null)
			{
				throw new ArgumentNullException(MessageInvalidBaseDataObject);
			}
		}

		/// <summary>
		/// Validate AutoDistributionRuleDO object
		/// </summary>
		/// <param name="theRule">Rule to be validated</param>
		private static void Validate(AutoDistributionRuleDO theRule)
		{
			if (theRule == null)
			{
				throw new ArgumentException(MessageInvalidEntity);
			}
		}

		/// <summary>
		/// Validates the given Rule
		/// customers for a given product.
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="theRule">Rule to be validated</param>
		private void Validate(SecurityClass mySecurity, AutoDistributionRuleDO theRule)
		{
			Validate(mySecurity);
			Validate(theRule);

			if (string.IsNullOrWhiteSpace(theRule.ID))
			{
				throw new ApplicationException(MessageIDRequired);
			}

			if (InternalClasses.Common.IsReservedWordForID(mySecurity, theRule.ID))
			{
				throw new ApplicationException( string.Format( MessageReservedWord, theRule.ID ) );
			}

			// check whether there is an existing Rule with the same ID
			Guid existingGuid = GetIdentityGuid(mySecurity, theRule.ID);

			if (existingGuid.IsNotEmptyAndNotEqualTo(theRule.IdentityGuid))
			{
				throw new ApplicationException( MessageExists );
			}

			// Make sure that a Distribution Transaction Alias and Default Reason Code were specified. 
			// You will get a foreign key violation if they are not provided.
			if (theRule.TransactionAliasGuid == Guid.Empty)
			{
				throw new ApplicationException( "You must specify a Distribution Transaction Alias" );
			}

			if (theRule.DefaultReasonCodeGuid == Guid.Empty)
			{
				throw new ApplicationException( "You must specify a Default Reason Code" );
			}
		}

		private void PostValidate(SecurityClass mySecurity, AutoDistributionRuleDO theRule)
		{
			if (theRule.DefaultEOM)
			{
				ValidateDefaultEOM(mySecurity, mySecurity.SiteGuid, mySecurity.LoginSiteGuid, theRule.IdentityGuid, false);
			}
			
		}

		/// <summary>
		/// Make sure that there is only 1 Default EOM for the combination of manager/product
		/// We could do it before we save but it is much easier to do it afterwards.		
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="selectedSiteGuid">Site selected by the user</param>
		/// <param name="loginSiteGuid">Site the user logged into</param>
		/// <param name="RuleGuid">Rule to be validated</param>
		/// <param name="isAssigningToSite">Are we validating for saving or for assigning entity</param>
		private void ValidateDefaultEOM(SecurityClass mySecurity, Guid selectedSiteGuid, Guid loginSiteGuid, Guid RuleGuid, bool isAssigningToSite=false)
		{
			Validate(mySecurity);			

			using (SqlCommand cmd = new SqlCommand())
			{
				AutoDistributionRuleDAC.PrepareFindDuplicateDefaultEOMSqlCommand(cmd, selectedSiteGuid, loginSiteGuid, RuleGuid);

				DataTable resultTable = this.consolidatedDA.GetDataTable(cmd, mySecurity);
				if (resultTable != null)
				{
					if (resultTable.Rows.Count > 0)
					{
						List<string> duplicateRuleList = new List<string>();
						foreach (DataRow currentRow in resultTable.Rows)
						{
							duplicateRuleList.Add(AutoDistributionRuleDAC.LoadDuplicateDefaultEOMInfo(currentRow));
						}

						string errorDescription;

						if (isAssigningToSite)
						{
							// Get the Site
							SitesClass siteService = new SitesClass();
							SiteClass theSite = siteService.Get(mySecurity, selectedSiteGuid, false);
						
							errorDescription = string.Format("This rule cannot be assigned to the site({0})", theSite.ID);
						}
						else
						{
							errorDescription = "This rule cannot be saved";
						}

						string[] duplicateRuleStrings = duplicateRuleList.ToArray<string>();
						string msg = string.Format("{0} because setting the rule to be the default EOM " +
													"conflicts with the following rule(manager/product): {1}", 
													errorDescription, string.Join(", ", duplicateRuleStrings));
						throw new ApplicationException(msg);
					}
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// This is the business class for auto distribution rule map.
	/// </summary>
	internal class AutoDistributionRuleMaps : BaseMaps<AutoDistributionRuleMapDO, AutoDistributionRuleMapDOCollection>
	{
		public AutoDistributionRuleMaps(BaseMapDAC srcMapClass)
			: base(srcMapClass)
		{
		}
	}
}