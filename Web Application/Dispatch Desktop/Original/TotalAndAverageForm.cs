using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.BusinessInterfaces;

namespace DispatchPrototype
{
	public partial class TotalAndAverageForm : FMBaseForm
	{
		private List<DispatchTransactionsSR.DispatchTranslationPair> Translations = null;
		private DispatchDataAccess DataAccess = null;
		private DateTime StartDateTime;
		private DateTime EndDateTime;

		public TotalAndAverageForm ( DispatchDataAccess dataAccess, List<DispatchTransactionsSR.DispatchTranslationPair> translations, DateTime startDateTime, DateTime endDateTime )
		{
			DataAccess = dataAccess;
			Translations = translations;
			StartDateTime = startDateTime;
			EndDateTime = endDateTime;
			InitializeComponent ( );
		}

		private void TotalAndAverageOnLoad ( object sender, EventArgs e )
		{
			try
			{
				GetSecurity ( );

				SetDialogDisplayDefaultValues ( );
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		private void SetDialogDisplayDefaultValues ( )
		{
			QuantityradioButton.Checked = true;
			ResponseTimeradioButton.Checked = false;
			VarianceradioButton.Checked = false;
			FuelTimeradioButton.Checked = false;
			AverageUnitslabel.Text = "Gallon(s)";
			TotalUnitslabel.Text = "Gallon(s)";
			UpdateDialogResponseTimes ( );
		}

		private void Closebutton_Click ( object sender, EventArgs e )
		{
			Close ( );
		}

		private void UpdateDialogResponseTimes ( )
		{
			bool DisplayingValuesInTime = false;
			double TotalAmount = 0.0;
			double NumberOfTransactions = 0;
			DispatchTransactionsSR sr = new DispatchTransactionsSR ( );

			FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
			ISites sites = sitesClient.CreateProxy ( );

			SiteClass site = sites.Get(Security, Security.SiteGuid, false, false, false);
			SiteTimeConverter timeConverter = new SiteTimeConverter ( site );
			sr.Security = Security;
			sr.Translations = Translations;
			sr.BeginDate = StartDateTime;
			sr.EndDate = EndDateTime;

			sr.Statuses.Add ( "Dispatched" );
			sr.Statuses.Add ( "Arrived" );
			sr.Statuses.Add ( "Started" );
			sr.Statuses.Add ( "Stopped" );
			sr.Statuses.Add ( "Completed" );

			sr.AliasNames.Add ( "Refuel" );
			sr.AliasNames.Add ( "Defuel" );
			if (IncludeFillStandcheckBox.Checked == true)
				sr.AliasNames.Add ( "Fillstand" );
			if (IncludeRTBcheckBox.Checked == true)
				sr.AliasNames.Add ( "Return to Bulk" );

			DispatchTransactionsDO results = DataAccess.GetTransactionsNoUpdateConnection ( sr );
			DataSet ds = (DataSet) results.Transactions;
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				// the following cells are used
				// Start Time = 8	FST
				// Stop Time = 9	TimeEnd
				// Arrival Time = 14	TimeIn
				// Departed Time = 10	TimeOut
				// Dispatch Time = 13	DispatchedDateTime
				// Quantity = 49	GrossQuantity
				// Variance = 17	Variance
				// status = 5	TransactionStatus
				if (QuantityradioButton.Checked == true)
				{
					TransactionStatus Status = (TransactionStatus) Enum.Parse ( typeof ( TransactionStatus ), (string) dr["TransactionStatus"] );
					if (Status == TransactionStatus.Completed)
					{
						// calculate the quantities
						TotalAmount += System.Convert.ToDouble ( dr["GrossQuantity"].ToString ( ) );
						++NumberOfTransactions;
					}
				}
				else if (ResponseTimeradioButton.Checked == true)
				{
					// this is the time difference between requested time and arrival time
					if (dr["DispatchedDateTime"].ToString ( ).Length > 0 &&
						dr["TimeIn"].ToString ( ).Length > 0)
					{
						// calculate the response time in minutes
						TimeSpan timeSpan = new TimeSpan ( );
						timeSpan = System.Convert.ToDateTime ( dr["TimeIn"].ToString ( ) ) - System.Convert.ToDateTime ( dr["RequestedDateTime"].ToString ( ) );
						TotalAmount += timeSpan.TotalMinutes;
						++NumberOfTransactions;
					}
					DisplayingValuesInTime = true;
				}
				else if (FuelTimeradioButton.Checked == true)
				{
					// this is the difference between start and stop time
					if (dr["FST"].ToString ( ).Length > 0 &&
						dr["TimeEnd"].ToString ( ).Length > 0)
					{
						// calculate the response time in minutes
						TimeSpan timeSpan = new TimeSpan ( );
						timeSpan = System.Convert.ToDateTime ( dr["TimeEnd"].ToString ( ) ) - System.Convert.ToDateTime ( dr["FST"].ToString ( ) );
						TotalAmount += timeSpan.TotalMinutes;
						++NumberOfTransactions;
					}
					DisplayingValuesInTime = true;
				}
				else if (VarianceradioButton.Checked == true)
				{
					if (dr["Variance"].ToString ( ).Length > 0)
					{
						TotalAmount += System.Convert.ToDouble ( dr["Variance"].ToString ( ) );
						++NumberOfTransactions;
					}
				}
			}

			if (NumberOfTransactions == 0)
				AveragetextBox.Text = "";
			else if (DisplayingValuesInTime == true)
				AveragetextBox.Text = ( TotalAmount / NumberOfTransactions ).ToString ( "F2" );
			else
				AveragetextBox.Text = ( TotalAmount / NumberOfTransactions ).ToString ( ( site.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.VOLUME ) ) );

			if (DisplayingValuesInTime == true)
				TotaltextBox.Text = TotalAmount.ToString ( "F2" );
			else
				TotaltextBox.Text = TotalAmount.ToString ( ( site.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.VOLUME ) ) );
		}

		private void IncludeFillStandcheckBox_CheckedChanged ( object sender, EventArgs e )
		{
			UpdateDialogResponseTimes ( );
		}

		private void ResponseTimeradioButton_CheckedChanged ( object sender, EventArgs e )
		{
			AverageUnitslabel.Text = "Minute(s)";
			TotalUnitslabel.Text = "Minute(s)";
			UpdateDialogResponseTimes ( );
		}

		private void FuelTimeradioButton_CheckedChanged ( object sender, EventArgs e )
		{
			AverageUnitslabel.Text = "Minute(s)";
			TotalUnitslabel.Text = "Minute(s)";
			UpdateDialogResponseTimes ( );
		}

		private void VarianceradioButton_CheckedChanged ( object sender, EventArgs e )
		{
			AverageUnitslabel.Text = "";
			TotalUnitslabel.Text = "";
			UpdateDialogResponseTimes ( );
		}

		private void QuantityradioButton_CheckedChanged ( object sender, EventArgs e )
		{
			AverageUnitslabel.Text = "Gallon(s)";
			TotalUnitslabel.Text = "Gallon(s)";
			UpdateDialogResponseTimes ( );
		}

	}
}
