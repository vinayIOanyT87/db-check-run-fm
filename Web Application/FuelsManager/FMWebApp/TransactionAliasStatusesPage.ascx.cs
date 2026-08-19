// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasStatusesPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	public partial class TransactionAliasStatusesPage : FMUserControlBase
	{
		#region Constants and Fields

		protected TransactionAliasClass TransactionAlias;

		#endregion

		#region Methods

		/// <summary>
		/// Inserts newItem into listBox in sorted order
		/// </summary>
		/// <param name="listBox">
		/// </param>
		/// <param name="newItem">
		/// </param>
		/// <param name="bIncludeInDefaultCombo">
		/// The b Include In Default Combo.
		/// </param>
		protected void InsertIntoListBoxSorted(FMListBox listBox, ListItem newItem, bool bIncludeInDefaultCombo)
		{
			// Search for the right place to insert the item into the list box
			foreach (ListItem listBoxItem in listBox.Items)
			{
				if (listBoxItem.Text.CompareTo(newItem.Text) > 0)
				{
					int idx = listBox.Items.IndexOf(listBoxItem);
					listBox.Items.Insert(idx, newItem);

					if (bIncludeInDefaultCombo)
					{
						this.ddlDefaultStatus.Items.Insert(idx, new ListItem(newItem.Text, newItem.Value));
					}

					return;
				}
			}

			// If we did not find the right place, add the item to the end.
			listBox.Items.Add(newItem);

			if (bIncludeInDefaultCombo)
			{
				this.ddlDefaultStatus.Items.Add(new ListItem(newItem.Text, newItem.Value));
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.TransactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

				if (!this.IsPostBack)
				{
					// Enable/Disable assign/unassign buttons based on rights
					if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
					{
						this.btnAssign.Enabled = false;
						this.btnUnassign.Enabled = false;
						this.ddlDefaultStatus.Enabled = false;
					}

					// Populate the list boxes
					this.PopulateStatusListBoxes();
                    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///     Populates both Assigned and Available list boxes and the Default Status drop down
		/// </summary>
		protected void PopulateStatusListBoxes()
		{
			// Clear the list boxes
			this.lbxAvailable.Items.Clear();
			this.lbxAssigned.Items.Clear();
			this.ddlDefaultStatus.Items.Clear();

			var statuses = (int[])Enum.GetValues(typeof(TransactionStatus));

			foreach (int status in statuses)
			{
				string display = Enum.GetName(typeof(TransactionStatus), status);
				var listValue = (int)Enum.Parse(typeof(TransactionStatus), display);
				var listItem = new ListItem(this.GetTranslatedText(display), listValue.ToString());

				ArrayList assigned = this.TransactionAlias.AssignedStatuses;
				if (assigned.Contains(status))
				{
					// The status has been assigned to the alias so
					// put it in the assigned list box and the default combo
					this.InsertIntoListBoxSorted(this.lbxAssigned, listItem, true);
				}
				else
				{
					// The status has not been assigned to the alias so
					// put it in the unassigned list box but not the default combo
					this.InsertIntoListBoxSorted(this.lbxAvailable, listItem, false);
				}
			}

			// Add in the no-selection option
			this.ddlDefaultStatus.Items.Insert(0, new ListItem("--Select--", "-1"));

			// Set the selection value
			this.SetDefaultStatusSelection();
		}

		protected void SetDefaultStatusSelection()
		{
			int idx = 0;

			foreach (ListItem item in this.ddlDefaultStatus.Items)
			{
				if (item.Value == this.TransactionAlias.LookupDefaultStatusIndex.ToString(CultureInfo.InvariantCulture))
				{
					this.ddlDefaultStatus.SelectedIndex = idx;
					return;
				}

				++idx;
			}

			// Default to the "--Select--" option
			this.ddlDefaultStatus.SelectedIndex = 0;
		}

		protected void btnAssign_Click(object sender, EventArgs e)
		{
			// Determine if a status was selected from the available statuses
			ListItem status;
			while ((status = this.lbxAvailable.SelectedItem) != null)
			{
				this.lbxAvailable.Items.Remove(status);
				status.Selected = false;

				// Add the status to the Transaction alias
				this.TransactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
				this.TransactionAlias.AssignedStatuses.Add(Convert.ToInt32(status.Value));
			}

			// Repopulate the drop down list boxes.  Another method was used originally
			// that did not require the repopulation, however, removing more than one
			// status caused the list box to vanish
			this.PopulateStatusListBoxes();
		}

		protected void btnUnassign_Click(object sender, EventArgs e)
		{
			// Determine if a status was selected from the assinged statuses
			ListItem status;
			while ((status = this.lbxAssigned.SelectedItem) != null)
			{
				this.lbxAssigned.Items.Remove(status);
				status.Selected = false;

				// Remove the status from the transaction alias
				this.TransactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
				this.TransactionAlias.AssignedStatuses.Remove(Convert.ToInt32(status.Value));
			}

			// Repopulate the drop down list boxes.  Another method was used originally
			// that did not require the repopulation, however, removing more than one
			// status caused the list box to vanish
			this.PopulateStatusListBoxes();
		}

		protected void ddlDefaultStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.TransactionAlias.LookupDefaultStatusIndex = Convert.ToInt32(this.ddlDefaultStatus.SelectedValue);
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (versionSpecificFields != null && (transactionAlias.IdentityGuid.Equals(Guid.Empty)
                                              || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid))))
            {
                return;
            }

            if (versionSpecificFields != null)
            {
                this.btnAssign.Enabled = (this.btnAssign.Enabled && versionSpecificFields.Contains("Statuses"));
                this.btnUnassign.Enabled = (this.btnUnassign.Enabled && versionSpecificFields.Contains("Statuses"));
                this.ddlDefaultStatus.Enabled = (this.ddlDefaultStatus.Enabled && versionSpecificFields.Contains("LookupDefaultStatusIndex"));
                this.labDefaultStatus.Enabled = (this.labDefaultStatus.Enabled && versionSpecificFields.Contains("LookupDefaultStatusesIndex"));
                this.lbxAssigned.Enabled = (this.lbxAssigned.Enabled && versionSpecificFields.Contains("Statuses"));
                this.lbxAvailable.Enabled = (this.lbxAvailable.Enabled && versionSpecificFields.Contains("Statuses"));
                this.labAvailable.Enabled = (this.labAvailable.Enabled && versionSpecificFields.Contains("Statuses"));
            }
        }

		#endregion
	}
}