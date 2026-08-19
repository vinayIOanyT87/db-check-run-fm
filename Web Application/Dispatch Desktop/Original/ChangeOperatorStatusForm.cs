
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

namespace DispatchPrototype
{
   public partial class ChangeOperatorStatusForm : FMBaseForm
   {
	  List<PersonClass> ChangedPeople = new List<PersonClass>();

	  public PersonClass InitialPerson { get; set; }

	  public ChangeOperatorStatusForm ()
	  {
		 try
		 {
			GetSecurity();
			InitializeComponent();
			
			OperatorGrid.CellFormatting += new DataGridViewCellFormattingEventHandler( OperatorGrid_CellFormatting );
			OperatorGrid.AutoGenerateColumns = false;

			UpdateView();

		 }
		 catch (Exception except)
		 {
			ErrorHandler( except );
		 }
	  }

	  void OperatorGrid_CellFormatting ( object sender, DataGridViewCellFormattingEventArgs e )
	  {
		 try
		 {
			PersonClass Person = (PersonClass) OperatorGrid.Rows[e.RowIndex].DataBoundItem;
			if (Person != null)
			{
			   e.CellStyle.SelectionBackColor = Color.Black;

			   if (Person.LockedOut)
			   {
				  e.CellStyle.ForeColor = Color.Red;
				  e.CellStyle.SelectionForeColor = Color.Red;
			   }
			   else if (Person.Status == PersonClass.STATUS.Out)
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
			ErrorHandler( except );
		 }
	  }

	  private void OKButton_Click ( object sender, EventArgs e )
	  {
		  try
		  {
			  FMChannelFactory<IPersonnel> personnelClient = new FMChannelFactory<IPersonnel> ( );
			  IPersonnel personnel = personnelClient.CreateProxy ( );

			  // Loop through all changed items and save them
			  foreach (PersonClass Person in ChangedPeople)
			  {
				  personnel.Modify( Security, DATA_TYPE.DYNAMIC, Person );
			  }

			  Close();

		  }
		  catch (Exception except)
		  {
			  ErrorHandler( except );
		  }
	  }

	  private void CancelButton_Click ( object sender, EventArgs e )
	  {
		 Close();
	  }

	  private void UpdateView ()
	  {
		 DispatchDataAccess DataAccess = new DispatchDataAccess(Security);
		 PersonCollectionClass Personnel = DataAccess.GetPersonnelNoUpdateConnection( Security );
		 OperatorGrid.DataSource = new SortableBindingList<PersonClass>(Personnel);
		 OperatorGrid.Sort( OperatorGrid.Columns[0], ListSortDirection.Ascending );

		 if (InitialPerson != null)
		 {
			foreach (DataGridViewRow Row in OperatorGrid.Rows)
			{
			   PersonClass person = (PersonClass)Row.DataBoundItem;
			   if (person.IdentityGuid == InitialPerson.IdentityGuid)
			   {
				  Row.Selected = true;
				  break;
			   }

			}

		 }

	  }

	  private void InButton_Click ( object sender, EventArgs e )
	  {
		 try
		 {
			foreach ( DataGridViewRow Row in OperatorGrid.SelectedRows )
			{
			   PersonClass Person = (PersonClass)Row.DataBoundItem;
			   if (Person != null)
			   {
				  if (Person.LockedOut)
				  {
					 throw new ApplicationException( "Operator locked-out" );
				  }

				  Person.Status = PersonClass.STATUS.In;
				  Person.AssignedEquipmentID = string.Empty;
				  Person.AssignedEquipmentGuid.Reset();

				  AddToUpdateList( Person );

				  OperatorGrid.Refresh();

			   }
			}
		 }
		 catch (Exception except)
		 {
			ErrorHandler( except );
		 }

	  }

	  private void AddToUpdateList ( PersonClass Person )
	  {
		 // Only add the person if they do not exist in the list already
		 foreach (PersonClass ChangedPerson in ChangedPeople)
		 {
			if (ChangedPerson.IdentityGuid == Person.IdentityGuid)
			{
			   return;
			}
		 }

		 ChangedPeople.Add( Person );

	  }

	  private void OutButton_Click ( object sender, EventArgs e )
	  {
		 try
		 {
			foreach ( DataGridViewRow Row in OperatorGrid.SelectedRows )
			{
			   PersonClass Person = (PersonClass)Row.DataBoundItem;
			   if (Person != null)
			   {
				  if (Person.LockedOut)
				  {
					 throw new ApplicationException( "Operator locked-out" );
				  }

				  Person.Status = PersonClass.STATUS.Out;
				  Person.AssignedEquipmentID = String.Empty;
				  Person.AssignedEquipmentGuid.Reset();

				  AddToUpdateList( Person );

				  OperatorGrid.Refresh();
			   }
			}
		 }
		 catch (Exception except)
		 {
			ErrorHandler( except );
		 }

	  }

	  private void StandByButton_Click ( object sender, EventArgs e )
	  {
		 try
		 {
			foreach ( DataGridViewRow Row in OperatorGrid.SelectedRows )
			{
			   PersonClass Person = (PersonClass)Row.DataBoundItem;
			   if (Person != null)
			   {
				  if (Person.LockedOut)
				  {
					 throw new ApplicationException( "Operator locked-out" );
				  }

				  DispatchDataAccess DataAccess = new DispatchDataAccess(Security);
				  StandbyRegistrationSelectionForm standbyForm = new StandbyRegistrationSelectionForm();
				  standbyForm.Person = Person;
				  standbyForm.RegistrationIDList = DataAccess.GetEquipmentNoUpdateConnection( Security );
				  standbyForm.ShowDialog( this );

				  if (standbyForm.SelectedItem != null)
				  {
					 Person.AssignedEquipmentGuid = standbyForm.SelectedItem.IdentityGuid;
					 Person.AssignedEquipmentID = standbyForm.SelectedItem.ID;
					 Person.Status = PersonClass.STATUS.STB;

					 AddToUpdateList( Person );

					 OperatorGrid.Refresh();
				  }
			   }
			}
		 }
		 catch (Exception except)
		 {
			ErrorHandler( except );
		 }

	  }

	  private void ChangeOperatorStatusForm_Load ( object sender, EventArgs e )
	  {
		 bool bEnable = Security.HasRight( RIGHT.MODIFY_DISPATCH );

		 OKButton.Enabled = bEnable;
		 InButton.Enabled = bEnable;
		 OutButton.Enabled = bEnable;
		 StandByButton.Enabled = bEnable;
	  }

   }
}
