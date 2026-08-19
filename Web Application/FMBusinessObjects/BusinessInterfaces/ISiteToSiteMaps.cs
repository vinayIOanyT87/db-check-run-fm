using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;


namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface ISiteToSiteMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, SiteToSiteMapClass SiteToSiteMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid ParentSiteGuid, Guid ChildSiteGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<Guid, object> GetSiteHierarchy(SecurityClass security, bool ignoreEnterprise = false);

		/// <summary>
		/// Gets the maximum site to site map row version.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns></returns>
		[OperationContract]
		Int64? GetMaxSiteToSiteMapRowVersion(SecurityClass security);


	}
}
