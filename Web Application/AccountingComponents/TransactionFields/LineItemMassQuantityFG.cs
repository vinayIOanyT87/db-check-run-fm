//******************************************************************************
//	FILE NAME:		LineItemMassQuantityFG.cs
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
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    /// <summary>
	/// Summary description for LineItemMassQuantityFG.
	/// </summary>
	public class LineItemMassQuantityFG : LineItemMassFG, ILineItemField, ISublineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_MASS_QUANTITY = "CLIENT_SIDE_SCRIPT_LINEITEM_MASS_QUANTITY";
		public const string CLIENT_SIDE_KEY_LINEITEM_MASS_QUANTITY = "CLIENT_SIDE_KEY_LINEITEM_MASS_QUANTITY";

		public LineItemMassQuantityFG()
		{
			bFieldRequired = false;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem MassQuantity";
			}
		}

		public override bool Required
		{
			get
			{
				return bFieldRequired;
			}
		}

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
				string additiveTypeID = ProductClass.ProductTypeID(ProductType.AdditiveProduct);
				string productID = "";
				string productType = "";

				if (this.sublineItem != null
				   && this.sublineItem.Product != null)
				{
					productID = this.sublineItem.Product;
					productType = this.sublineItem.ProductType;
				}

				else if (this.lineItem != null
				   && this.lineItem.Product != null)
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
														x.GetByProductAuthorizedCompanies(this.transContext.security, lineItem.ProductGuid, false)
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

		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox == null)
				{
					return;
				}

				textBox.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			this.ManualValueFlag = inLineItem.Quantity.MassManualValueFlag;

			if (inLineItem.Quantity.NullableMass == null)
			{
				return null;
			}

			return Math.Round(inLineItem.Quantity.MassInventoryChange, inLineItem.MassDecimalPlaces, MidpointRounding.AwayFromZero);
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			object obj = GetDataValue(inLineItem);

			if (obj == null)
			{
				return null;
			}

			return obj.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.Quantity.NullableMass = null;
			}
			else
			{
				inLineItem.Quantity.NullableMass = (double) newValue;
			}

			inLineItem.Quantity.IsMassDirty = true;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			this.ManualValueFlag = inSublineItem.Quantity.MassManualValueFlag;

			if (inSublineItem.Quantity.NullableMass == null)
			{
				return null;
			}

			return Math.Round(inSublineItem.Quantity.MassInventoryChange, inSublineItem.MassDecimalPlaces, MidpointRounding.AwayFromZero);
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			object obj = ((ISublineItemField) this).GetDataValue(inSublineItem);

			if (obj == null)
			{
				return null;
			}

			return obj.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			if (newValue == null)
			{
				inSublineItem.Quantity.NullableMass = null;
			}
			else
			{
				inSublineItem.Quantity.NullableMass = (double) newValue;
			}

			inSublineItem.Quantity.IsMassDirty = true;
			OnFieldChanged();
		}
		#endregion
	}
}
