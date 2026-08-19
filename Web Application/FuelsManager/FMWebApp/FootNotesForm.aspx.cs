// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FootNotesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FootNotesForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for FootNotesForm.
	/// </summary>
	public partial class FootNotesForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IFootNotes);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.FOOTNOTE;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			    && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			{
				return null;
			}

			// After checking the other rights, let this right rule supreme.
			if (security.HasRight(RIGHT.CONFIGURE_FOOTNOTES) == false)
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_OTHER_FOOTNOTES,
						RootMenuName = "Configuration",
						CategoryName = "Other",
						ItemName = "Footnotes",
						NavigateUrl = "FootNotesForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply,
					});

			return items;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			FootNoteCollectionClass FootNoteCollection;
			FootNoteCollection = FMChannelHelper.MakeCall<IFootNotes, FootNoteCollectionClass>(x => x.Enumerate(Security));

			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (FootNoteClass FootNote in FootNoteCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == FootNote.SiteGuid)
					{
						continue;
					}

					if (Security.LoginSiteGuid != FootNote.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (Security.SiteGuid != FootNote.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(FootNote);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}
			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IFootNotes, Guid>(x => x.GetIdentityGuid(security, ID));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			FootNoteClass FootNote = FMChannelHelper.MakeCall<IFootNotes, FootNoteClass>(x => x.Get(security, guid));

			FootNote.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IFootNotes>(x => x.Modify(security, FootNote));
		}

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

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["FootNotesPage"] != null)
					{
						this.FootNotesDataGrid.CurrentPageIndex = (int)this.Session["FootNotesPage"];
						this.Session.Remove("FootNotesPage");
					}
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				this.Session.Remove("IdentityGuid");
				this.Session["FootNotesPage"] = this.FootNotesDataGrid.CurrentPageIndex;
				this.Redirect("FootNoteForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection EnumerateFootNotes()
		{
			FootNoteCollectionClass FootNoteCollection =
				FMChannelHelper.MakeCall<IFootNotes, FootNoteCollectionClass>(x => x.Enumerate(this.Security));

			var FootNoteDataTable = new DataTable();
			DataRow FootNoteDataRow;

			FootNoteDataTable.Columns.Add("SiteGuid", typeof(Guid));
			FootNoteDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			FootNoteDataTable.Columns.Add("ID", typeof(string));

			foreach (FootNoteClass FootNote in FootNoteCollection)
			{
				FootNoteDataRow = FootNoteDataTable.NewRow();

				FootNoteDataRow["SiteGuid"] = FootNote.SiteGuid;
				FootNoteDataRow["IdentityGuid"] = FootNote.IdentityGuid;
				FootNoteDataRow["ID"] = FootNote.ID;

				FootNoteDataTable.Rows.Add(FootNoteDataRow);
			}
			var FootNoteDataView = new DataView(FootNoteDataTable);
			return FootNoteDataView;
		}

		private void FootNotesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[2];//bds

				FMChannelHelper.MakeCall<IFootNotes>(x => x.Purge(this.Security, Guid.Parse(indexCell.Text)));
				this.FootNotesDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.FootNotesDataGrid.Items.Count == 1 && this.FootNotesDataGrid.CurrentPageIndex > 0)
				{
					this.FootNotesDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void FootNotesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell guidCell = e.Item.Cells[2];//bds
			this.Session["IdentityGuid"] = guidCell.Text;
			this.Session["FootNotesPage"] = this.FootNotesDataGrid.CurrentPageIndex;
			this.Redirect("FootNoteForm.aspx");
		}

		private void FootNotesDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
				if (DeleteButton != null)
				{
					TableCell SiteGuidCell = e.Item.Cells[1];//bds
					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS) || this.Security.SiteGuid != Guid.Parse(SiteGuidCell.Text))
					{
						DeleteButton.Enabled = false;
						DeleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
					}
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		private void FootNotesDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.FootNotesDataGrid.EditItemIndex > -1)
				{
					return;
				}
				this.FootNotesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButton_Command;
			this.AddButton.Command += this.AddButton_Command;
			this.FootNotesDataGrid.EditCommand += this.FootNotesDataGrid_EditCommand;
			this.FootNotesDataGrid.PageIndexChanged += this.FootNotesDataGrid_PageIndexChanged;
			this.FootNotesDataGrid.DeleteCommand += this.FootNotesDataGrid_DeleteCommand;
			this.FootNotesDataGrid.ItemDataBound += this.FootNotesDataGrid_ItemDataBound;
		}

		private void UpdateView()
		{
			ICollection Groups = this.EnumerateFootNotes();

			this.FootNotesFormPageSizeDropDown.SetPageSize(this.FootNotesDataGrid, Groups.Count);

			this.FootNotesDataGrid.DataSource = Groups;
			this.FootNotesDataGrid.DataBind();
		}

		#endregion
	}
}