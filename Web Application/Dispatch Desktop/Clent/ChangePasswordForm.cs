namespace Dispatch
{
	using System;
	using System.Windows.Forms;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class ChangePasswordForm : Form
	{
		public ChangePasswordForm()
		{
			this.InitializeComponent();
			this.CenterToScreen();
		}

		private void OkButtonClick( object sender, EventArgs e )
		{
			try
			{
				if ( this.newPasswordTextBox.Text != this.reenterPasswordTextBox.Text )
					throw new Exception( "Password vs. Re-enter Password does not match" );

				var security = AppDomain.CurrentDomain.GetData( "Security" ) as SecurityClass;

				if (security == null)
				{
					throw new Exception( "Security not in AppDomain" );
				}


				UserClass user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, security.UserGuid));

				security.Password = user.Password;
				this.DialogResult = DialogResult.OK;
			}

			catch ( Exception exception )
			{
				this.newPasswordTextBox.Text = "";
				this.reenterPasswordTextBox.Text = "";
				MessageBox.Show( this, exception.Message, this.Text );
			}
		}

		private void CancelButtonClick( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
			AppDomain appDomain = AppDomain.CurrentDomain;
			var security = appDomain.GetData( "Security" ) as SecurityClass;
			FMChannelHelper.MakeCall<ISites>(x => x.Logout(security));
		}
	}
}
