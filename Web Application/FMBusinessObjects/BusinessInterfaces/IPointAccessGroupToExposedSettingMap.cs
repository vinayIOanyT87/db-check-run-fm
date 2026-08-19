using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using DataObjects;

	public interface IPointAccessGroupToExposedSettingMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<PointAccessGroupToExposedSettingMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);
	}
}
