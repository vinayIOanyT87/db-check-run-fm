using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IPointGroupSchedules
	{
		[OperationContract]
		Guid Add(SecurityClass security, PointGroupSchedule pointGroupSchedule);

		[OperationContract]
		void Purge(SecurityClass security, Guid pointGroupGuid, Guid userGuid, Guid siteGuid);

		[OperationContract]
		void Modify(SecurityClass security, PointGroupSchedule pointGroupSchedule);

		[OperationContract]
		PointGroupSchedule Get(SecurityClass security, Guid pointGroupGuid, Guid userGuid, Guid siteGuid);

		[OperationContract]
		PointGroupSchedule GetByPK(SecurityClass security, Guid scheduleGuid);

		[OperationContract]
		PointGroupScheduleCollection EnumerateAll(SecurityClass security);

	}
}
