namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemProductPriceFG.
	/// </summary>
	public class LineItemProductPriceFG : NumericTextFieldGenerator, ILineItemField
	{
		#region Public attributes
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCTPRICE = "CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCTPRICE";
		public const string CLIENT_SIDE_KEY_LINEITEM_PRODUCTPRICE = "CLIENT_SIDE_KEY_LINEITEM_PRODUCTPRICE";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Line Item Product Price field generator.
		/// </summary>
		public LineItemProductPriceFG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Field Identifier for this field.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem ProductPrice";
			}
		}

		/// <summary>
		/// This property will return the field type (numeric).
		/// </summary>
		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		/// <summary>
		/// This property will return unit type of the field (default).
		/// </summary>
		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				return SITE_VARIABLE_TYPE.DEFAULT;
			}
		}
		#endregion

		#region ILineItemField Members
		/// <summary>
		/// This method will return either a null or the an object representing
		/// the value.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.ProductPrice == null)
			{
				return null;
			}

			return inLineItem.ProductPrice.Value;
		}

		/// <summary>
		/// This method will return price in text form if present. Else,
		/// it will return an empty string.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		public string GetDataText(LineItemDO inLineItem)
		{
			object dataValue = this.GetDataValue(inLineItem);

			if (dataValue == null)
			{
				return string.Empty;
			}

			return dataValue.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.ProductPrice = null;
			}
			else
			{
				inLineItem.ProductPrice = (double) newValue;
			}
			
			OnFieldChanged();
		}
		#endregion

		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
				var hiddenField = updatePanel.ContentTemplateContainer.Controls[1] as HiddenField;

				if (textBox == null || hiddenField == null)
				{
					return;
				}

				if (this.transContext.Currencies != null)
				{
					//Delay client side scripting until page pre-render event in case user clicks edit button of a
					//line item while editing another line item. Such situation causes this method to be called 
					//twice, once for for each line item. Since client side script is  allowed only once to be registered,
					//later line item's client script is ignored, which is the one we actually want.
					textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCTPRICE] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oLineItemProductPrice  = document.getElementById('" + textBox.ClientID + "');\n " +
						"var oHiddenLineItemProductPrice  = document.getElementById('" + hiddenField.ClientID + "');\n " +
						"\n//--></script>";

					textBox.Attributes.Add("onChange", "javascript:try{CurrencyChange();}catch(err){;} try{MasterOnChange('" + this.FieldID + "');}" +
					                                   "catch(err){try{BaseMasterOnChange('" + this.FieldID + "');}catch(err){;}}");
				}
			}
		}
	}
}
