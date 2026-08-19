/// <summary>
/// File name:	TransactionConfigDetailProcessor.cs
/// Purpose:	To decipher the request to retrieve the transaction configuration
///				data object.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>
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
	public class TransactionConfigDetailProcessorClass : ITransactionConfigDetailProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		private TransactionConfigDetailSR transConfigDetailSR;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the closeout processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		/// <param name="accountingServiceImpl"></param>
		public TransactionConfigDetailProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public Methods
		public DataObject Process ( TransactionConfigDetailSR sr )
		{
			this.transConfigDetailSR = sr;

			// Call the appropriate private methods...
			return null;
		}
		#endregion

		#region Private Methods
		private DataObject getStandardFields ( )
		{
			return null;
		}

		private DataObject getCustomFields ( )
		{
			return null;
		}

		private DataObject getProductExclusion ( )
		{
			return null;
		}

		private DataObject getResetStandardFields ( )
		{
			return null;
		}

		private DataObject getDefaultStandardFields ( )
		{
			return null;
		}

		private DataObject applyCustomFields ( )
		{
			return null;
		}

		private void saveConfiguration ( )
		{
		}
		#endregion
	}
}