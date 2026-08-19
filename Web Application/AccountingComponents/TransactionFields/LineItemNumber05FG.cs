namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemNumber05FG.
	/// </summary>
	public class LineItemNumber05FG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_NUMBER05 = "CLIENT_SIDE_SCRIPT_LINEITEM_NUMBER05";
		public const string CLIENT_SIDE_KEY_LINEITEM_NUMBER05 = "CLIENT_SIDE_KEY_LINEITEM_NUMBER05";

		public LineItemNumber05FG()
		{
		}

		/// <summary>
		/// Returns the FieldID of the field generator
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem Number05";
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

				if (textBox != null)
				{

					// Register client scripts for this control if the custom client script registered is registered.
					var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

					if (!string.IsNullOrEmpty(customClientScript))
					{

						//Delay client side scripting until page pre-render event in case user clicks edit button of a
						//line item while editing another line item. Such situation causes this method to be called 
						//twice, once for for each line item. Since client side script is  allowed only once to be registered,
						//later line item's client script is ignored, which is the one we actually want.
						textBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_NUMBER05] =
														"<script type=\"text/javascript\"><!--\n" +
														"var oLineItemNumber01TextBox  = document.getElementById('" + textBox.ClientID + "');\n " +
														"\n//--></script>";

						textBox.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
					}
				}
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.Number05 == null)
			{
				return null;
			}

			return inLineItem.Number05.Value;
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
				inLineItem.Number05 = null;
			}
			else
			{
				inLineItem.Number05 = (double) newValue;
			}
			
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			if (inSublineItem.Number05 == null)
			{
				return null;
			}

			return inSublineItem.Number05.Value;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}
			
			return null;
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			if (newValue == null)
			{
				inSublineItem.Number05 = null;
			}
			else
			{
				inSublineItem.Number05 = (double) newValue;
			}
			
			OnFieldChanged();
		}
		#endregion
	}
}
