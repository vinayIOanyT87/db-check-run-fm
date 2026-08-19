using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ConsolidatedDBTransactions
{
    internal class DAL
    {
        #region Private Data Members

        private const string _xActDataSetName = @"Transactions";
        private const string _xActTableName = @"tblTransactions";
        private const string _xActLineItemsTableName = @"tblTransactionLineItems";
        private const string _xActSubLineItemsTableName = @"tblTransactionSubLineItems";
        private const string _xActUserDataTableName = @"tblTransactionUserData";
        private const string _xActNotesTableName = @"tblTransactionNotes";
        private const string _xActWeightReadingsTableName = @"tblTransactionWeightReadings";

        private string _strConnection;
        private SqlConnection _sqlConnection;

        #endregion Private Data Members

        #region Public Methods

        internal DAL(string dataSource, string initialCatalog)
        {
            _strConnection = string.Format("data source={0};initial catalog={1};integrated security=sspi",
                                           dataSource, initialCatalog);

        }

        internal string ConnectionString
        {
            get { return _strConnection; }
            set { _strConnection = value; }
        }

        internal DataSet CreateNewResultDataSet()
        {
            DataSet ds = new DataSet("B2BResults");

            DataTable resultTable = CreateResultsTable();
            ds.Tables.Add(resultTable);

            return ds;
        }

        /// <summary>
        /// This table does not yet exist in ConsolidatedDB..
        /// </summary>
        /// <returns></returns>
        internal DataTable CreateResultsTable()
        {
            string tblName = @"tblB2BResults";

            string strSelect = @"SELECT * FROM " + tblName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);

            DataTable tbl = new DataTable(tblName);
            adapter.FillSchema(tbl, SchemaType.Mapped);

            return tbl;
        }

        /// <summary>
        /// Creates a new empty DataSet containing the transactions table and the child tables
        /// related to transactions. This DataSet is created based on the schema of the database
        /// defined by the connection string.
        /// </summary>
        /// <returns></returns>
        internal DataSet CreateNewDataSet()
        {
            DataSet ds = new DataSet(DAL._xActDataSetName);

            DataTable xActTbl = CreateXActsTable();
            ds.Tables.Add(xActTbl);

            DataTable xActLineItemsTbl = CreateXActLineItemsTable();
            ds.Tables.Add(xActLineItemsTbl);

            DataTable xActSubLineItemsTbl = CreateXActSubLineItemsTable();
            ds.Tables.Add(xActSubLineItemsTbl);

            DataTable xActNotesTbl = CreateXActNotesTable();
            ds.Tables.Add(xActNotesTbl);

            DataTable xActUserDataTbl = CreateXActUserDataTable();
            ds.Tables.Add(xActUserDataTbl);

            DataTable xActWeightReadingsTbl = CreateXActWeightReadings();
            ds.Tables.Add(xActWeightReadingsTbl);

            // Create a foreign key relationship between the transaction and the transaction
            // line item tables.
            DataColumn[] fkParentCols = xActTbl.PrimaryKey;
            DataColumn[] fkChildCols = new DataColumn[] { xActLineItemsTbl.Columns["TransID"] };
            ForeignKeyConstraint fkc = new ForeignKeyConstraint(CreateFKName(xActTbl, xActLineItemsTbl), fkParentCols, fkChildCols);
            xActLineItemsTbl.Constraints.Add(fkc);

            // Create a foreign key relationship between the transction line item and the
            // transaction sub line items tables.
            fkParentCols = xActLineItemsTbl.PrimaryKey;
            fkChildCols = new DataColumn[] { xActSubLineItemsTbl.Columns["TransLineItemID"],
                                             xActSubLineItemsTbl.Columns["TransID"] };
            fkc = new ForeignKeyConstraint(CreateFKName(xActLineItemsTbl, xActSubLineItemsTbl), fkParentCols, fkChildCols);
            xActSubLineItemsTbl.Constraints.Add(fkc);

            // Create a foreign key relationship between the transaction and the transaction
            // notes tables.
            fkParentCols = xActTbl.PrimaryKey;
            fkChildCols = new DataColumn[] { xActNotesTbl.Columns["TransID"] };
            fkc = new ForeignKeyConstraint(CreateFKName(xActTbl, xActNotesTbl), fkParentCols, fkChildCols);
            xActNotesTbl.Constraints.Add(fkc);

            // Create a foreign key relationship between the transaction and the transaction
            // user data tables.
            fkParentCols = xActTbl.PrimaryKey;
            fkChildCols = new DataColumn[] { xActUserDataTbl.Columns["TransID"] };
            fkc = new ForeignKeyConstraint(CreateFKName(xActTbl, xActUserDataTbl), fkParentCols, fkChildCols);
            xActUserDataTbl.Constraints.Add(fkc);

            // Create a foreign key relationship between the transaction and the transaction
            // weight readings tables.
            fkParentCols = xActTbl.PrimaryKey;
            fkChildCols = new DataColumn[] { xActWeightReadingsTbl.Columns["TransID"] };
            fkc = new ForeignKeyConstraint(CreateFKName(xActTbl, xActWeightReadingsTbl), fkParentCols, fkChildCols);
            xActWeightReadingsTbl.Constraints.Add(fkc);

            return ds;
        }

        /// <summary>
        /// Creates a new DataSet populated with all of the transaction data from the database.
        /// </summary>
        /// <returns></returns>
        internal DataSet GetAllTransactions()
        {
            DataSet ds = CreateNewDataSet();

            FillXActsTable(ds.Tables[DAL._xActTableName]);
            FillXActLineItemsTable(ds.Tables[DAL._xActLineItemsTableName]);
            FillXActSubLineItemsTable(ds.Tables[DAL._xActSubLineItemsTableName]);
            FillXActUserDataTable(ds.Tables[DAL._xActUserDataTableName]);
            FillXActNotesTable(ds.Tables[DAL._xActNotesTableName]);
            FillXActWeightReadings(ds.Tables[DAL._xActWeightReadingsTableName]);

            return ds;
        }

        internal DataSet GetTransaction(string transID)
        {
            DataSet ds = CreateNewDataSet();
            GetTransaction(ds, transID);
            return ds;
        }

        internal DataSet GetTransactions(string[] transIDs)
        {
            DataSet ds = CreateNewDataSet();

            foreach (string transID in transIDs)
                GetTransaction(ds, transID);
            
            return ds;
        }

        #endregion Public Methods

        #region Create Table Methods

        /// <summary>
        /// Create a Transactions DataTable based on the current schema.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateXActsTable()
        {
            string strSelect = @"SELECT * FROM " + DAL._xActTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);

            DataTable tbl = new DataTable(DAL._xActTableName);
            adapter.FillSchema(tbl, SchemaType.Mapped);

            return tbl;
        }

        /// <summary>
        /// Create a TransacitonLineItems DataTable based on the current schema.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateXActLineItemsTable()
        {
            string strSelect = @"SELECT * FROM " + DAL._xActLineItemsTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);

            DataTable tbl = new DataTable(DAL._xActLineItemsTableName);
            adapter.FillSchema(tbl, SchemaType.Mapped);

            return tbl;
        }

        /// <summary>
        /// Create a new TransactionsSubLineItems DataTable based on the current schema.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateXActSubLineItemsTable()
        {
            string strSelect = @"SELECT * FROM " + DAL._xActSubLineItemsTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);

            DataTable tbl = new DataTable(DAL._xActSubLineItemsTableName);
            adapter.FillSchema(tbl, SchemaType.Mapped);

            return tbl;
        }

        /// <summary>
        /// Create a new TransactionNotes DataTable based on the current schema.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateXActNotesTable()
        {
            string strSelect = @"SELECT * FROM " + DAL._xActNotesTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);

            DataTable tbl = new DataTable(DAL._xActNotesTableName);
            adapter.FillSchema(tbl, SchemaType.Mapped);

            return tbl;
        }

        /// <summary>
        /// Create a new TransactionUserData DataTable based on the current schema.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateXActUserDataTable()
        {
            string strSelect = @"SELECT * FROM " + DAL._xActUserDataTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);

            DataTable tbl = new DataTable(DAL._xActUserDataTableName);
            adapter.FillSchema(tbl, SchemaType.Mapped);

            return tbl;
        }

        /// <summary>
        /// Create a new TransactionWeightReadings DataTable based on the current schema.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateXActWeightReadings()
        {
            string strSelect = @"SELECT * FROM " + DAL._xActWeightReadingsTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);

            DataTable tbl = new DataTable(DAL._xActWeightReadingsTableName);
            adapter.FillSchema(tbl, SchemaType.Mapped);

            return tbl;
        }

        #endregion Create Table Methods

        #region Fill Table Methods

        private void FillXActsTable(DataTable tbl)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActsTable(DataTable tbl, string transID)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActTableName + " WHERE [TransID]='" + transID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActLineItemsTable(DataTable tbl)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActLineItemsTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActLineItemsTable(DataTable tbl, string transID)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActLineItemsTableName + " WHERE [TransID]='" + transID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActSubLineItemsTable(DataTable tbl)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActSubLineItemsTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActSubLineItemsTable(DataTable tbl, string transID)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActSubLineItemsTableName + " WHERE [TransID]='" + transID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActNotesTable(DataTable tbl)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActNotesTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActNotesTable(DataTable tbl, string transID)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActNotesTableName + " WHERE [TransID]='" + transID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActUserDataTable(DataTable tbl)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActUserDataTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActUserDataTable(DataTable tbl, string transID)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActUserDataTableName + " WHERE [TransID]='" + transID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActWeightReadings(DataTable tbl)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActWeightReadingsTableName;
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        private void FillXActWeightReadings(DataTable tbl, string transID)
        {
            string strSelect = @"SELECT * FROM " + DAL._xActWeightReadingsTableName + " WHERE [TransID]='" + transID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(strSelect, Connection);
            adapter.Fill(tbl);
        }

        #endregion Fill Table Methods

        #region Private Helper Methods

        private SqlConnection Connection
        {
            get
            {
                if (_sqlConnection == null)
                    _sqlConnection = new SqlConnection(_strConnection);
                return _sqlConnection;
            }
        }

        /// <summary>
        /// Create a string that can be used as the name of a foreign key constraint. The name that
        /// is created is based on the name of the parent and child table passed as parameters.
        /// </summary>
        /// <param name="parentTbl"></param>
        /// <param name="childTable"></param>
        /// <returns></returns>
        private string CreateFKName(DataTable parentTbl, DataTable childTable)
        {
            return string.Format("FK_{0}_{1}", parentTbl.TableName, childTable.TableName);
        }

        private void GetTransaction(DataSet ds, string transID)
        {
            FillXActsTable(ds.Tables[DAL._xActTableName], transID);
            FillXActLineItemsTable(ds.Tables[DAL._xActLineItemsTableName], transID);
            FillXActSubLineItemsTable(ds.Tables[DAL._xActSubLineItemsTableName], transID);
            FillXActUserDataTable(ds.Tables[DAL._xActUserDataTableName], transID);
            FillXActNotesTable(ds.Tables[DAL._xActNotesTableName], transID);
            FillXActWeightReadings(ds.Tables[DAL._xActWeightReadingsTableName], transID);
        }

        #endregion Private Helper Methods
    }
}
