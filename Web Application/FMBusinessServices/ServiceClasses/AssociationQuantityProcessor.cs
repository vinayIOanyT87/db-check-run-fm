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
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.LogClient;

namespace FMBusinessServices.ServiceClasses
{
	public class AssociationQuantityProcessorClass : IAssociationQuantityProcessor
	{
		#region Attributes
		private Logger logger;
		private ConsolidatedDAClass consolidatedDA;
		#endregion // Attributes

		#region Construction
		public AssociationQuantityProcessorClass ( )
		{
			this.logger = new Logger ( "AssociationQuantityProcessorClass" ); ;
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion // Construction

		#region Public methods
		public AssociationQuantityDO Process( AssociationQuantitySR accountingSR )
		{
			AssociationQuantitySR sr = (AssociationQuantitySR) accountingSR;

			StopWatch timer = new StopWatch ( StopWatch.Appnames.AccountingBLL, "***###*** RetrieveAssociatedQuantity() Main Call" );
			AssociationQuantityDO results = this.RetrieveAssociatedQuantity ( sr );
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

		#region Private Methods
		private  AssociationQuantityDO RetrieveAssociatedQuantity ( AssociationQuantitySR sr )
		{
			AssociationQuantityDO results = new AssociationQuantityDO ( );

			try
			{
				// the commands table lets us reuse queries, improving performance
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandText = "EXEC dbo.fm_GetParentAssociationDetails " +
								  "@Product, @TransID, @ParentTypeID, @ChildTypeID";

					cmd.Parameters.Add( "@Product", SqlDbType.NVarChar, 64 );
					cmd.Parameters.Add( "@TransID", SqlDbType.NVarChar, 64 );
					cmd.Parameters.Add( "@ParentTypeID", SqlDbType.SmallInt );
					cmd.Parameters.Add( "@ChildTypeID", SqlDbType.SmallInt );

					cmd.Prepare();

					// check for correctness 
					if (sr.Validate() == false)
					{
						throw new Exception( "the input " + typeof( AssociationQuantitySR ).ToString() + " is malformed" );
					}

					// bind the data
					int index = -1;

					// use pre-increment for 30% speed increase
					cmd.Parameters[++index].Value = sr.Product;
					cmd.Parameters[++index].Value = sr.ChildTransID;
					cmd.Parameters[++index].Value = (short)sr.ParentTypeID;
					cmd.Parameters[++index].Value = sr.ChildTypeID;

					// get results
					DataSet dataSet = this.consolidatedDA.GetDataSet( cmd, sr.Security );

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							DataRow row = dataTable.Rows[0];

							if (row["TotalQuantity"] != System.DBNull.Value)
							{
								++results.SavedCount;
								results.TotalQuantity	= DataObject.getDouble( row["TotalQuantity"] );
								results.ProductPrice	= DataObject.getDouble( row["ProductPrice"] );
								results.Excise			= DataObject.getDouble( row["Excise"] );
								results.GST				= DataObject.getDouble( row["GST"] );
								results.MarkUp			= DataObject.getDouble( row["MarkUp"] );
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				results.Errors.Add ( new AccountingServicesException ( e.Message ) );
			}

			return results;
		}
		#endregion // Class Methods
	}
}