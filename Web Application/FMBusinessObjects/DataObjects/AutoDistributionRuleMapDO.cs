///***************************************************************************
/// Module Name:  AutoDistributionRuleMapDO
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Auto Distribution Rule Map class.  
	/// A map can be from Manager Group, Manager, Owner, OwnerGroup, ProductGroup, Product or Transaction Alias
	/// </summary>
	[DataContract]
   [Serializable]
   public class AutoDistributionRuleMapDO : BaseMapDO
	{
	}

	[Serializable]
	[CollectionDataContract]
	public class AutoDistributionRuleMapDOCollection : BaseMapDOCollection<AutoDistributionRuleMapDO>
	{
		/// <summary>
		/// Test whether this contains the given map (not reference equal, object equal comparison)
		/// </summary>
		/// <param name="targetMap">Map to be searched</param>
		/// <param name="foundRuleMap">The actual map in the list</param>
		/// <returns>True if found</returns>
		public bool FindMap(BaseMapDO targetMap, out AutoDistributionRuleMapDO foundRuleMap)
		{
			foundRuleMap = null;
			List<AutoDistributionRuleMapDO> foundList = this.FindAll(dataObject => dataObject.Equals(targetMap));

			bool found = foundList.Count > 0;

			if (found)
			{
				foundRuleMap = foundList[0];
			}
			return found;
		}

		/// <summary>
		/// Test whether this has any map with the given assigned Guid
		/// </summary>
		/// <param name="targetGuid">Assigned Guid to be found</param>
		/// <returns>True if found</returns>
		public bool ContainsAssignedGuid(Guid targetGuid)
		{
			return this.Exists(dataObject => dataObject.AssignedGuid == targetGuid);

		}		
	}
}
