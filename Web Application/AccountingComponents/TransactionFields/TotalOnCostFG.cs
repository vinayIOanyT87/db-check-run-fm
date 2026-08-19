namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for TotalOnCostFG.
	/// </summary>
	public class TotalOnCostFG : NumericTextFieldGenerator, IHeaderField
	{
		public TotalOnCostFG()
		{
			//virtualField = true;
		}


		public override string FieldID
		{
			get
			{
				return "TotalOnCost";
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
			get
			{
				return SITE_VARIABLE_TYPE.DEFAULT;
			}
		}

		/// <summary>
		/// Format the control as read-only without disabling the control
		/// </summary>
		/// <param name="control">The control to format</param>
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

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			// workout total oncost
			double oncost = 0.0;

			foreach (LineItemDO localLineItem in transaction.LineItems)
			{
				try
				{
					oncost += string.IsNullOrWhiteSpace(localLineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14]) ?
						0.0 : double.Parse(localLineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14]);
				}
				catch (Exception)
				{
					// Ignore
				}
			}

			return oncost;
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
			// cannot be set
		}
		#endregion
	}
}
