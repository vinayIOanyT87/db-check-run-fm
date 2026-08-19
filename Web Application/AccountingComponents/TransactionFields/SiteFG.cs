namespace TransactionFields
{
	using System;
	using System.Collections.Specialized;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	public class SiteFG : DropDownGenerator, IHeaderField
	{
		public SiteFG()
		{
		}

		public override string FieldID { get { return "Site"; } }

		public override HybridDictionary GetEntries()
		{
			var listEntries = new HybridDictionary();

			if (transContext.accountingSite.IsSiteGroup)
			{
				// If the alias is an Order type, include the site group in the list
				if ((transContext.aliasClass.TransTypeID == TransactionTypes.T17_Order) ||
				    (transContext.aliasClass.TransTypeID == TransactionTypes.T18_SupplyOrder))
				{
					listEntries.Add(transContext.accountingSite.CurrentSiteName, transContext.accountingSite.CurrentSiteName);
				}

				// This is a rare case.  Normally you would not create your own SecurityClass because it can cause serious
				// problems with systems with a hardware key that forces higher security with SQL Server.  In most cases
				// field generators should use the security object in the transContext to which they have access.
				// In this case, we need to create a security class and change it throughout a loop while evaluating aliases.
				var siteSecurity = new SecurityClass
				                   {
					                   UserID = this.transContext.security.UserID,
					                   Password = this.transContext.security.Password
				                   };

				siteSecurity.CloneRights(transContext.security);

				//For each site in the site group, add an entry in the drop-down list. But wait a minute.
				//If the transaction alias is not assigned to the site, don't put it in the drop-down list.
				foreach (Site site in transContext.accountingSite.SiteList)
				{
					siteSecurity.SiteGuid = site.IdentityGuid;

					Guid aliasGuid = this.GetIdentityGuidForAlias(siteSecurity, transContext.aliasClass.ID);

					if (aliasGuid == transContext.aliasClass.IdentityGuid)
					{
						if (listEntries.Contains(site.Name) == false)
						{
							listEntries.Add(site.Name, site.Name);
						}
					}
				}
			}
			else
			{
				listEntries.Add(transContext.accountingSite.CurrentSiteName,
					transContext.accountingSite.CurrentSiteName);
			}

			return listEntries;
		}

		private Guid GetIdentityGuidForAlias(SecurityClass siteSecurity, string key)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
																	 x =>
																	 x.GetIdentityGuid(siteSecurity, key)
																);
		}

		public override bool Editable
		{
			get
			{
				//Can't change site on an existing transaction.
				//If not a site group, there is nothing to change it to.
				if (transContext.accountingSite.IsSiteGroup)
				{
					if (transContext.mode == TransactionContext.Mode.Add)
					{
						return true;
					}
				}
				return false;
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Site;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{

			transaction.Site = newValue as string;
			if (transaction.Site == transContext.accountingSite.CurrentSiteName)
			{
				transaction.SiteGuid = transContext.accountingSite.CurrentSiteGuid;
			}
			else
			{
				Guid siteGuid = FMChannelHelper.MakeCall<ISites, Guid>(
																	 x =>
																	 x.GetIdentityGuid(transContext.security, transaction.Site)
																);

				transaction.SiteGuid = siteGuid;
			}

			OnFieldChanged();
		}
	}
}
