using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.DTO;
using FMWebAPIBusinessLogic.DTO.TransactionDTO;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FMWebAPIBusinessLogic.Services.Controllers
{
    public class TransactionController
    {
        private readonly ITransactionFieldsService _transactionFieldsService;
        private readonly ITransactionObjectTranslationService _createTransactionObjectService;
        private readonly ICurrentRequestContext _currentRequestContext;
        private readonly IProductsProxy _productProxy;
        private readonly ICompanyProxy _companyProxy;
        private readonly IEquipmentsProxy _equipmentsProxy;
        private readonly ITransactionAliasFieldPlacementInformationProxy _transansactionFieldPlacementProxy;
        private readonly ITransactionAliasesProxy _transactionAliasProxy;
        private readonly ITransactionProcessorProxy _transactionProcessorProxy;
        private readonly ITransactionPipeline _transactionPipeline;
        private readonly IAutoDocumentNumberService _autoDocumentNumberService;
        private readonly ITransactionPossibleActionsService _transactionPossibleActionsService;
        private readonly ITransactionActionsProcessorsService _transactionActionsProcessorService;
        private readonly ITanksProxy _tanksProxy;
        private readonly IFMCustomLogger _logger;
        private readonly IMetersProxy _meterProxy;

        public TransactionController(ITransactionFieldsService transactionFieldsService,
            ITransactionObjectTranslationService createTransactionObjectService,
            ICurrentRequestContext currentRequestContext,
            IProductsProxy productProxy,
            ICompanyProxy companyProxy,
            IEquipmentsProxy equipmentsProxy,
            ITransactionAliasFieldPlacementInformationProxy transansactionFieldPlacementProxy,
            ITransactionAliasesProxy transactionAliasProxy,
            ITransactionProcessorProxy transactionProcessorProxy,
            ITransactionPipeline transactionPipeline,
            IAutoDocumentNumberService autoDocumentNumberService,
            ITransactionPossibleActionsService transactionPossibleActionsService,
            ITransactionActionsProcessorsService transactionActionsProcessorService,
            ITanksProxy tanksProxy,
            IFMCustomLogger logger,
            IMetersProxy meterProxy)
        {
            this._transactionFieldsService = transactionFieldsService;
            this._createTransactionObjectService = createTransactionObjectService;
            this._currentRequestContext = currentRequestContext;
            this._productProxy = productProxy;
            this._companyProxy = companyProxy;
            this._equipmentsProxy = equipmentsProxy;
            this._transansactionFieldPlacementProxy = transansactionFieldPlacementProxy;
            this._transactionAliasProxy = transactionAliasProxy;
            this._transactionProcessorProxy = transactionProcessorProxy;
            this._transactionPipeline = transactionPipeline;
            this._autoDocumentNumberService = autoDocumentNumberService;
            this._transactionPossibleActionsService = transactionPossibleActionsService;
            this._transactionActionsProcessorService = transactionActionsProcessorService;
            this._tanksProxy = tanksProxy;
            this._logger = logger;
            this._meterProxy = meterProxy;
        }

        public TransactionDO SubmitNewTransaction(Dictionary<string, string> newTransactionUserValues, string transactionAliasGuid)
        {

            Guid parsedTransactionAliasGuid;
            if (!Guid.TryParse(transactionAliasGuid, out parsedTransactionAliasGuid))
            {
                throw new NotSupportedException("Cannot parse the submitted transaction alias guid");
            }

            return this._transactionActionsProcessorService.SubmitNewTransactionInDictionaryFormat(
                newTransactionUserValues,
                parsedTransactionAliasGuid);
        }

        public TransactionDO UpdateExistingTransaction(Dictionary<string, string> newTransactionUserValues, string transactionAliasGuid, string transactionGuid)
        {

            Guid parsedTransactionAliasGuid;
            if (!Guid.TryParse(transactionAliasGuid, out parsedTransactionAliasGuid))
            {
                throw new NotSupportedException("Cannot parse the submitted transaction alias guid");
            }
            Guid parsedTransactionGuid;
            if (!Guid.TryParse(transactionGuid, out parsedTransactionGuid))
            {
                throw new NotSupportedException("Cannot parse the submitted transaction guid");
            }

            return this._transactionActionsProcessorService.UpdateExistingTransactionInDictionaryFormat(
                newTransactionUserValues,
                parsedTransactionAliasGuid,
                parsedTransactionGuid);
        }

        public void ReverseUpdateTransaction(Guid originalTransactionGuid, Dictionary<string, string> updatedTransactionUserValues)
        {
            this._transactionActionsProcessorService.ReverseUpdateTransactionInDictionaryFormat(
                originalTransactionGuid,
                updatedTransactionUserValues);
        }

        public TransactionViewDTO GetTransaction(Guid transactionGuid)
        {
            //retrieve the transactiondo
            var sr = new TransactionSR { TransactionGuid = transactionGuid };
            var transaction = this._transactionProcessorProxy.Process(sr);
            //get the pipeline and run the transaction thru it
            var submittedTransactionAlias = this._transactionAliasProxy.Get(transaction.TransactionAliasGuid, true);
            var outboundPipeline = this._transactionPipeline.Outbound();
            foreach (var pipe in outboundPipeline)
            {
                pipe.Execute(transaction, submittedTransactionAlias);
            }
            var result = new TransactionViewDTO();
            result.TransactionPropertyValuePairs = this._createTransactionObjectService.CreateTransactionFromDataObject(transaction);
            result.TransactionAliasGuid = transaction.TransactionAliasGuid.ToString();
            result.CanBeEdited = this._transactionPossibleActionsService.CanTransactionBeEdited(transaction);
            result.CanBeReversed = result.CanBeEdited ? false : this._transactionPossibleActionsService.CanTransactionBeReversed(transaction);
            result.ReversalType = WasThisTransactionReversed(transaction);
            return result;
        }

        private string WasThisTransactionReversed(TransactionDO transaction)
        {
            if (transaction.ReversalType == TransactionDO.Original)
            {
                return "ReversalOrigin";
            }
            if (transaction.ReversalType == TransactionDO.Reversal)
            {
                return "ReversalResult";
            }

            return null;
        }

        /// <summary>
        /// Reverse the transaction.
        /// </summary>
        public void ReverseTransaction(Guid transactionGuid)
        {
            this._transactionActionsProcessorService.ReverseTransaction(transactionGuid);
        }

        public TransactionDetailsDTO GetTransactionDetails(
            string transactionAliasGuid)
        {

            var parsedTransactionAliasGuid = Guid.Parse(transactionAliasGuid);
            var transactionAlias = this._transactionAliasProxy.Get(parsedTransactionAliasGuid, false);
            var site = this._currentRequestContext.GetCurrentSite();
            var security = this._currentRequestContext.GetCurrentSecurityContext();
            var result = new TransactionDetailsDTO();
            result.TransactionFields = this._transactionFieldsService.GeTransactionFieldDefinitionsForUI(transactionAlias);
            result.AutoDocumentNumber = this._autoDocumentNumberService.HasAutoDocumentNumberAvaliable(transactionAlias, site);
            result.FieldsWithLists = this.GetTransactionAssociatedLists(transactionAlias, result.TransactionFields);
            AnnotateFieldsWithLists(result);
            var unitHelper = new UnitsHelperClass(security, site, transactionAlias, null);
            result.VolumeDecimalPrecision = unitHelper.VolumeDecimalPlaces;
            result.TemperatureDecimalPlaces = unitHelper.TemperatureDecimalPlaces;
            result.DensityDecimalPlaces = unitHelper.DensityDecimalPlaces;
            result.AllProducts = this.GetProducts(security, site, transactionAlias);
            result.TransactionAliasType = transactionAlias.TransTypeID;
            return result;
        }

        private static void AnnotateFieldsWithLists(TransactionDetailsDTO result)
        {
            foreach (var fieldWithList in result.FieldsWithLists)
            {
                var hasList = result.TransactionFields.SingleOrDefault(x => x.ID == fieldWithList.FieldName);
                if (hasList != null &&
                    hasList.ColumnDefinition != null)
                {
                    hasList.ColumnDefinition.HasListAttached = true;
                }
            }
        }

        private IEnumerable<ProductDTO> GetProducts(SecurityClass security, SiteClass site, TransactionAliasClass transactionAlias)
        {
            var retrievedProducts = this._productProxy.Enumerate().ToList();
            var result = new List<ProductDTO>();
            foreach (var product in retrievedProducts)
            {
                var unitHelper = new UnitsHelperClass(security, site, transactionAlias, product);
                result.Add(new ProductDTO()
                    {
                        ID = product.ID,
                        VolumeDecimalPlaces = unitHelper.VolumeDecimalPlaces,
                        TemperatureDecimalPlaces = unitHelper.TemperatureDecimalPlaces,
                        DensityDecimalPlaces = unitHelper.DensityDecimalPlaces
                    });
            }

            return result;
        }

        public IEnumerable<TransactionAliasFieldClassWithColumn> GetTransactionDetails(TransactionAliasClass transactionAlias)
        {
            var results = this._transactionFieldsService.GeTransactionFieldDefinitionsForUI(transactionAlias);
            return results;
        }

        public IEnumerable<FieldWithAssociatedList> GetTransactionAssociatedLists(
            string transactionAliasGuid,
            IEnumerable<TransactionAliasFieldClassWithColumn> fields = null)
        {
            var parsedTransactionAliasGuid = Guid.Parse(transactionAliasGuid);
            var transactionAlias = this._transactionAliasProxy.Get(parsedTransactionAliasGuid, true);
            return GetTransactionAssociatedLists(transactionAlias, fields);
        }

        public IEnumerable<FieldWithAssociatedList> GetTransactionAssociatedLists(
            TransactionAliasClass transactionAlias,
            IEnumerable<TransactionAliasFieldClassWithColumn> fields = null)
        {
            if (fields == null)
            {
                fields = this._transactionFieldsService.GeTransactionFieldDefinitionsForUI(transactionAlias);
            }

            var result = new List<FieldWithAssociatedList>();
            foreach (var field in fields)
            {
                switch (field.ID)
                {
                    case "Product":
                        var products = this.GetProductOptions();
                        result.Add(new FieldWithAssociatedList()
                        {
                            FieldName = "Product",
                            Options = products
                        });
                        break;
                    case "ManagerID":
                    case "FromManagerID":
                    case "ToManagerID":
                        var managerCompanies = this.GetCompaniesByRole(COMPANY_ROLE.MANAGER);
                        result.Add(new FieldWithAssociatedList()
                        {
                            FieldName = field.ID,
                            Options = managerCompanies
                        });
                        break;
                    case "OwnerID":
                    case "FromOwnerID":
                    case "ToOwnerID":
                        var ownerCompanies = this.GetCompaniesByRole(COMPANY_ROLE.OWNER);
                        result.Add(new FieldWithAssociatedList()
                        {
                            FieldName = field.ID,
                            Options = ownerCompanies
                        });
                        break;
                    case "CarrierID":
                    case "FromCarrierID":
                    case "ToCarrierID":
                        var carrierCompanies = this.GetCompaniesByRole(COMPANY_ROLE.CARRIER);
                        result.Add(new FieldWithAssociatedList()
                        {
                            FieldName = field.ID,
                            Options = carrierCompanies
                        });
                        break;
                    case "ShipToID":
                        var customerCompanies = this.GetCompaniesByRole(COMPANY_ROLE.CUSTOMER_SHIPTO);
                        result.Add(new FieldWithAssociatedList()
                                   {
                                       FieldName = "ShipToID",
                                       Options = customerCompanies
                                   });
                        break;
                    case "SupplierID":
                        var supplierCompanies = this.GetCompaniesByRole(COMPANY_ROLE.SUPPLIER);
                        result.Add(new FieldWithAssociatedList()
                                   {
                                       FieldName = "SupplierID",
                                       Options = supplierCompanies
                                   });
                        break;
                    case "ShipperID":
                        var shipperCompanies = this.GetCompaniesByRole(COMPANY_ROLE.SHIPPER);
                        result.Add(new FieldWithAssociatedList()
                                   {
                                       FieldName = "ShipperID",
                                       Options = shipperCompanies
                        });
                        break;
                    case "Customs":
                        var customsTaxFields = new string[] { "Bonded", "Domestic", "FTZ" };
                        result.Add(new FieldWithAssociatedList()
                        {
                            FieldName = "Customs",
                            Options = customsTaxFields
                        });
                        break;
                    case "LookupTransactionStatusIndex":
                        result.Add(new FieldWithAssociatedList()
                        {
                            FieldName = "LookupTransactionStatusIndex",
                            Options = Enum.GetNames(typeof(TransactionStatus))
                        });
                        break;
                    case "MeterID":
                        var meters = this.GetMetersAssociatedWithTransactionAlias(transactionAlias, field);
                        result.Add(new FieldWithAssociatedList()
                                   {
                                       FieldName = "MeterID",
                                       Options = meters
                                   });
                        break;
                    case "SourceRegistrationID1":
                    case "DestinationRegistrationID1":
                    case "SourceRegistrationID2":
                    case "DestinationRegistrationID2":
                    case "SourceRegistrationID3":
                    case "DestinationRegistrationID3":
                    case "SourceRegistrationID4":
                    case "DestinationRegistrationID4":
                        if (field.ID == "DestinationRegistrationID1" &&
                            !(transactionAlias.TransTypeID == TransactionTypes.T13_OwnerTransfer
                             || transactionAlias.TransTypeID == TransactionTypes.T7_FillStand))
                        {
                            //aviation wants DestinationRegistriationID1 aka "Airplane" to be a write in field except when in a transfer or loadrack situation.
                            break;
                        }
                        var equipments = this.GetEquipmentAssociatedWithTransactionAlias(transactionAlias, field);
                        result.Add(new FieldWithAssociatedList()
                                   {
                                       FieldName = field.ID,
                                       Options = equipments
                        });
                        break;
                    case "StorageLocationID":
                        var tanks = this.GetTanks(transactionAlias, field);
                        result.Add(new FieldWithAssociatedList()
                        {
                            FieldName = field.ID,
                            Options = tanks
                        });
                        break;
                }
            }

            return result;
        }

        private IEnumerable<string> GetTanks(TransactionAliasClass transactionAlias, TransactionAliasFieldClassWithColumn field)
        {
            return this._tanksProxy.Enumerate(true).Select(x => x.ID);
        }

        private byte GetRegistrationCount(string fieldName)
        {
            var lastCharacterInFieldName = fieldName.Substring(fieldName.Length - 1);
            int parsedFieldCount;
            if (!int.TryParse(lastCharacterInFieldName, out parsedFieldCount))
            {
                throw new NotSupportedException("Field name passed in is not a number");
            }
            return Convert.ToByte(parsedFieldCount);
        }

        private bool IsDestinationRegistration(string fieldName)
        {
            return fieldName.Contains("Destination");
        }

        private IEnumerable<string> GetEquipmentAssociatedWithTransactionAlias(TransactionAliasClass transactionAlias, TransactionAliasFieldClassWithColumn field)
        {
            var equipmentTypes = transactionAlias.GetEquipmentTypes(IsDestinationRegistration(field.ID), GetRegistrationCount(field.ID));
            var matchingEquipment = new List<EquipmentClass>();
            var allEquipment = this._equipmentsProxy.Enumerate();
            foreach (var equipment in allEquipment)
            {
                if (equipmentTypes.Contains(equipment.Type))
                {
                    matchingEquipment.Add((equipment));
                }
            }

            return matchingEquipment.Select(x => x.ID).ToList();
        }

        private IEnumerable<string> GetMetersAssociatedWithTransactionAlias(TransactionAliasClass transactionAlias, TransactionAliasFieldClassWithColumn field)
        {
            //we dont know the sourceequipment at this point, default to SourceEquipment1
            var equipmentTypes = transactionAlias.GetEquipmentTypes(false, 1);
            var matchingEquipment = new List<EquipmentClass>();
            var allEquipment = this._equipmentsProxy.Enumerate();
            foreach (var equipment in allEquipment)
            {
                if (equipmentTypes.Contains(equipment.Type))
                {
                    matchingEquipment.Add((equipment));
                }
            }

            var meters = new List<string>();
            meters = this._meterProxy.GetMeterIdsByAssetGuids(matchingEquipment);
            return meters;
        }

        public TransactionAliasFieldPlacementDTO GetPlacementInfo(string transactionAliasGuid)
        {
            Guid parsedGuid;
            if (!Guid.TryParse(transactionAliasGuid, out parsedGuid))
            {
                throw new ArgumentException("Cannot convert guid");
            }
            var fieldPlacment = this._transansactionFieldPlacementProxy.GetByTransactionAlias(parsedGuid);
            return new TransactionAliasFieldPlacementDTO()
            {
                TransactionAliasGuid = fieldPlacment?.TransactionAliasGuid ?? Guid.Empty,
                PlacementInformation = fieldPlacment?.PlacementInformation
            };
        }

        public void SavePlacementInfo(TransactionAliasFieldPlacementDTO toSave)
        {
            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            var fieldPlacment = this._transansactionFieldPlacementProxy.AddOrUpdate(new TransactionAliasFieldPlacementInformationClass()
            {
                TransactionAliasGuid = toSave.TransactionAliasGuid,
                PlacementInformation = toSave.PlacementInformation,
                CreatedBy = userSecurity.UserID,
                UpdatedBy = userSecurity.UserID
            });
        }

        private IEnumerable<string> GetCompaniesByRole(COMPANY_ROLE role)
        {
            var result = new List<string>();
            var companies = this._companyProxy.EnumerateByRole(role, false, false, false);
            foreach (var company in companies)
            {
                if (!company.LockedOut)
                {
                    result.Add(company.ID);
                }
            }
            return result;
        }

        private IEnumerable<string> GetProductOptions()
        {
            var results = new List<string>();
            var products = this._productProxy.Enumerate().ToList();
            foreach (var product in products)
            {
                if (product.InhibitAccounting)
                {
                    continue;
                }
                results.Add(product.ID);
            }
            return results;
        }

        public void DeleteTransaction(string transactionGuid)
        {
            Guid parsedTransactionGuid;
            if (!Guid.TryParse(transactionGuid, out parsedTransactionGuid))
            {
                throw new NotSupportedException("Cannot parse the submitted transaction alias guid");
            }
            this._transactionActionsProcessorService.DeleteTransaction(parsedTransactionGuid);
        }
    }
}
