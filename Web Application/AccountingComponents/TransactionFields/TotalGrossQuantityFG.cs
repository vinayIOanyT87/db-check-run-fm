namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	public class TotalGrossQuantityFG : NumericTextFieldGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This method is the default constructor for the Total Gross Quantity class.
		/// </summary>
		public TotalGrossQuantityFG()
		{
			virtualField = true;
		}
		#endregion

		#region Override properties
		/// <summary>
		/// This property returns the ID of the field.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "TotalGrossQuantity";
			}
		}

		/// <summary>
		/// This property returns the type of data being use in the field.
		/// </summary>
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
			get
			{
				return SITE_VARIABLE_TYPE.VOLUME;
			}
		}
		#endregion

		#region Override methods
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
					textBox.Enabled = false;
					textBox.BackColor = this.VarecBkgrndReadOnlyGray;
				}
			}
		}
		#endregion

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.TotalGrossQuantity;
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
			// This is a calculated field and cannot be set
			OnFieldChanged();
		}
		#endregion
	}
}
