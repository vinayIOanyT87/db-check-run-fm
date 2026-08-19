using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Wizard.UI;
using System.IO;

namespace DataMigration
{
    public partial class SecondPage : Wizard.UI.InternalWizardPage
    {
        public SecondPage()
        {
            InitializeComponent();
        }

        private void BaseBrowseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDlg = new OpenFileDialog();
            int iSel = (int)DataMigrationWizardSheet.DataMigrationSel;
            if (iSel != 2)
            {
                OpenFileDlg.Filter = "zip files (*.zip)|*.zip|All files(*.*)|*.*";
                // else
                //   OpenFileDlg.Filter = "bak files (*.bak)|*.bak|All files(*.*)|*.*";

                if (OpenFileDlg.ShowDialog() == DialogResult.OK)
                {
                    BaseFileName.Text = OpenFileDlg.FileName;
                }
            }
        }

        private void EnterpriseBrowseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDlg = new OpenFileDialog();
            OpenFileDlg.Filter = "bak files (*.bak)|*.bak|All files(*.*)|*.*";
            if (OpenFileDlg.ShowDialog() == DialogResult.OK)
            {
                EnterpriseFileName.Text = OpenFileDlg.FileName;                
            }
        }

        private void SecondPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            int iSel = (int)DataMigrationWizardSheet.DataMigrationSel;
           // if ((BaseFileName.Text == "") || ((iSel == 2) && (EnterpriseFileName.Text == "")))
            if ((iSel != 2)&&(BaseFileName.Text == ""))
            {
                MessageBox.Show("Please select database backup file.", "FuelsManager Data Migration", 
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }           

            string err = ((DataMigrationWizardSheet)GetWizard()).Error;

            DAService dbAdminConnect = new DAService();

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if ((int)DataMigrationWizardSheet.DataMigrationSel != 2)
                    dbAdminConnect.RestoreBaseDB(BaseFileName.Text);
               
                /*
                else
                {
                    if (File.Exists(BaseFileName.Text))
                        dbAdminConnect.RestoreDB("ConsolidatedDB6", BaseFileName.Text);
                    if (File.Exists(EnterpriseFileName.Text))
                        dbAdminConnect.RestoreDB("AccountingDB6", EnterpriseFileName.Text);
                    if (File.Exists(AviationDBFilename.Text))
                        dbAdminConnect.RestoreDB("AviationDB6", AviationDBFilename.Text);
                }
                */
                if (dbAdminConnect.Error != "")
                {
                    ((DataMigrationWizardSheet)GetWizard()).Error = dbAdminConnect.Error;
                }
                else
                {
                    if ((err == "") && ((int)DataMigrationWizardSheet.DataMigrationSel != 2))
                    {
                        DataSet dataSet = dbAdminConnect.DuplicatedUsers("ConsolidatedDB");
                        DataTable dataTable = dataSet.Tables[0];
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            string str = string.Format("User {0} is duplicated. \n", dataTable.Rows[i]["UserID"].ToString() );
                            err += str;
                        }
                    }
                    
                    ((DataMigrationWizardSheet)GetWizard()).Error = err;
                }
            }
            catch (Exception ex)
            {

                ((DataMigrationWizardSheet)GetWizard()).logger.Error("SecondPage_WizardNext. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("SecondPage_WizardNext. {0}", ex.Message));
            }
            Cursor.Current = Cursors.Default;  
            if (iSel != 2)
            {
                if (((DataMigrationWizardSheet)GetWizard()).Error == "")
                    e.NewPage = "FifthPage";
                else
                    e.NewPage = "FourthPage";
            }
        }

        private void SecondPage_SetActive(object sender, CancelEventArgs e)
        {
            int iSel = (int)DataMigrationWizardSheet.DataMigrationSel;
            BaseFileName.Text = "";
            EnterpriseFileName.Text = "";
            AviationDBFilename.Text = "";
            FirstLbl.Text = "Zip file containing backups of the three base level databases";
            EnterpriseFileName.Hide();
            EnterpriseBrowseBtn.Hide();
            Secendlbl.Hide();
            AviationDBFilename.Hide();
            AviationDBBtn.Hide();
            Thirdlbl.Hide();
            if (iSel != 2)
            {
                BaseFileName.Enabled = true;
            }
            else
            {
                BaseFileName.Enabled = false;
                /*
                FirstLbl.Text = "ConsolidatedDB to be migrated";
                Secendlbl.Text = "AccountingDB to be migrated";
                Thirdlbl.Text = "AviationDB to be migrated";                
                Secendlbl.Show();
                EnterpriseFileName.Show();
                EnterpriseBrowseBtn.Show();
                Thirdlbl.Show();
                AviationDBFilename.Show();
                AviationDBBtn.Show();*/
            }
            try
            {
                DAService dbAdminConnect = new DAService();
                Cursor.Current = Cursors.WaitCursor;
                dbAdminConnect.ExcuteStoredProcedure("dbo.Migration_DropStuff", "", "ConsolidatedDB", 0);
                dbAdminConnect.RestoreStoredProcedures(true);
                Cursor.Current = Cursors.Default;                
            }
            catch (Exception ex)
            {
                ((DataMigrationWizardSheet)GetWizard()).logger.Error("SecondPage_SetActive. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("SecondPage_SetActive. {0}", ex.Message));
                MessageBox.Show(ex.Message);
            }
        }

        private void AviationDBBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDlg = new OpenFileDialog();
            OpenFileDlg.Filter = "bak files (*.bak)|*.bak|All files(*.*)|*.*";
            if (OpenFileDlg.ShowDialog() == DialogResult.OK)
            {
                AviationDBFilename.Text = OpenFileDlg.FileName;
            }
        }        
    }
}
