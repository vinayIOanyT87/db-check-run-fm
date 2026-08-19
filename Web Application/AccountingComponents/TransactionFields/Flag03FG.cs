namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for Flag03FG.
	/// </summary>
	public class Flag03FG : CheckBoxGenerator, IHeaderField
	{
		public const string CLIENT_SIDE_SCRIPT_FLAG03 = "CLIENT_SIDE_SCRIPT_FLAG03";
		public const string CLIENT_SIDE_KEY_FLAG03 = "CLIENT_SIDE_KEY_FLAG03";

		public Flag03FG()
		{
		}

		public override string FieldID
		{
			get { return "Flag03"; }
		}

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Flag03;
		}

		public string GetDataText(TransactionDO transaction)
		{
			return transaction.Flag03.ToString();
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if (newValue is bool)
			{
				transaction.Flag03 = (bool) newValue;
				this.SetNewValue((bool?)newValue);
				OnFieldChanged();
			}
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control"></param>
		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);

			CheckBox checkBox = null;
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				checkBox = updatePanel.ContentTemplateContainer.Controls[0] as CheckBox;
			}

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
				checkBox.Page.Session[CLIENT_SIDE_SCRIPT_FLAG03] =
										"<script type=\"text/javascript\"><!--\n" +
										"var oFlag01CheckBox  = document.getElementById('" + checkBox.ClientID + "');\n " +
										"\n//--></script>";

				checkBox.Attributes.Add("onClick", "javascript:MasterOnClick('" + this.FieldID + "')");
			}
		}
		#endregion
	}
}
