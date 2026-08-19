// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListOfDispatchers.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Displays a list of currently logged on dispatachers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	///    This class is responsible for displaying a list of Dispatchers currently logged into the system.
	/// </summary>
	public partial class ListOfDispatchers : FMFormBase
	{
		/// <summary>
		/// If a user has no name specified, this value will be displayed instead of blank 
		/// in the name grid column
		/// </summary>
		private const string NoNameDisplayValue = "<Not Provided>";

		#region Public Methods and Operators

		/// <summary>
		///    Identifies the data dictionary keys needed for this page.
		/// </summary>
		/// <param name="security">
		///    The current security object.
		/// </param>
		/// <returns>
		///    An array of data dictionary keys.
		/// </returns>
		public string[] Keys(SecurityClass security)
		{
			string[] keys = {	"Dispatchers Logged Into System",
								"User ID",
								"Name",
								"Close",
								NoNameDisplayValue
							};

			return keys;
		}

		#endregion

		#region Methods

		/// <summary>
		///    Page_Load event handler for page.
		/// </summary>
		/// <param name="sender">The sender parameter</param>
		/// <param name="e">The event args parameter</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					this.Session["NavigateAction"] = this.Request.QueryString["navigateAction"];
				}

				this.DispatchersDataGrid.DataSource = this.RetrieveListOfDispatchers();
				this.DispatchersDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Closes the form and redirects client to previous page or FuelsManager home page.
		///    If a close button click was used to navigate to this page then the FuelsManager
		///    home page will be displayed when this page is closed.  Otherwise the previous
		///    page will be displayed.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void CloseOnClick(object sender, EventArgs e)
		{
			try
			{
				// If the menu bar was used to navigate to this page then the URL of the previous
				// page will be stored in the PreviousMenuItemUrl property.  If an open button
				// click was used to navigate to this page then the URL of the previous page
				// will be stored in the CurrentMenuItemUrl property.  The navigate action is
				// only provided on open and close button clicks.  A null or empty navigate
				// action indicates the menu bar was used to navigate to this page.
				var navigateAction = this.Session["NavigateAction"] as string;
				string redirectPageUrl;
				if (string.IsNullOrEmpty(navigateAction))
				{
					redirectPageUrl = this.ucFMMenuBar.PreviousMenuItemUrl;
				}
				else if (navigateAction == "openClick")
				{
					redirectPageUrl = this.ucFMMenuBar.CurrentMenuItemUrl;
				}
				else
				{
					redirectPageUrl = FMMenuBar.FuelsManagerHomePageUrl;
				}

				this.Redirect(redirectPageUrl + "?navigateAction=closeClick");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will retrieve the list of Dispatchers that have the VIEW_DISPATCH and MODIFY_DISPATCH rights assigned.
		/// </summary>
		/// <returns>List of users with view or modify dispatch rights</returns>
		private List<UserClass> RetrieveListOfDispatchers()
		{
			List<UserClass> retVal = null;
			var sessionsWithRights = new List<SessionClass>();

			// Get distinct list of currently logged on user sessions
			var loggedOnSessions = FMChannelHelper.MakeCall<ISessions, SessionClassCollection>(
				sessions => sessions.GetDistinctUserSessions(this.Security));
			if (loggedOnSessions.Count > 0)
			{
				sessionsWithRights.AddRange(loggedOnSessions);
				FMChannelHelper.MakeCall<IUsers>(
				users =>
				{
					foreach (SessionClass session in loggedOnSessions)
					{
						var loggedOnSession = session;  // temp variable assignment to avoid potential closure problem
						UserClass user = users.Get(this.Security, loggedOnSession.UserGuid);
						if (user != null)
						{
							// Get list of group rights
							bool hasBothRights = false;
							FMChannelHelper.MakeCall<IGroupRightMaps>(
								groupRightMaps =>
								{
									foreach (UserGroupMapClass userGroupMap in user.UserGroupMapCollection)
									{
										// Get list of group to rights with View Dispatch or Modify Dispatch security right
										bool viewDispatchRight = groupRightMaps.GroupHasRight(
											this.Security, false, userGroupMap.GroupGuid, RIGHT.VIEW_DISPATCH);
										bool modifyDispatchRight = groupRightMaps.GroupHasRight(
											this.Security, false, userGroupMap.GroupGuid, RIGHT.MODIFY_DISPATCH);
										if (viewDispatchRight & modifyDispatchRight)
										{
											hasBothRights = true;
										}
									}

									if (!hasBothRights)
									{
										sessionsWithRights.Remove(loggedOnSession);
									}
								});
						}
					}

					if (sessionsWithRights.Count > 0)
					{
						retVal = new List<UserClass>();
						foreach (SessionClass session in sessionsWithRights)
						{
							UserClass userClass = users.Get(this.Security, session.UserGuid);

							if (userClass != null && !string.IsNullOrEmpty(userClass.ID))
							{
								// Per feedback from Greg Kendall, display a default value for the name of the user 
								// if they have no name.
								if (string.IsNullOrEmpty(userClass.Name))
								{
									userClass.Name = HttpUtility.HtmlEncode(ListOfDispatchers.NoNameDisplayValue);
								}

								retVal.Add(userClass);
							}
						}
					}
				});
			}

			return retVal;
		}

		#endregion
	}
}