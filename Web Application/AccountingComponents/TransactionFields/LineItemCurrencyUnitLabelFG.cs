namespace TransactionFields
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class LineItemCurrencyUnitLabelFG : FieldGenerator, ILineItemField
	{
		public LineItemCurrencyUnitLabelFG()
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
				return "LineItem CurrencyUnitLabel";
			}
		}

		#region ILineItemField members
		public object GetDataValue(LineItemDO inLineItem)
		{
			object returnVal = inLineItem.CurrencyGuid;

			return returnVal;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			string result = string.Empty;

			object value = this.GetDataValue(inLineItem);

			if ((Guid) value != Guid.Empty)
			{
				var currencyDos = FMChannelHelper.MakeCall<ICurrencies, CurrencyDOCollectionClass>(
																	 x =>
																	 x.GetCurrencies(transContext.security)
																);

				foreach (CurrencyDO currencyDO in currencyDos)
				{
					if (currencyDO.IdentityGuid == (Guid) value)
					{
						result = currencyDO.UnitDisplayName;
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
			var lbl = control.Controls[0] as Label;
			return lbl != null ? lbl.Text : string.Empty;
		}
		#endregion // Abstracts
	}
}
