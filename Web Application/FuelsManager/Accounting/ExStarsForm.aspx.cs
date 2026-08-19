
namespace FuelsManager.Accounting
{
	using System;
	using FMBusinessObjects.DataObjects;

	public partial class ExStarsForm : AccountingWebFormView
	{
		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">
		/// The <see cref="System.EventArgs"/> instance containing the event data. 
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);




        }

		/// <summary>
		///   Required method for Designer support - do not modify
		///   the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += this.ExStarsFormLoad;			
		}

		private void ExStarsFormLoad(object sender, EventArgs e)
		{
			this.GetSecurity();
		    this.ExStarsCreateReports?.InitializeComponent();
		}

/*
		private void ApplyDataDictionary(System.Web.UI.Control parentControl)
		{
			//if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
			//{
			//	useDataDictionary = true;
			//}

			//if (!useDataDictionary || parentControl == null)
			//{
			//	return;
			//}

			//if (parentControl is FMUserControlBase)
			//{
			//	foreach (System.Web.UI.Control ctrl in (parentControl as FMUserControlBase).Controls)
			//	{
			//		ApplyDataDictionary(ctrl);
			//	}
			//}
			//else if (parentControl is FMButton)
			//{
			//	FMButton fmButton = (FMButton)parentControl;
			//	fmButton.Text = exStarsDataDictionary.GetText(fmButton.Text);
			//}
			//else if (parentControl is FMLabel)
			//{
			//	FMLabel fmLabel = (FMLabel)parentControl;
			//	fmLabel.Text = exStarsDataDictionary.GetText(fmLabel.Text);
			//}
			////else if (parentControl is FMDropDownList)
			////{
			////	FMDropDownList fmdd = parentControl as FMDropDownList;

			////}
			//else if (parentControl is FMDataGrid)
			//{
			//	FMDataGrid fmDataGrid = parentControl as FMDataGrid;
			//	foreach (System.Web.UI.WebControls.DataGridColumn column in fmDataGrid.Columns)
			//	{
			//		column.HeaderText = exStarsDataDictionary.GetText(column.HeaderText);
			//	}
			//}
		}
*/

	}
}