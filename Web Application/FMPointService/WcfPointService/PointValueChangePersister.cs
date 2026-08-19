namespace FMPointService.WcfPointService
{
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;


    internal class PointValueChangePersister 
	{
		public void PersistTagToDatabase(SecurityClass security, PointTag pointTag)
		{
			var pointTagList = new List<PointTag>();
			pointTagList.Add(pointTag);
			FMChannelHelper.MakeCall<IPointTags>( x => x.ModifyTagValues( security, pointTagList, false) );
		}

		public void PersistPropertyToDatabase(SecurityClass security, PointProperty pointProperty)
		{
			FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(security, pointProperty, true, true));
		}

	}
}
