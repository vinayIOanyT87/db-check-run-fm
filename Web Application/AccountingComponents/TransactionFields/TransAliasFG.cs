namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for TransAliasFG.
	/// </summary>
	public class TransAliasFG : TextFieldGenerator, IHeaderField
	{
		public TransAliasFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "AliasName";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Alias;
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
			throw new System.ApplicationException("TransAliasFG.SetDataValue() should never be called.");
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 25.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return base.GetFieldLength(FieldID, 25);
			}
		}

		/// <summary>
		/// This method will return True indicating that the field is not editable.
		/// </summary>
		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		protected override void SpecializeControl(WebControl control)
		{
			var updatePanel = control.Controls[0] as UpdatePanel;

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
