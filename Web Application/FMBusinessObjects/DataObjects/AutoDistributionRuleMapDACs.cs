///***************************************************************************
/// Module Name:  AutoDistributionRuleMapDACs
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Data;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// This is the data access helper class for AutoDistributionRuleMap
	/// </summary>
	public class AutoDistributionRuleMapDACs
	{
		public static Dictionary<AutoDistributionRuleChildMapTypes, BaseMapDAC> MapDACList { get; private set; }

		//The following are helper proerties to access individual element of the above collection
		public BaseMapDAC ManagerGroupList { get { return MapDACList[AutoDistributionRuleChildMapTypes.ManagerGroup]; } }
		public BaseMapDAC ManagerList { get { return MapDACList[AutoDistributionRuleChildMapTypes.Manager]; } }
		public BaseMapDAC OwnerGroupList { get { return MapDACList[AutoDistributionRuleChildMapTypes.OwnerGroup]; } }
		public BaseMapDAC OwnerList { get { return MapDACList[AutoDistributionRuleChildMapTypes.Owner]; } }
		public BaseMapDAC ProductGroupList { get { return MapDACList[AutoDistributionRuleChildMapTypes.ProductGroup]; } }
		public BaseMapDAC ProductList { get { return MapDACList[AutoDistributionRuleChildMapTypes.Product]; } }
		public BaseMapDAC TransactionAliasList { get { return MapDACList[AutoDistributionRuleChildMapTypes.TransactionAlias]; } }

		/// <summary>
		/// Add a new MapDAC
		/// </summary>
		/// <param name="mapType">Type of the child map</param>
		/// <param name="assignedName">Name of the assigned</param>
		/// <param name="assignedIDFieldName">Field Name used for the Assigned's ID</param>
		public static void AddMapConfig(AutoDistributionRuleChildMapTypes mapType, string assignedName, string assignedIDFieldName)
		{
			MapDACConfig newConfig = new MapDACConfig("AutoDistributionRule", assignedName, assignedIDFieldName);
			BaseMapDAC newDAC = new BaseMapDAC(newConfig);
			MapDACList.Add(mapType, newDAC);
		}

		/// <summary>
		/// static constuctor, just to create the singleton MapDACList
		/// </summary>
		static AutoDistributionRuleMapDACs()
		{
			MapDACList = new Dictionary<AutoDistributionRuleChildMapTypes, BaseMapDAC>();
			AddMapConfig(AutoDistributionRuleChildMapTypes.ManagerGroup, "ManagerGroup", "ID");
			AddMapConfig(AutoDistributionRuleChildMapTypes.Manager, "Manager", "ID");
			AddMapConfig(AutoDistributionRuleChildMapTypes.OwnerGroup, "OwnerGroup", "ID");
			AddMapConfig(AutoDistributionRuleChildMapTypes.Owner, "Owner", "ID");
			AddMapConfig(AutoDistributionRuleChildMapTypes.ProductGroup, "ProductGroup", "ID");
			AddMapConfig(AutoDistributionRuleChildMapTypes.Product, "Product", "ProductID");
			AddMapConfig(AutoDistributionRuleChildMapTypes.TransactionAlias, "TransactionAlias", "AliasName");
		}
	}
}
