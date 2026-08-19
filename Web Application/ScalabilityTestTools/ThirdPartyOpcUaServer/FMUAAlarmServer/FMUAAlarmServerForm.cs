using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMUAAlarmServer
{
	using System.Threading;

	public partial class FMUAAlarmServerForm : Form
	{
		private readonly FMUAAlarmServerService fuelsManagerService = new FMUAAlarmServerService();

		private Thread myth;

        private string[] Args;

        public FMUAAlarmServerForm(string[] args)
		{
            Args = args;
			InitializeComponent();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{

			// If you don't start the service on a new thread, the WCF calls will not act concurrently.
			//Task.Factory.StartNew(() => this.fuelsManagerService.MyStart());


			myth = new Thread(this.fuelsManagerService.MyStart);
			myth.SetApartmentState(ApartmentState.STA);
			myth.Start(Args);

			this.btnStart.Enabled = false;
			this.btnStop.Enabled = true;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			this.fuelsManagerService.Stop();

			this.btnStart.Enabled = true;
			this.btnStop.Enabled = false;
		}
	}
}
