// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityOwnershipForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityOwnershipForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for EntityOwnershipForm.
	/// </summary>
	public partial class EntityOwnershipForm : FMFormBase, IMenuDiscovery
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
				if (((word1 & 0x01) != 0x01) && security.LoginSiteGuid != Guids.SiteAdminGuid)// master data management)
					return null;
         }
         else
         {
            // Depends Upon Multi Site
            if ((options & 0x800000) == 0)
            {
               return null;
            }

            // Depends Upon Shared Components Config
            if ((options & 0x4000) == 0)
            {
               return null;
            }
         }

         if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ADMIN_SITES_ENTITY_OWNERSHIP,
					RootMenuName = "Administration",
					CategoryName = "Sites",
					ItemName = "Entity Ownership",
					NavigateUrl = "EntityOwnershipForm.aspx",
					SortOrder = 3,
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

		protected void EntityTypeListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.EntityListBox.Items.Clear();

				if (this.EntityTypeListBox.SelectedIndex == -1)
				{
					return;
				}

				char[] Seperators = { '/' };
				string[] Strings = this.EntityTypeListBox.SelectedValue.Split(Seperators, 2);

				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				Assembly DLL = null;
				if (!AssemblyDictionary.ContainsKey(Strings[1].ToLower()))
				{
					try
					{
						DLL = Assembly.LoadFrom(baseDirectory + "\\bin\\" + Strings[1]);
					}
					catch
					{
						try
						{
							DLL = Assembly.Load(Strings[1]);
						}
						catch (Exception ex)
						{
							string message = "Assembly Load Error on Entity Ownership Select Entity Type. " + ex.Message;
							FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
						}
					}

					if (DLL != null)
						AssemblyDictionary.Add(Strings[1].ToLower(), DLL);
				}
				else
				{
					DLL = AssemblyDictionary.Get(Strings[1].ToLower());
				}

				Type Module = DLL.GetType(Strings[0]);
				Object Engine = Activator.CreateInstance(Module);
				var Discovery = (IEntityDiscovery)Engine;

                bool entitySupportsRecordVersioning = false;
					 if ((Discovery.EntityType == ENTITY_TYPE.EQUIPMENT) || (Discovery.EntityType == ENTITY_TYPE.PRODUCT) || (Discovery.EntityType == ENTITY_TYPE.COMPANY) || (Discovery.EntityType == ENTITY_TYPE.TRANSACTION_ALIAS) || (Discovery.EntityType == ENTITY_TYPE.PERSONNEL))
                    entitySupportsRecordVersioning = true;

                EntityToSiteMapCollectionClass AssignedEntityToSiteMapCollection;
                if (entitySupportsRecordVersioning)
                {
                    AssignedEntityToSiteMapCollection = Discovery.EnumerateEntityMaps(
                        this.Security, ENTITY_ASSIGNMENT_TYPE.UNDELEGATED);
                }
                else
                {
                    AssignedEntityToSiteMapCollection = Discovery.EnumerateEntityMaps(
                        this.Security, ENTITY_ASSIGNMENT_TYPE.OWNED);
                }

				if (AssignedEntityToSiteMapCollection == null)
				{
					return;
				}

				foreach (EntityToSiteMapClass AssignedEntityToSiteMap in AssignedEntityToSiteMapCollection)
				{
					// Preclude change of Ownership for SiteAdmin Administrator
					if (Module == typeof(UsersForm) && AssignedEntityToSiteMap.IdentityGuid == Guids.SiteAdminGuid)
					{
						continue;
					}

					// Preclude change of Ownership for SiteAdmin Administrators
					if (Module == typeof(GroupsForm) && AssignedEntityToSiteMap.IdentityGuid == Guids.SiteAdminGuid)
					{
						continue;
					}

					var NewItem = new ListItem(AssignedEntityToSiteMap.ID, AssignedEntityToSiteMap.IdentityGuid.ToString());

					foreach (ListItem ExistingItem in this.EntityListBox.Items)
					{
						if (ExistingItem.Text.CompareTo(NewItem.Text) > 0)
						{
							int Index = this.EntityListBox.Items.IndexOf(ExistingItem);
							this.EntityListBox.Items.Insert(Index, NewItem);
							NewItem = null;
							break;
						}
					}

					if (NewItem != null)
					{
						this.EntityListBox.Items.Add(NewItem);
					}
				}
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

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.AssignButton.Enabled = false;
					}

					// Populate Sites
					SiteClass Site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
					SiteCollectionClass SiteCollection;
					SiteCollectionClass SiteChildCollection = null;
					if (Site.SiteGroup)
					{
						SiteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.EnumerateByParentSite(this.Security, this.Security.SiteGuid)
																);
						SiteChildCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.EnumerateByChildSite(this.Security, this.Security.SiteGuid)
																);
					}
					else
					{
						SiteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.EnumerateByChildSite(this.Security, this.Security.SiteGuid)
																);
					}

					foreach (SiteClass MappedSite in SiteCollection)
					{
						var NewItem = new ListItem(MappedSite.ID, MappedSite.SiteGuid.ToString());

						foreach (ListItem ExistingItem in this.SiteDropDownList.Items)
						{
							if (ExistingItem.Text.CompareTo(NewItem.Text) > 0)
							{
								int Index = this.SiteDropDownList.Items.IndexOf(ExistingItem);
								this.SiteDropDownList.Items.Insert(Index, NewItem);
								NewItem = null;
								break;
							}
						}

						if (NewItem != null)
						{
							this.SiteDropDownList.Items.Add(NewItem);
						}
					}

					if (Site.SiteGroup)
					{
						foreach (SiteClass MappedSite in SiteChildCollection)
						{
							var NewItem = new ListItem(MappedSite.ID, MappedSite.SiteGuid.ToString());

							foreach (ListItem ExistingItem in this.SiteDropDownList.Items)
							{
								if (ExistingItem.Text.CompareTo(NewItem.Text) > 0)
								{
									int Index = this.SiteDropDownList.Items.IndexOf(ExistingItem);
									this.SiteDropDownList.Items.Insert(Index, NewItem);
									NewItem = null;
									break;
								}
							}

							if (NewItem != null)
							{
								this.SiteDropDownList.Items.Add(NewItem);
							}
						}
					}

					// Populate EntityTypeListBox
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
										string message = "Assembly Load Error on Entity Ownership Form. " + ex.Message;
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
								continue;

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

									this.EntityTypeListBox.Items.Add(NewItem);
								}
							}
							catch { }
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AssignButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.SiteDropDownList.SelectedIndex == -1)
				{
					return;
				}

				Guid SiteGuid = Guid.Parse(this.SiteDropDownList.SelectedValue);

				if (this.EntityTypeListBox.SelectedIndex == -1)
				{
					return;
				}

				char[] Seperators = { '/' };
				string[] Strings = this.EntityTypeListBox.SelectedValue.Split(Seperators, 2);

				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				Assembly DLL = null;
				if (!AssemblyDictionary.ContainsKey(Strings[1].ToLower()))
				{
					try
					{
						DLL = Assembly.LoadFrom(baseDirectory + "\\bin\\" + Strings[1]);
					}
					catch
					{
						try
						{
							DLL = Assembly.Load(Strings[1]);
						}
						catch (Exception ex)
						{
							string message = "Assembly Load Error on Entity Ownership Assign. " + ex.Message;
							FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
						}
					}

					if (DLL != null)
						AssemblyDictionary.Add(Strings[1].ToLower(), DLL);
				}
				else
				{
					DLL = AssemblyDictionary.Get(Strings[1].ToLower());
				}

				Type Module = DLL.GetType(Strings[0]);
				Object Engine = Activator.CreateInstance(Module);
				var Discovery = (IEntityDiscovery)Engine;

				ListItem EntityItem;

				while ((EntityItem = this.EntityListBox.SelectedItem) != null)
				{
					Guid guid = Guid.Parse(EntityItem.Value);
					EquipmentClass targetEquipment = null;
					if (Discovery.EntityType == ENTITY_TYPE.EQUIPMENT)
					{
						//Capture the equipment and its collection before the Site Ownership change, to facilitate the reciprocal assignment for the compartments further down.
						targetEquipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(equipments => equipments.Get(this.Security, guid));
					}

						Discovery.SetSiteGuid(this.Security, guid, SiteGuid);

					// If we are moving ownership to a group site above the current site,
					// add a reciprocal entity assignment
					SiteCollectionClass siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.EnumerateByChildSite(this.Security, this.Security.SiteGuid)
																);

					foreach (SiteClass site in siteCollection)
					{
						if (site.IdentityGuid != this.Security.SiteGuid && site.IdentityGuid == SiteGuid)
						{
							Dictionary<string, Guid> lstTargetEntities = new Dictionary<string, Guid>();
							lstTargetEntities.Add(EntityItem.Text, guid);
							//If the target entity is an Equipment, extend the reciprocal entity assignment to its Compartments as well.
							if (Discovery.EntityType == ENTITY_TYPE.EQUIPMENT)
							{
								if ((targetEquipment != null) && (targetEquipment.CompartmentCollection != null))
								{
									for (int i = 0; i < targetEquipment.CompartmentCollection.Count; i++)
										lstTargetEntities.Add(targetEquipment.CompartmentCollection[i].ID, targetEquipment.CompartmentCollection[i].IdentityGuid);
								}
							}
							//Add the reciprocal assignment mapping
							foreach (KeyValuePair<string, Guid> kvp in lstTargetEntities)
							{
								var map = new EntityToSiteMapClass();
								map.ID = kvp.Key;
								map.IdentityGuid = kvp.Value;
								map.SiteGuid = this.Security.SiteGuid;
								map.TypeID = Discovery.EntityType;
								map.AssignedFromSiteGuid = SiteGuid;

								FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(this.Security, map, Discovery.EntityEngineType.GUID));
							}
							break;
						}
					}

					this.EntityListBox.Items.Remove(EntityItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignButton.Command += this.AssignButton_Command;
		}

		#endregion
	}
}