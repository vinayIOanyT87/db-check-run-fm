using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMPointService
{
	public partial class FMPointServiceInProcForm : Form
	{
		private readonly FMPointService fuelsManagerService;

		public FMPointServiceInProcForm(string host, string port)
		{
			this.fuelsManagerService = new FMPointService(host, port);
			InitializeComponent();
            this.Text = this.Text + " " + host + " " + port;
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
				// If you don't start the service on a new thread, the WCF calls will not act concurrently.
				Task.Factory.StartNew(() => this.fuelsManagerService.Start());

				this.btnStart.Enabled = false;
				this.btnStop.Enabled = true;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
				this.fuelsManagerService.Stop();

				this.btnStart.Enabled = true;
				this.btnStop.Enabled = false;
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.btnStop.Enabled)
			{
				this.fuelsManagerService.Stop();
			}

			this.btnStart.Enabled = true;
			this.btnStop.Enabled = false;
		}
	}
}
