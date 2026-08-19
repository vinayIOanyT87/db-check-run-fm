using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace DispatchPrototype
{
	public partial class AddMemoForm : FMBaseForm
	{
		public string UserID = "";
		private int _EditedItemIndex = -1;

		public int EditedItemIndex
		{
			get { return _EditedItemIndex; }
			set
			{
				_EditedItemIndex = value;

				this.Text = (_EditedItemIndex == -1) ? "Add Memo" : "Edit Memo";
			}
		}

		public AddMemoForm()
		{
			InitializeComponent();

			base.GetSecurity();
		}

		private void OnCancelClicked( object sender, EventArgs e )
		{
			Close();
		}

		private void InitializeDialogDisplay()
		{
			try
			{
				MemoDateTimeSelection.CustomFormat = "MM/dd/yyyy  -  hh:mm:ss tt";
				MemoDateTimeSelection.ShowCheckBox = false;
				MemoDateTimeSelection.ShowUpDown = true;

				if (EditedItemIndex < 0)
				{
					ControllerTextBox.Text = this.UserID;
					MemoDateTimeSelection.Value = System.DateTime.Now;
					MemotextBox.Text = "";
				}
				else
				{
					FMChannelFactory<IControllerLogs> cntrlLogsClient = new FMChannelFactory<IControllerLogs> ( );
					IControllerLogs controllerLogs = cntrlLogsClient.CreateProxy ( );

					ControllerLogClass controller = new ControllerLogClass ( );
					//controller = controllerLogs.EnumerateControllerLogByIdentityGuid ( base.Security, EditedItemIndex );

					// set the fields based on the record selected
					ControllerTextBox.Text = controller.Controller;
					MemoDateTimeSelection.Value = System.Convert.ToDateTime( controller.EventTime );
					MemotextBox.Text = controller.Memo;
				}

				MemotextBox.Focus();

				if (!base.Security.HasRight( RIGHT.MODIFY_DISPATCH ))
				{
					this.OKbutton.Enabled = false;
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show( this, exception.Message, this.Text );
			}
		}

		private void OnActivated( object sender, EventArgs e )
		{
			InitializeDialogDisplay();
		}

		private void OnOKClicked( object sender, EventArgs e )
		{
			if (!Security.HasRight( RIGHT.MODIFY_DISPATCH ))
			{
				return;
			}

			try
			{
				if (MemotextBox.Text.Length <= 0)
				{
					MessageBox.Show( this, "Memo Required", this.Text );
					return;
				}

				ControllerLogClass controller = new ControllerLogClass();

				FMChannelFactory<IControllerLogs> cntrlLogsClient = new FMChannelFactory<IControllerLogs> ( );
				IControllerLogs controllerLogs = cntrlLogsClient.CreateProxy ( );

				SecurityClass security = AppDomain.CurrentDomain.GetData( "Security" ) as SecurityClass;

				if (security == null)
				{
					throw new Exception ( "Security not in AppDomain" );
				}

				controller.Controller = this.UserID;
				controller.EventTime = MemoDateTimeSelection.Value.ToString();
				controller.Memo = MemotextBox.Text;
				controller.SiteIndex = security.SiteIndex;
				controller.CreatedBy = security.UserID;

				if (EditedItemIndex < 0)	// add operation
				{
					controllerLogs.Add ( security, controller );
				}
				else
				{
					controller.Index = EditedItemIndex;
					controllerLogs.Modify ( security, controller );
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show( this, exception.Message, this.Text );
			}
		}
	}
}
