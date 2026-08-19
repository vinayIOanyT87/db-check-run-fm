namespace TransactionFields
{
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	public class LineItemDeliveryLocationLabelFG : FieldGenerator, ILineItemField
	{
		public LineItemDeliveryLocationLabelFG()
		{
			virtualField = true;
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
			set
			{
				base.Editable = value;
			}
		}

		public override string FieldID
		{
			get
			{
				return "LineItem DeliveryLocationLabel";
			}
		}

		#region ILineItemField members
		public object GetDataValue(LineItemDO inLineItem)
		{
			object returnVal = null;

			if (inLineItem.AssociatedTransactions != null)
			{
				var uniqueList = new List<string>();

				// should get the product name for the associated transactions
				foreach (AssociatedTxDO tx in inLineItem.AssociatedTransactions)
				{
					if (!uniqueList.Contains(tx.DeliveryLocation))
					{
						uniqueList.Add(tx.DeliveryLocation);
						inLineItem.DeliveryLocation = tx.DeliveryLocation;
					}
				}

				if (uniqueList.Count == 0)
				{
					inLineItem.DeliveryLocation = null;
				}

				returnVal = uniqueList;
			}

			return returnVal;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			string result = string.Empty;

			object value = this.GetDataValue(inLineItem);

			if (value != null)
			{
				var productList = value as List<string>;

				if (productList != null)
				{
					foreach (string product in productList)
					{
						if (result.Length == 0)
						{
							result = product;
							continue;
						}
						result += ", " + product;
					}
				}
			}

			return result;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			// do nothing, this is a read only field
		}

		#endregion // ILineItemField members

		#region Abstracts
		public override void Generate(bool editable)
		{
			var lbl = new Label();
			cell.Controls.Add(lbl);

			lbl.Text = GetFormattedValue();
			lbl.ID = this.ID + this.FieldID;
		}

		public override object GetNewValue(WebControl control)
		{
			// nothing required here, just pass through
			var lbl = control.Controls[0] as Label;
			return lbl != null ? lbl.Text : string.Empty;
		}
		#endregion // Abstracts
	}
}
