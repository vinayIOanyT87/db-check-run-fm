using System.Web.Http;
using System;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.DTO.MeterDTO;
namespace FMWebAPI.Controllers
{

    [RoutePrefix("api/Meter")]
    public class MeterWebAPIController : ApiController
    {
        private readonly IMeterActionService _meterActionService;
        public MeterWebAPIController(IMeterActionService meterActionService)
        {
            this._meterActionService = meterActionService;
        }


        [Route("MeterHasRolled")]
        [HttpGet]
        public MeterOverflowDTO HasMeterRolled(string meterID, string transactionAliasGuid, double meterStart, double meterStop)
        {
            if (string.IsNullOrWhiteSpace(meterID))
            {
                throw new ArgumentNullException(nameof(meterID));
            }
            if (string.IsNullOrWhiteSpace(transactionAliasGuid))
            {
                throw new ArgumentNullException(nameof(transactionAliasGuid));
            }
            Guid parsedTransactionGuid;
            if (!Guid.TryParse(transactionAliasGuid, out parsedTransactionGuid))
            {
                throw new ArgumentException("Invalid transaction guid format");
            }

            return this._meterActionService.DidMeterRollover(meterID, parsedTransactionGuid, meterStart, meterStop);
            //return result;
        }
    }
}
