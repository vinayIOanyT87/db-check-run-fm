// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GatesForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for GatesForm.
	/// </summary>
	public partial class GatesForm : FMFormBase, IMenuDiscovery
	{
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
                // Depends Upon Intoplane, WebTicketing, Config Reference Data
                if ((options & 0x44040) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_SITES_GATES,
						RootMenuName = "Configuration",
						CategoryName = "Sites",
						ItemName = "Loading Locations",
						NavigateUrl = "GatesForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods

		protected void AddButton_Command(object sender, CommandEventArgs e)
		{
			var GateCollection = (GateCollectionClass)this.Session["GateCollection"];
			var Gate = new GateClass();

			GateCollection.Add(Gate);
			this.GateDataGrid.CurrentPageIndex = (GateCollection.Count - 1) / this.GateDataGrid.PageSize;
			this.GateDataGrid.EditItemIndex = (GateCollection.Count - 1) % this.GateDataGrid.PageSize;

			this.EnableControls(false);
			this.UpdateView();
		}

		/// <summary>
		///    This method enables and disables controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;
			this.GatesFormPageSizeDropDown.Enabled = enable;
		}

		protected void GateDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			var GateCollection = (GateCollectionClass)this.Session["GateCollection"];
			int Index = this.GateDataGrid.CurrentPageIndex * this.GateDataGrid.PageSize + e.Item.ItemIndex;
			GateClass Gate = GateCollection[Index];

			if (Gate.IdentityGuid == Guid.Empty)
			{
				GateCollection.RemoveAt(Index);

				if ((this.GateDataGrid.Items.Count == 1) && (this.GateDataGrid.CurrentPageIndex > 0))
				{
					this.GateDataGrid.CurrentPageIndex--;
				}
			}

			this.GateDataGrid.EditItemIndex = -1;
			this.EnableControls(true);
			this.UpdateView();
		}

		protected void GateDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				GateCollectionClass GateCollection;
				GateCollection = (GateCollectionClass)this.Session["GateCollection"];

				int Index = this.GateDataGrid.CurrentPageIndex * this.GateDataGrid.PageSize + e.Item.ItemIndex;
				GateClass Gate = GateCollection[Index];

				if (this.GateDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.GateDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.GateDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.GateDataGrid.EditItemIndex--;
				}

				// Non empty IdentityGuid indicates Gate has been committed to database
				if (Gate.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IGates>(x => x.Purge(this.Security, Gate.IdentityGuid));
				}

				GateCollection.RemoveAt(Index);
				if (this.GateDataGrid.Items.Count == 1 && this.GateDataGrid.CurrentPageIndex > 0)
				{
					this.GateDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void GateDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.EnableControls(false);
			this.GateDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.UpdateView();
		}

		protected void GateDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.GateDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.GateDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void GateDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				GateCollectionClass GateCollection;
				GateCollection = (GateCollectionClass)this.Session["GateCollection"];

				var IDTextBox = (TextBox)e.Item.FindControl("IDTextBox");
				var DescriptionTextBox = (TextBox)e.Item.FindControl("DescriptionTextBox");
				var ConcourseIDTextBox = (TextBox)e.Item.FindControl("ConcourseIDTextBox");

				int Index = this.GateDataGrid.CurrentPageIndex * this.GateDataGrid.PageSize + this.GateDataGrid.EditItemIndex;
				GateClass Gate = GateCollection[Index];

				Gate.ID = IDTextBox.Text;
				Gate.Description = DescriptionTextBox.Text;
				Gate.ConcourseID = ConcourseIDTextBox.Text;

				if (Gate.IdentityGuid == Guid.Empty)
				{
					Gate.IdentityGuid = FMChannelHelper.MakeCall<IGates, Guid>(x => x.Add(this.Security, Gate));
				}
				else
				{
					FMChannelHelper.MakeCall<IGates>(x => x.Modify(this.Security, Gate));
				}

				this.EnableControls(true);
				this.GateDataGrid.EditItemIndex = -1;
				this.Session.Remove("GateCollection");
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

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
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					this.Session.Remove("GateCollection");
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void GateDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var EditButton = (LinkButton)e.Item.FindControl("EditButton");
			var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			var siteGuidLabel = (Label)e.Item.FindControl("SiteGuidLabel");

			if ((EditButton != null) && (DeleteButton != null) && (siteGuidLabel != null))
			{
				if ((!this.Security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				    || (this.Security.SiteGuid != Guid.Parse(siteGuidLabel.Text)))
				{
					EditButton.Enabled = false;
					DeleteButton.Enabled = false;
				}
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButton_Command;
			this.GateDataGrid.EditCommand += this.GateDataGrid_EditCommand;
			this.GateDataGrid.PageIndexChanged += this.GateDataGrid_PageIndexChanged;
			this.GateDataGrid.CancelCommand += this.GateDataGrid_CancelCommand;
			this.GateDataGrid.UpdateCommand += this.GateDataGrid_UpdateCommand;
			this.GateDataGrid.DeleteCommand += this.GateDataGrid_DeleteCommand;
			this.GateDataGrid.ItemDataBound += this.GateDataGrid_ItemDataBound;
			this.AddButton.Command += this.AddButton_Command;
		}

		private void UpdateView()
		{
			try
			{
				if (this.Session["GateCollection"] == null)
				{
					this.Session["GateCollection"] =
						FMChannelHelper.MakeCall<IGates, GateCollectionClass>(x => x.Enumerate(this.Security));
				}

				var GatesCollection = (GateCollectionClass)this.Session["GateCollection"];

				this.GatesFormPageSizeDropDown.SetPageSize(this.GateDataGrid, GatesCollection.Count);

				this.GateDataGrid.DataSource = GatesCollection;
				this.GateDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion
	}
}