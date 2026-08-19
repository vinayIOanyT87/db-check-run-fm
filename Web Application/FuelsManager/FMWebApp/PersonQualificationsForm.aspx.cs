// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonQualificationsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PersonQualificationsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for PersonQualificationsForm.
	/// </summary>
	public partial class PersonQualificationsForm : QualificationsFormBase,
	                                                IEntityDiscovery,
	                                                IMenuDiscovery
	{
		#region Explicit Interface Properties
		bool IEntityDiscovery.EntityAssignable
		{
			get { return true; }
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get { return typeof(IQualifications); }
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get { return ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION; }
		}
		#endregion

		#region Properties
		protected override DataGrid ApplicationDataGrid
		{
			get { return this.QualificationsDataGrid; }
		}

		protected override QUALIFICATION_TYPE QualificationType
		{
			get { return QUALIFICATION_TYPE.PERSON_QUALIFICATION; }
		}
		#endregion

		#region Public Methods and Operators
		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.CONFIGURE_QUALIFICATIONS))
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			{
				return null;
			}


			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ASSETS_PERSONNEL_QUALIFICATIONS,
						RootMenuName = "Assets",
						CategoryName = "Personnel",
						ItemName = "Qualifications",
						NavigateUrl = "PersonQualificationsForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}
		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			QualificationCollectionClass qualificationCollection = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
				x =>
					x.EnumerateByType(security, this.QualificationType)
				);

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (QualificationClass qualification in qualificationCollection)
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == qualification.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != qualification.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != qualification.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(qualification);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}
			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IQualifications, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security,this.QualificationType, id)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			QualificationClass qualification = FMChannelHelper.MakeCall<IQualifications, QualificationClass>(
																	 x =>
																	 x.Get(security, guid)
																);

			qualification.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IQualifications>(x => x.Modify(security, qualification));
		}
		#endregion

		#region Methods
		protected override void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;

			this.PersonQualificationsFormPageSizeDropDown.Enabled = enable;
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					// Enumerate 
					QualificationCollectionClass qualificationCollection = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, this.QualificationType)
																);

					this.Session["QualificationCollection"] = qualificationCollection;

					if (!this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
					{
						this.EnableControls(false);
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected override void UpdateView()
		{
			this.UpdateView(this.PersonQualificationsFormPageSizeDropDown);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command							+= this.AddButtonCommand;
			this.QualificationsDataGrid.EditCommand			+= this.QualificationsDataGridEditCommand;
			this.QualificationsDataGrid.PageIndexChanged	+= this.QualificationsDataGridPageIndexChanged;
			this.QualificationsDataGrid.CancelCommand		+= this.QualificationsDataGridCancelCommand;
			this.QualificationsDataGrid.UpdateCommand		+= this.QualificationsDataGridUpdateCommand;
			this.QualificationsDataGrid.DeleteCommand		+= this.QualificationsDataGridDeleteCommand;
			this.QualificationsDataGrid.ItemDataBound		+= this.QualificationsDataGridItemDataBound;
			this.AddButton.Command							+= this.AddButtonCommand;
		}
		#endregion
	}
}