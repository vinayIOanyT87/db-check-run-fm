//******************************************************************************
//	FILE NAME:		LineItemMassFG.cs
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

    /// <summary>
	/// Summary description for LineItemMassFG.
	/// </summary>
	abstract public class LineItemMassFG : NumericTextFieldGenerator
	{
		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				string additiveTypeID = ProductClass.ProductTypeID( ProductType.AdditiveProduct );
				string productID = "";
				string productType = "";

				if ( this.sublineItem != null && this.sublineItem.Product != null )
				{
					productID = this.sublineItem.Product;
					productType = this.sublineItem.ProductType;
				}
				
				else if ( this.lineItem != null  && this.lineItem.Product != null )
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
	}
}
