///***************************************************************************
/// Module Name:  AutoDistributionRuleDAC
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMCore;

    /// <summary>
    /// Auto Distribution Rule Data Access Helper class
    /// This is the Data Access helper class which prepares SqlCommands and load into the Data Object class AutoDistributionRuleDO.
    /// This class should not really contain any data.
    /// This class is called mainly from the business class which is AutoDistributionRules
    /// </summary>
    public class AutoDistributionRuleDAC : BaseDataObject
	{
		#region constants
		private const string SqlSPPrefix = "dbo.usp_AutoDistributionRule";
		private const string SqlInsertSP = SqlSPPrefix + "InsertByRowGuid";
		private const string SqlDeleteSP = SqlSPPrefix + "DeleteApplication";
		private const string SqlUpdateSP = SqlSPPrefix + "UpdateByRowGuid";
		private const string SqlSelectSP = SqlSPPrefix + "Select";
		private const string SqlSelectManagersSP = SqlSPPrefix + "SelectManagers";
		private const string SqlSelectProductsSP = SqlSPPrefix + "SelectProducts";
		private const string SqlSelectOwnersSP = SqlSPPrefix + "SelectOwners";
		private const string SqlSelectByManagerProductTextSP = SqlSPPrefix + "SelectByManagerProductText";
		private const string SqlFindDuplicateEOMSP = SqlSPPrefix + "FindDuplicateDefaultEOM";

		#endregion

		#region public methods
		/// <summary>
		/// Loads from the given DataRow
		/// </summary>
		/// <param name="srcRow">source data row</param>
		public static AutoDistributionRuleDO Load(DataRow srcRow)
		{
            srcRow.ThrowIfNull("srcRow");

			AutoDistributionRuleDO theRule = new AutoDistributionRuleDO();
			Load(theRule, srcRow);
			return theRule;
		}

		/// <summary>
		/// Loads from the given DataRow
		/// </summary>
		/// <param name="theRule">Rule object to be loaded into</param>
		/// <param name="srcRow">DataRow which has the source data</param>
		public static void Load(AutoDistributionRuleDO theRule, DataRow srcRow)
		{
            theRule.ThrowIfNull("theRule");
            srcRow.ThrowIfNull("srcRow");

			theRule.IdentityGuid = DataObject.getValue<Guid>(srcRow["AutoDistributionRuleGuid"], Guid.Empty);
			theRule.SiteGuid = DataObject.getValue<Guid>(srcRow["SiteGuid"], Guid.Empty);
			theRule.ID = DataObject.getValue<string>(srcRow["RuleID"], string.Empty);
			theRule.Description = DataObject.getValue<string>(srcRow["RuleDescription"], string.Empty);
			theRule.Enabled = DataObject.getValue<bool>(srcRow["RuleEnabled"], false);
			theRule.DefaultEOM = DataObject.getValue<bool>(srcRow["DefaultEOM"], false);
			theRule.TransactionAliasGuid = DataObject.getValue<Guid>(srcRow["TransactionAliasGuid"], Guid.Empty);
			theRule.DefaultReasonCodeGuid = DataObject.getValue<Guid>(srcRow["DefaultReasonCodeGuid"], Guid.Empty);
			theRule.DefaultNotes = DataObject.getValue<string>(srcRow["DefaultNotes"], string.Empty);
		}

		/// <summary>
		/// The srcRow should have the duplicate rule.  This function generates the text about the rule.
		/// </summary>
		/// <param name="srcRow">Source date row</param>
		/// <returns>Display text about the rule including the manager and product</returns>
		public static string LoadDuplicateDefaultEOMInfo(DataRow srcRow)
		{
            srcRow.ThrowIfNull("srcRow");

			string ruleID = DataObject.getValue<string>(srcRow["RuleID"], string.Empty);
			string companyID = DataObject.getValue<string>(srcRow["CompanyID"], string.Empty);
			string productID = DataObject.getValue<string>(srcRow["ProductID"], string.Empty);
			return string.Format("{0}({1}/{2})", ruleID, companyID, productID);
		}

		#endregion public methods


		#region Methods with SQLs
		/// <summary>
		/// Prepares a SqlCommand to select from the Rule table.
		/// Filtering criteria will be included if specified.
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepread</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="bInTransaction">Are we in transaction?</param>
		/// <param name="ruleGuid">Select by Guid</param>
		/// <param name="ruleID">Select by RuleID</param>
		public static void PrepareSelectSqlCommand(SqlCommand cmd, SecurityClass security, bool bInTransaction, Guid ruleGuid, string ruleID)
		{
            cmd.ThrowIfNull("cmd");
            security.ThrowIfNull("security");

			cmd.CommandText = SqlSelectSP;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@SelectedSiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@LoginSiteGuid", security.LoginSiteGuid);
			cmd.Parameters.Add(DataObject.NewGuidParameter("@AutoDistributionRuleGuid", ruleGuid, true));
			cmd.Parameters.AddWithValue("@RuleID", string.IsNullOrWhiteSpace(ruleID) ? DBNull.Value : (object)ruleID);
		}

		/// <summary>
		/// Prepares a SqlCommand to select from the Rule table.
		/// Filtering criteria will be included if specified.
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="managerGuid">Manager Guid to be filtered by</param>
		/// <param name="productGuid">Product Guid to be filtered by</param>
		/// <param name="searchText">Text to be filtered by</param>
		public static void PrepareSelectSqlCommandByManagerProductDescription(SqlCommand cmd, SecurityClass security, Guid managerGuid, Guid productGuid, string searchText)
		{
            cmd.ThrowIfNull("cmd");
            security.ThrowIfNull("security");

            cmd.CommandText = SqlSelectByManagerProductTextSP;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@SelectedSiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@LoginSiteGuid", security.LoginSiteGuid);
			cmd.Parameters.Add(DataObject.NewGuidParameter("@ManagerGuid", managerGuid, true));
			cmd.Parameters.Add(DataObject.NewGuidParameter("@ProductGuid", productGuid, true));
			cmd.Parameters.AddWithValue("@FindText", searchText);
		}

		/// <summary>
		/// Prepares a SqlCommand for finding all managers including managers from the manager groups
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="selectedSiteGuid">The site the user has selected</param>
		/// <param name="loginSiteGuid">The site that the user has logged in from</param>
		/// <param name="ruleGuid">Rule Guid</param>
		public static void PrepareSelectManagersSqlCommand(SqlCommand cmd, Guid selectedSiteGuid, Guid loginSiteGuid, Guid ruleGuid)
		{
            cmd.ThrowIfNull("cmd");

            PrepareSqlWithLoginAndRuleGuid(cmd, SqlSelectManagersSP, selectedSiteGuid, loginSiteGuid, ruleGuid);
		}

		/// <summary>
		/// Prepares a SqlCommand for finding all products including products from the product groups
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="selectedSiteGuid">The site the user has selected</param>
		/// <param name="loginSiteGuid">The site that the user has logged in from</param>
		/// <param name="ruleGuid">Rule Guid</param>
		public static void PrepareSelectProductsSqlCommand(SqlCommand cmd, Guid selectedSiteGuid, Guid loginSiteGuid, Guid ruleGuid)
		{
            cmd.ThrowIfNull("cmd");

            PrepareSqlWithLoginAndRuleGuid(cmd, SqlSelectProductsSP, selectedSiteGuid, loginSiteGuid, ruleGuid);
		}

		/// <summary>
		/// Prepares a SqlCommand for finding all owners including owners from the owner groups
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="selectedSiteGuid">The site the user has selected</param>
		/// <param name="loginSiteGuid">The site that the user has logged in from</param>
		/// <param name="ruleGuid">Rule Guid</param>
		public static void PrepareSelectOwnersSqlCommand(SqlCommand cmd, Guid selectedSiteGuid, Guid loginSiteGuid, Guid ruleGuid)
		{
            cmd.ThrowIfNull("cmd");

            PrepareSqlWithLoginAndRuleGuid(cmd, SqlSelectOwnersSP, selectedSiteGuid, loginSiteGuid, ruleGuid);
		}

		/// <summary>
		/// Creates common parameters for Insert and Update operations
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="theRule">The rule to be inserted/updated</param>
		private static void AddCommonParameters(SqlCommand cmd, AutoDistributionRuleDO theRule)
		{
            cmd.ThrowIfNull("cmd");
            theRule.ThrowIfNull("theRule");

            cmd.Parameters.AddWithValue("@SiteGuid", theRule.SiteGuid);
			cmd.Parameters.AddWithValue("@RuleID", theRule.ID);
			cmd.Parameters.AddWithValue("@RuleDescription", theRule.Description);
			cmd.Parameters.AddWithValue("@RuleEnabled", theRule.Enabled);
			cmd.Parameters.AddWithValue("@DefaultEOM", theRule.DefaultEOM);
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", theRule.TransactionAliasGuid);
			cmd.Parameters.AddWithValue("@DefaultReasonCodeGuid", theRule.DefaultReasonCodeGuid);
			cmd.Parameters.AddWithValue("@DefaultNotes", theRule.DefaultNotes);
			cmd.Parameters.AddWithValue("@UpdatedDate", theRule.UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", theRule.UpdatedBy);
		}

		/// <summary>
		/// Prepares a SqlCommand for upate
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="theRule">The rule to be updated</param>
		public static void PrepareUpdateSqlCommand(SqlCommand cmd, SecurityClass security, AutoDistributionRuleDO theRule)
		{
            cmd.ThrowIfNull("cmd");
            security.ThrowIfNull("security");
            theRule.ThrowIfNull("theRule");

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = SqlUpdateSP;
			AddCommonParameters(cmd, theRule);
			cmd.Parameters.AddWithValue("@AutoDistributionRuleGuid", theRule.IdentityGuid);
		}

		/// <summary>
		/// Prepares a SqlCommand for Insert
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="theRule">The rule to be inserted</param>
		public static void PrepareInsertSqlCommand(SqlCommand cmd, SecurityClass security, AutoDistributionRuleDO theRule)
		{
            cmd.ThrowIfNull("cmd");
            security.ThrowIfNull("security");
            theRule.ThrowIfNull("theRule");

            cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = SqlInsertSP;
			AddCommonParameters(cmd, theRule);
			cmd.Parameters.AddWithValue("@CreatedDate", theRule.UpdatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", theRule.UpdatedBy);
			DataObject.AddGuidOutputParameter(cmd, "@AutoDistributionRuleGuid");
		}

		/// <summary>
		/// Prepares a SqlCommand for Delete
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="security">Security object used by FM</param>
		/// <param name="ruleGuid">The Guid of the rule to be deleted</param>
		public static void PrepareDeleteSqlCommand(SqlCommand cmd, SecurityClass security, Guid ruleGuid)
		{
            cmd.ThrowIfNull("cmd");
            security.ThrowIfNull("security");

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = SqlDeleteSP;
			cmd.Parameters.AddWithValue("@AutoDistributionRuleGuid", ruleGuid);
		}

		/// <summary>
		/// Prepares a SqlCommand for finding rules with duplicate EOM for the same manager/product combination
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="selectedSiteGuid">The site the user has selected</param>
		/// <param name="loginSiteGuid">The site that the user has logged in from</param>
		/// <param name="ruleGuid">Rule Guid</param>
		public static void PrepareFindDuplicateDefaultEOMSqlCommand(SqlCommand cmd, Guid selectedSiteGuid, Guid loginSiteGuid, Guid ruleGuid)
		{
            cmd.ThrowIfNull("cmd");

            PrepareSqlWithLoginAndRuleGuid(cmd, SqlFindDuplicateEOMSP, selectedSiteGuid, loginSiteGuid,	ruleGuid);
		}

		/// <summary>
		/// Prepares a SqlCommand with Site Guids and rule Guid.  Shared method to create common parameters
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="storedProcName">Name of the stored procedure</param>
		/// <param name="selectedSiteGuid">The site the user has selected</param>
		/// <param name="loginSiteGuid">The site that the user has logged in from</param>
		/// <param name="ruleGuid">Rule Guid</param>
		private static void PrepareSqlWithLoginAndRuleGuid(SqlCommand cmd, string storedProcName, Guid selectedSiteGuid, Guid loginSiteGuid, Guid ruleGuid)
		{
            cmd.ThrowIfNull("cmd");

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = storedProcName;

			cmd.Parameters.AddWithValue("@SelectedSiteGuid", selectedSiteGuid);
			cmd.Parameters.AddWithValue("@LoginSiteGuid", loginSiteGuid);

			cmd.Parameters.AddWithValue("@AutoDistributionRuleGuid", ruleGuid);
		}
		#endregion
	}
}
