using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IDatabaseMaintenance
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ReindexDatabase(SecurityClass security);
	}
}
