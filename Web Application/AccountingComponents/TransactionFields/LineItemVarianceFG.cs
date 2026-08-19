namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemVarianceFG.
	/// </summary>
	public class LineItemVarianceFG : NumericTextFieldGenerator, ILineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_VARIANCE = "CLIENT_SIDE_SCRIPT_VARIANCE";
		public const string CLIENT_SIDE_KEY_LINEITEM_VARIANCE = "CLIENT_SIDE_KEY_VARIANCE";

		public LineItemVarianceFG()
		{
		}

		/// <summary>
		/// Returns the FieldID of the field generator
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem Variance";
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
			set
			{
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
				return SITE_VARIABLE_TYPE.VOLUME;
			}
		}

		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control"></param>
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

				// Register client scripts for this control if the custom client script registered is registered.
				var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

				if (!string.IsNullOrEmpty(customClientScript))
				{

					//Delay client side scripting until page pre-render event in case user clicks edit button of a
					//line item while editing another line item. Such situation causes this method to be called 
					//twice, once for for each line item. Since client side script is  allowed only once to be registered,
					//later line item's client script is ignored, which is the one we actually want.
					textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_VARIANCE] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oLineItemVarianceTextBox  = document.getElementById('" + textBox.ClientID + "');\n " +
						"\n//--></script>";

					textBox.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
				}
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.Variance == null)
			{
				return null;
			}

			return inLineItem.Variance.Value;
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
				inLineItem.Variance = null;
			}
			else
			{
				inLineItem.Variance = (double) newValue;
			}
			
			OnFieldChanged();
		}
		#endregion
	}
}
