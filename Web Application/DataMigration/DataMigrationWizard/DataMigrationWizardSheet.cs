using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FMBusinessObjects.LogClient;

namespace DataMigration
{
    public enum DataMigrationOpt
    {
        BaseToBase, //0
        BaseToEnterprise, //1
        EnterpriseToEnterprise //2
    }

    public partial class DataMigrationWizardSheet : Wizard.UI.WizardSheet
    {
        public Logger logger;
        private static DataMigrationOpt dataMigrationSel = DataMigrationOpt.BaseToBase;            
        private string error = "";
        public DataMigrationWizardSheet()
        {
            InitializeComponent();
            this.Text = "FuelsManager Defense 6.0 to 8.0 Data Migration (Version " + Application.ProductVersion + ")";
            this.Pages.Add(new WelcomePage());
            this.Pages.Add(new FirstPage());
            this.Pages.Add(new SecondPage());            
            this.Pages.Add(new ThirdPage());
            this.Pages.Add(new FourthPage());
            this.Pages.Add(new FifthPage());
            this.Pages.Add(new SixPage());

            logger = new Logger("Data Migration DataMigrationWizard");
        }

        public static DataMigrationOpt DataMigrationSel
        {

            get { return dataMigrationSel; }
            set { dataMigrationSel = value; }
        }       

        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        private void DataMigrationWizardSheet_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DAService dbAdminConnect = new DAService();
                string err = "";
                err = dbAdminConnect.ExcuteStoredProcedure("dbo.Migration_DropStuff", "", "ConsolidatedDB", 0);
                err = dbAdminConnect.ExcuteStoredProcedure("dbo.Migration_ClearDBUsers", "ConsolidatedDB", "Master", 1);

                if (File.Exists(DAService.ConsolidatedDBBackupFile))
                    File.Delete(DAService.ConsolidatedDBBackupFile);
            }
            catch (Exception ex)
            {
                this.Error = ex.Message;
                logger.Error("DataMigrationWizardSheet: DataMigrationWizardSheet_FormClosing. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("DataMigrationWizardSheet: DataMigrationWizardSheet_FormClosing. {0}", ex.Message));
            }
        }

        private void DataMigrationWizardSheet_Load(object sender, EventArgs e)
        {
            try
            {
                DAService dbAdminConnect = new DAService();
                dbAdminConnect.RestoreStoredProcedures(false);
            }
            catch (Exception ex)
            {
                this.Error = ex.Message;
                logger.Error("DataMigrationWizardSheet: DataMigrationWizardSheet_Load. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("DataMigrationWizardSheet: DataMigrationWizardSheet_Load. {0}", ex.Message));
                MessageBox.Show(ex.Message);
            }
        }    
    }
}
