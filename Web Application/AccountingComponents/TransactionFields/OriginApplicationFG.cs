namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for CardNameFG.
	/// </summary>
	public class OriginApplicationFG : TextFieldGenerator, IHeaderField
	{
		public OriginApplicationFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LookupOriginApplicationIndex";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.OriginApplication.ToString();
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (this.transContext.useDataDictonary)
			{
				string datatext = GetDataDictionaryValueByKey(this.transContext.accountingSite.CurrentSiteGuid, GetDataValue(transaction).ToString());

                return datatext;
			}

			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			OnFieldChanged();
		}

		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 30);
			}
		}

		protected override void SpecializeControl(WebControl control)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.ReadOnly = true;
					textBox.BackColor = this.VarecBkgrndReadOnlyGray;
				}
			}
		}
	}
}
