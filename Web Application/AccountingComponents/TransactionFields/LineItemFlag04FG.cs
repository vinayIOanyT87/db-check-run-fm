namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemFlag04FG.
	/// Author: Van Thompson
	/// Created for ADF requirements for generic flag fields on line/sub-line items
	/// </summary>
	public class LineItemFlag04FG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		#region Public constants
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_FLAG04 = "CLIENT_SIDE_SCRIPT_LINEITEM_FLAG04";
		public const string CLIENT_SIDE_KEY_LINEITEM_FLAG04 = "CLIENT_SIDE_KEY_LINEITEM_FLAG04";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Line Item Flag 4 field generator.
		/// </summary>
		public LineItemFlag04FG()
		{
		}
		#endregion

		public override string FieldID
		{
			get
			{
				return "LineItem Flag04";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Flag04;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return inLineItem.Flag04.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			inLineItem.Flag04 = (bool) newValue;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Flag04;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Flag04.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			inSublineItem.Flag04 = (bool) newValue;
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
				var checkBox = updatePanel.ContentTemplateContainer.Controls[0] as CheckBox;

				if (checkBox == null)
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
					checkBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_FLAG04] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oLineItemFlag04CheckBox  = document.getElementById('" + checkBox.ClientID + "');\n " +
						"\n//--></script>";

					checkBox.Attributes.Add("onClick", "javascript:try{MasterOnClick('" + this.FieldID + "');}catch(err){;}");
				}
			}
		}
		#endregion
	}
}
