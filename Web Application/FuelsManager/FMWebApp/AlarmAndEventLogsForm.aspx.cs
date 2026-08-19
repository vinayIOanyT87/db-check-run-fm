// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmAndEventLogsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for AdditiveProfilesForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
   using System.Configuration;
   using System.Data;
	using System.Globalization;
	using System.Reflection;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Code behind for AlarmAndEventLogsForm.
	/// </summary>
	public partial class AlarmAndEventLogsForm : FMAutoSubmitFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		protected SiteClass CurrentSite;

		private const string AllText = "{All}";

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
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
			if (security.HasRight(RIGHT.VIEW_ALARM_EVENT_LOGS) == false)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.OPERATIONS_SYSTEM_LOGS_ALARM_AND_EVENT_LOG,
				RootMenuName = "Operations",
				CategoryName = "System Logs",
				ItemName = "Alarm & Event Log",
				NavigateUrl = "AlarmAndEventLogsForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion


		#region Methods

		/// <summary>
		/// Acknowledges the button click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void AcknowledgeButtonClick(object sender, EventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(this.AcknowledgeEvents);

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Acknowledges the events.
		/// </summary>
		/// <param name="alarmAndEventLogs">The alarm and event logs.</param>
		private void AcknowledgeEvents(IAlarmAndEventLogs alarmAndEventLogs)
		{
			foreach (DataGridItem item in this.AlarmAndEventLogsDataGrid.Items)
			{
				var selectedCheckBox = (CheckBox)item.FindControl("SelectedCheckBox");
				var acknowledgedCheckBox = (CheckBox)item.FindControl("AcknowledgedCheckBox");

				if (selectedCheckBox != null && acknowledgedCheckBox != null)
				{
					if (selectedCheckBox.Checked && !acknowledgedCheckBox.Checked)
					{
						// Only SequenceNumber and Acknowledged needed for Modify
						var alarmAndEventLog = new AlarmAndEventLogClass
						{
							SequenceNumber = Convert.ToInt64(item.Cells[1].Text),
							Acknowledged = true
						};

						alarmAndEventLogs.Modify(this.Security, alarmAndEventLog);
					}
				}
			}
		}

		/// <summary>
		/// Categories the drop down list selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void CategoryDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.CategoryDropDownList.SelectedValue == AllText)
			{
				this.Session.Remove("AlarmAndEventLogCategoryID");
			}
			else
			{
				this.Session["AlarmAndEventLogCategoryID"] = this.CategoryDropDownList.SelectedValue;
			}
		}

		/// <summary>
		/// IDs the drop down list selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void IDDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.IDDropDownList.SelectedValue == AllText)
			{
				this.Session.Remove("AlarmAndEventLogID");
			}
			else
			{
				this.Session["AlarmAndEventLogID"] = this.IDDropDownList.SelectedValue;
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
							x =>
							x.Get(this.Security, this.Security.SiteGuid, true, false, false)
					);

				if (!this.Page.IsPostBack)
				{
					DateTimeOffset siteTimeToday = TimeConverter.Today(this.CurrentSite);

					var dateFormat = this.Session["AlarmAndEventLogDateFormat"] as DateTimeFormatInfo;

					var beginningDateString = this.Session["AlarmAndEventLogBeginningDateTime"] as string;
					DateTimeOffset beginningDate;

					if (!string.IsNullOrEmpty(beginningDateString) && dateFormat != null && DateTimeOffset.TryParse(beginningDateString, dateFormat, DateTimeStyles.None, out beginningDate))
					{
						this.BeginningDateTime.Text = beginningDate.ToString(this.CurrentSite.GetDateTimeFormatInfo());
					}
					else
					{
						this.BeginningDateTime.Text = siteTimeToday.ToString(this.CurrentSite.GetDateTimeFormatInfo());
					}

					var endingDateString = this.Session["AlarmAndEventLogEndingDateTime"] as string;
					DateTimeOffset endingDate;

					if (!string.IsNullOrEmpty(endingDateString) && dateFormat != null && DateTimeOffset.TryParse(endingDateString, dateFormat, DateTimeStyles.None, out endingDate))
					{
						this.EndingDateTime.Text = endingDate.ToString(this.CurrentSite.GetDateTimeFormatInfo());
					}
					else
					{
						this.EndingDateTime.Text = siteTimeToday.AddDays(1).ToString(this.CurrentSite.GetDateTimeFormatInfo());
					}

					// Populate SiteDropDownList
					ListItem newItem;

					if (CurrentSite.SiteGroup)
					{
						newItem = new ListItem("{All}", string.Empty);
						SiteDropDownList.Items.Add(newItem);
						newItem = new ListItem("{" + CurrentSite.ID + "}", CurrentSite.SiteGuid.ToString());
						SiteDropDownList.Items.Add(newItem);

						if ((Session["AlarmAndEventLogSite"] != null) && (newItem.Text == (string)Session["AlarmAndEventLogSite"]))
						{
							SiteDropDownList.SelectedIndex = SiteDropDownList.Items.Count - 1;
						}

						foreach (SiteToSiteMapClass childSiteMap in CurrentSite.SiteToSiteMapCollection)
						{
							newItem = new ListItem(childSiteMap.ChildSiteID, childSiteMap.ChildSiteGuid.ToString());
							SiteDropDownList.Items.Add(newItem);

							if ((Session["AlarmAndEventLogSite"] != null) && (newItem.Text == (string)Session["AlarmAndEventLogSite"]))
							{
								SiteDropDownList.SelectedIndex = SiteDropDownList.Items.Count - 1;
							}
						}
					}
					else
					{
						newItem = new ListItem(CurrentSite.ID, CurrentSite.SiteGuid.ToString());
						SiteDropDownList.Items.Add(newItem);
					}

					Session["AlarmAndEventLogSite"] = SiteDropDownList.SelectedItem.Text;


					// Populate SourceDropDownList
					newItem = new ListItem(this.GetTranslatedText(AllText), AllText);
					this.SourceDropDownList.Items.Add(newItem);

					string discoveryAssem = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						configSettings => configSettings.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_IDiscoveryAssemblies));

					if (string.IsNullOrEmpty(discoveryAssem) == false)
					{
						char[] separator = { ';' };
						string[] discoveryAssemList = discoveryAssem.Split(separator, StringSplitOptions.RemoveEmptyEntries);

						string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
						foreach (string assemblyName in discoveryAssemList)
						{
							Assembly dll = null;
							if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
							{
								try
								{
									dll = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
								}
								catch
								{
									try
									{
										dll = Assembly.Load(assemblyName);
									}
									catch (Exception ex)
									{
										string message = "Assembly Load Error on Alarm And Event Logs Form. " + ex.Message;
										FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
									}
								}

								if (dll != null)
									AssemblyDictionary.Add(assemblyName.ToLower(), dll);
							}
							else
							{
								dll = AssemblyDictionary.Get(assemblyName.ToLower());
							}

							if (dll == null)
								continue;

							try
							{
								Type[] types = dll.GetTypes();

								foreach (Type module in types)
								{
									Type discoveryInterface = module.GetInterface("IAlarmAndEventDiscovery");

									if (discoveryInterface != null)
									{
										Object engine = Activator.CreateInstance(module);
										var discovery = (IAlarmAndEventDiscovery)engine;
										AlarmAndEventDescriptorClass[] descriptors = discovery.AlarmAndEvents;

										foreach (AlarmAndEventDescriptorClass descriptor in descriptors)
										{
											if (this.SourceDropDownList.Items.FindByValue(descriptor.Source) != null)
											{
												continue;
											}

											newItem = new ListItem(this.GetTranslatedText(descriptor.Source), descriptor.Source);
											this.SourceDropDownList.Items.Add(newItem);

											if ((this.Session["AlarmAndEventLogSource"] != null)
												 && (newItem.Text == (string)this.Session["AlarmAndEventLogSource"]))
											{
												this.SourceDropDownList.SelectedIndex = this.SourceDropDownList.Items.Count - 1;
											}
										}
									}
								}
							}
							catch { } // Try: Type[] types = dll.GetTypes()

							if (this.SourceDropDownList.SelectedIndex != -1)
							{
								this.Session["AlarmAndEventLogSource"] = this.SourceDropDownList.SelectedValue;
							}
						}
					}

					// Populate TypeDropDownList
					string[] alarmAndEventTypes = { "Alarms","Events","Alarms and Events","Inventory Management: Alarms" };

					if (this.Session["AlarmAndEventLogType"] == null)
					{
						this.Session["AlarmAndEventLogType"] = "Both";
					}

					foreach (string alarmAndEventType in alarmAndEventTypes)
					{
						newItem = new ListItem(this.GetTranslatedText(alarmAndEventType), alarmAndEventType);
						this.TypeDropDownList.Items.Add(newItem);

						if ((string)this.Session["AlarmAndEventLogType"] == alarmAndEventType)
						{
							this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
						}
					}

					this.Session["AlarmAndEventLogType"] = this.TypeDropDownList.SelectedValue;

					// Populate the CategoryDropDownList
					newItem = new ListItem(this.GetTranslatedText(AllText), AllText);
					this.CategoryDropDownList.Items.Add(newItem);
					this.CategoryDropDownList.SelectedValue = AllText;

					var categoryCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
						applicationStrings => applicationStrings.EnumerateByType(this.Security, STRING_TYPE.ALARM_EVENT_CATEGORY));

					foreach (ApplicationStringClass category in categoryCollection)
					{
						newItem = new ListItem(this.GetTranslatedText(category.ID), category.ID);
						foreach (ListItem existingItem in this.CategoryDropDownList.Items)
						{
							if (String.Compare(newItem.Text, existingItem.Text, StringComparison.Ordinal) < 0)
							{
								int index = this.CategoryDropDownList.Items.IndexOf(existingItem);
								this.CategoryDropDownList.Items.Insert(index, newItem);

								if (this.Session["AlarmAndEventLogCategoryID"] != null
									 && (string)this.Session["AlarmAndEventLogCategoryID"] == category.ID)
								{
									this.CategoryDropDownList.SelectedIndex = index;
								}

								newItem = null;
								break;
							}
						}

						if (newItem != null)
						{
							this.CategoryDropDownList.Items.Add(newItem);

							if (this.Session["AlarmAndEventLogCategoryID"] != null
								 && (string)this.Session["AlarmAndEventLogCategoryID"] == category.ID)
							{
								this.CategoryDropDownList.SelectedIndex = this.CategoryDropDownList.Items.Count - 1;
							}
						}
					}

					// Populate the PriorityDropDownList
					newItem = new ListItem(this.GetTranslatedText(AllText), AllText);
					this.PriorityDropDownList.Items.Add(newItem);

					var priorityCollection = FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(
						alarmPriorities => alarmPriorities.Enumerate(this.Security));

					foreach (AlarmPriorityClass priority in priorityCollection)
					{
						newItem = new ListItem(this.GetTranslatedText(priority.ID), priority.ID);
						foreach (ListItem existingItem in this.PriorityDropDownList.Items)
						{
							if (String.Compare(newItem.Text, existingItem.Text, StringComparison.Ordinal) < 0)
							{
								int index = this.PriorityDropDownList.Items.IndexOf(existingItem);
								this.PriorityDropDownList.Items.Insert(index, newItem);

								if ((this.Session["AlarmAndEventLogPriorityID"] != null)
									 && ((string)this.Session["AlarmAndEventLogPriorityID"] == priority.ID))
								{
									this.PriorityDropDownList.SelectedIndex = index;
								}

								newItem = null;
								break;
							}
						}

						if (newItem != null)
						{
							this.PriorityDropDownList.Items.Add(newItem);
							if ((this.Session["AlarmAndEventLogPriorityID"] != null)
								 && ((string)this.Session["AlarmAndEventLogPriorityID"] == priority.ID))
							{
								this.PriorityDropDownList.SelectedIndex = this.PriorityDropDownList.Items.Count - 1;
							}
						}
					}

					// are we looking at archive data?
					if (this.Session["AlarmAndEventLogUseArchiveData"] != null)
					{
						this.ArchiveCheckBox.Checked = (bool)this.Session["AlarmAndEventLogUseArchiveData"];
					}

					// Is there a persisted ID value?
					var id = this.Session["AlarmAndEventLogIDValue"] as string;
					if (string.IsNullOrEmpty(id))
					{
						id = string.Empty;
					}

					// Populate the ID dropdown list.
					this.SourceDropDownListSelectedIndexChanged(null, null);

					if (id != string.Empty)
					{
						int index = 0;
						foreach (ListItem item in this.IDDropDownList.Items)
						{
							if (item.Value == id)
							{
								this.IDDropDownList.SelectedIndex = index;
								this.Session["AlarmAndEventLogIDValue"] = id;
								break;
							}

							++index;
						}
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Priorities the drop down list selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void PriorityDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.PriorityDropDownList.SelectedValue == AllText)
			{
				this.Session.Remove("AlarmAndEventLogPriorityID");
			}
			else
			{
				this.Session["AlarmAndEventLogPriorityID"] = this.PriorityDropDownList.SelectedValue;
			}
		}

		/// <summary>
		/// Sources the drop down list selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SourceDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.IDDropDownList.Items.Clear();
			var newItem = new ListItem(this.GetTranslatedText(AllText), AllText);
			this.IDDropDownList.Items.Add(newItem);

			if (this.SourceDropDownList.SelectedValue == AllText)
			{
				this.Session.Remove("AlarmAndEventLogSource");
			}
			else
			{
				string type = "Both";

				if (this.Session["AlarmAndEventLogType"] != null)
				{
					type = (string)this.Session["AlarmAndEventLogType"];
				}

				this.Session["AlarmAndEventLogSource"] = this.SourceDropDownList.SelectedValue;

				string discoveryAssem = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					configSettings => configSettings.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_IDiscoveryAssemblies));

				if (string.IsNullOrEmpty(discoveryAssem) == false)
				{
					char[] separator = { ';' };
					string[] discoveryAssemList = discoveryAssem.Split(separator, StringSplitOptions.RemoveEmptyEntries);

					string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
					foreach (string assemblyName in discoveryAssemList)
					{
						Assembly dll = null;
						if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
						{
							try
							{
								dll = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
							}
							catch
							{
								try
								{
									dll = Assembly.Load(assemblyName);
								}
								catch (Exception ex)
								{
									string message = "Assembly Load Error on Alarm And Event Logs Source Select. " + ex.Message;
									FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
								}
							}

							if (dll != null)
								AssemblyDictionary.Add(assemblyName.ToLower(), dll);
						}
						else
						{
							dll = AssemblyDictionary.Get(assemblyName.ToLower());
						}

						if (dll == null)
							continue;

						try
						{
							Type[] types = dll.GetTypes();

							foreach (Type module in types)
							{
								Type discoveryInterface = module.GetInterface("IAlarmAndEventDiscovery");

								if (discoveryInterface != null)
								{
									Object engine = Activator.CreateInstance(module);
									var discovery = (IAlarmAndEventDiscovery)engine;
									AlarmAndEventDescriptorClass[] descriptors = discovery.AlarmAndEvents;

									foreach (AlarmAndEventDescriptorClass descriptor in descriptors)
									{
										if (type == "Events")
										{
											if (descriptor.Alarm)
											{
												continue;
											}
										}
										else if (type == "Alarms")
										{
											if (!descriptor.Alarm)
											{
												continue;
											}
										}

										if (this.SourceDropDownList.SelectedValue != descriptor.Source)
										{
											continue;
										}

										newItem = new ListItem(this.GetTranslatedText(descriptor.ID), descriptor.ID);
										this.IDDropDownList.Items.Add(newItem);

										if (this.IDDropDownList.SelectedIndex == 0 && descriptor.ID == this.Session["AlarmAndEventLogID"] as string)
										{
											this.IDDropDownList.SelectedIndex = this.IDDropDownList.Items.Count - 1;
										}
									}
								}
							}
						}
						catch { } // Try: Type[] types = dll.GetTypes()
					}
				}
			}

			if (this.IDDropDownList.SelectedIndex == 0)
			{
				this.Session.Remove("AlarmAndEventLogID");
			}
		}

		/// <summary>
		/// Types the drop down list selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.Session["AlarmAndEventLogType"] = this.TypeDropDownList.SelectedValue;
			this.SourceDropDownListSelectedIndexChanged(null, null);
		}

		/// <summary>
		/// Alarms the and event logs data grid page index changed.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridPageChangedEventArgs"/> instance containing the event data.</param>
		private void AlarmAndEventLogsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AlarmAndEventLogsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AlarmAndEventLogsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.Session.Remove("AlarmAndEventLogSelectAll");
			this.UpdateView();
		}

		/// <summary>
		/// Clears all button command.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		private void ClearAllButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("AlarmAndEventLogSelectAll");
			this.UpdateView();
		}

		/// <summary>
		/// Enumerates the alarm and event logs.
		/// </summary>
		/// <returns></returns>
		private ICollection EnumerateAlarmAndEventLogs()
		{
			bool selectAll;
			if (this.Session["AlarmAndEventLogSelectAll"] != null)
			{
				selectAll = true;
			}
			else
			{
				selectAll = false;
			}

			DateTimeOffset beginning = this.BeginningDateTime.CurrentValue;

			DateTimeOffset ending = this.EndingDateTime.CurrentValue;

			string source;
			if (this.Session["AlarmAndEventLogSource"] != null)
			{
				source = (string)this.Session["AlarmAndEventLogSource"];
			}
			else
			{
				source = string.Empty;
			}

			string type = "Both";

			if (this.Session["AlarmAndEventLogType"] != null)
			{
				type = (string)this.Session["AlarmAndEventLogType"];
			}

			this.SelectAllButton.Enabled = (type != "Events");
			this.ClearAllButton.Enabled = (type != "Events");
			this.AcknowledgeButton.Enabled = (type != "Events");

            // IAandEArchive does not support multi-site queries at this time. Switch to current site if {All} is selected. 
            if (type == "Inventory Management: Alarms" && CurrentSite.SiteGroup && this.SiteDropDownList.SelectedItem.Text == this.GetTranslatedText("{All}"))
            {
                SiteDropDownList.SelectedIndex = 1;
            }

            string id = string.Empty;
			if (this.Session["AlarmAndEventLogID"] != null)
			{
				id = (string)this.Session["AlarmAndEventLogID"];
			}

			string categoryID = string.Empty;
			if (this.Session["AlarmAndEventLogCategoryID"] != null)
			{
				categoryID = (string)this.Session["AlarmAndEventLogCategoryID"];
			}

			string priorityID = string.Empty;
			if (this.Session["AlarmAndEventLogPriorityID"] != null)
			{
				priorityID = (string)this.Session["AlarmAndEventLogPriorityID"];
			}

			var beginningDateAndTime = new DateAndTime(this.CurrentSite);
			var endingDateAndTime = new DateAndTime(this.CurrentSite);

			beginningDateAndTime.Value = beginning;
			endingDateAndTime.Value = ending;

			Guid siteGuid = Security.SiteGuid;
			bool includeMemberSites = false;
			bool includeGlobalSites = false;
			if (this.SiteDropDownList.SelectedItem.Text == this.GetTranslatedText("{All}"))
			{
				includeMemberSites = true;
				includeGlobalSites = true;
			}
			else
			{
				siteGuid = new Guid(this.SiteDropDownList.SelectedValue);
			}

			this.Session["AlarmAndEventLogSite"] = this.SiteDropDownList.SelectedItem.Text;

			Guid currentSiteGuid = Security.SiteGuid;

			Security.SiteGuid = siteGuid;

			bool useArchiveData = false;
			try
			{
				if (this.Session["AlarmAndEventLogUseArchiveData"] != null)
				{
					useArchiveData = (bool)this.Session["AlarmAndEventLogUseArchiveData"];
				}

			}
			catch (Exception)
			{
				useArchiveData = false;
			}

			AlarmAndEventLogCollectionClass alarmAndEventLogCollection;

			try
			{
				alarmAndEventLogCollection =
					 FMChannelHelper.MakeCall<IAlarmAndEventLogs, AlarmAndEventLogCollectionClass>(
						  alarmAndEventLogs =>
								alarmAndEventLogs.Enumerate(
									 this.Security,
									 beginningDateAndTime.Value,
									 endingDateAndTime.Value,
									 source,
									 type,
									 id,
									 categoryID,
									 priorityID,
									 includeMemberSites,
									 useArchiveData,
									 includeGlobalSites));

			}
			finally
			{
				Security.SiteGuid = currentSiteGuid;
			}

			string maxAlarmsEventsConfig = ConfigurationManager.AppSettings.Get("AlarmAndEventLogLimit");
			int maxAlarmsEvents;
			if (string.IsNullOrEmpty(maxAlarmsEventsConfig))
         {
				maxAlarmsEvents = 10000;
         }
			else
         {
				if (!int.TryParse(maxAlarmsEventsConfig, out maxAlarmsEvents))
            {
					maxAlarmsEvents = 10000;
            }
         }

			if (alarmAndEventLogCollection.Count >= maxAlarmsEvents)
			{
				this.AlarmAndEventLogsDataGrid.DataSource = null;
				this.AlarmAndEventLogsDataGrid.DataBind();
				string display = $"Returned dataset is greater than {maxAlarmsEvents} records. Change the selection criteria to narrow the returned data.";
				throw new Exception(display);
				//this.Page.ClientScript.RegisterStartupScript(this.GetType(), "CloseoutSuccessScript", "alert('" + display + "');", true);
			}

			var alarmAndEventLogDataTable = new DataTable();

			alarmAndEventLogDataTable.Columns.Add("Selected", typeof(bool));
			alarmAndEventLogDataTable.Columns.Add("SequenceNumber", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("CreatedDate", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("SiteID", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("Source", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("ID", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("Data", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("CategoryID", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("PriorityID", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("UpdatedBy", typeof(string));
			alarmAndEventLogDataTable.Columns.Add("Alarm", typeof(bool));
			alarmAndEventLogDataTable.Columns.Add("Acknowledged", typeof(bool));

			var createdDateAndTime = new DateAndTime(this.CurrentSite);
			foreach (AlarmAndEventLogClass alarmAndEventLog in alarmAndEventLogCollection)
			{
				DataRow alarmAndEventLogDataRow = alarmAndEventLogDataTable.NewRow();

				alarmAndEventLogDataRow["Selected"] = (selectAll && alarmAndEventLog.Alarm);
				alarmAndEventLogDataRow["SequenceNumber"] = alarmAndEventLog.SequenceNumber.ToString(CultureInfo.InvariantCulture);

				createdDateAndTime.Value = alarmAndEventLog.CreatedDate;
				alarmAndEventLogDataRow["CreatedDate"] = createdDateAndTime.ToString();

				alarmAndEventLogDataRow["SiteID"] = alarmAndEventLog.SiteID;
				alarmAndEventLogDataRow["Source"] = this.GetTranslatedText(alarmAndEventLog.Source);
				alarmAndEventLogDataRow["ID"] = this.GetTranslatedText(alarmAndEventLog.ID);

				alarmAndEventLogDataRow["Data"] = HttpUtility.HtmlEncode(alarmAndEventLog.AssociatedData);
				alarmAndEventLogDataRow["CategoryID"] = this.GetTranslatedText(alarmAndEventLog.CategoryID);
				alarmAndEventLogDataRow["PriorityID"] = this.GetTranslatedText(alarmAndEventLog.PriorityID);
				alarmAndEventLogDataRow["UpdatedBy"] = alarmAndEventLog.UpdatedBy;
				alarmAndEventLogDataRow["Alarm"] = alarmAndEventLog.Alarm;
				alarmAndEventLogDataRow["Acknowledged"] = alarmAndEventLog.Acknowledged;

				alarmAndEventLogDataTable.Rows.Add(alarmAndEventLogDataRow);
			}

			var alarmAndEventLogDataView = new DataView(alarmAndEventLogDataTable);
			return alarmAndEventLogDataView;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.RefreshButton.Command += this.RefreshButtonCommand;
			this.AlarmAndEventLogsDataGrid.PageIndexChanged += this.AlarmAndEventLogsDataGridPageIndexChanged;
			this.SelectAllButton.Command += this.SelectAllButtonCommand;
			this.ClearAllButton.Command += this.ClearAllButtonCommand;
		}

		/// <summary>
		/// Handles the Command event of the RefreshButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		private void RefreshButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				//verify beginning date recent than end date
				if (DateTimeOffset.Parse(this.BeginningDateTime.Text, this.CurrentSite.GetDateTimeFormatInfo())
					 > DateTimeOffset.Parse(this.EndingDateTime.Text, this.CurrentSite.GetDateTimeFormatInfo()))
				{
					throw new Exception("Ending Date must be more recent than Beginning Date");
				}

                this.Session["AlarmAndEventLogBeginningDateTime"] = this.BeginningDateTime.Text;
				this.Session["AlarmAndEventLogEndingDateTime"] = this.EndingDateTime.Text;
				this.Session["AlarmAndEventLogDateFormat"] = this.CurrentSite.GetDateTimeFormatInfo();
				this.Session["AlarmAndEventLogUseArchiveData"] = this.ArchiveCheckBox.Checked;


				this.AlarmAndEventLogsDataGrid.CurrentPageIndex = 0;
				this.Session.Remove("AlarmAndEventLogSelectAll");
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Command event of the SelectAllButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		private void SelectAllButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session["AlarmAndEventLogSelectAll"] = true;
			this.UpdateView();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			ICollection log = this.EnumerateAlarmAndEventLogs();

			this.AlarmAndEventLogsDataGrid.DataSource = log;
			this.AlarmAndEventLogsDataGrid.DataBind();
		}

		#endregion
	}
}
