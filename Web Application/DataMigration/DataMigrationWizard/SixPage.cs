using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Wizard.UI;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace DataMigration
{
    public partial class SixPage : Wizard.UI.ExternalWizardPage
    {
        DAService dbAdminConnect;
       
        public SixPage()
        {
            InitializeComponent();
            dbAdminConnect = new DAService();
        }

        private void SixPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Back | WizardButtons.Finish);

            ((Wizard.UI.WizardSheet)GetWizard()).finishButton.Enabled = false;
            ((Wizard.UI.WizardSheet)GetWizard()).backButton.Enabled = false;
           // ((Wizard.UI.WizardSheet)GetWizard()).cancelButton.Enabled = true;         
            BeginInvoke(new InvokeDelegate(InvokeMethod));          
        }
        
        private void ErrorHandler(string err)
        { 
            this.ProceesInfoLbl.AppendText("\n\n**** ERROR : " + err + ".\n");
            this.ProceesInfoLbl.AppendText("Migrating data is terminated with error.\n");
            ((Wizard.UI.WizardSheet)GetWizard()).finishButton.Enabled = true;            
           //  ((Wizard.UI.WizardSheet)GetWizard()).cancelButton.Enabled = false;
            
             DAService dbAdminConnect = new DAService();
             dbAdminConnect.ExcuteStoredProcedure("dbo.Migration_ClearDBUsers", "ConsolidatedDB", "Master", 0);
             if (File.Exists(DAService.ConsolidatedDBBackupFile))
                dbAdminConnect.RestoreDB("ConsolidatedDB", DAService.ConsolidatedDBBackupFile);
        }

        private void SixPage_WizardBack(object sender, WizardPageEventArgs e)
        {         
            e.NewPage = "ThirdPage";
        }

        public delegate void InvokeDelegate();

        public void InvokeMethod()
        {
            System.Diagnostics.Trace.WriteLine("InvokeMethod.");
            ((Wizard.UI.WizardSheet)GetWizard()).finishButton.Refresh();
            this.ProceesInfoLbl.Refresh();
            ProcessDataMigration();
        }

        private void ProcessDataMigration()
        {
            // this.Sidebar.BackgroundImage = new Bitmap(this.GetType(), "Bitmaps.Sidebar.bmp");
            try
            {
                this.ProceesInfoLbl.Text = "Migrating data... \n";

                if (File.Exists(DAService.ConsolidatedDBBackupFile))
                    File.Delete(DAService.ConsolidatedDBBackupFile);

                dbAdminConnect.BackupDabase8(DAService.ConsolidatedDBBackupFile);

                if(DAService.Site != null)
                    this.ProceesInfoLbl.AppendText("Site: " + DAService.Site + " " + DateTime.Now.ToString() + " \n");
                else
                    this.ProceesInfoLbl.AppendText(DateTime.Now.ToString() + " \n");

                if (ProcessingDataMigration("[dbo].Migration_DisableTriggers") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_SetBaseLevelSiteID") != "") return; 
                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDB6To8Site_1") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDB6To8Users_2") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDB6To8UserGroupMap_3") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDBtblEntityToSiteMap_4") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_CreateLoginUserRole_5") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDB6To8Products") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_ConsolidatedDBtblGroupTransactionAliasMapping") != "") return;

                if (ProcessingDataMigration("[dbo].Migration_FMD6ConsumersToFMD8ShipTo") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_FMD6SuppliersToFMD8Suppliers") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_FMD6VendorsToFMD8Carriers") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_FMD6ShippersToFMD8Shippers") != "") return; 

                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDB6To8EquipmentTypes") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDB6To8Equipment") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDB6EmployeesTo8Personnel_6") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_ConsolidatedDB6OperatorsTo8Personnel") != "") return;

                if (ProcessingDataMigration("[dbo].Migration_AviationDBTrainingToConsolidatedDB8_3") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AviationDB6To8QualityTag_1") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AviationDB6To8TestResults") != "") return;                
                if (ProcessingDataMigration("[dbo].Migration_AviationDB6To8Appointments") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AviationDB6QCAssignedTo8EqQualityTagLog") != "") return;                
                if (ProcessingDataMigration("[dbo].Migration_AviationDB6To8MaintenanceLog") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AviationDB6To8ControllersLog") != "") return;

                if (ProcessingDataMigration("[dbo].Migrate_FMD6CustomerAccountsToFMD8FuelCards") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6CommonRequestToFMD8FuelCards") != "") return;

                if (ProcessingDataMigration("[dbo].Migration_AviationDB6ControlLogTo8Transactions") != "") return;

                if (ProcessingDataMigration("[dbo].Migration_AccountingDB6RefContractTo8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AccountingDB6RefTransferTo8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AccountingDB6ReissueTo8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AccountingDB6ReceiveTo8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AccountingDB6InflightTo8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_AccountingDB6RegradeTo8Transactions") != "") return; 
                if (ProcessingDataMigration("[dbo].Migration_AccountingDB6CommercialTo8Transactions") != "") return; 

                if (ProcessingDataMigration("[dbo].Migrate_FMD6SALETransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SALEFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SALEUserDataToFMD8UserData") != "") return;

                if (ProcessingDataMigration("[dbo].Migrate_FMD6DEFUELTransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6DEFUELFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6DEFUELUserDataToFMD8UserData") != "") return;

                if (ProcessingDataMigration("[dbo].Migrate_FMD6DETERMINETransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6DETERMINEFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6DETERMINEUserDataToFMD8UserData") != "") return;
                                
                if (ProcessingDataMigration("[dbo].Migrate_FMD6REQUESTTransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6REQUESTFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6REQUESTUserDataToFMD8UserData") != "") return;

                if (ProcessingDataMigration("[dbo].Migrate_FMD6ADJUSTTransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6ADJUSTFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6ADJUSTUserDataToFMD8UserData") != "") return;

                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentTransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentUserDataToFMD8UserData") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentTransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentUserDataToFMD8UserData") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentTransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentUserDataToFMD8UserData") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentTransactionToFMD8Transactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentFieldsToFMD8LineItems") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentUserDataToFMD8UserData") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_FMD6SHIPMENTWithShippingDocumentFieldsToFMD8LineItemUserData") != "") return;
                
                if (ProcessingDataMigration("[dbo].Migrate_FMD6TransactionNotesToFMD8TransactionNotes") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_CreateEmptyTransactionWeightReadings") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_SetTransactionDocumentNumbers") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_LinkReversalTransactions") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_SetInhibitAccountingFlagForProducts") != "") return;
                if (ProcessingDataMigration("[dbo].Migrate_EnableDisableFuelCards") != "") return;
                if (ProcessingDataMigration("[dbo].Migration_EnableTriggers") != "") return;
                
                this.ProceesInfoLbl.AppendText("Migrating data is successfully finished. \n");
            }
            catch (Exception ex)
            {
                ((DataMigrationWizardSheet)GetWizard()).logger.Error("SixPage: ProcessDataMigration. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("SixPage: ProcessDataMigration. {0}", ex.Message));
                try
                {
                    ProcessingDataMigration("[dbo].Migrate_EnableTriggers");
                }
                catch (Exception ex2)
                {
                    ((DataMigrationWizardSheet)GetWizard()).logger.Error("SixPage: ProcessDataMigration. " + ex2.Message);
                    System.Diagnostics.Trace.WriteLine(String.Format("SixPage: ProcessDataMigration. {0}", ex2.Message));
                }
            }
            ((Wizard.UI.WizardSheet)GetWizard()).finishButton.Enabled = true;
            int iSel = (int)DataMigrationWizardSheet.DataMigrationSel;
                ((Wizard.UI.WizardSheet)GetWizard()).backButton.Enabled = true;
        }

        private string ProcessingDataMigration(string StoredProcedure)
        {
            string[] msg = StoredProcedure.Split('_');
            string start = "Start migrating " + msg[1] + " " + DateTime.Now.ToShortTimeString() + ". \n";
            this.ProceesInfoLbl.AppendText(start);
            string err = dbAdminConnect.ExcuteStoredProcedure(StoredProcedure, DAService.Site, "ConsolidatedDB", 0);
            string end = "End migrating " + msg[1] + " " + DateTime.Now.ToShortTimeString() + ". \n";
            this.ProceesInfoLbl.AppendText(end);
            if (err != "")
            {
                ErrorHandler(err);
                return err;
            }
            else
            {
                this.ProceesInfoLbl.AppendText(msg[1] + " is finished. \n");
            }
            return "";
        }

        private void SixPage_WizardFinish(object sender, CancelEventArgs e)
        {
            try
            {
                string text = this.ProceesInfoLbl.Text;
                text = text.Replace("Migrating data... \n", "");                

                RegistryKey Key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Varec\\InstallDetails", false);
                string logFile = (string)Key.GetValue("CommonDir");
                logFile = Path.Combine(logFile, "DataMigrationLog.txt");

                StreamWriter tw;
                FileInfo fileInfo= new FileInfo(logFile);
                if (File.Exists(logFile))
                {
                    tw = File.AppendText(logFile);
                }
                else
                    tw = fileInfo.CreateText();

                text = text.Replace("\n", tw.NewLine);
                tw.WriteLine(text);
                tw.Close();

                Process p = new Process();
                p.StartInfo.FileName = logFile;
                p.Start();
                p.WaitForExit();

            }           
            catch (Exception ex)
            {
                ((DataMigrationWizardSheet)GetWizard()).logger.Error("SixPage_WizardFinish. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("SixPage_WizardFinish. {0}", ex.Message));
            }
        }
    }
}
