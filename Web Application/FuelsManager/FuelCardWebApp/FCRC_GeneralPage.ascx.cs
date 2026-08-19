/******************************************************************************

	FILE NAME:		FRCRC_GeneralPage.ascx.cs


	PURPOSE:			Implementation of FRCRC_GeneralPage


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2009

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

namespace FuelsManager.FuelCardWebApp
{
	using System;
	using System.Collections;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMWebApp;

	public partial class FCRC_GeneralPage : FuelCardPageBase
	{
        /// <summary>
		/// This method creates the inactivity dropdown list.  The list contains numeric
		/// value from 1 - 24 respresenting months.
		/// </summary>
		private void BindInactivityDropdown()
		{
			ArrayList inactiveList = new ArrayList();
			ListItem inactiveItem = null;
			string monthText = string.Empty;
			string monthValue = string.Empty;

			for (int nextMonth = 1; nextMonth < 25; nextMonth++)
			{
				if (nextMonth < 10)
				{
					monthValue = "0" + nextMonth.ToString();
				}
				else
				{
					monthValue = nextMonth.ToString();
				}

				monthText = nextMonth.ToString();
				inactiveItem = new ListItem(monthText, monthValue);
				inactiveList.Add(inactiveItem);
			}

			this.InactivityPeriodDropDownList.DataSource = inactiveList;
			this.InactivityPeriodDropDownList.DataTextField = "Text";
			this.InactivityPeriodDropDownList.DataValueField = "Value";
			this.InactivityPeriodDropDownList.Sort = false;
			this.InactivityPeriodDropDownList.DataBind();
		}

		private void UpdateView()
		{
			if (FuelCard == null)
				return;

            this.FuelCardID.Text = FuelCard.ID;
            this.ProviderName.Text = FuelCard.Provider;
            this.ProviderID.Text = FuelCard.ProviderID;
            this.Notes.Text = FuelCard.Notes;

            this.ExpirationDate.FormatInfo = FuelCard.ExpirationDateFormat.Format;
            this.ExpirationDate.Text = FuelCard.ExpirationDate.HasValue ? FuelCard.ExpirationFormattedDate : string.Empty;

		    this.TransientCardFlag.Checked = FuelCard.TransientCardFlag;

            // Put the values in the attributes so that they are not lost during the postback.
            this.PIN.Attributes["value"] = GeneralConstants.PasswordPlaceholder;
            this.ConfirmPIN.Attributes["value"] = GeneralConstants.PasswordPlaceholder;

            // TypeDropDownList
            this.FuelCardTypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString()));

            ApplicationStringCollectionClass fuelCardTypes =
                FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
                    x => x.EnumerateByType(this.Security, STRING_TYPE.FUEL_CARD_TYPE));

            for (int itemIndex = 0; itemIndex < fuelCardTypes.Count; itemIndex++)
            {
                ApplicationStringClass fuelCardType = fuelCardTypes[itemIndex];

                var newTypeItem = new ListItem(fuelCardType.ID, fuelCardType.IdentityGuid.ToString());

                foreach (ListItem existingTypeItem in this.FuelCardTypeDropDownList.Items)
                {
                    if (existingTypeItem.Text.CompareTo(newTypeItem.Text) > 0)
                    {
                        int index = this.FuelCardTypeDropDownList.Items.IndexOf(existingTypeItem);
                        this.FuelCardTypeDropDownList.Items.Insert(index, newTypeItem);
                        
                        if (fuelCardType.IdentityGuid == this.FuelCard.FuelCardTypeApplicationStringGuid)
                        {
                            this.FuelCardTypeDropDownList.SelectedIndex = index;
                        }

                        newTypeItem = null;
                        break;
                    }
                }

                if (newTypeItem != null)
                {
                    this.FuelCardTypeDropDownList.Items.Add(newTypeItem);
                    if (fuelCardType.IdentityGuid == this.FuelCard.FuelCardTypeApplicationStringGuid)
                    {
                        this.FuelCardTypeDropDownList.SelectedIndex = this.FuelCardTypeDropDownList.Items.Count - 1;
                    }
                }
            }

            foreach (ListItem listItem in this.StatusDropDownList.Items)
			{
				if (listItem.Value == ((int) FuelCard.Status).ToString())
				{
					this.StatusDropDownList.SelectedIndex = this.StatusDropDownList.Items.IndexOf(listItem);
					break;
				}
			}

			//Set inactivity period.
			foreach (ListItem listItem in this.InactivityPeriodDropDownList.Items)
			{
				if (listItem.Text == FuelCard.InactivityPeriod.ToString())
				{
					this.InactivityPeriodDropDownList.SelectedIndex = this.InactivityPeriodDropDownList.Items.IndexOf(listItem);
					break;
				}
			}

			this.ManagerSelect.Text = FuelCard.ManagerID;
			this.OwnerSelect.Text = FuelCard.OwnerID;
			this.ShipperSelect.Text = FuelCard.ShipperID;
			this.BillToSelect.Text = FuelCard.BillToID;
			this.ShipToSelect.Text = FuelCard.ShipToID;
		    this.HiddenCheckBox.Checked = FuelCard.HiddenDate.HasValue;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				Session.Remove("Status");

				if (Page.IsPostBack == false)
				{
					this.BindInactivityDropdown();
					this.UpdateView();
				}
				else
				{
                    // During a postback event, we need to keep the contents of the password fields in the event they haven't been saved yet.
                    this.PIN.Attributes["value"] = this.PIN.Text;
                    this.ConfirmPIN.Attributes["value"] = this.ConfirmPIN.Text;
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		public void UpdateData()
		{
			try
			{
				this.FuelCard.ID = this.FuelCardID.Text;
                this.FuelCard.Provider = this.ProviderName.Text;
                this.FuelCard.ProviderID = this.ProviderID.Text;
                this.FuelCard.Notes = this.Notes.Text;

                if (string.IsNullOrEmpty(this.ExpirationDate.Text))
                {
                    this.FuelCard.ExpirationDate = null;
                }
                else
                {
                    this.FuelCard.ExpirationDate = this.ExpirationDate.CurrentValue;
                }
                              
                this.FuelCard.TransientCardFlag = this.TransientCardFlag.Checked;

                if (this.PIN.Text != GeneralConstants.PasswordPlaceholder)
                {
                    this.FuelCard.PIN = this.PIN.Text;
                }

                if (this.FuelCardTypeDropDownList.SelectedIndex != -1)
                {
                    this.FuelCard.FuelCardTypeApplicationStringGuid = Guid.Parse(this.FuelCardTypeDropDownList.SelectedValue);
                    this.FuelCard.FuelCardTypeApplicationStringID = this.FuelCardTypeDropDownList.SelectedItem.Text;
                }
                else
                {
                    this.FuelCard.FuelCardTypeApplicationStringGuid = Guid.Empty;
                    this.FuelCard.FuelCardTypeApplicationStringID = "{None}";
                }

				FMChannelHelper.MakeCall<ICompanies>(
					companies =>
						{
							this.FuelCard.ManagerID = this.ManagerSelect.Text;
							if (this.FuelCard.ManagerID != string.Empty)
							{
								this.FuelCard.ManagerGuid = companies.GetMasterRecordGuid(Security, this.FuelCard.ManagerID);
							}
							else
							{
								this.FuelCard.ManagerGuid = Guid.Empty;
							}

							this.FuelCard.OwnerID = this.OwnerSelect.Text;
							if (this.FuelCard.OwnerID != string.Empty)
							{
								this.FuelCard.OwnerGuid = companies.GetMasterRecordGuid(Security, this.FuelCard.OwnerID);
							}
							else
							{
								this.FuelCard.OwnerGuid = Guid.Empty;
							}

							this.FuelCard.ShipperID = this.ShipperSelect.Text;
							if (this.FuelCard.ShipperID != string.Empty)
							{
								this.FuelCard.ShipperGuid = companies.GetMasterRecordGuid(Security, this.FuelCard.ShipperID);
							}
							else
							{
								this.FuelCard.ShipperGuid = Guid.Empty;
							}

							this.FuelCard.BillToID = this.BillToSelect.Text;
							if (this.FuelCard.BillToID != string.Empty)
							{
								this.FuelCard.BillToGuid = companies.GetMasterRecordGuid(Security, this.FuelCard.BillToID);
							}
							else
							{
								this.FuelCard.BillToGuid = Guid.Empty;
							}

							this.FuelCard.ShipToID = this.ShipToSelect.Text;
							if (this.FuelCard.ShipToID != string.Empty)
							{
								this.FuelCard.ShipToGuid = companies.GetMasterRecordGuid(Security, this.FuelCard.ShipToID);
							}
							else
							{
								this.FuelCard.ShipToGuid = Guid.Empty;
							}

							if (this.StatusDropDownList.SelectedIndex >= 0)
							{
								this.FuelCard.Status = (FuelCardClass.Statuses)Convert.ToInt32(StatusDropDownList.SelectedItem.Value);
							}

							this.FuelCard.InactivityPeriod = (this.InactivityPeriodDropDownList.SelectedIndex >= 0)
								                            ? Convert.ToInt32(this.InactivityPeriodDropDownList.SelectedValue)
								                            : 4;
						});

                // Only set the hidden date if the hidden check box is checked and there isn't already a value
                if (this.HiddenCheckBox.Checked && !this.FuelCard.HiddenDate.HasValue)
                {
                    this.FuelCard.HiddenDate = DateTimeOffset.Now;
                }
                else if (!this.HiddenCheckBox.Checked)
                {
                    this.FuelCard.HiddenDate = null;
                }
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				Response.End();
			}
		}
	}
}