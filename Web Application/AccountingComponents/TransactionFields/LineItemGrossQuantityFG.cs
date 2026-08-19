/*****************************************************************************
 LineItemGrossQuantityFG.cs

 Original Author: 
 Revisions: See source control comments

 (C) Copyright 2007 by Varec, Inc.  All rights reserved.

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		2008-05-29	I. Orndorff			- Modified "SetDataValue()" to set the dirty
										  flag when data changes. This fixs CSI #5910.
										- Modified "GetDataValue()" to return the rounded
										  double. This fixes CSI #5911.
		
		09-29-2008	V. Thompson			Added the CheckQuantityAggregation function

		09-30-2008	V. Thompson			Removed the CheckQuantityAggregation function
	 
	  01-27-2009  A. Coker       Fixed defect 1162.

	  03-31-2009  A. Coker       Change Request 2488. Quantity field in transaction detail page header is set
								 to blank when initially displayed in Add mode.
//*****************************************************************************/

namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemGrossQuantityFG.
	/// </summary>
	public class LineItemGrossQuantityFG : LineItemVolumeFG, ILineItemField, ISublineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_GROSSQUANTITY = "CLIENT_SIDE_SCRIPT_LINEITEM_GROSSQUANTITY";
		public const string CLIENT_SIDE_KEY_LINEITEM_GROSSQUANTITY = "CLIENT_SIDE_KEY_LINEITEM_GROSSQUANTITY";

		public LineItemGrossQuantityFG()
		{
			bFieldRequired = false;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem GrossQuantity";
			}
		}
		public override bool Required
		{
			get
			{
				return bFieldRequired;
			}
		}

		#region ILineItemField Members
		public virtual object GetDataValue(LineItemDO inLineItem)
		{
			this.ManualValueFlag = inLineItem.Quantity.GrossManualValueFlag;

			if (inLineItem.Quantity.NullableGross == null)
			{
				return null;
			}

			return Math.Round(inLineItem.Quantity.GrossInventoryChange, inLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
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

		public virtual void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.Quantity.NullableGross = null;
			}
			else
			{
				inLineItem.Quantity.NullableGross = (double) newValue;
			}

			inLineItem.Quantity.IsGrossDirty = true;
			OnFieldChanged();
		}

		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox == null)
				{
					return;
				}

				textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_GROSSQUANTITY] =
					"<script type=\"text/javascript\"><!--\n" +
					"var oLineItemGrossQuanity  = document.getElementById('" + textBox.ClientID + "');\n " +
					"\n//--></script>";

				textBox.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
			}
		}
		#endregion

		#region ISublineItemField Members

		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			this.ManualValueFlag = sublineItem.Quantity.GrossManualValueFlag;

			if (inSublineItem.Quantity.NullableGross == null)
			{
				return null;
			}

			return Math.Round(inSublineItem.Quantity.GrossInventoryChange, inSublineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
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
				inSublineItem.Quantity.NullableGross = null;
			}
			else
			{
				inSublineItem.Quantity.NullableGross = (double) newValue;
			}

			inSublineItem.Quantity.IsGrossDirty = true;
			OnFieldChanged();
		}
		#endregion
	}
}
