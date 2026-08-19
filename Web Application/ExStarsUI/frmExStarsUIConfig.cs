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
	public partial class frmExStarsUIConfig : Form
	{
		public frmExStarsUIConfig()
		{
			InitializeComponent();
			tbUserLogin.Text = Properties.Settings.Default.UserId;
			tbPassword.Text = Properties.Settings.Default.UserPassword;
			tbSite.Text = Properties.Settings.Default.AviationSite;
		}

		public void loadValues(ref string userId, ref string password, ref string site)
		{
			userId = Properties.Settings.Default.UserId;
			password = Properties.Settings.Default.UserPassword;
			site = Properties.Settings.Default.AviationSite;
		}


		private void btnSave_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.UserId = tbUserLogin.Text;
			Properties.Settings.Default.UserPassword = tbPassword.Text;
			Properties.Settings.Default.AviationSite = tbSite.Text;
			Properties.Settings.Default.Save();
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}
	}
}
