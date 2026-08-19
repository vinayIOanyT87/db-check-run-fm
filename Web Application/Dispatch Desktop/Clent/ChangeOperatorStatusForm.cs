namespace Dispatch
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class ChangeOperatorStatusForm : FMBaseForm
	{
		readonly List<PersonClass> changedPeople = new List<PersonClass>();

		public PersonClass InitialPerson
		{
			get;
			set;
		}

		public ChangeOperatorStatusForm()
		{
			try
			{
				this.GetSecurity();
				this.InitializeComponent();

				this.OperatorGrid.CellFormatting += this.OperatorGridCellFormatting;
				this.OperatorGrid.AutoGenerateColumns = false;

				this.UpdateView();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void OperatorGridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			try
			{
				var person = (PersonClass) this.OperatorGrid.Rows[e.RowIndex].DataBoundItem;

				if (person != null)
				{
					e.CellStyle.SelectionBackColor = Color.Black;

					if (person.LockedOut)
					{
						e.CellStyle.ForeColor = Color.Red;
						e.CellStyle.SelectionForeColor = Color.Red;
					}
					else if (person.Status == PersonClass.STATUS.Out)
					{
						e.CellStyle.ForeColor = Color.Gray;
						e.CellStyle.SelectionForeColor = Color.Gray;
					}
					else
					{
						e.CellStyle.ForeColor = Color.Blue;
						e.CellStyle.SelectionForeColor = Color.Yellow;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IPersonnel>(
				x =>
				{
					foreach (PersonClass Person in changedPeople)
					{
						x.Modify(Security, DATA_TYPE.DYNAMIC, Person);
					}
				});


				this.Close();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CancelButtonClick(object sender, EventArgs e)
		{
			this.Close();
		}

		private void UpdateView()
		{
			var dataAccess = new DispatchDataAccess(this.Security);
			PersonCollectionClass personnel = dataAccess.GetPersonnelNoUpdateConnection();
			var sortedPersonnelList = new SortableBindingList<PersonClass>(personnel);

			this.OperatorGrid.DataSource = sortedPersonnelList;
			this.OperatorGrid.Sort(this.OperatorGrid.Columns[0], ListSortDirection.Ascending);

			if (this.InitialPerson != null)
			{
				foreach (DataGridViewRow row in this.OperatorGrid.Rows)
				{
					var person = (PersonClass) row.DataBoundItem;

					if (person.IdentityGuid == this.InitialPerson.IdentityGuid)
					{
						row.Selected = true;
						break;
					}
				}
			}
		}

		private void InButtonClick(object sender, EventArgs e)
		{
			try
			{
				foreach (DataGridViewRow row in this.OperatorGrid.SelectedRows)
				{
					var person = (PersonClass) row.DataBoundItem;

					if (person != null)
					{
						if (person.LockedOut)
						{
							throw new ApplicationException("Operator locked-out");
						}

						person.Status = PersonClass.STATUS.In;
						person.AssignedEquipmentID = string.Empty;
						person.AssignedEquipmentGuid = Guid.Empty;

						this.AddToUpdateList(person);
						this.OperatorGrid.Refresh();
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddToUpdateList(PersonClass person)
		{
			// Only add the person if they do not exist in the list already
			foreach (PersonClass changedPerson in this.changedPeople)
			{
				if (changedPerson.IdentityGuid == person.IdentityGuid)
				{
					return;
				}
			}

			this.changedPeople.Add(person);
		}

		private void OutButtonClick(object sender, EventArgs e)
		{
			try
			{
				foreach (DataGridViewRow row in this.OperatorGrid.SelectedRows)
				{
					var person = (PersonClass) row.DataBoundItem;

					if (person != null)
					{
						if (person.LockedOut)
						{
							throw new ApplicationException("Operator locked-out");
						}

						person.Status = PersonClass.STATUS.Out;
						person.AssignedEquipmentID = String.Empty;
						person.AssignedEquipmentGuid = Guid.Empty;

						this.AddToUpdateList(person);

						this.OperatorGrid.Refresh();
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void StandByButtonClick(object sender, EventArgs e)
		{
			try
			{
				foreach (DataGridViewRow row in this.OperatorGrid.SelectedRows)
				{
					var person = (PersonClass) row.DataBoundItem;

					if (person != null)
					{
						if (person.LockedOut)
						{
							throw new ApplicationException("Operator locked-out");
						}

						var dataAccess = new DispatchDataAccess(this.Security);
						var standbyForm = new StandbyRegistrationSelectionForm
						                  {
							                  Person = person,
							                  RegistrationIDList = dataAccess.GetEquipmentNoUpdateConnection()
						                  };
						standbyForm.ShowDialog(this);

						if (standbyForm.SelectedItem != null)
						{
							person.AssignedEquipmentGuid = standbyForm.SelectedItem.MasterRecordGuid;
							person.AssignedEquipmentID = standbyForm.SelectedItem.ID;
							person.Status = PersonClass.STATUS.STB;

							this.AddToUpdateList(person);
							this.OperatorGrid.Refresh();
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

		}

		private void ChangeOperatorStatusFormLoad(object sender, EventArgs e)
		{
			bool bEnable = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

			this.OKButton.Enabled = bEnable;
			this.InButton.Enabled = bEnable;
			this.OutButton.Enabled = bEnable;
			this.StandByButton.Enabled = bEnable;
		}
	}
}
