namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	public class ROSupplierFG : TextFieldGenerator, IHeaderField
	{
		#region Construction
		public ROSupplierFG()
		{
			virtualField = true;
		}
		#endregion // Construction

		#region Overrides
		public override string FieldID
		{
			get
			{
				return "ROSupplier";
			}
		}

		protected override void SpecializeControl(WebControl control)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var companyBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (companyBox != null)
				{
					companyBox.Enabled = false;
					companyBox.BackColor = this.VarecBkgrndReadOnlyGray;
				}
			}
		}
		#endregion // Overrides

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.SupplierID;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			// do not set, ro field
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 30);
			}
		}
	}
}
