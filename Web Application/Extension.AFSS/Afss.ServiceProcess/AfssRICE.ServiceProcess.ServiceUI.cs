namespace AfssRICE.ServiceProcess
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Generic;

	using FuelsManager.Afss.ServiceProcess;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ServiceProcessInterfaces;

	public partial class ServiceUI : Form
	{
		private bool isStarted = false;

		private string sessionID = string.Empty;

		private readonly AfssServiceProcess service = null;

		public ServiceUI(AfssServiceProcess service)
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

			this.lblRunning.Text = @"FuelsManager Automated Fuel Service Station Service stopping...";

			this.btnStop.Enabled = false;

			try
			{
				// Need to stop the WCF services in the current thread so we can wait for it to terminate.
				//Task.Factory.StartNew(() => this.service.Stop());
				this.service.Stop();

				this.lblRunning.Text = @"FuelsManager Automated Fuel Service Station Service stopped.";
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

			this.lblRunning.Text = @"FuelsManager Automated Fuel Service Station Service stopped.";
		}

		private async void btnStart_Click(object sender, EventArgs e)
		{
			if (this.isStarted)
			{
				return;
			}

			this.lblRunning.Text = @"FuelsManager Automated Fuel Service Station Service starting...";

			this.btnStart.Enabled = false;

			try
			{
				// If you don't start the service on a new thread, the WCF calls will not act concurrently.
				Task serviceTask = Task.Factory.StartNew(() => this.service.Start());

				await serviceTask;

				if (serviceTask.IsCompleted)
				{
					this.lblRunning.Text = @"FuelsManager External Fuel Station Service started.";
					this.isStarted = true;
					this.btnStop.Enabled = true;
				}
				else if (serviceTask.IsCanceled)
				{
					this.lblRunning.Text = @"FuelsManager External Fuel Station Service startup canceled.";
				}
				else if (serviceTask.IsFaulted)
				{
					this.lblRunning.Text = @"FuelsManager Automated Fuel Service Station Service failed to start.";
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

		private void LoginBtn_Click(object sender, EventArgs e)
		{
			// richTextBox1.AppendText(string.Format("Login Session ID: {0}", sessionID));
		}

		private void TestConnectBtn_Click(object sender, EventArgs e)
		{
			GasboyStation station = new GasboyStation();
			station.IdentityGuid = new Guid("4984F7AD-F1DC-4A06-A9FC-39769F5F3CAD");
			station.IpAddress = this.StationURLTextBox.Text;
			station.UserName = "Admin";
			station.Password = "Admin";
			station.SiteCode = 123456;

			var result = GasboyManagerChannelHelper.MakeCall<IGasboyStationServices, string>(
					serviceChannel => serviceChannel.TestConnection(GetServiceSecurityInstance(), station));

			richTextBox1.AppendText(string.Format("Test Connection: {0}", result));
		}

		private void GetTransBtn_Click(object sender, EventArgs e)
		{
			GasboyStation station = new GasboyStation();
			station.IdentityGuid = new Guid("4984F7AD-F1DC-4A06-A9FC-39769F5F3CAD");
			station.IpAddress = this.StationURLTextBox.Text;
			station.UserName = "Admin";
			station.Password = "Admin";
			station.SiteCode = 123456;

			var stationList = new List<Guid>();
			stationList.Add(station.IdentityGuid);

			var result =
				GasboyManagerChannelHelper.MakeCall<IGasboyStationServices, Dictionary<Guid,string>>(
					serviceChannel => serviceChannel.DownloadNewTransactionsForStations(GetServiceSecurityInstance(), stationList));

			richTextBox1.AppendText(string.Format("Test Connection: {0}", result));
		}

		private static SecurityClass GetServiceSecurityInstance()
		{
			SecurityClass serviceProcessSecurity = new SecurityClass();
			serviceProcessSecurity.LoginSiteGuid = Guids.SiteAdminGuid;
			serviceProcessSecurity.LoginSiteID = "SiteAdmin";
			serviceProcessSecurity.SiteGuid = Guids.SiteAdminGuid;
			serviceProcessSecurity.SiteID = "SiteAdmin";
			serviceProcessSecurity.UserGuid = Guids.UserAdminGuid;
			serviceProcessSecurity.UserID = "GasboyService";
			serviceProcessSecurity.AddRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION);
			serviceProcessSecurity.AddRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION);
			serviceProcessSecurity.AddRight(RIGHT.MODIFY_TRANSACTION_DATA);
			return serviceProcessSecurity;
		}
	}
}
