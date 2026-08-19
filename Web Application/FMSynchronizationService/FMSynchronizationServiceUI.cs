using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMSynchronizationService
{
	using System.ServiceProcess;
	using System.Threading;

	public partial class FMSynchronizationServiceUI : Form
	{
		private bool isStarted = false;

		private readonly FMSynchronizationService service = null;

		protected Task TaskToRun = null;
		protected CancellationTokenSource TaskCancelSource = null;
		protected TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
		

		public FMSynchronizationServiceUI(FMSynchronizationService service)
		{
			this.service = service;
			this.InitializeComponent();
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			if (!this.isStarted)
			{
				return;
			}

			this.lblRunning.Text = @"FMSynchronizationService stopping...";

			this.btnStop.Enabled = false;

			try
			{
				// If you don't start the service on a new thread, the WCF calls will not act concurrently.
				Task.Factory.StartNew(() => this.service.Stop());

				if (this.TaskCancelSource != null)
				{
					this.TaskCancelSource.Cancel();
				}

				if (this.TaskToRun != null)
				{
					this.TaskToRun.Wait();
				}

				this.lblRunning.Text = @"FMSynchronizationService stopped.";
				this.isStarted = false;
				this.btnStart.Enabled = true;
			}
			catch (Exception eX)
			{
				MessageBox.Show(eX.Message, @"Exception Encountered", MessageBoxButtons.OK);
			}
			finally
			{
				// Something happened, reset the buttons.
				if (this.isStarted)
				{
					this.btnStop.Enabled = true;
				}
			}

			this.lblRunning.Text = @"FMSynchronizationService stopped.";
		}

		private async void btnStart_Click(object sender, EventArgs e)
		{
			if (this.isStarted)
			{
				return;
			}

			if (this.TaskToRun != null)
			{
				this.TaskToRun.Dispose();
			}

			if (this.TaskCancelSource != null)
			{
				this.TaskCancelSource.Dispose();
			}

			this.TaskCancelSource = new CancellationTokenSource();

			CancellationToken taskToken = this.TaskCancelSource.Token;

			this.lblRunning.Text = @"FMSynchronizationService starting...";

			this.btnStart.Enabled = false;

			try
			{
				// If you don't start the service on a new thread, the WCF calls will not act concurrently.
				this.TaskToRun = Task.Factory.StartNew(() => this.service.Start(), CancellationToken.None, TaskCreationOptions.None, this.UiScheduler);

				await this.TaskToRun;

				if (this.TaskToRun.IsCompleted)
				{
					this.lblRunning.Text = @"FMSynchronizationService started.";
					this.isStarted = true;
					this.btnStop.Enabled = true;
				}
				else if (this.TaskToRun.IsCanceled)
				{
					this.lblRunning.Text = @"FMSynchronizationService startup cancelled.";
				}
				else if (this.TaskToRun.IsFaulted)
				{
					this.lblRunning.Text = @"FMSynchronizationService failed to start.";
				}
			}
			catch (Exception eX)
			{
				MessageBox.Show(eX.Message, @"Exception Encountered", MessageBoxButtons.OK);
			}
			finally
			{
				// Something happened, reset the buttons.
				if (!this.isStarted)
				{
					this.btnStart.Enabled = true;
				}
			}
		}

		private void checkServiceTimer_Tick(object sender, EventArgs e)
		{

		}
	}
}
