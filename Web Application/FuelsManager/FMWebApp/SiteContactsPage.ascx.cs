namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

using FMCore;
using FMControls;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// This page provides a way for a user to specify people to contact for a site
	/// </summary>
	public partial class SiteContactsPage : FMUserControlBase
	{
		/// <summary>
		/// Loads information from the Session Site object and updates the controls on the page with the data
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">unused</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				SiteClass site = (SiteClass)this.Session["Site"];

				if (!this.Page.IsPostBack)
				{
					this.Contact1NameTextBox.Text = site.Contact1Name;
					this.Contact1Address1TextBox.Text = site.Contact1Address1;
					this.Contact1Address2TextBox.Text = site.Contact1Address2;
					this.Contact1CityTextBox.Text = site.Contact1City;
					this.Contact1StateTextBox.Text = site.Contact1State;
					this.Contact1ZipTextBox.Text = site.Contact1Zip;
					this.Contact1CountryTextBox.Text = site.Contact1Country;
					this.Contact1PhoneOfficeTextBox.Text = site.Contact1PhoneOffice;
					this.Contact1PhoneMobileTextBox.Text = site.Contact1PhoneMobile;
					this.Contact1FaxTextBox.Text = site.Contact1Fax;
					this.Contact1EmailAddressTextBox.Text = site.Contact1EmailAddress;

					this.Contact2NameTextBox.Text = site.Contact2Name;
					this.Contact2Address1TextBox.Text = site.Contact2Address1;
					this.Contact2Address2TextBox.Text = site.Contact2Address2;
					this.Contact2CityTextBox.Text = site.Contact2City;
					this.Contact2StateTextBox.Text = site.Contact2State;
					this.Contact2ZipTextBox.Text = site.Contact2Zip;
					this.Contact2CountryTextBox.Text = site.Contact2Country;
					this.Contact2PhoneOfficeTextBox.Text = site.Contact2PhoneOffice;
					this.Contact2PhoneMobileTextBox.Text = site.Contact2PhoneMobile;
					this.Contact2FaxTextBox.Text = site.Contact2Fax;
					this.Contact2EmailAddressTextBox.Text = site.Contact2EmailAddress;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the updating the Site data object with the information on the 
		/// Site Contacts page.
		/// </summary>
		public void UpdateData()
		{
			SiteClass site = (SiteClass)this.Session["Site"];

			if ( this.Contact1EmailAddressTextBox.Text.IsValidEmailAddressSyntax() == false )
			{
				throw new FMEmailFormatException( "Contact 1 Email" );
			}

			if ( this.Contact2EmailAddressTextBox.Text.IsValidEmailAddressSyntax() == false )
			{
				throw new FMEmailFormatException( "Contact 2 Email" );
			}

			site.Contact1Name = this.Contact1NameTextBox.Text;
			site.Contact1Address1 = this.Contact1Address1TextBox.Text;
			site.Contact1Address2 = this.Contact1Address2TextBox.Text;
			site.Contact1City = this.Contact1CityTextBox.Text;
			site.Contact1State = this.Contact1StateTextBox.Text;
			site.Contact1Zip = this.Contact1ZipTextBox.Text;
			site.Contact1Country = this.Contact1CountryTextBox.Text;
			site.Contact1PhoneOffice = this.Contact1PhoneOfficeTextBox.Text;
			site.Contact1PhoneMobile = this.Contact1PhoneMobileTextBox.Text;
			site.Contact1Fax = this.Contact1FaxTextBox.Text;
			site.Contact1EmailAddress = this.Contact1EmailAddressTextBox.Text;

			site.Contact2Name = this.Contact2NameTextBox.Text;
			site.Contact2Address1 = this.Contact2Address1TextBox.Text;
			site.Contact2Address2 = this.Contact2Address2TextBox.Text;
			site.Contact2City = this.Contact2CityTextBox.Text;
			site.Contact2State = this.Contact2StateTextBox.Text;
			site.Contact2Zip = this.Contact2ZipTextBox.Text;
			site.Contact2Country = this.Contact2CountryTextBox.Text;
			site.Contact2PhoneOffice = this.Contact2PhoneOfficeTextBox.Text;
			site.Contact2PhoneMobile = this.Contact2PhoneMobileTextBox.Text;
			site.Contact2Fax = this.Contact2FaxTextBox.Text;
			site.Contact2EmailAddress = this.Contact2EmailAddressTextBox.Text; 
		}
	}
}