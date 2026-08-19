namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemNonDomesticPriceFG.
	/// </summary>
	public class LineItemNonDomesticPriceFG : NumericTextFieldGenerator, ILineItemField
	{
		#region Public data members
		public const string CLIENT_SIDE_KEY_LINEITEM_NONDOMESTIC_PRICE = "CLIENT_SIDE_KEY_LINEITEM_NONDOMESTIC_PRICE";
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_NONDOMESTIC_PRICE = "CLIENT_SIDE_SCRIPT_LINEITEM_NONDOMESTIC_PRICE";
		#endregion

		public LineItemNonDomesticPriceFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem NonDomesticPrice";
			}
		}

		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		/// <summary>
		/// This property will return the unit type which is set to default.
		/// </summary>
		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				return SITE_VARIABLE_TYPE.DEFAULT;
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.NonDomesticPrice == null)
			{
				return null;
			}

			return inLineItem.NonDomesticPrice.Value;
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
			if (newValue == null)
			{
				inLineItem.NonDomesticPrice = null;
			}
			else
			{
				inLineItem.NonDomesticPrice = (double) newValue;
			}

			OnFieldChanged();
		}
		#endregion

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

				if (this.transContext.Currencies != null)
				{
					//Delay client side scripting until page pre-render event in case user clicks edit button of a
					//line item while editing another line item. Such situation causes this method to be called 
					//twice, once for for each line item. Since client side script is  allowed only once to be registered,
					//later line item's client script is ignored, which is the one we actually want.
					textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_NONDOMESTIC_PRICE] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oNonDomesticPriceTextBox  = document.getElementById('" + textBox.ClientID + "');\n " +
						"\n//--></script>";
					textBox.Attributes.Add("onChange", "javascript:try{CurrencyChange();}catch(err){;}");
				}
			}
		}
	}
}
