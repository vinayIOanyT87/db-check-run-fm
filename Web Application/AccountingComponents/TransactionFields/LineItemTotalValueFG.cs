namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	public class LineItemTotalValueFG : NumericTextFieldGenerator, ILineItemField
	{
		#region Public data members
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_VALUE = "CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_VALUE";
		public const string CLIENT_SIDE_KEY_LINEITEM_TOTAL_VALUE = "CLIENT_SIDE_KEY_LINEITEM_TOTAL_VALUE";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Line Item Total Value field generator.
		/// </summary>
		public LineItemTotalValueFG()
		{
			virtualField = true;
		}
		#endregion

		#region Override properties
		/// <summary>
		/// This property will return the Field ID.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem TotalValue";
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
		/// This property will return the unit type.
		/// </summary>
		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				return SITE_VARIABLE_TYPE.DEFAULT;
			}
		}

		/// <summary>
		/// This property will return that this field is not required (false).
		/// </summary>
		public override bool Required
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// This property will return that this field is not editable (false).
		/// </summary>
		public override bool Editable
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region Override methods
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
					textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_VALUE] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oLineItemTotalValueTextBox  = document.getElementById('" +
						textBox.ClientID + "');\n " + "\n//--></script>";
				}
			}
		}
		#endregion

		#region Public Methods
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.TotalValue;
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
			// Calculated field value - not setable
			OnFieldChanged();
		}
		#endregion
	}
}