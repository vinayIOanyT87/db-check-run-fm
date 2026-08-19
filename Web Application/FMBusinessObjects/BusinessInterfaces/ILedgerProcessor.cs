using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface ILedgerProcessor
	{
		[OperationContract]
		LedgerDO Process ( LedgerSR sr, AccountingSite accountingSite = null );
	}
}
