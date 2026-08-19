namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMControls;

	public class LineItemSelectedQualityFG : LineItemTransactionQualityFG
	{
		public LineItemSelectedQualityFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem SelectedQuality";
			}
		}

		public override void Generate(bool editable)
		{
			base.Generate(editable);
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			// sometimes it casts as htmlselect for some reason...
			if (updatePanel != null)
			{
				var selectedQualityDdl = updatePanel.ContentTemplateContainer.Controls[0] as FMDropDownList;

				if (selectedQualityDdl != null)
				{
					if (selectedQualityDdl.SelectedItem.Text.Equals(notSelectedText))
					{
						selectedQualityDdl.Items.Remove(notSelectedText);
					}
				}
				else
				{
					var selectedQualityHtmlSelect = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;

					if (selectedQualityHtmlSelect != null)
					{
						if (selectedQualityHtmlSelect.Items.Contains(new ListItem(notSelectedText)))
						{
							selectedQualityHtmlSelect.Items.Remove(notSelectedText);
						}
					}
				}
			}
		}
	}
}
