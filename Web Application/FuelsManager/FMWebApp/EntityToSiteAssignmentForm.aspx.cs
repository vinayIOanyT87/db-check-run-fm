// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityToSiteAssignmentForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for Entity Assignment page
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

    using FMCore;

	using global::FMWebApp;

	/// <summary>
	/// Code behind for Entity to Sites assignement page.
	/// </summary>
	public partial class EntityToSiteAssignmentForm : FMFormBase, IMenuDiscovery
	{
		private const string AllStr = "{All}";

		private const string NoneStr = "{None}";

		/// <summary>
		/// Check to see if any record versioning aware entities have been unassigned from sites.
		/// In the case of Record Versioning, unassigning an entity is more than just a mapping deletion, but can also be
		/// be accompanied with child record version deletions, i.e. the deletion of user entered data.
		/// By checking the changes made by the user, we are able to present the user with a warning message 
		/// and a confirmation prompt at the time the user hits the Apply button.
		/// </summary>
		private bool HasRecordVersioningEntityUnassignments
		{
			get
			{
				var selectedEntityType = ENTITY_TYPE.UNKNOWN;
				if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] != null)
				{
					if (Enum.TryParse(
						(string)this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT],
						out selectedEntityType) == false)
					{
						selectedEntityType = ENTITY_TYPE.UNKNOWN;
					}
				}

				bool entitySupportsRecordVersioning = this.EntitySupportsRecordVersioning(selectedEntityType);

				if (!entitySupportsRecordVersioning)
				{
					return false;
				}

				var dataItems = (List<EntityToSiteMapClass>) this.Session["EntityAssignmentDataSource"];

				foreach (GridViewRow dataGridItem in this.Grid.Rows)
				{
					var dataItem = dataItems[dataGridItem.DataItemIndex];

					var assignedCheckBox = dataGridItem.FindControl("ACB") as CheckBox;

					// If the assigned check box is not checked but the original check box was checked, an entity has been unassigned
					if (assignedCheckBox != null && dataItem.IsAssigned && !assignedCheckBox.Checked)
					{
						return true;
					}
				}

				return false;
			}
		}

		#region Public Methods and Operators

		/// <summary>
		/// Enumerates the by criterion.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="currentSiteGuid">The current site GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <param name="includeMemberSites">if set to <c>true</c> [include member sites].</param>
		/// <param name="entityGuid">Identifies the entity to display assignments for.</param>
		/// <param name="entityId">The entity ID.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="sortKey">The sort key.</param>	
		/// <returns>A list of site maps.</returns>
		public List<EntityToSiteMapClass> EnumerateByCriterion(
			SecurityClass security, 
			Guid? currentSiteGuid, 
			Guid? siteGuid, 
			bool includeMemberSites,
            Guid entityGuid,
            string entityId,
			ENTITY_TYPE entityType, 
			string sortKey)
		{
			var entities = new List<EntityToSiteMapClass>();
			var fromSiteGuid = security.SiteGuid;
			var loginSiteGuid = security.LoginSiteGuid;

			try
			{
				// Populate EntityTypeListBox
				string discoveryAssem =
					FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						configSettings => configSettings.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_IDiscoveryAssemblies));

				if (string.IsNullOrEmpty(discoveryAssem) == false)
				{
					char[] separator = { ';' };
					string[] discoveryAssemList = discoveryAssem.Split(separator, StringSplitOptions.RemoveEmptyEntries);

					security.LoginSiteGuid = currentSiteGuid == null ? fromSiteGuid : currentSiteGuid.Value;
					security.SiteGuid = siteGuid == null ? fromSiteGuid : siteGuid.Value;

					List<KeyValuePair<Guid, string>> sites =
						FMChannelHelper.MakeCall<IEntityToSiteMaps, List<KeyValuePair<Guid, string>>>(
							entityToSiteMaps => entityToSiteMaps.EnumerateEntitySites(security, security.SiteGuid, includeMemberSites));

                    bool allEntities = (entityGuid == Guid.Empty);
					bool done = false;

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
									string message = "Assembly Load Error on Entity To Site Assignment Form. " + ex.Message;
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
						{
							continue;
						}

						try
						{
							Type[] types = dll.GetTypes();
							var assignedEntityGuids = new Hashtable();

							foreach (Type module in types)
							{
								Type entityDiscoveryInterface = module.GetInterface("IEntityDiscovery");

								if (entityDiscoveryInterface == null)
								{
									continue;
								}

								object engine = Activator.CreateInstance(module);
								var discovery = (IEntityDiscovery)engine;

								if (discovery.EntityAssignable == false)
								{
									continue;
								}

								if (discovery.EntityType != entityType)
								{
									continue;
								}

								security.SiteGuid = fromSiteGuid;

								EntityToSiteMapCollectionClass unassignedEntityToSiteMapCollection;

								if (allEntities)
								{
									unassignedEntityToSiteMapCollection = this.GetEntitiesAvailableForAssignment(security, entityType, discovery);
								}
								else
								{
									unassignedEntityToSiteMapCollection = new EntityToSiteMapCollectionClass();
									var unassignedEntityToSiteMap = new EntityToSiteMapClass
									{
										TypeID = entityType,
										ID = entityId,
										IdentityGuid = entityGuid,
										IsAssigned = true,
										SiteGuid = fromSiteGuid,
										SiteID = security.SiteID,
									};

									if (entityType == ENTITY_TYPE.USER && this.GetActiveDirectoryUserFlag(security, entityGuid))
									{
										unassignedEntityToSiteMap.DisableSelection = true;
									}

									unassignedEntityToSiteMapCollection.Add(unassignedEntityToSiteMap);
								}

								foreach (KeyValuePair<Guid, string> site in sites)
								{
									if (site.Key == fromSiteGuid)
									{
										continue;
									}

									if (siteGuid != null && !includeMemberSites && site.Key != siteGuid)
									{
										continue;
									}

									security.SiteGuid = site.Key;
									assignedEntityGuids.Clear();

									EntityToSiteMapCollectionClass mappings = null;

									// Depending on the filter configuration it may be performance savvy to get all mappings at once
									// or do check one at a time.  If there are more than one entity to check, get all the mappings so we
									// can search more efficiently.  Otherwise, we will do a check for the entity individually in the loop
									// that comes right after this.
									if (unassignedEntityToSiteMapCollection.Count > 1)
									{
										mappings =
											FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
												x => x.EnumerateByTypeIDAndSiteGuid(security, entityType, security.SiteGuid));
									}

									foreach (EntityToSiteMapClass unassignedEntityToSiteMap in unassignedEntityToSiteMapCollection)
									{
										EntityToSiteMapClass assignedEntityToSiteMap;

										if (mappings != null)
										{
											assignedEntityToSiteMap = this.GetMappingByRecordGuid(entityType, mappings, unassignedEntityToSiteMap.IdentityGuid);
										}
										else
										{
											assignedEntityToSiteMap = this.GetMappingByRecordGuid(security, entityType, unassignedEntityToSiteMap.IdentityGuid, site.Key);
										}

										if ((assignedEntityToSiteMap != null) && (assignedEntityToSiteMap.IdentityGuid != Guid.Empty))
										{
											assignedEntityToSiteMap.SiteGuid = site.Key;
											assignedEntityToSiteMap.SiteID = site.Value;
											assignedEntityToSiteMap.IsAssigned = true;
											entities.Add(assignedEntityToSiteMap);

											if (entityType == ENTITY_TYPE.USER && this.GetActiveDirectoryUserFlag(security, unassignedEntityToSiteMap.IdentityGuid))
											{
												assignedEntityToSiteMap.DisableSelection = true;
											}

											assignedEntityGuids.Add(assignedEntityToSiteMap.IdentityGuid, assignedEntityToSiteMap);
										}
									}

									foreach (EntityToSiteMapClass unassignedEntityToSiteMap in unassignedEntityToSiteMapCollection)
									{
										if (!allEntities && entityId != unassignedEntityToSiteMap.ID)
										{
											continue;
										}

										// Compare the unassigned entity to assigned entities.  If the unassigned entity matches the
										// assigned entity then do not add it to the unassigned entities box.
										if (assignedEntityGuids.Contains(unassignedEntityToSiteMap.IdentityGuid))
										{
											continue;
										}

										var e = new EntityToSiteMapClass
										{
											TypeID = discovery.EntityType,
											ID = unassignedEntityToSiteMap.ID,
											IdentityGuid = unassignedEntityToSiteMap.IdentityGuid,
											IsAssigned = false,
											SiteID = site.Value,
											SiteGuid = site.Key
										};

										if (entityType == ENTITY_TYPE.USER && this.GetActiveDirectoryUserFlag(security, unassignedEntityToSiteMap.IdentityGuid))
										{
											e.DisableSelection = true;
										}

										entities.Add(e);
									}
								}

								if (discovery.EntityType == entityType)
								{
									done = true;
									break;
								}
							}
						}
						catch { } // Try: Type[] types = dll.GetTypes()

						if (done)
						{
							break;
						}
					}
				}

				int sortDir = sortKey.IndexOf(" DESC", StringComparison.Ordinal) > 0 ? -1 : 1;

				if (sortKey.ToUpper().IndexOf("ENTITY ", StringComparison.Ordinal) == 0)
				{
					entities.Sort(
						delegate(EntityToSiteMapClass p1, EntityToSiteMapClass p2)
						{
							var c = string.Compare( p1.ID, p2.ID, StringComparison.InvariantCultureIgnoreCase );
							if ( c == 0 )
							{
								if ( p1.TypeID.CompareTo( p2.TypeID ) == 0 )
								{
									return string.Compare( p1.SiteID, p2.SiteID, StringComparison.InvariantCultureIgnoreCase );
								}

								return p1.TypeID.CompareTo( p2.TypeID );
							}

							return c * sortDir;
						});
				}
				else if (sortKey.ToUpper().IndexOf("SITE ", StringComparison.Ordinal) == 0)
				{
					entities.Sort(
						delegate(EntityToSiteMapClass p1, EntityToSiteMapClass p2)
						{
							var c = string.Compare( p1.SiteID, p2.SiteID, StringComparison.InvariantCultureIgnoreCase );
							if ( c == 0 )
							{
								if ( p1.TypeID.CompareTo( p2.TypeID ) == 0 )
								{
									return string.Compare( p1.ID, p2.ID, StringComparison.InvariantCultureIgnoreCase );
								}

								return p1.TypeID.CompareTo( p2.TypeID );
							}

							return c * sortDir;
						});
				}
				else if (sortKey.ToUpper().IndexOf("ASSIGNEDFROMSITE ", StringComparison.Ordinal) == 0)
				{
					entities.Sort(
						delegate(EntityToSiteMapClass p1, EntityToSiteMapClass p2)
						{
							var value1 = p1.AssignedFromSiteId ?? string.Empty;
							var value2 = p2.AssignedFromSiteId ?? string.Empty;

							if (value1 == string.Empty && value2 != string.Empty)
							{
								return 1 * sortDir;
							}

							if (value1 != string.Empty && value2 == string.Empty)
							{
								return -1 * sortDir;
							}

							var c = string.Compare(
								p1.AssignedFromSiteId ?? string.Empty,
								p2.AssignedFromSiteId ?? string.Empty,
								StringComparison.InvariantCultureIgnoreCase);

							if (c == 0)
							{
								return string.Compare(p1.ID, p2.ID, StringComparison.InvariantCultureIgnoreCase);
							}

							return c * sortDir;
						});
				}
				else
				{
					entities.Sort(
						delegate(EntityToSiteMapClass p1, EntityToSiteMapClass p2)
						{
							var c = p1.TypeID.CompareTo( p2.TypeID );
							if ( c == 0 )
							{
								if ( string.Compare( p1.ID, p2.ID, StringComparison.InvariantCultureIgnoreCase ) == 0 )
								{
									return string.Compare( p1.SiteID, p2.SiteID, StringComparison.InvariantCultureIgnoreCase );
								}

								return string.Compare( p1.ID, p2.ID, StringComparison.InvariantCultureIgnoreCase );
							}

							return c * sortDir;
						});
				}
			}
			finally
			{
				security.LoginSiteGuid = loginSiteGuid;
				security.SiteGuid = fromSiteGuid;
			}

			return entities;
		}

		public EntityToSiteMapCollectionClass GetEntitiesAvailableForAssignment(SecurityClass security, ENTITY_TYPE entityType, IEntityDiscovery discovery)
        {
		    EntityToSiteMapCollectionClass entityToSiteMapCollection = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
                                                                                entityToSiteMaps => entityToSiteMaps.EnumerateEntityMapsBySiteGuid(
                                                                                security, entityType, security.SiteGuid, true));
            if (!entityType.IsEntityTypeSupportsIndividualEntityMapping())
            {
                EntityToSiteMapClass entityToSiteMap;

                if ((entityToSiteMapCollection == null) || (entityToSiteMapCollection.Count == 0))
                {
                    entityToSiteMap = new EntityToSiteMapClass
                    {
                        SiteGuid = Guid.Empty,
                        TypeID = entityType,
                        IdentityGuid = security.SiteGuid
                    };

                    entityToSiteMapCollection = new EntityToSiteMapCollectionClass { entityToSiteMap };
                }

                //For Entity Types that are mapped as a whole, the EntitytoSiteMapCollection would always have a single entry, which will correspond to the "All" grouping.
                entityToSiteMap = entityToSiteMapCollection[0];

                //Reset the Id value. Entity Types that are mapped as a whole use pre-defined Id labels to indicate the mapping of a whole collection of entities.
                entityToSiteMap.ID = this.GetDefaultEntityIdForEntityType(entityType);
                entityToSiteMapCollection[0] = entityToSiteMap;
            }
            return entityToSiteMapCollection;
        }


        /// <summary>
        /// Returns the display-formatted version of the EntityToSiteMap of a given entity record to a given site
        /// </summary>
        /// <param name="security">The security object</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="identityGuid">The guid of the entity record. For entity records under record versioning, this should be the MasterRecordGuid.</param>
        /// <param name="assignedToSiteGuid">The target AssignedTo site for which the mapping is to be retrieved.</param>
        /// <returns></returns>
        public EntityToSiteMapClass GetMappingByRecordGuid(SecurityClass security, ENTITY_TYPE entityType, Guid identityGuid, Guid assignedToSiteGuid)
        {
            EntityToSiteMapClass entityToSiteMap = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
                                                                        entityToSiteMaps => entityToSiteMaps.GetByRecordGuid(
                                                                        security, entityType, identityGuid, assignedToSiteGuid));
            if (!entityType.IsEntityTypeSupportsIndividualEntityMapping())
            {
                //Reset the Id value. Entity Types that are mapped as a whole use pre-defined Id labels to indicate the mapping of a whole collection of entities.
                if (entityToSiteMap != null)
                    entityToSiteMap.ID = this.GetDefaultEntityIdForEntityType(entityType);
            }
            return entityToSiteMap;
        }

		public EntityToSiteMapClass GetMappingByRecordGuid( ENTITY_TYPE entityType, EntityToSiteMapCollectionClass mappings, Guid identityGuid )
		{
			EntityToSiteMapClass entityToSiteMap = mappings.Find(x => x.IdentityGuid == identityGuid);

			if ( !entityType.IsEntityTypeSupportsIndividualEntityMapping() )
			{
				//Reset the Id value. Entity Types that are mapped as a whole use pre-defined Id labels to indicate the mapping of a whole collection of entities.
				if (entityToSiteMap != null)
				{
					entityToSiteMap.ID = this.GetDefaultEntityIdForEntityType( entityType );
				}
			}

			return entityToSiteMap;
		}

		/// <summary>
        /// This operation is used to return the default Id to be used for entity types that are mapped as a whole, and not individually, e.g. Alarm & Events.
        /// For those entity types, instead of mapping individual entity records of the entity types, the Owner Sitegroup is mapped to indicate that all the entities of the selected
        /// entity type owned by the given Owner Sitegroup are being mapped.
        /// The Entity Record Guid used in the mapping corresponds to the Owner Site Guid. The Id displayed for the record being mapped is a made-up Id that helps indicate that the
        /// whole set of entities are being mapped.
        /// This operation provides that display Id label that is used to represent the collection of entities for each of the entity types that are mapped as a whole.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private string GetDefaultEntityIdForEntityType(ENTITY_TYPE entityType)
        {
            string result = null;
            switch (entityType)
            {
                case ENTITY_TYPE.ALARM_AND_EVENT:
                    { result = "All Alarm & Events"; break; }
                case ENTITY_TYPE.DATA_DICTIONARY:
                    { result = "All Key/Value Pairs"; break; }
                case ENTITY_TYPE.QUERY:
                case ENTITY_TYPE.QUERY_DEFAULT:
                case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
                    { result = "Default Fields and Settings"; break; }
                case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
                    { result = "Default Reports and Settings"; break; }
                case ENTITY_TYPE.USER_DATA_FIELD:
                    { result = "All User Data Configuration"; break; }
					 case ENTITY_TYPE.EXTERNAL_STATION_DEVICE:
						  {result = "All Payment Cards"; break;}  //Changed from All Gasboy Devices to All Payment Cards since we are only using the Gasboy devices for payment cards at this point.
            }
            return result;
        }


		/// <summary>
		/// Enumerates the entities.
        /// Note: EntityTypes that are mapped as a whole, e.g. Alarm & Events, do not support local definitions of the entity when the entity has been assigned down 
        /// to the site/sitegroup from a higher level sitegroup. This means that the Entity Listing is not applicable to Entity Types that are not mapped individually, and
        /// this operation therefore returns an emtpy list for those entity types.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <returns>A list of entity maps.</returns>
		public List<EntityToSiteMapClass> EnumerateEntities(SecurityClass security, ENTITY_TYPE entityType)
		{
            List<EntityToSiteMapClass> entities = new List<EntityToSiteMapClass>();
            
            if ((entityType != ENTITY_TYPE.UNKNOWN) && entityType.IsEntityTypeSupportsIndividualEntityMapping())
            {
                EntityToSiteMapCollectionClass entityToSiteMapCollection =
                                                    FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
                                                        entityToSiteMaps => entityToSiteMaps.EnumerateEntityMapsBySiteGuid(
                                                            security, entityType, security.SiteGuid, true));

                foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
                {
                    entities.Add(entityToSiteMap);
                }
            }
            return entities;
		}

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
         if (useNewLicenseKey == 1)
         {
               if (((word1 & 0x01) != 0x01) && security.LoginSiteGuid != Guids.SiteAdminGuid)// master data management)

					{
                  return null;
               }
         }
         else
         {
               // Depends Upon Shared Components Config
               if ((options & 0x4000) == 0)
               {
                  return null;
               }
         }

         if ((security.HasRight(RIGHT.VIEW_ENTITY_ASSIGNMENTS) == false)
			   && (security.HasRight(RIGHT.MODIFY_ENTITY_ASSIGNMENTS) == false))
			{
				return null;
			}

			// The Login Site must be a Site Group
			if (siteGroup == false)
			{
				return null;
			}


			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				               {
					               MenuItemType = FMMenuItemType.ADMIN_SITES_ENTITY_ASSIGNMENTS, 
					               RootMenuName = "Administration", 
					               CategoryName = "Sites", 
					               ItemName = "Entity Assignments", 
					               NavigateUrl = "EntityToSiteAssignmentForm.aspx", 
					               SortOrder = 2, 
					               ApplyDataDictionary = ApplyDataDictionary.Apply
				               };

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method will apply all changes to the database.
		/// We use a button hidden from the user to allow us to perform some checks server side and then optionally
		/// display a confirmation dialog to the user. The hidden button does the actual saving of the records. 
		/// When a confirmation dialog is displayed, the button is clicked through javascript. 
		/// When no confirmation dialog is displayed, the button is "clicked" by the server 
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void HiddenApplyBtnClick(object sender, EventArgs e)
		{
			try
			{
				var entityEngineHshTbl =
					this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_ENTITY_ENGINE] as Dictionary<ENTITY_TYPE, Guid>;
				if (entityEngineHshTbl == null)
				{
					throw new NullReferenceException(PageSessionKeyConstants.EAF_SESSION_ENTITY_ENTITY_ENGINE);
				}

				var gridItems = this.Grid.Rows;

				if (gridItems.Count > 0)
				{
					var entityType = (ENTITY_TYPE)Convert.ToInt32(this.EntityTypeDropdown.SelectedValue);

					Guid entityEngineTypeGuid = entityEngineHshTbl[entityType];

					var dataItems = (List<EntityToSiteMapClass>)this.Session["EntityAssignmentDataSource"];

					FMChannelHelper.MakeCall<IEntityToSiteMaps>(
						entityToSiteMaps => this.UpdateValues(gridItems, dataItems, entityType, entityToSiteMaps, entityEngineTypeGuid));
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateValues(
			GridViewRowCollection gridItems,
			List<EntityToSiteMapClass> dataItems,
			ENTITY_TYPE entityType,
			IEntityToSiteMaps entityToSiteMaps,
			Guid entityEngineTypeGuid)
		{
			var purgeList = new List<EntityToSiteMapClass>();
			var addList = new List<EntityToSiteMapClass>();

			foreach (GridViewRow item in gridItems)
			{
				var dataItem = dataItems[item.DataItemIndex];

				var currentCheckBox = item.FindControl("ACB") as CheckBox;
				if (currentCheckBox == null)
				{
					throw new NullReferenceException("ACB");
				}

				if (currentCheckBox.Checked == dataItem.IsAssigned)
				{
					continue;
				}

				var entityToSiteMap = new EntityToSiteMapClass
				                      {
					                      TypeID = entityType,
					                      ID = dataItem.ID,
					                      IdentityGuid = dataItem.IdentityGuid,
					                      SiteID = dataItem.SiteID,
					                      SiteGuid = dataItem.SiteGuid,
					                      AssignedFromSiteId = dataItem.AssignedFromSiteId,
					                      AssignedFromSiteGuid = dataItem.AssignedFromSiteGuid,
					                      IsAssigned = currentCheckBox.Checked
				                      };

				if (entityToSiteMap.IsAssigned)
				{
					addList.Add( entityToSiteMap );
				}
				else
				{
					purgeList.Add( entityToSiteMap );
				}

				// Reset the data dictionary cache if we are changing assignments.
				if ( entityType == ENTITY_TYPE.DATA_DICTIONARY )
				{
					FMChannelHelper.MakeCall<IDataDictionariesClass>( x => x.ResetDataDictionaryCache( entityToSiteMap.SiteGuid ) );
				}
			}

			if (purgeList.Count > 0)
			{
                List<EntityToSiteMapClass> smallPurgeList = new List<EntityToSiteMapClass>();
                foreach(EntityToSiteMapClass entity in purgeList)
                {
                    smallPurgeList.Add(entity);
                    entityToSiteMaps.PurgeList(this.Security, smallPurgeList);
                    smallPurgeList.Clear();
                }
				
			}

			if (addList.Count > 0)
			{
                List<EntityToSiteMapClass> smallPurgeList = new List<EntityToSiteMapClass>();
                foreach (EntityToSiteMapClass entity in addList)
                {
                    smallPurgeList.Add(entity);
					if (entityType == ENTITY_TYPE.EQUIPMENT)
						entityToSiteMaps.AddEquipmentMappingList(this.Security, smallPurgeList, true);
					else
						entityToSiteMaps.AddList(this.Security, smallPurgeList, entityEngineTypeGuid);
                    smallPurgeList.Clear();
                }
			}
		}

		/// <summary>
		/// Check if there have been any record versioning aware entities unassigned from sites. 
		/// If so, then display a confirmation dialog to the user, otherwise, save the records
		/// </summary>
		/// <param name="sender">The sender, i.e. the apply button. Other than being passed to the OnClick of the hidden apply button it is not used.</param>
		/// <param name="e">Event data. Other than being passed to the OnClick of the hidden apply button it is not used.</param>
		protected void ApplyBtn_Onclick(object sender, EventArgs e)
		{
			if (this.HasRecordVersioningEntityUnassignments)
			{
				// Record versioning aware entities are being unassigned. 
				// Show the user a confirmation dialog
				const string RecordVersioningWarningMessage =
					"This operation covers an Entity Type that supports modifications of the assigned entity records at the assigned sites. "
					+ "Any site-specific data maintained for the entity record/s affected by the deleted assignment/s will also be deleted. "
					+ "Are you sure you want to proceed with the entity assignment changes?";

				ScriptManager.RegisterStartupScript(
					this,
					this.GetType(),
					"ConfirmDialog",
					"ShowConfirmationDialogAndClickButton('" + HttpUtility.JavaScriptStringEncode(RecordVersioningWarningMessage) + "','" + this.HiddenApplyButton.ID + "');",
					true);
			}
			else
			{
				// We don't have to display the confirmation dialog 
				// Call the method to save the entity assignments
				this.HiddenApplyBtnClick(sender, e);
			}
		}

		/// <summary>
		/// This method will handle the entity assignment grid Item Data Bound event. It will
		/// load the value of each field into the control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridItemEventArgs" /> instance containing the event data.</param>
		protected void EntityAssignmentGridItemDataBound( object sender, GridViewRowEventArgs e )
		{
			bool bAllEntitySelected = false;
			if (this.EntityDropdown.SelectedItem != null && this.EntityDropdown.SelectedItem.Text == AllStr)
				bAllEntitySelected = true;

			bool bAllAssignmentEntity = false;
			if (this.EntityTypeDropdown.SelectedItem != null)
			{
				string entityTypeString = this.EntityTypeDropdown.SelectedItem.Value;
				var entityType = (ENTITY_TYPE)Convert.ToInt32(entityTypeString);
				if ((entityType == ENTITY_TYPE.ALARM_AND_EVENT) || (entityType == ENTITY_TYPE.DATA_DICTIONARY) || (entityType == ENTITY_TYPE.QUERY_DEFAULT_FIELD) || (entityType == ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS) || (entityType == ENTITY_TYPE.USER_DATA_FIELD))
					bAllAssignmentEntity = true;
			}
			bool bAllEntityRestrictionsMet = false;
			if (!bAllEntitySelected || bAllAssignmentEntity)
				bAllEntityRestrictionsMet = true;

			try
			{
				if (e.Row.RowIndex > -1)
				{
					var entityToSiteMap = (EntityToSiteMapClass) e.Row.DataItem;

					if (this.Security.HasRight(RIGHT.MODIFY_ENTITY_ASSIGNMENTS) == false
						  || (entityToSiteMap.IsAssigned && entityToSiteMap.AssignedFromSiteGuid != this.Security.SiteGuid)
						  || !bAllEntityRestrictionsMet)
					{
						e.Row.Enabled = false;
					}

                    var disableSelectionCheckbox = (CheckBox)e.Row.FindControl("DisableSelectionCheckbox");
				    if (disableSelectionCheckbox != null && disableSelectionCheckbox.Checked)
				    {
				        e.Row.Enabled = false;
				    }
                }
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will handle the sort command event. It will save the sort column in session.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridSortCommandEventArgs" /> instance containing the event data.</param>
		protected void EntityAssignmentGridSortCommand( object source, GridViewSortEventArgs e )
		{
			try
			{
				bool validSortKey = false;
				string sortExpression = e.SortExpression;

				// Can only sort on the Entity Type, Entity, or Site columns.
				if (sortExpression != null)
				{
					if (sortExpression.ToUpper().Equals("ENTITY"))
					{
						validSortKey = true;
					}
					else if (sortExpression.ToUpper().Equals("SITE"))
					{
						validSortKey = true;
					}
					else if (sortExpression.ToUpper().Equals("ASSIGNEDFROMSITE"))
					{
						validSortKey = true;
					}
				}

				if (validSortKey)
				{
					var sortField = this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SORT_KEY] as string;
					if (string.IsNullOrEmpty(sortField))
					{
						this.Page.Session.Add(PageSessionKeyConstants.EAF_SESSION_SORT_KEY, sortExpression + " ASC");
						//this.PersistFilters();
						this.UpdateView();
					}
					else
					{
						if (sortField.IndexOf(" ASC", StringComparison.Ordinal) > 0)
						{
							sortField = sortExpression + " DESC";
						}
						else
						{
							sortField = sortExpression + " ASC";
						}

						this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SORT_KEY] = sortField;
						this.UpdateView();
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The modify site drop down.
		/// </summary>
		protected void ModifySiteDropDown()
		{
			if (this.EntityDropdown.SelectedItem != null && this.EntityDropdown.SelectedItem.Text == AllStr)
			{
				var allItem = this.SiteDropDown.Items.FindByText(AllStr);
				if (allItem != null)
				{
					this.SiteDropDown.Items.Remove(allItem);
				}
			}
			else
			{
				var allItem = this.SiteDropDown.Items.FindByText(AllStr);
				if (allItem == null)
				{
					this.SiteDropDown.Items.Insert(0, new ListItem(AllStr, this.Security.SiteGuid.ToString()));
				}
			}
		}

		/// <summary>
		/// The modify entity drop down.
		/// </summary>
		protected void ModifyEntityDropDown()
		{
			if (this.SiteDropDown.SelectedItem != null && this.SiteDropDown.SelectedItem.Text == AllStr)
			{
				var allItem = this.EntityDropdown.Items.FindByText(AllStr);
				if (allItem != null)
				{
					this.EntityDropdown.Items.Remove(allItem);
				}
			}
			else
			{
				var allItem = this.EntityDropdown.Items.FindByText(AllStr);
				if (allItem == null)
				{
					this.EntityDropdown.Items.Insert(0, new ListItem(AllStr, Convert.ToString(Guid.Empty)));
				}
			}
		}

		/// <summary>
		/// This method handles the entity selection change event. It will update
		/// the grid based on the filters.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void EntitySelectChange(object sender, EventArgs e)
		{
			try
			{
				if (e != null)
				{
					this.ModifySiteDropDown();
				}

				this.RefreshGrid();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the entity type selection change event. It will update
		/// the grid based on the filters.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void EntityTypeSelectChange(object sender, EventArgs e)
		{
			try
			{
				this.PersistFilters();
				this.LoadEntityDropdown();
				this.ModifyEntityDropDown();
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the Include Member Site checkbox change event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void IncludeMemberSiteChange(object sender, EventArgs e)
		{
			try
			{
				this.RefreshGrid();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// This method is the main entry point into the entity assignment page.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Disable the Assign buttons unless the user has modify site rights. 
				this.AssignBtnSecurityCheck();

				if (this.Page.IsPostBack == false)
				{
					if (false == this.IsFromUserForm)
					{
					    this.BottomCloseBtn.Visible = false;
					    this.TopCloseBtn.Visible = false;
					}
					else
					{
					    this.BottomCloseBtn.Visible = true;
					    this.TopCloseBtn.Visible = true;
					}

					this.LoadSiteDropDown();
					this.LoadEntityTypeDropdown();
					this.LoadEntityDropdown();
					this.SetFilterFields();
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the Site dropdown selection change. It disables the Include
		/// Member Site check box if the site selected is not a site group.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void SiteSelectionChange(object sender, EventArgs e)
		{
			try
			{
				Guid selectedSiteGuid = Guid.Parse(this.SiteDropDown.SelectedValue);
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
					sites => sites.Get(
						this.Security, 
						selectedSiteGuid, 
						getMemberSites: true, 
						getSchedulesAndProcessVariables: true, 
						bGetAssociatedAliases: true));

				if (site.SiteGroup == false)
				{
					this.IncludeMemberSitesCheckBox.Checked = false;
					this.IncludeMemberSitesCheckBox.Enabled = false;
				}
				else if (selectedSiteGuid == this.Security.SiteGuid)
				{
					this.IncludeMemberSitesCheckBox.Checked = true;
					this.IncludeMemberSitesCheckBox.Enabled = false;
				}
				else
				{
					this.IncludeMemberSitesCheckBox.Enabled = true;

					if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_INCLUDE_MEMBERS] != null)
					{
						var isChecked = (bool)this.Page.Session[PageSessionKeyConstants.EAF_SESSION_INCLUDE_MEMBERS];
						this.IncludeMemberSitesCheckBox.Checked = isChecked;
					}
				}

				// When the event is null, that means it was called on the initial page
				// load and there is no need to refresh the grid.
				if (e != null)
				{
					this.ModifyEntityDropDown();
					// Refresh the grid based on the filter settings.
					this.RefreshGrid();
				}
			}
			catch (Exception)
			{
				this.IncludeMemberSitesCheckBox.Checked = false;
				this.IncludeMemberSitesCheckBox.Enabled = false;
			}
		}


		/// <summary>
		/// Handles the Onclick event of the CloseBtn control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void CloseBtn_Onclick(object sender, EventArgs e)
		{
			this.Redirect(UserForm.UserFormUrl + "?" + this.Security.CSRFTokenWithParamName);
		}

		/// <summary>
		///     This method will disable the Assign and Unassign buttons if the user does not have
		///     modify sites rights.
		/// </summary>
		private void AssignBtnSecurityCheck()
		{
			bool bAllEntitySelected = false;
			if (this.EntityDropdown.SelectedItem != null && this.EntityDropdown.SelectedItem.Text == AllStr)
				bAllEntitySelected = true;

			bool bAllAssignmentEntity = false;
			if (this.EntityTypeDropdown.SelectedItem != null)
			{
				string entityTypeString = this.EntityTypeDropdown.SelectedItem.Value;
				var entityType = (ENTITY_TYPE)Convert.ToInt32(entityTypeString);
				if ((entityType == ENTITY_TYPE.ALARM_AND_EVENT) || (entityType == ENTITY_TYPE.DATA_DICTIONARY) || (entityType == ENTITY_TYPE.QUERY_DEFAULT_FIELD) || (entityType == ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS) || (entityType == ENTITY_TYPE.USER_DATA_FIELD))
					bAllAssignmentEntity = true;
			}
				
			this.TopAssignAllButton.Enabled = false;
			this.TopUnassignAllButton.Enabled = false;
			this.TopApplyBtn.Enabled = false;
			this.BottomApplyBtn.Enabled = false;

			if ((!bAllEntitySelected || bAllAssignmentEntity) && this.Security.HasRight(RIGHT.MODIFY_ENTITY_ASSIGNMENTS))
			{
				this.TopAssignAllButton.Enabled = true;
				this.TopUnassignAllButton.Enabled = true;
				this.TopApplyBtn.Enabled = true;
				this.BottomApplyBtn.Enabled = true;
			}
		}

		/// <summary>
		/// True if the specified entity type is record versioning aware
		/// </summary>
		/// <param name="entityType">The entity type to check</param>
		/// <returns>True for record versioning aware entities</returns>
        private bool EntitySupportsRecordVersioning(ENTITY_TYPE entityType)
        {
	        bool result = entityType == ENTITY_TYPE.EQUIPMENT 
				|| entityType == ENTITY_TYPE.PRODUCT
	            || entityType == ENTITY_TYPE.COMPANY 
				|| entityType == ENTITY_TYPE.TRANSACTION_ALIAS
	            || entityType == ENTITY_TYPE.PERSONNEL;

	        return result;
        }
        
		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Grid.RowDataBound += this.EntityAssignmentGridItemDataBound;
			this.Grid.Sorting += this.EntityAssignmentGridSortCommand;
		}

		/// <summary>
		///     This method will load the entity dropdown list with a list of all the entities.
		/// </summary>
		private void LoadEntityDropdown()
		{
			ENTITY_TYPE selectedEntityType;

			// Set the Entity dropdown to be empty if the entity type is set to UNKNOWN
			if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] != null)
			{
				selectedEntityType = (ENTITY_TYPE) Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] as string);
			}
			else
			{
				this.EntityDropdown.Items.Clear();
				return;
			}

			Guid currentSiteGuid = this.Security.SiteGuid;
			Guid loginSiteGuid = this.Security.LoginSiteGuid;

			this.Security.LoginSiteGuid = currentSiteGuid;

			try
			{
				List<EntityToSiteMapClass> list = this.EnumerateEntities(this.Security, selectedEntityType);

				this.EntityDropdown.Items.Clear();
                var newItem = new ListItem(AllStr, Convert.ToString(Guid.Empty));
				if (this.SiteDropDown.SelectedItem == null || this.SiteDropDown.SelectedItem.Text.CompareTo(AllStr) != 0)
				{
					this.EntityDropdown.Items.Add(newItem);
				}

				foreach (EntityToSiteMapClass entityToSiteMap in list)
				{
					string entityID = entityToSiteMap.ID;
                    string entityGuid = Convert.ToString(entityToSiteMap.IdentityGuid);
					newItem = new ListItem(entityID, entityGuid);
					this.EntityDropdown.Items.Add(newItem);
				}

				if (this.IsFromUserForm)
				{
				    this.EntityDropdown.SelectedValue = this.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_SELECT] as string;
				    this.EntityDropdown.Enabled = false;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Security.SiteGuid = currentSiteGuid;
				this.Security.LoginSiteGuid = loginSiteGuid;
			}
		}

		/// <summary>
		///     This method will load the entity type dropdown list with a list of all the entity types. It should start
		///     with an "{None}" in the list.
		/// </summary>
		private void LoadEntityTypeDropdown()
		{
			Guid siteGuid = this.Security.SiteGuid;
			Guid loginSiteGuid = this.Security.LoginSiteGuid;
			try
			{
				ListItem newItem;
				this.EntityTypeDropdown.Items.Clear();

				var entityEngineHshTbl = new Dictionary<ENTITY_TYPE, Guid>();

				if (this.IsFromUserForm)
				{
					ENTITY_TYPE entityType = (ENTITY_TYPE) Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] as string);
					string entityTypeID = EntityToSiteMapClass.GetEntityTypeID(entityType);
					newItem = new ListItem(entityTypeID, ((int)entityType).ToString(CultureInfo.InvariantCulture));
					this.EntityTypeDropdown.Items.Add(newItem);
					entityEngineHshTbl.Add(entityType, typeof(IUsers).GUID);
				}
				else
				{
					// Populate EntityTypeListBox
					string discoveryAssem =
						FMChannelHelper.MakeCall<IConfigurationSettings, string>(
							configSettingsClient =>
							configSettingsClient.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_IDiscoveryAssemblies));

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
										string message = "Assembly Load Error on Entity To Site Load Entity Types. " + ex.Message;
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
									Type entityDiscoveryInterface = module.GetInterface("IEntityDiscovery");

									if (entityDiscoveryInterface == null)
									{
										continue;
									}

									object engine = Activator.CreateInstance(module);
									var discovery = (IEntityDiscovery)engine;

									if (discovery.EntityAssignable == false)
									{
										continue;
									}

									ENTITY_TYPE entityType = discovery.EntityType;
									string entityTypeID = EntityToSiteMapClass.GetEntityTypeID(entityType);

									newItem = new ListItem(entityTypeID, ((int)entityType).ToString(CultureInfo.InvariantCulture));
									this.EntityTypeDropdown.Items.Add(newItem);

									// This information is used during the Add entity assignment process.
									if (!entityEngineHshTbl.ContainsKey(entityType))
									{
										Type entityEngineType = discovery.EntityEngineType;
										entityEngineHshTbl.Add(entityType, entityEngineType.GUID);
									}
								}
							}
							catch { }
						}
					}

					if ((this.EntityTypeDropdown != null) && (this.EntityTypeDropdown.Items.Count > 0))
					{
						newItem = new ListItem("{None}", ((int)ENTITY_TYPE.UNKNOWN).ToString(CultureInfo.InvariantCulture));
						this.EntityTypeDropdown.Items.Insert(0, newItem);
						this.EntityTypeDropdown.SelectedIndex = 0;
					}
				}

				this.Page.Session.Remove(PageSessionKeyConstants.EAF_SESSION_ENTITY_ENTITY_ENGINE);

				if (entityEngineHshTbl.Count > 0)
				{
					this.Page.Session.Add(PageSessionKeyConstants.EAF_SESSION_ENTITY_ENTITY_ENGINE, entityEngineHshTbl);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Security.SiteGuid = siteGuid;
				this.Security.LoginSiteGuid = loginSiteGuid;
			}
		}
        

		/// <summary>
		///     This method will load the site dropdown list with a list of sites if the login site
		///     is a site group or just one site if not a site group.
		/// </summary>
		private void LoadSiteDropDown()
		{
			this.SiteDropDown.Items.Clear();

			this.SiteDropDown.Items.Add(new ListItem(NoneStr, Guid.Empty.ToString()));
			if (this.EntityDropdown.SelectedItem == null || this.EntityDropdown.SelectedItem.Text.CompareTo(AllStr) != 0)
			{
				this.SiteDropDown.Items.Add(new ListItem(AllStr, this.Security.SiteGuid.ToString()));
			}

			SiteClass site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(
					sites => sites.Get(
						this.Security, 
						this.Security.SiteGuid, 
						getMemberSites: true, 
						getSchedulesAndProcessVariables: true, 
						bGetAssociatedAliases: true));

			if (site != null)
			{
				if (site.SiteGroup)
				{
					SiteCollectionClass siteCollection =
						FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
							sites => sites.EnumerateByParentSite(this.Security, this.Security.SiteGuid));

					foreach (SiteClass nextSite in siteCollection)
					{
						this.SiteDropDown.Items.Add(new ListItem(nextSite.ID, nextSite.IdentityGuid.ToString()));
					}
				}
				else
				{
					this.SiteDropDown.Items.Add(new ListItem(site.ID, this.Security.SiteGuid.ToString()));
				}

				// Initially set the default to the first item in the list.
				this.SiteDropDown.SelectedIndex = 0;

				// This will set the Include member sites checkbox.
				this.SiteSelectionChange(null, null);
			}
		}

		/// <summary>
		///     This method will persist the entity assignment page filters.
		/// </summary>
		private void PersistFilters()
		{
			this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] = this.EntityTypeDropdown.SelectedValue;
			this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_SELECT] = this.EntityDropdown.SelectedValue;
			this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SITE_SELECT] = this.SiteDropDown.SelectedItem.Text;
			this.Page.Session[PageSessionKeyConstants.EAF_SESSION_INCLUDE_MEMBERS] = this.IncludeMemberSitesCheckBox.Checked;			
		}

		/// <summary>
		///     This method refreshes the grid.
		/// </summary>
		private void RefreshGrid()
		{
			this.PersistFilters();
			this.UpdateView();
		}


		/// <summary>
		///     This method will set all the Filters to their previous values.
		/// </summary>
		private void SetFilterFields()
		{
			if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] != null)
			{
				string selectedValue = this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] as string;
				this.EntityTypeDropdown.SelectedValue = selectedValue;
			}

			if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_SELECT] != null)
			{
				string selectedValue = this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_SELECT] as string;
				this.EntityDropdown.SelectedValue = selectedValue;

				if (this.EntityDropdown.SelectedItem.Text.Equals(AllStr))
				{
					string targetSiteId = Convert.ToString(this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SITE_SELECT]);
					if (targetSiteId.Equals(AllStr))
					{
						this.EntityDropdown.ClearSelection();
						if ((this.EntityDropdown.SelectedItem.Text.Equals(AllStr)) && (this.EntityDropdown.Items.Count > this.EntityDropdown.SelectedIndex + 1))
							this.EntityDropdown.SelectedIndex = this.EntityDropdown.SelectedIndex + 1;
					}
				}
			}			


			if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SITE_SELECT] != null)
			{
                string targetSiteId = Convert.ToString(this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SITE_SELECT]);
				if (targetSiteId.Equals(this.Security.SiteID))
				{
					targetSiteId = NoneStr;
				}

                for (int i = 0; i < this.SiteDropDown.Items.Count; i++)
                {
                    if (this.SiteDropDown.Items[i].Text.Equals(targetSiteId))
                    {
                        this.SiteDropDown.SelectedIndex = i;
                        this.SiteSelectionChange(null, null);  //Enable/disable the IncludeMemberSitesCheckBox according to the site selection.
                        break;
                    }
                }

                //Check/uncheck the IncludeMemberSitesCheckBox according to the site selection and the corresponding session variable value.
                this.IncludeMemberSitesCheckBox.Checked = false;
				if (this.SiteDropDown.SelectedValue == Convert.ToString(this.Security.SiteGuid))
				{
					this.IncludeMemberSitesCheckBox.Checked = true;
				}
                else if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_INCLUDE_MEMBERS] != null)
                {
	                if (this.SiteDropDown.SelectedItem.Text.Equals(targetSiteId))
	                {
		                this.IncludeMemberSitesCheckBox.Checked = (bool)this.Page.Session[PageSessionKeyConstants.EAF_SESSION_INCLUDE_MEMBERS];
	                }
                }       
			}
		}

		/// <summary>
		///     This method will retrieve new entity data and bind the data to
		///     the entity assignment grid.  The data will be retrieved based on the filting
		///     criterion.
		/// </summary>
		private void UpdateView()
		{
            Guid entityGuid = Guid.Empty;
            string entityId = null;
			string entityTypeString = this.EntityTypeDropdown.SelectedItem.Value;
			string siteID = this.SiteDropDown.SelectedItem.Text;
			Guid? siteGuid = null;
			bool includeMemberSites = this.IncludeMemberSitesCheckBox.Checked;
			string sortKey = "ASSIGNEDFROMSITE ASC";

			if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SORT_KEY] != null)
			{
				sortKey = (string)this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SORT_KEY];
			}
			else
			{
				this.Page.Session.Add(PageSessionKeyConstants.EAF_SESSION_SORT_KEY, sortKey);
			}

			try
			{
				if (string.IsNullOrEmpty(this.EntityDropdown.SelectedValue) == false)
				{
					if (this.EntityDropdown.SelectedValue != AllStr)
					{
                        entityGuid = new Guid(this.EntityDropdown.SelectedValue);
                        entityId = this.EntityDropdown.SelectedItem.Text;
					}
				}

				if (string.IsNullOrEmpty(this.SiteDropDown.SelectedValue) == false)
				{
					siteGuid = Guid.Parse(this.SiteDropDown.SelectedValue);
				}

				var entityType = (ENTITY_TYPE)Convert.ToInt32(entityTypeString);

				List<EntityToSiteMapClass> list = (string.IsNullOrEmpty(entityTypeString) || entityType == ENTITY_TYPE.UNDEFINED
				                                   || entityType == ENTITY_TYPE.UNKNOWN || siteID.Equals("{None}"))
					                                  ? new List<EntityToSiteMapClass>()
					                                  : this.EnumerateByCriterion(
						                                  this.Security, 
						                                  this.Security.SiteGuid, 
						                                  siteGuid, 
						                                  includeMemberSites,
                                                          entityGuid,
                                                          entityId,
						                                  entityType, 
						                                  sortKey);

				// Bind the data to the grid.
				this.Grid.DataSource = list;
				this.Grid.DataBind();
				this.AssignBtnSecurityCheck();

				this.Session["EntityAssignmentDataSource"] = list;
			}
			catch (Exception)
			{
				const string ErrMsg = "Error retrieving Entity Assignments.";
				this.ErrorHandler(new Exception(ErrMsg));
			}
		}

	    /// <summary>
	    /// This method will return true if the user is an active directory user.
	    /// </summary>
	    /// <param name="security">The security object.</param>
	    /// <param name="userGuid">The user's Guid to retrieve.</param>
	    /// <returns>Returns true if the user is an active directory user.</returns>
	    private bool GetActiveDirectoryUserFlag(SecurityClass security, Guid userGuid)
	    {
	        if (userGuid == Guid.Empty) return false;

	        try
	        {
	            var userObj = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, userGuid));
	            if (userObj == null) return false;

	            return userObj.ActiveDirectoryUser;
	        }
	        catch (Exception)
	        {
                const string ErrMsg = "Error, could not retrieve User.";
                this.ErrorHandler(new Exception(ErrMsg));
            }

	        return false;
	    }

		private bool IsFromUserForm
		{
			get { return this.Page.Request.GetQueryOrFormValue("Mode") != null && this.Page.Request.GetQueryOrFormValue("Mode").Equals("User"); }
		}


		#endregion
	}
}
