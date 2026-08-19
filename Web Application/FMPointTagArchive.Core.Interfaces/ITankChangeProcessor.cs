namespace FMPointTagArchive.Core.Interfaces
{
	using ServiceRequests;
	using System.Data;

	public interface ITankChangeProcessor
	{
		DataSet Process(TankChangeProcessorSR serviceRequest);
	}
}
