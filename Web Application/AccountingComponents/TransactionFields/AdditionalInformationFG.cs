// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditionalInformationFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for AdditionalInformationFG.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The AdditionalInformationFG field generator class.
	/// </summary>
	public class AdditionalInformationFG : TextFieldGenerator, IHeaderField
	{
		/// <summary>
		/// This is an abstract property that will be implemented by the derived class.
		/// It will return the field ID.
		/// </summary>
		public override string FieldID
		{
			get { return "AdditionalInformation"; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 255.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength( this.FieldID, 255 ); }
		}

		/// <summary>
		/// Gets the data value.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The data value object./</returns>
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.AdditionalInformation;
		}

		/// <summary>
		/// Gets the data text.
		/// </summary>
		/// <param name="transaction">The transaction object.</param>
		/// <returns>The data value text.</returns>
		public string GetDataText(TransactionDO transaction)
		{
			if (this.GetDataValue(transaction) != null)
			{
				return this.GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		/// <summary>
		/// Sets the data value.
		/// </summary>
		/// <param name="transaction">The transaction object to use.</param>
		/// <param name="newValue">The new value to set.</param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.AdditionalInformation = newValue as string;
			this.OnFieldChanged();
		}

		/// <summary>
		/// Specializes the control.
		/// </summary>
		/// <param name="control">The control to specialize.</param>
		protected override void SpecializeControl(WebControl control)
		{
			var tableCell = control as TableCell;

			if (tableCell != null)
			{
				tableCell.ColumnSpan = 4;
				var updatePanel = control.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

					if (textBox != null)
					{
						textBox.TextMode = TextBoxMode.MultiLine;
						textBox.Columns = 120;
						textBox.Wrap = true;
						textBox.CssClass = "formfield";

						textBox.Attributes.Add("maxLength", this.MaxColumns.ToString(CultureInfo.InvariantCulture));
					}
				}
			}
		}
	}
}
