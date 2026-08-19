namespace FMBusinessObjects.BusinessInterfaces
{
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface ISiteSyncList
	{
		[OperationContract]
		void Add(int pNodeLevel, SiteClass pSite);

		[OperationContract]
		void Merge(SiteSyncList pSynchronizationList);

		[OperationContract]
        void Remove(SiteClass pSite);

		[OperationContract]
        void Remove(SiteClass pSite, int pNodeLevel);

		[OperationContract]
        bool Contains(int pNodeLevel, SiteClass pSite);

        [OperationContract]
        SiteClass FindSiteByID(string pSiteID);

		[OperationContract]
        SiteCollectionClass EnumerateDeleteSynchronizationList(SYNCSCOPETYPE pScopeType);

		[OperationContract]
        SiteCollectionClass EnumerateInsertUpdateSynchronizationList(SYNCSCOPETYPE pScopeType);

        [OperationContract]
        SiteCollectionClass EnumerateAllSitesList();

        [OperationContract]
        SiteCollectionClass EnumerateReferenceSitesList();

        [OperationContract]
        SiteCollectionClass EnumerateHostedSitesList();

	}
}
