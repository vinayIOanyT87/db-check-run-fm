namespace TransactionFields
{
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for NotesFG.
	/// </summary>
	public class NotesFG : TextFieldGenerator, IHeaderField
	{
		public NotesFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "Notes";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Notes;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.Notes = newValue as string;
			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 1000.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 1000);
			}
		}

		protected override void SpecializeControl(System.Web.UI.WebControls.WebControl control)
		{
			var cell = control as TableCell;
			TextBox textBox = null;

			if (cell != null)
			{
				cell.ColumnSpan = 4;

				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
				}
				else
				{
					textBox = control.Controls[0] as TextBox;
				}

				if (textBox != null)
				{
					textBox.TextMode = TextBoxMode.MultiLine;
					textBox.Columns = 120;
					textBox.Rows = 3;
					textBox.Wrap = true;
					textBox.CssClass = "formfield";

					textBox.Attributes.Add("maxLength", MaxColumns.ToString());
				}
			}
		}
	}
}
