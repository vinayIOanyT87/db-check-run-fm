using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
	public class SiteInfoDO
	{
		#region private data members
		private SiteCollectionClass siteCollection;
		private List<SiteToSiteMapClass> siteToSiteMaps;
		#endregion

		#region Constructors
		public SiteInfoDO ( )
		{
			this.Init ( );
		}
		#endregion

		#region Properties
		[DataMember]
		public SiteCollectionClass SiteCollection
		{
			get { return this.siteCollection; }
			set { this.siteCollection = value; }
		}

		[DataMember]
		public List<SiteToSiteMapClass> SiteToSiteMaps
		{
			get { return this.siteToSiteMaps; }
			set { this.siteToSiteMaps = value; }
		}
		#endregion

		#region Public methods
		public SiteCollectionClass EnumerateByParentSite ( Guid siteGuid )
		{
			SiteCollectionClass sites = new SiteCollectionClass ( );

			foreach (SiteToSiteMapClass Map in this.siteToSiteMaps)
			{
				if (Map.ParentSiteGuid == siteGuid && Map.ChildSiteGuid != siteGuid)
				{
					// Add the child site to the collection
					sites.Add(this.GetSite(Map.ChildSiteGuid));
				}
			}

			return sites;
		}

		public SiteCollectionClass EnumerateParentSites(Guid siteGuid)
		{
			SiteCollectionClass sites = new SiteCollectionClass ( );

			foreach (SiteToSiteMapClass Map in this.siteToSiteMaps)
			{
				if (Map.ChildSiteGuid == siteGuid && Map.ParentSiteGuid != siteGuid)
				{
					sites.Add(this.GetSite(Map.ParentSiteGuid));
				}
			}

			return sites;
		}

		public SiteClass GetSite(Guid siteGuid)
		{
			foreach (SiteClass site in this.siteCollection)
			{
				if (site.SiteGuid == siteGuid)
				{
					return site;
				}
			}

			return null;
		}

		public string GetSiteID ( Guid siteGuid )
		{
			foreach (SiteClass site in this.siteCollection)
			{
				if (site.SiteGuid == siteGuid)
				{
					return site.ID;
				}
			}

			return "";
		}

		public Guid GetSiteGuid ( string SiteID )
		{
			foreach (SiteClass Site in this.siteCollection)
			{
				if (Site.ID.ToUpper ( ) == SiteID.ToUpper ( ))
				{
					return Site.SiteGuid;
				}
			}

			return Guid.Empty; //was -99
		}

		public void SortByGroupThenId(SiteCollectionClass sites)
		{
			sites.Sort(CompareSitesByGroupThenId);
		}

		#endregion

		#region Private methods
		private void Init ()
		{
			this.siteCollection = new SiteCollectionClass ( );
			this.siteToSiteMaps = new List<SiteToSiteMapClass> ( );
		}

		private static int CompareSitesByGroupThenId(SiteClass x, SiteClass y)
		{
			if (x.SiteGroup)
			{
				if (y.SiteGroup)
				{
					return string.Compare(x.ID, y.ID, StringComparison.InvariantCulture);
				}

				return -1;
			}

			if (y.SiteGroup)
			{
				return 1;
			}

			return string.Compare(x.ID, y.ID, StringComparison.InvariantCulture);
		}

		#endregion
	}
}
