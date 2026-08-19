using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMOpcUaServerService
{
	public partial class FMOpcUaServerServiceForm : Form
	{
		private readonly OpcUaServerService fmOpcUaServerService = new OpcUaServerService();

		public FMOpcUaServerServiceForm()
		{
			this.InitializeComponent();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
				// If you don't start the service on a new thread, the WCF calls will not act concurrently.
			Task.Factory.StartNew(() => this.fmOpcUaServerService.Start());

			this.btnStart.Enabled = false;
			this.btnStop.Enabled = true;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			this.fmOpcUaServerService.Stop();

			this.btnStart.Enabled = true;
			this.btnStop.Enabled = false;
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.btnStop.Enabled)
			{
				this.fmOpcUaServerService.Stop();
			}

			this.btnStart.Enabled = true;
			this.btnStop.Enabled = false;
		}
	}
}
