namespace FuelsManagerService.Record_Version_Propagation
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    class RecordVersionPropagation
    {
        public static void PerformRecordVersionPropagation(SecurityClass security)
        {
            FMChannelHelper.MakeCall<ISites, bool>(x => x.ApplyGlobalRecordVersionUpdates(security));
        }
    }
}
