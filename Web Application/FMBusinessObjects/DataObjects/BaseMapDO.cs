///***************************************************************************
/// Module Name:  BaseMapDO
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Data;

    using FMCore;

    /// <summary>
    /// This is the data holding object/data container for the Map
    /// When we assign like a product to a site.  Product is the Assigned and Site is the Assignee
    /// </summary>
    [DataContract]
   [Serializable]
	public class BaseMapDO : BaseDataObject
	{

		[DataMember]
		public Guid AssigneeGuid { get; set; }

		[DataMember]
		public Guid AssignedGuid { get; set; }

		/// <summary>
		/// Load the info from srcRow into me
		/// </summary>
		/// <param name="myDACHelper"></param>
		/// <param name="srcRow"></param>
		public virtual void Load(BaseMapDAC myDACHelper, DataRow srcRow)
		{
            myDACHelper.ThrowIfNull("myDACHelper");
            srcRow.ThrowIfNull("srcRow");

			Reset();
			myDACHelper.Load(srcRow, this);
		}

		/// <summary>
		/// Compare to see whether two maps are equal
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			BaseMapDO targetMap = obj as BaseMapDO;
			if (targetMap == null)
			{
				return false;
			}
			return (this.AssignedGuid == targetMap.AssignedGuid) &&
					(this.AssigneeGuid == targetMap.AssigneeGuid);
		}

		public override int GetHashCode()
		{
			return (this.AssignedGuid.GetHashCode() +
					this.AssigneeGuid.GetHashCode());
		}
	}

   [Serializable]
   [CollectionDataContract]
	public class BaseMapDOCollection<MapDOType> : List<MapDOType> where MapDOType : BaseMapDO
	{
		public MapDOType this[Guid targetGuid]
		{
			get
			{
				MapDOType retValue = null;
				foreach (MapDOType theMapDO in this)
				{
					if (theMapDO.IdentityGuid == targetGuid)
					{
						retValue = theMapDO;
					}
				}
				return retValue;
			}
		}

		/// <summary>
		/// Remove the object with the given assigned Guid in the given list
		/// </summary>
		/// <param name="self"></param>
		/// <returns>True if empty and false otherwise</returns>
		public bool RemoveByAssignedGuid(Guid targetGuid)
		{
			MapDOType foundObject = this.Single<MapDOType>(currentObject => currentObject.AssignedGuid == targetGuid);
			return this.Remove(foundObject);
		}

	}

	/// <summary>
	/// This is a mini class/data container used by BaseMapDAC and BaseMapDO.  
	/// It is mainly to prepare a list of Assigned entities with just the Guid and their IDs.
	/// </summary>
	[DataContract]
   [Serializable]
	public class BaseMapAssignedInfoDO
	{		
		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public Guid AssignedGuid { get; set; }

	}
}
