namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Net.Sockets;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	public class ApplicationStringsFormBase : FMFormBase
	{
		#region Constants and Fields

		protected int PriorEditItemIndex = -2;

		private const string ErrorMsg001 = "Duplicate ID";

		#endregion

		#region Properties

		protected virtual DataGrid ApplicationDataGrid
		{
			get
			{
				return null;
			}
		}

		protected virtual bool DisableEditDeleteButtons(object sender, DataGridItemEventArgs e)
		{
			return ((this.StringType == STRING_TYPE.PRODUCT_MESSAGE || this.StringType == STRING_TYPE.DOT_HAZARDOUS_MESSAGE)
                    && !this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
                    || (this.StringType == STRING_TYPE.COMPANY_TYPE && !this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
                    || (this.StringType == STRING_TYPE.ALLOCATION_GROUP && !this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
                    || (this.StringType == STRING_TYPE.FUEL_CARD_TYPE && !this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA));

        }

        protected virtual STRING_TYPE StringType
		{
			get
			{
				return STRING_TYPE.MAX_STRING_TYPE;
			}
		}

		#endregion

		#region Methods

		protected void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var applicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];
			var applicationString = new ApplicationStringClass { Type = this.StringType };
			applicationStringCollection.Add(applicationString);
			this.ApplicationDataGrid.CurrentPageIndex = (applicationStringCollection.Count - 1)
																	  / this.ApplicationDataGrid.PageSize;
			this.ApplicationDataGrid.EditItemIndex = (applicationStringCollection.Count - 1) % this.ApplicationDataGrid.PageSize;
			this.EnableControls(false);
			this.UpdateView();
		}

		protected void ApplicationStringsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var identityGuidLabel = (Label)e.Item.FindControl("IdentityGuidLabel");
				if (identityGuidLabel != null)
				{
					var applicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];
					ApplicationStringClass applicationString = applicationStringCollection.Find(x => x.IdentityGuid == Guid.Parse(identityGuidLabel.Text));

					if (applicationString.IdentityGuid == Guid.Empty)
					{
						applicationStringCollection.RemoveAt(
							applicationStringCollection.FindIndex(x => x.IdentityGuid == Guid.Parse(identityGuidLabel.Text)));

						if (this.ApplicationDataGrid.Items.Count == 1 && this.ApplicationDataGrid.CurrentPageIndex > 0)
						{
							this.ApplicationDataGrid.CurrentPageIndex--;
						}
					}
					else
					{
						ApplicationStringClass originalApplicationString = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(
								x =>
								x.Get(this.Security, applicationString.IdentityGuid)
						);

						applicationString.ID = originalApplicationString.ID;
					}

					this.PriorEditItemIndex = this.ApplicationDataGrid.EditItemIndex;
					this.ApplicationDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ApplicationStringsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var identityGuidLabel = (Label)e.Item.FindControl("IdentityGuidLabel");
				if (identityGuidLabel != null)
				{
					var applicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];

					ApplicationStringClass applicationString = applicationStringCollection.Find(x => x.IdentityGuid == Guid.Parse(identityGuidLabel.Text));

					if (this.ApplicationDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.ApplicationDataGrid.EditItemIndex = -1;
						this.EnableControls(true);
					}
					else if (this.ApplicationDataGrid.EditItemIndex > e.Item.ItemIndex)
					{
						this.ApplicationDataGrid.EditItemIndex--;
					}

					// Non empty guid indicates ApplicationString has been committed to database
					if (applicationString.IdentityGuid != Guid.Empty)
					{
						this.GetSecurity();

						FMChannelHelper.MakeCall<IApplicationStrings>(
																	 x =>
																	 x.Purge(this.Security, applicationString.IdentityGuid)
																);

						if ((applicationString.Type == STRING_TYPE.PROCESS_VARIABLE_MESSAGE) && UsingLoadRack)
						{
							try
							{
								ILoadRackManager loadRackManager = this.GetLoadRackManager();
								loadRackManager.Add(this.Security, typeof(ApplicationStringClass), applicationString.IdentityGuid);
							}
							catch (SocketException socketExcept)
							{
								if (socketExcept.ErrorCode != 10061)
								{
									throw;
								}
							}
						}
					}

					applicationStringCollection.RemoveAt(
						applicationStringCollection.FindIndex(x => x.IdentityGuid == Guid.Parse(identityGuidLabel.Text)));

					if (this.ApplicationDataGrid.Items.Count == 1 && this.ApplicationDataGrid.CurrentPageIndex > 0)
					{
						this.ApplicationDataGrid.CurrentPageIndex--;
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ApplicationStringsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			if (this.ApplicationDataGrid.EditItemIndex < 0)
			{
				this.EnableControls(false);
				this.ApplicationDataGrid.EditItemIndex = e.Item.ItemIndex;
				this.UpdateView();
			}
		}

		protected virtual void ApplicationStringsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			this.GetSecurity();

			var siteGuidLabel = (Label)e.Item.FindControl("SiteGuidLabel");

			if (siteGuidLabel != null)
			{
				bool disable = this.DisableEditDeleteButtons(sender, e) || (this.Security.SiteGuid != Guid.Parse(siteGuidLabel.Text));

				// Update the edit button setting text and image file based on "enabled" status
				var editButton = (LinkButton)e.Item.FindControl("EditButton");

				if (editButton != null)
				{
					editButton.Enabled = disable == false;
				}

				// Update the delete button setting text and image file based on "enabled" status
				var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

				if (deleteButton != null)
				{
					deleteButton.Enabled = disable == false;
				}
			}

			// The select and delete buttons need to be disabled when in edit mode.
			if (this.ApplicationDataGrid != null && this.ApplicationDataGrid.EditItemIndex != -1)
			{
				var control = e.Item.FindControl("SelectButton") as LinkButton;
				if (control != null)
				{
					control.Enabled = false;
				}

				control = e.Item.FindControl("DeleteButton") as LinkButton;
				if (control != null)
				{
					control.Enabled = false;
				}

				control = e.Item.FindControl("EditButton") as LinkButton;
				if (control != null)
				{
					control.Enabled = false;
				}
			}

			if ((this.ApplicationDataGrid != null && this.ApplicationDataGrid.EditItemIndex == e.Item.ItemIndex)
				 || this.PriorEditItemIndex == e.Item.ItemIndex)
			{
				// Now set the focus to the edit control
				Control ctrl;

				var applicationDataGrid = this.ApplicationDataGrid;
				if (applicationDataGrid != null && applicationDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					ctrl = e.Item.FindControl("StringTextBox");
				}
				else
				{
					ctrl = e.Item.FindControl("EditButton");
				}

				if (ctrl != null)
				{
					const string Script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
					this.Page.ClientScript.RegisterStartupScript(
						this.GetType(), "page_set_focus", string.Format(Script, ctrl.ClientID));
				}
			}
		}

		protected void ApplicationStringsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.ApplicationDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.ApplicationDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void ApplicationStringsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.PriorEditItemIndex = this.ApplicationDataGrid.EditItemIndex;

				var identityGuidLabel = (Label)e.Item.FindControl("IdentityGuidLabel");
				if (identityGuidLabel != null)
				{
					var applicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];

					var stringTextBox = (TextBox)e.Item.FindControl("StringTextBox");

					ApplicationStringClass applicationString = applicationStringCollection.Find(x => x.IdentityGuid == Guid.Parse(identityGuidLabel.Text));
					if (stringTextBox.Text.Length == 0)
					{
						throw new Exception("ID is required");
					}

					applicationString.ID = stringTextBox.Text;

					this.GetSecurity();

					if (applicationString.IdentityGuid == Guid.Empty)
					{
						applicationString.IdentityGuid = FMChannelHelper.MakeCall<IApplicationStrings, Guid>(
																	 x =>
																	 x.Add(this.Security, applicationString)
																);

						applicationString.SiteGuid = this.Security.SiteGuid;
					}
					else
					{
						FMChannelHelper.MakeCall<IApplicationStrings>(
																	 x =>
																	 x.Modify(this.Security, applicationString)
																);

						if ((applicationString.Type == STRING_TYPE.PROCESS_VARIABLE_MESSAGE) && UsingLoadRack)
						{
							try
							{
								ILoadRackManager loadRackManager = this.GetLoadRackManager();
								loadRackManager.Modify(this.Security, typeof(ApplicationStringClass), applicationString.IdentityGuid);
							}
							catch (SocketException socketExcept)
							{
								if (socketExcept.ErrorCode != 10061)
								{
									throw;
								}
							}
						}
					}

					this.EnableControls(true);
					this.ApplicationDataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				if (except.Message.ToUpper().StartsWith("APPLICATION STRING EXISTS"))
				{
					var newExcept = new Exception(ErrorMsg001);
					this.ErrorHandler(newExcept);
				}
				else
				{
					this.ErrorHandler(except);
				}

				this.UpdateView();
			}
		}

		protected void ApplicationStringsFormBaseLoad(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
			{
				this.SetPageFocus();
			}
		}

		protected virtual void EnableControls(bool enable)
		{
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void SetPageFocus()
		{
			const string Script = "<script language=\"jscript\">\n" + "var AddButton=document.getElementById(\"AddButton2\");\n"
			                      + "if(!AddButton.disabled)\n" + "AddButton.focus();\n" + "</script>\n";

			this.Page.ClientScript.RegisterStartupScript(this.GetType(), "page_set_focus", Script);
		}

		protected virtual void UpdateView()
		{
			this.UpdateView(null);
		}

		protected void UpdateView(FMPageSizeDropDown pageSizeDropDown)
		{
			ICollection applicationStrings = this.EnumerateApplicationStrings();

			if (pageSizeDropDown != null)
			{
				pageSizeDropDown.SetPageSize(this.ApplicationDataGrid, applicationStrings.Count);
			}

			this.ApplicationDataGrid.DataSource = applicationStrings;
			this.ApplicationDataGrid.DataBind();
		}

		/// <summary>
		///    Enumerates the application strings.
		/// </summary>
		/// <returns>
		///    A collection of application strings.
		/// </returns>
		private ICollection EnumerateApplicationStrings()
		{
			var applicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];

			var applicationStringDataTable = new DataTable();

			applicationStringDataTable.Columns.Add("SiteGuid", typeof(Guid));
			applicationStringDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			applicationStringDataTable.Columns.Add("String", typeof(string));

			foreach (ApplicationStringClass applicationStringClass in applicationStringCollection)
			{
				DataRow applicationStringDataRow = applicationStringDataTable.NewRow();

				applicationStringDataRow["SiteGuid"] = applicationStringClass.SiteGuid;
				applicationStringDataRow["IdentityGuid"] = applicationStringClass.IdentityGuid;
				applicationStringDataRow["String"] = applicationStringClass.ID;

				applicationStringDataTable.Rows.Add(applicationStringDataRow);
			}

			return new DataView(applicationStringDataTable);
		}

		private void InitializeComponent()
		{
			this.Load += this.ApplicationStringsFormBaseLoad;
			this.ApplicationDataGrid.SelectedIndexChanged += this.ApplicationDataGridSelectedIndexChanged;
		}

		/// <summary>
		/// The purpose of this routine is to provide the ability to de-select items on the strings grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void ApplicationDataGridSelectedIndexChanged(object sender, EventArgs e)
		{
			var lastValue = this.ViewState["AppStringLastSelect"] as int?;
			if (lastValue != null && lastValue == this.ApplicationDataGrid.SelectedIndex)
			{
				this.ApplicationDataGrid.SelectedIndex = -1;
				this.ViewState.Remove("AppStringLastSelect");
			}
			else
			{
				this.ViewState["AppStringLastSelect"] = this.ApplicationDataGrid.SelectedIndex;
			}
		}

		#endregion
	}
}