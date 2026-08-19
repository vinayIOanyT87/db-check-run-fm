// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonLoadRackPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PersonLoadRackPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

   /// <summary>
   ///    Summary description for PersonLoadRackPage.
   /// </summary>
   public partial class PersonLoadRackPage : PersonPageBase
   {
      #region Constants and Fields
      protected SiteClass CurrentSite;
      protected FMLabel Label20;
      protected FMLabel Label21;
      protected TextBox PersonIDTextbox;
      protected DropDownList PersonNameDropDownList;
      #endregion

      #region Public Methods and Operators
      public void UpdateData()
      {
         if (this.Person != null)
         {
            if (this.PINNumberTextbox.Text != this.ConfirmPINTextBox.Text)
            {
               throw new Exception("PIN Numbers must match. Please verify that the values in the PIN Number and Confirm PIN Number fields are the same.");
            }

            this.Person.CardNumber = this.CardNumberTextbox.Text;
            this.Person.PINRequired = this.PINRequiredCheckbox.Checked;

            // We have to make this check for PINNumberTextBox because we explictly set the
            // value for the PIN mask, which muddles with pulling .Text back from the disabled field
            if (this.PINNumberTextbox.Enabled)
            {
               this.Person.PINNumber = this.PINNumberTextbox.Text;
            }

            this.Person.LockedOut = this.LockedOutCheckBox.Checked;
            this.Person.LockedOutReason = this.LockedOutReasonTextbox.Text;
            this.Person.ShortCardNumber = this.ShortCardNumberTextbox.Text;
            this.Person.InhibitInactivityLockout = this.InhibitInactivityLockOutCheckBox.Checked;
         }
      }
      #endregion


      #region Methods
      protected void LockedOutCheckBoxCheckedChanged(object sender, EventArgs e)
      {
         if (!this.LockedOutCheckBox.Checked)
         {
            this.LockedOutDateTextbox.Text = "";
            this.LockedOutReasonTextbox.Text = "";
            this.LockedOutReasonTextbox.Enabled = false;
         }
         else
         {
            this.LockedOutReasonTextbox.Enabled = true;
            this.Person._LockedOutDate.Value = TimeConverter.Today();
            this.LockedOutDateTextbox.Text = this.Person.LockedOutDate;
         }
      }

      protected override void OnInit(EventArgs e)
      {
         //
         // CODEGEN: This call is required by the ASP.NET Web Form Designer.
         //
         this.InitializeComponent();
         base.OnInit(e);
      }

      private void SetFieldAccessibilityForChildRecordVersion()
      {
         bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

         if (this.Person.IdentityGuid.Equals(Guid.Empty)
              || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid))
              || (this.VersionSpecificFields == null))
         {
            return;
         }

         this.CardNumberTextbox.Enabled = (this.CardNumberTextbox.Enabled && this.VersionSpecificFields.Contains("CardNumber"));
         this.PINRequiredCheckbox.Enabled = (this.PINRequiredCheckbox.Enabled && this.VersionSpecificFields.Contains("PINRequired"));
         this.PINNumberTextbox.Enabled = (this.PINNumberTextbox.Enabled && this.VersionSpecificFields.Contains("PINNumber"));
         this.ConfirmPINTextBox.Enabled = (this.ConfirmPINTextBox.Enabled && this.VersionSpecificFields.Contains("PINNumber"));
         this.ShortCardNumberTextbox.Enabled = (this.ShortCardNumberTextbox.Enabled && this.VersionSpecificFields.Contains("ShortCardNumber"));
         this.LastActivityTextbox.Enabled = (this.LastActivityTextbox.Enabled && this.VersionSpecificFields.Contains("LastActivityDate"));
         this.CardedInCheckBox.Enabled = (this.CardedInCheckBox.Enabled && this.VersionSpecificFields.Contains("CardedIn"));
         this.LockedOutCheckBox.Enabled = (this.LockedOutCheckBox.Enabled && this.VersionSpecificFields.Contains("LockedOut"));
         this.LockedOutReasonTextbox.Enabled = (this.LockedOutReasonTextbox.Enabled && this.VersionSpecificFields.Contains("LockedOutReason"));
         this.LockedOutDateTextbox.Enabled = (this.LockedOutDateTextbox.Enabled && this.VersionSpecificFields.Contains("LockedOutDate"));
         this.signatureOnFile.Enabled = (this.signatureOnFile.Enabled && this.VersionSpecificFields.Contains("OnFileSignature"));
         this.signatureStationList.Enabled = (this.signatureStationList.Enabled && this.VersionSpecificFields.Contains("OnFileSignature"));
         this.captureSignature.Enabled = (this.captureSignature.Enabled && this.VersionSpecificFields.Contains("OnFileSignature"));
         this.clearSignature.Enabled = (this.clearSignature.Enabled && this.VersionSpecificFields.Contains("OnFileSignature"));
         this.InhibitInactivityLockOutCheckBox.Enabled = (this.InhibitInactivityLockOutCheckBox.Enabled && this.VersionSpecificFields.Contains("InhibitInactivityLockout"));
         this.AddButton.Enabled = (this.AddButton.Enabled && this.VersionSpecificFields.Contains("Carrier"));
      }

      public bool IsLicenseChildVersionEnabled()
      {
         bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

         if ((this.Person.IdentityGuid.Equals(Guid.Empty)
              || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid))))
         {
            return true;
         }

         bool ret = ((this.VersionSpecificFields != null) && this.VersionSpecificFields.Contains("License"));
         return ret;
      }

      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
            this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                    x =>
                                                    x.Get(this.Security, this.Security.SiteGuid, false, false, true)
                                                );
            if (!this.Page.IsPostBack)
            {
               // Need to have a collection of available carriers for potential assignment
               this.Session["CompanyCollection"] = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(x => x.EnumerateByRole(this.Security, COMPANY_ROLE.CARRIER, false, false, true));

               this.CardNumberTextbox.Text = this.Person.CardNumber;
               this.PINRequiredCheckbox.Checked = this.Person.PINRequired;

               // Instead of setting the text to the password, we add a default value to the text box
               // We then check the text of the text box against this value when the user saves the changes to see if a change was made
               // This is done to avoid having the password be visible in the page source.
               if (!string.IsNullOrEmpty(this.Person.PINNumber))
               {
                  this.PINNumberTextbox.Attributes.Add("value", PersonClass.MaskedPasswordText);
                  this.ConfirmPINTextBox.Attributes.Add("value", PersonClass.MaskedPasswordText);
               }

               this.ShortCardNumberTextbox.Text = this.Person.ShortCardNumber;
               this.signatureOnFile.Enabled = false;

               if (this.Person.OnFileSignature == null || this.Person.OnFileSignature.Length == 0)
               {
                  this.signatureOnFile.Checked = false;
               }
               else
               {
                  this.signatureOnFile.Checked = true;
               }

               SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                    x =>
                                                    x.Get(
                                                         this.Security,
                                                         this.Security.SiteGuid,
                                                         getMemberSites: true,
                                                         getSchedulesAndProcessVariables: true,
                                                         bGetAssociatedAliases: true)
                                                   );

               DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();

               this.Person._LastActivityDate.Format = dateTimeFormatInfo;
               this.LastActivityTextbox.Text = this.Person.LastActivityDate;
               this.CardedInCheckBox.Checked = this.Person.CardedIn;
               this.LockedOutCheckBox.Checked = this.Person.LockedOut;
               this.InhibitInactivityLockOutCheckBox.Checked = this.Person.InhibitInactivityLockout;
               this.Person._LockedOutDate.Format = dateTimeFormatInfo;

               // Always disable Reason Testbox on page load. (IGO 11-Dec-2009) This fixes bug #8410.
               this.LockedOutReasonTextbox.Enabled = false;

               if (this.Person.LockedOut)
               {
                  this.LockedOutReasonTextbox.Text = this.Person.LockedOutReason;
                  this.LockedOutDateTextbox.Text = this.Person.LockedOutDate;
               }

               // Load stations here
               this.PopulateSignatureStations();

               // companies data grid
               this.PopulateCompaniesDataGrid();

               // if greater then two companies are assigned disable the add button
               this.SetAddButtonAccess(true);

               this.SetFieldAccessibilityForChildRecordVersion();
            }
            else // page posted back
            {
               // if PINNumber and ConfirmPIN are disabled (Field Level Control can disable them), the Text property will 
               // come back empty.  In that case, fall back to the personnel record PIN to determin if we need to display
               // a mask value.  We can count on them being enabled or disabled together.
               if (this.PINNumberTextbox.Enabled)
               {
                  // If the page ever posts back, we have to re-add the password the user entered or the value will be lost
                  this.PINNumberTextbox.Attributes.Add("value", this.PINNumberTextbox.Text);

                  // If the page ever posts back, we have to re-add the password the user entered or the value will be lost
                  this.ConfirmPINTextBox.Attributes.Add("value", this.ConfirmPINTextBox.Text);
               }
               else
               {
                  if (!string.IsNullOrEmpty(this.Person.PINNumber))
                  {
                     this.PINNumberTextbox.Attributes.Add("value", PersonClass.MaskedPasswordText);
                     this.ConfirmPINTextBox.Attributes.Add("value", PersonClass.MaskedPasswordText);
                  }
               }

               // Check dummy text fields for new TWIC enrollment data
               if ("" != this.DummyTextBox1.Text && "" != this.DummyTextBox2.Text)
               {
                  // Create or update license for TWIC enrollment
                  Guid twicGuid = FMChannelHelper.MakeCall<IQualifications, Guid>(
                                                    x =>
                                                    x.GetIdentityGuid(this.Security, QUALIFICATION_TYPE.PERSON_LICENSE, "TWIC")
                                                );
                  if (!twicGuid.IsEmpty())
                  {
                     bool existinglicense = false;

                     // Search for existing license map
                     foreach (QualificationMapClass licensemap in this.Person.LicenseCollection)
                     {
                        // when found update this information with the newly read information
                        if (licensemap.AssignedGuid == twicGuid)
                        {
                           licensemap.Number = this.DummyTextBox1.Text;
                           var expirationdate = new Date(this.CurrentSite);
                           this.CurrentSite.SetDate("Expiration Date", this.DummyTextBox2.Text, ref expirationdate);
                           licensemap.ExpirationDate = expirationdate;
                           existinglicense = true;
                           break;
                        }
                     }

                     // if existing license not found create new one
                     if (false == existinglicense)
                     {
                        var licensemap = new QualificationMapClass
                        {
                           ID = "TWIC",
                           IdentityGuid = Guid.NewGuid(),
                           AssigneeGuid = this.Person.IdentityGuid,
                           AssignedGuid = twicGuid,
                           Type = QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON,
                           Number = this.DummyTextBox1.Text,
                           Sequence = this.Person.LicenseCollection.Count
                        };

                        var expirationdate = new Date(this.CurrentSite);
                        this.CurrentSite.SetDate("Expiration Date", this.DummyTextBox2.Text, ref expirationdate);
                        licensemap.ExpirationDate = expirationdate;

                        this.Person.LicenseCollection.Add(licensemap);
                     }

                     ((PersonForm)this.Page).UpdateData();
                  }

                  this.DummyTextBox1.Text = string.Empty;
                  this.DummyTextBox2.Text = string.Empty;
               }
            }
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }
      }

      /// <summary>
      ///    Required method for Designer support - do not modify
      ///    the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.captureSignature.Command += this.CaptureSignatureCommand;
         this.clearSignature.Command += this.ClearSignatureCommand;
         this.AddButton.Command += this.AddButton_Command;
         this.AssignedCompaniesDataGrid.ItemDataBound += this.AssignedCompaniesDataGrid_ItemDataBound;
         this.AssignedCompaniesDataGrid.CancelCommand += this.AssignedCompaniesDataGrid_CancelCommand;
         this.AssignedCompaniesDataGrid.UpdateCommand += this.AssignedCompaniesDataGrid_UpdateCommand;
         this.AssignedCompaniesDataGrid.EditCommand += this.AssignedCompaniesDataGrid_EditCommand;
         this.AssignedCompaniesDataGrid.DeleteCommand += this.AssignedCompaniesDataGrid_DeleteCommand;
      }

      private void PopulateSignatureStations()
      {
         StationCollectionClass stationCollection = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(
                                                    x =>
                                                    x.EnumerateByType(this.Security, STATION_TYPE.SIGNATURE)
                                                );
         foreach (StationClass station in stationCollection)
         {
            var newTypeItem = new ListItem(station.ID, station.IdentityGuid.ToString());

            foreach (ListItem existingTypeItem in this.signatureStationList.Items)
            {
               if (string.Compare(existingTypeItem.Text, newTypeItem.Text, StringComparison.Ordinal) > 0)
               {
                  int index = this.signatureStationList.Items.IndexOf(existingTypeItem);
                  this.signatureStationList.Items.Insert(index, newTypeItem);
                  newTypeItem = null;
                  break;
               }
            }

            if (newTypeItem != null)
            {
               this.signatureStationList.Items.Add(newTypeItem);
            }
         }
      }

      private void CaptureSignatureCommand(object sender, CommandEventArgs e)
      {
         try
         {
            if (this.signatureStationList.SelectedIndex == -1)
            {
               return;
            }

            if (FMFormBase.UsingLoadRack)
            {
               Guid selectedStationGuid = Guid.Parse(this.signatureStationList.SelectedValue);
               ILoadRackManager loadRackManager = this.GetLoadRackManager();

               this.Person.OnFileSignature = loadRackManager.GetSignature(this.Security, selectedStationGuid);

               this.signatureOnFile.Checked = this.Person.OnFileSignature != null;
            }
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }
      }

      private void ClearSignatureCommand(object sender, CommandEventArgs e)
      {
         this.Person.OnFileSignature = null;
         this.signatureOnFile.Checked = false;
      }
      #endregion

      private void PopulateCompaniesDataGrid()
      {
         this.UpdateView();
      }

      private void UpdateView()
      {
         this.AssignedCompaniesDataGrid.DataSource = this.EnumerateCompaniesAssigned();
         this.AssignedCompaniesDataGrid.DataBind();
      }

      private ICollection EnumerateCompaniesAssigned()
      {
         bool isCarrierEditingEnabled = false;
         bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
         if (this.Person.IdentityGuid.Equals(Guid.Empty)
               || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid))
               || (this.VersionSpecificFields == null))
         {
            isCarrierEditingEnabled = true;
         }
         else
            isCarrierEditingEnabled = (this.VersionSpecificFields.Contains("Carrier"));


         DataTable mapDataTable = new DataTable();

         mapDataTable.Columns.Add("Index", typeof(int));
         mapDataTable.Columns.Add("ID", typeof(string));
         mapDataTable.Columns.Add("AssignedToolTip", typeof(string));
         mapDataTable.Columns.Add("EditingEnabled", typeof(string));

         for (int iItem = 0; iItem < this.Person.AssignedCompaniesCollection.Count; iItem++)
         {
            var mapDataRow = mapDataTable.NewRow();

            var companyMap = this.Person.AssignedCompaniesCollection[iItem];
            mapDataRow[0] = iItem;
            mapDataRow[1] = companyMap.AssignedID;
            mapDataRow[2] = companyMap.AssignedToolTip;
            mapDataRow[3] = isCarrierEditingEnabled.ToString();

            mapDataTable.Rows.Add(mapDataRow);
         }
         DataView carrierDataView = new DataView(mapDataTable);
         return carrierDataView;
      }

      // ReSharper disable once InconsistentNaming
      protected void AddButton_Command(object sender, CommandEventArgs e)
      {
         try
         {
            CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY);

            this.Person.AssignedCompaniesCollection.Add(companyMap);

            this.AssignedCompaniesDataGrid.CurrentPageIndex = (this.Person.AssignedCompaniesCollection.Count - 1) / this.AssignedCompaniesDataGrid.PageSize;
            this.AssignedCompaniesDataGrid.EditItemIndex = (this.Person.AssignedCompaniesCollection.Count - 1) % this.AssignedCompaniesDataGrid.PageSize;
            this.Session["AddingCarrierCompany"] = true;

            this.EnableControls(false);
            this.UpdateView();
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
            this.Person.AssignedCompaniesCollection.Remove(this.Person.AssignedCompaniesCollection.Count - 1);

            if (this.AssignedCompaniesDataGrid.CurrentPageIndex > 0
                && this.AssignedCompaniesDataGrid.EditItemIndex == 0)
            {
               this.AssignedCompaniesDataGrid.CurrentPageIndex--;
            }

            this.AssignedCompaniesDataGrid.EditItemIndex = -1;

            // Enable the add button.
            this.EnableControls(true);

            this.UpdateView();
         }
      }

      protected ListItemCollection EnumerateCarrierCompanies()
      {
         var companyCollection = (CompanyCollectionClass)this.Session["CompanyCollection"];

         ListItemCollection companyItems = new ListItemCollection();

         CompanyMapClass selectedCompanyMap = null;

         if (this.AssignedCompaniesDataGrid.EditItemIndex > -1 && this.AssignedCompaniesDataGrid.EditItemIndex < this.Person.AssignedCompaniesCollection.Count)
            selectedCompanyMap = this.Person.AssignedCompaniesCollection[this.AssignedCompaniesDataGrid.EditItemIndex];

         // ReSharper disable once ForCanBeConvertedToForeach
         for (int iItem = 0; iItem < companyCollection.Count; iItem++)
         {
            var company = companyCollection[iItem];

            // make sure we do not add any already selected compamies
            var addToCollection = true;
            foreach (var companyMap in this.Person.AssignedCompaniesCollection)
            {
               if (selectedCompanyMap != null)
               {
                  if (selectedCompanyMap.AssignedID == company.ID)
                     continue;
               }

               if (companyMap.AssignedID == company.ID)
               {
                  addToCollection = false;
                  break;
               }
            }

            if (addToCollection)
            {
               ListItem newCompanyItem = new ListItem(company.ID, company.IdentityGuid.ToString());
               foreach (ListItem existingCompanyItem in companyItems)
               {
                  if (string.Compare(existingCompanyItem.Text, newCompanyItem.Text, StringComparison.Ordinal) > 0)
                  {
                     int index = companyItems.IndexOf(existingCompanyItem);
                     companyItems.Insert(index, newCompanyItem);
                     newCompanyItem = null;
                     break;
                  }
               }

               if (newCompanyItem != null)
                  companyItems.Add(newCompanyItem);
            }
         }

         if (companyItems.Count == 0)
         {
            string errMsg = "No more Carriers available.";

            throw (new Exception(errMsg));
         }

         return companyItems;
      }


      // ReSharper disable once InconsistentNaming
      protected void AssignedCompaniesDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
      {

         if (e.Item.ItemType != ListItemType.Header &&
             e.Item.ItemType != ListItemType.Footer)
         {
            LinkButton editButton = (LinkButton)e.Item.FindControl("FMEditLinkButton1");
            LinkButton deleteButton = (LinkButton)e.Item.FindControl("FMDeleteLinkButton1");

            //Set Field Accessibility For Child Record Version
            bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

            if (!this.Person.IdentityGuid.Equals(Guid.Empty)
                && !(currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
            {
               if (editButton != null)
               {
                  editButton.Enabled = (editButton.Enabled &&
                                          ((this.VersionSpecificFields != null)
                                                 && (this.VersionSpecificFields.Count > 0)));
               }

               if (deleteButton != null)
               {
                  deleteButton.Enabled = (deleteButton.Enabled &&
                                              ((this.VersionSpecificFields != null)
                                              && (this.VersionSpecificFields.Count > 0)));
               }
            }

            if (this.AssignedCompaniesDataGrid != null && this.AssignedCompaniesDataGrid.EditItemIndex == e.Item.ItemIndex)
            {
               // Now set the focus to the edit control
               Control ctrl;

               if (deleteButton != null)
               {
                  deleteButton.Enabled = false;
               }

               if (this.AssignedCompaniesDataGrid.EditItemIndex == e.Item.ItemIndex)
               {
                  ctrl = e.Item.FindControl("CompaniesDropDownList");

                  Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

                  if (indexLabel != null)
                  {
                     var companyMap = this.Person.AssignedCompaniesCollection[Convert.ToInt32(indexLabel.Text)];

                     DropDownList companiesDropDownList = (DropDownList)e.Item.FindControl("CompaniesDropDownList");

                     if (companiesDropDownList != null && companyMap != null)
                     {
                        ListItem selItem = companiesDropDownList.Items.FindByText(companyMap.AssignedID);

                        if (selItem != null)
                        {
                           companiesDropDownList.SelectedIndex = companiesDropDownList.Items.IndexOf(selItem);
                        }
                     }
                  }
               }
               else
               {
                  ctrl = e.Item.FindControl("EditButton");
               }

               if (ctrl != null)
               {
                  string script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
                  this.Page.ClientScript.RegisterStartupScript(this.GetType(), "page_set_focus", string.Format(script, ctrl.ClientID));
               }
            }
         }
      }

      protected void SetAddButtonAccess(bool enable)
      {
         if (enable)
         {
            // disable the add button if there are three or more items in the grid
            if (this.AssignedCompaniesDataGrid.Items.Count > 2)
               this.AddButton.Enabled = false;
            else
               this.AddButton.Enabled = true;
         }
         else
            this.AddButton.Enabled = false;
      }

      // ReSharper disable once InconsistentNaming
      protected void AssignedCompaniesDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
      {
         try
         {
            // user cancelled editing so just remove the object we added during the add evolution
            bool addingRecord = (bool)this.Session["AddingCarrierCompany"];
            if (addingRecord)
            {
               // delete the last record in the collection
               this.Person.AssignedCompaniesCollection.Remove(this.Person.AssignedCompaniesCollection.Count - 1);
               this.Session["AddingCarrierCompany"] = false;
            }
         }
         catch //(Exception except)
         {
            // if we are in modify and NOT ADD this is expected when the addingcarriercompany above DOES NOT EXIST.
            // no reason to log or report an expected error.
            //this.ErrorHandler(except);
         }
         finally
			{
            this.AssignedCompaniesDataGrid.EditItemIndex = -1;
            this.EnableControls(true);
            this.UpdateView();
         }
      }


      // ReSharper disable once InconsistentNaming
      protected void AssignedCompaniesDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
      {
         try
         {
            Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

            var companyMap = this.Person.AssignedCompaniesCollection[Convert.ToInt32(indexLabel.Text)];

            DropDownList companiesDropDownList = (DropDownList)e.Item.FindControl("CompaniesDropDownList");

            //CompaniesClass Companies = new CompaniesClass();
            //AssignedIndex = System.Convert.ToInt32(CompaniesDropDownList.SelectedValue);	// company index
            var id = companiesDropDownList.SelectedItem.Text;
            CompanyClass company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
                                                                companies => companies.Get(this.Security, companies.GetIdentityGuid(this.Security, id), false));

            companyMap.AssignedID = company.ID;
            companyMap.AssignedGuid = company.IdentityGuid;
            companyMap.AssignedName = company.Name;
            companyMap.AssignedAddress = company.Address1;
            companyMap.AssignedCity = company.City;
            companyMap.AssignedState = company.State;

            companyMap.AssignedToID = this.Person.ID;
            companyMap.AssignedToGuid = this.Person.MasterRecordGuid;

            this.Session["AddingCarrierCompany"] = false;

            this.AssignedCompaniesDataGrid.EditItemIndex = -1;

            this.Person.AssignedCompaniesCollection.Sort(COMPANY_MAP_SORT_CRITERIA.ASSIGNED);

            this.EnableControls(true);
            this.UpdateView();
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
            this.EnableControls(true);
            this.UpdateView();
         }
      }

      // ReSharper disable once InconsistentNaming
      protected void AssignedCompaniesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
      {
         try
         {
            this.AssignedCompaniesDataGrid.EditItemIndex = e.Item.ItemIndex;
            this.EnableControls(false);
            this.UpdateView();
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
            this.AssignedCompaniesDataGrid.EditItemIndex = -1;
            this.EnableControls(true);
            this.UpdateView();
         }
      }

      // ReSharper disable once InconsistentNaming
      protected void AssignedCompaniesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
      {
         Label indexLabel = (Label)e.Item.FindControl("IndexLabel");
         if (indexLabel != null)
         {
            if (Convert.ToInt32(indexLabel.Text) > -1 &&
                Convert.ToInt32(indexLabel.Text) < this.Person.AssignedCompaniesCollection.Count) this.Person.AssignedCompaniesCollection.Remove(Convert.ToInt32(indexLabel.Text));
            this.UpdateView();
            this.SetAddButtonAccess(true);
         }
      }

      protected void EnableControls(bool enable)
      {
         if (this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
            this.AddButton.Enabled = enable;

         // Call the main form to disable buttons and tabs.
         PersonForm personForm = (PersonForm)this.Page;
         personForm.EnableControls(enable);
         this.SetAddButtonAccess(enable);
      }

      protected void LockedOutCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         if (!this.LockedOutCheckBox.Checked)
         {
            this.LockedOutDateTextbox.Text = "";
            this.LockedOutReasonTextbox.Text = "";
            this.LockedOutReasonTextbox.Enabled = false;
         }
         else
         {
            this.LockedOutReasonTextbox.Enabled = true;
            this.Person._LockedOutDate.Value = DateTime.UtcNow;
            this.LockedOutDateTextbox.Text = this.Person.LockedOutDate.ToString();
         }
      }

   }
}