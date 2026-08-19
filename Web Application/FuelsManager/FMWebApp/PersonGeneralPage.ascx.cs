/******************************************************************************
	FILE NAME:		PersonGeneralPage.ascx.cs
	PURPOSE:		Implementation of PersonGeneralPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	7.4.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-11-09	Richard Panachida	Fixed data dictionary issues (CSI 3375).

		2008-17-04	Chris Knight		Add support for capture/clear stored signature - CSI 5503
 
		2008-12-12  A. Coker          Regional settings for date and time is based on entity's site index. In
												case entity is new and a site index is not assigned to it, use site index acquired
												from page's security object.
 
		2009-05-27	W.Gray				Added support for TWIC card enrollment (CSI 3501)

		2009-07-31	A. Coker	         WI 5055 - Moved some fields to Additional Data and Load Rack pages.
 
		2009-10-27	I.Orndorff			- Modified "UpdatePersonRoles()" to Redirect back to the main "PersonForm".
												  This will trigger the "Driver" page to be visible or not. This also fixes
												  bug #8813.
 
*******************************************************************************/

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMControls;

    using FMCore;

    /// <summary>
	/// Summary description for PersonGeneralPage.
	/// </summary>
	public partial class PersonGeneralPage : PersonPageBase
	{
		protected FMLabel Label20;
		protected TextBox PersonIDTextbox;
		protected FMLabel Label21;
		protected DropDownList PersonNameDropDownList;
		protected SiteClass CurrentSite;
	

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

			if (this.Person.IdentityGuid.Equals(Guid.Empty)
				  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid))
				  || (this.VersionSpecificFields == null))
			{
				return;
			}

			if (!this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid))
			{
				this.IDTextbox.Enabled = false;
				this.FirstNameTextbox.Enabled = false;
				this.MiddleNameTextbox.Enabled = false;
				this.LastNameTextbox.Enabled = false;
			}
            this.IDTextbox.Enabled = (this.IDTextbox.Enabled && this.VersionSpecificFields.Contains("PersonId"));
            this.FirstNameTextbox.Enabled = (this.FirstNameTextbox.Enabled && this.VersionSpecificFields.Contains("FirstName"));
            this.MiddleNameTextbox.Enabled = (this.MiddleNameTextbox.Enabled && this.VersionSpecificFields.Contains("MiddleName"));
            this.LastNameTextbox.Enabled = (this.LastNameTextbox.Enabled && this.VersionSpecificFields.Contains("LastName"));

            this.IDTextbox.Enabled = (this.IDTextbox.Enabled);
		    this.AssignRolesButton.Enabled = (this.AssignRolesButton.Enabled 
                                                && this.VersionSpecificFields.Contains("Roles"));
		    this.UnassignRolesButton.Enabled = (this.UnassignRolesButton.Enabled
                                                  && this.VersionSpecificFields.Contains("Roles"));
		    this.AssignedRolesListBox.Enabled = (this.AssignedRolesListBox.Enabled 
                                                    && this.VersionSpecificFields.Contains("Roles"));
		    this.UnassignedRolesListbox.Enabled = (this.UnassignRolesButton.Enabled  
                                                    && this.VersionSpecificFields.Contains("Roles"));
            this.Address1Textbox.Enabled = (this.Address1Textbox.Enabled 
											  && this.VersionSpecificFields.Contains("Address1"));
			this.Address2Textbox.Enabled = (this.Address2Textbox.Enabled 
								  && this.VersionSpecificFields.Contains("Address2"));
			this.CityTextbox.Enabled = (this.CityTextbox.Enabled 
								  && this.VersionSpecificFields.Contains("City"));
			this.StateTextbox.Enabled = (this.StateTextbox.Enabled 
								  && this.VersionSpecificFields.Contains("State"));
			this.ZipTextbox.Enabled = (this.ZipTextbox.Enabled 
					  && this.VersionSpecificFields.Contains("Zip"));
			this.CountryTextbox.Enabled = (this.CountryTextbox.Enabled 
								  && this.VersionSpecificFields.Contains("Country"));
			this.Phone1Textbox.Enabled = (this.Phone1Textbox.Enabled 
								  && this.VersionSpecificFields.Contains("Phone1"));
			this.Phone2Textbox.Enabled = (this.Phone2Textbox.Enabled 
								  && this.VersionSpecificFields.Contains("Phone2"));
			this.TitleTextbox.Enabled = (this.TitleTextbox.Enabled 
								  && this.VersionSpecificFields.Contains("Title"));
			this.EMailTextbox.Enabled = (this.EMailTextbox.Enabled 
								  && this.VersionSpecificFields.Contains("Email"));
			this.AssignRolesButton.Enabled = (this.AssignRolesButton.Enabled 
								  && this.VersionSpecificFields.Contains("Roles"));
			this.UnassignRolesButton.Enabled = (this.UnassignRolesButton.Enabled 
								  && this.VersionSpecificFields.Contains("Roles"));
            this.HiddenCheckBox.Enabled = this.HiddenCheckBox.Enabled 
                && this.VersionSpecificFields.Contains("HiddenDate");
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
					this.IDTextbox.Text = this.Person.ID;
					this.FirstNameTextbox.Text = this.Person.FirstName;
					this.MiddleNameTextbox.Text = this.Person.MiddleName;
					this.LastNameTextbox.Text = this.Person.LastName;
					this.Address1Textbox.Text = this.Person.Address1;
					this.Address2Textbox.Text = this.Person.Address2;
					this.CityTextbox.Text = this.Person.City;
					this.StateTextbox.Text = this.Person.State;
					this.ZipTextbox.Text = this.Person.Zip;
					this.CountryTextbox.Text = this.Person.Country;
					this.Phone1Textbox.Text = this.Person.Phone1;
					this.Phone2Textbox.Text = this.Person.Phone2;
					this.TitleTextbox.Text = this.Person.Title;
					this.EMailTextbox.Text = this.Person.Email;

					this.PopulateRoleLists();

					Guid siteGuid = (this.Person.SiteGuid == Guid.Empty) ? this.Security.SiteGuid : this.Person.SiteGuid;
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
													x =>
													x.Get(this.Security, siteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
															bGetAssociatedAliases: true)
											);

					DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();

					this.Person._LastActivityDate.Format = dateTimeFormatInfo;
					this.Person._LockedOutDate.Format = dateTimeFormatInfo;

					this.SetFieldAccessibilityForChildRecordVersion();
				}
			}	
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void PopulateRoleLists ()
		{
			this.AssignedRolesListBox.Items.Clear();
			this.UnassignedRolesListbox.Items.Clear();
			
			// AssignedRolesListBox
			foreach (PersonRoleMapClass roleMap in this.Person.RoleCollection)
			{
				var unassignedRoleItem = new ListItem( PersonRoleMapClass.RoleID( roleMap.Role ), 
														((int)roleMap.Role).ToString(CultureInfo.InvariantCulture) );
				this.AssignedRolesListBox.Items.Add( unassignedRoleItem );
			}

			// UnassignedRolesListBox
			for (PERSON_ROLE role = PERSON_ROLE.LOADER_ROLE; role < PERSON_ROLE.MAX_PERSON_ROLE; role++)
			{
				if (null == this.AssignedRolesListBox.Items.FindByValue( ((int)role).ToString(CultureInfo.InvariantCulture) ))
				{
					var assignedRoleItem = new ListItem( PersonRoleMapClass.RoleID( role ), 
														((int)role).ToString(CultureInfo.InvariantCulture) );
					this.UnassignedRolesListbox.Items.Add( assignedRoleItem );
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.UnassignRolesButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.UnassignRolesButtonCommand);
			this.AssignRolesButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AssignRolesButtonCommand);
		}
		#endregion

		public void UpdateData()
		{
			if (this.Person != null)
			{
				if (this.EMailTextbox.Text.IsValidEmailAddressSyntax() == false)
				{
					throw new FMEmailFormatException();
				}

				this.Person.ID = this.IDTextbox.Text;
				this.Person.FirstName = this.FirstNameTextbox.Text;
				this.Person.MiddleName = this.MiddleNameTextbox.Text;
				this.Person.LastName = this.LastNameTextbox.Text;
				this.Person.Address1 = this.Address1Textbox.Text;
				this.Person.Address2 = this.Address2Textbox.Text;
				this.Person.City = this.CityTextbox.Text;
				this.Person.State = this.StateTextbox.Text;
				this.Person.Zip = this.ZipTextbox.Text;
				this.Person.Country = this.CountryTextbox.Text;
				this.Person.Phone1 = this.Phone1Textbox.Text;
				this.Person.Phone2 = this.Phone2Textbox.Text;
				this.Person.Title = this.TitleTextbox.Text;
				this.Person.Email = this.EMailTextbox.Text;

                // Only set the hidden date if the hidden check box is checked and there isn't already a value
                if (this.HiddenCheckBox.Checked && !this.Person.HiddenDate.HasValue)
                {
                    this.Person.HiddenDate = DateTimeOffset.Now;
                }
                else if (!this.HiddenCheckBox.Checked)
                {
                    this.Person.HiddenDate = null;
                }
			}
		}
		 
		private void AssignRolesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedRoleItem;

			while((unassignedRoleItem=this.UnassignedRolesListbox.SelectedItem) != null)
			{
				this.UnassignedRolesListbox.Items.Remove(unassignedRoleItem);
				unassignedRoleItem.Selected=false;
				this.AssignedRolesListBox.Items.Add(unassignedRoleItem);
			}

			this.UpdatePersonRoles();
		}

		private void UnassignRolesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedRoleItem;

			while((assignedRoleItem=this.AssignedRolesListBox.SelectedItem) != null)
			{
				this.AssignedRolesListBox.Items.Remove(assignedRoleItem);
				assignedRoleItem.Selected=false;
				this.UnassignedRolesListbox.Items.Add(assignedRoleItem);
			}
		
			this.UpdatePersonRoles();
		}
		
		private void UpdatePersonRoles()
		{
			try
			{
				((PersonForm) this.Page).UpdateData();

				var roleCollection = new PersonRoleMapCollectionClass();

				foreach(ListItem assignedRoleItem in this.AssignedRolesListBox.Items)
				{
					var	roleMap = new PersonRoleMapClass
					   	          {
						   	          Role = (PERSON_ROLE)Convert.ToInt32(assignedRoleItem.Value),
						   	          PersonGuid = this.Person.IdentityGuid
					   	          };
					roleCollection.Add(roleMap);
				}

				this.Person.RoleCollection=roleCollection;
				this.PopulateRoleLists();
			}
			catch(Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			
			string redirectString="PersonForm.aspx";
			
			var fmForm = this.Page as FMFormBase;

			if (fmForm != null && fmForm.IsFromDispatch)
			{
				redirectString += "?DispatchEdit=" + fmForm.DispatchEntityGuid;
			}			
			else if (fmForm != null && fmForm.IsFromQueryWriter)
			{
				redirectString += "?QueryEdit=" + fmForm.QueryEntityGuid;
			}

			this.Redirect(redirectString);
		}
	}
}

