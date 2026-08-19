namespace FuelsManager.DispatchWebApp
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public partial class OptionalTimesPage : FMFormBase
	{
		/// <summary>
		/// Key to save and retrieve the Optional Time setting in table tblConfigurationSetting.
		/// </summary>
		public const string WebDispatchOptionTimesConfigSettingKey = "WebDispatchOptionalTimes";

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Page.IsPostBack == false)
				{
					this.ReadAndSetOptionalTimeSettings();
				}

				this.OkButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#region Event methods
		/// <summary>
		/// This method will save the Option Time values to table tblConfigurationSetting.
		/// </summary>
		/// <param name="sender">The calling object.</param>
		/// <param name="e">The event arguments</param>
		protected void OkButtonOnClick(object sender, EventArgs e)
		{
			string arrivalTimeFlag = this.ArrivalTimeCheckbox.Checked ? "T" : "F";
			string startTimeFlag = this.StartTimeCheckbox.Checked ? "T" : "F";
			string stopTimeFlag = this.StopTimeCheckbox.Checked ? "T" : "F";

			string updateStr = string.Format("Arrival:{0}|Start:{1}|Stop:{2}", arrivalTimeFlag, startTimeFlag, stopTimeFlag);

			try
			{
				FMChannelHelper.MakeCall<IConfigurationSettings>(
										x => x.Modify(this.Security, WebDispatchOptionTimesConfigSettingKey, updateStr));

				// Close dialog window.
				this.ClientScript.RegisterStartupScript(this.GetType(), "CloseScript", "window.close();", true);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will read the Optional Time settings from table tblConfigurationSetting.
		/// It will set the controls accordingly.
		/// </summary>
		private void ReadAndSetOptionalTimeSettings()
		{
			this.ArrivalTimeCheckbox.Checked = false;
			this.StartTimeCheckbox.Checked = false;
			this.StopTimeCheckbox.Checked = false;

			string optionalTimesSettingValue = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
													x => x.GetKeyValueByKey(this.Security, WebDispatchOptionTimesConfigSettingKey));

			if (string.IsNullOrEmpty(optionalTimesSettingValue) == false)
			{
				// Parse "Arrival:T|Start:T|Stop:T"
				string[] parts = optionalTimesSettingValue.Split('|');

				if (parts.Length < 3)
				{
					throw new Exception("Invalid parse of: " + optionalTimesSettingValue);
				}

				string[] arrivalTimeParts = parts[0].Split(':');
				string[] startTimeParts = parts[1].Split(':');
				string[] stopTimeParts = parts[2].Split(':');

				if (arrivalTimeParts.Length == 2)
				{
					if (arrivalTimeParts[1].ToUpper().Equals("T"))
					{
						this.ArrivalTimeCheckbox.Checked = true;
					}
				}

				if (startTimeParts.Length == 2)
				{
					if (startTimeParts[1].ToUpper().Equals("T"))
					{
						this.StartTimeCheckbox.Checked = true;
					}
				}

				if (stopTimeParts.Length == 2)
				{
					if (stopTimeParts[1].ToUpper().Equals("T"))
					{
						this.StopTimeCheckbox.Checked = true;
					}
				}
			}
		}
		#endregion
	}
}