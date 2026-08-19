// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RightClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RightCollectionClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	/// <summary>
	/// The right collection class.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(RIGHT))]
	public class RightCollectionClass : List<RIGHT>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RightCollectionClass"/> class.
		/// </summary>
		public RightCollectionClass()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RightCollectionClass"/> class.
		/// </summary>
		/// <param name="capacity">
		/// The capacity.
		/// </param>
		public RightCollectionClass(int capacity)
			: base(capacity)
		{
		}

		/// <summary>
		/// The add unique.
		/// </summary>
		/// <param name="groupRights">
		/// The group rights.
		/// </param>
		public void AddUnique(RightCollectionClass groupRights)
		{
			foreach (RIGHT right in groupRights)
			{
				if (this.Contains(right) == false)
				{
					this.Add(right);
				}
			}
		}

		/// <summary>
		/// The right in collection.
		/// </summary>
		/// <param name="right">
		/// The right.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool RightInCollection(RIGHT right)
		{
			return this.Contains(right);
		}

		/// <summary>
		/// The clone.
		/// </summary>
		/// <returns>
		/// The <see cref="RightCollectionClass"/>.
		/// </returns>
		public RightCollectionClass Clone()
		{
			var newObject = new RightCollectionClass();

			foreach (RIGHT right in this)
			{
				newObject.Add(right);
			}

			return newObject;
		}
	}

    [Serializable]
    [CollectionDataContract]
    public class RightCollectionExtClass : List<RightClass>
    {
    }

    /// <summary>
    /// The right class.
    /// </summary>
    [Serializable]
	[DataContract]
	public class RightClass : BaseDataObject
	{
        [DataMember]
        public int RightIndex { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }


        /// <summary>
        /// The enumerate by user and site SQL.
        /// </summary>
        /// <param name="cmd">
        /// The SQL Command.
        /// </param>
        /// <param name="userGuid">
        /// The user GUID.
        /// </param>
        /// <param name="siteGuid">
        /// The site GUID.
        /// </param>
        /// <param name="expirationDate">The expiration date of the user group association</param>
        /// <param name="inTransaction">
        /// The transaction.
        /// </param>
        public void EnumerateByUserAndSiteSQL(SqlCommand cmd, Guid userGuid, Guid siteGuid, DateTime expirationDate, bool inTransaction)
		{
			cmd.CommandText =	"SELECT DISTINCT GRM.LookupRightIndex" +
								" FROM map.tblGroupToRight GRM " + BaseDataObject.SQLUpdateLock(inTransaction) +
								" INNER JOIN map.tblUserToGroup UGM " + BaseDataObject.SQLUpdateLock(inTransaction) + " ON GRM.GroupGuid = UGM.GroupGuid" +
								" INNER JOIN map.tblEntityUserGroupToSite EUGS ON EUGS.GroupGuid = UGM.GroupGuid" +
								" WHERE UGM.UserGuid = @UserGuid AND EUGS.SiteGuid = @SiteGuid AND UGM.SiteGuid = @SiteGuid AND GRM.LookupRightIndex NOT IN(94,95,112,188)" +
			
			// Commented out 2/15/2022 WCG per TFS Bug 143248 - Expiration Date added to UserGroupMapClass as part of merge but not
			// supported by UI revisions.  Regardless Administrator User to Administrator Group mapping wouldn't be expired or there
			// would be potentially no administrator remaining to administrate the system.  Also there is a capability to designate
			// an administrator when adding a site, and this user mapping would not exipire either.

			//					" AND UGM.ExpirationDate >= @ExpirationDate" + 
                                " AND (UGM.DenyADPermission = 0 OR UGM.DenyADPermission IS NULL)";

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = userGuid;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;
			cmd.Parameters.Add("@ExpirationDate", SqlDbType.DateTime).Value = expirationDate;
		}

		/// <summary>
		/// The enumerate by group SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL Command.
		/// </param>
		/// <param name="groupGuid">
		/// The group GUID.
		/// </param>
		/// <param name="inTransaction">
		/// The transaction.
		/// </param>
		public void EnumerateByGroupSQL(SqlCommand cmd, Guid groupGuid, bool inTransaction)
		{
			cmd.CommandText = "SELECT LookupRightIndex" +
				" FROM map.tblGroupToRight " + SQLUpdateLock(inTransaction) +
				" WHERE map.tblGroupToRight.GroupGuid = @GroupGuid AND LookupRightIndex NOT IN(94,95,112,188)";

			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier).Value = groupGuid;
		}
	}
}