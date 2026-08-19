namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	public class LineItemTax1FG : NumericTextFieldGenerator, ILineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_TAX1 = "CLIENT_SIDE_SCRIPT_LINEITEM_TAX1";
		public const string CLIENT_SIDE_KEY_LINEITEM_TAX1 = "CLIENT_SIDE_KEY_LINEITEM_TAX1";

		#region Constructors
		/// <summary>
		/// This is the default constructor for the line item Tax 1 field generator.
		/// </summary>
		public LineItemTax1FG()
		{
		}
		#endregion

		#region Override properties
		/// <summary>
		/// This property will return the ID of the field.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem Tax1";
			}
		}

		/// <summary>
		/// This property will return the type of the field (double).
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
		#endregion

		#region ILineItemField Members
		/// <summary>
		/// This method will return either null if there is no value or the 
		/// actual value as a double?.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.Tax1 == null)
			{
				return null;
			}

			return inLineItem.Tax1.Value;
		}

		/// <summary>
		/// This method will return the actual value as a string.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}

		/// <summary>
		/// This method will set the new value in the object.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.Tax1 = null;
			}
			else
			{
				inLineItem.Tax1 = (double) newValue;
			}

			base.OnFieldChanged();
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
				var hiddenField = updatePanel.ContentTemplateContainer.Controls[1] as HiddenField;

				if (textBox == null || hiddenField == null)
				{
					return;
				}

				// Save the Page object for processing in the SetDataValue method.
				this.Page = control.Page;

         // Register client scripts for this control if the custom client script registered is registered.
         var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

				if (!string.IsNullOrEmpty(customClientScript))
				{

					//Delay client side scripting until page pre-render event in case user clicks edit button of a
					//line item while editing another line item. Such situation causes this method to be called 
					//twice, once for for each line item. Since client side script is  allowed only once to be registered,
					//later line item's client script is ignored, which is the one we actually want.
					textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_TAX1] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oLineItemTax1TextBox  = document.getElementById('" + textBox.ClientID + "');\n " +
						"var oHiddenLineItemTax1TextBox  = document.getElementById('" + hiddenField.ClientID + "');\n " +
						"\n//--></script>";
					textBox.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
				}
			}
		}
		#endregion
	}
}
