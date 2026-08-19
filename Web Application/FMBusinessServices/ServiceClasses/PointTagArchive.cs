namespace FMBusinessServices.ServiceClasses
{
	using System.Collections.Generic;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ChannelFactories;

	using DataAccessLayer;
	using InternalInterfaces;
	using System;
	using Cassandra;

	[SecuritySafeCritical]
	[ServiceBehavior( TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted )]
	public class PointTagArchive : FMServiceBase, IPointTagArchive
	{
		private static readonly IPointTagArchiveDatabase PointTagArchiveDatabase = new PointTagArchiveDatabase();
	
		public void InitializeArchive(SecurityClass security)
		{
			// TOOD: Check security rights

			PointTagArchiveDatabase.Initialize(security);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddArchiveData(SecurityClass security, List<ArchiveDataElement> archiveDataElementList)
		{
			// TOOD: Check security rights

			PointTagArchiveDatabase.AddArchiveData(security, archiveDataElementList);
		}


		public List<List<TrendArchiveDataElement>> GetTrendArchiveData(SecurityClass security, List<Guid> tagGuids, DateTimeOffset start, DateTimeOffset end)
		{
			// TOOD: Check security rights
			var pointTagGuidsFilteredbyPointAccess = FMChannelHelper.MakeCall<IPointTags, List<Guid>>(x => x.EnumerateTagListByPointAccess(security, tagGuids));
			List<Guid> tagGuidsToRequest = new List<Guid>();
			foreach (Guid tagGuid in tagGuids)
			{
				if (!pointTagGuidsFilteredbyPointAccess.Contains(tagGuid))
				{
					//send an empty request so we have a place holder for the trend
					tagGuidsToRequest.Add(Guid.Empty);
				}
				else
				{
					tagGuidsToRequest.Add(tagGuid);
				}
			}
			return PointTagArchiveDatabase.GetTrendArchiveData(security, tagGuidsToRequest, start, end);

		}

        public List<List<TrendArchiveDataElement>> GetHistoryArchiveData(SecurityClass security, List<Guid> tagGuids, DateTimeOffset start, DateTimeOffset end, int numberOfSamplesPerPen)
        {
            // TOOD: Check security rights
            var pointTagGuidsFilteredbyPointAccess = FMChannelHelper.MakeCall<IPointTags, List<Guid>>(x => x.EnumerateTagListByPointAccess(security, tagGuids));
            List<Guid> tagGuidsToRequest = new List<Guid>();
            foreach (Guid tagGuid in tagGuids)
            {
                if (!pointTagGuidsFilteredbyPointAccess.Contains(tagGuid))
                {
                    //send an empty request so we have a place holder for the trend
                    tagGuidsToRequest.Add(Guid.Empty);
                }
                else
                {
                    tagGuidsToRequest.Add(tagGuid);
                }
            }
            return PointTagArchiveDatabase.GetHistoryArchiveData(security, tagGuidsToRequest, start, end, numberOfSamplesPerPen);

        }

		public List<SimpleArchiveDataElement> GetArchiveDataValues(SecurityClass security, List<Guid> tagGuids, DateTimeOffset start, DateTimeOffset end)
		{
			return PointTagArchiveDatabase.GetArchiveDataValues(security, tagGuids, start, end);
		}

	}
}
