using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class CompanyGuidResolver : IPipelineCommand
    {
        private readonly ICompanyProxy _companyProxy;

        public CompanyGuidResolver(ICompanyProxy companyProxy)
        {
            _companyProxy = companyProxy;
        }
        
        public bool TryGetManager(string ID, out CompanyClass manager)
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                manager = null;
                return false;
            }
            var managers = _companyProxy.EnumerateByRole(COMPANY_ROLE.MANAGER, false, false, true);
            manager = managers.FirstOrDefault(x => x.ID == ID);
            return manager != null;
        }

        public bool TryGetOwner(string ID, out CompanyClass owner)
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                owner = null;
                return false;
            }
            var owners = _companyProxy.EnumerateByRole(COMPANY_ROLE.OWNER, false, false, true);
            owner = owners.FirstOrDefault(x => x.ID == ID);
            return owner != null;
        }

        public bool TryGetSupplier(string ID, out CompanyClass supplier)
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                supplier = null;
                return false;
            }
            var suppliers = _companyProxy.EnumerateByRole(COMPANY_ROLE.SUPPLIER, false, false, true);
            supplier = suppliers.FirstOrDefault(x => x.ID == ID);
            return supplier != null;
        }

        public bool TryGeCarrier(string ID, out CompanyClass carrier)
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                carrier = null;
                return false;
            }
            var carriers = _companyProxy.EnumerateByRole(COMPANY_ROLE.CARRIER, false, false, true);
            carrier = carriers.FirstOrDefault(x => x.ID == ID);
            return carrier != null;
        }

        public bool TryGetShipTo(string ID, out CompanyClass shipTo)
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                shipTo = null;
                return false;
            }
            var shipToCompanies = _companyProxy.EnumerateByRole(COMPANY_ROLE.CUSTOMER_SHIPTO, false, false, true);
            shipTo = shipToCompanies.FirstOrDefault(x => x.ID == ID);
            return shipTo != null;
        }

        public bool TryGetShipper(string ID, out CompanyClass shipper)
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                shipper = null;
                return false;
            }
            var shipperCompanies = _companyProxy.EnumerateByRole(COMPANY_ROLE.SHIPPER, false, false, true);
            shipper = shipperCompanies.FirstOrDefault(x => x.ID == ID);
            return shipper != null;
        }


        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            setupForTransferTransaction(trxDO, trxAlias);
            CompanyClass manager;
            if (TryGetManager(trxDO.ManagerID, out manager))
            {
                trxDO.ManagerCompanyGuid = manager.MasterRecordGuid;
                trxDO.ManagerCode = manager.Code;
            }
            CompanyClass company;
            if (TryGetOwner(trxDO.OwnerID, out company))
            {
                trxDO.OwnerCompanyGuid = company.MasterRecordGuid;
                trxDO.OwnerCode = company.Code;
            }
            CompanyClass supplier;
            if (TryGetSupplier(trxDO.SupplierID, out supplier))
            {
                trxDO.SupplierCompanyGuid = supplier.MasterRecordGuid;
                trxDO.SupplierCode = supplier.Code;
            }
            CompanyClass carrier;
            if (TryGeCarrier(trxDO.CarrierID, out carrier))
            {
                trxDO.CarrierCompanyGuid = carrier.MasterRecordGuid;
                trxDO.CarrierCode = carrier.Code;
            }
            CompanyClass shipTo;
            if (TryGetShipTo(trxDO.ShipToID, out shipTo))
            {
                trxDO.ShipToCompanyGuid = shipTo.MasterRecordGuid;
                trxDO.ShipToCode = shipTo.Code;
            }
            CompanyClass shipper;
            if (TryGetShipper(trxDO.ShipperID, out shipper))
            {
                trxDO.ShipperCompanyGuid = shipper.MasterRecordGuid;
                trxDO.ShipperCode = shipper.Code;
            }
            

            var ownerTransaction = trxDO as OwnerTransferDO;
            if (ownerTransaction != null)
            {
                CompanyClass toManager;
                if (TryGetManager(ownerTransaction.ToManagerID, out toManager))
                {
                    ownerTransaction.ToManagerCompanyGuid = toManager.MasterRecordGuid;
                    ownerTransaction.ToManagerCode = toManager.Code;
                }
                CompanyClass toCompany;
                if (TryGetOwner(ownerTransaction.ToOwnerID, out toCompany))
                {
                    ownerTransaction.ToOwnerCompanyGuid = toCompany.MasterRecordGuid;
                    ownerTransaction.ToOwnerCode = toCompany.Code;
                }
                CompanyClass toCarrier;
                if (TryGeCarrier(ownerTransaction.ToCarrierID, out toCarrier))
                {
                    ownerTransaction.ToCarrierCompanyGuid = toCarrier.MasterRecordGuid;
                    ownerTransaction.ToCarrierCode = toCarrier.Code;
                }
            }
        }

        private void setupForTransferTransaction(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (trxAlias.TransTypeID != TransactionTypes.T13_OwnerTransfer)
            {
                return;
            }
            //lets move over the from fields over to this transaction fields
            if (string.IsNullOrWhiteSpace(trxDO.OwnerID) && !string.IsNullOrWhiteSpace(trxDO.FromOwnerID))
            {
                trxDO.OwnerID = trxDO.FromOwnerID;
            }
            if (string.IsNullOrWhiteSpace(trxDO.ManagerID) && !string.IsNullOrWhiteSpace(trxDO.FromManagerID))
            {
                trxDO.ManagerID = trxDO.FromManagerID;
            }
            if (string.IsNullOrWhiteSpace(trxDO.CarrierID) && !string.IsNullOrWhiteSpace(trxDO.FromCarrierID))
            {
                trxDO.CarrierID = trxDO.FromCarrierID;
            }
        }
    }
}
