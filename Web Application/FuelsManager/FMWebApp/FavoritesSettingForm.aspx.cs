// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FavoritesSettingForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FavoritesSettingForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FMCore;

    /// <summary>
	///    A web page for configuring the list of user Favorites, or the list of
	///    Quick Links. Request parameter of "QuickLinks=true" indicates that Quick
	///    Links should be edited.
	/// </summary>
	public partial class FavoritesSettingForm : FMFormBase
	{
		#region Constants and Fields

		public const string SessionFavorites = "FavoritesSettingForm.Favorites";

		public const string SessionIsQuickLinks = "FavoritesSettingForm.IsQuickLinks";

		// The collection of Favorites or Quick Links we're working with
		protected MenuFavoriteCollectionClass Favorites;

		// Whether or not we're working with Quick Links or Favorites (i.e. My Menu/Favorites)
		protected bool IsQuickLinks;

		#endregion

		#region Methods

		/// <summary>
		///    Bind grid to collection
		/// </summary>
		protected void BindControls()
		{
			this.dgFavorites.DataSource = this.Favorites;
			this.dgFavorites.DataBind();
		}

		/// <summary>
		///    Enable/disable controls during grid item editing
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			this.btnCancel.Enabled = enable;
			this.btnOK.Enabled = enable;
		}

		/// <summary>
		///    Hook up events
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			this.dgFavorites.EditCommand += this.DgFavoritesEditCommand;
			this.dgFavorites.PageIndexChanged += this.DgFavoritesPageIndexChanged;
			this.dgFavorites.CancelCommand += this.DgFavoritesCancelCommand;
			this.dgFavorites.UpdateCommand += this.DgFavoritesUpdateCommand;
			this.dgFavorites.DeleteCommand += this.DgFavoritesDeleteCommand;
			this.dgFavorites.ItemDataBound += this.DgFavoritesItemDataBound;
			this.dgFavorites.ItemCommand += this.DgFavoritesItemCommand;

			base.OnInit(e);
		}

		/// <summary>
		///    Load security info and data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					this.IsQuickLinks = (this.Request.GetQueryOrFormValue("QuickLinks") == "true");
					this.Session[SessionIsQuickLinks] = this.IsQuickLinks;
					if (this.IsQuickLinks)
					{
						this.lblTitle.Text = "Quick Links Configuration";
					}

					// Load from database
					this.Favorites =
						FMChannelHelper.MakeCall<IMenuFavorites, MenuFavoriteCollectionClass>(
							x => x.EnumerateByUserAndIsQuickLink(this.Security, this.Security.UserGuid, this.IsQuickLinks));

					this.Session[SessionFavorites] = this.Favorites;

					this.BindControls();
				}
				else
				{
					this.IsQuickLinks = (bool)this.Session[SessionIsQuickLinks];
					this.Favorites = (MenuFavoriteCollectionClass)this.Session[SessionFavorites];
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Leave page without saving data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void BtnCancelClick(object sender, EventArgs e)
		{
			this.Session.Remove(SessionIsQuickLinks);
			this.Session.Remove(SessionFavorites);

			this.Redirect("FuelsManagerForm.aspx");
		}

		/// <summary>
		///    Save data and leave page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void BtnOkClick(object sender, EventArgs e)
		{
			try
			{
				// Load from database
				MenuFavoriteCollectionClass origCollection =
					FMChannelHelper.MakeCall<IMenuFavorites, MenuFavoriteCollectionClass>(
						x => x.EnumerateByUserAndIsQuickLink(this.Security, this.Security.UserGuid, this.IsQuickLinks));

				// Delete deleted ones
				if (origCollection.Count != this.Favorites.Count)
				{
					foreach (MenuFavoriteClass origMenuFav in origCollection)
					{
						if (this.Favorites.Find(x => (x.IdentityGuid == origMenuFav.IdentityGuid)) == null)
						{
							this.PurgeFavoritesProxy(this.Security, origMenuFav.IdentityGuid);
						}
					}
				}

				// Update the rest. Do DisplayOrders from scratch, so there's less chance of concurrent
				// DB updates messing them up.
				int dispOrder = 1;
				foreach (MenuFavoriteClass menuFav in this.Favorites)
				{
					menuFav.DisplayOrder = dispOrder++;
					this.ModifyFavoritesProxy(this.Security, menuFav);
				}

				this.ucFMMenuBar.Refresh();

				this.Session.Remove(SessionIsQuickLinks);
				this.Session.Remove(SessionFavorites);

				this.Redirect("FuelsManagerForm.aspx");
			}
			catch (Exception except)
			{
                this.ErrorHandler(except);
			}
		}

		private void ModifyFavoritesProxy(SecurityClass securityClass, MenuFavoriteClass menuFav)
		{
			FMChannelHelper.MakeCall<IMenuFavorites>(
																	 x =>
																	 x.Modify(securityClass, menuFav)
																);
		}

		private void PurgeFavoritesProxy(SecurityClass securityClass, Guid guid)
		{
			FMChannelHelper.MakeCall<IMenuFavorites>(
																	 x =>
																	 x.Purge(securityClass, guid)
																);
		}

		/// <summary>
		///    Handle Move Up and Move Down grid item commands
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DgFavoritesItemCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				if (e.CommandName == "MoveUp")
				{
					// Determine the index of the item using the
					// current page and datagrid line
					int page = this.dgFavorites.CurrentPageIndex;
					int pageSize = this.dgFavorites.PageSize;
					int menuFavIndex = (page * pageSize) + e.Item.ItemIndex;
					if (menuFavIndex > 0)
					{
						this.Favorites.Reverse(menuFavIndex - 1, 2);

						// Bind the controls again
						this.BindControls();
					}
				}
				else if (e.CommandName == "MoveDown")
				{
					// Determine the index of the item using the
					// current page and datagrid line
					int page = this.dgFavorites.CurrentPageIndex;
					int pageSize = this.dgFavorites.PageSize;
					int menuFavIndex = (page * pageSize) + e.Item.ItemIndex;
					if (menuFavIndex < this.Favorites.Count - 1)
					{
						this.Favorites.Reverse(menuFavIndex, 2);

						// Bind the controls again
						this.BindControls();
					}
				}
			}
			catch (Exception except)
			{
                this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Bind event for each row of grid. Used to set "Path" of each menu item
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void DgFavoritesItemDataBound(object source, DataGridItemEventArgs eventArgs)
		{
			try
			{
				if (eventArgs.Item.DataItem != null)
				{
					var menuFav = (MenuFavoriteClass)eventArgs.Item.DataItem;
					var lblDisplayPath = (Label)eventArgs.Item.FindControl("lblDisplayPath");
					lblDisplayPath.Text = (this.Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData)?.GetMenuItemPath(
						menuFav.MenuItemType, menuFav.DynamicMenuItemGuid, this.useDataDictionary);

					if (string.IsNullOrEmpty(lblDisplayPath.Text))
					{
						// This means that in the current site, the user doesn't have this menu option
						lblDisplayPath.Text = "(Not Available for current site)";
						var btnEdit = (LinkButton)eventArgs.Item.FindControl("btnEdit");

					    if (btnEdit != null)
					    {
					        btnEdit.Visible = false;
					    }
					}

                    LinkButton deleteButton = eventArgs.Item.FindControl("btnDelete") as LinkButton;

                    if (deleteButton != null)
                    {
                        // If a row in the grid is being edited, disable the delete button for all rows in the grid
                        deleteButton.Enabled = this.dgFavorites.EditItemIndex == -1;
                    }

                    Button moveUpButton = eventArgs.Item.FindControl("btnMoveUp") as Button;

                    if (moveUpButton != null)
                    {
                        // If a row in the grid is being edited, disable the move up button for all rows in the grid
                        moveUpButton.Enabled = this.dgFavorites.EditItemIndex == -1;
                    }

                    Button moveDownButton = eventArgs.Item.FindControl("btnMoveDown") as Button;

                    if (moveDownButton != null)
                    {
                        // If a row in the grid is being edited, disable the move down button for all rows in the grid
                        moveDownButton.Enabled = this.dgFavorites.EditItemIndex == -1;
                    }
				}
			}
			catch (Exception except)
			{
                this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Cancel edit of grid item
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DgFavoritesCancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Put the item back in regular mode
				this.dgFavorites.EditItemIndex = -1;

				// Bind the controls again
				this.BindControls();

				this.EnableControls(true);
			}
			catch (Exception except)
			{
                this.ErrorHandler(except);
			}
		}

        /// <summary>
        /// Delete grid item
        /// </summary>
        /// <param name="source">The parameter is not used</param>
        /// <param name="e">Contains the index of the item being deleted</param>
		private void DgFavoritesDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Determine the index of the item using the
				// current page and datagrid line
				int page = this.dgFavorites.CurrentPageIndex;
				int pageSize = this.dgFavorites.PageSize;
				int menuFavIndex = (page * pageSize) + e.Item.ItemIndex;

				this.Favorites.RemoveAt(menuFavIndex);

                // If we are deleting the last item on a page, display the previous page, 
                // unless of course we're on the first page
                if (this.dgFavorites.Items.Count == 1 && this.dgFavorites.CurrentPageIndex > 0)
                {
                    this.dgFavorites.CurrentPageIndex--;
                }

				this.BindControls();
			}
			catch (Exception except)
			{
                this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Begin edit of grid item
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DgFavoritesEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Put the selected item into edit mode
				this.dgFavorites.EditItemIndex = e.Item.ItemIndex;

				this.BindControls();

				// Disable the controls
				this.EnableControls(false);
			}
			catch (Exception except)
			{
                this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Handle attempt to change page
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DgFavoritesPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.dgFavorites.EditItemIndex > -1)
				{
					return;
				}

				this.dgFavorites.CurrentPageIndex = e.NewPageIndex;
				this.BindControls();
			}
			catch (Exception except)
			{
                this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Confirm edit of grid item
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DgFavoritesUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get the menu item

			    // Determine the index of the item using the
				// current page and datagrid line
				int page = this.dgFavorites.CurrentPageIndex;
				int pageSize = this.dgFavorites.PageSize;
				int menuFavIndex = (page * pageSize) + e.Item.ItemIndex;
				var menuFav = this.Favorites[menuFavIndex];

				// Throw an exception if the item was not found
				if (menuFav == null)
				{
					throw new ApplicationException("Could not find the item to update.");
				}

				menuFav.CustomName = ((TextBox)e.Item.FindControl("txtCustomName")).Text;

				if (string.IsNullOrWhiteSpace(menuFav.CustomName))
				{
					menuFav.CustomName = "";
				}


				// Get out of edit mode
				this.dgFavorites.EditItemIndex = -1;

				this.BindControls();
				this.EnableControls(true);
			}
			catch (Exception except)
			{
                this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Override the help context key to detect whether we are displaying the form in quick links mode or not.
		/// </summary>
		/// <returns>The help context key of the page. If we are in quick links mode the key will indicate that
		/// by appending ?QuickLinks=true</returns>
		public override string GetHelpContextKey()
		{
			string helpContextKey = base.GetHelpContextKey();

			if (this.IsQuickLinks)
			{
				helpContextKey += "?QuickLinks=true";
			}

			return helpContextKey;
		}

        public override List<string> GetHelpContextKeys()
        {

            string helpContextKey = base.GetHelpContextKey();
            if (this.IsQuickLinks)
            {
                helpContextKey += "?QuickLinks=true";
            }
            List<string> list = new List<string>() { helpContextKey };
            return list;
        }


        #endregion
    }
}