namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemTankLevelUnitsFG.
	/// 05-22-2008 V. Thompson
	/// Line Item field added for ADF
	/// </summary>
	public class LineItemAlternativeGrossVolumeFG : NumericTextFieldGenerator, ILineItemField
	{

      public const string CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEGROSSVOLUME = "CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEGROSSVOLUME";
      public const string CLIENT_SIDE_KEY_LINEITEM_ALTERNATIVEGROSSVOLUME = "CLIENT_SIDE_KEY_LINEITEM_ALTERNATIVEGROSSVOLUME";
      
      public LineItemAlternativeGrossVolumeFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem AlternativeGrossVolume";
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
			get { return SITE_VARIABLE_TYPE.DEFAULT; }
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.AlternativeGrossVolume == null)
				return null;
			else
				return lineItem.AlternativeGrossVolume.Value;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if (GetDataValue(lineItem) != null)
			{
				return GetDataValue(lineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			if (newValue == null)
				lineItem.AlternativeGrossVolume = null;
			else
				lineItem.AlternativeGrossVolume = new double?((double) newValue);
			OnFieldChanged();
		}

		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);

			TextBox textBox = null;
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
			}

			if (textBox == null)
			{
				return;
			}

         textBox.Page.Session[LineItemAlternativeGrossVolumeFG.CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEGROSSVOLUME] =
                                 "<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
                                 "var oLineItemAlternativeGrossVolume  = document.getElementById('" + textBox.ClientID + "');\n " +
                                 "\n//--></script>";
      }


		#endregion
	}
}
