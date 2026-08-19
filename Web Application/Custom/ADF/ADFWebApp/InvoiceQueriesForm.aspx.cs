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
using FMControls;

using Accounting;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;

namespace ADFWebApp
{
	public class InvoiceQueriesContext
	{
		public InvoiceQueriesContext ( )
		{
			FMChannelFactory<IAccountingSites> accountingSitesClient = new FMChannelFactory<IAccountingSites> ( );
			this.AcctSites = accountingSitesClient.CreateProxy ( );
			this.AcctSite = new AccountingSite ( );
			this.Keyword = "";
			this.Collection = new InvoiceQueryCollectionClass ( );
		}

		#region Properties
		public IAccountingSites AcctSites { get; set; }
		public AccountingSite AcctSite { get; set; }
		public string Keyword { get; set; }
		public InvoiceQueryCollectionClass Collection { get; set; }
		#endregion // Properties

		public static string CONTEXT_KEY = typeof ( InvoiceQueriesContext ).ToString ( );
	}

	public partial class InvoiceQueriesForm : AccountingWebFormView
	{
		#region Attributes
		protected AccountingSite m_accountingSite = null;
		protected bool m_isAdding;
		#endregion // Attributes

		protected void Page_Load ( object sender, EventArgs e )
		{
			try
			{
				Processing_PageLoad ( );
			}
			catch (Exception ex)
			{
				base.ErrorHandler ( ex );
			}
		}

		#region Event processing
		protected void Processing_PageLoad ( )
		{
			// check the presence of security
			base.security = Session["Security"] as SecurityClass;
			if (null == base.security)
			{
				base.ErrorHandler ( new FMSessionInvalidException ( ) );
			}

			InvoiceQueriesContext context = this.GetContext ( );

			context.AcctSite.GetUserCompanies = false;
			context.AcctSite = context.AcctSites.LoadSiteInfo(base.security, base.security.SiteGuid);

			this.CheckSecurity ( );

			m_isAdding = false;

			if (!Page.IsPostBack)
			{
				this.EnableDisableControls ( true );
				this.BindControls ( );
				this.UpdateView ( );
			}

			// add context back to our session
		}

		protected void Processing_Add ( )
		{
			try
			{
				m_isAdding = true;

				InvoiceQueriesContext context = this.GetContext ( );
				context.Collection.Add ( new InvoiceQueryClass ( ) );
				this.StoreContext ( context );

				this.InvoiceQueriesDataGrid.CurrentPageIndex = ( context.Collection.Count - 1 ) / this.InvoiceQueriesDataGrid.PageSize;
				this.InvoiceQueriesDataGrid.EditItemIndex = ( context.Collection.Count - 1 ) % this.InvoiceQueriesDataGrid.PageSize;

				this.EnableDisableControls ( false );
				this.UpdateView ( );
			}
			catch (Exception e)
			{
				base.ErrorHandler ( e );
			}
		}

		protected void Processing_Refresh ( )
		{
			try
			{
				// go back to first page when keyword changed because the currently selected page may no longer exist
				this.InvoiceQueriesDataGrid.CurrentPageIndex = 0;
				InvoiceQueriesContext context = this.GetContext ( );
				context = this.LoadToContext ( ref context );
				this.StoreContext ( context );

				this.UpdateView ( );
			}
			catch (Exception e)
			{
				base.ErrorHandler ( e );
			}
		}
		#endregion // Event processing

		#region Context operations
		protected InvoiceQueriesContext GetContext ( )
		{
			InvoiceQueriesContext context = Session[InvoiceQueriesContext.CONTEXT_KEY] as InvoiceQueriesContext;
			if (null == context)
			{
				context = new InvoiceQueriesContext ( );
			}

			return context;
		}

		protected void StoreContext ( InvoiceQueriesContext a_context )
		{
			Session[InvoiceQueriesContext.CONTEXT_KEY] = a_context;
		}

		protected InvoiceQueriesContext LoadToContext ( ref InvoiceQueriesContext a_context )
		{
			a_context.Keyword = this.txtSearchText.Value;

			return a_context;
		}

		protected void LoadFromContext ( InvoiceQueriesContext a_context )
		{
			this.txtSearchText.Value = a_context.Keyword;
		}
		#endregion // Context operations

		protected void UpdateView ( )
		{
			try
			{
				FMChannelFactory<IInvoiceQueries> invQueriesClient = new FMChannelFactory<IInvoiceQueries> ( );
				IInvoiceQueries queries = invQueriesClient.CreateProxy ( );

				// get all the data to be displayed
				InvoiceQueriesContext context = this.GetContext ( );
				if (!m_isAdding)
				{
					if (0 == this.txtSearchText.Value.Length)
					{
						context.Collection = queries.Enumerate ( base.security );
					}
					else
					{
						context.Collection = queries.EnumerateByKeyword ( base.security, this.txtSearchText.Value );
					}
					// save context back to session
					this.StoreContext ( context );
				}

				DataView dw = this.BuildDataView ( context.Collection );

				// configure paging (+1 because of the adding of a new query)
				this.ddlPageSize.SetPageSize ( this.InvoiceQueriesDataGrid, context.Collection.Count + 1 );

				this.InvoiceQueriesDataGrid.DataSource = dw;
				this.InvoiceQueriesDataGrid.DataBind ( );
			}
			catch (Exception e)
			{
				base.ErrorHandler ( e );
			}
		}

