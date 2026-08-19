using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace DispatchPrototype
{
	public partial class StandbyStatusForm : FMBaseForm
	{
		public string SelectedPerson = null;
		public string SelectedEquipment = null;

		public StandbyStatusForm ( )
		{
			InitializeComponent ( );
			GetSecurity ( );
		}

		private void Closebutton_Click ( object sender, EventArgs e )
		{
			Close ( );
		}

		private void InitializeListViewDisplay ( )
		{
			StandbyStatuslistView.Clear ( );
			StandbyStatuslistView.View = View.Details;
			StandbyStatuslistView.Columns.Add ( "Personnel", 150, HorizontalAlignment.Left );
			StandbyStatuslistView.Columns.Add ( "Equipment", 150, HorizontalAlignment.Left );
		}

		private void StandbyFormLoad ( object sender, EventArgs e )
		{
			InitializeListViewDisplay ( );
			PopulateListViewDisplay ( );
			Dispatchbutton.Enabled = false;
		}

		private void PopulateListViewDisplay ( )
		{
			FMChannelFactory<IPersonnel> personnelClient = new FMChannelFactory<IPersonnel> ( );
			IPersonnel Personnel = personnelClient.CreateProxy ( );

			PersonCollectionClass Persons = (PersonCollectionClass) Personnel.EnumerateByRole ( Security, PERSON_ROLE.DRIVER_ROLE );
			ListViewItem li;

			foreach (PersonClass Person in Persons)
			{
				if (Person.Status == PersonClass.STATUS.STB)
				{
					li = StandbyStatuslistView.Items.Add ( Person.FullName );
					li.SubItems.Add ( Person.AssignedEquipmentID );
				}
			}
		}

		private void Dispatchbutton_Click ( object sender, EventArgs e )
		{
			try
			{
				if (StandbyStatuslistView.SelectedItems.Count != 1)
				{
					return;
				}

				SelectedPerson = StandbyStatuslistView.SelectedItems[0].Text;
				SelectedPerson = SelectedPerson.Substring( 0, SelectedPerson.IndexOf( ',' ) );
				SelectedEquipment = StandbyStatuslistView.SelectedItems[0].SubItems[1].Text;

				if (SelectedEquipment.Length > 4)
				{
					SelectedEquipment = SelectedEquipment.Substring( SelectedEquipment.Length - 4 );
				}

				DialogResult = DialogResult.OK;
			}
			catch (Exception except)
			{
				ErrorHandler( except );
			}
		}

		private void OnListViewSelectedIndexChanged ( object sender, EventArgs e )
		{
			Dispatchbutton.Enabled = Security.HasRight ( RIGHT.MODIFY_DISPATCH );
		}

	}
}
