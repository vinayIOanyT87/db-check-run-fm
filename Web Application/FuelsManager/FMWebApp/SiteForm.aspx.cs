// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SiteForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Net.Sockets;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	/// <summary>
	///    Summary description for SiteForm.
	/// </summary>
	public partial class SiteForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		protected TextBox InputDirectory;

		protected Label Label1;

		protected Label Label2;

		protected Label Label6;

		protected Label Label7;

		protected Label Label8;

		protected TextBox OutputDirectory;

		protected Label UserRequired;

		protected DropDownList FromDropDownList;

		protected DropDownList ToDropDownList;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the site form.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				this.OK.Enabled = enable;
			}

			this.Cancel.Enabled = enable;

			this.tcSiteTabs.HeaderEnabled = enable;
		}

		public void UpdateData()
		{
			this.SiteGeneralPage.UpdateData();
			this.SiteContactsPage.UpdateData();
			this.SiteSystemPage.UpdateData();
			this.SiteTransactionPage.UpdateData();
			this.SiteUserDataPage.UpdateData();
			this.SiteLoadRackPage.UpdateData();
			this.SiteNotesPage.UpdateData();
			this.SiteGroupPage.UpdateData();
			this.SiteOpcUaPage.UpdateData();
			this.SiteReportsPage.UpdateData();
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

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("Status");

				this.GetSecurity();

				SiteClass site;

				if (!this.Page.IsPostBack)
				{
					site = (SiteClass)this.Session["Site"];

					if (site == null)
					{
						this.Session.Remove("TabIndex");

						// Get Site
						if (this.Session["IdentityGuid"] != null)
						{
							site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, ((Guid)this.Session["IdentityGuid"]), true, true, true)
																);
						}
						else
						{
							site = new SiteClass();
						}

						this.Session["Site"] = site;
					}

					this.Session.Remove("SiteHolidayScheduleModified");

					//Set the title label with a key field from the bound object appended
					if (site != null)
					{
						this.labSiteConfig.Text = this.GetTitleLabelText(this.labSiteConfig.Text, site.ID);
					}
				}

				else
				{
					if (this.Session["Site"] == null)
					{
						throw new Exception("Site not in Session");
					}



					site = (SiteClass)this.Session["Site"];

					if (site.SiteGroup != this.SiteGeneralPage.SiteGroup)
					{
						site.SiteGroup = this.SiteGeneralPage.SiteGroup;

						if (!site.SiteGroup)
						{
							site.SiteToSiteMapCollection = new SiteToSiteMapCollectionClass();
						}
						else
						{
							site.InventoryTransactionAliasGuid = Guid.Empty;
							site.InventoryTransactionAliasID = "";
							site.AdjustmentTransactionAliasGuid = Guid.Empty;
							site.AdjustmentTransactionAliasID = "";
						}

					}

					if(site.Enterprise != this.SiteGeneralPage.Enterprise)
                    {
						site.Enterprise = this.SiteGeneralPage.Enterprise;
                    }
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				{
					this.OK.Enabled = false;
				}

				// Set up tab text and enable/disable
				this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");
				this.tpContactsPage.HeaderText = this.GetTranslatedText("Contacts");
				this.tpUnitsPage.HeaderText = this.GetTranslatedText("Units");
				this.tpLoadRackPage.HeaderText = this.GetTranslatedText("Load Rack");
				this.tpTransactionPage.HeaderText = this.GetTranslatedText("Transaction Tickets");
				this.tpOperatingSchedulePage.HeaderText = this.GetTranslatedText("Terminal Schedule");
				this.tpSystemPage.HeaderText = this.GetTranslatedText("System");
				this.tpProcessVariablesPage.HeaderText = this.GetTranslatedText("Process Variables");
				this.tpUserDataPage.HeaderText = this.GetTranslatedText("User Data");
				this.tpGroupPage.HeaderText = this.GetTranslatedText("Sites");
				this.tpNotesPage.HeaderText = this.GetTranslatedText("Notes");
				this.tpCertificatePage.HeaderText = this.GetTranslatedText("Certificates");
				this.tpOpcUaPage.HeaderText = this.GetTranslatedText("Opc Ua");
				this.tpSiteReportsPage.HeaderText = this.GetTranslatedText("Reports");

                if (site != null)
				{
					this.tpLoadRackPage.Visible = !site.SiteGroup;
					this.tpOperatingSchedulePage.Visible = !site.SiteGroup;
					this.tpProcessVariablesPage.Visible = !site.SiteGroup;
					this.tpGroupPage.Visible = site.SiteGroup;
					this.tpOpcUaPage.Visible = !site.Enterprise;
				}

				if (this.Security.SiteGuid != Guids.SiteAdminGuid)
				{
					this.tpGroupPage.Visible = false;
				}


				// We could be coming back from another page
				if (this.Session["TabIndex"] != null)
				{
					this.tcSiteTabs.ActiveTabIndex = (int)this.Session["TabIndex"];
					this.Session.Remove("TabIndex");
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("User");
			this.Session.Remove("Site");
			this.Redirect("SitesForm.aspx");
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.No.Command += this.CancelCommand;
			this.Yes.Command += this.OkCommand;
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData();

				var site = (SiteClass)this.Session["Site"];

				Guid guid = site.SiteGuid;
				if (site.SiteGuid != Guid.Empty)
				{
					try
					{
						var button = sender as FMButton;
						if (button != null && (button.ID == "OK" || button.ID == "Yes"))
						{
							FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, site, (button.ID != "OK"))
																);
						}
						else
						{
							FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, site, true)
																);
						}

						foreach (SiteToSiteMapClass siteToSiteMap in site.SiteToSiteMapCollection)
						{
							var userToSiteMap = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
											 x =>
											 x.GetByRecordGuid(Security, ENTITY_TYPE.USER, Guids.UserAdminGuid, siteToSiteMap.ChildSiteGuid)
										);
							
							if ((userToSiteMap == null) || (userToSiteMap.IdentityGuid == Guid.Empty))
							{
								//assign admin user to site
								userToSiteMap = new EntityToSiteMapClass
								{
									SiteGuid = siteToSiteMap.ChildSiteGuid,
									AssignedFromSiteGuid = guid,
									IdentityGuid = Guids.UserAdminGuid,
									TypeID = ENTITY_TYPE.USER
								};
								FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(Security, userToSiteMap, typeof(IUsers).GUID));
								var usergroupToSiteMap = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
											 x =>
											 x.GetByRecordGuid(Security, ENTITY_TYPE.GROUP, Guids.GroupAdminGuid, siteToSiteMap.ChildSiteGuid)
										);
								if ((usergroupToSiteMap == null) || (usergroupToSiteMap.IdentityGuid == Guid.Empty))
								{
									//assign admin group to site
									usergroupToSiteMap = new EntityToSiteMapClass
									{
										SiteGuid = siteToSiteMap.ChildSiteGuid,
										AssignedFromSiteGuid = guid,
										IdentityGuid = Guids.GroupAdminGuid,
										TypeID = ENTITY_TYPE.GROUP
									};
									FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(Security, usergroupToSiteMap, typeof(IGroups).GUID));
								}
							}							
						}
					}
					catch (Exception except)
					{
						if (except.Message == "Change Document Numbers")
						{
							this.Page.ClientScript.RegisterStartupScript(
								this.GetType(),
								"ChangeConfirmation",
								"<script type='text/javascript'>\r\n" + "<!--\r\n" + "if(window.confirm(\""
								+ HttpUtility.JavaScriptStringEncode(
									this.GetTranslatedText("Transaction Tickets Numbers differ from the Database.")) + "\\r\\n"
								+ HttpUtility.JavaScriptStringEncode(
									this.GetTranslatedText("A transaction may have been started since beginning site edit.")) + "\\r\\n\\n"
								+ HttpUtility.JavaScriptStringEncode(
									this.GetTranslatedText("Click OK to apply the Transaction Tickets Numbers.")) + "\\r\\n"
								+ HttpUtility.JavaScriptStringEncode(
									this.GetTranslatedText("To avoid possible duplication of Ticket Numbers, press Cancel.")) + "\"))\r\n"
								+ "   document.getElementById('Yes').click();\r\n" + "else\r\n"
								+ "   document.getElementById('No').click();\r\n" + "\r\n-->\r\n</script>");

							return;
						}

						throw;
					}
				}

				else
				{
					var user = this.Session["User"] as UserClass;
					string id = (user == null) ? "" : user.ID;
					string password = (user == null) ? "" : user.Password;
					guid = FMChannelHelper.MakeCall<ISites, Guid>(
																	 x =>
																	 x.Add(this.Security, site, id, password)
																);

					// check if we are mdm and this is the second site being added
					bool isMultipleSite = true;
					int siteCount = FMChannelHelper.MakeCall<ISites, int>(x => x.GetSiteCount(Security));
					if (siteCount <= 2)
					{
						FMChannelHelper.MakeCall<IHardwareKey>(hardwareKeyChannel =>
															{
																isMultipleSite = hardwareKeyChannel.IsMultipleSiteKey();
															});
					}

					if (isMultipleSite == false)  // this should only happen if this is the first site being added
					{
						try
						{
							// if required map all of the entities to this site
							// this is done only when mdm is enabled and the user adds there first site after siteadmin
							// add this site as a site to siteadmin

							var Siteadmin = FMChannelHelper.MakeCall<ISites, SiteClass>(
											 x =>
											 x.GetUsingGuid(Security, Guids.SiteAdminGuid)
										);

							SiteToSiteMapClass AssignedSiteToSiteMap = new SiteToSiteMapClass();
							AssignedSiteToSiteMap.ParentSiteGuid = Siteadmin.IdentityGuid;
							AssignedSiteToSiteMap.ParentSiteID = Siteadmin.ID;
							AssignedSiteToSiteMap.ChildSiteGuid = guid;
							AssignedSiteToSiteMap.ChildSiteID = site.ID;
							Siteadmin.SiteToSiteMapCollection.Add(AssignedSiteToSiteMap);

							FMChannelHelper.MakeCall<ISites>(x => x.Modify(Security, DATA_TYPE.CONFIG, Siteadmin, false));

							string discoveryAssem =
					  FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						  x => x.GetKeyValueByKey(base.Security, ConfigurationSettingDOClass.Key_IDiscoveryAssemblies));


							if (string.IsNullOrEmpty(discoveryAssem) == false)
							{
								char[] separator = { ';' };
								string[] discoveryAssemList = discoveryAssem.Split(separator, StringSplitOptions.RemoveEmptyEntries);

								string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
								foreach (string assemblyName in discoveryAssemList)
								{
									Assembly DLL = null;
									if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
									{
										try
										{
											DLL = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
										}
										catch
										{
											try
											{
												DLL = Assembly.Load(assemblyName);
											}
											catch (Exception ex)
											{
												string message = "Assembly Load Error on Site Form. " + ex.Message;
												FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
											}
										}

										if (DLL != null)
											AssemblyDictionary.Add(assemblyName.ToLower(), DLL);
									}
									else
									{
										DLL = AssemblyDictionary.Get(assemblyName.ToLower());
									}

									if (DLL == null)
									{
										continue;
									}

									try
									{
										Type[] Types = DLL.GetTypes();

										foreach (Type Module in Types)
										{
											Type IEntityDiscoveryInterface = Module.GetInterface("IEntityDiscovery");

											if (IEntityDiscoveryInterface == null)
											{
												continue;
											}
											Object Engine = Activator.CreateInstance(Module);
											var Discovery = (IEntityDiscovery)Engine;

											if (Discovery == null)
											{
												continue;
											}

											if (!Discovery.EntityAssignable)
											{
												continue;
											}

											string entityTypeID = EntityToSiteMapClass.GetEntityTypeID(Discovery.EntityType);
											var NewItem = new ListItem(entityTypeID, Module + "/" + assemblyName);

											bool entitySupportsRecordVersioning = false;
											if ((Discovery.EntityType == ENTITY_TYPE.EQUIPMENT)
											|| (Discovery.EntityType == ENTITY_TYPE.PRODUCT)
											|| (Discovery.EntityType == ENTITY_TYPE.COMPANY)
											|| (Discovery.EntityType == ENTITY_TYPE.TRANSACTION_ALIAS)
											|| (Discovery.EntityType == ENTITY_TYPE.PERSONNEL))
											{
												entitySupportsRecordVersioning = true;
											}

											EntityToSiteMapCollectionClass EntityToSiteMapCollection = null;
											try
											{

												if (entitySupportsRecordVersioning)
												{
													EntityToSiteMapCollection = Discovery.EnumerateEntityMaps(
														this.Security, ENTITY_ASSIGNMENT_TYPE.UNDELEGATED);
												}
												else
												{
													EntityToSiteMapCollection = Discovery is IPointTemplateDiscovery pointTemplateDiscovery
														? pointTemplateDiscovery.EnumerateEntityMapsForSiteCreation(this.Security, ENTITY_ASSIGNMENT_TYPE.OWNED)
														: Discovery.EnumerateEntityMaps(this.Security, ENTITY_ASSIGNMENT_TYPE.OWNED);
												}
											}
											catch
											{
												// some of these entities will return access denied so we just need to skip them
												EntityToSiteMapCollection = null;
											}

											if (EntityToSiteMapCollection == null)
											{
												continue;
											}

											foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
											{
												// anything that exists needs to be moved to this site. This mainly required for iventory management stuff because they can not be edited
												// unless the site owns them. It is also necessary for the users and user groups
												// Preclude change of Ownership for SiteAdmin Administrator
												if (Module == typeof(UsersForm))
												{
													// we just need to assign this entry to the created site not take ownership

													Guid entityEngineTypeGuid = typeof(UsersForm).GUID;

													EntityToSiteMap.SiteID = site.ID;
													EntityToSiteMap.SiteGuid = guid;

													var entityToSiteMap = new EntityToSiteMapClass
													{
														TypeID = EntityToSiteMap.TypeID,
														ID = EntityToSiteMap.ID,
														IdentityGuid = EntityToSiteMap.IdentityGuid,
														SiteID = EntityToSiteMap.SiteID,
														SiteGuid = EntityToSiteMap.SiteGuid,
														AssignedFromSiteId = EntityToSiteMap.AssignedFromSiteId,
														AssignedFromSiteGuid = EntityToSiteMap.AssignedFromSiteGuid,
														IsAssigned = true
													};
													List<EntityToSiteMapClass> entitylist = new List<EntityToSiteMapClass>();

													entitylist.Add(entityToSiteMap);

													FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.AddList(Security, entitylist, entityEngineTypeGuid));

													continue;
												}

												// Preclude change of Ownership for SiteAdmin Administrators
												if (Module == typeof(GroupsForm))// && EntityToSiteMap.IdentityGuid == Guids.SiteAdminGuid)
												{
													// we just need to assign this entry to the created site not take ownership

													Guid entityEngineTypeGuid = typeof(GroupsForm).GUID;

													EntityToSiteMap.SiteID = site.ID;
													EntityToSiteMap.SiteGuid = guid;

													var entityToSiteMap = new EntityToSiteMapClass
													{
														TypeID = EntityToSiteMap.TypeID,
														ID = EntityToSiteMap.ID,
														IdentityGuid = EntityToSiteMap.IdentityGuid,
														SiteID = EntityToSiteMap.SiteID,
														SiteGuid = EntityToSiteMap.SiteGuid,
														AssignedFromSiteId = EntityToSiteMap.AssignedFromSiteId,
														AssignedFromSiteGuid = EntityToSiteMap.AssignedFromSiteGuid,
														IsAssigned = true
													};
													List<EntityToSiteMapClass> entitylist = new List<EntityToSiteMapClass>();

													entitylist.Add(entityToSiteMap);

													FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.AddList(Security, entitylist, entityEngineTypeGuid));

													continue;
												}

												var currentSiteGuid = this.Security.SiteGuid;
												this.Security.SiteGuid = guid;

												Guid entityGuid = EntityToSiteMap.IdentityGuid;

												try
												{
													Discovery.SetSiteGuid(this.Security, entityGuid, guid);
													this.Security.SiteGuid = currentSiteGuid;
												}
												catch (Exception except)
												{
													this.ErrorHandler(except);
													this.Security.SiteGuid = currentSiteGuid;
													return;
												}
											}
										}
									}
									catch { } // Try: Type[] Types = DLL.GetTypes()
								}
							}
						}   // end remap entities
						catch (Exception except)
						{
							this.ErrorHandler(except);
						}
					}
					else
					{
						//assign admin user to site
						var userToSite = new EntityToSiteMapClass
						{
							SiteGuid = guid,
							AssignedFromSiteGuid = Guids.SiteAdminGuid,
							IdentityGuid = Guids.UserAdminGuid,
							TypeID = ENTITY_TYPE.USER
						};

						FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(Security, userToSite, typeof(IUsers).GUID));

						//assign admin group to site
						var groupToSite = new EntityToSiteMapClass
						{
							SiteGuid = guid,
							AssignedFromSiteGuid = Guids.SiteAdminGuid,
							IdentityGuid = Guids.GroupAdminGuid,
							TypeID = ENTITY_TYPE.GROUP
						};

						FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(Security, groupToSite, typeof(IGroups).GUID));



						// Assign down standard point types
						FMChannelHelper.MakeCall<IEntityToSiteMaps>(x =>
						{
							foreach (var type in Guids.StandardPointTypes)
							{
								var pointTypeToSite = new EntityToSiteMapClass
								{
									SiteGuid = guid,
									AssignedFromSiteGuid = Guids.SiteAdminGuid,
									IdentityGuid = type,
									TypeID = ENTITY_TYPE.POINT_TEMPLATE_TYPE
								};

								x.Add(Security, pointTypeToSite, typeof(IApplicationStrings).GUID);
							}
						});

						// Assign down standard point templates
						FMChannelHelper.MakeCall<IEntityToSiteMaps>(x =>
						{
							foreach (var type in Guids.StandardPointTemplatesGuids)
							{
								var pointTemplatesToSite = new EntityToSiteMapClass
								{
									SiteGuid = guid,
									AssignedFromSiteGuid = Guids.SiteAdminGuid,
									IdentityGuid = type,
									TypeID = ENTITY_TYPE.POINT_TEMPLATE
								};

								x.Add(Security, pointTemplatesToSite, typeof(IPointTemplates).GUID);
							}
						});

						// Assign down standard Alarm Priorities
						FMChannelHelper.MakeCall<IEntityToSiteMaps>(x =>
						{
							foreach (var type in Guids.StandardAlarmPriorityGuids)
							{
								var alarmPriorityToSite = new EntityToSiteMapClass
								{
									SiteGuid = guid,
									AssignedFromSiteGuid = Guids.SiteAdminGuid,
									IdentityGuid = type,
									TypeID = ENTITY_TYPE.ALARM_PRIORITY
								};

								x.Add(Security, alarmPriorityToSite, typeof(IAlarmPriorities).GUID);
							}
						});

						// Assign down standard Alarm Category
						var alarmCategoryToSite = new EntityToSiteMapClass
						{
							SiteGuid = guid,
							AssignedFromSiteGuid = Guids.SiteAdminGuid,
							IdentityGuid = Guids.AlarmGroupCategoryApplicationStringGuid,
							TypeID = ENTITY_TYPE.ALARM_EVENT_CATEGORY
						};

						FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(Security, alarmCategoryToSite, typeof(IApplicationStrings).GUID));

						// Assign down License expiration notification email group
						FMChannelHelper.MakeCall<IEntityToSiteMaps>(x =>
						{
							foreach (var type in Guids.StandardEmailGroupGuids)
							{
								var emailGroupToSite = new EntityToSiteMapClass
								{
									SiteGuid = guid,
									AssignedFromSiteGuid = Guids.SiteAdminGuid,
									IdentityGuid = type,
									TypeID = ENTITY_TYPE.EMAIL_GROUP
								};

								x.Add(Security, emailGroupToSite, typeof(IAlarmPriorities).GUID);
							}
						});
					}
				}

				try
				{
					if (UsingLoadRack)
					{
						ILoadRackManager loadRackManager = this.GetLoadRackManager();
						if (!site.IdentityGuid.IsEmpty())
						{
							loadRackManager.Modify(this.Security, typeof(SiteClass), site.IdentityGuid);
						}
						else
						{
							loadRackManager.Add(this.Security, typeof(SiteClass), guid);
						}
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw;
					}
				}

				this.Session.Remove("Site");
				this.Session.Remove("User");
				this.Session.Remove("PendingOK");
				this.Session.Remove("LedgerGrossNetSelection");

				if (site.IdentityGuid == this.Security.SiteGuid)
				{
					this.Security.SiteID = site.ID;
				}

				if (site.IdentityGuid == this.Security.LoginSiteGuid)
				{
					this.Security.LoginSiteID = site.ID;
				}

				// Always refresh menu data, because Help URL may have changed
				this.ucFMMenuBar.Refresh();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			// if the user has changed the holiday schedule we need to check all of the configured equipment qc due date and recalculate
			if (this.Session["SiteHolidayScheduleModified"] != null && (bool)this.Session["SiteHolidayScheduleModified"])
			{
				var equipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
					 x =>
						  x.EnumerateManagedEquipment(this.Security)
					 );

				foreach (EquipmentClass equipment in equipmentCollection)
				{
					// check the qc due date
					DateTimeOffset dateNow = TimeConverter.Today().AddMilliseconds(-1);
					DateTimeOffset qcdate = this.GetNextQCDateForAsset(
						 this.Security, equipment.MasterRecordGuid, this.GetTranslatedText("Equipment"), dateNow);
					if (qcdate >= dateNow)
					{
						bool isQcDateEditable = this.IsFieldRecordVersionSpecific(this.Security, ENTITY_TYPE.EQUIPMENT.ToString(),
							equipment.IdentityGuid, equipment.MasterRecordGuid, equipment.SiteGuid, "QCDate");
						if (isQcDateEditable)
						{
							equipment._QCDate.Value = qcdate;
							this.ModifyEquipment(this.Security, equipment);
						}
					}
				}

				this.Session.Remove("SiteHolidayScheduleModified");
			}

			this.Redirect("SitesForm.aspx");
		}

		private bool IsFieldRecordVersionSpecific(SecurityClass security, string equipment, Guid equipmentIdentityGuid,
				Guid masterRecordGuid, Guid siteGuid, string param2)
		{
			return FMChannelHelper.MakeCall<IFieldLevelConfigMaps, bool>(
					x =>
					x.IsFieldRecordVersionSpecific(security, equipment, equipmentIdentityGuid,
						masterRecordGuid, siteGuid, param2)
			);
		}

		private DateTimeOffset GetNextQCDateForAsset(SecurityClass securityClass, Guid guid, string p, DateTimeOffset dateTimeOffset)
		{
			return
				FMChannelHelper.MakeCall<IAppointments, DateTimeOffset>(
					x => x.GetNextQCDateForAsset(securityClass, guid, p, dateTimeOffset));
		}

		private void ModifyEquipment(SecurityClass securityClass, EquipmentClass equipment)
		{
			FMChannelHelper.MakeCall<IEquipments>(
																	 x =>
																	 x.Modify(securityClass, equipment)
																);
		}

		#endregion
	}

	public class SiteFormBase : FMFormBase
	{
		#region Public Methods and Operators

		public void CheckPendingOk(bool main)
		{
		}

		#endregion
	}

}