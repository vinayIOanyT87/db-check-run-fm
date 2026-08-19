using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class TankListProcessorClass : ITankListProcessor
	{
		#region Private Attributes
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the month/year processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public TankListProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public Override methods
		/// <summary>
		/// This method starts the processing of gathering all the data for the month/year
		/// dates.
		/// </summary>
		/// <param name="tankListSR"></param>
		/// <returns></returns>
		public TankListDO Process ( TankListSR tankListSR )
		{
			TankListDO tankListDO = new TankListDO ( );
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				if (string.IsNullOrEmpty(tankListSR.ProductId) == true)
				{
					tankListDO.retrieveTankSelectSql(cmd, tankListSR.ManagerId);
				}
				else
				{
					tankListDO.retrieveTankSelectSqlForProduct(cmd, tankListSR.ProductId, tankListSR.ManagerId);
				}

				dataSet = this.consolidatedDA.GetDataSet(cmd, tankListSR.Security);

				if (dataSet != null)
				{
					tankListDO.loadTankListData(dataSet);
				}

				return tankListDO;
			}
		}
		#endregion
	}
}