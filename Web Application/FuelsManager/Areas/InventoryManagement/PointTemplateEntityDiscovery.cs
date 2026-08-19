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
	public class PointTemplateEntityDiscovery : IPointTemplateDiscovery
	{
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.POINT_TEMPLATE;
			}
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			var pointCollection = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(
																	 x =>
																	 x.EnumerateByType(Security, null)
																);


			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (Security.HasRight(RIGHT.MODIFY_POINT_TEMPLATES))
			{
				foreach (PointTemplate pointTemplate in pointCollection)
				{
					if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
					{
						if (Security.SiteGuid == pointTemplate.SiteGuid)
						{
							continue;
						}

						if (Security.LoginSiteGuid != pointTemplate.SiteGuid)
						{
							continue;
						}
					}
					else
					{
						if (Security.SiteGuid != pointTemplate.SiteGuid)
						{
							continue;
						}
					}

					var EntityToSiteMap = new EntityToSiteMapClass(pointTemplate);
					EntityToSiteMapCollection.Add(EntityToSiteMap);
				}
			}

			return EntityToSiteMapCollection;
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IPointTemplates);
			}
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(
											x =>
											x.Get(security, guid)
									);

			pointTemplate.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IPointTemplates>(x => x.Modify(security, pointTemplate));
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IPointTemplates, Guid>(
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

		EntityToSiteMapCollectionClass IPointTemplateDiscovery.EnumerateEntityMapsForSiteCreation(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			var pointCollection = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(
																	 x =>
																	 x.EnumerateForSiteCreation(Security)
																);


			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (Security.HasRight(RIGHT.MODIFY_POINT_TEMPLATES))
			{
				foreach (PointTemplate pointTemplate in pointCollection)
				{
					if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
					{
						if (Security.SiteGuid == pointTemplate.SiteGuid)
						{
							continue;
						}

						if (Security.LoginSiteGuid != pointTemplate.SiteGuid)
						{
							continue;
						}
					}
					else
					{
						if (Security.SiteGuid != pointTemplate.SiteGuid)
						{
							continue;
						}
					}

					var EntityToSiteMap = new EntityToSiteMapClass(pointTemplate);
					EntityToSiteMapCollection.Add(EntityToSiteMap);
				}
			}

			return EntityToSiteMapCollection;
		}
	}
}