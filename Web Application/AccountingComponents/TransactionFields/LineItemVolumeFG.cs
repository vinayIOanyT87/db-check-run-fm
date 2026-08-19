//******************************************************************************
//	FILE NAME:		LineItemVolumeFG.cs
//	PURPOSE:		This class inherits from NumericTextFieldGenerator and sets
//					the volume value.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Thomas Beckum
//	VERSION:		1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		2006-11-01	Richard Panachida	Correct an exception that occurs when a transaction
//										regrade is saved. Commented out the code, since it
//										does look like it is performing anything (CSI 3574).
//		2006-12-13	Richard Panachida	Removed reference to transaction context which is
//										nolonger needed (CSI 3811).
//*******************************************************************************       

namespace TransactionFields
{
    using System.Globalization;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Summary description for LineItemVolumeFG.
    /// </summary>
    abstract public class LineItemVolumeFG : NumericTextFieldGenerator
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
				string productID = string.Empty;
				string productType = string.Empty;

				if ( this.sublineItem != null  && this.sublineItem.Product != null )
				{
					productID = this.sublineItem.Product;
					productType = this.sublineItem.ProductType;
				}
				
				else if ( this.lineItem != null && this.lineItem.Product != null )
				{
					productID = this.lineItem.Product;
					productType = this.lineItem.ProductType;
				}

				if ( string.IsNullOrEmpty(productType) )
				{
					if ( !string.IsNullOrEmpty(productID) )
					{
						ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByID( this.transContext.security, productID )
																);

						productType = ProductClass.ProductTypeID( product.ProductType );
					}
				}

				if ( productType == additiveTypeID )
				{
					return SITE_VARIABLE_TYPE.ADDITIVE_VOLUME;
				}

				return SITE_VARIABLE_TYPE.VOLUME;
			}
		}
		
		public override object GetNewValue(WebControl control)
		{
			return base.GetNewValue(control);
		}
	}
}
