using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DispatchPrototype
{
	public partial class AppointmentReminderForm : Form
	{
		public AppointmentReminderForm(DispatchContainerForm DispatchForm)
		{
			InitializeComponent();
			InitializeListViewDisplay();

         timer1.Interval = 60000;
         timer1.Tick += new EventHandler(timer1_Tick);
         timer1.Start();
      }

		private void OnCancelClick(object sender, EventArgs e)
		{
         timer1.Stop();
         timer1.Enabled = false;
         Close();
		}

		private void InitializeListViewDisplay()
		{
			AppointmentlistView.Clear();
			AppointmentlistView.View = View.Details;
			AppointmentlistView.Columns.Add("Index", 0, HorizontalAlignment.Left);
			AppointmentlistView.Columns.Add("Personnel", 140, HorizontalAlignment.Left);
			AppointmentlistView.Columns.Add("Type", 100, HorizontalAlignment.Left);
			AppointmentlistView.Columns.Add("Description", 150, HorizontalAlignment.Left);
			AppointmentlistView.Columns.Add("Time", 140, HorizontalAlignment.Left);
			AppointmentlistView.Columns.Add("Status", 190, HorizontalAlignment.Left);

			SleepTimeValue.ResetText();
			SleepTimeValue.Items.Add("Minutes");
			SleepTimeValue.Items.Add("Hours");
			SleepTimeValue.SelectedItem = SleepTimeValue.Items[0];

			SleepNumber.Text = "15";
		}

		private void OnActivated(object sender, EventArgs e)
		{
			RefreshListView();
		}

      void timer1_Tick(object sender, EventArgs e)
      {
         timer1.Stop();
         DispatchContainerForm DispatchForm = Owner as DispatchContainerForm;
         DispatchForm.GetTodaysScheduledEvents();
         DispatchForm.CheckIfAppoinmentIsDue();
         RefreshListView();
         timer1.Enabled = true;
      }

		private void RefreshListView()
		{
			int TimeSpanInInt = 0;
			ListViewItem li;
			DateTime CurrentDateTime = System.DateTime.Now;
			TimeSpan DifferenceInTime = new TimeSpan();
			DispatchContainerForm DispatchForm = Owner as DispatchContainerForm;
			AppointmentlistView.Items.Clear();
			// populate the listview with the due appointments
			foreach (TodaysAppointmentClass TodaysAppointment in DispatchForm.TodaysAppointmentsCollection)
			{
				if (TodaysAppointment.AppointmentIsDue == false)
					continue;

				li = AppointmentlistView.Items.Add(TodaysAppointment.Index.ToString());
				li.SubItems.Add(TodaysAppointment.AssetText);
				li.SubItems.Add(TodaysAppointment.AppointmentCategory);
				li.SubItems.Add(TodaysAppointment.Description);
				li.SubItems.Add(TodaysAppointment.DueDate.ToString());

				// calculate the status of this appointment
				DifferenceInTime = TodaysAppointment.DueDate - CurrentDateTime;
				TimeSpanInInt = System.Convert.ToInt32(DifferenceInTime.TotalMinutes);
				if (TimeSpanInInt >= 0)
					li.SubItems.Add("Due in " + TimeSpanInInt.ToString() + " Minutes");
				else
					li.SubItems.Add("Over Due by " + TimeSpanInInt.ToString() + " Minutes");
			}

			// disable the controls until a selection is made
			SleepNumber.Enabled = false;
			SleepButton.Enabled = false;
			SleepTimeValue.Enabled = false;
			Dismissbutton.Enabled = false;

		}

		private void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			SleepNumber.Enabled = true;
			SleepButton.Enabled = true;
			SleepTimeValue.Enabled = true;
			Dismissbutton.Enabled = true;
		}

		private void OnDismissSelectedItemClicked(object sender, EventArgs e)
		{
			int iLoop = 0;
			if (AppointmentlistView.SelectedItems.Count == 0)
				return;

			DispatchContainerForm DispatchForm = Owner as DispatchContainerForm;
			for (iLoop = 0; iLoop < AppointmentlistView.SelectedItems.Count; iLoop++)
			{
				string selecteditemtext = AppointmentlistView.SelectedItems[iLoop].Text;
				foreach (TodaysAppointmentClass TodaysAppointment in DispatchForm.TodaysAppointmentsCollection)
				{
					if (TodaysAppointment.Index == System.Convert.ToInt32(selecteditemtext))
					{
						DispatchForm.TodaysAppointmentsCollection.Remove(TodaysAppointment);
						TodaysAppointmentClass NewAppointment = new TodaysAppointmentClass();
						NewAppointment.Index = TodaysAppointment.Index;
						NewAppointment.DueDate = TodaysAppointment.DueDate;
						NewAppointment.Description = TodaysAppointment.Description;
						NewAppointment.AppointmentCategory = TodaysAppointment.AppointmentCategory;
						NewAppointment.AssetText = TodaysAppointment.AssetText;
						NewAppointment.Duration = TodaysAppointment.Duration;
						NewAppointment.DoNotNotifyAgain = true;
						DispatchForm.TodaysAppointmentsCollection.Add(NewAppointment);
						break;
					}
				}
			}
			RefreshListView();
		}

		private void OnSleepButtonClick(object sender, EventArgs e)
		{
			int iLoop = 0;
			DateTime CurrentDate = System.DateTime.Now;
			if (AppointmentlistView.SelectedItems.Count == 0)
				return;

			if (SleepNumber.Text.Length <= 0)
				return;

			if (System.Convert.ToInt32(SleepNumber.Text) <= 0)
				return;

			DispatchContainerForm DispatchForm = Owner as DispatchContainerForm;
			for (iLoop = 0; iLoop < AppointmentlistView.SelectedItems.Count; iLoop++)
			{
				string selecteditemtext = AppointmentlistView.SelectedItems[iLoop].Text;
				foreach (TodaysAppointmentClass TodaysAppointment in DispatchForm.TodaysAppointmentsCollection)
				{
					if (TodaysAppointment.Index == System.Convert.ToInt32(selecteditemtext))
					{
						DispatchForm.TodaysAppointmentsCollection.Remove(TodaysAppointment);
						TodaysAppointmentClass NewAppointment = new TodaysAppointmentClass();
						NewAppointment.Index = TodaysAppointment.Index;
						NewAppointment.DueDate = TodaysAppointment.DueDate;
						NewAppointment.Description = TodaysAppointment.Description;
						NewAppointment.AppointmentCategory = TodaysAppointment.AppointmentCategory;
						NewAppointment.AssetText = TodaysAppointment.AssetText;
						NewAppointment.Duration = TodaysAppointment.Duration;
						NewAppointment.InSleepMode = true;
						if(SleepTimeValue.SelectedItem.ToString() == "Minutes")
						{
							NewAppointment.SleepTimeInterval = CurrentDate.AddMinutes(System.Convert.ToInt32(SleepNumber.Text));
						}
						else
						{
							NewAppointment.SleepTimeInterval = CurrentDate.AddHours(System.Convert.ToInt32(SleepNumber.Text));
						}
						
						DispatchForm.TodaysAppointmentsCollection.Add(NewAppointment);
						break;
					}
				}
			}
			RefreshListView();

		}

		private void OnAppointmentColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
			// do not allow the user to display the index column
			if (e.ColumnIndex == 0)
			{
				if (AppointmentlistView.Columns[e.ColumnIndex].Width > 0)
					AppointmentlistView.Columns[e.ColumnIndex].Width = 0;
			}
		}





	}
}
