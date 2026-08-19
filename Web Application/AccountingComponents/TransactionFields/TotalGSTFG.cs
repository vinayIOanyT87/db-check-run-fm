namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for TotalGSTFG.
	/// </summary>
	public class TotalGSTFG : NumericTextFieldGenerator, IHeaderField
	{
		public TotalGSTFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "TotalGST";
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

		/// <summary>
		/// Format the control as read-only without disabling the control
		/// </summary>
		/// <param name="control">The control to format</param>
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

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.TotalGST;
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
			// This is a derived field and cannot be set
		}
		#endregion
	}
}
