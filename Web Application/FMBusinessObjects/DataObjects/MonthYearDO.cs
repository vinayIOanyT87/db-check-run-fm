namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Runtime.Serialization;

	using FMBusinessObjects.UtilityObjects;

	[Serializable]
	[DataContract]
	public class MonthYearDO : DataObject
	{
		#region Attributes
		[DataMember]
		private ArrayList monthList;
		[DataMember]
		private ArrayList yearList;
		[DataMember]
		private Hashtable monthsHshTbl;
		#endregion

		#region Contructor
		/// <summary>
		/// This is the default constructor for the month year data object.
		/// </summary>
		public MonthYearDO()
		{
			this.monthsHshTbl = new Hashtable
			                    {
				                    { 1, "January" },
				                    { 2, "February" },
				                    { 3, "March" },
				                    { 4, "April" },
				                    { 5, "May" },
				                    { 6, "June" },
				                    { 7, "July" },
				                    { 8, "August" },
				                    { 9, "September" },
				                    { 10, "October" },
				                    { 11, "November" },
				                    { 12, "December" }
			                    };

			monthList = new ArrayList();
			yearList = new ArrayList();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will be the month/year list using the maximum date and
		/// minimum inventory dates found in the transaction table.
		/// </summary>
		/// <param name="inMaxInvDate"></param>
		/// <param name="inMinInvDate"></param>
		private void MakeMonthYearList(DateTime inMaxInvDate, DateTime inMinInvDate)
		{
			DateTimeOffset maxInvDate = inMaxInvDate;
			DateTimeOffset minInvDate = inMinInvDate;

			this.monthList		= new ArrayList();
			this.yearList		= new ArrayList();
			var monthTempList	= new ArrayList();
			var yearTempList	= new ArrayList();

			if (inMaxInvDate < TimeConverter.Today())
			{
				maxInvDate = TimeConverter.Today();
			}

			int maxYear = Convert.ToInt32(maxInvDate.Year);
			int minYear = Convert.ToInt32(minInvDate.Year);

			// Loop through all the years found.
			for (int nextYear = minYear; nextYear <= maxYear; nextYear++)
			{
				int startMonth = 1;
				int endMonth = 12;

				// For the maximum year, which may be the current year we
				// need to find the last month that we have data on. This
				// month will be the ending month for create the month/year
				// list.
				if (nextYear == maxYear)
				{
					endMonth = Convert.ToInt32(maxInvDate.Month);
				}
				else
				{
					// For the minimum year we need to find the starting month
					// for the data that had been collected.  The data may have
					// started in the middle of the year.
					if (nextYear == minYear)
					{
						startMonth = Convert.ToInt32(minInvDate.Month);
					}
				}

				// Build month/year list for each year and month within the year.
				for (int nextMonth = startMonth; nextMonth <= endMonth; nextMonth++)
				{
					var monthStr = (string)this.monthsHshTbl[nextMonth];
					monthTempList.Add(monthStr);
					yearTempList.Add(Convert.ToString(nextYear));
				}
			}

			// Reverse order the list.
			int tempCount = monthTempList.Count - 1;
			for (int next = tempCount; next >= 0; next--)
			{
				this.monthList.Add(monthTempList[next]);
				this.yearList.Add(yearTempList[next]);
			}
		}
		#endregion

		#region SQL Public Methods
		/// <summary>
		/// This method will return a SQL that will retrieve the max and minimum
		/// inventory dates.
		/// </summary>
		/// <returns>A sql string.</returns>
		public string RetrieveMonthYearSelectSQL()
		{
			return "SELECT MIN(InventoryDate) AS MinInventoryDate, MAX(InventoryDate) AS MaxInventoryDate FROM tblTransactions WITH(NOLOCK)";
		}

		/// <summary>
		/// This mehtod will retrieve the max and min inventory dates from the 
		/// transaction table. It will call a private method to create the month
		/// year list.
		/// </summary>
		/// <param name="dataSet"></param>
		public void LoadMonthYearData(DataSet dataSet)
		{
			if ( dataSet != null )
			{
				DataTable table = dataSet.Tables[0];

				if ( table.Rows.Count > 0 )
				{
					DataRow row = table.Rows[0];

					DateTime? minInventoryDate = row.IsNull("MinInventoryDate") ? null : (DateTime?) row["MinInventoryDate"];
					DateTime? maxInventoryDate = row.IsNull("MaxInventoryDate") ? null : (DateTime?) row["MaxInventoryDate"];

					// If either the max or min dates are null, then default both dates to the current
					// date. If both are not null, then make month year to the selected range.
					if (maxInventoryDate.Equals(null) || minInventoryDate.Equals(null))
					{
						DateTime currentDate = DateTime.Now;
						this.MakeMonthYearList( currentDate, currentDate );
					}
					else
					{
						DateTime maxInventoryDateTime = getValue(maxInventoryDate, TimeConverter.Today().Date);
						DateTime minInventoryDateTime = getValue(minInventoryDate, TimeConverter.Today().Date);

						this.MakeMonthYearList( maxInventoryDateTime, minInventoryDateTime );
					}
				}
				else
				{
					DateTime currentDate = DateTime.Now;
					this.MakeMonthYearList( currentDate, currentDate );
				}
			} 
		}

		public void AddYear(string month)
		{
			string year = month.Substring(month.Length - 4, 4);
			this.yearList.Add(year);
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the month data list.
		/// </summary>
		public ArrayList MonthList
		{
			get { return this.monthList; }
			private set { this.monthList = value; }
		}

		/// <summary>
		/// This property will return the year data list.
		/// </summary>
		public ArrayList YearList
		{
			get { return this.yearList; }
			private set { this.yearList = value; }
		}

		/// <summary>
		/// If the MonthYear processor was called with a DataDictionary defined in the
		/// MonthYearSR, this will contain a combined, translated list suitable for binding
		/// to a MonthYear drop down.
		/// </summary>
		[DataMember]
		public DropdownItemCollectionClass CombinedList
		{
			get;
			set;
		}
		#endregion

		#region Override Methods
		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return "SELECT * from tblTransactions";
		}
		#endregion
	}
}
