// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupClass.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Constants;

	/// <summary>
	///	  A collection object for groups of GroupClass objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class GroupCollectionClass : List<GroupClass>
	{
	}

	/// <summary>
	///	  Data object describing a user group.
	/// </summary>
	[DataContract]
	[Serializable]
	[KnownType(typeof(CompanyMapClass))]
    [KnownType(typeof(CompanyMapAuthorizedCarrierClass))]
    [KnownType(typeof(CompanyMapBillToShipperClass))]
    [KnownType(typeof(CompanyMapCompanyGroupCompanyClass))]
    [KnownType(typeof(CompanyMapFootNoteShipperClass))]
    [KnownType(typeof(CompanyMapFootNoteShipToClass))]
    [KnownType(typeof(CompanyMapLoadIdShipToClass))]
    [KnownType(typeof(CompanyMapLoadOwnerManagerClass))]
    [KnownType(typeof(CompanyMapOffloadIdSupplierClass))]
    [KnownType(typeof(CompanyMapOffloadOwnerManagerClass))]
    [KnownType(typeof(CompanyMapPersonAssignedCompanyClass))]
    [KnownType(typeof(CompanyMapShipperOwnerClass))]
    [KnownType(typeof(CompanyMapShipToBillToClass))]
    [KnownType(typeof(CompanyMapSupplierOwnerClass))]
    [KnownType(typeof(CompanyMapUserGroupCompanyClass))]
    [XMLObject(NodeName = "Group")]
	public class GroupClass : BaseDataObject, IEquatable<GroupClass>
	{
		#region Constants

		public const string ENTITY_TYPE_ID = "User Groups";

		#endregion

		#region Fields

		[DataMember]
		public CompanyMapCollectionClass CompanyMapCollection;

		[DataMember]
		public RightCollectionClass RightCollection;

        [DataMember]
        public RightCollectionExtClass RightCollectionExt;

        [DataMember]
		public UserGroupMapCollectionClass UserGroupMapCollection;

		[DataMember]
		private string description = string.Empty;

		[DataMember]
		private int sessionTimeout;

		[DataMember]
		private DateTime assignedExpirationDate = DateTime.Today;

        [DataMember]
        private Guid activeDirectoryUserGroupGuid;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupClass"/> class.
		/// </summary>
		public GroupClass()
		{
			this.Initialize();
		}

		#endregion

		#region Public Properties

		/// <summary>
		///	  Get or set the description of the user group.
		///	  When setting the description, make sure the string is not longer than the
		///	  length of the database column
		/// </summary>
		public string Description
		{
			get
			{
				return this.description;
			}

			set
			{
				this.SetString("Description", 80, value, ref this.description);
			}
		}

		public int SessionTimeout
		{
			get
			{
				return sessionTimeout;
			}
			set
			{
				sessionTimeout = value;
			}
		}

		/// <summary>
		///	  Get or set the Assigned Expiration Date of the user group.
		/// </summary>
		public DateTime AssignedExpirationDate
		{
			get
			{
				return this.assignedExpirationDate;
			}

			set
			{
				this.assignedExpirationDate = value;
			}
		}

	    public Guid ActiveDirectoryUserGroupGuid
	    {
            get { return this.activeDirectoryUserGroupGuid; }
            set { this.activeDirectoryUserGroupGuid = value;  }
	    }

		/// <summary>
		///	  Gets the type of the entity.
		/// </summary>
		/// <value>
		///	  The type of the entity.
		/// </value>
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.GROUP;
			}
		}

		/// <summary>
		/// The XMLProperty attribute here is used by the Query Writer to save assigned groups
		/// </summary>
		[QueryWriterField("ID", "GroupID")]
		[XMLProperty]
		public override string ID
		{
			get
			{
				return this._ID;
			}

			set
			{
				string temp = value;

				if (string.IsNullOrEmpty(temp) == false)
				{
					temp = temp.Trim();
				}

				this.SetString("ID", 30, temp, ref this._ID);
			}
		}

		/// <summary>
		///	  Gets a value indicating whether this instance is admin group.
		/// </summary>
		/// <value>
		///	  <c>true</c> if this instance is admin group; otherwise, <c>false</c>.
		/// </value>
		public bool IsAdminGroup
		{
			get
			{
				return IsAdminGroupGuid(this.IdentityGuid);
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Determines whether [is admin group GUID] [the specified group GUID].
		/// </summary>
		/// <param name="groupGuid">
		/// The group GUID.
		/// </param>
		/// <returns>
		/// <c>true</c> if [is admin group GUID] [the specified group GUID]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsAdminGroupGuid(Guid groupGuid)
		{
			return groupGuid == Guids.GroupAdminGuid;
		}

		public bool Equals(GroupClass other)
		{
			if (other == null)
			{
				return false;
			}

			return other.IdentityGuid == this.IdentityGuid;
		}

		public override void Reset()
		{
			base.Reset();
			this.Initialize();
		}

		#endregion

		#region Methods

		/// <summary>
		///	  Initializes this instance.
		/// </summary>
		private void Initialize()
		{
			this.description = string.Empty;
			this.sessionTimeout                 = 5;
			this.AssignedExpirationDate         = DateTime.Today.AddYears(1).Date;
			this.RightCollection                = new RightCollectionClass();
			this.UserGroupMapCollection         = new UserGroupMapCollectionClass();
			this.CompanyMapCollection           = new CompanyMapCollectionClass();
            this.activeDirectoryUserGroupGuid   = Guid.Empty;
            this.RightCollectionExt             = new RightCollectionExtClass();
		}

		#endregion
	}
}