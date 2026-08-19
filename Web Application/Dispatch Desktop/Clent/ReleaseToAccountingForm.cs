namespace Dispatch
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class ReleaseToAccountingForm : FMBaseForm
	{
		private readonly DispatchDataAccess dataAccess;

		public DateTime OperationLockDate;
		private DateTime lastSelectedDateTime;

		public ReleaseToAccountingForm(DispatchDataAccess dataAccess, DateTime lockDate)
		{
			this.InitializeComponent();
			this.dataAccess = dataAccess;
			this.OperationLockDate = lockDate;
		}

		private void ClosebuttonClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void FormLoad(object sender, EventArgs e)
		{
			this.GetSecurity();

			DateTime today = DateTime.Now;
			var todayMidNight = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);

			this.lockoutdateTimePicker.Value = todayMidNight;
			this.lockoutTimePicker.Value = todayMidNight;
		}

		/// <summary>
		/// This method handles the apply button click event. It will determine if the dispatch
		/// process can begin and if so, then the transactions will be updated to indicate they
		/// are ready for accounting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ApplybuttonClick(object sender, EventArgs e)
		{
			try
			{
				string errorMessage;
				DateTime? selectedDateTime = null;

				try
				{
					selectedDateTime = this.lockoutdateTimePicker.Value.Date + this.lockoutTimePicker.Value.TimeOfDay;
				}
				catch (Exception)
				{
					errorMessage = "Invalid date time.";
					MessageBox.Show(errorMessage);
				}

				if ((selectedDateTime == null) || (selectedDateTime > DateTime.Now))
				{
					errorMessage = "Lock out date/time can not be in the future";
					MessageBox.Show(errorMessage);
					return;
				}

				Dictionary<string, string> results = FMChannelHelper.MakeCall<IClientDispatchService, Dictionary<string, string>>(
					dispatchRequests => dispatchRequests.ReleaseToAccounting(this.Security, selectedDateTime.Value));

				if (results.ContainsKey("OK"))
				{
					this.DialogResult = DialogResult.OK;
					this.ErrorHandler(results["OK"]);
				}

				if (results.ContainsKey("Failed"))
				{
					MessageBox.Show(results["Failed"]);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void LockoutdateTimePickerValueChanged(object sender, EventArgs e)
		{
			DateTime selectedDateTime = this.lockoutdateTimePicker.Value.Date + this.lockoutTimePicker.Value.TimeOfDay;

			if (this.lastSelectedDateTime == selectedDateTime)
			{
				if (selectedDateTime < this.OperationLockDate ||
					 selectedDateTime > DateTime.Now.Date)
				{
					this.lastSelectedDateTime = this.OperationLockDate;
					this.lockoutdateTimePicker.Value = this.OperationLockDate;
					this.lockoutTimePicker.Value = this.OperationLockDate;
				}
			}
		}

		private void SetToCurrentDateTimebutton_OnClick(object sender, EventArgs e)
		{
			this.lockoutdateTimePicker.Value = DateTime.Now;
			this.lockoutTimePicker.Value = DateTime.Now;
		}
	}
}
