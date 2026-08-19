namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemFlag02FG.
	/// Author: Van Thompson
	/// Created for ADF requirements for generic flag fields on line/sub-line items
	/// </summary>
	public class LineItemFlag02FG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		#region Public constants
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_FLAG02 = "CLIENT_SIDE_SCRIPT_LINEITEM_FLAG02";
		public const string CLIENT_SIDE_KEY_LINEITEM_FLAG02 = "CLIENT_SIDE_KEY_LINEITEM_FLAG02";
		#endregion

		public LineItemFlag02FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Flag02";
			}
		}


		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.Flag02;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			return lineItem.Flag02.ToString();
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.Flag02 = (bool)newValue;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			return sublineItem.Flag02;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			return sublineItem.Flag02.ToString();
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.Flag02 = (bool)newValue;
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
					checkBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_FLAG02] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oLineItemFlag02CheckBox  = document.getElementById('" + checkBox.ClientID + "');\n " +
						"\n//--></script>";

					checkBox.Attributes.Add("onClick", "javascript:try{MasterOnClick('" + this.FieldID + "');}catch(err){;}");
				}
			}
		}
		#endregion

	}
}
