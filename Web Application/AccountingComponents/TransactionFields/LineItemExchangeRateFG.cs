namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemExchangeRateFG.
	/// </summary>
	public class LineItemExchangeRateFG : NumericTextFieldGenerator, ILineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_EXCHANGE_RATE = "CLIENT_SIDE_SCRIPT_EXCHANGERATE";
		public const string CLIENT_SIDE_KEY_LINEITEM_EXCHANGE_RATE = "EXCHANGERATE";

		public LineItemExchangeRateFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem ExchangeRate";
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
			if (inLineItem.ExchangeRate == null)
			{
				return null;
			}

			return inLineItem.ExchangeRate.Value;
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
				inLineItem.ExchangeRate = null;
			}
			else
			{
				inLineItem.ExchangeRate = (double) newValue;
			}

			OnFieldChanged();
		}
		#endregion

		#region Override methods
		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control"></param>
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

				// Register client scripts for this control if the custom client script registered is registered.
				var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

				if (!string.IsNullOrEmpty(customClientScript))
				{

					//Delay client side scripting until page pre-render event in case user clicks edit button of a
					//line item while editing another line item. Such situation causes this method to be called 
					//twice, once for for each line item. Since client side script is  allowed only once to be registered,
					//later line item's client script is ignored, which is the one we actually want.
					textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_EXCHANGE_RATE] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oExchangeRateTextBox  = document.getElementById('" + textBox.ClientID + "');\n " +
						"\n//--></script>";
				}
			}
		}
		#endregion
	}
}
