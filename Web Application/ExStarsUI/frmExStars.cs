using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExStarsUI
{
	public partial class frmExStars : Form
	{
		private string UserId;
		private string UserPassword;
		private string AviationSite;
		frmExStarsUIConfig UIConfig;
		WebBrowser browser;

		public frmExStars()
		{
			InitializeComponent();
			UIConfig = new frmExStarsUIConfig();
			browser = new WebBrowser();
		}

		protected void Init()
		{
			UIConfig.loadValues(ref UserId, ref UserPassword, ref AviationSite);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void stdMonthlyToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (UIConfig.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				UIConfig.loadValues(ref UserId, ref UserPassword, ref AviationSite);
			}

		}

		private void browserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.Location = new Point(10, 10);
			browser.MinimumSize = new System.Drawing.Size(20, 20);
			browser.MaximumSize = new System.Drawing.Size(this.Size.Width - 20, this.Size.Height - 20);
			browser.Url = new Uri( "http://www.google.com");

			//browser.Show();

		}
	}
}
