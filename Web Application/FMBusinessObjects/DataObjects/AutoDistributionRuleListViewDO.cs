///***************************************************************************
/// Module Name:  AutoDistributionRuleListViewDO
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
    using FMCore;
	
	/// <summary>
	/// This is a data holder class to be used by Auto Distribution Rule Summary Page with our ListView
	/// Basically a wrapper to the rule data object
	/// </summary>
   [Serializable]
	public class AutoDistributionRuleListViewDO : BaseLineItemDO
	{
		private AutoDistributionRuleDO sourceData = null;
		public AutoDistributionRuleListViewDO(AutoDistributionRuleDO source)
		{
            source.ThrowIfNull("source");

			this.sourceData = source;
		}

		public AutoDistributionRuleDO Data { get { return this.sourceData; } }

		// The following are just wrapper properties
		public Guid IdentityGuid { get { return this.sourceData.IdentityGuid; } }
		public Guid SiteGuid { get { return this.sourceData.SiteGuid; } }
		public string RuleID { get { return this.sourceData.ID; } }
		public string Description { get { return this.sourceData.Description; } }
		public bool Enabled { get { return this.sourceData.Enabled; } }
		public bool DefaultEOM { get { return this.sourceData.DefaultEOM; } }
		public string DefaultNotes { get { return this.sourceData.DefaultNotes; } }

		public string ManagerList { get { return this.sourceData.ManagerListText; } }
		public string OwnerList { get { return this.sourceData.OwnerListText; } }
		public string ProductList { get { return this.sourceData.ProductListText; } }
		public string TransactionAliasList { get { return this.sourceData.TransactionAliasListText; } }

		public string DefaultReasonCodeString
		{
			get
			{
				string retValue = string.Empty;

				AutoDistributionReasonCodeClass reasonCode = this.sourceData.DefaultReasonCode;
				if (reasonCode != null)
				{
					retValue = reasonCode.Code;
				}
				return retValue;
			}
		}

		public string TransactionAliasName
		{
			get
			{
				string retValue = string.Empty;

				TransactionAliasClass alias = this.sourceData.TransactionAlias;
				if (alias != null)
				{
					retValue = alias.ID;
				}
				return retValue;
			}
		}
		#region Overrides
		public override string getSelectCommand() { return null; }
		public override string getDeleteCommand() { return null; }
		public override string getInsertCommand() { return null; }
		public override string getUpdateCommand() { return null; }
		#endregion Overrides
	}


	[Serializable]
	[CollectionDataContract]
	public class AutoDistributionRuleListViewDOCollection : BaseCollections
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoDistributionRuleListViewDOCollection"/> class.
		/// </summary>
		public AutoDistributionRuleListViewDOCollection()
		{
		}

		/// <summary>
		/// Constructor for AutoDistributionRuleListViewDO List
		/// </summary>
		/// <param name="sourceList"></param>
		public AutoDistributionRuleListViewDOCollection(IEnumerable<AutoDistributionRuleListViewDO> sourceList)
		{
			foreach (AutoDistributionRuleListViewDO sourceRule in sourceList)
			{
				this.Add(sourceRule);
			}
		}

		/// <summary>
		/// Constructor for AutoDistributionRuleDOCollection
		/// </summary>
		/// <param name="sourceList"></param>
		public AutoDistributionRuleListViewDOCollection(AutoDistributionRuleDOCollection sourceList)
		{
			foreach (AutoDistributionRuleDO sourceRule in sourceList)
			{
				this.Add(new AutoDistributionRuleListViewDO(sourceRule));
			}
		}

		public AutoDistributionRuleListViewDO this[Guid targetGuid]
		{
			get
			{
				AutoDistributionRuleListViewDO targetRuleDO = null;
				foreach (AutoDistributionRuleListViewDO ruleDO in this)
				{
					if (ruleDO.IdentityGuid == targetGuid)
					{
						targetRuleDO = ruleDO;
						break;
					}
				}
				return targetRuleDO;
			}
		}

		public List<AutoDistributionRuleListViewDO> RuleGenericList
		{
			get
			{
				var ret = this.Cast<AutoDistributionRuleListViewDO>().ToList();
				return ret;
			}
			set
			{
				this.Clear();
				foreach (var it in value)
				{
					this.Add(it);
				}
			}
		}
	}
}
