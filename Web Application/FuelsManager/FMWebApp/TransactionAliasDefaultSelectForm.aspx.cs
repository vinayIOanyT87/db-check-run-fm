// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TankSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	public partial class TransactionAliasDefaultSelectForm : FMFormBase
	{
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Page.IsPostBack == false)
				{
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.DefaultDataGrid.ItemDataBound +=
				this.DefaultDataGridItemDataBound;
		}

		/// <summary>
		///    This method create all the links for the tank list and places them
		///    on the client side.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefaultDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				e.Item.Cells[0].Text = this.GetTranslatedText("Select");
				if (this.DefaultDataGrid.Columns.Count > 0)
				{
					this.DefaultDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;
				}
			}
			else
			{
				string id = "";
				string name = "";

				// Leave hard space zero length string
				if (e.Item.Cells[1].Text != "&nbsp;")
				{
					id = HttpUtility.HtmlDecode(e.Item.Cells[1].Text);
				}
				if (e.Item.Cells[2].Text != "&nbsp;")
				{
					name = HttpUtility.HtmlDecode(e.Item.Cells[2].Text);
				}

				var select = new HtmlAnchor
				{
					ID = "Select",
					HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(id) + "','" + HttpUtility.JavaScriptStringEncode(name) + "')")
				};
				Image im = new Image
				{
					ImageUrl = "../FMWebApp/Images/Select.gif",
					BorderWidth = 0
				};
				im.Style.Add("align", "absmiddle");
				select.Controls.Add(im);

				e.Item.Cells[0].Controls.Add(select);
			}
		}

		private void UpdateView()
		{
			List<TransactionAliasDefaultsSelection> defaultsCollection = new List<TransactionAliasDefaultsSelection> {
				new TransactionAliasDefaultsSelection { DefaultSelectionId = TransactionAliasDefaultsType.Aviation },
				new TransactionAliasDefaultsSelection { DefaultSelectionId = TransactionAliasDefaultsType.TerminalAutomation,
																		DefaultSelectionName = "Terminal Automation"}
			};

			this.DefaultDataGrid.DataSource = defaultsCollection;
			this.DefaultDataGrid.DataBind();
		}
	}
}