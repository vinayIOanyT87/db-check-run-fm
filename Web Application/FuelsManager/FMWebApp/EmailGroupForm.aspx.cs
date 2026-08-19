// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailGroupForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EmailGroupForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;
    using FMCore;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for EmailGroupForm.
	/// </summary>
	public partial class EmailGroupForm : FMFormBase
	{
		#region Constants and Fields

		protected FMLabel EmailGroupNameRequiredLabel;

		protected Image Image2;

		protected Image Image3;

		protected FMLabel Label6;

		protected TextBox Password;

		protected DateTimeFormatInfo formatInfo = DateTimeFormatInfo.CurrentInfo;

		#endregion

		#region Public Properties

		public DateTimeFormatInfo FormatInfo
		{
			get
			{
				return this.formatInfo;
			}
		}

		#endregion

		#region Methods

		protected void AlwaysEnabledCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			this.StartTime.Enabled = (!this.AlwaysEnabledCheckBox.Checked);
			this.EndTime.Enabled = (!this.AlwaysEnabledCheckBox.Checked);
		}

		/// <summary>
		///    This method enables and disables controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			EmailGroupClass emailGroup;

			var sessionGuid = this.Session["IdentityGuid"] as string;
			// Get IdentityGuid
			if (sessionGuid != null)
			{
				// Get EmailGroup
				emailGroup =
					FMChannelHelper.MakeCall<IEmailGroups, EmailGroupClass>(
						x => x.Get(this.Security, Guid.Parse(sessionGuid)));
			}
			else
			{
				emailGroup = new EmailGroupClass();
			}

			if (this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && (emailGroup.SiteGuid == this.Security.SiteGuid || emailGroup.SiteGuid == Guid.Empty))
			{
				this.AddButton.Enabled = enable;
				this.OK.Enabled = enable;
				this.AssignCategoriesButton.Enabled = enable;
				this.AssignPrioritiesButton.Enabled = enable;
				this.UnassignCategoriesButton.Enabled = enable;
				this.UnassignPrioritiesButton.Enabled = enable;
				this.AlwaysEnabledCheckBox.Enabled = enable;
				this.AndRadioButton.Enabled = enable;
				this.OrRadioButton.Enabled = enable;
			}

			this.Cancel.Enabled = enable;
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

				this.ApplyDataDictionary();

				if (!this.Page.IsPostBack)
				{
					EmailGroupClass emailGroup;
					var sessionGuid = this.Session["IdentityGuid"] as string;

					// Get IdentityGuid
					if ( sessionGuid != null)
					{
						// Get EmailGroup
						emailGroup =
							FMChannelHelper.MakeCall<IEmailGroups, EmailGroupClass>(x => x.Get(this.Security, Guid.Parse(sessionGuid)));
					}
					else
					{
						emailGroup = new EmailGroupClass();
					}

					this.Session["EmailGroup"] = emailGroup;

					if (this.Security != null)
					{
						SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security,
																			emailGroup.SiteGuid,
																			getMemberSites: true,
																			getSchedulesAndProcessVariables: true,
																			bGetAssociatedAliases: true)
																);
						if (site != null)
						{
							DateTimeFormatInfo d = site.GetDateTimeFormatInfo();
							if (d != null)
							{
								this.formatInfo = d;
							}
						}
					}

					this.DataBind();

					this.Name.Text = emailGroup.ID;
					this.AlwaysEnabledCheckBox.Checked = emailGroup.AlwaysEnabled;
					this.AlwaysEnabledCheckBoxCheckedChanged(null, null);
					this.AndRadioButton.Checked = (emailGroup.CategoriesAndPriorities);
					this.OrRadioButton.Checked = (!emailGroup.CategoriesAndPriorities);

					// Populate StartTime and EndTime DropDownList's
					this.StartTime.Text = emailGroup.StartTime.ToString();
					this.EndTime.Text = emailGroup.EndTime.ToString();

					// Populate AssignedCategoriesListBox
					foreach (ApplicationStringMapClass applicationStringMap in emailGroup.CategoryCollection)
					{
						var unassignedCategoryItem = new ListItem(
							applicationStringMap.ID, applicationStringMap.ApplicationStringGuid.ToString());
						foreach (ListItem assignedCategoryItem in this.AssignedCategoriesListBox.Items)
						{
							if (String.Compare(assignedCategoryItem.Text, unassignedCategoryItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.AssignedCategoriesListBox.Items.IndexOf(assignedCategoryItem);
								this.AssignedCategoriesListBox.Items.Insert(index, unassignedCategoryItem);
								unassignedCategoryItem = null;
								break;
							}
						}

						if (unassignedCategoryItem != null)
						{
							this.AssignedCategoriesListBox.Items.Add(unassignedCategoryItem);
						}
					}

					// Populate AssignedPrioritiesListBox
					foreach (AlarmPriorityClass alarmPriority in emailGroup.PriorityCollection)
					{
						var unassignedPriorityItem = new ListItem(alarmPriority.ID, alarmPriority.IdentityGuid.ToString());
						foreach (ListItem assignedPriorityItem in this.AssignedPrioritiesListBox.Items)
						{
							if (String.Compare(assignedPriorityItem.Text, unassignedPriorityItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.AssignedPrioritiesListBox.Items.IndexOf(assignedPriorityItem);
								this.AssignedPrioritiesListBox.Items.Insert(index, unassignedPriorityItem);
								unassignedPriorityItem = null;
								break;
							}
						}

						if (unassignedPriorityItem != null)
						{
							this.AssignedPrioritiesListBox.Items.Add(unassignedPriorityItem);
						}
					}

					// Populate UnassignedCategorysListBox
					ApplicationStringCollectionClass categoryCollection =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x => x.EnumerateByType(this.Security, STRING_TYPE.ALARM_EVENT_CATEGORY));

					foreach (ApplicationStringClass category in categoryCollection)
					{
						if (null == this.AssignedCategoriesListBox.Items.FindByValue(category.IdentityGuid.ToString()))
						{
							var assignedCategoryItem = new ListItem(category.ID, category.IdentityGuid.ToString());
							foreach (ListItem unassignedCategoryItem in this.UnassignedCategoriesListBox.Items)
							{
								if (String.Compare(unassignedCategoryItem.Text, assignedCategoryItem.Text, StringComparison.Ordinal) > 0)
								{
									int index = this.UnassignedCategoriesListBox.Items.IndexOf(unassignedCategoryItem);
									this.UnassignedCategoriesListBox.Items.Insert(index, assignedCategoryItem);
									assignedCategoryItem = null;
									break;
								}
							}

							if (assignedCategoryItem != null)
							{
								this.UnassignedCategoriesListBox.Items.Add(assignedCategoryItem);
							}
						}
					}

					// Populate UnassignedPrioritiesListBox
					AlarmPriorityCollectionClass priorityCollection =
						FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(x => x.Enumerate(this.Security));

					foreach (AlarmPriorityClass priority in priorityCollection)
					{
						if (null == this.AssignedPrioritiesListBox.Items.FindByValue(priority.IdentityGuid.ToString()))
						{
							var assignedPriorityItem = new ListItem(priority.ID, priority.IdentityGuid.ToString());
							foreach (ListItem unassignedPriorityItem in this.UnassignedPrioritiesListBox.Items)
							{
								if (String.Compare(unassignedPriorityItem.Text, assignedPriorityItem.Text, StringComparison.Ordinal) > 0)
								{
									int index = this.UnassignedPrioritiesListBox.Items.IndexOf(unassignedPriorityItem);
									this.UnassignedPrioritiesListBox.Items.Insert(index, assignedPriorityItem);
									assignedPriorityItem = null;
									break;
								}
							}

							if (assignedPriorityItem != null)
							{
								this.UnassignedPrioritiesListBox.Items.Add(assignedPriorityItem);
							}
						}
					}

					this.UpdateEmailAddressView();

					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					    || (emailGroup.SiteGuid != this.Security.SiteGuid && emailGroup.SiteGuid != Guid.Empty))
					{
						this.OK.Enabled = false;
						this.AddButton.Enabled = false;
						this.AssignCategoriesButton.Enabled = false;
						this.AssignPrioritiesButton.Enabled = false;
						this.UnassignCategoriesButton.Enabled = false;
						this.UnassignPrioritiesButton.Enabled = false;
						this.AlwaysEnabledCheckBox.Enabled = false;
						this.AndRadioButton.Enabled = false;
						this.OrRadioButton.Enabled = false;
					}

					//Set the title label with a key field from the bound object appended
					this.EmailGroupTitleLabel.Text = this.GetTitleLabelText(this.EmailGroupTitleLabel.Text, emailGroup.ID);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UnassignedPrioritiesListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
		}

		protected void UpdateEmailAddressView()
		{
			this.EmailAddressDataGrid.DataSource = this.EnumerateEmailAddresses();
			this.EmailAddressDataGrid.DataBind();
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var emailGroup = (EmailGroupClass)this.Session["EmailGroup"];
			var applicationStringMap = new ApplicationStringMapClass { Type = STRING_MAP_TYPE.EMAIL_ADDRESS };

			emailGroup.EmailAddressCollection.Add(applicationStringMap);
			this.EmailAddressDataGrid.CurrentPageIndex = (emailGroup.EmailAddressCollection.Count - 1)
			                                             / this.EmailAddressDataGrid.PageSize;
			this.EmailAddressDataGrid.EditItemIndex = (emailGroup.EmailAddressCollection.Count - 1)
			                                          % this.EmailAddressDataGrid.PageSize;

			this.EnableControls(false);
			this.UpdateEmailAddressView();
		}

		/// <summary>
		///    This method will apply the data dictionary to items that are not FMControl.
		/// </summary>
		private void ApplyDataDictionary()
		{
			string newText = this.GetTranslatedText(this.AndRadioButton.Text);
			this.AndRadioButton.Text = newText;

			newText = this.GetTranslatedText(this.OrRadioButton.Text);
			this.OrRadioButton.Text = newText;
		}

		private void AssignCategoriesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedCategoryItem;
			while ((unassignedCategoryItem = this.UnassignedCategoriesListBox.SelectedItem) != null)
			{
				this.UnassignedCategoriesListBox.Items.Remove(unassignedCategoryItem);
				unassignedCategoryItem.Selected = false;

				foreach (ListItem assignedCategoryItem in this.AssignedCategoriesListBox.Items)
				{
					if (String.Compare(assignedCategoryItem.Text, unassignedCategoryItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.AssignedCategoriesListBox.Items.IndexOf(assignedCategoryItem);
						this.AssignedCategoriesListBox.Items.Insert(index, unassignedCategoryItem);
						unassignedCategoryItem = null;
						break;
					}
				}

				if (unassignedCategoryItem != null)
				{
					this.AssignedCategoriesListBox.Items.Add(unassignedCategoryItem);
				}
			}
		}

		private void AssignPrioritiesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedPriorityItem;
			while ((unassignedPriorityItem = this.UnassignedPrioritiesListBox.SelectedItem) != null)
			{
				this.UnassignedPrioritiesListBox.Items.Remove(unassignedPriorityItem);
				unassignedPriorityItem.Selected = false;

				foreach (ListItem assignedPriorityItem in this.AssignedPrioritiesListBox.Items)
				{
					if (String.Compare(assignedPriorityItem.Text, unassignedPriorityItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.AssignedPrioritiesListBox.Items.IndexOf(assignedPriorityItem);
						this.AssignedPrioritiesListBox.Items.Insert(index, unassignedPriorityItem);
						unassignedPriorityItem = null;
						break;
					}
				}

				if (unassignedPriorityItem != null)
				{
					this.AssignedPrioritiesListBox.Items.Add(unassignedPriorityItem);
				}
			}
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("EmailGroup");
			this.Redirect( "AlarmEventConfigurationForm.aspx?EmailGroups=true" );
		}

		private void EmailAddressDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				var emailGroup = (EmailGroupClass)this.Session["EmailGroup"];

				if (string.IsNullOrEmpty(emailGroup.EmailAddressCollection[Convert.ToInt32(indexLabel.Text)].ID))
				{
					emailGroup.EmailAddressCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));

					if ((this.EmailAddressDataGrid.Items.Count == 1) && (this.EmailAddressDataGrid.CurrentPageIndex > 0))
					{
						this.EmailAddressDataGrid.CurrentPageIndex--;
					}
				}

				this.EmailAddressDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateEmailAddressView();
			}
		}

		private void EmailAddressDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				var emailGroup = (EmailGroupClass)this.Session["EmailGroup"];

				if (this.EmailAddressDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.EmailAddressDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}

				else if (this.EmailAddressDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.EmailAddressDataGrid.EditItemIndex--;
				}

				emailGroup.EmailAddressCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));
				if (this.EmailAddressDataGrid.Items.Count == 1 && this.EmailAddressDataGrid.CurrentPageIndex > 0)
				{
					this.EmailAddressDataGrid.CurrentPageIndex--;
				}

				this.UpdateEmailAddressView();
			}
		}

		private void EmailAddressDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.EnableControls(false);
			this.EmailAddressDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.UpdateEmailAddressView();
		}

		private void EmailAddressDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.EmailAddressDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.EmailAddressDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateEmailAddressView();
		}

		private void EmailAddressDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					var emailGroup = (EmailGroupClass)this.Session["EmailGroup"];
					var idTextBox = (TextBox)e.Item.FindControl("IDTextBox");

					if (string.IsNullOrEmpty(idTextBox.Text))
					{
						this.EmailAddressDataGridDeleteCommand(source, e);
					}
					else
					{
						int index = Convert.ToInt32(indexLabel.Text);

						if (emailGroup.EmailAddressCollection[index].ID != idTextBox.Text)
						{
							if (idTextBox.Text.IsValidEmailAddressSyntax() == false)
							{
								throw new FMEmailFormatException();
							}

							emailGroup.EmailAddressCollection[index].ID = idTextBox.Text;
							emailGroup.EmailAddressCollection[index].ApplicationStringGuid = Guid.Empty;
						}

						this.EmailAddressDataGrid.EditItemIndex = -1;
						this.UpdateEmailAddressView();
					}

					this.EnableControls(true);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection EnumerateEmailAddresses()
		{
			var emailGroup = (EmailGroupClass)this.Session["EmailGroup"];

			var emailAddressDataTable = new DataTable();

			emailAddressDataTable.Columns.Add("Index", typeof(Int32));
			emailAddressDataTable.Columns.Add("ID", typeof(string));

			foreach (ApplicationStringMapClass applicationStringMap in emailGroup.EmailAddressCollection)
			{
				DataRow emailAddressDataRow = emailAddressDataTable.NewRow();

				emailAddressDataRow["Index"] = emailAddressDataTable.Rows.Count;
				emailAddressDataRow["ID"] = applicationStringMap.ID;

				emailAddressDataTable.Rows.Add(emailAddressDataRow);
			}
			var emailAddressDataView = new DataView(emailAddressDataTable);
			return emailAddressDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UnassignCategoriesButton.Command += this.UnassignCategoriesButtonCommand;
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
			this.AssignCategoriesButton.Command += this.AssignCategoriesButtonCommand;
			this.UnassignPrioritiesButton.Command += this.UnassignPrioritiesButtonCommand;
			this.AssignPrioritiesButton.Command += this.AssignPrioritiesButtonCommand;
			this.EmailAddressDataGrid.EditCommand += this.EmailAddressDataGridEditCommand;
			this.EmailAddressDataGrid.PageIndexChanged += this.EmailAddressDataGridPageIndexChanged;
			this.EmailAddressDataGrid.CancelCommand += this.EmailAddressDataGridCancelCommand;
			this.EmailAddressDataGrid.UpdateCommand += this.EmailAddressDataGridUpdateCommand;
			this.EmailAddressDataGrid.DeleteCommand += this.EmailAddressDataGridDeleteCommand;
			this.AddButton.Command += this.AddButtonCommand;
		}

		/// <summary>
		///    This method will handle the OK button being pressed event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			bool noPrioritiesAreAssigned = true;

			try
			{
				this.GetSecurity();

				SiteClass currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				// Create a CategoryCollection
				var categoryCollection = new ApplicationStringMapCollectionClass();
				foreach (ListItem assignedCategoryItem in this.AssignedCategoriesListBox.Items)
				{
					var applicationStringMap = new ApplicationStringMapClass
					                           {
						                           ApplicationStringGuid = Guid.Parse(assignedCategoryItem.Value),
						                           Type = STRING_MAP_TYPE.ALARM_EVENT_CATEGORY,
						                           ID = assignedCategoryItem.Text
					                           };
					categoryCollection.Add(applicationStringMap);
				}

				// Create a PriortyCollection
				var priorityCollection = new AlarmPriorityCollectionClass();
				foreach (ListItem assignedPriorityItem in this.AssignedPrioritiesListBox.Items)
				{
					var alarmPriority = new AlarmPriorityClass
					                    {
						                    IdentityGuid = Guid.Parse(assignedPriorityItem.Value),
						                    ID = assignedPriorityItem.Text
					                    };
					priorityCollection.Add(alarmPriority);
					noPrioritiesAreAssigned = false;
				}

				var emailGroup = (EmailGroupClass)this.Session["EmailGroup"];

				emailGroup.ID = this.Name.Text;
				emailGroup.AlwaysEnabled = this.AlwaysEnabledCheckBox.Checked;

				string dayOne = TimeConverter.MinFMDate.ToString("d", currentSite.GetDateTimeFormatInfo());
				emailGroup.StartTime.Value = DateTimeOffset.Parse(
					dayOne + " " + this.StartTime.Text, currentSite.GetDateTimeFormatInfo());
				emailGroup.EndTime.Value = DateTimeOffset.Parse(
					dayOne + " " + this.EndTime.Text, currentSite.GetDateTimeFormatInfo());

				// If there are no priorities assigned, then ensure that categories and priorities
				// AND/OR radio button is set to OR.
				if (noPrioritiesAreAssigned)
				{
					this.AndRadioButton.Checked = false;
					this.OrRadioButton.Checked = true;
					emailGroup.CategoriesAndPriorities = false;
				}
				else
				{
					emailGroup.CategoriesAndPriorities = this.AndRadioButton.Checked;
				}

				emailGroup.CategoryCollection = categoryCollection;
				emailGroup.PriorityCollection = priorityCollection;

				if (emailGroup.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IEmailGroups>(x => x.Modify(this.Security, emailGroup));
				}
				else
				{
					FMChannelHelper.MakeCall<IEmailGroups>(x => x.Add(this.Security, emailGroup));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Session.Remove("EmailGroup");
			this.Redirect("AlarmEventConfigurationForm.aspx?EmailGroups=true");
		}

		private void UnassignCategoriesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedCategoryItem;
			while ((assignedCategoryItem = this.AssignedCategoriesListBox.SelectedItem) != null)
			{
				this.AssignedCategoriesListBox.Items.Remove(assignedCategoryItem);
				assignedCategoryItem.Selected = false;

				foreach (ListItem unassignedCategoryItem in this.UnassignedCategoriesListBox.Items)
				{
					if (String.Compare(unassignedCategoryItem.Text, assignedCategoryItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedCategoriesListBox.Items.IndexOf(unassignedCategoryItem);
						this.UnassignedCategoriesListBox.Items.Insert(index, assignedCategoryItem);
						assignedCategoryItem = null;
						break;
					}
				}

				if (assignedCategoryItem != null)
				{
					this.UnassignedCategoriesListBox.Items.Add(assignedCategoryItem);
				}
			}
		}

		private void UnassignPrioritiesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedPriorityItem;
			while ((assignedPriorityItem = this.AssignedPrioritiesListBox.SelectedItem) != null)
			{
				this.AssignedPrioritiesListBox.Items.Remove(assignedPriorityItem);
				assignedPriorityItem.Selected = false;

				foreach (ListItem unassignedPriorityItem in this.UnassignedPrioritiesListBox.Items)
				{
					if (String.Compare(unassignedPriorityItem.Text, assignedPriorityItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedPrioritiesListBox.Items.IndexOf(unassignedPriorityItem);
						this.UnassignedPrioritiesListBox.Items.Insert(index, assignedPriorityItem);
						assignedPriorityItem = null;
						break;
					}
				}

				if (assignedPriorityItem != null)
				{
					this.UnassignedPrioritiesListBox.Items.Add(assignedPriorityItem);
				}
			}
		}

		#endregion
	}
}