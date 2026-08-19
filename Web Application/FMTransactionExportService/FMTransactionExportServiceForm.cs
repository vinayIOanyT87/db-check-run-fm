using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMTransactionExportService
{
	public partial class FMTransactionExportServiceInProcForm : Form
	{
		private readonly FMTransactionExportService fuelsManagerService;

		public FMTransactionExportServiceInProcForm()
		{
			this.fuelsManagerService = new FMTransactionExportService();
			InitializeComponent();
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
				this.fuelsManagerService.Kill();

				this.btnStart.Enabled = true;
				this.btnStop.Enabled = false;
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.btnStop.Enabled)
			{
				this.fuelsManagerService.Kill();
			}

			this.btnStart.Enabled = true;
			this.btnStop.Enabled = false;
		}
	}
}
