namespace FMPointService.Archiving
{
    using System.Collections.Generic;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;
    using FMCore;


	internal class ArchiveDataSaver 
	{
		public void SaveArchiveData(SecurityClass security, List<ArchiveDataElement> archiveDataElementList)
		{
            security.ThrowIfNull("security");
            archiveDataElementList.ThrowIfNull("archiveDataElementList");

			FMChannelHelper.MakeCall<IPointTagArchive>(x => x.AddArchiveData(security, archiveDataElementList));
		}
	}
}
