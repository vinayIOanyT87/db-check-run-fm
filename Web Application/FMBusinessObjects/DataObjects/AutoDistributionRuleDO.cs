///***************************************************************************
/// Module Name:  AutoDistributionRuleDO
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
	/// Types of Auto Distribution Rule child map
	/// </summary>
	public enum AutoDistributionRuleChildMapTypes
	{
		ManagerGroup = 0,
		Manager,
		OwnerGroup,
		Owner,
		ProductGroup,
		Product,
		TransactionAlias
	}

	/// <summary>
	/// A list of AutoDistributionRuleDO
	/// </summary>
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(AutoDistributionRuleDO))]
	public class AutoDistributionRuleDOCollection : List<AutoDistributionRuleDO>
	{
		/// <summary>
		/// Use this only if you know your list has unique Guid value
		/// Empty Guid is ok.  It typically happens for a short moment when we are adding a new one.
		/// </summary>
		/// <param name="targetGuid"></param>
		/// <returns></returns>
		public AutoDistributionRuleDO this[Guid targetGuid]
		{
			get
			{
				return this.Single<AutoDistributionRuleDO>(rule => rule.IdentityGuid == targetGuid);
			}
		}
	}

	/// <summary>
	/// This is the Rule Data Object class for Auto Distribution.
	/// This should not contain any sql or database access code.  If there is a neeed, please add to the DAC class which is AutoDistributionRuleDAC
	/// </summary>
	[DataContract]
   [Serializable]
   public class AutoDistributionRuleDO : BaseDataObject
	{
		#region Ctors and initialization
		/// <summary>
		/// Default Constructor
		/// </summary>
		public AutoDistributionRuleDO()
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

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public bool DefaultEOM { get; set; }

		[DataMember]
		public string DefaultNotes { get; set; }

		[DataMember]
		public Guid TransactionAliasGuid { get; set; }

		[DataMember]
		public Guid DefaultReasonCodeGuid { get; set; }

		[DataMember]
		public TransactionAliasClass TransactionAlias { get; set; }

		[DataMember]
		public AutoDistributionReasonCodeClass DefaultReasonCode { get; set; }

		[DataMember]
		public Dictionary<AutoDistributionRuleChildMapTypes, AutoDistributionRuleMapDOCollection> AllMapList { get; private set; }

		//The following are helper proerties to access individual element of the above collection
		public AutoDistributionRuleMapDOCollection ManagerGroupList { get { return AllMapList[AutoDistributionRuleChildMapTypes.ManagerGroup]; } }
		public AutoDistributionRuleMapDOCollection ManagerList { get { return AllMapList[AutoDistributionRuleChildMapTypes.Manager]; } }
		public AutoDistributionRuleMapDOCollection OwnerGroupList { get { return AllMapList[AutoDistributionRuleChildMapTypes.OwnerGroup]; } }
		public AutoDistributionRuleMapDOCollection OwnerList { get { return AllMapList[AutoDistributionRuleChildMapTypes.Owner]; } }
		public AutoDistributionRuleMapDOCollection ProductGroupList { get { return AllMapList[AutoDistributionRuleChildMapTypes.ProductGroup]; } }
		public AutoDistributionRuleMapDOCollection ProductList { get { return AllMapList[AutoDistributionRuleChildMapTypes.Product]; } }
		public AutoDistributionRuleMapDOCollection TransactionAliasList { get { return AllMapList[AutoDistributionRuleChildMapTypes.TransactionAlias]; } }

		// The following few prpoerties are for display purpose only.

		[DataMember]
		public string ManagerListText { get; set; }

		[DataMember]
		public string OwnerListText { get; set; }

		[DataMember]
		public string ProductListText { get; set; }

		[DataMember]
		public string TransactionAliasListText { get; set; }


		#endregion Public Data Members

		#region override properties

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.AUTODISTRIBUTION_RULE; }
			set { }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#endregion override properties

		/// <summary>
		/// Loads from the given srcObject
		/// </summary>
		/// <param name="o"></param>
		public override void Load(Object o)
		{
			if (typeof(DataRow).IsInstanceOfType(o))
			{
				Reset();
				AutoDistributionRuleDAC.Load(this, (DataRow)o);
			}
		}

		#region private methods

		/// <summary>
		/// This is basically reset.  
		/// This is created to follow the rule: CA2214: Do not call overridable methods in constructors
		/// </summary>
		private void ResetInternal()
		{
			_ID = string.Empty;
			Description = string.Empty;
			Enabled = true;
			DefaultEOM = false;
			DefaultNotes = string.Empty;
			TransactionAlias = new TransactionAliasClass();
			DefaultReasonCode = new AutoDistributionReasonCodeClass();
			AllMapList = new Dictionary<AutoDistributionRuleChildMapTypes, AutoDistributionRuleMapDOCollection>();
			AddMapDOCollection(AutoDistributionRuleChildMapTypes.ManagerGroup);
			AddMapDOCollection(AutoDistributionRuleChildMapTypes.Manager);
			AddMapDOCollection(AutoDistributionRuleChildMapTypes.OwnerGroup);
			AddMapDOCollection(AutoDistributionRuleChildMapTypes.Owner);
			AddMapDOCollection(AutoDistributionRuleChildMapTypes.ProductGroup);
			AddMapDOCollection(AutoDistributionRuleChildMapTypes.Product);
			AddMapDOCollection(AutoDistributionRuleChildMapTypes.TransactionAlias);
			base.Reset();
		}

		/// <summary>
		/// Add a new child MapDOCollection
		/// </summary>
		/// <param name="mapType"></param>
		private void AddMapDOCollection(AutoDistributionRuleChildMapTypes mapType)
		{
			AutoDistributionRuleMapDOCollection newDOCollection = new AutoDistributionRuleMapDOCollection();
			AllMapList.Add(mapType, newDOCollection);
		}
		#endregion private methods

		#region public methods
		/// <summary>
		/// Update all assignee Guids.  This is called after an insert.
		/// </summary>
		public void UpdateMapGuids()
		{
			foreach (AutoDistributionRuleMapDOCollection currentMapDOCollection in AllMapList.Values)
			{
				foreach (BaseMapDO currentMap in currentMapDOCollection)
				{
					currentMap.AssigneeGuid = this.IdentityGuid;
				}
			}
		}

		/// <summary>
		/// This return a list of all child map types
		/// </summary>
		public static Array AllMapTypes
		{
			get
			{
				return Enum.GetValues(typeof(AutoDistributionRuleChildMapTypes));
			}
		}
		#endregion public  methods
	}
}
