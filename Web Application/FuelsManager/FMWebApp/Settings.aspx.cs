// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Settings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	public partial class Settings : FMAutoSubmitFormBase, IMenuDiscovery
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
               // Depends Upon Shared Components Config
               if ((options & 0x4000) == 0)
               {
                  return null;
               }
         }
         var items = new List<FMMenuItem>();

			if ((!security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)) || (!security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA)))
			{
				return null;
			}
			
			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_IMPORT_EXPORT_ENTERPRISE_SETTINGS,
						RootMenuName = "Configuration",
						CategoryName = "Import/Export",
						ItemName = "Enterprise Settings",
						NavigateUrl = "Settings.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.GetSecurity();
			if (!this.Page.IsPostBack)
			{
				this.RefreshValues();
			}
		}

		protected void Yes_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.ValidatedValues())
				{
					FMChannelHelper.MakeCall<IEnterpriseImportExportSettings>(x => this.SaveUpdateValues(x));
					this.RefreshValues();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private Boolean IsStrAnInteger(String strNum)
		{
			if (strNum == null)
			{
				return false;
			}
			string strTemp = strNum.Trim();
			if (strTemp.Length == 0)
			{
				return false;
			}
			int number = 0;
			return (int.TryParse(strTemp, out number));
		}

		private void RefreshValues()
		{
			try
			{
				DataTable dt = FMChannelHelper.MakeCall<IEnterpriseImportExportSettings, DataTable>(
																	 x =>
																	 x.SelectAll(this.Security)
																);


				if (dt != null && dt.Rows.Count > 0)
				{
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						DataRow nextRow = dt.Rows[i];
						string strSettingKey = nextRow["SettingKey"].ToString().Trim();
						if (strSettingKey == "EnterpriseDataIntervalBetweenSendAttemptsInMinutes")
						{
							this.TextBoxAttemptsInMinutes.Text = nextRow["SettingValue"].ToString().Trim();
						}
						else if (strSettingKey == "EnterpriseDataSendAttempts")
						{
							this.TextBoxNumAttempts.Text = nextRow["SettingValue"].ToString().Trim();
						}
						else if (strSettingKey == "ExportArchiveDir")
						{
							this.TextBoxExportArchiveDir.Text = nextRow["SettingValue"].ToString().Trim();
						}
						else if (strSettingKey == "ImportArchiveDir")
						{
							this.TextBoxImportArchiveDir.Text = nextRow["SettingValue"].ToString().Trim();
						}
						else if (strSettingKey == "ExportingSiteGuid")
						{
							this.FMDropDownListSites.Items.Clear();
							var FirstItem = new ListItem("", Guid.Empty.ToString());
							this.FMDropDownListSites.Items.Add(FirstItem);

							List<SiteClass> siteCollection = FMChannelHelper.MakeCall<ISites, List<SiteClass>>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
							foreach (SiteClass site in siteCollection)
							{
								string strSiteGuid = site.SiteGuid.ToString();
								string strID = site.ID;

								var NewItem = new ListItem(strID, strSiteGuid);

								this.FMDropDownListSites.Items.Add(NewItem);
							}

							foreach (ListItem listItem in this.FMDropDownListSites.Items)
							{
								if (listItem.Value == nextRow["SettingValue"].ToString().Trim())
								{
									listItem.Selected = true;
									break;
								}
							}
						}
						else if (strSettingKey == "LogImportProcessRunInformation")
						{
							string strLogImport = nextRow["SettingValue"].ToString().Trim();

							if ((strLogImport.ToUpper() == "TRUE") || (strLogImport == "1"))
							{
								this.CheckBoxLogImport.Checked = true;
							}
							else
							{
								this.CheckBoxLogImport.Checked = false;
							}
						}
						else if (strSettingKey == "URLofEnterpriseDataWebService")
						{
							this.TextBoxURLOfImportWebSvs.Text = nextRow["SettingValue"].ToString().Trim();
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		private Boolean SaveUpdateValues(IEnterpriseImportExportSettings settings)
		{
			Boolean bSavedUpdated = false;

			try
			{
				settings.Update(this.Security, "EnterpriseDataIntervalBetweenSendAttemptsInMinutes", this.TextBoxAttemptsInMinutes.Text.Trim());

				settings.Update(this.Security, "EnterpriseDataSendAttempts", this.TextBoxNumAttempts.Text.Trim());

				settings.Update(this.Security, "ExportArchiveDir", this.TextBoxExportArchiveDir.Text.Trim());

				settings.Update(this.Security, "ImportArchiveDir", this.TextBoxImportArchiveDir.Text.Trim());

				string strSiteGuid = Guid.Empty.ToString();
				foreach (ListItem listItem in this.FMDropDownListSites.Items)
				{
					if (listItem.Selected)
					{
						strSiteGuid = listItem.Value;
					}
				}

				settings.Update(this.Security, "ExportingSiteGuid", strSiteGuid);


				if (this.CheckBoxLogImport.Checked)
				{
					settings.Update(this.Security, "LogImportProcessRunInformation", "true");
				}
				else
				{
					settings.Update(this.Security, "LogImportProcessRunInformation", "false");
				}

				settings.Update(this.Security, "URLofEnterpriseDataWebService", this.TextBoxURLOfImportWebSvs.Text.Trim());
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return bSavedUpdated;
		}

		private bool ValidatedValues()
		{
			string strErrorMessage = string.Empty;
			if (!this.IsStrAnInteger(this.TextBoxAttemptsInMinutes.Text))
			{
				strErrorMessage += "Retry interval in minutes is invalid. ";
			}

			if (!this.IsStrAnInteger(this.TextBoxNumAttempts.Text))
			{
				strErrorMessage += "Number Of retries are invalid. ";
			}

			if (this.TextBoxURLOfImportWebSvs.Text.Trim().Length == 0)
			{
				strErrorMessage += "Target web service URL is empty. ";
			}

			if (strErrorMessage != string.Empty)
			{
				this.LabelErrorMessage.Visible = true;
				this.LabelErrorMessage.Text = strErrorMessage;
				return false;
			}
			
			this.LabelErrorMessage.Text = string.Empty;
			this.LabelErrorMessage.Visible = false;
			
			return true;
		}

		#endregion
	}
}