using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class ThirdPage : Wizard.UI.InternalWizardPage
    {
       // private List<string> sitesIn8;  
        public ThirdPage()
        {
            InitializeComponent();            
        }

        private void ThirdPage_WizardNext(object sender, Wizard.UI.WizardPageEventArgs e)
        {
            string err = ((DataMigrationWizardSheet)GetWizard()).Error;
            DAService.Site = (string)SitesList.SelectedItem;
            try
            {
                DAService dbAdminConnect = new DAService();
                DataSet dataSet = dbAdminConnect.DuplicatedSites(DAService.Site);
                DataTable dataTable = dataSet.Tables[0];

                if (dataTable.Rows.Count > 0)
                {
                    string msg;
                    msg = string.Format("The sites you selected already exist in FulesManager 8.0. \n Please select different one.");
                    MessageBox.Show(msg, "FuelsManager Data Migration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }

                if (DAService.Site == "All Sites")
                {
                    dataSet = dbAdminConnect.DuplicatedUsers("ConsolidatedDB6");
                    dataTable = dataSet.Tables[0];
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string str = string.Format("User {0} is duplicated {1} times. \n", dataTable.Rows[i]["UserID"].ToString(), dataTable.Rows[i]["Num"].ToString());
                        err += str;
                    }
                }

                if (err == "")
                {
                    dataSet = dbAdminConnect.DuplicatedUsers("ConsolidatedDB");
                    dataTable = dataSet.Tables[0];
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string str = string.Format("User {0} is duplicated. \n", dataTable.Rows[i]["UserID"].ToString());
                        err += str;
                    }
                }
               
            }
            catch (Exception ex)
            {

                ((DataMigrationWizardSheet)GetWizard()).logger.Error("ThirdPage: ThirdPage_WizardNext. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("ThirdPage: ThirdPage_WizardNext. {0}", ex.Message));
            }

            ((DataMigrationWizardSheet)GetWizard()).Error = err;

            if (err == "")
                e.NewPage = "FifthPage";
        }

        private void ShowSitesBtn_Click(object sender, EventArgs e)
        {
            ExistingSites dlg = new ExistingSites();
            dlg.ShowDialog();
        }

        private void ThirdPage_Load(object sender, EventArgs e)
        {
            SitesList.Items.Clear();
            SitesList.Items.Add("All Sites");
            DAService dbAdminConnect = new DAService();

            try
            {
                DataSet dataSet = dbAdminConnect.GetSites("ConsolidatedDB6");
                DataTable dataTable = dataSet.Tables[0];
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i]["SiteID"].ToString() == "SiteAdmin")
                        continue;
                    SitesList.Items.Add(dataTable.Rows[i]["SiteID"].ToString());
                }
            }
            catch (Exception ex)
            {

                ((DataMigrationWizardSheet)GetWizard()).logger.Error("ThirdPage: ThirdPage_Load. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("ThirdPage: ThirdPage_Load. {0}", ex.Message));
            }
            SitesList.SelectedItem = "All Sites";
        }
    }
}
