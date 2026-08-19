using FMWebAPIBusinessLogic.DTO;
using FMWebAPIBusinessLogic.Interfaces.Controllers;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using FMWebAPIBusinessLogic.Services.Controllers;
using FMWebAPIBusinessLogic.Services.FMBusinessLogic;
using FMWebAPIBusinessLogic.Services.FMProxy;
using Inbound = FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound;
using Outbound = FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Outbound;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using FMBusinessObjects.BusinessInterfaces;

namespace FMWebAPIBusinessLogic
{
    public static class RegisterFuelMangaerWebAPIServices
    {
        /// <summary>
        /// register all the classes inside of this project using defaults
        /// </summary>
        /// <param name="fmService"></param>
        public static void RegisterFuelManagerWebAPIBusinessServices(this IUnityContainer fmService)
        {
            //fmService.RegisterType<IFMCustomLogger, GenericSerilogEventlogLogger>(new ContainerControlledLifetimeManager()); //singleton
            fmService.RegisterType<ICurrentRequestContext, CurrentRequestContext>(new HierarchicalLifetimeManager()); //singleton to the request
            fmService.RegisterType<ISiteController, SiteController>();
            fmService.RegisterType<ITransactionObjectTranslationService, TransactionObjectTranslationService>();
            fmService.RegisterType<IFMVCFService, FMVCFService>();

            fmService.RegisterType<ISiteProxy, SiteProxy>();
            fmService.RegisterType<ICompanyProxy, CompanyProxy>();
            fmService.RegisterType<IEquipmentsProxy, EquipmentsProxy>();
            fmService.RegisterType<IPersonnelProxy, PersonnelProxy>();
            fmService.RegisterType<IProductsProxy, ProductsProxy>();
            fmService.RegisterType<ITransactionAliasFieldsProxy, TransactionAliasFieldsProxy>();
            fmService.RegisterType<ITransactionAliasesProxy, TransactionAliasesProxy>();
            fmService.RegisterType<ITransactionAliasFieldPlacementInformationProxy, TransactionAliasFieldPlacementInformationProxy>();
            fmService.RegisterType<IErrorTransactionSubmissionProxy, ErrorTransactionSubmissionProxy>();
            fmService.RegisterType<ITransactionFieldsService, TransactionFieldsService>();
            fmService.RegisterType<IFMWebAPIConfigurationFactory, FMWebAPIConfigurationFactory>();
            fmService.RegisterType<ISaveTransactionsProcessorProxy, SaveTransactionsProcessorProxy>();
            fmService.RegisterSingleton<FMWebAPIConfiguration>(new InjectionFactory(c => c.Resolve<IFMWebAPIConfigurationFactory>().GetConfig()));
            fmService.RegisterType<IMetersProxy, MetersProxy>();
            fmService.RegisterType<IEquipmentTypesProxy, EquipmentTypesProxy>();
            fmService.RegisterType<ITransactionProcessorProxy, TransactionProcessorProxy>();
            fmService.RegisterType<IAutoDocumentNumberService, AutoDocumentNumberService>();
            fmService.RegisterType<IMeterActionService, MeterActionService>();
            fmService.RegisterType<ITransactionPossibleActionsService, TransactionPossibleActionsService>();
            fmService.RegisterType<ITransactionActionsProcessorsService, TransactionActionsProcessorService>();
            fmService.RegisterType<IProxySecurityFactory, ProxySecurityFactory>();

            fmService.RegisterType<ITanks>(new InjectionFactory(x => new FMChannelProxyAOPWrapper<ITanks>().GetTransparentProxy()));
            fmService.RegisterType<IProducts>(new InjectionFactory(x => new FMChannelProxyAOPWrapper<IProducts>().GetTransparentProxy()));

            fmService.RegisterType<ITanksProxy>(new InjectionFactory(x => new FMSecurityClassInjector<ITanksProxy, ITanks>(x.Resolve<ITanks>(), 
                x.Resolve<IProxySecurityFactory>()).GetTransparentProxy()));

            fmService.RegisterType<ITransactionPipeline, TransactionPipelineAviation>();
            fmService.RegisterType<Inbound.TransactionAliasResolver>();
            fmService.RegisterType<Inbound.ProductGuidResolver>();
            fmService.RegisterType<Inbound.CompanyGuidResolver>();
            fmService.RegisterType<Inbound.MeterGuidResolver>();
            fmService.RegisterType<Inbound.EquipmentGuidResolver>();
            fmService.RegisterType<Inbound.AssignEquipmentToMeterGuidResolver>();
            fmService.RegisterType<Inbound.PersonnelGuidResolver>();
            fmService.RegisterType<Inbound.TransactionIssueConverter>();
            fmService.RegisterType<Inbound.Transaction24HourConverter>();
            fmService.RegisterType<Inbound.TransactionRotationConverter>();
            fmService.RegisterType<Inbound.TransactionDefuelConverter>();
            fmService.RegisterType<Inbound.TransactionAdjustmentConverter>();
            fmService.RegisterType<Inbound.TransactionFillStandConverter>();
            fmService.RegisterType<Inbound.TransactionTransferConverter>();
            fmService.RegisterType<Inbound.TransactionFillStandReceiptConverter>();
            fmService.RegisterType<Outbound.IssueTransactionConverter>();
            fmService.RegisterType<Outbound.TransactionTransferConverter>();
            fmService.RegisterType<Inbound.Rotation24HourDestinationEquipmentResolver>();
        }
    }
}
