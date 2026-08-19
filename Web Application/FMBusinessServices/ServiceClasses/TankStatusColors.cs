using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.BusinessInterfaces;
namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TankStatusColorsClass : ITankStatusColors
	{
		#region Constructors
		public TankStatusColorsClass()
		{
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]

		public TankStatusColorsCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var tankStatusColor = new TankStatusColorClass();

			var tankStatusColorsCollection = new TankStatusColorsCollectionClass();

			tankStatusColorsCollection.Add(tankStatusColor);

			return tankStatusColorsCollection;
		}

	}
}