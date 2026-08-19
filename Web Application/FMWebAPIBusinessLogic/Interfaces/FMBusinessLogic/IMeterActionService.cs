using FMWebAPIBusinessLogic.DTO.MeterDTO;
using System;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    public interface IMeterActionService
    {
        MeterOverflowDTO DidMeterRollover(string meterId, Guid transactionAliasGuid, double meterStart, double meterStop);
    }
}
