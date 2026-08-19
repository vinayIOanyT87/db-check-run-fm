// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonthYearProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The month year processor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// The month year processor class.
	/// </summary>
	public class MonthYearProcessor : IMonthYearProcessor
	{
		#region Private Attributes

		/// <summary>
		/// The consolidated DA object for database access.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA;

		/// <summary>
		/// The month year SR object
		/// </summary>
		private MonthYearSR monthYearSR;

		/// <summary>
		/// The month year DO object.
		/// </summary>
		private MonthYearDO monthYearDO;

		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="MonthYearProcessor"/> class. 
		/// This is the default constructor for the month/year processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public MonthYearProcessor ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method starts the processing of gathering all the data for the month/year
		/// dates.
		/// </summary>
		/// <param name="inMonthYearSR">The in month year SR.</param>
		/// <returns>
		/// A MonthYearDO object for use in populating the ledger dropdown.
		/// </returns>
		public MonthYearDO Process ( MonthYearSR inMonthYearSR )
		{
			this.monthYearSR = inMonthYearSR;
			this.monthYearDO = new MonthYearDO();

			string sql;
            switch(inMonthYearSR.DeterminationType)
            {
                case MonthYearSR.MonthYearDetermiationType.Standard:
                    sql = this.monthYearDO.RetrieveMonthYearSelectSQL();
                    break;
                case MonthYearSR.MonthYearDetermiationType.EndOfMonth:
                    sql = "dbo.usp_MonthYearList";
                    break;
                default:
                    sql = this.monthYearDO.RetrieveMonthYearSelectSQL();
                    break;
            }

			DataSet dataSet; 
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = sql;
				dataSet = this.consolidatedDA.GetDataSet( cmd, this.monthYearSR.Security);
			}

			if (dataSet != null)
			{
                if (inMonthYearSR.DeterminationType == MonthYearSR.MonthYearDetermiationType.EndOfMonth)
				{
					DataTable table = dataSet.Tables[0];

					foreach (DataRow row in table.Rows)
					{
						string month = row[0].ToString ( );
						this.monthYearDO.AddYear ( month );
						this.monthYearDO.MonthList.Add ( month.Substring ( 0, month.Length - 5 ) );
					}
				}
				else
				{
					this.monthYearDO.LoadMonthYearData ( dataSet );
				}

				if (this.monthYearSR.UseDataDictionary)
				{
					this.monthYearDO.CombinedList = this.ApplyDictionaryToMonths ( this.monthYearSR.Security, this.monthYearDO.MonthList, this.monthYearDO.YearList );
				}
			}

			return this.monthYearDO;
		}

		/// <summary>
		/// This method will run through the list of months and process each month name
		/// through the data dictionary. It will build the month/year string (i.e. June 2004)
		/// and return the new list.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="monthList">The month list.</param>
		/// <param name="yearList">The year list.</param>
		/// <returns>A drop down item collection.</returns>
		private DropdownItemCollectionClass ApplyDictionaryToMonths ( SecurityClass security, ArrayList monthList, ArrayList yearList )
		{
			var dictionary = new DataDictionariesClass();
			var combined = new DropdownItemCollectionClass();

			var translatedMonths = new string[12];

			for ( int nextMonth = 0; nextMonth < monthList.Count; nextMonth++ )
			{
				if ( nextMonth < 12 )
				{
					translatedMonths[nextMonth] = dictionary.Get(security.SiteGuid, (string) monthList[nextMonth] );
				}

				var item = new DropdownItem 
				{
					Text = translatedMonths[nextMonth % 12] + " " + yearList[nextMonth],
					TextValue = monthList[nextMonth] + " " + yearList[nextMonth]
				};

				combined.Add( item );
			}

			return combined;
		}
		#endregion
	}
}