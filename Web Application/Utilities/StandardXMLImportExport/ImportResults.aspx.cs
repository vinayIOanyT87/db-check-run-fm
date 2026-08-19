using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using FM7Accounting;
using XMLImport;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for ImportResults.
	/// </summary>
	public class ImportResults : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid resultsGrid;
		protected System.Web.UI.WebControls.Image FadeImage;
		protected System.Web.UI.WebControls.Label I;
		protected System.Web.UI.WebControls.Label ImportResultsLabel;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			System.Data.DataSet ds;
			if( ! IsPostBack)
			{
				AccountingSecurity accountingSecurity = new AccountingSecurity();
				accountingSecurity.GetSecurity(Session["Token"] as string);

				ApplyDataDictionary();

				ImportFilter filter = (ImportFilter) Session["ImportFilter"];
				Session.Remove("ImportFilter");
				System.IO.Stream inputStream = (System.IO.Stream) Session["ImportFileStream"];
				Session.Remove("ImportFileStream");
				ImportProcessor importProcessor = new ImportProcessor();
				ImportValidationResults results = 
					importProcessor.Import(accountingSecurity, filter, accountingSecurity.CurrentSiteID, inputStream);


				ds = new DataSet();
				System.Data.DataTable table = new DataTable("table1");
				table.Columns.Add("TransID");
				table.Columns.Add("Level");
				table.Columns.Add("Message");

				ds.Tables.Add(table);
				foreach(TransactionValidationResult result in results)
				{
					foreach(string error in result.ErrorList)
					{
						System.Data.DataRow row = table.NewRow();
						row[0] = result.TransID;
						row[1] = "Error";
						row[2] = error;
						table.Rows.Add(row);
					}
					foreach(string warning in result.WarningList)
					{
						System.Data.DataRow row = table.NewRow();
						row[0] = result.TransID;
						row[1] = "Warning";
						row[2] = warning;
						table.Rows.Add(row);
					}
				}
				Session.Add("ImportResults", ds);

			}
			else
			{
				ds = (System.Data.DataSet) Session["ImportResults"];
			}

			this.resultsGrid.DataSource = ds;
			this.resultsGrid.DataBind();
		}

		private void ApplyDataDictionary()
		{
			// Determine if the data dictionary shall be used.  True indicates that it
			// shall be used and false otherwise.
			bool useDataDictionary=false;
			if(Page.Session["UseDataDictionary"] == null
			|| (bool) Page.Session["UseDataDictionary"])
				useDataDictionary=true;

			DataDictionary dd = new DataDictionary(new AccountingSecurity().GetSecurity(Session["Token"] as string),useDataDictionary);

			foreach(System.Web.UI.WebControls.DataGridColumn column in this.resultsGrid.Columns)
			{
				column.HeaderText = dd.getNameFromGlobalDictionary(column.HeaderText);
			}

			this.ImportResultsLabel.Text = dd.getNameFromGlobalDictionary(this.ImportResultsLabel.Text);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.resultsGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.resultsGrid_PageIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void resultsGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.resultsGrid.CurrentPageIndex = e.NewPageIndex;
			this.resultsGrid.DataBind();
		}
	}
}
