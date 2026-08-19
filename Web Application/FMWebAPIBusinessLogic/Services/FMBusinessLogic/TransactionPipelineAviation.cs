using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using Inbound = FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound;
using Outbound = FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Outbound;
using System;
using System.Collections.Generic;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    public class TransactionPipelineAviation : ITransactionPipeline
    {
        private readonly Inbound.TransactionAliasResolver _transactionResolver;
        private readonly Inbound.ProductGuidResolver _productGuidResolver;
        private readonly Inbound.CompanyGuidResolver _companyGuidResolver;
        private readonly Inbound.MeterGuidResolver _meterGuidResolver;
        private readonly Inbound.EquipmentGuidResolver _equipmentGuidResolver;
        private readonly Inbound.AssignEquipmentToMeterGuidResolver _assignEquipmentToMeterGuidResolver;
        private readonly Inbound.PersonnelGuidResolver _personnelGuidResolver;
        private readonly Inbound.TransactionIssueConverter _transactionIssueConverterInbound;
        private readonly Inbound.Transaction24HourConverter _transaction24HourConverter;
        private readonly Inbound.TransactionRotationConverter _transactionRotationConverter;
        private readonly Inbound.TransactionDefuelConverter _transactionDefuelConverter;
        private readonly Inbound.TransactionAdjustmentConverter _transactionAdjustmentConverter;
        private readonly Inbound.TransactionFillStandConverter _transactionFillStandConverter;
        private readonly Inbound.TransactionTransferConverter _transactionTransferConverter;
        private readonly Inbound.TransactionFillStandReceiptConverter _transactionFillStandReceiptConverter;
        private readonly Outbound.IssueTransactionConverter _issueTransactionConverterOutbound;
        private readonly Outbound.TransactionTransferConverter _outboundTransactionTransferConverter;
        private readonly Inbound.Rotation24HourDestinationEquipmentResolver _rotation24HourDestinationEquipmentResolver;

        public TransactionPipelineAviation(
            Inbound.TransactionAliasResolver transactionFixup,
            Inbound.ProductGuidResolver productGuidFixup, 
            Inbound.CompanyGuidResolver companyGuidFixup,
            Inbound.MeterGuidResolver meterGuidFixup, 
            Inbound.EquipmentGuidResolver equipmentGuidResolver, 
            Inbound.AssignEquipmentToMeterGuidResolver assignEquipmentToMeterGuidFixup,
            Inbound.PersonnelGuidResolver personnelGuidFixup,
            Inbound.TransactionIssueConverter transactionIssueConverterInbound,
            Inbound.Transaction24HourConverter transaction24HourConverter,
            Inbound.TransactionRotationConverter transactionRotationConverter,
            Inbound.TransactionDefuelConverter transactionDefuelConverter,
            Inbound.TransactionAdjustmentConverter transactionAdjustmentConverter, 
            Inbound.TransactionFillStandConverter transactionFillStandConverter,
            Inbound.TransactionTransferConverter transactionTransferConverter,
            Inbound.TransactionFillStandReceiptConverter transactionFillStandReceiptConverter,
            Outbound.IssueTransactionConverter issueTransactionFixupOutbound,
            Outbound.TransactionTransferConverter outboundTransactionTransferConverter,
            Inbound.Rotation24HourDestinationEquipmentResolver rotation24HourDestinationEquipmentResolver)
        {
            this._transactionResolver = transactionFixup;
            this._productGuidResolver = productGuidFixup;
            this._companyGuidResolver = companyGuidFixup;
            this._meterGuidResolver = meterGuidFixup;
            this._equipmentGuidResolver = equipmentGuidResolver;
            this._assignEquipmentToMeterGuidResolver = assignEquipmentToMeterGuidFixup;
            this._personnelGuidResolver = personnelGuidFixup;
            this._transactionIssueConverterInbound = transactionIssueConverterInbound;
            this._transaction24HourConverter = transaction24HourConverter;
            this._issueTransactionConverterOutbound = issueTransactionFixupOutbound;
            this._transactionDefuelConverter = transactionDefuelConverter;
            this._transactionRotationConverter = transactionRotationConverter;
            this._transactionAdjustmentConverter = transactionAdjustmentConverter;
            this._transactionTransferConverter = transactionTransferConverter;
            this._transactionFillStandConverter = transactionFillStandConverter;
            this._transactionFillStandReceiptConverter = transactionFillStandReceiptConverter;
            this._outboundTransactionTransferConverter = outboundTransactionTransferConverter;
            this._rotation24HourDestinationEquipmentResolver = rotation24HourDestinationEquipmentResolver;
        }

        public IEnumerable<IPipelineCommand> Inbound()
        {
            return new List<IPipelineCommand>()
            {
                this._transactionResolver,
                this._productGuidResolver,
                this._transactionRotationConverter,
                this._companyGuidResolver,
                this._meterGuidResolver,
                this._equipmentGuidResolver,
                this._assignEquipmentToMeterGuidResolver,
                this._personnelGuidResolver,
                this._transactionIssueConverterInbound,
                this._transaction24HourConverter,
                this._transactionDefuelConverter,
                this._transactionRotationConverter,
                this._transactionAdjustmentConverter,
                this._transactionTransferConverter, 
                this._transactionFillStandConverter,
                this._transactionFillStandReceiptConverter,
                this._rotation24HourDestinationEquipmentResolver
        };
        }

        public IEnumerable<IPipelineCommand> Outbound()
        {
            return new List<IPipelineCommand>()
            {
                this._issueTransactionConverterOutbound,
                this._outboundTransactionTransferConverter
            };
        }
    }
}
