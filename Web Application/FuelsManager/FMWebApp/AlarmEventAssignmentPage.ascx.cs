namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Reflection;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
   using global::FMWebApp;

   /// <summary>
   /// Code behind for AlarmEventAssignmentPage.
   /// </summary>
	public partial class AlarmEventAssignmentPage : FMUserControlBase, IEntityDiscovery
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
				return typeof(IAlarmAndEvents);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.ALARM_AND_EVENT;
			}
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (type == ENTITY_ASSIGNMENT_TYPE.OWNED)
			{
				var alarmAndEventCollection = FMChannelHelper.MakeCall<IAlarmAndEvents, AlarmAndEventCollectionClass>(
						alarmAndEvents => alarmAndEvents.Enumerate(security));

				foreach (AlarmAndEventClass alarmAndEvent in alarmAndEventCollection)
				{
					if (alarmAndEvent.SiteGuid == security.SiteGuid)
					{
						var entityToSiteMap = new EntityToSiteMapClass();
						entityToSiteMap.ID = "All Alarm & Events";
						entityToSiteMapCollection.Add(entityToSiteMap);
						break;
					}
				}
			}
			else
			{
				EntityToSiteMapClass entityToSiteMap =
					FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
						entityToSiteMaps => entityToSiteMaps.Get(security, ((IEntityDiscovery)this).EntityType, security.LoginSiteGuid));

				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.LoginSiteGuid == entityToSiteMap.IdentityGuid)
					{
						entityToSiteMap.ID = "All Alarm & Events";
						entityToSiteMapCollection.Add(entityToSiteMap);
					}
				}
				else
				{
					if (entityToSiteMap.IdentityGuid == Guid.Empty)
					{
						entityToSiteMap = new EntityToSiteMapClass
						{
							SiteGuid = Guid.Empty,
							ID = "All Alarm & Events",
							TypeID = ((IEntityDiscovery)this).EntityType,
							IdentityGuid = security.LoginSiteGuid
						};

						entityToSiteMapCollection.Add(entityToSiteMap);
					}
				}
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			EntityToSiteMapClass entityToSiteMap = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
				entityToSiteMaps => entityToSiteMaps.Get(security, ((IEntityDiscovery)this).EntityType, security.LoginSiteGuid));

			return (entityToSiteMap.IdentityGuid == Guid.Empty) ? security.SiteGuid : entityToSiteMap.IdentityGuid;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<IEntityToSiteMaps>(
				entityToSiteMaps =>
				{
					EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
						security, ((IEntityDiscovery)this).EntityType, security.SiteGuid);

					foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
					{
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				});

			FMChannelHelper.MakeCall<IAlarmAndEvents>(
				alarmAndEvents =>
				{
					AlarmAndEventCollectionClass alarmAndEventCollection = alarmAndEvents.Enumerate(security);
					foreach (AlarmAndEventClass alarmAndEvent in alarmAndEventCollection)
					{
						if (alarmAndEvent.SiteGuid == security.SiteGuid)
						{
							alarmAndEvent.SiteGuid = siteGuid;
							alarmAndEvents.Modify(security, alarmAndEvent);
						}
					}
				});
		}

		#endregion

		#region Methods

		/// <summary>
		///     This method enables and disables controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			this.TypeDropDownList.Enabled = enable;
			this.SourceDropDownList.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var alarmEventConfigurationForm = (AlarmEventConfigurationForm)this.Page;
			alarmEventConfigurationForm.EnableControls(enable);
		}

		protected ListItemCollection EnumerateCategories()
		{
			var listItems = new ListItemCollection();

			var newItem = new ListItem();

			var categoryCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
				applicationStrings => applicationStrings.EnumerateByType(this.Security, STRING_TYPE.ALARM_EVENT_CATEGORY));

			foreach (ApplicationStringClass category in categoryCollection)
			{
				newItem = new ListItem(category.ID, category.IdentityGuid.ToString());
				foreach (ListItem existingItem in listItems)
				{
					if (String.Compare(newItem.Text, existingItem.Text, StringComparison.Ordinal) < 0)
					{
						int index = listItems.IndexOf(existingItem);
						listItems.Insert(index, newItem);
						newItem = null;
						break;
					}
				}

				if (newItem != null)
				{
					listItems.Add(newItem);
				}
			}

			newItem = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
			listItems.Insert(0, newItem);

			return listItems;
		}

		protected ListItemCollection EnumeratePriorities()
		{
			var listItems = new ListItemCollection();

			var newItem = new ListItem();

			AlarmPriorityCollectionClass priorityCollection =
				FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(alarmPriorities => alarmPriorities.Enumerate(this.Security));

			foreach (AlarmPriorityClass priority in priorityCollection)
			{
				newItem = new ListItem(priority.ID, priority.IdentityGuid.ToString());
				foreach (ListItem existingItem in listItems)
				{
					if (String.Compare(newItem.Text, existingItem.Text, StringComparison.Ordinal) < 0)
					{
						int index = listItems.IndexOf(existingItem);
						listItems.Insert(index, newItem);
						newItem = null;
						break;
					}
				}

				if (newItem != null)
				{
					listItems.Add(newItem);
				}
			}

			newItem = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
			listItems.Insert(0, newItem);
			return listItems;
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
		/// Handles the SelectedIndexChanged event of the PageSizeDropDown control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
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
				if (this.Page.IsPostBack == false)
				{
					// Populate SourceDropDownList
					// Get the Varec Assemblies registry key.
					string discoveryAssem = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						configSettings => configSettings.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_IDiscoveryAssemblies));

					if (string.IsNullOrEmpty(discoveryAssem) == false)
					{
						char[] separator = { ';' };
						string[] discoveryAssemList = discoveryAssem.Split(separator, StringSplitOptions.RemoveEmptyEntries);

						// Loop through the assemblies.  With each one iterate through all the types contained
						// withing the assembly.  If the type implements the IAlarmAndEventDiscovery interface
						// add it to the source drop down.
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
										string message = "Assembly Load Error on Alarm Event Assignment Page. " + ex.Message;
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

									// An instance of IAlarmAndEventDiscovery interface was found.  Grab the alarm
									// and event descriptor objects and use those to populate the source drop down
									if (discoveryInterface != null)
									{
										Object engine = Activator.CreateInstance(module);
										var discovery = (IAlarmAndEventDiscovery)engine;

										AlarmAndEventDescriptorClass[] descriptors = discovery.AlarmAndEvents;

										foreach (AlarmAndEventDescriptorClass descriptor in descriptors)
										{
											if (this.SourceDropDownList.Items.FindByText(descriptor.Source) != null)
											{
												continue;
											}

											var newItem = new ListItem(descriptor.Source, descriptor.Source);

											foreach (ListItem existingItem in this.SourceDropDownList.Items)
											{
												if (String.Compare(newItem.Text, existingItem.Text, StringComparison.Ordinal) < 0)
												{
													int index = this.SourceDropDownList.Items.IndexOf(existingItem);
													this.SourceDropDownList.Items.Insert(index, newItem);

													if ((this.Session["AlarmEventAssignmentSource"] != null)
														 && (newItem.Text == (string)this.Session["AlarmEventAssignmentSource"]))
													{
														this.SourceDropDownList.SelectedIndex = index;
													}

													newItem = null;
													break;
												}
											}

											if (newItem != null)
											{
												this.SourceDropDownList.Items.Add(newItem);

												if ((this.Session["AlarmEventAssignmentSource"] != null)
													 && (newItem.Text == (string)this.Session["AlarmEventAssignmentSource"]))
												{
													this.SourceDropDownList.SelectedIndex = this.SourceDropDownList.Items.Count - 1;
												}
											}
										}
									}
								}
							}
							catch { } // Try: Type[] types = dll.GetTypes()
						}

						if (this.SourceDropDownList.SelectedIndex != -1)
						{
							this.Session["AlarmEventAssignmentSource"] = this.SourceDropDownList.SelectedItem.Value;
						}
					}

					// Populate TypeDropDownList
					string[] alarmAndEventTypes = { "Alarms", "Events" };

					foreach (string alarmAndEventType in alarmAndEventTypes)
					{
						var newItem = new ListItem(alarmAndEventType, alarmAndEventType);
						this.TypeDropDownList.Items.Add(newItem);

						if ((this.Session["AlarmEventAssignmentType"] != null)
							 && ((string)this.Session["AlarmEventAssignmentType"] == alarmAndEventType))
						{
							this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
						}
					}

					this.Session["AlarmEventAssignmentType"] = this.TypeDropDownList.SelectedValue;

					// Remove Assignments for Sources that are no longer present
					FMChannelHelper.MakeCall<IAlarmAndEvents>(
						alarmAndEvents =>
						{
							string[] sources = alarmAndEvents.EnumerateSources(this.Security);

							foreach (string source in sources)
							{
								if (this.SourceDropDownList.Items.FindByValue(source) != null)
								{
									continue;
								}

								AlarmAndEventCollectionClass alarmAndEventCollection = alarmAndEvents.EnumerateBySourceAndType(
									this.Security, source, (string)this.Session["AlarmEventAssignmentType"]);

								foreach (AlarmAndEventClass alarmAndEvent in alarmAndEventCollection)
								{
									alarmAndEvents.Purge(this.Security, alarmAndEvent.IdentityGuid);
								}
							}
						});

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the SourceDropDownList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SourceDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.Session["AlarmEventAssignmentSource"] = this.SourceDropDownList.SelectedItem.Value;
			this.AssignmentDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the TypeDropDownList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.Session["AlarmEventAssignmentType"] = this.TypeDropDownList.SelectedItem.Value;
			this.AssignmentDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected void UpdateView()
		{
			ICollection assignments = this.EnumerateAssignments();

			this.AlarmAssignPageSizeDropDown.SetPageSize(this.AssignmentDataGrid, assignments.Count);

			this.AssignmentDataGrid.DataSource = assignments;
			this.AssignmentDataGrid.DataBind();
		}

		/// <summary>
		/// Assignments the data grid cancel command.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void AssignmentDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			this.AssignmentDataGrid.EditItemIndex = -1;

			// Enable controls after line item editing.
			this.EnableControls(true);
			this.UpdateView();
		}

		/// <summary>
		/// Handles the EditCommand event of the AssignmentDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void AssignmentDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.AssignmentDataGrid.EditItemIndex = e.Item.ItemIndex;

			// Disable controls while in line item edit mode.
			this.EnableControls(false);
			this.UpdateView();
		}

		/// <summary>
		/// Assignments the data grid item data bound.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
		private void AssignmentDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var editButton = (LinkButton)e.Item.FindControl("EditButton");
			if (editButton != null)
			{
				var siteGuidLabel = (Label)e.Item.FindControl("SiteGuidLabel");
				if (siteGuidLabel != null)
				{
					if (this.Security.SiteGuid != Guid.Parse(siteGuidLabel.Text)
						 || !this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						FMControls.FMElipseButton editEmailForm = e.Item.FindControl("EditEmailForm") as FMControls.FMElipseButton;
						if (editEmailForm != null)
						{
							editEmailForm.Enabled = false;
						}

						editButton.Enabled = false;
						editButton.Text = "<img src=Images/Edit_un.gif border=0 align=absmiddle alt='Edit this item'>";
					}
					var assignmentDataView = (DataView)this.AssignmentDataGrid.DataSource;
               string cat = assignmentDataView[e.Item.DataSetIndex][3] as string;
               string pri = assignmentDataView[e.Item.DataSetIndex][4] as string;
					if ((string.IsNullOrWhiteSpace(cat) || cat == "{None}") && (string.IsNullOrWhiteSpace(pri) || pri == "{None}"))

               {
                  FMControls.FMElipseButton editEmailForm = e.Item.FindControl("EditEmailForm") as FMControls.FMElipseButton;
                  if (editEmailForm != null)
                  {
                     editEmailForm.Enabled = false;
                  }
               }
				}
			}

			var categoryDropDownList = (DropDownList)e.Item.FindControl("CategoryDropDownList");
			var priorityDropDownList = (DropDownList)e.Item.FindControl("PriorityDropDownList");

			if ((categoryDropDownList != null) && (priorityDropDownList != null))
			{
				var assignmentDataView = (DataView)this.AssignmentDataGrid.DataSource;

				foreach (ListItem item in this.EnumerateCategories())
				{
					categoryDropDownList.Items.Add(item);

					if ((string)assignmentDataView[e.Item.DataSetIndex][3] == item.Text)
					{
						categoryDropDownList.SelectedIndex = categoryDropDownList.Items.Count - 1;
					}
				}

				foreach (ListItem item in this.EnumeratePriorities())
				{
					priorityDropDownList.Items.Add(item);

					if ((string)assignmentDataView[e.Item.DataSetIndex][4] == item.Text)
					{
						priorityDropDownList.SelectedIndex = priorityDropDownList.Items.Count - 1;
					}
				}
			}
		}

		/// <summary>
		/// Handles the PageIndexChanged event of the AssignmentDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridPageChangedEventArgs"/> instance containing the event data.</param>
		private void AssignmentDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AssignmentDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AssignmentDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		/// Handles the UpdateCommand event of the AssignmentDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void AssignmentDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var identityGuidLabel = (Label)e.Item.FindControl("IdentityGuidLabel");

				if (identityGuidLabel != null)
				{
					var categoryGuidOriginal = (Label)e.Item.FindControl("lblCategoryGuidOriginal");
					var priorityGuidOriginal = (Label)e.Item.FindControl("lblPriorityGuidOriginal");

					var alarmAndEvent = new AlarmAndEventClass();
					alarmAndEvent.IdentityGuid = Guid.Parse(identityGuidLabel.Text);
					alarmAndEvent.SiteGuid = this.Security.SiteGuid;
					alarmAndEvent.Source = this.SourceDropDownList.SelectedItem.Value;
					alarmAndEvent.Alarm = ((string)this.Session["AlarmEventAssignmentType"] == "Alarms");
					alarmAndEvent.CategoryGuid = Guid.Parse(categoryGuidOriginal.Text);
					alarmAndEvent.PriorityGuid = Guid.Parse(priorityGuidOriginal.Text);

					var idLabel = (Label)e.Item.FindControl("IDLabel");
					alarmAndEvent.ID = idLabel.Text;

					var categoryDropDownList = (DropDownList)e.Item.FindControl("CategoryDropDownList");
					var categoryChanged = (alarmAndEvent.CategoryGuid != Guid.Parse(categoryDropDownList.SelectedValue)) ? true : false;
					alarmAndEvent.CategoryGuid = Guid.Parse(categoryDropDownList.SelectedValue);
					if (categoryDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
					{
						alarmAndEvent.CategoryID = "{None}";
					}
					else
					{
						alarmAndEvent.CategoryID = categoryDropDownList.SelectedItem.Text;
					}

					bool priorityChanged = false;

					if (alarmAndEvent.Alarm)
					{
						var priorityDropDownList = (DropDownList)e.Item.FindControl("PriorityDropDownList");
						priorityChanged = (alarmAndEvent.PriorityGuid != Guid.Parse(priorityDropDownList.SelectedValue)) ? true : false;
						alarmAndEvent.PriorityGuid = Guid.Parse(priorityDropDownList.SelectedValue);
						if (priorityDropDownList.SelectedItem.Text == this.GetTranslatedText("None"))
						{
							alarmAndEvent.PriorityID = "";
						}
						else
						{
							alarmAndEvent.PriorityID = priorityDropDownList.SelectedItem.Text;
						}
					}

					var enabledCheckbox = (CheckBox)e.Item.FindControl("EnabledCheckbox");
					// find the label containing the original value of enabled.  If the values have
					// changed then the event/alarm needs to be saved in the database.
					var enabledOriginal = (Label)e.Item.FindControl("lblEnabledOriginal");
					bool enabledChanged = (enabledCheckbox.Checked.ToString() != enabledOriginal.Text);
					alarmAndEvent.Enabled = enabledCheckbox.Checked;

					if (enabledChanged || categoryChanged || priorityChanged)
					{

						FMChannelHelper.MakeCall<IAlarmAndEvents>(
							alarmAndEvents =>
								{
									if (alarmAndEvent.Enabled == true
									&& alarmAndEvent.CategoryGuid == Guid.Empty
									&& alarmAndEvent.PriorityGuid == Guid.Empty)
									{
										alarmAndEvents.Purge(this.Security, alarmAndEvent.IdentityGuid);
									}
									else
									{
										if (alarmAndEvent.IdentityGuid == Guid.Empty)
										{
											alarmAndEvents.Add(this.Security, alarmAndEvent);
										}
										else
										{
											alarmAndEvents.Modify(this.Security, alarmAndEvent);
										}
									}
								});
					}

					this.AssignmentDataGrid.EditItemIndex = -1;

					// Enable controls after line item editing.
					this.EnableControls(true);
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				// Enable controls after line item editing.
				this.EnableControls(true);

				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Enumerates the assignments.
		/// </summary>
		/// <returns>A collection of assignments.</returns>
		private ICollection EnumerateAssignments()
		{
			var assignmentsDataTable = new DataTable();

			assignmentsDataTable.Columns.Add("SiteGuid", typeof(Guid));
			assignmentsDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			assignmentsDataTable.Columns.Add("ID", typeof(string));
			assignmentsDataTable.Columns.Add("CategoryID", typeof(string));
			assignmentsDataTable.Columns.Add("PriorityID", typeof(string));
			assignmentsDataTable.Columns.Add("Alarm", typeof(bool));
			assignmentsDataTable.Columns.Add("Enabled", typeof(bool));
			assignmentsDataTable.Columns.Add("CategoryGuid", typeof(string));
			assignmentsDataTable.Columns.Add("PriorityGuid", typeof(string));

			string translatedNone = this.GetTranslatedText("{None}");

			if (this.SourceDropDownList.SelectedIndex != -1)
			{
				bool alarms = (string)this.Session["AlarmEventAssignmentType"] == "Alarms";

				// If the alarms and events have been assigned down from another site, we need to 
				// use that site guid rather than the current site guid when creating entries 
				// for alarm and events that aren't configured (i.e. those discovered through reflection rather than those that exist in the DB).
				// Otherwise, they'll show up as editable even though they shouldn't be because they've been assigned down from a parent site.
				Guid owningSiteGuid = this.Security.SiteGuid;

				EntityToSiteMapCollectionClass alarmAndEventAssignment = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
					 entityToSiteMaps => entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(this.Security, ENTITY_TYPE.ALARM_AND_EVENT, this.Security.SiteGuid));

				if (alarmAndEventAssignment.Count > 0)
				{
					owningSiteGuid = alarmAndEventAssignment[0].AssignedFromSiteGuid;
				}

				AlarmAndEventCollectionClass alarmAndEventCollection =
					FMChannelHelper.MakeCall<IAlarmAndEvents, AlarmAndEventCollectionClass>(
						alarmAndEvents =>
						alarmAndEvents.EnumerateBySourceAndType(
							this.Security,
							this.Session["AlarmEventAssignmentSource"] as string,
							this.Session["AlarmEventAssignmentType"] as string));

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
									string message = "Assembly Load Error on Alarm Event Enumerate Assignments. " + ex.Message;
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

									// FuelsManager comes with "default" alarms and events that are "discovered" using
									// reflection on dll's listed in a registry key.
									foreach (AlarmAndEventDescriptorClass descriptor in descriptors)
									{
										if (descriptor.Alarm != alarms)
										{
											continue;
										}

										if (descriptor.Source != this.SourceDropDownList.SelectedItem.Value)
										{
											continue;
										}

										DataRow assignmentsDataRow = assignmentsDataTable.NewRow();
										assignmentsDataRow["SiteGuid"] = owningSiteGuid;
										assignmentsDataRow["IdentityGuid"] = Guid.Empty;
										assignmentsDataRow["ID"] = descriptor.ID;
										assignmentsDataRow["CategoryID"] = translatedNone;
										assignmentsDataRow["PriorityID"] = (alarms) ? translatedNone : string.Empty;
										assignmentsDataRow["Alarm"] = descriptor.Alarm;
										assignmentsDataRow["Enabled"] = true;
										assignmentsDataRow["CategoryGuid"] = Guid.Empty;
										assignmentsDataRow["PriorityGuid"] = Guid.Empty;

										// In order to modify the default events/alarms a copy of the default is
										// stored in the database with updated values.  Should one of the events
										// in the collection from the database have the same ID as the discovered
										// default event then use the attributes pulled from the database to 
										// populate the event object.
										foreach (AlarmAndEventClass alarmAndEvent in alarmAndEventCollection)
										{
											if (alarmAndEvent.ID == descriptor.ID)
											{
												assignmentsDataRow["SiteGuid"] = alarmAndEvent.SiteGuid;
												assignmentsDataRow["IdentityGuid"] = alarmAndEvent.IdentityGuid;
												assignmentsDataRow["CategoryID"] = alarmAndEvent.CategoryID;
												assignmentsDataRow["PriorityID"] = alarmAndEvent.PriorityID;
												assignmentsDataRow["Enabled"] = alarmAndEvent.Enabled;
												assignmentsDataRow["CategoryGuid"] = alarmAndEvent.CategoryGuid;
												assignmentsDataRow["PriorityGuid"] = alarmAndEvent.PriorityGuid;
												alarmAndEventCollection.Remove(alarmAndEvent);
												break;
											}
										}
										if (!string.IsNullOrEmpty(assignmentsDataRow["ID"] as string))
										{
											assignmentsDataTable.Rows.Add(assignmentsDataRow);
										}
									}
								}
							}
						}
						catch { } // Try: Type[] types = dll.GetTypes()
					}
			}

				FMChannelHelper.MakeCall<IAlarmAndEvents>(
					alarmAndEvents =>
						{
							// Delete all AlarmAndEvent records no longer supported by Source
							foreach (AlarmAndEventClass alarmAndEvent in alarmAndEventCollection)
							{
								bool found = false;
								foreach (DataRow row in assignmentsDataTable.Rows)
								{
									if ((string)row["ID"] == alarmAndEvent.ID)
									{
										found = true;
										break;
									}
								}

								if (!found)
								{
									alarmAndEvents.Purge(this.Security, alarmAndEvent.IdentityGuid);
								}
							}
						});
			}

			var assignmentsDataView = new DataView(assignmentsDataTable) { Sort = "ID" };
			return assignmentsDataView;
		}

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignmentDataGrid.EditCommand += this.AssignmentDataGridEditCommand;
			this.AssignmentDataGrid.PageIndexChanged += this.AssignmentDataGridPageIndexChanged;
			this.AssignmentDataGrid.CancelCommand += this.AssignmentDataGridCancelCommand;
			this.AssignmentDataGrid.UpdateCommand += this.AssignmentDataGridUpdateCommand;
			this.AssignmentDataGrid.ItemDataBound += this.AssignmentDataGridItemDataBound;
		}

		#endregion
	}
}
