namespace Dispatch
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows.Forms;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class StandbyStatusForm : FMBaseForm
	{
		public string SelectedPerson;
		public string SelectedEquipment;

		public StandbyStatusForm()
		{
			this.InitializeComponent();
			this.GetSecurity();
		}

		private void ClosebuttonClick(object sender, EventArgs e)
		{
			this.Close();
		}

		private void InitializeListViewDisplay()
		{
			this.StandbyStatuslistView.Clear();
			this.StandbyStatuslistView.View = View.Details;
			this.StandbyStatuslistView.Columns.Add("Personnel", 150, HorizontalAlignment.Left);
			this.StandbyStatuslistView.Columns.Add("Equipment", 150, HorizontalAlignment.Left);
		}

		private void StandbyFormLoad(object sender, EventArgs e)
		{
			this.InitializeListViewDisplay();
			this.PopulateListViewDisplay();
			this.Dispatchbutton.Enabled = false;
		}

		private void PopulateListViewDisplay()
		{
			PersonCollectionClass persons =
				FMChannelHelper.MakeCall<IClientDispatchService, PersonCollectionClass>(
					x => x.EnumeratePersonnelByRole(this.Security, PERSON_ROLE.LOADER_ROLE));

			var standbyPersonnel = new PersonCollectionClass();

			foreach (PersonClass person in persons)
			{
				if (person.Status == PersonClass.STATUS.STB)
				{
					standbyPersonnel.Add(person);
				}
			}

			// Sort by UpdatedDate which will order by length of time on standby in descending order.
			List<PersonClass> standbyPersonnelSorted = standbyPersonnel.OrderBy(x => x.UpdatedDate).ToList();

			foreach (PersonClass standbyPerson in standbyPersonnelSorted)
			{
				ListViewItem li = this.StandbyStatuslistView.Items.Add(standbyPerson.FullName);
				li.SubItems.Add(standbyPerson.AssignedEquipmentID);
			}
		}

		private void DispatchbuttonClick(object sender, EventArgs e)
		{
			if (this.StandbyStatuslistView.SelectedItems.Count != 1)
			{
				return;
			}

			this.SelectedPerson = this.StandbyStatuslistView.SelectedItems[0].Text;
			this.SelectedPerson = this.SelectedPerson.Substring(0, this.SelectedPerson.IndexOf(','));
			this.SelectedEquipment = this.StandbyStatuslistView.SelectedItems[0].SubItems[1].Text;
			this.SelectedEquipment = this.SelectedEquipment.Substring(this.SelectedEquipment.Length - 4);

			this.DialogResult = DialogResult.OK;
		}

		private void OnListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			this.Dispatchbutton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
		}
	}
}
