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
	public interface ICloseoutSiteProcessor
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		DataObject Process ( CloseoutSiteSR sr );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ProcessForSite ( CloseoutSiteSR sr );

        /// <summary>
        /// Create closeouts for the provided product, manager, and site
        /// </summary>
        /// <param name="closeoutSiteSR">Contains the site to process closeouts for as well as security information</param>
        /// <param name="manager">The manager to process closeouts for</param>
        /// <param name="product">The product to process closeouts for</param>
        /// <param name="closeoutDate">The date to closeout. Closeouts will be created for each end of month between the previous closeout and this date.</param>       
        /// <param name="siteShortDatePattern">The site's short date pattern. This will be used if we have to create an alarm and event record
        /// when there are no physical inventory transactions</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void ProcessForSiteManagerAndProduct(CloseoutSiteSR closeoutSiteSR, 
            CompanyClass manager, ProductClass product, DateTime closeoutDate, 
            string siteShortDatePattern);
	}
}
