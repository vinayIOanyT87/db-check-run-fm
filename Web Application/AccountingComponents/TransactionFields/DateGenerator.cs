namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMControls;

	public abstract class DateGenerator : FieldGenerator
	{
		#region Private data members
		private FMDate fmDate;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public DateGenerator()
		{
		}
		#endregion

		/// <summary>
		/// This method will generate the date control.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate(bool editable)
		{
			var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };

			//Create Date 
			fmDate = new FMDate { ID = this.ID + " Date", Enabled = (editable && this.Editable), Visible = true };

			fmDate.ToolTip = this.DisplayName;

			updatePanel.ContentTemplateContainer.Controls.Add(fmDate);
			this.cell.Controls.Add(updatePanel);

			object dateValue = GetDataValue();

			if (dateValue is DateTimeOffset)
			{
				var dateTimeOffset = ((DateTimeOffset)dateValue).Date;
				fmDate.Text = fieldGenerator.accountingSite.FormatDateTime(dateTimeOffset);
			}
			else if (dateValue is DateTime)
			{
				var dateTime = ((DateTime)dateValue).Date;
				fmDate.Text = fieldGenerator.accountingSite.FormatDateTime(dateTime);
			}
			else
			{
				fmDate.Text = string.Empty;
			}
		}

		/// <summary>
		/// This method overrides and implements the get new value method. It will throw
		/// an exception if the date is in the incorrect format or if the date field is
		/// required and no date is present.
		/// </summary>
		/// <param name="control"></param>
		/// <returns>Return the current date value.</returns>
		public override object GetNewValue(WebControl control)
		{
			cell.BackColor = System.Drawing.Color.Red;

			if (this.Required && (fmDate.Text.Trim().Length == 0))
			{
				const string Msg = "Date is required.";
				throw new Exception(Msg);
			}

			cell.BackColor = System.Drawing.Color.Transparent;

			if (fmDate.Text.Trim().Length == 0)
			{
				return null;
			}

			try
			{
				// Validate that this is a validate date.
				string tempDate = fmDate.Text;
				fmDate.Text = tempDate;

				return fmDate.CurrentValue;
			}
			catch (FormatException)
			{
				fmDate.Text = string.Empty;
				const string Msg = "Date Format is invalid.";
				throw new Exception(Msg);
			}
		}

		/// <summary>
		/// This method will return the formatted date value.
		/// </summary>
		/// <returns></returns>
		public override string GetFormattedValue()
		{
			object dateValue = GetDataValue();

			if (dateValue == null)
			{
				return string.Empty;
			}

			if (dateValue is DateTimeOffset)
			{
				var date = (DateTimeOffset)dateValue;
				return this.fieldGenerator.accountingSite.FormatDate(date);
			}

			return null;
		}

		/// <summary>
		/// This method will populate the control with a new value to be displayed.
		/// </summary>
		/// <param name="dateValue">The date value to display.</param>
		public void SetDisplayValue(DateTimeOffset? dateValue)
		{
			if (this.cell == null || dateValue == null)
			{
				return;
			}

			var fmDateControl = this.cell.Controls[0] as FMDate;

			if (fmDateControl != null)
			{
				fmDateControl.CurrentValue = dateValue.Value;
			}
		}
	}
}
