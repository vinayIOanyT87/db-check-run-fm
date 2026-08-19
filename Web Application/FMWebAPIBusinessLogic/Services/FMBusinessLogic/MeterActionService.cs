using System;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;


namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    using FMBusinessObjects.DataObjects;
    using FMWebAPIBusinessLogic.DTO.MeterDTO;

    public class MeterActionService : IMeterActionService
    {
        private readonly IMetersProxy _meterProxy;
        private readonly ITransactionAliasesProxy _transactionAliasProxy;
        public MeterActionService(IMetersProxy meterProxy, ITransactionAliasesProxy transactionAliasProxy)
        {
            this._meterProxy = meterProxy;
            this._transactionAliasProxy = transactionAliasProxy;
        }
        public MeterOverflowDTO DidMeterRollover(string meterId, Guid transactionAliasGuid, double meterStart, double meterStop)
        {
            var transactionAlias = this._transactionAliasProxy.Get(transactionAliasGuid, false);
            var result = new MeterOverflowDTO();
            Guid meterGuid = Guid.Empty;
            MeterClass meter = null;
            // defuel types need to be flipped
            if (transactionAlias.TransTypeID == TransactionTypes.T4_SecondaryDefuel)
            {
                meterGuid = this._meterProxy.GetIdentityGuid(meterId);
                meter = this._meterProxy.Get(meterGuid);
                if (meter.RotatesBackwardsFlag)
                {
                    var temp = meterStart;
                    meterStart = meterStop;
                    meterStop = temp; 
                }
            }
            result.Difference = Math.Abs(meterStop - meterStart);
            // no rollover happened
            if (meterStart < meterStop)
            {
                result.MeterOverflowed =  false;
                return result;
            }
            if (transactionAlias.TransTypeID != TransactionTypes.T4_SecondaryDefuel)
            {
                meterGuid = this._meterProxy.GetIdentityGuid(meterId);
                meter = this._meterProxy.Get(meterGuid);
            }

            result.NumberOfDigitsInMeter = meter.NumberOfDigits;

            //meter rollover 3 decimals
            //largest possible number = 999
            var largestPossibleNumber = Math.Pow(10, meter.NumberOfDigits) -1;
            //lowestPossibeMeterStart = 900
            var lowestPossibleMeterStart = largestPossibleNumber - (largestPossibleNumber * .1);
            //if meterstart is lower than the lowestPossibleMeterStart, meter rollover did not happen
            if (lowestPossibleMeterStart > meterStart)
            {
                result.MeterOverflowed = false;
                return result;
            }
            //meterStart = 999
            //meterStop = 3
            //diff = 4
            var diff = (meterStop + (largestPossibleNumber + 1)) - meterStart;
            //if the diff is larger than 10% it is probably a typo
            if (diff > (largestPossibleNumber * .1))
            {
                result.MeterOverflowed =  false;
                return result;
            }

            result.MeterOverflowed = true;
            result.Difference = diff;
            return result;
        }
        
    }
}
