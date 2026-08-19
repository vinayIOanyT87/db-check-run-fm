namespace TransactionFields
{
	using System;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Security;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Summary description for LineItemTankLevelUnitsFG.
	/// 05-22-2008 V. Thompson
	/// Line Item field added for ADF
	/// </summary>
	public class LineItemAlternativeUnitsFG : DropDownGenerator, ILineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEUNITS = "CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEUNITS";
		public const string CLIENT_SIDE_KEY_LINEITEM_ALTERNATIVEUNITS = "CLIENT_SIDE_KEY_LINEITEM_ALTERNATIVEUNITS";

		public LineItemAlternativeUnitsFG()
		{
		}

		public override string FieldID => "LineItem AlternativeUnits";

        #region ILineItemField Members

		public object GetDataValue(LineItemDO inLineItem)
		{
		    return inLineItem.AlternativeUnits?.ToString(CultureInfo.InvariantCulture);
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (inLineItem.AlternativeUnits == null)
			{
				return string.Empty;
			}

			string unit;

			try
			{
				var cUnit =
					(EngineeringUnit) Enum.Parse(typeof(EngineeringUnit), inLineItem.AlternativeUnits.Value.ToString(CultureInfo.InvariantCulture));
				unit = EngineeringUnits.GetUnitAbbreviation(cUnit);
			}
			catch (ArgumentException)
			{
				unit = string.Empty;
			}

			return unit;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.AlternativeUnits = null;
			}
			else if (newValue.ToString() == this.NotSetText)
			{
				inLineItem.AlternativeUnits = null;
			}
			else
			{
				inLineItem.AlternativeUnits = Convert.ToInt32(newValue);
			}

		    this.OnFieldChanged();
		}
		#endregion

		// vthompson 11/10/2008
		// Changed this field generator from a text generator to a drop down generator
		[SecurityCritical]
		public override HybridDictionary GetEntries()
		{
			var entries = new HybridDictionary();

			// The range of units below was pulled from the range used to
			// populate volume units on the site unit configuration page
			for (EngineeringUnit index = EngineeringUnit.FmvCm3; index <= EngineeringUnit.FmvKl; index++)
			{
				if (this.transContext.accountingSite.LoginSite.VolumeUnits == index)
				{
					continue;
				}

				string valueMember = Convert.ToInt32(index).ToString(CultureInfo.InvariantCulture);
				string displayMember = EngineeringUnits.GetUnitAbbreviation(index);

				entries.Add(displayMember, valueMember);
			}

			return entries;
		}

		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var htmlSelect = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;

				if (htmlSelect == null)
				{
					return;
				}

		htmlSelect.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEUNITS] =
			"<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
			"var oLineItemAlternativeUnits  = document.getElementById('" + htmlSelect.ClientID + "');\n " +
			"\n//--></script>";

				htmlSelect.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
			}
		}
	}
}
