
namespace FMPointTagArchive.Core.Interfaces
{
    using ServiceRequests;
    using System.Data;

    public interface ICurrentTankInventoryProcessor
    {
        DataSet Process(CurrentTankInventoryProcessorSR serviceRequest);
    }
}
