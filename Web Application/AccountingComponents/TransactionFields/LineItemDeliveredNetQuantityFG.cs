//*****************************************************************************
// LineItemDeliveredNetQuantityFG.cs
//
// Original Author: 
// Revisions: See source control comments
//
// (C) Copyright 2007 by Varec, Inc.  All rights reserved.
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		2007-10-05	I. Orndorff			- Modified "SetDataValue()" and "UpdateRemaining()"
//										  to calculate ValueRemaining.
//				
//		2008-05-29	I. Orndorff			- Modified "SetDataValue()" to set the dirty
//										  flag when data changes. This fixs CSI #5910.
//										- Modified "GetDataValue()" to return the rounded
//										  double. This fixes CSI #5911.
//
//*****************************************************************************

namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemDeliveredNetQuantityFG.
	/// </summary>
	public class LineItemDeliveredNetQuantityFG : LineItemVolumeFG, ILineItemField, ISublineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_NET_QUANTITY = "CLIENT_SIDE_SCRIPT_LINEITEM_DELIVERED_NET_QUANTITY";
		public const string CLIENT_SIDE_KEY_LINEITEM_NET_QUANTITY = "CLIENT_SIDE_KEY_LINEITEM_DELIVERED_NET_QUANTITY";

		public LineItemDeliveredNetQuantityFG()
		{
			bFieldRequired = false;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem DeliveredNetQuantity";
			}
		}

		public override bool Required
		{
			get
			{
				return bFieldRequired;
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

				textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_NET_QUANTITY] =
					"<script type=\"text/javascript\"><!--\n" +
					"var oLineItemDeliveredNetQuantityTextBox  = document.getElementById('" + textBox.ClientID + "');\n " +
					"\n//--></script>";

				textBox.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
			}
		}

		#region ILineItemField Members
		public virtual object GetDataValue(LineItemDO inLineItem)
		{
			this.ManualValueFlag = inLineItem.Quantity.DeliveredNetManualValueFlag;

			if (inLineItem.Quantity.NullableDeliveredNet == null)
			{
				return null;
			}

			return Math.Round(inLineItem.Quantity.DeliveredNetInventoryChange, inLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
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
				inLineItem.Quantity.NullableDeliveredNet = null;
			}
			else
			{
				inLineItem.Quantity.NullableDeliveredNet = (double) newValue;
			}

			inLineItem.Quantity.IsDeliveredNetDirty = true;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			this.ManualValueFlag = inSublineItem.Quantity.DeliveredNetManualValueFlag;

			if (inSublineItem.Quantity.NullableDeliveredNet == null)
			{
				return null;
			}

			return Math.Round(inSublineItem.Quantity.DeliveredNetInventoryChange, inSublineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
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
				inSublineItem.Quantity.NullableDeliveredNet = null;
			}
			else
			{
				inSublineItem.Quantity.NullableDeliveredNet = (double) newValue;
			}

			inSublineItem.Quantity.IsDeliveredNetDirty = true;
			OnFieldChanged();
		}
		#endregion
	}
}
