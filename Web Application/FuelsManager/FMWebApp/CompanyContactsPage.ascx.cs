namespace FMWebApp
{
    using System;
    using System.Web.UI;

    using FMBusinessObjects.Exceptions;
    using FMCore;
    using FuelsManager.FMWebApp;

	/// <summary>
	/// This page provides a way for a user to specify people to contact for a company
	/// </summary>
	public partial class CompanyContactsPage : CompanyPageBase
	{
		/// <summary>
		/// Loads information from the Company object and updates the controls on the page with the data
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">unused</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!Page.IsPostBack)
				{
					Contact1NameTextBox.Text = Company.Contact1Name;
					Contact1Address1TextBox.Text = Company.Contact1Address1;
					Contact1Address2TextBox.Text = Company.Contact1Address2;
					Contact1CityTextBox.Text = Company.Contact1City;
					Contact1StateTextBox.Text = Company.Contact1State;
					Contact1ZipTextBox.Text = Company.Contact1Zip;
					Contact1CountryTextBox.Text = Company.Contact1Country;
					Contact1PhoneOfficeTextBox.Text = Company.Contact1PhoneOffice;
					Contact1PhoneMobileTextBox.Text = Company.Contact1PhoneMobile;
					Contact1FaxTextBox.Text = Company.Contact1Fax;
					Contact1EmailAddressTextBox.Text = Company.Contact1EmailAddress;

					Contact2NameTextBox.Text = Company.Contact2Name;
					Contact2Address1TextBox.Text = Company.Contact2Address1;
					Contact2Address2TextBox.Text = Company.Contact2Address2;
					Contact2CityTextBox.Text = Company.Contact2City;
					Contact2StateTextBox.Text = Company.Contact2State;
					Contact2ZipTextBox.Text = Company.Contact2Zip;
					Contact2CountryTextBox.Text = Company.Contact2Country;
					Contact2PhoneOfficeTextBox.Text = Company.Contact2PhoneOffice;
					Contact2PhoneMobileTextBox.Text = Company.Contact2PhoneMobile;
					Contact2FaxTextBox.Text = Company.Contact2Fax;
					Contact2EmailAddressTextBox.Text = Company.Contact2EmailAddress;

                    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the updating the company data object with the information on the 
		/// Company Contacts page.
		/// </summary>
		public void UpdateData()
		{
			if (this.Contact1EmailAddressTextBox.Text.IsValidEmailAddressSyntax() == false)
			{
				throw new FMEmailFormatException("Contact 1 Email");
			}

			if (this.Contact2EmailAddressTextBox.Text.IsValidEmailAddressSyntax() == false)
			{
				throw new FMEmailFormatException( "Contact 2 Email" );
			}

			Company.Contact1Name = Contact1NameTextBox.Text;
			Company.Contact1Address1 = Contact1Address1TextBox.Text;
			Company.Contact1Address2 = Contact1Address2TextBox.Text;
			Company.Contact1City = Contact1CityTextBox.Text;
			Company.Contact1State = Contact1StateTextBox.Text;
			Company.Contact1Zip = Contact1ZipTextBox.Text;
			Company.Contact1Country = Contact1CountryTextBox.Text;
			Company.Contact1PhoneOffice = Contact1PhoneOfficeTextBox.Text;
			Company.Contact1PhoneMobile = Contact1PhoneMobileTextBox.Text;
			Company.Contact1Fax = Contact1FaxTextBox.Text;
			Company.Contact1EmailAddress = Contact1EmailAddressTextBox.Text;

			Company.Contact2Name = Contact2NameTextBox.Text;
			Company.Contact2Address1 = Contact2Address1TextBox.Text;
			Company.Contact2Address2 = Contact2Address2TextBox.Text;
			Company.Contact2City = Contact2CityTextBox.Text;
			Company.Contact2State = Contact2StateTextBox.Text;
			Company.Contact2Zip = Contact2ZipTextBox.Text;
			Company.Contact2Country = Contact2CountryTextBox.Text;
			Company.Contact2PhoneOffice = Contact2PhoneOfficeTextBox.Text;
			Company.Contact2PhoneMobile = Contact2PhoneMobileTextBox.Text;
			Company.Contact2Fax = Contact2FaxTextBox.Text;
			Company.Contact2EmailAddress = Contact2EmailAddressTextBox.Text; 
		}



        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }
            this.Contact1NameTextBox.Enabled = (this.Contact1NameTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1Name"));
            this.Contact1Address1TextBox.Enabled = (this.Contact1Address1TextBox.Enabled && this.VersionSpecificFields.Contains("Contact1Address1"));
            this.Contact1Address2TextBox.Enabled = (this.Contact1Address2TextBox.Enabled && this.VersionSpecificFields.Contains("Contact1Address2"));
            this.Contact1CityTextBox.Enabled = (this.Contact1CityTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1City"));
            this.Contact1StateTextBox.Enabled = (this.Contact1StateTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1State"));
            this.Contact1ZipTextBox.Enabled = (this.Contact1ZipTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1Zip"));
            this.Contact1CountryTextBox.Enabled = (this.Contact1CountryTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1Country"));
            this.Contact1PhoneOfficeTextBox.Enabled = (this.Contact1PhoneOfficeTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1PhoneOffice"));
            this.Contact1PhoneMobileTextBox.Enabled = (this.Contact1PhoneMobileTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1PhoneMobile"));
            this.Contact1FaxTextBox.Enabled = (this.Contact1FaxTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1Fax"));
            this.Contact1EmailAddressTextBox.Enabled = (this.Contact1EmailAddressTextBox.Enabled && this.VersionSpecificFields.Contains("Contact1EmailAddress"));
            this.Contact2NameTextBox.Enabled = (this.Contact2NameTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2Name"));
            this.Contact2Address1TextBox.Enabled = (this.Contact2Address1TextBox.Enabled && this.VersionSpecificFields.Contains("Contact2Address1"));
            this.Contact2Address2TextBox.Enabled = (this.Contact2Address2TextBox.Enabled && this.VersionSpecificFields.Contains("Contact2Address2"));
            this.Contact2CityTextBox.Enabled = (this.Contact2CityTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2City"));
            this.Contact2StateTextBox.Enabled = (this.Contact2StateTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2State"));
            this.Contact2ZipTextBox.Enabled = (this.Contact2ZipTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2Zip"));
            this.Contact2CountryTextBox.Enabled = (this.Contact2CountryTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2Country"));
            this.Contact2PhoneOfficeTextBox.Enabled = (this.Contact2PhoneOfficeTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2PhoneOffice"));
            this.Contact2PhoneMobileTextBox.Enabled = (this.Contact2PhoneMobileTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2PhoneMobile"));
            this.Contact2FaxTextBox.Enabled = (this.Contact2FaxTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2Fax"));
            this.Contact2EmailAddressTextBox.Enabled = (this.Contact2EmailAddressTextBox.Enabled && this.VersionSpecificFields.Contains("Contact2EmailAddress"));
            
        }
	}
}