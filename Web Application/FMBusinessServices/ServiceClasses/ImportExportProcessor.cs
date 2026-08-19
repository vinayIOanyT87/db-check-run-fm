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
	public class ImportExportProcessorClass : IImportExportProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the closeout processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public ImportExportProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public Methods
		public DataObject Process ( ImportExportSR importExportSR )
		{
			return null;
		}
		#endregion

		#region Private Methods
		private void browse ( )
		{
		}

		private void manageConfigurations ( )
		{
		}

		private void import ( )
		{
		}

		private void export ( )
		{
		}
		#endregion
	}
}