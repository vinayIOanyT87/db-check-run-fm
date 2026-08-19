/// <summary>
/// File name:	TransactionConfigProcessor.cs
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
	public class TransactionConfigProcessorClass : ITransactionConfigProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		private TransactionConfigSR transConfigSR;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the transaction configuration processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public TransactionConfigProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public Methods
		public DataObject Process ( TransactionConfigSR sr )
		{
			transConfigSR = sr;
			return null;
		}
		#endregion
	}
}