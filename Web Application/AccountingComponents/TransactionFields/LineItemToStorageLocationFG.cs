/*****************************************************************************
LineItemToStorageLocationFG

Original Author: Van Thompson
Revisions: See source control comments

(C) Copyright 2008 by Varec, Inc.  All rights reserved.

Revision History
Date:		By:					Reason:

//*****************************************************************************/

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using System;

	public class LineItemToStorageLocationFG : LineItemStorageLocationFG
	{
		public override string FieldID
		{
			get
			{
				return "LineItem ToStorageLocationID";
			}
		}

		public override bool Required
		{
			get
			{
				return base.Required;
			}
		}

		/// <summary>
		/// This method will return the to storage location from the line item as a type regrade line item
		/// or the default is storage transfer line item.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		public override object GetDataValue(LineItemDO inLineItem)
		{
			// Storage location field can be in either a Regrade Line Item DO
			// or the default Storage Transfer Line Item DO.
			if (inLineItem.GetType() == typeof(RegradeLineItemDO))
			{
				var regradeLineItem = (RegradeLineItemDO) inLineItem;
				return regradeLineItem.ToStorageLocation;
			}
			var storageTransferLineItem = (StorageTransferLineItemDO)inLineItem;
			return storageTransferLineItem.ToStorageLocation;
		}

		/// <summary>
		/// This method will set the to storage location as regrade line item or
		/// storage location line item.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <param name="newValue"></param>
		public override void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			var storageID = newValue as string;

			if (storageID != null)
			{
				if (inLineItem.GetType() == typeof(RegradeLineItemDO))
				{
					var regradeLineItem = (RegradeLineItemDO) inLineItem;
					regradeLineItem.ToStorageLocation = storageID;
					TankClass tank = this.GetTankObject(storageID);

					try
					{
						regradeLineItem.ToStorageLocationTankGuid = tank.IdentityGuid;
					}
					catch (NullReferenceException nullEx)
					{
						throw new ApplicationException("Tank or storage location " +
						   "was not found.", nullEx);
					}
				}
				else
				{
					var storageTransferLineItem = (StorageTransferLineItemDO) inLineItem;
					storageTransferLineItem.ToStorageLocation = storageID;
					TankClass tank = this.GetTankObject(storageID);

					try
					{
						storageTransferLineItem.ToStorageLocationTankGuid = tank.IdentityGuid;
					}
					catch (NullReferenceException nullEx)
					{
						throw new ApplicationException("Tank or storage location " +
						   "was not found.", nullEx);
					}
				}
			}
			else
			{
				if (inLineItem.GetType() == typeof(RegradeLineItemDO))
				{
					var regradeLineItem = (RegradeLineItemDO) inLineItem;
					regradeLineItem.ToStorageLocation = null;
					regradeLineItem.ToStorageLocationTankGuid = Guid.Empty;
				}
				else
				{
					var storageTransferLineItem = (StorageTransferLineItemDO) inLineItem;
					storageTransferLineItem.ToStorageLocation = null;
					storageTransferLineItem.ToStorageLocationTankGuid = Guid.Empty;
					this.SetTank();
				}
			}

			OnFieldChanged();
		}

		public override string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}
	}
}
