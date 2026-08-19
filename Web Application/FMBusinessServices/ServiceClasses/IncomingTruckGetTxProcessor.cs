using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;

namespace FMBusinessServices.ServiceClasses
{
	public class IncomingTruckGetTxProcessorClass : IIncomingTruckGetTxProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public IncomingTruckGetTxProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		public IncomingTruckGetTxDO Process ( IncomingTruckGetTxSR sr )
		{
			this.CheckSR ( sr );

			IncomingTruckGetTxDO incomingTruckGetTxDO = new IncomingTruckGetTxDO ( );

			DataSet dataSet = null;
			using (SqlCommand cmd = sr.GetSQL())
			{
				dataSet = this.consolidatedDA.GetDataSet(cmd, sr.Security);
			}
			incomingTruckGetTxDO.Items = dataSet;

			return incomingTruckGetTxDO;
		}

		private void CheckSR ( IncomingTruckGetTxSR sr )
		{
			if (sr.Security == null)
			{
				throw new ArgumentNullException ( "Security" );
			}

			if (sr.IATAGuid == Guid.Empty)
			{
				throw new ArgumentOutOfRangeException("IATAGuid");
			}
		}
	}
}