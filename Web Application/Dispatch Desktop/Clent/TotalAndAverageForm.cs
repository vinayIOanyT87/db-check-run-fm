namespace Dispatch
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class TotalAndAverageForm : FMBaseForm
	{
		private readonly List<DispatchTransactionsSR.DispatchTranslationPair> translations;
		private readonly DispatchDataAccess dataAccess;
		private readonly DateTime startDateTime;
		private readonly DateTime endDateTime;

		public TotalAndAverageForm(DispatchDataAccess dataAccess, 
									List<DispatchTransactionsSR.DispatchTranslationPair> translations, 
									DateTime startDateTime, 
									DateTime endDateTime)
		{
			this.dataAccess		= dataAccess;
			this.translations	= translations;
			this.startDateTime	= startDateTime;
			this.endDateTime	= endDateTime;

			this.InitializeComponent();
		}

		private void TotalAndAverageOnLoad(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.SetDialogDisplayDefaultValues();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SetDialogDisplayDefaultValues()
		{
			this.QuantityradioButton.Checked		= true;
			this.ResponseTimeradioButton.Checked	= false;
			this.VarianceradioButton.Checked		= false;
			this.FuelTimeradioButton.Checked		= false;
			this.AverageUnitslabel.Text				= "Gallon(s)";
			this.TotalUnitslabel.Text				= "Gallon(s)";

			this.UpdateDialogResponseTimes();
		}

		private void ClosebuttonClick(object sender, EventArgs e)
		{
			this.Close();
		}

		private void UpdateDialogResponseTimes()
		{
			bool displayingValuesInTime = false;
			double totalAmount = 0.0;
			double numberOfTransactions = 0;

			SiteClass site = FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(this.Security, this.Security.SiteGuid));

			var sr = new DispatchTransactionsSR
			         {
				         Security		= this.Security,
				         Translations	= this.translations,
				         BeginDate		= this.startDateTime,
				         EndDate		= this.endDateTime
			         };

			sr.Statuses.Add("Dispatched");
			sr.Statuses.Add("Arrived");
			sr.Statuses.Add("Started");
			sr.Statuses.Add("Stopped");
			sr.Statuses.Add("Completed");
            sr.Statuses.Add("Posted");
			sr.Statuses.Add("Pending");
			sr.AliasNames.Add("Refuel");
			sr.AliasNames.Add("Defuel");

			if (this.IncludeFillStandcheckBox.Checked)
			{
				sr.AliasNames.Add("Fillstand");
			}

			if (this.IncludeRTBcheckBox.Checked)
			{
				sr.AliasNames.Add("Return to Bulk");
			}

			DispatchTransactionsDO results = this.dataAccess.GetTransactionsNoUpdateConnection(sr);
			DataSet ds = results.Transactions;

			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				// The following cells are used
				// Start Time		= 8		FST
				// Stop Time		= 9		TimeEnd
				// Arrival Time		= 14	TimeIn
				// Departed Time	= 10	TimeOut
				// Dispatch Time	= 13	DispatchedDateTime
				// Quantity			= 49	GrossQuantity
				// Variance			= 17	Variance
				// status			= 5		TransactionStatus
				if (this.QuantityradioButton.Checked)
				{
					var status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), (string)dr["TransactionStatus"]);
					
					if ((status == TransactionStatus.Completed)|| (status == TransactionStatus.Posted))
					{
						// calculate the quantities
						totalAmount += Convert.ToDouble(dr["GrossQuantity"].ToString());
						++numberOfTransactions;
					}
				}
				else if (this.ResponseTimeradioButton.Checked)
				{
					// this is the time difference between requested time and arrival time
					if (dr["DispatchedDateTime"].ToString().Length > 0 &&
						dr["TimeIn"].ToString().Length > 0)
					{
						// calculate the response time in minutes
						TimeSpan timeSpan = Convert.ToDateTime(dr["TimeIn"].ToString()) - Convert.ToDateTime(dr["RequestedDateTime"].ToString());
						totalAmount += timeSpan.TotalMinutes;
						++numberOfTransactions;
					}

					displayingValuesInTime = true;
				}
				else if (this.FuelTimeradioButton.Checked)
				{
					// this is the difference between start and stop time
					if (dr["FST"].ToString().Length > 0 &&
						dr["TimeEnd"].ToString().Length > 0)
					{
						// calculate the response time in minutes
						TimeSpan timeSpan = Convert.ToDateTime(dr["TimeEnd"].ToString()) - Convert.ToDateTime(dr["FST"].ToString());
						totalAmount += timeSpan.TotalMinutes;
						++numberOfTransactions;
					}

					displayingValuesInTime = true;
				}
				else if (this.VarianceradioButton.Checked)
				{
					if (dr["Variance"].ToString().Length > 0)
					{
						totalAmount += Convert.ToDouble(dr["Variance"].ToString());
						++numberOfTransactions;
					}
				}
			}

			if (numberOfTransactions == 0.0)
			{
				this.AveragetextBox.Text = string.Empty;
			}
			else if (displayingValuesInTime)
			{
				this.AveragetextBox.Text = (totalAmount / numberOfTransactions).ToString("F2");
			}
			else
			{
				this.AveragetextBox.Text = (totalAmount / numberOfTransactions).ToString((site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME)));
			}

			if (displayingValuesInTime)
			{
				this.TotaltextBox.Text = totalAmount.ToString("F2");
			}
			else
			{
				this.TotaltextBox.Text = totalAmount.ToString((site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME)));
			}
		}

		private void IncludeFillStandcheckBoxCheckedChanged(object sender, EventArgs e)
		{
			this.UpdateDialogResponseTimes();
		}

		private void ResponseTimeradioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.AverageUnitslabel.Text = "Minute(s)";
			this.TotalUnitslabel.Text = "Minute(s)";
			this.UpdateDialogResponseTimes();
		}

		private void FuelTimeradioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.AverageUnitslabel.Text = "Minute(s)";
			this.TotalUnitslabel.Text = "Minute(s)";
			this.UpdateDialogResponseTimes();
		}

		private void VarianceradioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.AverageUnitslabel.Text = string.Empty;
			this.TotalUnitslabel.Text = string.Empty;
			this.UpdateDialogResponseTimes();
		}

		private void QuantityradioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.AverageUnitslabel.Text = "Gallon(s)";
			this.TotalUnitslabel.Text = "Gallon(s)";
			this.UpdateDialogResponseTimes();
		}
	}
}
