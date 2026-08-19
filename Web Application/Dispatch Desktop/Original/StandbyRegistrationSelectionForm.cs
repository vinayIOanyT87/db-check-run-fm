
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

namespace DispatchPrototype
{
	public partial class StandbyRegistrationSelectionForm : FMBaseForm
	{
		private PersonClass _Person;
		public PersonClass Person
		{
			set
			{
				_Person = value;
				OperatorNameTextBox.Text = String.Format ( "{0},{1}", value.LastName, value.FirstName );
				EmployeeIDTextBox.Text = value.ID;
			}
		}

		public List<EquipmentClass> RegistrationIDList
		{
			set { RegistrationIDComboBox.DataSource = value; }
		}

		public EquipmentClass InitialSelection { get; set; }

		public EquipmentClass SelectedItem { get; set; }

		public StandbyRegistrationSelectionForm ( )
		{
			GetSecurity ( );
			InitializeComponent ( );
			RegistrationIDComboBox.DisplayMember = "XREF";
		}

		private void OKButton_Click ( object sender, EventArgs e )
		{
			try
			{
				EquipmentClass SelectedEquipment = (EquipmentClass) RegistrationIDComboBox.SelectedItem;

				if (SelectedEquipment == null)
				{
					MessageBox.Show ( this, "The Reference Number is empty or is not in the list.  Please select a Reference Number from the list.", "Standby" );
					return;
				}

				if (_Person.AssignedEquipmentGuid.IsNotEmptyAndNotEqualTo(SelectedEquipment.IdentityGuid))
				{
					string message = String.Format ( "{0},{1} is currently assigned to {2}.  Do you wish to reassign {0},{1} to vehicle {3}",
					   _Person.LastName, _Person.FirstName, _Person.AssignedEquipmentID, SelectedEquipment.ID );

					DialogResult result = MessageBox.Show ( this, message, "Standby", MessageBoxButtons.YesNo );

					if (result == DialogResult.No)
					{
						return;
					}

				}

				SelectedItem = (EquipmentClass) RegistrationIDComboBox.SelectedItem;
				Close ( );

			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		private void StandbyRegistrationSelectionForm_Load ( object sender, EventArgs e )
		{
			RegistrationIDComboBox.SelectedItem = InitialSelection;

			OKButton.Enabled = Security.HasRight ( RIGHT.MODIFY_DISPATCH );
		}

	}

}
