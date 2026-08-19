// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardLimitAssignedFuelCardsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// The Assigned Fuel Cards tab for the Fuel Card Limit Detail Form. Displays information about Fuel Cards associated with the Fuel Card Limit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FuelCardWebApp
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.UtilityObjects;

	/// <summary>
    /// The Assigned Fuel Cards tab for the Fuel Card Limit Detail Form. Displays information about Fuel Cards associated with the Fuel Card Limit.
    /// </summary>
    public partial class FuelCardLimitAssignedFuelCardsPage : FuelCardLimitPageBase
    {
        /// <summary>
        /// Enable or disable controls on the form.
        /// This is used when the user is doing something like editing a row in a grid
        /// and we don't want them to switch tabs or pressing OK unless they cancel or save the edit 
        /// </summary>
        /// <param name="enable">True to enable the controls, false to disable them</param>
        public void EnableControls(bool enable)
        {
            bool enableControls = enable && this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT)
                                  && (this.FuelCardLimit.SiteGuid == Guid.Empty
                                      || this.FuelCardLimit.SiteGuid == this.Security.SiteGuid);

            this.AssignButton.Enabled = enableControls;
            this.UnassignButton.Enabled = enableControls;

            FuelCardLimitDetailForm detailForm = (FuelCardLimitDetailForm)this.Page;
            detailForm.EnableControls(enable);
        }

        /// <summary>
        /// When the page loads, display the assigned fuel cards for the fuel card limit
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.Page.IsPostBack)
                {
                    this.UpdateView();
                    this.EnableControls(true);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user changes the page, change the page and update the view
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Identifies the page selected by the user.</param>
        protected void AssignedFuelCardsGrid_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.EnableControls(true);
                this.AssignedFuelCardsGrid.PageIndex = e.NewPageIndex;

                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Update the view when the user changes the grid page size
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void PageSizeDropDown_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.EnableControls(true);
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// The assignmentTextBox is a hidden control which receives values selected from the fuel card assignment form.
        /// When the text is changed, that means the user just got back from assigning fuel cards. 
        /// We need to parse the value in the text box and update the screen to show the fuel cards selected.
        /// </summary>
        /// <param name="sender">The parameter is not used</param>
        /// <param name="e">The parameter is not used</param>
        protected void AssignmentTextBoxTextChanged(object sender, EventArgs e)
        {
            try
            {
                List<Guid> assignedGuids = new List<Guid>();

                if (!string.IsNullOrEmpty(this.AssignmentTextBox.Text))
                {
                    // Get the guids of the fuel cards the user assigned from the text box
                    List<string> assignedGuidStrings = this.AssignmentTextBox.Text.Split(';').ToList();

                    // Convert the strings to guids and add them to the list
                    foreach (string guidString in assignedGuidStrings)
                    {
                        Guid assignedGuid;

                        if (Guid.TryParse(guidString, out assignedGuid))
                        {
                            assignedGuids.Add(assignedGuid);
                        }
                    }

                    // Retrieve all fuel cards assigned to or owned by the site, and if the guids match, add it to the grid of assigned fuel cards
                    FuelCardCollectionClass fuelCards = FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(fuelCardsServiceClass => fuelCardsServiceClass.EnumerateFuelCards(this.Security));

                    foreach (Guid assignedGuid in assignedGuids)
                    {
                        FuelCardClass assignedFuelCard = fuelCards.Find(matchingFuelCard => matchingFuelCard.IdentityGuid == assignedGuid);

                        if (assignedFuelCard != null)
                        {
                            this.FuelCardLimit.AssignedFuelCards.Add(assignedFuelCard);
                        }
                    }
                }

                this.AssignmentTextBox.Text = string.Empty;
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// The UnassignmentTextBox is a hidden control which receives values selected from the fuel card assignment form.
        /// When the text is changed, that means the user just got back from unassigning fuel cards. 
        /// We need to parse the value in the text box and update the screen to remove the fuel cards selected.
        /// </summary>
        /// <param name="sender">The parameter is not used</param>
        /// <param name="e">The parameter is not used</param>
        protected void UnassignmentTextBoxTextChanged(object sender, EventArgs e)
        {
            try
            {
                List<Guid> unassignedGuids = new List<Guid>();

                if (!string.IsNullOrEmpty(this.UnassignmentTextBox.Text))
                {
                    // Get the guids of the fuel cards the user assigned from the text box
                    List<string> unassignedGuidStrings = this.UnassignmentTextBox.Text.Split(';').ToList();

                    // Convert the strings to guids and add them to the list
                    foreach (string guidString in unassignedGuidStrings)
                    {
                        Guid unassignedGuid;
                        if (Guid.TryParse(guidString, out unassignedGuid))
                        {
                            unassignedGuids.Add(unassignedGuid);
                        }
                    }

                    // Remove any fuel cards that were assigned to the limit from the limit if the guid is in the list of guids we just retreived 
                    // from the text box
                    foreach (Guid identityGuid in unassignedGuids)
                    {
	                    var index = this.FuelCardLimit.AssignedFuelCards.FindIndex(fuelCard => fuelCard.IdentityGuid == identityGuid);
	                    if (index >= 0)
	                    {
		                    this.FuelCardLimit.AssignedFuelCards.RemoveAt(index);
	                    }
                    }
                }

                this.UnassignmentTextBox.Text = string.Empty;
                this.UpdateView();
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
				var limits = new EnumerationLimits();
				int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.FUEL_CARD_ASSIGNMENT);

				if (this.FuelCardLimit.AssignedFuelCards.Count >= limit && limit > 0)
				{
					string str1 = "Results limited to first";
					string str2 = "records.";

					str1 = this.GetTranslatedText(str1);
					str2 = this.GetTranslatedText(str2);

					this.lblWarning.UseDataDictionary = false;
					this.lblWarning.Text = str1 + " " + limit + " " + str2;
					this.lblWarning.Visible = true;
				}
				else
				{
					this.lblWarning.Visible = false;
				}

                this.PageSizeDropDown.SetPageSize(this.AssignedFuelCardsGrid, this.FuelCardLimit.AssignedFuelCards.Count);
                this.BindData(this.FuelCardLimit.AssignedFuelCards);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Bind the assigned fuel cards to the grid
        /// </summary>
        /// <param name="assignedFuelCards">The assigned fuel cards to bind to the grid</param>
        private void BindData(List<FuelCardClass> assignedFuelCards)
        {
            this.AssignedFuelCardsGrid.DataSource = assignedFuelCards;
            this.AssignedFuelCardsGrid.DataBind();
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
			var limits = new EnumerationLimits();
			int pageLimit = limits.GetLimit(EnumerationLimits.EnumerationOptions.FUEL_CARD_ASSIGNMENT);
			this.PageSizeDropDown.SetLimit(pageLimit);
			this.AssignedFuelCardsGrid.PageSize = pageLimit;
		}
		#endregion
    }
}