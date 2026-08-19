// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Flag01FG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for Flag01FG.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Field generator for Flag01 transaction flag.
	/// </summary>
	public class Flag01FG : CheckBoxGenerator, IHeaderField
	{
		/// <summary>
		/// The text used to identify this control in client-side script.
		/// </summary>
		public const string ClientSideScriptFlag01 = "CLIENT_SIDE_SCRIPT_FLAG01";

		/// <summary>
		/// The key used to identify this control in client-side script.
		/// </summary>
		public const string ClientSideKeyFlag01 = "CLIENT_SIDE_KEY_FLAG01";

		/// <summary>
		/// Gets FieldID.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "Flag01";
			}
		}

		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control">The control to specialize.</param>
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

         if (string.IsNullOrEmpty(customClientScript) == false)
         {
            // Delay client side scripting until page pre-render event in case user clicks edit button of a
            // line item while editing another line item. Such situation causes this method to be called 
            // twice, once for for each line item. Since client side script is  allowed only once to be registered,
            // later line item's client script is ignored, which is the one we actually want.
            checkBox.Page.Session[ClientSideScriptFlag01] =
                                    "<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
                                    "var oFlag01CheckBox  = document.getElementById('" + checkBox.ClientID + "');\n " +
                                    "\n//--></script>";

            checkBox.Attributes.Add("onClick", "javascript:try{MasterOnClick('" + this.FieldID + "');}catch(err){;}");
         }
      }

		#region IHeaderField Members

	  /// <summary>
	  /// Gets the data value.
	  /// </summary>
	  /// <param name="transaction">The transaction.</param>
	  /// <returns>A data object of the type represented by this field generator.</returns>
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Flag01;
		}

		/// <summary>
		/// Gets the data text.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The text representation of the data value represented by this field generator.</returns>
		public string GetDataText(TransactionDO transaction)
		{
			return transaction.Flag01.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Sets the data value.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="newValue">The new value.</param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if (newValue is bool)
			{
				transaction.Flag01 = (bool) newValue;
				this.SetNewValue((bool?)newValue);
				OnFieldChanged();
			}
		}

		#endregion
	}
}
