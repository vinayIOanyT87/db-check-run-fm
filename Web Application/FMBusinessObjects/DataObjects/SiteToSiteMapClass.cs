// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteToSiteMapClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Site to site mappings
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	[Serializable]
	[CollectionDataContract]
	public class SiteToSiteMapCollectionClass : List<SiteToSiteMapClass>
	{
	}

	/// <summary>
	/// Site to site mappings
	/// </summary>
	[Serializable]
	[DataContract]
	public class SiteToSiteMapClass : BaseDataObject
	{
		#region Constants and Fields

		[DataMember]
		public bool ChildGroup;

		[DataMember]
		public Guid ChildSiteGuid;

		[DataMember]
		public string ChildSiteID;

		[DataMember]
		public Guid ParentSiteGuid;

		[DataMember]
		public string ParentSiteID;

		#endregion

		#region Constructors and Destructors

		public SiteToSiteMapClass()
		{
			this.Reset();
		}

		#endregion

		#region Public Properties

		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.SITE_TO_SITE;
			}
		}

		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		#endregion

		#region Public Methods and Operators

		public override void Reset()
		{
			base.Reset();
			this.ParentSiteGuid = Guid.Empty;
			this.ChildSiteGuid = Guid.Empty;
			this.ParentSiteID = "";
			this.ChildSiteID = "";
			this.ChildGroup = false;
		}

		#endregion
	}
}
