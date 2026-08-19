namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemTotalPriceWithTaxFG.
	/// </summary>
	public class LineItemTotalPriceWithTaxFG : NumericTextFieldGenerator, ILineItemField
	{
		#region Public data members
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_PRICE_TAX = "CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_PRICE_TAX";
		public const string CLIENT_SIDE_KEY_LINEITEM_TOTAL_PRICE_TAX = "CLIENT_SIDE_KEY_LINEITEM_TOTAL_PRICE_TAX";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Line Item Total Price With Tax field generator.
		/// </summary>
		public LineItemTotalPriceWithTaxFG()
		{
			virtualField = true;
		}
		#endregion

		#region Override methods
		/// <summary>
		/// This property will return the Field ID.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem TotalPriceWithTax";
			}
		}

		/// <summary>
		/// This property will return the numeric type for this field which is a double.
		/// </summary>
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

		/// <summary>
		/// Format the control as read-only without disabling the control
		/// </summary>
		/// <param name="control">The control to format</param>
		protected override void SpecializeControl(WebControl control)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox == null)
				{
					return;
				}

				textBox.ReadOnly = true;
				textBox.BackColor = this.VarecBkgrndReadOnlyGray;

				// Register client scripts for this control if the custom client script registered is registered.
				var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

				if (!string.IsNullOrEmpty(customClientScript))
				{
					//Delay client side scripting until page pre-render event in case user clicks edit button of a
					//line item while editing another line item. Such situation causes this method to be called 
					//twice, once for for each line item. Since client side script is  allowed only once to be registered,
					//later line item's client script is ignored, which is the one we actually want.
					textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_PRICE_TAX] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oLineItemTotalPriceWithTaxTextBox  = document.getElementById('" +
						textBox.ClientID + "');\n " + "\n//--></script>";
				}
			}
		}
		#endregion

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.TotalPriceWithTax;
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
			// This is a derived field and cannot be set
		}
		#endregion
	}
}

