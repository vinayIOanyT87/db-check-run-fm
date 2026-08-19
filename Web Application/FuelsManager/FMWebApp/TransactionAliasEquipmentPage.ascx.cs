// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasEquipmentPage.ascx.cs" company="Varec, Inc.">
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

	/// <summary>
	/// Code behind for TransactionAliasEquipmentPage.
	/// </summary>
	public partial class TransactionAliasEquipmentPage : FMUserControlBase
	{

		#region Methods

		/// <summary>
		/// Handles the SelectedIndexChanged event of the EquipmentDropDownList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void EquipmentDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			////////////////////////////////////////////////////////////////////////
			// Modify this block of code to use EquipmentTypeClass in correct manner.
			////////////////////////////////////////////////////////////////////////
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			byte equipmentNumber = Convert.ToByte(this.EquipmentDropDownList.SelectedItem.Value);
			bool destination = true;
			if (equipmentNumber > 3)
			{
				equipmentNumber -= 3;
				destination = false;
			}
            string targetField = null;
            if (destination)
            {
                if (equipmentNumber == 1)
                    targetField = "DestinationEquipmentTypes1";
                else if (equipmentNumber == 2)
                    targetField = "DestinationEquipmentTypes2";
                else if (equipmentNumber == 3)
                    targetField = "DestinationEquipmentTypes3";                   
            }
            else
            {
                if (equipmentNumber == 1)
                    targetField = "SourceEquipmentTypes1";
                else if (equipmentNumber == 2)
                    targetField = "SourceEquipmentTypes2";
                else if (equipmentNumber == 3)
                    targetField = "SourceEquipmentTypes3";
            }

		    this.SetFieldAccessibilityForChildRecordVersion(targetField);

			this.AssignedTypesListBox.Items.Clear();
			EQUIPMENT_TYPE[] types = transactionAlias.GetEquipmentTypes(destination, equipmentNumber);
			foreach (EQUIPMENT_TYPE type in types)
			{
				var item = new ListItem(EquipmentTypeClass.TypeID(type), ((int)type).ToString(CultureInfo.InvariantCulture));
				this.AssignedTypesListBox.Items.Add(item);
			}

			this.UnassignedTypesListBox.Items.Clear();
			for (var type = EQUIPMENT_TYPE.TRAILER_TYPE; type < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; type++)
			{
				var item = new ListItem(EquipmentTypeClass.TypeID(type), ((int)type).ToString(CultureInfo.InvariantCulture));
				if (this.AssignedTypesListBox.Items.FindByValue(item.Value) == null)
				{
					this.UnassignedTypesListBox.Items.Add(item);
				}
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
				if (!this.Page.IsPostBack)
				{
					// CSI 5856 - disable buttons if user has no modify right.
					if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
					{
						this.AssignButton.Enabled = false;
						this.UnassignButton.Enabled = false;
					}

					// Populate EquipmentDropDownList
					for (int equipmentNumber = 1; equipmentNumber < 4; equipmentNumber++)
					{
						var item = new ListItem("[Destination] [Equipment] " + equipmentNumber.ToString(CultureInfo.InvariantCulture), equipmentNumber.ToString(CultureInfo.InvariantCulture));
						this.EquipmentDropDownList.Items.Add(item);
					}

					for (int equipmentNumber = 1; equipmentNumber < 4; equipmentNumber++)
					{
						var item = new ListItem("[Source] [Equipment] " + equipmentNumber.ToString(CultureInfo.InvariantCulture), (equipmentNumber + 3).ToString(CultureInfo.InvariantCulture));
						this.EquipmentDropDownList.Items.Add(item);
					}

					this.EquipmentDropDownListSelectedIndexChanged(null, null);
                    this.SetFieldAccessibilityForChildRecordVersion("DestinationEquipmentTypes1");
                }
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Command event of the AssignButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		private void AssignButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedTypeItem;
			while ((assignedTypeItem = this.AssignedTypesListBox.SelectedItem) != null)
			{
				this.AssignedTypesListBox.Items.Remove(assignedTypeItem);
				assignedTypeItem.Selected = false;

				foreach (ListItem unassignedTypeItem in this.UnassignedTypesListBox.Items)
				{
					if (unassignedTypeItem.Text.CompareTo(assignedTypeItem.Text) > 0)
					{
						int idx = this.UnassignedTypesListBox.Items.IndexOf(unassignedTypeItem);
						this.UnassignedTypesListBox.Items.Insert(idx, assignedTypeItem);
						assignedTypeItem = null;
						break;
					}
				}

				if (assignedTypeItem != null)
				{
					this.UnassignedTypesListBox.Items.Add(assignedTypeItem);
				}
			}

			this.UpdateEquipmentTypes();
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UnassignButton.Command += this.UnassignButtonCommand;
			this.AssignButton.Command += this.AssignButtonCommand;
		}

		/// <summary>
		/// Handles the Command event of the UnassignButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		private void UnassignButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedTypeItem;
			while ((unassignedTypeItem = this.UnassignedTypesListBox.SelectedItem) != null)
			{
				this.UnassignedTypesListBox.Items.Remove(unassignedTypeItem);
				unassignedTypeItem.Selected = false;

				foreach (ListItem assignedTypeItem in this.AssignedTypesListBox.Items)
				{
					if (assignedTypeItem.Text.CompareTo(unassignedTypeItem.Text) > 0)
					{
						int idx = this.AssignedTypesListBox.Items.IndexOf(assignedTypeItem);
						this.AssignedTypesListBox.Items.Insert(idx, unassignedTypeItem);
						unassignedTypeItem = null;
						break;
					}
				}

				if (unassignedTypeItem != null)
				{
					this.AssignedTypesListBox.Items.Add(unassignedTypeItem);
				}
			}

			this.UpdateEquipmentTypes();
		}

		/// <summary>
		/// Updates the equipment types.
		/// </summary>
		private void UpdateEquipmentTypes()
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			byte equipmentNumber = Convert.ToByte(this.EquipmentDropDownList.SelectedItem.Value);
			bool destination = true;
			if (equipmentNumber > 3)
			{
				equipmentNumber -= 3;
				destination = false;
			}

			var types = new ArrayList();
			foreach (ListItem assignedTypeItem in this.AssignedTypesListBox.Items)
			{
				types.Add((EQUIPMENT_TYPE)Convert.ToInt32(assignedTypeItem.Value));
			}

			transactionAlias.SetEquipmentTypes(
				destination, equipmentNumber, (EQUIPMENT_TYPE[])types.ToArray(typeof(EQUIPMENT_TYPE)));
		}

        private void SetFieldAccessibilityForChildRecordVersion(string targetField)
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
                this.AssignButton.Enabled = (this.AssignButton.Enabled && versionSpecificFields.Contains(targetField));
                this.AssignedTypesListBox.Enabled = (this.AssignedTypesListBox.Enabled && versionSpecificFields.Contains(targetField));
                this.UnassignButton.Enabled = (this.UnassignButton.Enabled && versionSpecificFields.Contains(targetField));
                this.UnassignedTypesListBox.Enabled = (this.UnassignedTypesListBox.Enabled && versionSpecificFields.Contains(targetField));
            }
        }
        #endregion
    }
}