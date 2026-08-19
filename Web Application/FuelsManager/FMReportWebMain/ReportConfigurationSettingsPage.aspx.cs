// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportConfigurationSettingsPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportConfigurationSettingsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMWebApp;

	using global::FMWebApp;

	public partial class ReportConfigurationSettingsPage : FMFormBase, IEntityDiscovery
	{
		#region Constants and Fields

		private const string EntityName = "Default Reports and Settings";
		private string errorMsg001 = "Invalid entry";
		private string errorMsg002 = "Business objects not available";

		private string tokenCookie;

		/// <summary>
		/// Expose the menu bar so it can be refreshed by user controls (tabs) contained within this page
		/// </summary>
		public FMMenuBar MenuBar
		{
			get
			{
				return this.ucFMMenuBar;
			}
		}

		#endregion

		#region Explicit Interface Properties

		/// <summary>
		///    Gets a value indicating whether [entity assignable].
		/// </summary>
		/// <value>
		///    <c>true</c> if [entity assignable]; otherwise, <c>false</c>.
		/// </value>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		///    Gets the type of the entity engine.
		/// </summary>
		/// <value>
		///    The type of the entity engine.
		/// </value>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IReportConfigurationDetailProcessor);
			}
		}

		/// <summary>
		///    Gets the type of the entity.
		/// </summary>
		/// <value>
		///    The type of the entity.
		/// </value>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS;
			}
		}

		#endregion

		#region Explicit Interface Methods

		/// <summary>
		///    Enumerates the entity maps.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="type">The type.</param>
		/// <returns>An entity to site map collection.</returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (type == ENTITY_ASSIGNMENT_TYPE.OWNED)
			{
			}
			else
			{
				EntityToSiteMapClass entityToSiteMap = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
																	 x =>
																	 x.Get(security, ((IEntityDiscovery)this).EntityType, security.LoginSiteGuid)
																);

				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.LoginSiteGuid == entityToSiteMap.IdentityGuid)
					{
						entityToSiteMap.ID = EntityName;
						entityToSiteMapCollection.Add(entityToSiteMap);
					}
				}
				else
				{
					if (entityToSiteMap.IdentityGuid == Guid.Empty)
					{
						entityToSiteMap = new EntityToSiteMapClass
							{
								SiteGuid = Guid.Empty,
								ID = EntityName,
								TypeID = ((IEntityDiscovery)this).EntityType,
								IdentityGuid = security.SiteGuid
							};

						entityToSiteMapCollection.Add(entityToSiteMap);
					}
				}
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		///    Gets the identity GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="id">The id.</param>
		/// <returns>The Identity Guid of the object.</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			EntityToSiteMapClass entityToSiteMap = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
																	 x =>
																	 x.Get(security, ((IEntityDiscovery)this).EntityType, security.LoginSiteGuid)
																);

			return (entityToSiteMap.IdentityGuid == Guid.Empty) ? security.SiteGuid : entityToSiteMap.IdentityGuid;
		}

		/// <summary>
		///    Sets the site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="guid">The GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    This is the main entry point to the report configuration assignment page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack)
			{
				this.Security = (SecurityClass)this.Session["Security"];
				this.tokenCookie = (string)this.Session["SToken"];

				// Apply the data dictionary to the page.
				this.ApplyDataDictionary();
			}
			else
			{
				try
				{
					GetSecurity();
					if (this.Session["Token"] != null)
					{
						this.tokenCookie = this.Session["Token"] as string;
					}
					if (string.IsNullOrEmpty(this.tokenCookie))
					{
						this.HandleErrorCondition(this.errorMsg001 + "!");
					}
					this.Session.Add("SToken", this.tokenCookie);

					// Apply the data dictionary to the page.
					this.ApplyDataDictionary();

					// Disable the add buttons if the user does not have modify permissions.
					this.CheckPriviledges();
				}
				catch (Exception exception)
				{
					string msg = exception.Message;

					if (msg.StartsWith("Thread was being aborted.") == false)
					{
						this.HandleErrorCondition(this.errorMsg002 + "!");
					}
				}
			}
		}

		/// <summary>
		///    This method will apply the data dictionary to this page.  If the data dictionary
		///    use flag is set to true, then it will apply data dictionary.
		/// </summary>
		private void ApplyDataDictionary()
		{
			this.errorMsg001 = this.GetTranslatedText(this.errorMsg001);
			this.errorMsg002 = this.GetTranslatedText(this.errorMsg002);
			this.ReportLabel.Text = this.GetTranslatedText(this.ReportLabel.Text);
			this.tpReportsPage.HeaderText = this.GetTranslatedText(this.tpReportsPage.HeaderText);
			this.tpDirectoriesPage.HeaderText = this.GetTranslatedText(this.tpDirectoriesPage.HeaderText);
		}

		/// <summary>
		///    This method will check the security privileges and disable edit type functionality.
		/// </summary>
		private void CheckPriviledges()
		{
		}

		/// <summary>
		///    This method will check to see if there is an error, if so, then it will display an
		///    error dialog and transfer control to the error page.
		/// </summary>
		/// <param name="erroMsg"></param>
		private void HandleErrorCondition(string errMsg)
		{
			if (string.IsNullOrEmpty(errMsg) == false)
			{
				this.RenderErrorMessage(errMsg);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		#endregion
	}
}