		protected DataView BuildDataView ( InvoiceQueryCollectionClass a_col )
		{
			DataView result = null;

			// create a matching table
			DataTable table = new DataTable ( );
			table.Columns.Add ( "ID", typeof ( Int32 ) );
			table.Columns.Add ( "Description", typeof ( string ) );

			// add one row per query in the collection
			foreach (InvoiceQueryClass query in a_col)
			{
				DataRow row = table.NewRow ( );

				row["ID"] = query.Index;
				row["Description"] = query.Description == null ? "" : query.Description;

				table.Rows.Add ( row );
			}

			result = new DataView ( table );

			return result;
		}

		protected bool CheckSecurity ( )
		{
			btnAddBottom.Enabled = base.security.HasRight ( RIGHT.MODIFY_INVOICE_QUERIES );
			btnAddTop.Enabled = base.security.HasRight ( RIGHT.MODIFY_INVOICE_QUERIES );

			return true;
		}

		protected void BindControls ( )
		{
			// bind form events
			this.btnAddBottom.Click += new EventHandler ( btnAddBottom_Click );
			this.btnAddTop.Click += new EventHandler ( btnAddTop_Click );
			this.btnRefresh.Click += new EventHandler ( btnRefresh_Click );
			this.btnShowAll.Click += new EventHandler ( btnShowAll_Click );
			this.ddlPageSize.SelectedIndexChanged += new EventHandler ( ddlPageSize_SelectedIndexChanged );
		}

		protected override void OnInit ( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent ( );
			base.OnInit ( e );
			base.init ( );
		}


		private void InitializeComponent ( )
		{
			// bind data grid events
			this.InvoiceQueriesDataGrid.ItemCommand += new DataGridCommandEventHandler ( InvoiceQueriesDataGrid_ItemCommand );
			this.InvoiceQueriesDataGrid.EditCommand += new DataGridCommandEventHandler ( InvoiceQueriesDataGrid_EditCommand );
			this.InvoiceQueriesDataGrid.UpdateCommand += new DataGridCommandEventHandler ( InvoiceQueriesDataGrid_UpdateCommand );
			this.InvoiceQueriesDataGrid.CancelCommand += new DataGridCommandEventHandler ( InvoiceQueriesDataGrid_CancelCommand );
			this.InvoiceQueriesDataGrid.ItemDataBound += new DataGridItemEventHandler ( InvoiceQueriesDataGrid_ItemDataBound );
			this.InvoiceQueriesDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler ( InvoiceQueriesDataGrid_PageIndexChanged );
		}

		void InvoiceQueriesDataGrid_ItemCommand ( object source, DataGridCommandEventArgs e )
		{

		}

		protected void InvoiceQueriesDataGrid_PageIndexChanged ( object source, DataGridPageChangedEventArgs e )
		{
			try
			{
				if (this.InvoiceQueriesDataGrid.EditItemIndex > -1)
					return;
				this.InvoiceQueriesDataGrid.CurrentPageIndex = e.NewPageIndex;

				this.UpdateView ( );
			}
			catch (Exception ex)
			{
				base.ErrorHandler ( ex );
			}
		}

		protected void InvoiceQueriesDataGrid_ItemDataBound ( object sender, DataGridItemEventArgs e )
		{
			try
			{
				LinkButton EditButton = (LinkButton) e.Item.FindControl ( "EditLinkButton" );
				LinkButton DeleteButton = (LinkButton) e.Item.FindControl ( "DeleteLinkButton" );

				if (e.Item.ItemIndex != -1)
				{
					//((System.Data.DataRowView)(e.Item.DataItem)).Row.ItemArray
					string queryIndex = "";
					string Description = "";

					// Leave hard space zero length string
					DataRowView view = e.Item.DataItem as DataRowView;
					if (view.Row.ItemArray[0] != null)
					{
						queryIndex = ( (int) view.Row.ItemArray[0] ).ToString ( );
						Description = (string) view.Row.ItemArray[1];
					}

					HtmlAnchor Select = new HtmlAnchor ( );
					Select.ID = "Select";
					Select.HRef = "javascript:Select('" + queryIndex + "','" + Description + "')";
					Select.InnerHtml = "<img src=\"../FMWebApp/Images/Select.gif\" border=\"0\" align=\"absmiddle\" alt='Select this item'>";

					e.Item.Cells[0].Controls.Add ( Select );
				}

				if (this.InvoiceQueriesDataGrid != null && this.InvoiceQueriesDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					Control ctrl = e.Item.FindControl ( "EditLinkButton" );

					if (ctrl != null)
					{
						string script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
						Page.ClientScript.RegisterStartupScript ( this.GetType ( ), "page_set_focus", string.Format ( script, ctrl.ClientID ) );
					}

				}
			}
			catch (Exception except)
			{
				base.ErrorHandler ( except );
			}
		}

