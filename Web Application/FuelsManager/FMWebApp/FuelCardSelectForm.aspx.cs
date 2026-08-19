// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FuelCardSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

    using FMCore;

	/// <summary>
	///    Summary description for FuelCardSelectForm.
	/// </summary>
	public partial class FuelCardSelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		protected FuelCardSelectContextClass FuelCardSelectContext;

		protected string SelectThisItemText;

		#endregion

		#region Methods

		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			this.FuelCardSelectContext.SearchString = null;
			this.FindTextBox.Text = "";
			this.UpdateView();
		}

		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			if (this.FindTextBox.Text.Length < 1)
			{
				this.FuelCardSelectContext.SearchString = null;
			}
			else
			{
				this.FuelCardSelectContext.SearchString = this.FindTextBox.Text.ToUpper();
			}

			this.UpdateView();
		}

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

				this.SelectThisItemText = this.GetTranslatedText("Select this item");

				if (this.Page.IsPostBack == false)
				{
					this.FuelCardSelectContext = new FuelCardSelectContextClass();

					if (this.Request.GetQueryOrFormValue("Mode") != null)
					{
						this.FuelCardSelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");
					}

					if (this.Request.GetQueryOrFormValue("All") != null)
					{
						this.FuelCardSelectContext.All = Convert.ToBoolean(this.Request.GetQueryOrFormValue("All"));
					}

					if (this.Request.GetQueryOrFormValue("Null") != null)
					{
						this.FuelCardSelectContext.Null = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Null"));
					}

					if (this.Request.GetQueryOrFormValue("Unassigned") != null)
					{
						this.FuelCardSelectContext.Unassigned = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Unassigned"));
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA) || this.FuelCardSelectContext.Mode == "Unassign")
					{
						this.AddButton1.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Request.GetQueryOrFormValue("SearchString") != null)
					{
						this.FuelCardSelectContext.SearchString = this.Request.GetQueryOrFormValue("SearchString");
						this.FindTextBox.Text = this.FuelCardSelectContext.SearchString;
					}

                    if (this.Request.GetQueryOrFormValue("HideHidden") != null)
                    {
                        this.FuelCardSelectContext.HideHidden = Convert.ToBoolean(this.Request.GetQueryOrFormValue("HideHidden"));
                    }

					this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_SELECT_CONTEXT] = this.FuelCardSelectContext;

					this.UpdateView();
				}
				else
				{
					this.FuelCardSelectContext =
						this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_SELECT_CONTEXT] as FuelCardSelectContextClass;
				}

				if (this.FuelCardSelectContext?.Mode != null)
				{
					var form1 = (HtmlForm)this.FindControl("Form1");
					var okButton = new HtmlInputButton();
					okButton.Attributes.Add("value", this.GetTranslatedText("OK"));
					okButton.Attributes.Add("id", "OkButton");
					okButton.Attributes.Add("class", "formfieldtitle");
					okButton.Attributes.Add("onclick", "MultipleSelect()");
					okButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
					form1.Controls.Add(okButton);

					var cancelButton = new HtmlInputButton();
					cancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
					cancelButton.Attributes.Add("id", "CancelButton");
					cancelButton.Attributes.Add("class", "formfieldtitle");
					cancelButton.Attributes.Add("onclick", "NoSelect()");
					cancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
					form1.Controls.Add(cancelButton);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var fuelCardArrayList = this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] as ArrayList;
			if (fuelCardArrayList == null)
			{
				fuelCardArrayList = new ArrayList();
				this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] = fuelCardArrayList;
			}

			var siteClass = FMChannelHelper.MakeCall<ISites, SiteClass>(
													sites => sites.Get(
														this.Security,
														this.Security.SiteGuid,
														getMemberSites: false,
														getSchedulesAndProcessVariables: false,
														bGetAssociatedAliases: false));


			var fuelCard = new FuelCardClass(siteClass);
			fuelCardArrayList.Add(fuelCard);

			if (this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_CONTEXT_ARRAY_LIST] == null)
			{
			    var fuelCardSelectContextArrayList = new ArrayList
			                                         {
			                                             this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_SELECT_CONTEXT]
			                                         };
			    this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_CONTEXT_ARRAY_LIST] = fuelCardSelectContextArrayList;
			}
			else
			{
				(this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_CONTEXT_ARRAY_LIST] as List<FuelCardSelectContextClass>)?.Add(
                    this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_SELECT_CONTEXT] as FuelCardSelectContextClass);
			}

			this.Redirect("../FuelCardWebApp/FCRC_DetailForm.aspx?CSRFToken=" + this.Security.CSRFToken);
		}

		private void FuelCardDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get identityGuid
				TableCell identityGuidCell = e.Item.Cells[3];//bds
				FMChannelHelper.MakeCall<IFuelCards>(x => x.Purge(this.Security, Guid.Parse(identityGuidCell.Text)));
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void FuelCardDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				TableCell identityGuidCell = e.Item.Cells[3];//bds

				var fuelCardArrayList = this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] as ArrayList;
				if (fuelCardArrayList == null)
				{
					fuelCardArrayList = new ArrayList();
					this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] = fuelCardArrayList;
				}

				// Get FuelCard
				FuelCardClass fuelCard =
					FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(
						x => x.Get(this.Security, Guid.Parse(identityGuidCell.Text), true));

				fuelCardArrayList.Add(fuelCard);

				if (this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_CONTEXT_ARRAY_LIST] == null)
				{
				    var fuelCardSelectContextArrayList = new List<FuelCardSelectContextClass>
				                                         {
				                                             this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_SELECT_CONTEXT] as
				                                                 FuelCardSelectContextClass
				                                         };
				    this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_CONTEXT_ARRAY_LIST] = fuelCardSelectContextArrayList;
				}
				else
				{
                    (this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_CONTEXT_ARRAY_LIST] as List<FuelCardSelectContextClass>)?.Add(
						this.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_SELECT_CONTEXT] as FuelCardSelectContextClass);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("../FuelCardWebApp/FCRC_DetailForm.aspx?" + this.Security.CSRFTokenWithParamName);
		}

		/// <summary>
		///    This method create all the links for the FuelCard list and places them
		///    on the client side.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FuelCardDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (e.Item.ItemType == ListItemType.Header)
				{
				    e.Item.Cells[0].Text = this.FuelCardSelectContext.Mode != null
				        ? this.GetTranslatedText(this.FuelCardSelectContext.Mode)
				        : this.GetTranslatedText("Select");

					if (this.FuelCardDataGrid.Columns.Count > 0)
						this.FuelCardDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;
				}
			}

			else
			{
				if (this.FuelCardSelectContext.Mode != null)
				{
					var select = new HtmlInputCheckBox();
					select.ID = "Select";
					e.Item.Cells[0].Controls.Add(select);
					select.Attributes.Add("Title", HttpUtility.JavaScriptStringEncode(this.FuelCardDataGrid.Columns[0].HeaderText + " " + ID));
					e.Item.Cells[4].Text = e.Item.Cells[4].Text.Replace(" ", "&nbsp;");//bds
				}
				else
				{
					string id = "";

					// Leave hard space zero length string
					if (e.Item.Cells[4].Text != "&nbsp;")//bds
					{
						id = HttpUtility.HtmlDecode(e.Item.Cells[4].Text);//bds
					}

					string toolTip = ((e.Item.Cells[5].Text != "&nbsp;") ? e.Item.Cells[5].Text + ", " : "")//bds
					                 + ((e.Item.Cells[6].Text != "&nbsp;") ? e.Item.Cells[6].Text + ", " : "");//bds

					var select = new HtmlAnchor();
					select.ID = "Select";
                    select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(id ?? string.Empty) + "','" + HttpUtility.JavaScriptStringEncode(toolTip) + "')");
					Image im = new Image();
					im.ImageUrl = "../FMWebApp/Images/Select.gif";
					im.BorderWidth = 0;
					im.Style.Add("align", "absmiddle");
					select.Controls.Add(im);

					e.Item.Cells[0].Controls.Add(select);
				}

				Guid siteGuid = Guid.Parse(e.Item.Cells[2].Text);//bds
				Guid fuelCardGuid = Guid.Parse(e.Item.Cells[3].Text);//bds

				var deleteButton = (LinkButton)e.Item.FindControl("Fmdeletelinkbutton1");
				if (deleteButton != null)
				{
					deleteButton.Enabled = (this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA) && this.Security.SiteGuid == siteGuid
					                        && fuelCardGuid != Guid.Empty && this.FuelCardSelectContext.Mode != "Unassign");
				}

				var editButton = (LinkButton)e.Item.FindControl("Fmeditlinkbutton1");
				if (editButton != null)
				{
					editButton.Enabled = ((this.FuelCardSelectContext.Mode != "Unassign") && (fuelCardGuid != Guid.Empty)
					                      && (this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA)
					                          || this.Security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)));
				}
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButtonCommand;
			this.FuelCardDataGrid.EditCommand += this.FuelCardDataGridEditCommand;
			this.FuelCardDataGrid.DeleteCommand += this.FuelCardDataGridDeleteCommand;
			this.FuelCardDataGrid.ItemDataBound += this.FuelCardDataGridItemDataBound;
			this.AddButton1.Command += this.AddButtonCommand;
		}

		private void UpdateView()
		{
			this.FindTextBox.Text = this.FuelCardSelectContext.SearchString;
			FuelCardCollectionClass fuelCardCollection;

			if (this.FindTextBox.Text != "")
			{
				fuelCardCollection =
					FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(
						x =>
						x.EnumerateFuelCardsByCompanyAndFilter(
							this.Security, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, this.FindTextBox.Text, hideHiddenFuelCards: this.FuelCardSelectContext.HideHidden ));
			}
			else
			{
				fuelCardCollection =
					FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(x => x.EnumerateFuelCards(this.Security, hideHiddenFuelCards: this.FuelCardSelectContext.HideHidden));
			}

			if (this.FuelCardSelectContext.Null)
			{
			    var fuelCard = new FuelCardClass { ID = "" };
			    fuelCardCollection.Insert(0, fuelCard);
			}

			if (this.FuelCardSelectContext.All)
			{
			    var fuelCard = new FuelCardClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}")) };
			    fuelCardCollection.Insert(0, fuelCard);
			}

			if (this.FuelCardSelectContext.Unassigned)
			{
			    var fuelCard = new FuelCardClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{Unassigned}")) };
			    fuelCardCollection.Insert(0, fuelCard);
			}

			var fuelCardDataTable = new DataTable();

		    fuelCardDataTable.Columns.Add("SiteGuid", typeof(Guid));
			fuelCardDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			fuelCardDataTable.Columns.Add("ID", typeof(string));
			fuelCardDataTable.Columns.Add("Manager", typeof(string));
			fuelCardDataTable.Columns.Add("ManagerTip", typeof(string));
			fuelCardDataTable.Columns.Add("Owner", typeof(string));
			fuelCardDataTable.Columns.Add("OwnerTip", typeof(string));
			fuelCardDataTable.Columns.Add("Shipper", typeof(string));
			fuelCardDataTable.Columns.Add("ShipperTip", typeof(string));
			fuelCardDataTable.Columns.Add("BillTo", typeof(string));
			fuelCardDataTable.Columns.Add("BillToTip", typeof(string));
			fuelCardDataTable.Columns.Add("ShipTo", typeof(string));
			fuelCardDataTable.Columns.Add("ShipToTip", typeof(string));
			fuelCardDataTable.Columns.Add("Provider", typeof(string));
			fuelCardDataTable.Columns.Add("Status", typeof(string));

			foreach (FuelCardClass fuelCard in fuelCardCollection)
			{
				var fuelCardDataRow = fuelCardDataTable.NewRow();

				fuelCardDataRow["SiteGuid"] = fuelCard.SiteGuid;
				fuelCardDataRow["IdentityGuid"] = fuelCard.IdentityGuid;
				fuelCardDataRow["ID"] = fuelCard.ID;
				fuelCardDataRow["Manager"] = HttpUtility.HtmlEncode(fuelCard.ManagerID);
				fuelCardDataRow["ManagerTip"] = HttpUtility.HtmlEncode(fuelCard.ManagerToolTip);
				fuelCardDataRow["Owner"] = HttpUtility.HtmlEncode(fuelCard.OwnerID);
				fuelCardDataRow["OwnerTip"] = HttpUtility.HtmlEncode(fuelCard.OwnerToolTip);
				fuelCardDataRow["Shipper"] = HttpUtility.HtmlEncode(fuelCard.ShipperID);
				fuelCardDataRow["ShipperTip"] = HttpUtility.HtmlEncode(fuelCard.ShipperToolTip);
				fuelCardDataRow["BillTo"] = HttpUtility.HtmlEncode(fuelCard.BillToID);
				fuelCardDataRow["BillToTip"] = HttpUtility.HtmlEncode(fuelCard.BillToToolTip);
				fuelCardDataRow["ShipTo"] = HttpUtility.HtmlEncode(fuelCard.ShipToID);
				fuelCardDataRow["ShipToTip"] = HttpUtility.HtmlEncode(fuelCard.ShipToToolTip);
				fuelCardDataRow["Provider"] = fuelCard.Provider;
				fuelCardDataRow["Status"] = fuelCard.Status;

				fuelCardDataTable.Rows.Add(fuelCardDataRow);
			}

			var fuelCardDataView = new DataView(fuelCardDataTable);

			this.FuelCardDataGrid.DataSource = fuelCardDataView;
			this.FuelCardDataGrid.DataBind();
		}

		#endregion
	}

	[Serializable]
	public class FuelCardSelectContextClass
	{
		//	public Type					MapType=null;

		#region Constants and Fields

		public bool All;

		//	public string				IDLink=null;
		//	public IDLINK_TYPE		IDLinkType=IDLINK_TYPE.NONE;
		//	public int					Map;
		public string Mode;

		public bool Null;

		public string SearchString;

		public bool Unassigned;

        /// <summary>
        /// If true, only fuel cards not marked as hidden will be returned
        /// </summary>
	    public bool HideHidden;

	    #endregion
	}
}