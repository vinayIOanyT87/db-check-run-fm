using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMNotificationService
{
    public partial class NotificationServiceForm : Form
    {
        private DirectoryWatcher dirWatcher;
        public NotificationServiceForm()
        {
            InitializeComponent();
            dirWatcher = new DirectoryWatcher();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            dirWatcher.ProxyStart();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            dirWatcher.ProxyStop();
        }
    }
}
