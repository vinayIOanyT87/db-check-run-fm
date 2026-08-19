// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardLimitDetailForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Allows a user to view, add, and modify a Fuel Card Limit. Contains user control tabs which display fields 
// associated with Fuel Card Limits
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FuelCardWebApp
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMCore;
    using FMWebApp;
    using System;

    /// <summary>
    /// A user control which all fuel card limit detail tabs can derive from. 
    /// Used to provide access to the Fuel Card Limit we're viewing or editing.
    /// </summary>
    public class FuelCardLimitPageBase : FMUserControlBase
    {
        /// <summary>
        /// The Fuel Card Limit we're viewing or editing
        /// </summary>
        public FuelCardLimit FuelCardLimit
        {
            get
            {
                return ((FuelCardLimitDetailForm)this.Page).FuelCardLimit;
            }
        }
    }

    /// <summary>
    /// Allows a user to view, add, and modify a Fuel Card Limit. Contains user control tabs which display fields 
    /// associated with Fuel Card Limits
    /// </summary>
    public partial class FuelCardLimitDetailForm : FMFormBase
    {
        /// <summary>
        /// Get or set the Fuel Card Limit object from Session so that we can preserve it during postbacks
        /// </summary>
        private FuelCardLimit SessionFuelCardLimit
        {
            get
            {
                FuelCardLimit limit = this.Session["SessionFuelCardLimit"] as FuelCardLimit;
                return limit;
            }
            set
            {
                this.Session.Add("SessionFuelCardLimit", value);
            }
        }

        /// <summary>
        /// The fuel card limit we're viewing or editing
        /// </summary>
        public FuelCardLimit FuelCardLimit { get; set; }

        /// <summary>
        /// When the page loads, do things like retrieve the user's security rights and update the view
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                if (!this.Page.IsPostBack)
                {
                    this.UpdateView();
                    this.EnableControls(true);
                }
                else
                {
                    this.FuelCardLimit = this.SessionFuelCardLimit;
                }

                // Apply the data dictionary to the tab control headers
                this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");
                this.tpAssignedFuelCardsPage.HeaderText = this.GetTranslatedText("Assigned Fuel Cards");
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Populate the fields on the screen with data
        /// </summary>
        private void UpdateView()
        {
            try
            {
                string fuelCardLimitGuidString = this.Request.GetQueryOrFormValue("FuelCardLimitGuid");

                // The fuel card limit we're configuring is identified by a parameter contained in the page request
                if (!string.IsNullOrEmpty(fuelCardLimitGuidString))
                {
                    Guid fuelCardLimitGuid = Guid.Parse(fuelCardLimitGuidString);

                    // Get the fuel card limit we're modifying
                    this.FuelCardLimit = FMChannelHelper.MakeCall<IFuelCardLimits, FuelCardLimit>(fuelCardLimitServiceClass => fuelCardLimitServiceClass.Get(this.Security, fuelCardLimitGuid));
                }
                else // We are adding a new fuel card limit
                {
                    this.FuelCardLimit = new FuelCardLimit();
                }

                this.SessionFuelCardLimit = this.FuelCardLimit;
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Fires when the user presses the OK button.
        /// We save the data entered in the fields on the form.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        protected void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.FuelCardLimit != null)
                {
                    this.FuelCardLimitGeneralPage.UpdateData();

                    // If there is no identity guid, we are adding a new fuel card limit, otherwise, we're modifying an existing fuel card limit
                    if (this.FuelCardLimit.IdentityGuid == Guid.Empty)
                    {
                        FMChannelHelper.MakeCall<IFuelCardLimits>(fuelCardLimitServiceClass => fuelCardLimitServiceClass.Add(this.Security, this.FuelCardLimit));
                    }
                    else
                    {
                        FMChannelHelper.MakeCall<IFuelCardLimits>(fuelCardLimitServiceClass => fuelCardLimitServiceClass.Modify(this.Security, this.FuelCardLimit));
                    }
                }

				this.Redirect( "FuelCardLimitSummaryForm.aspx?CSRFToken=" + this.Security.CSRFToken );
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Fires when the user presses the Cancel button.
        /// We return the user to the fuel card limit summary form
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
				this.Redirect( "FuelCardLimitSummaryForm.aspx?CSRFToken=" + this.Security.CSRFToken);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Enable or disable controls on the form.
        /// This is used when the user is doing something like editing a row in a grid
        /// and we don't want them to switch tabs or pressing OK unless they cancel or save the edit 
        /// </summary>
        /// <param name="enable">True to enable the controls, false to disable them</param>
        public void EnableControls(bool enable)
        {
            // In order for the OK button to be enabled
            // you have to have the modify fuel card limit security right, and the fuel card limited being edited must be new or owned by the current site.
            this.btnOK.Enabled = enable && this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT)
                                  && (this.FuelCardLimit.SiteGuid == Guid.Empty
                                      || this.FuelCardLimit.SiteGuid == this.Security.SiteGuid);

            this.tcFuelCardLimit.HeaderEnabled = enable;
        }
     }
}