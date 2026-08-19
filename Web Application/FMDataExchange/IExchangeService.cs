using System.ServiceModel;

namespace FMDataExchange
{
	// NOTE: If you change the interface name "IExchangeService" here, you must also update the reference to "IExchangeService" in Web.config.
	[ServiceContract]
	public interface IExchangeService
	{
		// See comment for FaultExceptionActionName the purpose of FaultExceptionActionName and NotUsed method.

		[OperationContract(ReplyAction = MessageInspector.FaultExceptionActionName)]
		void NotUsed();

		[OperationContract]
		string Exchange(string user, string password, bool bCAC, string site, string interfaceID, string xmlData);

		[OperationContract]
		string ExchangeCompressed(string user, string password, bool bCAC, string site, string interfaceID, string compressedXmlData);
	}
}
