using System.Security;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[SecuritySafeCriticalAttribute]
	public class TFMServicesClass : ITFMSServices
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the TFMS class.
		/// </summary>
		public TFMServicesClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will return True if an existing purchase number for a given
		/// transaction type of 12.  Otherwise, a false is returned.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="siteIndex"></param>
		/// <param name="purchaseNumber"></param>
		/// <returns></returns>
		public bool IsDirectPurchaseNumberUnique ( SecurityClass security, string purchaseNumber )
		{
			bool unique = false;

			if (( purchaseNumber != null ) && ( purchaseNumber.Length > 0 ))
			{
				try
				{
					TFMSDO tfmsDO   = new TFMSDO ( );
					using (SqlCommand cmd = new SqlCommand())
					{
						tfmsDO.GetPurchaseNumberCount(cmd, purchaseNumber);
						DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

						if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
						{
							DataTable dataTable = dataSet.Tables[0];

							if (dataTable.Rows != null)
							{
								DataRow dataRow = dataTable.Rows[0];
								int count = DataObject.getValue<int>(dataRow["PurchaseNumberCount"], 0);

								if (count <= 0)
								{
									unique = true;
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception ( "An error occurred attempting to retrieve excise tax record from the database.  " + ex.Message );
				}
			}

			return unique;
		}
		#endregion
	}
}