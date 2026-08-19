namespace Dispatch
{
	using System;
	using System.Text.RegularExpressions;
	using System.Windows.Forms;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class AddMemoForm : FMBaseForm
	{
		public string UserID = string.Empty;
		private Guid editedItemGuid = Guid.Empty;

		public Guid EditedItemGuid
		{
			get
			{
				return this.editedItemGuid;
			}
			set
			{
				this.editedItemGuid = value;

				this.Text = (this.editedItemGuid == Guid.Empty) ? "Add Memo" : "Edit Memo";
			}
		}

		public AddMemoForm()
		{
			this.InitializeComponent();

			this.GetSecurity();
		}

		private void OnCancelClicked(object sender, EventArgs e)
		{
			this.Close();
		}

		private void InitializeDialogDisplay()
		{
			try
			{
				this.MemoDateTimeSelection.CustomFormat = "MM/dd/yyyy  -  hh:mm:ss tt";
				this.MemoDateTimeSelection.ShowCheckBox = false;
				this.MemoDateTimeSelection.ShowUpDown = true;

				if (this.EditedItemGuid == Guid.Empty)
				{
					this.ControllerTextBox.Text = this.UserID;

					var site = FMChannelHelper.MakeCall<ISites, SiteClass>(
						x => x.Get(Security, Security.SiteGuid, false, false, false));

					var converter = new SiteTimeConverter(site);

					this.MemoDateTimeSelection.Value = converter.Now().DateTime;
					this.MemotextBox.Text = string.Empty;
				}
				else
				{
					ControllerLogClass Controller = new ControllerLogClass();

					Controller = FMChannelHelper.MakeCall<IControllerLogs, ControllerLogClass>(x => x.EnumerateControllerLogByIdentityGuid(base.Security, EditedItemGuid));

					// set the fields based on the record selected
					ControllerTextBox.Text = Controller.Controller;
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(Security, Security.SiteGuid, false, false, false));
					System.Globalization.DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();
					MemoDateTimeSelection.Value = System.Convert.ToDateTime(Controller.EventTime, dateTimeFormatInfo);
					MemotextBox.Text = Controller.Memo;
				}

				this.MemotextBox.Focus();

				if (!this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
				{
					this.OKbutton.Enabled = false;
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
			}
		}

		private void OnShown(object sender, EventArgs e)
		{
			this.InitializeDialogDisplay();
		}

		private void OnOkClicked(object sender, EventArgs e)
		{
			if (!this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				return;
			}

			try
			{
				if (this.MemotextBox.Text.Length <= 0)
				{
					MessageBox.Show(this, "Memo Required", this.Text);
					return;
				}

				var reg = new Regex(@"[';]|(--|/\*|\*/)"); //exclude /*, */, --, ' and ;

				if (reg.IsMatch(MemotextBox.Text))
				{
					this.DialogResult = DialogResult.None;
					MessageBox.Show(this, "Memo contains invalid characters", this.Text);
					return;
				}

				var controller = new ControllerLogClass();
				var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

				if (security == null)
				{
					throw new Exception("Security not in AppDomain");
				}

				controller.Controller = this.UserID;
				controller.EventTime = this.MemoDateTimeSelection.Value.ToString();
				controller.Memo = this.MemotextBox.Text;
				controller.SiteGuid = security.SiteGuid;
				controller.CreatedBy = security.UserID;

				if (this.EditedItemGuid == Guid.Empty) // add operation
				{
					FMChannelHelper.MakeCall<IControllerLogs>(x => x.Add(security, controller));
				}
				else
				{
					controller.IdentityGuid = this.EditedItemGuid;
					FMChannelHelper.MakeCall<IControllerLogs>(x => x.Modify(security, controller));
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
			}
		}
	}
}
