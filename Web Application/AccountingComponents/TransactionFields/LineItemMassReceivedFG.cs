//******************************************************************************
//	FILE NAME:		LineItemMassQuantityReceivedFG.cs
//	PURPOSE:		This class inherits from NumericTextFieldGenerator and sets
//					the Mass value.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Sijuan Jiang
//	VERSION:		1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		
//*******************************************************************************       

namespace TransactionFields
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using System;

	/// <summary>
	/// Summary description for LineItemMassQuantityReceivedFG.
	/// </summary>
	internal class LineItemMassQuantityReceivedFG : QuantityReceivedFG, ILineItemField
	{
		public LineItemMassQuantityReceivedFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem MassQuantityReceived";
			}
		}

		public override bool Required
		{
			get
			{
				return false;
			}
		}

		public override bool Editable
		{
			get
			{
				// Only editable if this is a Order type transaction
				if (trans != null && 
					trans.TransTypeID != TransactionTypes.T17_Order && 
					trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
				{
					return false;
				}

				var transactionDO = this.trans;

				if (transactionDO != null)
				{
					LineItemDO localLineItem = transactionDO.LineItems[0];
					return localLineItem.TransactionLineItemGuid != Guid.Empty;
				}

				return false;
			}
		}

		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				string additiveTypeID	= ProductClass.ProductTypeID(FMBusinessObjects.DataObjects.ProductType.AdditiveProduct);
				string productID		= string.Empty;
				string productType		= string.Empty;

				if (this.sublineItem != null && this.sublineItem.Product != null)
				{
					productID = this.sublineItem.Product;
					productType = this.sublineItem.ProductType;
				}
				else if (this.lineItem != null && this.lineItem.Product != null)
				{
					productID = this.lineItem.Product;
					productType = this.lineItem.ProductType;
				}

				if (string.IsNullOrEmpty(productType))
				{
					if (!string.IsNullOrEmpty(productID))
					{
						ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
															  x =>
															  x.GetByID(this.transContext.security, productID)
														);

						productType = ProductClass.ProductTypeID(product.ProductType);
					}

				}

				if (productType == additiveTypeID)
				{
					return SITE_VARIABLE_TYPE.ADDITIVE_MASS;
				}

				return SITE_VARIABLE_TYPE.MASS;
			}
		}

		public object GetDataValue(LineItemDO inLineItem)
		{
			return Math.Round(inLineItem.MassQuantityReceived, inLineItem.MassDecimalPlaces, MidpointRounding.AwayFromZero);
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			OnFieldChanged();
		}
	}
}
