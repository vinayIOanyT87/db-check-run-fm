// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardAssignmentForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Allows a user to select one or more fuel cards to assign or unassign to a Fuel Card Limit. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FuelsManager.FMWebApp;

namespace FuelsManager.FuelCardWebApp
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;
    using FMControls;
    using FMCore;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Represents a fuel card object that we can bind to the page's grid along with an indicator of whether or not it is currently
    /// selected (checked) for assignment or unassignment.
    /// </summary>
    [Serializable]
    public class FuelCardAssignment
    {
        /// <summary>
        /// The identity guid (primary key) of a fuel card
        /// </summary>
        public Guid IdentityGuid { get; set; }

        /// <summary>
        /// True if the user checked the check box for this fuel card to assign or unassign it
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// The manager of the fuel card
        /// </summary>
        public string ManagerID { get; set; }

        /// <summary>
        /// The BillTo of the fuel card
        /// </summary>
        public string BillToID { get; set; }

        /// <summary>
        /// The ID of the Fuel Card
        /// </summary>
        public string ID { get; set; }
    }

    /// <summary>
    /// Allows a user to select one or more fuel cards to assign or unassign to a Fuel Card Limit. 
    /// </summary>
    public partial class FuelCardAssignmentForm : FMFormBase
    {
        /// <summary>
        /// The text that appears in the grid when there are no fuel cards to display in unassignment mode
        /// </summary>
        private const string UnassignmentEmptyGridText = "No Fuel Cards Available for Unassignment";

        /// <summary>
        /// The text that should display in the header of the grid column when we're in unassignment mode.
        /// </summary>
        private const string UnassignmentGridHeaderText = "Unassign";

        /// <summary>
        /// The name of the variable in the page's query string that indicates whether we are assigning or unassigning fuel cards 
        /// </summary>
        private const string ModePageQueryStringVariableName = "Mode";

        /// <summary>
        /// True if we are assigning Fuel Cards, false if we are unassigning them
        /// </summary>
        public bool InAssignmentMode { get; set; }

        /// <summary>
        /// The identity guids of the fuel cards the user selected to assign or unassign.
        /// All of the guids are stored in a single string to return to the Fuel Card Limit
        /// </summary>
        public string SelectedIdentityGuids { get; set; }

        /// <summary>
        /// The fuel card assignments we are displaying on the page
        /// </summary>
        public List<FuelCardAssignment> FuelCardAssignments { get; set; }

        /// <summary>
        /// Stores the text the user searched on when the Find button is pressed
        /// </summary>
        private string SessionFindTextBoxSearchString
        {
            get
            {
                if (this.Session["FuelCardAssignmentFindSearchString"] is string)
                {
                    return this.Session["FuelCardAssignmentFindSearchString"] as string;
                }

                return string.Empty;
            }
            set
            {
                this.Session.Add("FuelCardAssignmentFindSearchString", value);
            }
        }

        /// <summary>
        /// Stores the fuel card assignment / unassignment information we are displaying in Session
        /// </summary>
        private List<FuelCardAssignment> SessionFuelCardAssignments
        {
            get
            {
                if (this.Session["SessionFuelCardAssignments"] is List<FuelCardAssignment>)
                {
                    return this.Session["SessionFuelCardAssignments"] as List<FuelCardAssignment>;
                }
                    
                return new List<FuelCardAssignment>();
            }
            set
            {
                this.Session.Add("SessionFuelCardAssignments", value);
            }
        }

        /// <summary>
        /// Get or set the Fuel Card Limit object from Session so that we can preserve it during postbacks
        /// </summary>
        private FuelCardLimit SessionFuelCardLimit
        {
            get
            {
                if (this.Session["SessionFuelCardLimit"] is FuelCardLimit)
                {
                    return (FuelCardLimit)this.Session["SessionFuelCardLimit"];
                }

                return null;
            }
        }

        /// <summary>
        /// When the page loads, get the user's security rights, determine which mode the form is operating in and which fuel card limit it is operating for, 
        /// and update the view.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                string modeString = this.Request.GetQueryOrFormValue(ModePageQueryStringVariableName);

                //  The page request also contains a value indicating whether we are in assignment or unassignment mode
                if (!string.IsNullOrEmpty(modeString))
                {
                    this.InAssignmentMode = String.Compare(modeString, "Assign", StringComparison.OrdinalIgnoreCase).Equals(0);
                }

                // Change the text displayed by the grid when no records are found if we're in unassignment mode.
                if (!this.InAssignmentMode)
                {
                    this.FuelCardsGrid.EmptyDataText = UnassignmentEmptyGridText;
                }

                if (!this.Page.IsPostBack)
                {
                    this.SessionFindTextBoxSearchString = string.Empty;

                    this.FuelCardAssignments = new List<FuelCardAssignment>();
                    this.UpdateView();
                }
                else
                {
                    this.FuelCardAssignments = this.SessionFuelCardAssignments;
                }
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
				this.PageSizeDropDown.SetPageSize(this.FuelCardsGrid, this.FuelCardAssignments.Count);
				this.PreserveFuelCardSelections();
				this.BindData(this.FuelCardAssignments);
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
        protected void FuelCardsGrid_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.FuelCardsGrid.PageIndex = e.NewPageIndex;
				this.PreserveFuelCardSelections();

				this.PageSizeDropDown.SetPageSize(this.FuelCardsGrid, this.FuelCardAssignments.Count);
				this.BindData(this.FuelCardAssignments);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user clicks the find button, limit the results to those
        /// that contain the value the user typed into the find box
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void FindButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.SessionFindTextBoxSearchString = this.FindTextBox.Text;

                this.FuelCardsGrid.PageIndex = 0;

				this.PreserveFuelCardSelections();
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user clicks the Show All button, 
        /// display all fuel cards that can be assigned or unassigned.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void ShowAllButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.FindTextBox.Text = string.Empty;
                this.SessionFindTextBoxSearchString = string.Empty;

                this.FuelCardsGrid.PageIndex = 0;

				this.PreserveFuelCardSelections();
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Retrieve fuel cards from the database and display them on the grid
        /// </summary>
        private void UpdateView()
        {
			var limits = new EnumerationLimits();
			int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.FUEL_CARD_ASSIGNMENT);

            var fuelCards = new FuelCardCollectionClass();

            FuelCardLimit fuelCardLimit = this.SessionFuelCardLimit;

            if (this.InAssignmentMode)
            {
                // When we're in assignment mode, get fuel cards not assigned to a limit, except for those assigned to the current limit.
                // If the user provided a value in the find box, use that value to filter the result set
                fuelCards = FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(fuelCardsServiceClass =>
                        fuelCardsServiceClass.EnumerateNotAssignedToFuelCardLimit(
                            this.Security,
                            fuelCardLimit.IdentityGuid,
                            string.IsNullOrWhiteSpace(this.SessionFindTextBoxSearchString) ? null : this.SessionFindTextBoxSearchString));

                // Remove any fuel cards that are already assigned from the list.
                // We do this because the fuel cards returned include those assigned to the current limit, which is necessary in case the user 
                // unassigns a fuel card and then wants to reassign it
                foreach (FuelCardClass assignedFuelCard in fuelCardLimit.AssignedFuelCards)
                {
                    fuelCards.RemoveAll(
                        alreadyAssignedFuelCard => alreadyAssignedFuelCard.IdentityGuid == assignedFuelCard.IdentityGuid);
                }
            }
            else
            {
                // When we're in unassignment mode, list the fuel cards that are currently assigned to the limit
                if (fuelCardLimit != null && fuelCardLimit.AssignedFuelCards != null)
                {
                    if (string.IsNullOrWhiteSpace(this.SessionFindTextBoxSearchString) == false)
                    {
                        fuelCards.AddRange(fuelCardLimit.AssignedFuelCards.Where(fuelCard => fuelCard.ID.IndexOf(this.SessionFindTextBoxSearchString, StringComparison.OrdinalIgnoreCase) >= 0
                            || fuelCard.ManagerID.IndexOf(this.SessionFindTextBoxSearchString, StringComparison.OrdinalIgnoreCase) >= 0
                            || fuelCard.BillToID.IndexOf(this.SessionFindTextBoxSearchString, StringComparison.OrdinalIgnoreCase) >= 0)
                            );
                    }
                    else
                    {
                        fuelCards.AddRange(fuelCardLimit.AssignedFuelCards);
                    }
                }
            }

            var newAssignments = new List<FuelCardAssignment>();

            if (fuelCards != null)
            {
                foreach (FuelCardAssignment assignment in fuelCards.Select(fuelCard =>
                    new FuelCardAssignment
                    {
                        ID = fuelCard.ID,
                        ManagerID = fuelCard.ManagerID,
                        BillToID = fuelCard.BillToID,
                        IsSelected = this.FuelCardAssignments.Find(existingAssignment => existingAssignment.IsSelected && existingAssignment.ID == fuelCard.ID) != null,
                        IdentityGuid = fuelCard.IdentityGuid
                    }))
                {
                    newAssignments.Add(assignment);
                }
            }

            this.FuelCardAssignments = newAssignments;
            this.SessionFuelCardAssignments = this.FuelCardAssignments;

            if (this.FuelCardAssignments.Count >= limit && limit > 0)
			{
				this.lblWarning.Text = string.Format("Results limited to first {0} records.  Use filters to narrow search.", limit);
				this.lblWarning.Visible = true;
			}
			else
			{
				this.lblWarning.Visible = false;
			}

			this.PageSizeDropDown.SetPageSize(this.FuelCardsGrid, this.FuelCardAssignments.Count);
			this.BindData(this.FuelCardAssignments);
        }

        /// <summary>
        /// When a row is bound to the grid, change the header text of the "Assign" column to "Unassign" if we're in unassignment mode
        /// </summary>
        /// <param name="sender">The parameter is not used</param>
        /// <param name="e">Identifies the row being bound</param>
        protected void FuelCardsGrid_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                // Change the grid view's column header from Assign to Unassign if we are in unassignment mode
                if (e.Row.RowType == DataControlRowType.Header && !this.InAssignmentMode)
                {
                    e.Row.Cells[0].Text = this.GetTranslatedText(UnassignmentGridHeaderText);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Bind the fuel card assignments provided to the grid
        /// </summary>
        /// <param name="fuelCards">The fuel card assignments to bind to the grid</param>
        private void BindData(IEnumerable fuelCards)
        {
            this.FuelCardsGrid.DataSource = fuelCards;
            this.FuelCardsGrid.DataBind();
        }

        /// <summary>
        /// When the user presses OK, store the assignments or unassignments the user 
        /// has selected and then return them to the Fuel Card Limit Detail Form
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void btnOK_OnClick(object sender, EventArgs e)
        {
            try
            {
				this.PreserveFuelCardSelections();

                foreach (FuelCardAssignment assignment in this.FuelCardAssignments)
                {
                    if (assignment.IsSelected)
                    {
                        this.SelectedIdentityGuids += assignment.IdentityGuid + ";";
                    }
                }

                // Register the javascript to close the form and return the selected fuel card guids to the Fuel Card Limit Detail Form
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ReturnSelectedFuelCardsScript", "ReturnSelectedFuelCards();", true);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the Select All button is pressed, check the box for all fuel cards displayed
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void SelectAllButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.ToggleSelectionForAll(true);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the Unselect All button is pressed, uncheck the box for all fuel cards displayed
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void UnselectAllButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.ToggleSelectionForAll(false);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Select or unselect all fuel cards displayed
        /// </summary>
        /// <param name="isSelected">True to select all fuel cards, false to unselect all fuel cards</param>
        private void ToggleSelectionForAll(bool isSelected)
        {
            List<FuelCardAssignment> assignments = this.FuelCardAssignments;

            assignments.ForEach(assignment => assignment.IsSelected = isSelected);

			this.BindData(assignments);
        }

        /// <summary>
        /// Preserve the selections (checkboxes checked or unchecked) by the user.
        /// This is used as an alternative to having a postback everytime the user checks a box.
        /// </summary>
        private void PreserveFuelCardSelections()
        {
            List<FuelCardAssignment> assignments = this.FuelCardAssignments;

            // Loop through each displayed fuel card and remember the selection made by the user
            foreach (GridViewRow row in this.FuelCardsGrid.Rows)
            {
                // Don't bother with rows that aren't data rows like the header or footer.
                if (row.RowType != DataControlRowType.DataRow)
                {
                    continue;
                }

                // Get the IdentityGuid of the Fuel Card bound to the row
                DataKey dataKey = this.FuelCardsGrid.DataKeys[row.RowIndex];

                if (dataKey != null)
                {
                    var guid = (Guid)dataKey.Value;
                 
                    // Find the fuel card matching the identity guid of the row
                    FuelCardAssignment assignment = assignments.Find(matchingAssignment => matchingAssignment.IdentityGuid == guid);

                    var checkBox = row.FindControl("AssignedCheckBox") as FMCheckBox;

                    // Update the assignment with the state of the checkbox.
                    if (checkBox != null && assignment != null)
                    {
                        assignment.IsSelected = checkBox.Checked;
                    }
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
			var limits = new EnumerationLimits();
			int pageLimit = limits.GetLimit(EnumerationLimits.EnumerationOptions.FUEL_CARD_ASSIGNMENT);
			this.PageSizeDropDown.SetLimit(pageLimit);
			this.FuelCardsGrid.PageSize = pageLimit;
		}
		#endregion
    }
}