		protected void InvoiceQueriesDataGrid_CancelCommand ( object source, DataGridCommandEventArgs e )
		{
			if (m_isAdding)
			{
				InvoiceQueriesContext context = this.GetContext ( );

				// if adding, remove the end item
				if (context.Collection.Count > 0)
				{
					context.Collection.Remove ( context.Collection.Count - 1 );
					this.StoreContext ( context );
				}
				m_isAdding = false;
			}
			this.EnableDisableControls ( true );

			this.InvoiceQueriesDataGrid.EditItemIndex = -1;

			this.UpdateView ( );
		}

		protected void InvoiceQueriesDataGrid_UpdateCommand ( object source, DataGridCommandEventArgs e )
		{
			// invoked on data grid save
			// get the relevant controls
			TextBox txtEditID = e.Item.FindControl ( "txtEditQueryID" ) as TextBox;
			TextBox txtEditDescription = e.Item.FindControl ( "txtEditDescription" ) as TextBox;

			if (txtEditID != null && txtEditDescription != null)
			{
				InvoiceQueryDO query = new InvoiceQueryDO ( );

				if (txtEditID.Text.Length > 0)
				{
					query.InvoiceQueryGuid = int.Parse ( txtEditID.Text );
				}

				if (txtEditDescription.Text.Length > 0)
				{
					query.Description = txtEditDescription.Text;
				}

				if (query.InvoiceQueryGuid != 0)
				{
					// if updating, then should restore created dates

					FMChannelFactory<IInvoiceQueries> invQueriesClient = new FMChannelFactory<IInvoiceQueries> ( );
					IInvoiceQueries queries = invQueriesClient.CreateProxy ( );

					InvoiceQueryClass oldQuery = queries.GetByIndex ( base.security, query.InvoiceQueryGuid );

					query.CreatedBy = oldQuery.CreatedBy;
					query.CreatedDate = oldQuery.CreatedDate;
				}

				// try to save it
				try
				{
					SaveInvoiceQuerySR sr = new SaveInvoiceQuerySR ( base.security );
					sr.Security = base.security;
					sr.InvoiceQueries.Add ( query );

					FMChannelFactory<ISaveInvoiceQueryProcessor> saveInvProcessorClient = new FMChannelFactory<ISaveInvoiceQueryProcessor> ( );
					ISaveInvoiceQueryProcessor saveInvProcessor = saveInvProcessorClient.CreateProxy ( );

					CustomResultDO result = saveInvProcessor.Process ( sr );

					if (result.Errors.Count > 0)
					{
						// just throw the first one
						throw result.Errors[0];
					}
				}
				catch (Exception ex)
				{
					base.ErrorHandler ( ex );
				}
			}

			if (m_isAdding)
			{
				// if adding, remove the end item
				InvoiceQueriesContext context = this.GetContext ( );
				if (context.Collection.Count > 0)
				{
					context.Collection.Remove ( context.Collection.Count - 1 );
					StoreContext ( context );
				}
				m_isAdding = false;
			}

			this.EnableDisableControls ( true );

			this.InvoiceQueriesDataGrid.EditItemIndex = -1;

			// refresh the view to reflect changes
			this.UpdateView ( );
		}

		protected void InvoiceQueriesDataGrid_EditCommand ( object source, DataGridCommandEventArgs e )
		{
			// invoked on data grid edit

			if (base.security.HasRight ( RIGHT.MODIFY_INVOICE_QUERIES ))
			{
				try
				{
					this.InvoiceQueriesDataGrid.EditItemIndex = e.Item.ItemIndex;
					this.EnableDisableControls ( false );
					this.UpdateView ( );
				}
				catch (Exception ex)
				{
					base.ErrorHandler ( ex );
				}
			}
		}

		protected void EnableDisableControls ( bool a_enable )
		{
			btnAddBottom.Enabled = a_enable && base.security.HasRight ( RIGHT.MODIFY_INVOICE_QUERIES );
			btnAddTop.Enabled = a_enable && base.security.HasRight ( RIGHT.MODIFY_INVOICE_QUERIES );
			btnShowAll.Enabled = a_enable;
			btnRefresh.Enabled = a_enable;
			txtSearchText.Disabled = !a_enable;
			ddlPageSize.Enabled = a_enable;
		}

		protected void ddlPageSize_SelectedIndexChanged ( object sender, EventArgs e )
		{
			this.UpdateView ( );
		}

		protected void btnShowAll_Click ( object sender, EventArgs e )
		{
			this.txtSearchText.Value = "";

			this.btnRefresh_Click ( sender, e );
		}

		protected void btnRefresh_Click ( object sender, EventArgs e )
		{
			InvoiceQueriesContext context = this.GetContext ( );
			this.LoadToContext ( ref context );
			this.StoreContext ( context );

			this.Processing_Refresh ( );
		}

		protected void btnAddTop_Click ( object sender, EventArgs e )
		{
			this.Processing_Add ( );
		}

		protected void btnAddBottom_Click ( object sender, EventArgs e )
		{
			this.Processing_Add ( );
		}
	}
}
