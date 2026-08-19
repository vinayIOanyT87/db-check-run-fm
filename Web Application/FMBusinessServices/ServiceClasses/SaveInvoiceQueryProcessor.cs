using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.LogClient;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SaveInvoiceQueryProcessorClass : ISaveInvoiceQueryProcessor
	{
		#region Attributes
		protected Logger logger;
		protected InvoiceQueryDBI queryDBI;
		protected Object singleton = new Object ( );
		#endregion // Attributes

		#region Constructors
		public SaveInvoiceQueryProcessorClass ( )
		{
			this.logger = new Logger ( "Save Invoice Query Processor" );
		}
		#endregion // Constructors

		#region Public methods
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public CustomResultDO Process ( SaveInvoiceQuerySR sr )
		{
			StopWatch timer = new StopWatch ( StopWatch.Appnames.AccountingBLL, "***###*** SaveInvoiceQueries() Main Call" );
			CustomResultDO results = this.SaveInvoiceQueries ( sr );
			timer.Stop ( );

			// write any errors or warnings to the application event log
			timer.Start ( "***###*** WriteLog() Main Call (errors)" );
			CustomResultDO.WriteLog ( "Accounting BLL", results.Errors, EventLogEntryType.Error );
			timer.Stop ( );

			timer.Start ( "***###*** WriteLog() Main Call (warnings)" );
			CustomResultDO.WriteLog ( "Accounting BLL", results.Warnings, EventLogEntryType.Warning );
			timer.Stop ( );

			// if there has been errors, throw them
			if (results.Errors.Count > 0)
			{
				throw new AccountingServicesException ( "errors in " + MethodBase.GetCurrentMethod ( ).ToString ( ) + " check event log for more details" );
			}

			return results;
		}
		#endregion

		#region Protected operations
		protected CustomResultDO SaveInvoiceQueries ( SaveInvoiceQuerySR sr )
		{
			CustomResultDO result = new CustomResultDO ( );

			lock (singleton)
			{
				using (InvoiceQueryDBI queryDBI = new InvoiceQueryDBI(sr.Security, sr.Security.UserID, DateTimeOffset.Now))
				{
					result.SavedCount = 0;

					// attempt the database write
					try
					{
						foreach (InvoiceQueryDO query in sr.InvoiceQueries)
						{
							StopWatch timer = new StopWatch(StopWatch.Appnames.AccountingBLL, "### InvoiceQueryDBI.Save()");

							queryDBI.Save(query);

							// increase count on success
							++result.SavedCount;

							timer.Stop();
						}
					}
					catch (Exception e)
					{
						result.Errors.Add(new AccountingServicesException(e.Message));
					}
				}
			}

			return result;
		}
		#endregion // Protected operations
	}
}