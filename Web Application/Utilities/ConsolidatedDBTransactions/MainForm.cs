using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConsolidatedDBTransactions
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ApplicationSettings.Instance.DataSource = txtBoxDataSource.Text;
            ApplicationSettings.Instance.InitialCatalog = txtBoxInitialCatalog.Text;
        }

        private void txtBoxDataSource_TextChanged(object sender, EventArgs e)
        {
            ApplicationSettings.Instance.DataSource = txtBoxDataSource.Text;
        }

        private void txtBoxInitialCatalog_TextChanged(object sender, EventArgs e)
        {
            ApplicationSettings.Instance.InitialCatalog = txtBoxInitialCatalog.Text;
        }

        private void btnWriteTransaction_Click(object sender, EventArgs e)
        {
            SelectTransactionForm form = new SelectTransactionForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                string filename = SelectFileName("XML");
                if (!string.IsNullOrEmpty(filename))
                {
                    XmlWriteMode mode = WriteSchema() ? XmlWriteMode.WriteSchema : XmlWriteMode.IgnoreSchema;

                    ApplicationSettings appSettings = ApplicationSettings.Instance;
                    DAL dal = new DAL(appSettings.DataSource, appSettings.InitialCatalog);
                    DataSet ds = dal.GetTransactions(form.SelectedTransIDs);
                    ds.WriteXml(filename, mode);
                }
            }
        }

        private void btnWriteSchema_Click(object sender, EventArgs e)
        {
            string filename = SelectFileName("XSD");
            if (!string.IsNullOrEmpty(filename))
            {
                ApplicationSettings appSettings = ApplicationSettings.Instance;
                DAL dal = new DAL(appSettings.DataSource, appSettings.InitialCatalog);
                DataSet ds = dal.CreateNewDataSet();
                ds.WriteXmlSchema(filename);
            }
        }

        private void btnWriteDataSet_Click(object sender, EventArgs e)
        {
            string filename = SelectFileName("XML");
            if (!string.IsNullOrEmpty(filename))
            {
                XmlWriteMode mode = WriteSchema() ? XmlWriteMode.WriteSchema : XmlWriteMode.IgnoreSchema;

                ApplicationSettings appSettings = ApplicationSettings.Instance;
                DAL dal = new DAL(appSettings.DataSource, appSettings.InitialCatalog);
                DataSet ds = dal.GetAllTransactions();
                ds.WriteXml(filename, mode);
            }
        }

        private void btnWriteResultsSchema_Click(object sender, EventArgs e)
        {
            string filename = SelectFileName("XSD");
            if (!string.IsNullOrEmpty(filename))
            {
                ApplicationSettings appSettings = ApplicationSettings.Instance;
                DAL dal = new DAL(appSettings.DataSource, appSettings.InitialCatalog);
                DataSet ds = dal.CreateNewResultDataSet();
                ds.WriteXmlSchema(filename);
            }
        }

        private string SelectFileName(string extention)
        {
            string filename = string.Empty;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.DefaultExt = extention;
            dlg.Filter = @"XML Files(*.XML)|*.XML|XSD Files(*.XSD)|*.XSD|All files (*.*)|*.*";

            if (string.Compare(extention, "XML", true) == 0)
                dlg.FilterIndex = 1;
            else if (string.Compare(extention, "XSD", true) == 0)
                dlg.FilterIndex = 2;

            if (dlg.ShowDialog(this) == DialogResult.OK)
                filename = dlg.FileName;

            return filename;
        }

        private bool WriteSchema()
        {
            string msg = "Do you wish to persist the schema with the data?";
            return (MessageBox.Show(msg, "Persist Schema?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
        }
    }
}