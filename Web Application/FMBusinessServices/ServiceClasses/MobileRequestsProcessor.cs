using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using System.Data.SqlClient;

namespace FMBusinessServices.ServiceClasses
{
	public class MobileRequestsProcessor : IMobileRequestsProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the get transaction processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public MobileRequestsProcessor()
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Methods
		public TransactionSelectionCollectionDO GetTransactionSelection(SecurityClass security, 
													string operatorID,
													bool filterByOperatorID,
													string vehicleID,
													bool filterByVehicleID,
													string gateID,
													bool filterByGateID,
													int hoursInPast,
													int hoursInFuture)
		{
			TransactionSelectionCollectionDO ret = new TransactionSelectionCollectionDO();
			using (SqlCommand sqlcommand = new SqlCommand())
			{
				ret.Get(sqlcommand, security, operatorID, filterByOperatorID, vehicleID,
							filterByVehicleID, gateID, filterByGateID, hoursInPast, hoursInFuture);
				var dataSet = this.consolidatedDA.GetDataSet(sqlcommand, security);

				ret.Load(dataSet);
			}
			return ret;
		}

		public TransactionLineItemSelectionCollectionDO GetTransactionLineItemSelection(SecurityClass security,
													string operatorID,
													bool filterByOperatorID,
													string vehicleID,
													bool filterByVehicleID,
													string gateID,
													bool filterByGateID,
													int hoursInPast,
													int hoursInFuture)
		{
			TransactionLineItemSelectionCollectionDO ret = new TransactionLineItemSelectionCollectionDO();
			using (SqlCommand sqlcommand = new SqlCommand())
			{
				ret.Get(sqlcommand, security, operatorID, filterByOperatorID, vehicleID,
							filterByVehicleID, gateID, filterByGateID, hoursInPast, hoursInFuture);
				var dataSet = this.consolidatedDA.GetDataSet(sqlcommand, security);

				ret.Load(dataSet);
			}
			return ret;
		}
		#endregion
	}
}