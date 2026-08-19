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
	public partial class DispatchExportForm : FMBaseForm
	{
		public DateTime OperationLockDate = System.DateTime.Now;
		private List<DispatchTransactionsSR.DispatchTranslationPair> Translations = null;
		private DispatchDataAccess DataAccess = null;
		private DateTime LastSelectedDateTime = new DateTime ( );

		public DispatchExportForm ( DispatchDataAccess dataAccess, DateTime LockDate, List<DispatchTransactionsSR.DispatchTranslationPair> translations )
		{
			InitializeComponent ( );
			OperationLockDate = LockDate;
			Translations = translations;
			DataAccess = dataAccess;
		}

		private void closebutton_Click ( object sender, EventArgs e )
		{
			DialogResult = DialogResult.Cancel;
		}

		private void dispatchexportFormLoad ( object sender, EventArgs e )
		{
			GetSecurity ( );
			lockoutdateTimePicker.Value = OperationLockDate;
			lockoutTimePicker.Value = OperationLockDate;
		}

		private void applybutton_Click ( object sender, EventArgs e )
		{

			string ErrorMessage = string.Empty;
			DateTime SelectedDateTime = new DateTime ( );

			SelectedDateTime = lockoutdateTimePicker.Value.Date + lockoutTimePicker.Value.TimeOfDay;
			// if the user changes the value ensure it is not before the current lock out date
			// and do not allow the user to set it into the future
			if (SelectedDateTime < OperationLockDate)
			{
				ErrorMessage = "Lock out date/time can not be before current lock out date/time";
			}
			else if (SelectedDateTime > System.DateTime.Now)
			{
				ErrorMessage = "Lock out date/time can not be in the future";
			}

			if (ErrorMessage.Length > 0)
			{
				MessageBox.Show ( ErrorMessage );
				return;
			}

			// we need to ensure that every defuel/refeul transaction is completed before we allow the export
			DispatchTransactionsSR sr = new DispatchTransactionsSR ( );

			FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
			ISites sites = sitesClient.CreateProxy ( );

			SiteClass site = sites.Get(Security, Security.SiteGuid, false, false, false);
			SiteTimeConverter timeConverter = new SiteTimeConverter ( site );
			sr.Security = Security;
			sr.Translations = Translations;
			sr.BeginDate = OperationLockDate;
			sr.EndDate = lockoutdateTimePicker.Value;

			sr.Statuses.Add ( "Requested" );
			sr.Statuses.Add ( "Dispatched" );
			sr.Statuses.Add ( "Arrived" );
			sr.Statuses.Add ( "Started" );
			sr.Statuses.Add ( "Stopped" );
			sr.Statuses.Add ( "Completed" );

			sr.AliasNames.Add ( "Refuel" );
			sr.AliasNames.Add ( "Defuel" );

			DispatchTransactionsDO results = DataAccess.GetTransactionsNoUpdateConnection ( sr );
			DataSet ds = (DataSet) results.Transactions;
			foreach (DataRow Row in ds.Tables[0].Rows)
			{
				// ensure all transactions have a completed date	TimeOut
				TransactionStatus Status = (TransactionStatus) Enum.Parse ( typeof ( TransactionStatus ), (string) Row["TransactionStatus"] );
				if (( (string) Row["AliasName"] == "Refuel" ||
					(string) Row["AliasName"] == "Defuel" ) &&
					Status != TransactionStatus.Completed &&
					Status != TransactionStatus.Cancelled)
				{
					MessageBox.Show ( "Lock out date can not be set unless all Refuel and Defuel transactions are complete" );
					return;
				}
			}

			try
			{
				site.OperationalLockDate = ( lockoutdateTimePicker.Value.Date + lockoutTimePicker.Value.TimeOfDay ).ToString ( );
				sites.Modify ( Security, DATA_TYPE.CONFIG, site, false );
				OperationLockDate = lockoutdateTimePicker.Value.Date + lockoutTimePicker.Value.TimeOfDay;
				DialogResult = DialogResult.OK;
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		private void lockoutdateTimePickerValueChanged ( object sender, EventArgs e )
		{
			string ErrorMessage = string.Empty;
			DateTime SelectedDateTime = new DateTime ( );

			SelectedDateTime = lockoutdateTimePicker.Value.Date + lockoutTimePicker.Value.TimeOfDay;
			if (LastSelectedDateTime == SelectedDateTime)
			{
				if (SelectedDateTime < OperationLockDate ||
					SelectedDateTime > System.DateTime.Now.Date)
				{
					LastSelectedDateTime = OperationLockDate;
					lockoutdateTimePicker.Value = OperationLockDate;
					lockoutTimePicker.Value = OperationLockDate;
					return;
				}
			}
			// if the user changes the value ensure it is not before the current lock out date
			// and do not allow the user to set it into the future
			//			if (SelectedDateTime < OperationLockDate)
			//			{
			//				ErrorMessage = "Lock out date/time can not be before current lock out date/time";
			//			}
			//			else if (SelectedDateTime > System.DateTime.UtcNow.Date)
			//			{
			//				ErrorMessage = "Lock out date/time can not be in the future";
			//			}

			//			if(ErrorMessage.Length > 0)
			//			{
			//				MessageBox.Show(ErrorMessage);
			//				LastSelectedDateTime = SelectedDateTime;
			//				lockoutdateTimePicker.Value = OperationLockDate;
			//				lockoutTimePicker.Value = OperationLockDate;
			//			}
		}

		private void SetToCurrentDateTimebutton_OnClick ( object sender, EventArgs e )
		{
			lockoutdateTimePicker.Value = System.DateTime.Now;
			lockoutTimePicker.Value = System.DateTime.Now;
		}


	}
}
