// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFuelOrderReceiptedLineItemsProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GetFuelOrderReceiptedLineItemsProcessorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// The get fuel order receipted line items processor class.
	/// </summary>
	public class GetFuelOrderReceiptedLineItemsProcessorClass : IGetFuelOrderReceiptedLineItemsProcessor
	{
		#region Private data members
		/// <summary>
		/// The consolidated data layer.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Construction
		/// <summary>
		/// Initializes a new instance of the <see cref="GetFuelOrderReceiptedLineItemsProcessorClass"/> class.
		/// </summary>
		public GetFuelOrderReceiptedLineItemsProcessorClass ( )
		{
			this.consolidatedDa = new ConsolidatedDAClass ( );
		}
		#endregion // Construction

		#region Public methods

		/// <summary>
		/// The process.
		/// </summary>
		/// <param name="inSr">
		/// The service request.
		/// </param>
		/// <returns>
		/// The <see cref="GetFuelOrderReceiptedLineItemsDO"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// Error retrieving fuel order line item data object.
		/// </exception>
		public GetFuelOrderReceiptedLineItemsDO Process ( GetFuelOrderReceiptedLineItemsSR inSr )
		{
			GetFuelOrderReceiptedLineItemsSR sr = inSr;

			if (null == sr)
			{
				return null; // failsafe
			}

			var result = new GetFuelOrderReceiptedLineItemsDO ( );

			using (var cmd = new SqlCommand())
			{
				// TODO: Could not find this SP!!!
				cmd.CommandText = "EXEC fm_ADF_FuelOrderReceiptedLineItems @TransID";

				cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 60);
				cmd.Parameters["@TransID"].Value = sr.TransID;

				// execute the SP
				DataSet dataSet;

				try
				{
					dataSet = this.consolidatedDa.GetDataSet(cmd, sr.Security);
				}
				catch (Exception e)
				{
					throw e;
				}

				// process results
				if ((dataSet != null) && (dataSet.Tables.Count > 0))
				{
					DataTable dataTable = dataSet.Tables[0];

					if (dataTable.Rows != null)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							Guid lineItemGuid = DataObject.getGuid(row[0]);
							result.AddLineItemGuid(lineItemGuid);
						}
					}
				}
			}

			return result;
		}
		#endregion
	}
}