using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.Interfaces
{
	public enum ENTITY_ASSIGNMENT_TYPE
	{
		ASSIGNED = 1,
		UNASSIGNED = 2,
		OWNED = 3,
        UNDELEGATED = 4
	};

	public interface IEntityDiscovery
	{
		ENTITY_TYPE EntityType { get; }
		EntityToSiteMapCollectionClass EnumerateEntityMaps ( SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type );
		Type EntityEngineType { get; }

		void SetSiteGuid(SecurityClass Security, Guid guid, Guid SiteGuid);

		Guid GetIdentityGuid ( SecurityClass security, string ID );
		
		bool EntityAssignable { get; }
	}

	public interface IPointTemplateDiscovery : IEntityDiscovery
	{
		EntityToSiteMapCollectionClass EnumerateEntityMapsForSiteCreation(SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type);
	}
}
