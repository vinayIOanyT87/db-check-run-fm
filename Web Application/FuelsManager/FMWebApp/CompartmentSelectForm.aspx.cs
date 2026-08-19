// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompartmentSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompartmentSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

    using FMCore;
    using FMControls;

	/// <summary>
	///    Summary description for CompartmentSelectForm.
	/// </summary>
	public partial class CompartmentSelectForm : FMFormBase
	{
		#region Constants and Fields

		protected FMButton FindBtn;

		protected TextBox FindTextBox;

		protected FMButton ShowAllBtn;

		#endregion

		#region Methods

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
					if (this.Request.GetQueryOrFormValue("EquipmentID") != null)
					{
						this.Session["CompartmentSelectForm.EquipmentID"] = this.Request.GetQueryOrFormValue("EquipmentID");
					}
					else
					{
						this.Session.Remove("CompartmentSelectForm.EquipmentID");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CompartmentDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				e.Item.Cells[0].Text = this.GetTranslatedText("Select");
				if (this.CompartmentDataGrid.Columns.Count > 0)
					this.CompartmentDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;
			}
			else
			{
				string ID = HttpUtility.HtmlDecode(e.Item.Cells[1].Text);

				string ToolTip = ((e.Item.Cells[2].Text != "&nbsp;") ? "Capacity " + e.Item.Cells[2].Text : "")
				                 + ((e.Item.Cells[3].Text != "&nbsp;") ? " Safe Fill " + e.Item.Cells[3].Text : "");

				var Select = new HtmlAnchor();
				Select.ID = "Select";
				Select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(ID) + "','" + HttpUtility.JavaScriptStringEncode(ToolTip) + "')");
				Image im = new Image();
				im.ImageUrl = "../FMWebApp/Images/Select.gif";
				im.BorderWidth = 0;
				im.Style.Add("align", "absmiddle");
				Select.Controls.Add(im);

				e.Item.Cells[0].Controls.Add(Select);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CompartmentDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.CompartmentDataGrid_ItemDataBound);
		}

		private void UpdateView()
		{
			var EquipmentCollection = new EquipmentCollectionClass();

			// EquipmentTextBoxID indicates load from TransactionDetail
			if (this.Session["CompartmentSelectForm.EquipmentID"] != null)
			{
				var EquipmentID = this.Session["CompartmentSelectForm.EquipmentID"] as string;

				if (!string.IsNullOrEmpty(EquipmentID))
				{
					EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.Security, x.GetIdentityGuid(this.Security, EquipmentID))
																);

					EquipmentCollection = equipment.CompartmentCollection;
				}
			}

			this.CompartmentDataGrid.DataSource = EquipmentCollection;
			this.CompartmentDataGrid.DataBind();
		}

		#endregion
	}
}