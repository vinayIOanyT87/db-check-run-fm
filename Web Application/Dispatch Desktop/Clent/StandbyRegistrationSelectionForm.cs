namespace Dispatch
{
	using System;
	using System.Windows.Forms;

	using FMBusinessObjects.DataObjects;

	public partial class StandbyRegistrationSelectionForm : FMBaseForm
	{
		private PersonClass person;

		public PersonClass Person
		{
			set
			{
				this.person = value;
				this.OperatorNameTextBox.Text = String.Format("{0},{1}", value.LastName, value.FirstName);
				this.EmployeeIDTextBox.Text = value.ID;
			}
		}

		public EquipmentCollectionClass RegistrationIDList
		{
			set
			{
				this.RegistrationIDComboBox.DataSource = value;
			}
		}

		public EquipmentClass InitialSelection
		{
			get;
			set;
		}
		public EquipmentClass SelectedItem
		{
			get;
			set;
		}

		public StandbyRegistrationSelectionForm()
		{
			this.GetSecurity();
			this.InitializeComponent();
			this.RegistrationIDComboBox.DisplayMember = "XREF";
		}

		private void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				var selectedEquipment = (EquipmentClass) this.RegistrationIDComboBox.SelectedItem;

				if (selectedEquipment == null)
				{
					MessageBox.Show(this, "The Reference Number is empty or is not in the list.  Please select a Reference Number from the list.", "Standby");
					return;
				}

				if (this.person.AssignedEquipmentGuid != Guid.Empty
				   && this.person.AssignedEquipmentGuid != selectedEquipment.MasterRecordGuid)
				{
					string message = String.Format("{0},{1} is currently assigned to {2}.  Do you wish to reassign {0},{1} to vehicle {3}",
					   this.person.LastName, this.person.FirstName, this.person.AssignedEquipmentID, selectedEquipment.ID);

					DialogResult result = MessageBox.Show(this, message, "Standby", MessageBoxButtons.YesNo);

					if (result == DialogResult.No)
					{
						return;
					}

				}

				this.SelectedItem = (EquipmentClass) this.RegistrationIDComboBox.SelectedItem;
				this.Close();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void StandbyRegistrationSelectionFormLoad(object sender, EventArgs e)
		{
			this.RegistrationIDComboBox.SelectedItem = this.InitialSelection;

			this.OKButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
		}
	}
}
