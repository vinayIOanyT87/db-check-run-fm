/******************************************************************************
	FILE NAME:		EquipmentCompartmentsPage.ascx.cs
	PURPOSE:		Implementation of EquipmentCompartmentsPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2007-02-08	Richard Panachida	Added an override method to disable/enable controls. 
												Currently, it disables/enables the Add button (CSI 4083).

		2007-08-09	Richard Panachida	Added an event "CompartmentsDataGrid_PageIndxChange" to handle
												page index changes (CSI 5031).

		10-17-2008	V. Thompson			Work Task 264
												Changed Equipment.EquipmentSequence type from int to string
 
		11-10-2009	W.Gray				Change to not remove compartments with zero szfe fill on edit cancel (WI 9097)
*******************************************************************************/
namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    /// <summary>
	///		Summary description for EquipmentCompartmentsPage.
	/// </summary>
	public partial class EquipmentCompartmentsPage : EquipmentPageBase
	{
		protected FMControls.FMLabel Label3;
		protected DropDownList CarrierDropDownList;

		private void UpdateCompartmentsView()
		{
			this.CompartmentsDataGrid.DataSource = this.EnumerateCompartments();
			this.CompartmentsDataGrid.DataBind();
		}


		private ICollection EnumerateCompartments()
		{
			DataTable compartmentDataTable = new DataTable();

		    compartmentDataTable.Columns.Add("Index", typeof(Int32));
			compartmentDataTable.Columns.Add("Number", typeof(string));
			compartmentDataTable.Columns.Add("Capacity", typeof(string));
			compartmentDataTable.Columns.Add("SafeFill", typeof(string));

			int iItem = 0;
			foreach (EquipmentClass compartment in this.Equipment.CompartmentCollection)
			{
				var compartmentDataRow = compartmentDataTable.NewRow();

				compartmentDataRow["Index"] = iItem;
				compartmentDataRow["Number"] = compartment.EquipmentSequence;
				compartmentDataRow["Capacity"] = compartment.Capacity;
				compartmentDataRow["SafeFill"] = compartment.SafeFill;

				compartmentDataTable.Rows.Add(compartmentDataRow);
				iItem++;
			}
			DataView compartmentDataView = new DataView(compartmentDataTable);
			return compartmentDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.Equipment == null || this.Equipment.Type.IsMultiCompartmentCapable() == false )
				{
					this.AddButton.Visible = false;
					this.AddButton.Enabled = false;
					return;
				}

				if ((!this.Equipment.SiteGuid.Equals(Guid.Empty) && this.Equipment.SiteGuid != this.Security.SiteGuid)
				|| (!this.Equipment.IdentityGuid.Equals(Guid.Empty) && !this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid)))
                {
					this.AddButton.Enabled = false;
				}

				if (!this.Page.IsPostBack)
				{
					// CSI 3754 - Need to call view update only on non-postback; otherwise, the values in the controls
					// get overwritten before they can be read for saving.
					this.UpdateCompartmentsView();
					if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
						this.AddButton.Enabled = false;
				}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
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
			this.CompartmentsDataGrid.EditCommand += new DataGridCommandEventHandler(this.CompartmentsDataGridEditCommand);
			this.CompartmentsDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.CompartmentsDataGridPageIndxChange);
			this.CompartmentsDataGrid.CancelCommand += new DataGridCommandEventHandler(this.CompartmentsDataGridCancelCommand);
			this.CompartmentsDataGrid.UpdateCommand += new DataGridCommandEventHandler(this.CompartmentsDataGridUpdateCommand);
			this.CompartmentsDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.CompartmentsDataGridDeleteCommand);
			this.CompartmentsDataGrid.ItemDataBound += new DataGridItemEventHandler(this.CompartmentsDataGridItemDataBound);
			this.AddButton.Command += new CommandEventHandler(this.AddButtonCommand);

		}
		#endregion


		/// <summary>
		/// This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
            if ((this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)) && (this.Equipment.SiteGuid == this.Security.SiteGuid) && ((this.Equipment.IdentityGuid.Equals(Guid.Empty)) || (this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid))))
				this.AddButton.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			EquipmentForm equipmentForm = (EquipmentForm)this.Page;
			equipmentForm.EnableControls(enable);
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			EquipmentCollectionClass compartments = this.Equipment.CompartmentCollection;
		    EquipmentClass compartment = new EquipmentClass
		                                 {
		                                     Type = EQUIPMENT_TYPE.COMPARTMENT_TYPE,
		                                     EquipmentSequence = (compartments.Count + 1).ToString(),
		                                     IdentityGuid = Guid.Empty,
		                                     ParentEquipmentGuid = Guid.Empty
		                                 };


		    compartments.Add(compartment);
			this.CompartmentsDataGrid.CurrentPageIndex = (compartments.Count - 1) / this.CompartmentsDataGrid.PageSize;
			this.CompartmentsDataGrid.EditItemIndex = (compartments.Count - 1) % this.CompartmentsDataGrid.PageSize;

			this.EnableControls(false);
			this.UpdateCompartmentsView();
		}

		private void CompartmentsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				EquipmentCollectionClass compartments = this.Equipment.CompartmentCollection;
				EquipmentClass compartment = compartments[Convert.ToInt32(indexLabel.Text)];

				if (compartment.ParentEquipmentGuid == Guid.Empty)
				{
					compartments.RemoveAt(Convert.ToInt32(indexLabel.Text));

					if ((this.CompartmentsDataGrid.Items.Count == 1) && (this.CompartmentsDataGrid.CurrentPageIndex > 0))
					{
						this.CompartmentsDataGrid.CurrentPageIndex--;
					}
				}

				this.CompartmentsDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateCompartmentsView();
			}
		}

		private void CompartmentsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			Label indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				EquipmentCollectionClass compartments = this.Equipment.CompartmentCollection;

				if (this.CompartmentsDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.CompartmentsDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}

				else if (this.CompartmentsDataGrid.EditItemIndex > e.Item.ItemIndex)
					this.CompartmentsDataGrid.EditItemIndex--;

				compartments.RemoveAt(Convert.ToInt32(indexLabel.Text));

				// Resesequence from point for deletion
				for (int iItem = Convert.ToInt32(indexLabel.Text); iItem < compartments.Count; iItem++)
				{
					EquipmentClass compartment = compartments[iItem];
					int compartmentNumber;
					if (int.TryParse(compartment.EquipmentSequence, out compartmentNumber))
						compartment.EquipmentSequence = (compartmentNumber - 1).ToString();
				}

				if (this.CompartmentsDataGrid.Items.Count == 1
					&& this.CompartmentsDataGrid.CurrentPageIndex > 0)
					this.CompartmentsDataGrid.CurrentPageIndex--;
				this.UpdateCompartmentsView();
			}
		}

		private void CompartmentsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.CompartmentsDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.EnableControls(false);
			this.UpdateCompartmentsView();
		}

		private void CompartmentsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					EquipmentCollectionClass compartments = this.Equipment.CompartmentCollection;
					EquipmentClass compartment = compartments[Convert.ToInt32(indexLabel.Text)];

					TextBox capacityTextBox = (TextBox)e.Item.FindControl("CapacityTextBox");
					compartment.Capacity = capacityTextBox.Text;
					TextBox safeFillTextBox = (TextBox)e.Item.FindControl("SafeFillTextBox");
					compartment.SafeFill = safeFillTextBox.Text;
					this.CompartmentsDataGrid.EditItemIndex = -1;

					// vthompson 10/17/2008
					// Make the compartment number editable
					TextBox txtNumber = (TextBox)e.Item.FindControl("txtNumber");
					if (txtNumber.Text.Trim().Length == 0)
					{
						throw new ApplicationException(
							"The value for Number is required.");
					}

					compartment.EquipmentSequence = txtNumber.Text;
					compartment.ParentEquipmentGuid = this.Equipment.IdentityGuid;

					//If the user updates an existing compartment we don't want it to disappear if they press the cancel button. 
					//The cancel button will remove any compartment with an empty IdentityGuid.
					//Compartments that are not yet in the database will have an empty IdentityGuid

					this.EnableControls(true);
					this.UpdateCompartmentsView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the page index change.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void CompartmentsDataGridPageIndxChange(object source, DataGridPageChangedEventArgs e)
		{
			this.CompartmentsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateCompartmentsView();
		}

		/// <summary>
		/// This method will handle the item data bound and disable the edit and delete links if the user does
		/// not have the modify equipment data right.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CompartmentsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			//disable edit and delete button if no security rights
			LinkButton editButton = (LinkButton)e.Item.FindControl("EditButton");
			LinkButton deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if ((editButton != null) && (deleteButton != null))
			{
                bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
                if ((!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)) || !currentSiteOwnsRecordVersion || ((!this.Equipment.IdentityGuid.Equals(Guid.Empty)) && (!this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid))))
				{
					editButton.Enabled = false;
					deleteButton.Enabled = false;
				}
			}
		}
	}
}
