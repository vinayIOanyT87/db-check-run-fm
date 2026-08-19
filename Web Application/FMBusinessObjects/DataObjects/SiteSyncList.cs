namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using FMBusinessObjects.BusinessInterfaces;

    /// <summary>
    /// SiteCollection provides a hierarchical collection of Sites with each Site associated with a specific Node Level.  
    /// </summary>
    /// <remarks>
    /// This collection is primarily populated starting with a specific Site or SiteGroup at Node Level zero (0).  Immediate parent(s) of 
    /// the starting Site/SiteGroup would be located at Node Level -1, continuing with parent(s) of these located at Node Level -2.
    /// Immediate member Sites/SiteGroups of the Site/SiteGroup located at Level zero (0) would be located at Node Level +1, continuing down until 
    /// the leaf nodes (Sites) are reached.
    /// </remarks>
    /// <example>
    /// Assuming the SiteCollection was populated with the following Sites/SiteGroups: 
    /// 
    /// Node Level: -2, SiteGroups: SiteRegion
    /// Node Level: -1, SiteGroups: SiteGroupA, SiteGroupB
    /// Node Level:  0,  SiteGroup: SiteGroupC
    /// Node Level: +1,       Site: SiteA
    /// Node Level: +1,  SiteGroup: SiteGroupD
    /// Node Level: +2,       Site: SiteB
    /// 
    /// In this example, SiteGroupC is the Site/SiteGroup at the center of this hierarchy.  SiteGroupA and SiteGroupB are parents of SiteGroupC, which 
    /// means that SiteGroupC is a member of both of these SiteGroups.  Continuing up the hierarchy, SiteGroupA and SiteGroupB are both members of the SiteRegion SiteGroup.
    /// Moving down, SiteA and SiteGroupD are child/member Site/SiteGroup of SiteGroupC.  Continuing down the hierarchy, SiteB is a child/member Site of SiteGroupD.
    /// </example>
    [Serializable]
    [DataContract]
    [KnownType(typeof(SiteCollectionClass))]
    public class SiteCollection
    {
        private int _NodeLevel;
        private SiteCollectionClass _SiteList;

        [DataMember]
        public int NodeLevel
        {
            get { return (_NodeLevel); }
            set { _NodeLevel = value; }
        }

        [DataMember]
        public SiteCollectionClass SiteList
        {
            get { return (_SiteList); }
            set { _SiteList = value; }
        }

        public SiteCollection()
        {
            _SiteList = new SiteCollectionClass();
        }
    }

    /// <summary>
    /// SiteSyncList provides management functionality to maintain a list of Sites within a collection of <seealso cref="SiteCollection"/> instances.  Each SiteCollection contains one or more 
    /// Sites or SiteGroups that exist at the same Node Level.  Sites are classified as Reference Sites or Hosted Sites.  A Hosted Site is any Site whose NodeLevel is GTE 0 (current/children).  
    /// A Reference Site is any Site whose NodeLevel is LT than 0 (parents).
    /// </summary>
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SiteCollection))]
    public class SiteSyncList : List<SiteCollection>, ISiteSyncList
    {
        #region Public Methods
        /// <summary>
        /// Add the specified SiteClass to the Synchronization Collection at the specified Node Level
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="EntitySiteNode"></param>
        public void Add(int pNodeLevel, SiteClass pSite)
        {
            SiteCollection levelCollection = null;

            if (this.ContainsKey(pNodeLevel))
            {
                levelCollection = GetValue(pNodeLevel);
                levelCollection.SiteList.Add(pSite);
            }
            else
            {
                levelCollection = new SiteCollection();
                levelCollection.NodeLevel = pNodeLevel;
                levelCollection.SiteList.Add(pSite);

                this.Add(levelCollection);
            }
        }
        /// <summary>
        /// Merges the passed in collection into the current collection.  If the same node exists in both lists, the incoming entry is ignored.
        /// </summary>
        /// <param name="SynchronizationList"></param>
        public void Merge(SiteSyncList pSynchronizationList)
        {
            IEnumerator incomingList = pSynchronizationList.GetEnumerator();

            // Technically the SiteClass could appear in any level depending on how the consumer of this collection
            // is managing the list.  Go through each level and remove the SiteClass instance if found.
            while (incomingList.MoveNext())
            {
                SiteCollection entry = incomingList.Current as SiteCollection;

                foreach (SiteClass sc in entry.SiteList)
                {
                    // If we already have it, move on (it's a complete SiteClass so we'll keep the existing one.)
                    if (this.ContainsSite(sc))
                        continue;

                    // If the server list didn't have it, it either means it isn't defined on the server (which could happen if a new Member Site was created
                    // on the remote node) or the server did not have any server changes for that Site.
                    // We should add the entry we received from the client (which would be fully populated if it was a new Member Site) or an existing Member Site
                    // that's being synchronized.  If this was the first time we sync'd (new remote node), then the incoming SiteClass would be incomplete, we don't
                    // want to include it and if it didn't exist in the server list, then we shouldn't be synchronizing it anyways, right?
                    // 
                    if (this.ContainsKey(entry.NodeLevel))
                        this.GetValue(entry.NodeLevel).SiteList.Add(sc);
                    else
                    {
                        var newNode = new SiteCollection() { NodeLevel = entry.NodeLevel };
                        newNode.SiteList.Add(sc);
                        this.Add(newNode);
                    }
                }
            }
        }
        public SiteCollection GetValue(int pNodeLevel)
        {
            var scol = (from t in this.OfType<SiteCollection>()
                        where t.NodeLevel == pNodeLevel
                        select t).FirstOrDefault();

            return (scol);
        }
        public SiteClass FindSiteByID(string pSiteID)
        {
            var siteCollections = (from scol in this select scol.SiteList).OfType<SiteCollectionClass>();

            var sites = from s in
                            siteCollections.SelectMany(sc => sc.ToArray())
                        where s.ID == pSiteID
                        select s;

            if (null != sites && (sites.Count() > 0))
                return (sites.FirstOrDefault());
            else
                return (null);
        }
        public bool ContainsKey(int pNodeLevel)
        {
            var siteCollections = from sc in this.OfType<SiteCollection>()
                                  where sc.NodeLevel == pNodeLevel
                                  select sc;

            return (null != siteCollections && (siteCollections.Count() > 0));
        }
        public bool Contains(int pNodeLevel, SiteClass pSite)
        {
            if (!this.ContainsKey(pNodeLevel))
                return (false);

            var sites = from s in
                            ((from t in this.OfType<SiteCollection>()
                              where t.NodeLevel == pNodeLevel
                              select t).SelectMany(sc => sc.SiteList.ToArray()))
                        where s.SiteGuid == pSite.SiteGuid ||
                             s.ID == pSite.ID
                        select s;

            return (null != sites && (sites.Count() > 0));
        }
        public bool ContainsSite(SiteClass pSite)
        {
            var siteCollections = (from scol in this select scol.SiteList).OfType<SiteCollectionClass>();

            var sites = from s in
                            siteCollections.SelectMany(sc => sc.ToArray())
                        where s.SiteGuid == pSite.SiteGuid ||
                             s.ID == pSite.ID
                        select s;

            return (null != sites && (sites.Count() > 0));
        }
        public void Remove(SiteClass pSite)
        {
            List<int> emptyLevels = new List<int>();

            IEnumerator ssc = this.GetEnumerator();

            // Technically the SiteClass could appear in any level depending on how the consumer of this collection
            // is managing the list.  Go through each level and remove the SiteClass instance if found.
            while (ssc.MoveNext())
            {
                SiteCollection entry = ssc.Current as SiteCollection;

                var sc = (from t in entry.SiteList
                          where t.ID == pSite.ID ||
                                  t.SiteGuid == pSite.SiteGuid
                          select t).FirstOrDefault();

                if (null != sc)
                    entry.SiteList.Remove(sc);

                if (entry.SiteList.Count() == 0)
                    emptyLevels.Add(entry.NodeLevel);
            }

            // Remove any empty Synchronization Node Levels
            if (emptyLevels.Count > 0)
            {
                var removalList = from u in this.OfType<SiteCollection>()
                                  where emptyLevels.Contains(u.NodeLevel)
                                  select u;

                if (null != removalList && (removalList.Count() > 0))
                {
                    foreach (var scol in removalList)
                        this.Remove(scol);
                }
            }
        }
        public void Remove(SiteClass pSite, int pNodeLevel)
        {
            if (!this.ContainsKey(pNodeLevel))
                return;

            SiteCollection levelCollection = this.GetValue(pNodeLevel);

            levelCollection.SiteList.Remove(pSite);

            if (levelCollection.SiteList.Count == 0)
            {
                this.Remove(levelCollection);
            }
        }
        public SiteCollectionClass EnumerateAllSitesList()
        {
            IEnumerable<SiteClass> sites = this.SelectMany(col => col.SiteList);

            SiteCollectionClass syncCollection = new SiteCollectionClass();
            syncCollection.AddRange(sites);

            return (syncCollection);
        }
        public SiteCollectionClass EnumerateReferenceSitesList()
        {
            IEnumerable<SiteClass> sites = this.Where(sc => sc.NodeLevel < 0).OrderBy(coll => coll.NodeLevel).SelectMany(coll => coll.SiteList);

            SiteCollectionClass syncCollection = new SiteCollectionClass();
            syncCollection.AddRange(sites);

            return (syncCollection);
        }
        public SiteCollectionClass EnumerateHostedSitesList()
        {
            IEnumerable<SiteClass> sites = this.Where(sc => sc.NodeLevel >= 0).OrderBy(coll => coll.NodeLevel).SelectMany(coll => coll.SiteList);

            SiteCollectionClass syncCollection = new SiteCollectionClass();
            syncCollection.AddRange(sites);

            return (syncCollection);
        }
        public SiteCollectionClass EnumerateInsertUpdateSynchronizationList(SYNCSCOPETYPE pScopeType)
        {
            SiteCollectionClass col = null;

            // The Insert Update order is the default order.
            switch (pScopeType)
            {
                case SYNCSCOPETYPE.BOTH:
                    col = EnumerateAllSitesList();
                    break;
                case SYNCSCOPETYPE.REFERENCE_ONLY:
                    col = EnumerateReferenceSitesList();
                    break;
                case SYNCSCOPETYPE.HOSTED_ONLY:
                    col = EnumerateHostedSitesList();
                    break;
                default:
                    col = new SiteCollectionClass();
                    break;
            }

            return (col);
        }
        public SiteCollectionClass EnumerateDeleteSynchronizationList(SYNCSCOPETYPE pScopeType)
        {
            SiteCollectionClass col = null;

            // The Insert Update order is the default order.
            switch (pScopeType)
            {
                case SYNCSCOPETYPE.BOTH:
                    col = EnumerateAllSitesList();
                    break;
                case SYNCSCOPETYPE.REFERENCE_ONLY:
                    col = EnumerateReferenceSitesList();
                    break;
                case SYNCSCOPETYPE.HOSTED_ONLY:
                    col = EnumerateHostedSitesList();
                    break;
                default:
                    col = new SiteCollectionClass();
                    break;
            }

            col.Reverse();

            return (col);
        }
        #endregion Public Methods
    }
}
