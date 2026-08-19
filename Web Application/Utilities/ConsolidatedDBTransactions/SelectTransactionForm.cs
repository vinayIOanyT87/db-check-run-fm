using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace ConsolidatedDBTransactions
{
    public partial class SelectTransactionForm : Form
    {
        private DataSet _ds = null;
        private StringCollection _selectedTransIDs = new StringCollection();

        public SelectTransactionForm()
        {
            InitializeComponent();
        }

        public string[] SelectedTransIDs
        {
            get
            {
                string[] transIDs = new string[_selectedTransIDs.Count];
                _selectedTransIDs.CopyTo(transIDs, 0);
                return transIDs;
            }
        }

        private void SelectTransactionForm_Load(object sender, EventArgs e)
        {
            ApplicationSettings appSettings = ApplicationSettings.Instance;
            DAL dal = new DAL(appSettings.DataSource, appSettings.InitialCatalog);
            _ds = dal.GetAllTransactions();
            DataTable tbl = _ds.Tables["tblTransactions"];
            gridTransactions.DataSource = tbl;
        }

        private void gridTransactions_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = gridTransactions.SelectedRows;
            if (rows.Count > 0)
            {
                _selectedTransIDs.Clear();
                foreach (DataGridViewRow row in rows)
                    _selectedTransIDs.Add(row.Cells[0].Value.ToString());

                btnOK.Enabled = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }
    }
}