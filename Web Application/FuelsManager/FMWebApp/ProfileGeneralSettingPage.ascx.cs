// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileGeneralSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileGeneralSettingPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Web.UI.WebControls;

	using Crypt;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    This class handles the functionality for the Profile General tab page.
	/// </summary>
	public partial class ProfileGeneralSettingPage : FMUserControlBase
	{
		#region Constants and Fields

		/// <summary>
		///    The dummy data.
		/// </summary>
		private byte[] dummyData;

		/// <summary>
		///    The encryptor.
		/// </summary>
		private AESCrypt encryptor;

		/// <summary>
		///    The mobile device profile.
		/// </summary>
		private MobileDeviceProfile mobileDeviceProfile;

		/// <summary>
		///    The seed.
		/// </summary>
		private byte[] seed;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will reset all the fields when the new button is
		///    selected.
		/// </summary>
		public void ResetFieldsForNewEvent()
		{
			this.LoadData();
		}

		/// <summary>
		///    This method will update the profile configuration table from the general page.
		/// </summary>
		public void UpdateChanges()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile != null)
			{
				this.mobileDeviceProfile.ProfileId = this.ProfileIDTB.Text;
				this.mobileDeviceProfile.Description = this.DescriptionTB.Text;
				this.mobileDeviceProfile.ShutdownHotKey = this.GetShutdownHotkeyString();
				this.mobileDeviceProfile.VehicleId = this.VehicleIdTB.Text;

				this.mobileDeviceProfile.ShowProductScreen = this.ShowProductScreenCB.Checked;
				this.mobileDeviceProfile.GenerateTicketNumber = this.GenerateTicketNumberCB.Checked;
				this.mobileDeviceProfile.ShowOperatorFieldInFlightList = this.ShowOperatorFieldCB.Checked;
				this.mobileDeviceProfile.MonitorScreenTransitionTiming = this.MonitorScreenTransitionTimingCB.Checked;
				this.mobileDeviceProfile.BypassDistributionTolerance = this.BypassFsrCheckCB.Checked;
				this.mobileDeviceProfile.ShowFuelUpdateCheckStatusWin = this.ShowFuelUpdateCheckStatusWindowCB.Checked;
				this.mobileDeviceProfile.UseDefaultPrinter = this.UseDefaultPrinterCB.Checked;
				this.mobileDeviceProfile.MakeDefaultProfile = this.MakeDefaultProfileCB.Checked;
				this.mobileDeviceProfile.LoggingOption = this.LoggingOptionCB.Checked;

				this.mobileDeviceProfile.AdminPassword = null;
				if (string.IsNullOrEmpty(this.AdminPasswordTextbox.Text) == false)
				{
					this.mobileDeviceProfile.AdminPassword = this.Encode(this.AdminPasswordTextbox.Text);
				}

				// Set the Search and Dispatch dropdowns.
				this.mobileDeviceProfile.SearchType = Convert.ToInt32(this.SearchTypeDD.SelectedValue);

				// Convert to integer and check range.
				this.mobileDeviceProfile.AllowableFailedLoginAttempts =
					this.ConvertToIntAndTestRange(this.AllowableFailedLoginAttemptsTB.Text, 1, 100, "Failed Login Attempts");
				this.mobileDeviceProfile.FuelDistributionPrecision =
					this.ConvertToIntAndTestRange(this.FuelDistributionPrecisionTB.Text, 1, 100, "Fuel Distribution Precision");

				// Get the selected default printer selection
				Guid printerGuid = Guid.Parse(this.DefaultPrinterDD.SelectedItem.Value);
				this.mobileDeviceProfile.DefaultPrinter = string.Empty;

				if (printerGuid != Guid.Empty)
				{
					this.mobileDeviceProfile.DefaultPrinter = this.DefaultPrinterDD.SelectedItem.Text;
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method handles the page load event.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			// Used for encoding/decoding the admin password.
			this.encryptor = new AESCrypt();
			this.seed = (new Guid("1488AE9C-6813-49AE-AF08-155A53D99CE6")).ToByteArray();
			this.dummyData = (new Guid("4BE74006-F456-4399-86C5-03613D7FB234")).ToByteArray();

			if (this.Page.IsPostBack == false)
			{
				this.LoadData();
			}

			this.DisableFields();
		}

		/// <summary>
		///    This method handles the Shutdown Hotkey 1 event. It will disable all the other
		///    hotkey if the selected value is "None". Else it will enable the next hot key.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void ShutdownHotKey1OnChange(object sender, EventArgs e)
		{
			if (this.ShutdownHotKey1DD.SelectedItem.Value.Equals("None"))
			{
				this.ShutdownHotKey2DD.SelectedIndex = 0;
				this.ShutdownHotKey3DD.SelectedIndex = 0;
				this.ShutdownHotKey4DD.SelectedIndex = 0;

				this.ShutdownHotKey2DD.Enabled = false;
				this.ShutdownHotKey3DD.Enabled = false;
				this.ShutdownHotKey4DD.Enabled = false;
			}
			else
			{
				this.ShutdownHotKey2DD.Enabled = true;
			}
		}

		/// <summary>
		///    This method handles the Shutdown Hotkey 2 event. It will disable all the other
		///    hotkey if the selected value is "None". Else it will enable the next hot key.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void ShutdownHotKey2OnChange(object sender, EventArgs e)
		{
			if (this.ShutdownHotKey2DD.SelectedItem.Value.Equals("None"))
			{
				this.ShutdownHotKey3DD.SelectedIndex = 0;
				this.ShutdownHotKey4DD.SelectedIndex = 0;

				this.ShutdownHotKey3DD.Enabled = false;
				this.ShutdownHotKey4DD.Enabled = false;
			}
			else
			{
				this.ShutdownHotKey3DD.Enabled = true;
			}
		}

		/// <summary>
		///    This method handles the Shutdown Hotkey 3 event. It will disable all the other
		///    hotkey if the selected value is "None". Else it will enable the next hot key.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void ShutdownHotKey3OnChange(object sender, EventArgs e)
		{
			if (this.ShutdownHotKey3DD.SelectedItem.Value.Equals("None"))
			{
				this.ShutdownHotKey4DD.SelectedIndex = 0;
				this.ShutdownHotKey4DD.Enabled = false;
			}
			else
			{
				this.ShutdownHotKey4DD.Enabled = true;
			}
		}

		/// <summary>
		///    This method handles the Use Default Printer Checkbox checked change. It will either
		///    enable or disable the Default Printer dropdown.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void UseDefaultPrinterCheckedChange(object sender, EventArgs e)
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			if (this.UseDefaultPrinterCB.Checked)
			{
				this.DefaultPrinterDD.Enabled = true;
				this.DefaultPrinterDD.SelectedIndex = 0;

				this.mobileDeviceProfile.UseDefaultPrinter = true;
			}
			else
			{
				this.mobileDeviceProfile.UseDefaultPrinter = false;
				this.mobileDeviceProfile.DefaultPrinter = string.Empty;

				this.DefaultPrinterDD.Enabled = false;
				this.DefaultPrinterDD.SelectedIndex = 0;
			}
		}

		/// <summary>
		///    This method will build the default printer dropdown.
		/// </summary>
		private void BuildDefaultPrinterDropdown()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			var dropDownList = new List<ListItem>();
			ListItem item;

			foreach (MobileDeviceProfilePrinter printer in this.mobileDeviceProfile.PrinterCollection)
			{
				item = new ListItem { Text = printer.PrinterId, Value = printer.MobileDeviceProfilePrinterGuid.ToString() };
				dropDownList.Add(item);
			}

			dropDownList = dropDownList.OrderBy(x => x.Text).ToList();

			item = new ListItem { Text = this.GetDictionaryValueByKey(this.Security.SiteGuid, "None"), Value = Guid.Empty.ToString() };
			dropDownList.Insert(0, item);

			this.DefaultPrinterDD.DataSource = dropDownList;
			this.DefaultPrinterDD.DataTextField = "Text";
			this.DefaultPrinterDD.DataValueField = "Value";
			this.DefaultPrinterDD.Sort = false;
			this.DefaultPrinterDD.DataBind();

			// Set the appropriate selected item.
			if (string.IsNullOrEmpty(this.mobileDeviceProfile.DefaultPrinter))
			{
				this.DefaultPrinterDD.SelectedIndex = 0;
			}
			else
			{
				int selectedIndex = 0;
				int count = 0;

				foreach (ListItem listItem in dropDownList)
				{
					if (listItem.Text.Equals(this.mobileDeviceProfile.DefaultPrinter))
					{
						selectedIndex = count;
						break;
					}

					count++;
				}

				this.DefaultPrinterDD.SelectedIndex = selectedIndex;
			}

			// Enable or disable the Default Printer dropdown based on the 
			// Use Default Printer checkbox setting.
			if (this.mobileDeviceProfile.UseDefaultPrinter)
			{
				this.DefaultPrinterDD.Enabled = true;
			}
			else
			{
				this.mobileDeviceProfile.DefaultPrinter = string.Empty;
				this.DefaultPrinterDD.Enabled = false;
				this.DefaultPrinterDD.SelectedIndex = 0;
			}
		}

		private string GetDictionaryValueByKey(Guid guid, string key)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(guid, key)
																);
		}

		/// <summary>
		///    This method converts a string to integer and test the range. It will return null
		///    if the text value is null.
		/// </summary>
		/// <param name="textValue">
		///    The text value.
		/// </param>
		/// <param name="startRange">
		///    The start range.
		/// </param>
		/// <param name="endRange">
		///    The end range.
		/// </param>
		/// <param name="errMessage">
		///    The err message.
		/// </param>
		/// <returns>
		///    The System.Nullable`1[T -&gt; System.Int32].
		/// </returns>
		private int? ConvertToIntAndTestRange(string textValue, int startRange, int endRange, string errMessage)
		{
			int? returnValue;
			string mustBe1 = " " + this.GetDictionaryValueByKey(this.Security.SiteGuid, "must be between") + " ";
			string mustBe2 = " " + this.GetDictionaryValueByKey(this.Security.SiteGuid, "must be a number between") + " ";

			string err1 = mustBe1 + startRange.ToString(CultureInfo.InvariantCulture) + " and "
			              + endRange.ToString(CultureInfo.InvariantCulture) + ".";
			string err2 = mustBe2 + startRange.ToString(CultureInfo.InvariantCulture) + " and "
			              + endRange.ToString(CultureInfo.InvariantCulture) + ".";

			if (string.IsNullOrEmpty(textValue))
			{
				return null;
			}

			try
			{
				int intValue = Convert.ToInt32(textValue);

				if (intValue < startRange || intValue > endRange)
				{
					throw new ApplicationException(this.GetDictionaryValueByKey(this.Security.SiteGuid, errMessage) + err1);
				}

				returnValue = intValue;
			}
			catch (FormatException)
			{
				throw new ApplicationException(this.GetDictionaryValueByKey(this.Security.SiteGuid, errMessage) + err2);
			}
			catch (OverflowException)
			{
				throw new ApplicationException(this.GetDictionaryValueByKey(this.Security.SiteGuid, errMessage) + err2);
			}

			return returnValue;
		}

		/// <summary>
		///    This method will decode the admin password coming from the database.
		/// </summary>
		/// <param name="encodedData">Encoded password.</param>
		/// <returns>Returns a decoded string.</returns>
		private string Decode(byte[] encodedData)
		{
			using (AESKey key = this.GetKey())
			{
				return this.encryptor.DecryptToText(encodedData, key);
			}
		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device profile" right.
		/// </summary>
		private void DisableFields()
		{
			this.ProfileIDTB.Enabled = this.HasPermission();
			this.DescriptionTB.Enabled = this.HasPermission();
			this.SearchTypeDD.Enabled = this.HasPermission();
			this.ShutdownHotKey1DD.Enabled = this.HasPermission();
			this.ShutdownHotKey2DD.Enabled = this.HasPermission();
			this.ShutdownHotKey3DD.Enabled = this.HasPermission();
			this.ShutdownHotKey4DD.Enabled = this.HasPermission();
			this.AdminPasswordTextbox.Enabled = this.HasPermission();
			this.VehicleIdTB.Enabled = this.HasPermission();
			this.AllowableFailedLoginAttemptsTB.Enabled = this.HasPermission();
			this.FuelDistributionPrecisionTB.Enabled = this.HasPermission();
			this.DefaultPrinterDD.Enabled = this.HasPermission();
			this.ShowProductScreenCB.Enabled = this.HasPermission();
			this.GenerateTicketNumberCB.Enabled = this.HasPermission();
			this.ShowOperatorFieldCB.Enabled = this.HasPermission();
			this.MonitorScreenTransitionTimingCB.Enabled = this.HasPermission();
			this.BypassFsrCheckCB.Enabled = this.HasPermission();
			this.ShowFuelUpdateCheckStatusWindowCB.Enabled = this.HasPermission();
			this.UseDefaultPrinterCB.Enabled = this.HasPermission();
			this.LoggingOptionCB.Enabled = this.HasPermission();
			this.MakeDefaultProfileCB.Enabled = this.HasPermission();
		}

		/// <summary>
		///    This method will encode the plain text admin password to be saved
		///    in the database.
		/// </summary>
		/// <param name="plainTextData">Test to be encoded.</param>
		/// <returns>Returns an ecoded byte array.</returns>
		private byte[] Encode(string plainTextData)
		{
			using (AESKey key = this.GetKey())
			{
				return this.encryptor.Encrypt(plainTextData, key);
			}
		}

		/// <summary>
		///    This method generates a key for encoding.
		/// </summary>
		/// <returns>Returns a new AES key.</returns>
		private AESKey GetKey()
		{
			var newSeed = new byte[this.seed.Length + this.dummyData.Length];

			Buffer.BlockCopy(this.seed, 0, newSeed, 0, this.seed.Length);
			Buffer.BlockCopy(this.dummyData, 0, newSeed, this.seed.Length, this.dummyData.Length);

			return new AESKey(newSeed, this.Security.SiteGuid.ToByteArray());
		}

		/// <summary>
		///    This method will return the Shutdown Hotkey key sequence that was
		///    selected by the user.
		/// </summary>
		/// <returns>
		///    The System.String.
		/// </returns>
		private string GetShutdownHotkeyString()
		{
			if (this.ShutdownHotKey1DD.SelectedItem.Value.Equals("None"))
			{
				return string.Empty;
			}

			string returnStr = this.ShutdownHotKey1DD.SelectedItem.Value;

			if (this.ShutdownHotKey2DD.SelectedItem.Value.Equals("None") == false)
			{
				returnStr = returnStr + "-" + this.ShutdownHotKey2DD.SelectedItem.Value;
			}

			if (this.ShutdownHotKey3DD.SelectedItem.Value.Equals("None") == false)
			{
				returnStr = returnStr + "-" + this.ShutdownHotKey3DD.SelectedItem.Value;
			}

			if (this.ShutdownHotKey4DD.SelectedItem.Value.Equals("None") == false)
			{
				returnStr = returnStr + "-" + this.ShutdownHotKey4DD.SelectedItem.Value;
			}

			return returnStr;
		}

		/// <summary>
		///    This method returns true if the user has the MODIFY_MOBILE_DEVICE_PROFILES right and the
		///    entity has not been assigned down.
		/// </summary>
		/// <returns>
		///    The System.Boolean.
		/// </returns>
		private bool HasPermission()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return false;
			}

			if (this.mobileDeviceProfile.SiteGuid == Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES))
			{
				return true;
			}

			return this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES)
			       && (this.Security.SiteGuid == this.mobileDeviceProfile.SiteGuid);
		}

		/// <summary>
		///    This method will load the profile generate page with the data from the database.
		/// </summary>
		private void LoadData()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile != null)
			{
				this.ProfileIDTB.Text = this.mobileDeviceProfile.ProfileId;
				this.DescriptionTB.Text = this.mobileDeviceProfile.Description;
				this.VehicleIdTB.Text = this.mobileDeviceProfile.VehicleId;

				this.ShowProductScreenCB.Checked = this.mobileDeviceProfile.ShowProductScreen;
				this.GenerateTicketNumberCB.Checked = this.mobileDeviceProfile.GenerateTicketNumber;
				this.ShowOperatorFieldCB.Checked = this.mobileDeviceProfile.ShowOperatorFieldInFlightList;

				this.MonitorScreenTransitionTimingCB.Checked = this.mobileDeviceProfile.MonitorScreenTransitionTiming;
				this.BypassFsrCheckCB.Checked = this.mobileDeviceProfile.BypassDistributionTolerance;
				this.ShowFuelUpdateCheckStatusWindowCB.Checked = this.mobileDeviceProfile.ShowFuelUpdateCheckStatusWin;
				this.UseDefaultPrinterCB.Checked = this.mobileDeviceProfile.UseDefaultPrinter;
				this.MakeDefaultProfileCB.Checked = this.mobileDeviceProfile.MakeDefaultProfile;
				this.LoggingOptionCB.Checked = this.mobileDeviceProfile.LoggingOption;

				this.AdminPasswordTextbox.Text = null;
				if (this.mobileDeviceProfile.AdminPassword != null)
				{
					this.AdminPasswordTextbox.Attributes["value"] = this.Decode(this.mobileDeviceProfile.AdminPassword);
				}

				this.AllowableFailedLoginAttemptsTB.Text = string.Empty;
				if (this.mobileDeviceProfile.AllowableFailedLoginAttempts != null)
				{
					this.AllowableFailedLoginAttemptsTB.Text =
						this.mobileDeviceProfile.AllowableFailedLoginAttempts.Value.ToString(CultureInfo.InvariantCulture);
				}

				this.FuelDistributionPrecisionTB.Text = string.Empty;
				if (this.mobileDeviceProfile.FuelDistributionPrecision != null)
				{
					this.FuelDistributionPrecisionTB.Text =
						this.mobileDeviceProfile.FuelDistributionPrecision.Value.ToString(CultureInfo.InvariantCulture);
				}

				// Set the dropdown lists.
				this.SetShutdownHotkeyDropdown(this.mobileDeviceProfile.ShutdownHotKey);
				this.SetSearchType(this.mobileDeviceProfile.SearchType);

				// Populate the default printer dropdown.
				this.BuildDefaultPrinterDropdown();
			}
		}

		/// <summary>
		///    This method will build the search type dropdown and select the appropriate
		///    item.
		/// </summary>
		/// <param name="searchType">Search type operation</param>
		private void SetSearchType(int? searchType)
		{
			var searchTypes = new List<ListItem>();

			var listItem = new ListItem { Text = this.GetDictionaryValueByKey(this.Security.SiteGuid, "None"), Value = "0" };
			searchTypes.Add(listItem);

			listItem = new ListItem { Text = this.GetDictionaryValueByKey(this.Security.SiteGuid, "Dispatch by operator"), Value = "1" };
			searchTypes.Add(listItem);

			listItem = new ListItem { Text = this.GetDictionaryValueByKey(this.Security.SiteGuid, "Dispatch by vehicle"), Value = "2" };
			searchTypes.Add(listItem);

			listItem = new ListItem { Text = this.GetDictionaryValueByKey(this.Security.SiteGuid, "Dispatch by gate"), Value = "3" };
			searchTypes.Add(listItem);

			listItem = new ListItem
				{
					Text = this.GetDictionaryValueByKey(this.Security.SiteGuid, "Dispatch by time window"),
					Value = "4"
				};
			searchTypes.Add(listItem);

			this.SearchTypeDD.DataSource = searchTypes;
			this.SearchTypeDD.DataTextField = "Text";
			this.SearchTypeDD.DataValueField = "Value";
			this.SearchTypeDD.DataBind();

			if ((searchType == null) || (searchType < 0) || (searchType > 4))
			{
				this.SearchTypeDD.SelectedIndex = 0;
			}
			else
			{
				this.SearchTypeDD.SelectedIndex = searchType.Value;
			}
		}

		/// <summary>
		///    This method will populated the shutdown hotkey dropdowns and select
		///    the correct item.
		/// </summary>
		/// <param name="hotKeySetting">
		///    The hot key setting.
		/// </param>
		private void SetShutdownHotkeyDropdown(string hotKeySetting)
		{
			const string Characters =
				"None,Cntl,Alt,Shft,0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
			string[] characterList = Characters.Split(',');

			var compareList = new Dictionary<string, int>();
			int charIndex = 0;

			var hotkeyList = new List<ListItem>();

			foreach (string character in characterList)
			{
				ListItem listItem;

				if (character.Equals("None"))
				{
					listItem = new ListItem { Text = this.GetDictionaryValueByKey(this.Security.SiteGuid, character), Value = character };
				}
				else
				{
					listItem = new ListItem { Text = character, Value = character };
				}

				hotkeyList.Add(listItem);
				compareList.Add(character, charIndex++);
			}

			this.ShutdownHotKey1DD.DataSource = hotkeyList;
			this.ShutdownHotKey1DD.DataTextField = "Text";
			this.ShutdownHotKey1DD.DataValueField = "Value";
			this.ShutdownHotKey1DD.Sort = false;
			this.ShutdownHotKey1DD.DataBind();

			this.ShutdownHotKey2DD.DataSource = hotkeyList;
			this.ShutdownHotKey2DD.DataTextField = "Text";
			this.ShutdownHotKey2DD.DataValueField = "Value";
			this.ShutdownHotKey2DD.Sort = false;
			this.ShutdownHotKey2DD.DataBind();

			this.ShutdownHotKey3DD.DataSource = hotkeyList;
			this.ShutdownHotKey3DD.DataTextField = "Text";
			this.ShutdownHotKey3DD.DataValueField = "Value";
			this.ShutdownHotKey3DD.Sort = false;
			this.ShutdownHotKey3DD.DataBind();

			this.ShutdownHotKey4DD.DataSource = hotkeyList;
			this.ShutdownHotKey4DD.DataTextField = "Text";
			this.ShutdownHotKey4DD.DataValueField = "Value";
			this.ShutdownHotKey4DD.Sort = false;
			this.ShutdownHotKey4DD.DataBind();

			// Set the selection to the appropriate item.
			this.ShutdownHotKey1DD.SelectedIndex = 0;
			this.ShutdownHotKey2DD.SelectedIndex = 0;
			this.ShutdownHotKey3DD.SelectedIndex = 0;
			this.ShutdownHotKey4DD.SelectedIndex = 0;

			this.ShutdownHotKey2DD.Enabled = false;
			this.ShutdownHotKey3DD.Enabled = false;
			this.ShutdownHotKey4DD.Enabled = false;

			if (string.IsNullOrEmpty(hotKeySetting) == false && hotKeySetting.Equals("None") == false)
			{
				string[] parsedHotkey = hotKeySetting.Split('-');

				for (int nextValue = 0; nextValue < parsedHotkey.Length; nextValue++)
				{
					string key = parsedHotkey[nextValue];
					int selectionIndex;

					if (compareList.TryGetValue(key, out selectionIndex))
					{
						switch (nextValue)
						{
							case 0:
								this.ShutdownHotKey1DD.SelectedIndex = selectionIndex;

								if (selectionIndex != 0)
								{
									this.ShutdownHotKey2DD.Enabled = true;
								}

								break;
							case 1:
								this.ShutdownHotKey2DD.SelectedIndex = selectionIndex;

								if (selectionIndex != 0)
								{
									this.ShutdownHotKey3DD.Enabled = true;
								}

								break;
							case 2:
								this.ShutdownHotKey3DD.SelectedIndex = selectionIndex;

								if (selectionIndex != 0)
								{
									this.ShutdownHotKey4DD.Enabled = true;
								}

								break;
							case 3:
								this.ShutdownHotKey4DD.SelectedIndex = selectionIndex;
								break;
						}
					}
				}
			}
		}

		#endregion
	}
}