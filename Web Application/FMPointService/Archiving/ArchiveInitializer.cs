namespace FMPointService.Archiving
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	internal class ArchiveInitializer
	{
		public void Initialize(SecurityClass security)
		{
			security.ThrowIfNull("security");

			FMChannelHelper.MakeCall<IPointTagArchive>(x => x.InitializeArchive(security));
		}
	}
}
