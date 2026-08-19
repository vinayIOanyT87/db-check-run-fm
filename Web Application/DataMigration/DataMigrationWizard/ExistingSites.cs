using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class ExistingSites : Form
    {
       // public List<string> sitesIn8;
        public ExistingSites()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ExistingSites_Load(object sender, EventArgs e)
        {
            DAService dbAdminConnect = new DAService();
            DataSet dataSet = dbAdminConnect.GetSites("ConsolidatedDB");
            DataTable dataTable = dataSet.Tables[0];
            for (int i = 0; i < dataTable.Rows.Count; i++)
           // for (int i = 0; i < sitesIn8.Count; i++) 
            {
                Siteslst.Items.Add(dataTable.Rows[i]["ID"].ToString());                
               // Siteslst.Items.Add(sitesIn8[i]);                
            }
        }
    }
}
