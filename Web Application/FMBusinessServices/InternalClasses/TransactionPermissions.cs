// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionPermissions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///     Summary description for TransactionPermissions.
	/// </summary>
	public class TransactionPermissions
	{
		#region Constants and Fields
		private readonly AccountingSite accountingSite;
		#endregion

		#region Constructors and Destructors
		public TransactionPermissions(AccountingSite accountingSite)
		{
			this.accountingSite = accountingSite;
		}
		#endregion

		#region Public Methods and Operators
		public bool TransactionIsFromUserSites(TransactionDO trans)
		{
			foreach (Site site in this.accountingSite.SiteList)
			{
				if (site.IdentityGuid.Equals(trans.SiteGuid))
				{
					return true;
				}
			}

			// If Order type transaction, allow site group
			if ((trans.TransTypeID == TransactionTypes.T17_Order) || (trans.TransTypeID == TransactionTypes.T18_SupplyOrder))
			{
				return true;
			}

			return false;
		}

		public bool UserIsPartyToTransaction(TransactionDO trans)
		{
			// Check that the user is associated with a company that is a party to the transaction.
			if (this.accountingSite.HasViewPermissionForAllCompanies)
			{
				return true;
			}

			if (accountingSite.UserCompanyList.Contains(Guid.Empty))
			{
				return true;
			}

			if (this.accountingSite.UserCompanyList.Contains(trans.ManagerCompanyGuid))
			{
				return true;
			}

			if (trans.OwnerCompanyGuid != Guid.Empty && 
				accountingSite.UserCompanyList.Contains(trans.OwnerCompanyGuid))
			{
				return true;
			}

			if (trans.ShipToCompanyGuid != Guid.Empty && 
				this.accountingSite.UserCompanyList.Contains(trans.ShipToCompanyGuid))
			{
				return true;
			}

			if (trans.BillToCompanyGuid != Guid.Empty && 
				this.accountingSite.UserCompanyList.Contains(trans.BillToCompanyGuid))
			{
				return true;
			}

			if (trans.CarrierCompanyGuid != Guid.Empty && 
				this.accountingSite.UserCompanyList.Contains(trans.CarrierCompanyGuid))
			{
				return true;
			}

			if (trans.SupplierCompanyGuid != Guid.Empty && 
				this.accountingSite.UserCompanyList.Contains(trans.SupplierCompanyGuid))
			{
				return true;
			}

			if (trans.ShipperCompanyGuid != Guid.Empty && 
				this.accountingSite.UserCompanyList.Contains(trans.ShipperCompanyGuid))
			{
				return true;
			}

			var ownerTransfer = trans as OwnerTransferDO;
			if (ownerTransfer != null)
			{
				if (ownerTransfer.ToCarrierCompanyGuid != Guid.Empty && 
					this.accountingSite.UserCompanyList.Contains(ownerTransfer.ToCarrierCompanyGuid))
				{
					return true;
				}

				if (ownerTransfer.ToManagerCompanyGuid != Guid.Empty && 
					this.accountingSite.UserCompanyList.Contains(ownerTransfer.ToManagerCompanyGuid))
				{
					return true;
				}

				if (ownerTransfer.ToOwnerCompanyGuid != Guid.Empty && 
					this.accountingSite.UserCompanyList.Contains(ownerTransfer.ToOwnerCompanyGuid))
				{
					return true;
				}
			}

			var consumerTransfer = trans as ConsumerTransferDO;
			if (consumerTransfer != null)
			{
				if (consumerTransfer.ToBillToCompanyGuid != Guid.Empty && 
					this.accountingSite.UserCompanyList.Contains(consumerTransfer.ToBillToCompanyGuid))
				{
					return true;
				}

				if (consumerTransfer.ToShipToCompanyGuid != Guid.Empty &&
					this.accountingSite.UserCompanyList.Contains(consumerTransfer.ToShipToCompanyGuid))
				{
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}