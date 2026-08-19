namespace FuelsManager.Areas.InventoryManagement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;

	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	public class ModuleEntityDiscovery : IEntityDiscovery
	{
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.MODULE;
			}
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			var moduleDictionary = FMChannelHelper.MakeCall<IModules, Dictionary<Guid, Module>>(
																	 x =>
																	 x.EnumerateBySiteGuid(Security, Security.SiteGuid)
																);


			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (Security.HasRight(RIGHT.MODIFY_MODULE_LIBRARY))
			{
				foreach (var module in moduleDictionary.Values)
				{
					if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
					{
						if (Security.SiteGuid == module.SiteGuid)
						{
							continue;
						}

						if (Security.LoginSiteGuid != module.SiteGuid)
						{
							continue;
						}
					}
					else
					{
						if (Security.SiteGuid != module.SiteGuid)
						{
							continue;
						}
					}

					var EntityToSiteMap = new EntityToSiteMapClass(module);
					EntityToSiteMapCollection.Add(EntityToSiteMap);
				}
			}

			return EntityToSiteMapCollection;
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IModules);
			}
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			var pointTemplate = FMChannelHelper.MakeCall<IModules, Module>(
											x =>
											x.Get(security, guid)
									);

			pointTemplate.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IModules>(x => x.Modify(security, pointTemplate));
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IModules, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, ID)
																);
		}

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}
	}
}