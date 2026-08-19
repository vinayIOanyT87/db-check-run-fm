using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class TransactionAliasListProcessorClass : ITransactionAliasListProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public TransactionAliasListProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		public TransactionAliasListDO Process ( TransactionAliasListSR sr )
		{
			return this.GetTransactionAliases ( sr );
		}

		private TransactionAliasListDO GetTransactionAliases ( TransactionAliasListSR sr )
		{
			TransactionAliasListDO transAliasListDO = new TransactionAliasListDO ( );

			TransactionAliasesClass transAliases = new TransactionAliasesClass ( );
			TransactionAliasCollectionClass aliasCollection = transAliases.Enumerate ( sr.Security );

			SitesInfoClass siteInfo = new SitesInfoClass ( );
			SiteInfoDO siteInfoDO = new SiteInfoDO ( );

			if (sr.GetOwnerSiteID == true)
			{
				siteInfoDO = siteInfo.RefreshSiteInfo ( sr.Security );
			}

			foreach (TransactionAliasClass aliasClass in aliasCollection)
			{
				if (sr.TransType == 0 || sr.TransType == (short) aliasClass.TransTypeID)
				{
					TransactionAliasDO aliasDO	= new TransactionAliasDO ( );
					aliasDO.TransactionAliasGuid				= aliasClass.IdentityGuid;
					aliasDO.AliasName			= aliasClass.ID;
					aliasDO.Bulk				= aliasClass.BulkShipment;
					aliasDO.DistributedImpact	= aliasClass.DistributedImpact;

					if (sr.GetOwnerSiteID == true)
					{
						aliasDO.SiteOwner = siteInfoDO.GetSiteID ( aliasClass.SiteGuid );
					}

					aliasDO.TransactionTypeID = (TransactionTypes) aliasClass.TransTypeID;
					aliasDO.TwentyFourHr = aliasClass.MeterCloseout;

					transAliasListDO.aliasList.Add ( aliasClass.ID, aliasClass );
				}
			}

			return transAliasListDO;
		}
	}
}
