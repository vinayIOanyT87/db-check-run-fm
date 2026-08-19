namespace Dispatch
{
	using System;
	using System.Windows.Forms;

	using Dispatch;

	public partial class AppointmentReminderForm : Form
	{
		private readonly DispatchContainerForm dispatchForm;

		public AppointmentReminderForm(DispatchContainerForm dispatchForm)
		{
			this.dispatchForm = dispatchForm;
			this.InitializeComponent();
			this.InitializeListViewDisplay();

			this.timer1.Interval = 60000;
			this.timer1.Tick += this.Timer1Tick;
			this.timer1.Start();
		}

		private void OnCancelClick(object sender, EventArgs e)
		{
			this.timer1.Stop();
			this.timer1.Enabled = false;
			this.Close();
		}

		private void InitializeListViewDisplay()
		{
			this.AppointmentlistView.Clear();
			this.AppointmentlistView.View = View.Details;
			this.AppointmentlistView.Columns.Add("Index", 0, HorizontalAlignment.Left);
			this.AppointmentlistView.Columns.Add("Personnel", 140, HorizontalAlignment.Left);
			this.AppointmentlistView.Columns.Add("Type", 100, HorizontalAlignment.Left);
			this.AppointmentlistView.Columns.Add("Description", 150, HorizontalAlignment.Left);
			this.AppointmentlistView.Columns.Add("Time", 140, HorizontalAlignment.Left);
			this.AppointmentlistView.Columns.Add("Status", 190, HorizontalAlignment.Left);

			this.SleepTimeValue.ResetText();
			this.SleepTimeValue.Items.Add("Minutes");
			this.SleepTimeValue.Items.Add("Hours");
			this.SleepTimeValue.SelectedItem = this.SleepTimeValue.Items[0];

			this.SleepNumber.Text = "15";
		}

		private void OnActivated(object sender, EventArgs e)
		{
			this.RefreshListView();
		}

		void Timer1Tick(object sender, EventArgs e)
		{
			this.timer1.Stop();
			var dispatchContainerForm = this.Owner as DispatchContainerForm;

			if (dispatchContainerForm != null)
			{
				dispatchContainerForm.GetTodaysScheduledEvents();
				dispatchContainerForm.CheckIfAppoinmentIsDue();
			}

			this.RefreshListView();
			this.timer1.Enabled = true;
		}

		private void RefreshListView()
		{
			DateTime currentDateTime = DateTime.Now;
			var dispatchContainerForm = this.Owner as DispatchContainerForm;
			this.AppointmentlistView.Items.Clear();

			// populate the listview with the due appointments
			if (dispatchContainerForm != null)
			{
				foreach (TodaysAppointmentClass todaysAppointment in dispatchContainerForm.TodaysAppointmentsCollection)
				{
					if (todaysAppointment.AppointmentIsDue == false)
					{
						continue;
					}

					ListViewItem li = this.AppointmentlistView.Items.Add(todaysAppointment.IdentityGuid.ToString());
					li.SubItems.Add(todaysAppointment.AssetText);
					li.SubItems.Add(todaysAppointment.AppointmentCategory);
					li.SubItems.Add(todaysAppointment.Description);
					li.SubItems.Add(todaysAppointment.DueDate.ToString());

					// calculate the status of this appointment
					TimeSpan differenceInTime = todaysAppointment.DueDate - currentDateTime;
					int timeSpanInInt = Convert.ToInt32(differenceInTime.TotalMinutes);

					if (timeSpanInInt >= 0)
					{
						li.SubItems.Add("Due in " + timeSpanInInt + " Minutes");
					}
					else
					{
						li.SubItems.Add("Over Due by " + timeSpanInInt + " Minutes");
					}
				}
			}

			// disable the controls until a selection is made
			this.SleepNumber.Enabled = false;
			this.SleepButton.Enabled = false;
			this.SleepTimeValue.Enabled = false;
			this.Dismissbutton.Enabled = false;
		}

		private void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			this.SleepNumber.Enabled = true;
			this.SleepButton.Enabled = true;
			this.SleepTimeValue.Enabled = true;
			this.Dismissbutton.Enabled = true;
		}

		private void OnDismissSelectedItemClicked(object sender, EventArgs e)
		{
			int iLoop;

			if (this.AppointmentlistView.SelectedItems.Count == 0)
			{
				return;
			}

			var dispatchContainerForm = this.Owner as DispatchContainerForm;

			for (iLoop = 0; iLoop < this.AppointmentlistView.SelectedItems.Count; iLoop++)
			{
				string selecteditemtext = this.AppointmentlistView.SelectedItems[iLoop].Text;

				if (dispatchContainerForm != null)
				{
					foreach (TodaysAppointmentClass todaysAppointment in dispatchContainerForm.TodaysAppointmentsCollection)
					{
						if (todaysAppointment.IdentityGuid == Guid.Parse(selecteditemtext))
						{
							dispatchContainerForm.TodaysAppointmentsCollection.Remove(todaysAppointment);
							var newAppointment = new TodaysAppointmentClass
							                     {
								                     IdentityGuid = todaysAppointment.IdentityGuid,
								                     DueDate = todaysAppointment.DueDate,
								                     Description = todaysAppointment.Description,
								                     AppointmentCategory = todaysAppointment.AppointmentCategory,
								                     AssetText = todaysAppointment.AssetText,
								                     Duration = todaysAppointment.Duration,
								                     DoNotNotifyAgain = true
							                     };
							dispatchContainerForm.TodaysAppointmentsCollection.Add(newAppointment);
							break;
						}
					}
				}
			}

			this.RefreshListView();
		}

		private void OnSleepButtonClick(object sender, EventArgs e)
		{
			int iLoop;
			DateTime currentDate = DateTime.Now;

			if (this.AppointmentlistView.SelectedItems.Count == 0)
			{
				return;
			}

			if (this.SleepNumber.Text.Length <= 0)
			{
				return;
			}

			if (Convert.ToInt32(this.SleepNumber.Text) <= 0)
			{
				return;
			}

			var dispatchContainerForm = this.Owner as DispatchContainerForm;

			for (iLoop = 0; iLoop < this.AppointmentlistView.SelectedItems.Count; iLoop++)
			{
				string selecteditemtext = this.AppointmentlistView.SelectedItems[iLoop].Text;

				if (dispatchContainerForm != null)
				{
					foreach (TodaysAppointmentClass todaysAppointment in dispatchContainerForm.TodaysAppointmentsCollection)
					{
						if (todaysAppointment.IdentityGuid == Guid.Parse(selecteditemtext))
						{
							dispatchContainerForm.TodaysAppointmentsCollection.Remove(todaysAppointment);
							var newAppointment = new TodaysAppointmentClass
							                     {
								                     IdentityGuid = todaysAppointment.IdentityGuid,
								                     DueDate = todaysAppointment.DueDate,
								                     Description = todaysAppointment.Description,
								                     AppointmentCategory = todaysAppointment.AppointmentCategory,
								                     AssetText = todaysAppointment.AssetText,
								                     Duration = todaysAppointment.Duration,
								                     InSleepMode = true
							                     };

							if (this.SleepTimeValue.SelectedItem.ToString() == "Minutes")
							{
								newAppointment.SleepTimeInterval = currentDate.AddMinutes(System.Convert.ToInt32(this.SleepNumber.Text));
							}
							else
							{
								newAppointment.SleepTimeInterval = currentDate.AddHours(System.Convert.ToInt32(this.SleepNumber.Text));
							}

							dispatchContainerForm.TodaysAppointmentsCollection.Add(newAppointment);
							break;
						}
					}
				}
			}
			this.RefreshListView();

		}

		private void OnAppointmentColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
			// do not allow the user to display the index column
			if (e.ColumnIndex == 0)
			{
				if (this.AppointmentlistView.Columns[e.ColumnIndex].Width > 0)
					this.AppointmentlistView.Columns[e.ColumnIndex].Width = 0;
			}
		}
	}
}
