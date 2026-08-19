// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportConfigurationDetailPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportConfigurationDetailPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing.Printing;
	using System.Globalization;
	using System.Net;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ReportSvr2005;
	using FMBusinessObjects.ServiceRequests;

	using FMWebApp;

	public partial class ReportConfigurationDetailPage : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		private const int EmptyString = 0;
		private const int REPORT_NAME_MAX_LENGTH = 50;
		private const int REPORT_PATH_MAX_LENGTH = 200;
		private const int REPORT_DESCRIPTION_MAX_LENGTH = 255;
		private string errorMsg001 = "Unable to access the Report Server with URL";
		private string errorMsg002 = "Report Detail/Group is null";
		private string errorMsg003 = "Invalid entry";
		private string errorMsg004 = "Report name is required";
		private string errorMsg005 = "Report name exceeded maxium length (" + REPORT_NAME_MAX_LENGTH + ")";
		private string errorMsg006 = "Report description is required";
		private string errorMsg007 = "Report description exceeded maxium length (" + REPORT_DESCRIPTION_MAX_LENGTH + ")";
		private string errorMsg008 = "Report Path is required";
		private string errorMsg009 = "Report Path exceeded maxium length (" + REPORT_PATH_MAX_LENGTH + ")";
		private string errorMsg010 = "Business objects not available";
		private string errorMsg011 = "No report config detail selected/created";
		private const string ErrorMsg012 = "Report group is required";

		private string reportUrl;

		#endregion

		#region Methods

		/// <summary>
		///    This method will not save the report detail data and return control back to the
		///    report configuration assignment page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CancelButtonOnClick(object sender, EventArgs e)
		{
			// Remove the object from the session. Once it is save we do not need the object
			// anymore.
			this.Session.Remove("ReportConfigurationDetailDO");
			this.Session.Remove("ReportConfigurationGroupListDO");
			//Session.Remove("Security"); // Don't do this; it will prevent the user from doing anything.
			this.Session.Remove("SToken");

			// Transfer control back to the report configuration settings page.
			this.Redirect(this.reportUrl + "ReportConfigurationSettingsPage.aspx");
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    This is the entry point into the report configuration detail page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.reportUrl = "../FMReportWebMain/";
				this.GetSecurity();
				this.ApplyDataDictionary();

				// This method will load the page with the selected report detail information from the
				// report assignment page.
				if (this.Page.IsPostBack == false)
				{
					bool dataWarehouseKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDatawarehouseKey());
					this.DWReportCheckBox.Enabled = this.Security.HasRight(RIGHT.VIEW_DATA_ANALYTICS) && dataWarehouseKey;

               try
					{
						this.LoadPageData();
					}
					catch (Exception)
					{
						this.HandleErrorCondition(this.errorMsg010 + "!");
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    This method initiate the saving of a new or existing report detail page. The
		///    user must enter in the report name, URL, and description in order for the
		///    save to work. If the above info is not populated, then an error dialog appears.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void SaveButtonOnClick(object sender, EventArgs e)
		{
			string message = "";
			bool successful = true;

			// Ensure that the report name is entered. If not, create an error message.
			if (this.CheckForEntries(this.ReportNameTextBox.Text) == false)
			{
				message = this.errorMsg004 + "!";
				successful = false;
			}
			else if (this.CheckForLength(this.ReportNameTextBox.Text, "ReportName") == false)
			{
				message = this.errorMsg005 + "!";
				successful = false;
			}

			// Ensure that the report description is entered. If not, create an error message.
			if (this.CheckForEntries(this.ReportDescriptionTextBox.Text) == false)
			{
				message = message + " " + this.errorMsg006 + "!";
				successful = false;
			}
			else if (this.CheckForLength(this.ReportDescriptionTextBox.Text, "ReportDescription") == false)
			{
				message = this.errorMsg007 + "!";
				successful = false;
			}

			// Ensure that the report Path is selected. If not, create an error message.
			if (this.ReportPathDropDownList.SelectedIndex == -1)
			{
				message = message + " " + this.errorMsg008 + "!";
				successful = false;
			}
			else if (this.CheckForLength(this.ReportPathDropDownList.SelectedItem.Text, "ReportPath") == false)
			{
				message = this.errorMsg009 + "!";
				successful = false;
			}

			if (this.GroupDropDownList.SelectedIndex == -1)
			{
				message = message + " " + ErrorMsg012 + "!";
				successful = false;
			}

			// If the required fields are not populated, display an error dialog informing the user
			// to enter the required fields. Otherwise, attempt to save the report configuration detail.
			if (successful == false)
			{
				this.RenderErrorMessage(message);
			}
			else
			{
				// There should always be a report configuration detail in the session.
				var rptConfigDetail = (ReportConfigurationDetailDO)this.Session["ReportConfigurationDetailDO"];
				var reportGroupListDO = (ReportConfigurationGroupListDO)this.Session["ReportConfigurationGroupListDO"];

				// Something is very wrong if the report configuration detail is not in the session. Display an error
				// message.
				if ((rptConfigDetail == null) || (reportGroupListDO == null))
				{
					this.HandleErrorCondition(this.errorMsg011 + ".");
				}
				else
				{
					try
					{
						// If this report is for printing only, then set the flag.
						rptConfigDetail.ForPrintingOnly = this.ReportForPrintOnlyCheckBox.Checked;
						rptConfigDetail.PrintAtEndOfDay = this.PrintAtEndOfDayCheckBox.Checked;
						rptConfigDetail.PrintAtEndOfMonth = this.PrintAtEndOfMonthCheckBox.Checked;

						if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDatawarehouseKey()))
						{
							rptConfigDetail.DWReportFlag = this.DWReportCheckBox.Checked;

						}

						if (this.PrimaryPrinterDropDownList.SelectedIndex != -1)
						{
							rptConfigDetail.PrimaryPrinterName = this.PrimaryPrinterDropDownList.SelectedItem.Text;
						}

						if (this.SecondaryPrinterDropDownList.SelectedIndex != -1)
						{
							rptConfigDetail.SecondaryPrinterName = this.SecondaryPrinterDropDownList.SelectedItem.Text;
						}

						rptConfigDetail.ReportName = this.ReportNameTextBox.Text;
						rptConfigDetail.ReportPath = this.ReportPathDropDownList.SelectedItem.Text;
						rptConfigDetail.ReportDescription = this.ReportDescriptionTextBox.Text;

						if (this.GroupDropDownList.SelectedIndex != -1)
						{
							rptConfigDetail.ReportGroupGuid = Guid.Parse(this.GroupDropDownList.SelectedValue);
						}
						else
						{
							rptConfigDetail.ReportGroupGuid = Guid.Empty;
						}

						rptConfigDetail.UserGroupMap = new ArrayList();

						foreach (ListItem assignedUserGroupItem in this.AssignedUserGroupsListBox.Items)
						{
							var reportDetailUserGroupMapDO = new ReportDetailUserGroupMapDO
							{
								GroupGuid =
																	 Guid.Parse(assignedUserGroupItem.Value),
								GroupID = assignedUserGroupItem.Text
							};
							rptConfigDetail.UserGroupMap.Add(reportDetailUserGroupMapDO);
						}

						// Setup the request
						var reportDetailSR = new ReportConfigurationDetailSR
						{
							RequestType = ReportConfigurationDetailSR.RequestTypes.SAVE,
							ReportConfigurationDetailDO = rptConfigDetail,
							CurrentSiteGuid = this.Security.SiteGuid,
							Security = this.Security
						};

						// Call the BLL save the object
						try
						{
							FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor>(
																	 x =>
																	 x.Save(reportDetailSR)
																);
						}
						catch (Exception ex)
						{
							this.HandleErrorCondition(ex.Message);
							this.ucFMMenuBar.Refresh();
							return;
						}

						// Remove the object from the session. Once it is save we do not need the object
						// anymore.
						this.Session.Remove("ReportConfigurationDetailDO");
						this.Session.Remove("ReportConfigurationGroupListDO");
						this.ucFMMenuBar.Refresh();
						// Transfer control back to the report configuration settings page.
						this.Redirect(this.reportUrl + "ReportConfigurationSettingsPage.aspx");
					}
					catch (Exception exception)
					{
						string msg = exception.Message;

						if (msg.StartsWith("Thread was being aborted.") == false)
						{
							this.HandleErrorCondition(this.errorMsg010 + "!");
						}
					}
				}
			}
			this.ucFMMenuBar.Refresh();
		}

		/// <summary>
		///    This method will apply the data dictionary to this page.  If the data dictionary
		///    use flag is set to true, then it will apply data dictionary.
		/// </summary>
		private void ApplyDataDictionary()
		{
			string newText = this.GetTranslatedText(this.ReportDetailLabel.Text);
			this.ReportDetailLabel.Text = newText;

			newText = this.GetTranslatedText(this.ReportNameLabel.Text);
			this.ReportNameLabel.Text = newText;

			newText = this.GetTranslatedText(this.ReportPathLabel.Text);
			this.ReportPathLabel.Text = newText;

			newText = this.GetTranslatedText(this.ReportDescriptionLabel.Text);
			this.ReportDescriptionLabel.Text = newText;

			newText = this.GetTranslatedText(this.GroupAssociationLabel.Text);
			this.GroupAssociationLabel.Text = newText;

			newText = this.GetTranslatedText(this.SaveButton.Text);
			this.SaveButton.Text = newText;

			newText = this.GetTranslatedText(this.CancelButton.Text);
			this.CancelButton.Text = newText;

			newText = this.GetTranslatedText(this.ReportDetailLabel.Text);
			this.ReportDetailLabel.Text = newText;

			newText = this.GetTranslatedText(this.ReportForPrintOnlyCheckBox.Text);
			this.ReportForPrintOnlyCheckBox.Text = newText;

			newText = this.GetTranslatedText(this.PrintAtEndOfDayCheckBox.Text);
			this.PrintAtEndOfDayCheckBox.Text = newText;

			newText = this.GetTranslatedText(this.PrintAtEndOfMonthCheckBox.Text);
			this.PrintAtEndOfMonthCheckBox.Text = newText;

			newText = this.GetTranslatedText(this.PrinterName1Label.Text);
			this.PrinterName1Label.Text = newText;

			newText = this.GetTranslatedText(this.PrinterName2Label.Text);
			this.PrinterName2Label.Text = newText;

			newText = this.GetTranslatedText(this.AssignedUserGroupsLabel.Text);
			this.AssignedUserGroupsLabel.Text = newText;

			newText = this.GetTranslatedText(this.UnassignedUserGroupsLabel.Text);
			this.UnassignedUserGroupsLabel.Text = newText;

			newText = this.GetTranslatedText(this.errorMsg001);
			this.errorMsg001 = newText;

			newText = this.GetTranslatedText(this.errorMsg002);
			this.errorMsg002 = newText;

			newText = this.GetTranslatedText(this.errorMsg003);
			this.errorMsg003 = newText;

			newText = this.GetTranslatedText(this.errorMsg004);
			this.errorMsg004 = newText;

			newText = this.GetTranslatedText(this.errorMsg005);
			this.errorMsg005 = newText;

			newText = this.GetTranslatedText(this.errorMsg006);
			this.errorMsg006 = newText;

			newText = this.GetTranslatedText(this.errorMsg007);
			this.errorMsg007 = newText;

			newText = this.GetTranslatedText(this.errorMsg008);
			this.errorMsg008 = newText;

			newText = this.GetTranslatedText(this.errorMsg009);
			this.errorMsg009 = newText;

			newText = this.GetTranslatedText(this.errorMsg010);
			this.errorMsg010 = newText;

			newText = this.GetTranslatedText(this.errorMsg011);
			this.errorMsg011 = newText;
		}

		private void AssignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedUserGroupItem;
			while ((unassignedUserGroupItem = this.UnassignedUserGroupsListBox.SelectedItem) != null)
			{
				this.UnassignedUserGroupsListBox.Items.Remove(unassignedUserGroupItem);
				unassignedUserGroupItem.Selected = false;

				foreach (ListItem assignedGroupItem in this.AssignedUserGroupsListBox.Items)
				{
					if (String.Compare(assignedGroupItem.Text, unassignedUserGroupItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.AssignedUserGroupsListBox.Items.IndexOf(assignedGroupItem);
						this.AssignedUserGroupsListBox.Items.Insert(index, unassignedUserGroupItem);
						unassignedUserGroupItem = null;
						break;
					}
				}

				if (unassignedUserGroupItem != null)
				{
					this.AssignedUserGroupsListBox.Items.Add(unassignedUserGroupItem);
				}
			}
		}

		/// <summary>
		///    This method will return true if the string has been populated. Otherwise,
		///    it will return false.
		/// </summary>
		/// <param name="strValue"></param>
		/// <returns></returns>
		private bool CheckForEntries(string strValue)
		{
			bool okay = (strValue != null) && (strValue.Length > EmptyString);

			return okay;
		}

		/// <summary>
		///    This method will return true of the field length of the specified
		///    field is within tolerance.  Otherwise, it will return false.  The value
		///    of the field and the field name are passed in.
		/// </summary>
		/// <param name="strValue"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		private bool CheckForLength(string strValue, string field)
		{
			bool okay = false;

			if (field == "ReportName")
			{
				if (strValue.Length <= REPORT_NAME_MAX_LENGTH)
				{
					okay = true;
				}
			}

			if (field == "ReportPath")
			{
				if (strValue.Length <= REPORT_PATH_MAX_LENGTH)
				{
					okay = true;
				}
			}

			if (field == "ReportDescription")
			{
				if (strValue.Length <= REPORT_DESCRIPTION_MAX_LENGTH)
				{
					okay = true;
				}
			}

			if (field == "PrinterName1")
			{
				if (strValue.Length <= 100)
				{
					okay = true;
				}
			}

			if (field == "PrinterName2")
			{
				if (strValue.Length <= 100)
				{
					okay = true;
				}
			}

			return okay;
		}

		/// <summary>
		///    This method will check to see if there is an error, if so, then it will display an
		///    error dialog.
		/// </summary>
		/// <param name="errMsg"></param>
		private void HandleErrorCondition(string errMsg)
		{
			if (string.IsNullOrEmpty(errMsg) == false)
			{
				errMsg = errMsg.Replace(Environment.NewLine, " ");
				this.RenderErrorMessage(errMsg);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UnassignGroupsButton.Command += this.UnassignGroupsButtonCommand;
			this.AssignGroupsButton.Command += this.AssignGroupsButtonCommand;
		}

		/// <summary>
		///    This method will load the page with the current select report detail information.
		/// </summary>
		private void LoadPageData()
		{
			// Retrieve the new or existing report detail and the report group list data objects from the session.
			// This information should be in the session.
			var reportDetailDO = (ReportConfigurationDetailDO)this.Session["ReportConfigurationDetailDO"];
			var reportGroupListDO = (ReportConfigurationGroupListDO)this.Session["ReportConfigurationGroupListDO"];

			// Load the page if the objects are present. They should always be there.
			if ((reportDetailDO != null) && (reportGroupListDO != null))
			{
				if (!this.Security.HasRight(RIGHT.MODIFY_REPORTS)
					|| (this.Security.SiteGuid != reportDetailDO.SiteGuid) && reportDetailDO.SiteGuid != Guid.Empty)
				{
					this.SaveButton.Enabled = false;
				}

				//Set the title label with a key field from the bound object appended
				this.ReportDetailLabel.Text = this.GetTitleLabelText(this.ReportDetailLabel.Text, reportDetailDO.ReportName);

				this.ReportNameTextBox.Text = reportDetailDO.ReportName;
				this.ReportDescriptionTextBox.Text = reportDetailDO.ReportDescription;

				SystemSettingClass systemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(
																	 x =>
																	 x.Get(this.Security)
																);

				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetUsingGuid(this.Security, this.Security.SiteGuid)
																);
				try
				{
					var reportingService = new ReportingService2005
					{
						Url = systemSetting.ReportServerUrl + "/ReportService2005.asmx",
						CookieContainer = new CookieContainer()
					};
					if (!string.IsNullOrEmpty(systemSetting.ReportServerUserName))
					{
						string[] userName = systemSetting.ReportServerUserName.Split('\\');
						if (userName.Length > 1)
						{
							reportingService.Credentials = new NetworkCredential(userName[1], systemSetting.ReportServerPassword, userName[0]);
						}
						else
						{
							reportingService.Credentials = new NetworkCredential(userName[0], systemSetting.ReportServerPassword, ".");
						}
					}
					else
					{
						reportingService.Credentials = CredentialCache.DefaultCredentials;
					}

					//replace // with / if necessary.  ReportPath in db may or may not have preceeding /
					string tempPath = ("/" + site.ReportDirectory).Replace("//", "/");
					//remove trailing / if necessary
					if (tempPath.Substring(tempPath.Length - 1) == "/")
					{
						tempPath = tempPath.Substring(0, tempPath.Length - 1);
					}

					CatalogItem[] items = reportingService.ListChildren(tempPath, false);

					int index = 0;

					foreach (CatalogItem item in items)
					{
						if ((item.Type != ItemTypeEnum.Report) && (item.Type != ItemTypeEnum.LinkedReport))
						{
							continue;
						}

						var listItem = new ListItem(item.Name, index.ToString(CultureInfo.InvariantCulture));
						this.ReportPathDropDownList.Items.Add(listItem);

						if (item.Name == reportDetailDO.ReportPath)
						{
							this.ReportPathDropDownList.SelectedIndex = this.ReportPathDropDownList.Items.Count - 1;
						}

						index++;
					}
				}
				catch (Exception ex)
				{
					this.HandleErrorCondition(ex.Message);
				}

				this.ReportForPrintOnlyCheckBox.Checked = reportDetailDO.ForPrintingOnly;
				this.PrintAtEndOfDayCheckBox.Checked = reportDetailDO.PrintAtEndOfDay;
				this.PrintAtEndOfMonthCheckBox.Checked = reportDetailDO.PrintAtEndOfMonth;
				this.DWReportCheckBox.Checked = reportDetailDO.DWReportFlag;

				// PrimaryPrinterDropDownList
				this.PrimaryPrinterDropDownList.Items.Add(new ListItem("{None}", "0"));
				this.PrimaryPrinterDropDownList.SelectedIndex = 0;
				var installedPrinters = getInstalledPrinters();
				for (int iItem = 0; iItem < installedPrinters?.Count; iItem++)
				{
					var newPrinterItem = new ListItem(installedPrinters?[iItem], (iItem + 1).ToString(CultureInfo.InvariantCulture));
					foreach (ListItem existingPrinterItem in this.PrimaryPrinterDropDownList.Items)
					{
						if (String.Compare(existingPrinterItem.Text, newPrinterItem.Text, StringComparison.Ordinal) > 0)
						{
							int insert = this.PrimaryPrinterDropDownList.Items.IndexOf(existingPrinterItem);
							if (insert != 0)
							{
								this.PrimaryPrinterDropDownList.Items.Insert(insert, newPrinterItem);
								if (reportDetailDO.PrimaryPrinterName == newPrinterItem.Text)
								{
									this.PrimaryPrinterDropDownList.SelectedIndex = insert;
								}
								newPrinterItem = null;
								break;
							}
						}
					}

					if (newPrinterItem != null)
					{
						this.PrimaryPrinterDropDownList.Items.Add(newPrinterItem);

						if (reportDetailDO.SecondaryPrinterName == newPrinterItem.Text)
						{
							this.PrimaryPrinterDropDownList.SelectedIndex = this.PrimaryPrinterDropDownList.Items.Count - 1;
						}
					}
				}

				// SecondaryPrinterDropDownList
				this.SecondaryPrinterDropDownList.Items.Add(new ListItem("{None}", "0"));
				this.SecondaryPrinterDropDownList.SelectedIndex = 0;
				for (int iItem = 0; iItem < installedPrinters?.Count; iItem++)
				{
					var newPrinterItem = new ListItem(installedPrinters?[iItem], (iItem + 1).ToString(CultureInfo.InvariantCulture));
					foreach (ListItem existingPrinterItem in this.SecondaryPrinterDropDownList.Items)
					{
						if (String.Compare(existingPrinterItem.Text, newPrinterItem.Text, StringComparison.Ordinal) > 0)
						{
							int insert = this.SecondaryPrinterDropDownList.Items.IndexOf(existingPrinterItem);
							if (insert != 0)
							{
								this.SecondaryPrinterDropDownList.Items.Insert(insert, newPrinterItem);
								if (reportDetailDO.SecondaryPrinterName == newPrinterItem.Text)
								{
									this.SecondaryPrinterDropDownList.SelectedIndex = insert;
								}
								newPrinterItem = null;
								break;
							}
						}
					}

					if (newPrinterItem != null)
					{
						this.SecondaryPrinterDropDownList.Items.Add(newPrinterItem);

						if (reportDetailDO.SecondaryPrinterName == newPrinterItem.Text)
						{
							this.SecondaryPrinterDropDownList.SelectedIndex = this.SecondaryPrinterDropDownList.Items.Count - 1;
						}
					}
				}

				List<ReportConfigurationGroupDO> reportGroups = reportGroupListDO.ReportGroupDOList;

				// Load the group dropdown list with all possible groups.
				this.GroupDropDownList.DataSource = reportGroups;
				this.GroupDropDownList.DataTextField = "GroupName";
				this.GroupDropDownList.DataValueField = "ReportGroupGuid";
				this.GroupDropDownList.DataBind();

				if (reportDetailDO.ReportGroupGuid != Guid.Empty)
				{
					this.GroupDropDownList.SelectedValue = reportDetailDO.ReportGroupGuid.ToString();
				}

				// Load the AssignedUserGroups
				foreach (ReportDetailUserGroupMapDO reportDetailUserGroupMapDO in reportDetailDO.UserGroupMap)
				{
					var newUserGroupListItem = new ListItem(
						reportDetailUserGroupMapDO.GroupID, reportDetailUserGroupMapDO.GroupGuid.ToString());

					foreach (ListItem existingUserGroupList in this.AssignedUserGroupsListBox.Items)
					{
						if (String.Compare(existingUserGroupList.Text, newUserGroupListItem.Text, StringComparison.Ordinal) > 0)
						{
							int insert = this.AssignedUserGroupsListBox.Items.IndexOf(existingUserGroupList);
							if (insert != 0)
							{
								this.AssignedUserGroupsListBox.Items.Insert(insert, newUserGroupListItem);
								newUserGroupListItem = null;
								break;
							}
						}
					}

					if (newUserGroupListItem != null)
					{
						this.AssignedUserGroupsListBox.Items.Add(newUserGroupListItem);
					}
				}

				// Load the UnassignedUserGroups
				GroupCollectionClass groupCollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

				foreach (GroupClass group in groupCollection)
				{
					if (this.AssignedUserGroupsListBox.Items.FindByText(group.ID) != null)
					{
						continue;
					}

					var newUserGroupListItem = new ListItem(group.ID, group.IdentityGuid.ToString());
					foreach (ListItem existingUserGroupList in this.UnassignedUserGroupsListBox.Items)
					{
						if (String.Compare(existingUserGroupList.Text, newUserGroupListItem.Text, StringComparison.Ordinal) > 0)
						{
							int insert = this.UnassignedUserGroupsListBox.Items.IndexOf(existingUserGroupList);
							if (insert != 0)
							{
								this.UnassignedUserGroupsListBox.Items.Insert(insert, newUserGroupListItem);
								newUserGroupListItem = null;
								break;
							}
						}
					}

					if (newUserGroupListItem != null)
					{
						this.UnassignedUserGroupsListBox.Items.Add(newUserGroupListItem);
					}
				}
			}
			else
			{
				this.HandleErrorCondition(this.errorMsg002 + "!");
			}
		}

		private void UnassignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedUserGroupItem;
			while ((assignedUserGroupItem = this.AssignedUserGroupsListBox.SelectedItem) != null)
			{
				this.AssignedUserGroupsListBox.Items.Remove(assignedUserGroupItem);
				assignedUserGroupItem.Selected = false;

				foreach (ListItem unassignedGroupItem in this.UnassignedUserGroupsListBox.Items)
				{
					if (String.Compare(unassignedGroupItem.Text, assignedUserGroupItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedUserGroupsListBox.Items.IndexOf(unassignedGroupItem);
						this.UnassignedUserGroupsListBox.Items.Insert(index, assignedUserGroupItem);
						assignedUserGroupItem = null;
						break;
					}
				}

				if (assignedUserGroupItem != null)
				{
					this.UnassignedUserGroupsListBox.Items.Add(assignedUserGroupItem);
				}
			}
		}
		  private System.Drawing.Printing.PrinterSettings.StringCollection getInstalledPrinters()
		  {
				try
				{
					 return PrinterSettings.InstalledPrinters;

            }
				catch (Exception e)
				{
                string msg = "Error retrieving Printers. Restart or enable the print spooler.";
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + e.Message, FMEventLogEntryType.Error));
                return null;
				}
		  }

		#endregion
	}
}