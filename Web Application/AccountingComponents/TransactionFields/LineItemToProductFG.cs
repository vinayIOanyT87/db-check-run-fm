/// <summary>
///   FILE NAME:  LineItemToProductFG.cs
///	PURPOSE:		This class inherits from the LineItemProductFG class. It is used during
///
///	COMMENTS:
///		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Endress+Hauser.
///
///	AUTHOR(S):	Thomas Beckum
///	VERSION:		1.0.0  Current version
///
///	MODIFICATION HISTORY:
///   Date:		   By:					Reason:
///   ----------	-----------------	-------------------------------------------
///   2006-11-02	Richard Panachida	Corrected the defect that places the same product in the TO and FROM
///										   fields (CSI 3277 & 3574).
///   2008-10-29  Richard Panachida Changed the casting method to the old style in order to throw an exception. This
///                                 was related to CSI 203.
///                                 
///   2009-03-25  Richard Panachida Defect 2380: The To-Product index was not being saved.
///   
///	2009-07-28	W.Gray				Fix to SetDataValue to set ToProductIndex to null when product doesn't exist
/// </summary>
namespace TransactionFields
{
    using System;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class LineItemToProductFG : LineItemProductFG
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Line Item To Product field generator.
		/// </summary>
		public LineItemToProductFG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property implement the return of the field ID.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem ToProduct";
			}
		}

		/// <summary>
		/// This property implements the return of the required field flag.
		/// True equal required.
		/// </summary>
		public override bool Required
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return base.GetFieldLength(FieldID, FieldLength);
			}
		}
		#endregion

		#region Public override methods
		/// <summary>
		/// This method implements the retrieval of the value of this field generator.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		override public object GetDataValue(LineItemDO inLineItem)
		{
			var regradeLineItem = (RegradeLineItemDO) inLineItem;

			return regradeLineItem.ToProduct;
		}

		/// <summary>
		/// This method implements the setting of a new data value for the TO-Product
		/// transaction field.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <param name="newValue"></param>
		override public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			var regradeLineItemDO = inLineItem as RegradeLineItemDO;

			var productID = newValue as string;
            regradeLineItemDO.ToProduct = productID;

			if (string.IsNullOrEmpty(productID))
			{
                regradeLineItemDO.ToProductCode = null;
                regradeLineItemDO.ToProductType = null;
				regradeLineItemDO.ToProductGuid = Guid.Empty;
			}
			else
			{
				ProductClass product = this.GetProductObject(productID);

				if (product != null)
				{
				    regradeLineItemDO.ToProduct = product.ID;
					regradeLineItemDO.ToProductCode = product.Code;
					regradeLineItemDO.ToProductGuid = product.MasterRecordGuid;
					regradeLineItemDO.ToProductType = ProductClass.ProductTypeID(product.ProductType);
					regradeLineItemDO.BrokenBlend = false;
					regradeLineItemDO.ImproperAdditization = false;
				}
                else
                {
                    inLineItem.Product = null;
                    inLineItem.ProductCode = null;
                    inLineItem.ProductType = null;
                    inLineItem.ProductGuid = Guid.Empty;

                    inLineItem.SubLineItems.Clear();
                    this.RenderErrorMessage(string.Format(ErrMsg002, productID));
                }

				SetUnitsAndDecimalPlaces(inLineItem, product);
			}

            object fieldValue = this.GetDataValue(lineItem);

            // If using autocomplete controls, update the value displayed in the box with the product provided.
            // This is necessary to ensure that if a user types in a product that differs in case from the configured value,
            // the configured value is shown instead of the value the user typed in.
            // For example, if the user types f-54-sum and the product ID configured is F-54-SUM, we want to display F-54-SUM
            if (transContext.EnableAutoComplete)
            {
                if (cell.Controls.Count > 0)
                {
                    var updatePanel = cell.Controls[0] as UpdatePanel;
                    TextBox textBox;

                    if (updatePanel != null)
                    {
                        updatePanel.Update();
                        textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
                    }
                    else
                    {
                        textBox = cell.Controls[0] as TextBox;
                    }

                    if (textBox != null)
                    {
                        if (fieldValue != null)
                        {
                            textBox.Text = fieldValue.ToString();
                        }
                        else
                        {
                            textBox.Text = string.Empty;
                        }
                    }
                }
            }
			this.OnFieldChanged();
		}

		/// <summary>
		/// This method overrides the method in the LineItemProductFG base class. The
		/// reason is to ensure that the product is set to the TO value.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		override public string GetDataText(LineItemDO inLineItem)
		{
			var regradeLineItem = inLineItem as RegradeLineItemDO;

			if (regradeLineItem != null)
			{
				return regradeLineItem.ToProduct;
			}

			return string.Empty;
		}

		private void SetUnitsAndDecimalPlaces(LineItemDO lineItem,ProductClass product)
		{
			UnitsHelperClass unitsHelper=new UnitsHelperClass(transContext.security,transContext.accountingSite.CurrentSite,transContext.aliasClass,product);
			if(lineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
				unitsHelper.SetUnits(lineItem,ProductType.AdditiveProduct,product);
			else
				unitsHelper.SetUnits(lineItem,ProductType.ComponentProduct,product);
		}

      #endregion
   }
}
