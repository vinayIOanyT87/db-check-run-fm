namespace FuelsManager.TrainingWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMWebApp;

	public partial class TrainingAssignments : FMAutoSubmitFormBase
	{
		private const string TrainingSelectedType = "TrainingSelectedType";
		private QUALIFICATION_MAP_TYPE selectedTrainingType = QUALIFICATION_MAP_TYPE.MAX_QUALIFICATION_MAP_TYPE;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.GetSecurity();

			if (!this.IsPostBack)
			{
				this.Session.Remove(TrainingSelectedType);

				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

				DateTimeOffset siteTimeNow = TimeConverter.Now(site);

				this.ExpirationDate.CurrentValue = siteTimeNow;
				this.CompletionDate.CurrentValue = siteTimeNow;
				this.DueDate.CurrentValue = siteTimeNow;

				this.PopulateListBoxes();
				this.PopulateTrainingTypeItems();
				this.PopulateTrainingQualificationItems();
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

		}
		#endregion

		private void PopulateListBoxes()
		{
			// Clear the assigned listbox and populate the available with the configured personnel
			this.lbxAssigned.Items.Clear();
			this.lbxAvailable.Items.Clear();

			const PERSON_ROLE Role = PERSON_ROLE.MAX_PERSON_ROLE;

			PersonCollectionClass personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(x => x.EnumerateByRoleSortByName(this.Security, Role));

			foreach (PersonClass person in personCollection)
			{
				string userCombinedName = person.LastName + "," + person.FirstName + "," + person.MiddleName;
				var li = new ListItem(userCombinedName, person.IdentityGuid.ToString());
				this.lbxAvailable.Items.Add(li);
			}

		}

		private void PopulateTrainingQualificationItems()
		{
			bool bDisplayQuality = false;
			bool bDisplayTraining = false;

			QualificationCollectionClass qualificationCollection = null;
			QualificationCollectionClass trainingCollection = null;

			FMChannelHelper.MakeCall<IQualifications>(
				maps =>
					{
						qualificationCollection = maps.EnumerateByType(this.Security, QUALIFICATION_TYPE.PERSON_QUALIFICATION);
						trainingCollection = maps.EnumerateByType(this.Security, QUALIFICATION_TYPE.PERSON_TRAINING);
					});

			if (this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) ||
				(this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) &&
				this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)))
			{
				if (this.TrainingTypeDropdownlist.SelectedIndex == 1 ||
					this.TrainingTypeDropdownlist.SelectedIndex == 2)
					bDisplayQuality = true;
				if (this.TrainingTypeDropdownlist.SelectedIndex == 0 ||
					this.TrainingTypeDropdownlist.SelectedIndex == 2)
					bDisplayTraining = true;
			}
			else if (this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) &&
					(this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS) == false))
			{
				bDisplayTraining = true;
			}
			else if ((this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) == false) &&
					this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS))
			{
				bDisplayQuality = true;
			}

			this.TrainingDropDownList.Items.Clear();
			// populate the training items drop down list
			if (bDisplayQuality)
			{
				foreach (QualificationClass qualification in qualificationCollection)
				{
					if (this.TrainingDropDownList.Items.FindByText(qualification.ID) == null)
					{
						var li = new ListItem(qualification.ID, qualification.IdentityGuid.ToString());
						this.TrainingDropDownList.Items.Add(li);
					}
				}
			}
			if (bDisplayTraining)
			{
				foreach (QualificationClass qualification in trainingCollection)
				{
					if (this.TrainingDropDownList.Items.FindByText(qualification.ID) == null)
					{
						var li = new ListItem(qualification.ID, qualification.IdentityGuid.ToString());
						this.TrainingDropDownList.Items.Add(li);
					}
				}
			}
			if (this.TrainingDropDownList.Items.Count > 0)
			{
				this.TrainingDropDownList.SelectedIndex = 0;
				this.UpdateSelectedDataType();
			}
		}

		protected void BtnAssignClick(object sender, EventArgs e)
		{
			var selectedItems = new ListItemCollection();
			foreach (ListItem li in this.lbxAvailable.Items)
			{
				if (li.Selected)
				{
					selectedItems.Add(li);
				}
			}

			// If none are selected return
			if (selectedItems.Count == 0)
			{
				return;
			}

			// move the items to the assigned list box
			foreach (ListItem li in selectedItems)
			{
				this.lbxAssigned.Items.Add(li);
				this.lbxAvailable.Items.Remove(li);
			}
			this.lbxAssigned.ClearSelection();
		}

		protected void BtnUnassignClick(object sender, EventArgs e)
		{
			var selectedItems = new ListItemCollection();
			foreach (ListItem li in this.lbxAssigned.Items)
			{
				if (li.Selected)
				{
					selectedItems.Add(li);
				}
			}

			// If none are selected return
			if (selectedItems.Count == 0)
			{
				return;
			}

			// move the items to the assigned list box
			foreach (ListItem li in selectedItems)
			{
				this.lbxAvailable.Items.Add(li);
				this.lbxAssigned.Items.Remove(li);
			}
			this.lbxAvailable.ClearSelection();
		}

		protected void OnButtonApplyClick(object sender, EventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IPersonnel>(
					persons => FMChannelHelper.MakeCall<IQualifications>(
						qualifications => FMChannelHelper.MakeCall<IQualificationMaps>(
							qualificationMaps => this.ApplyButtonProcessing(persons, qualifications, qualificationMaps)
						)
					)
				);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ApplyButtonProcessing(IPersonnel persons, IQualifications qualifications, IQualificationMaps qualificationMaps)
		{
			if (this.Session[TrainingSelectedType] != null)
			{
				var typeExpression = this.Session[TrainingSelectedType] as string;
				if (typeExpression == "PERSON_TRAINING_TO_PERSON")
				{
					this.selectedTrainingType = QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;
				}
				else if (typeExpression == "PERSON_QUALIFICATION_TO_PERSON")
				{
					this.selectedTrainingType = QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON;
				}
				else
				{
					this.selectedTrainingType = QUALIFICATION_MAP_TYPE.MAX_QUALIFICATION_MAP_TYPE;
				}
			}
			if (this.selectedTrainingType == QUALIFICATION_MAP_TYPE.MAX_QUALIFICATION_MAP_TYPE)
			{
				return;
			}

			var selectedPersonnel = new ListItemCollection();
			foreach (ListItem li in this.lbxAssigned.Items)
			{
				selectedPersonnel.Add(li);
			}

			// If none are selected return
			if (selectedPersonnel.Count == 0)
			{
				return;
			}

			// get the select item
			ListItem trainingItem = this.TrainingDropDownList.SelectedItem;

			// load the selected item
			QualificationClass selectedTrainingItem = qualifications.Get(this.Security, Guid.Parse(trainingItem.Value));

			// get the hull number
			string hullNumber = this.HullNoTextBox.Text;

			// get the expiration date
			DateTimeOffset completionDateValue = this.CompletionDate.CurrentValue;
			DateTimeOffset dueDateValue;

			// the due date needs to be calculated per Brian B.
			if (selectedTrainingItem.Reoccurrence <= 0)
			{
				dueDateValue = completionDateValue.AddYears(100);
			}
			else
			{
				dueDateValue = completionDateValue.AddDays(selectedTrainingItem.Reoccurrence);
			}

			DateTimeOffset expirationDateValue = dueDateValue.AddDays(1);

			this.DueDate.CurrentValue = dueDateValue;

			this.ExpirationDate.CurrentValue = expirationDateValue;

			string instructorText = this.InstructorTextbox.Text;

			string ratingText = this.RatingTextbox.Text;

			// go through the personnel and update any existing items or add the new one
			foreach (ListItem li in selectedPersonnel)
			{
				bool bTrainingItemUpdated = false;

				var personGuid = Guid.Parse(li.Value);
				var person = persons.Get(this.Security, personGuid);

				person.QualificationCollection = qualificationMaps.EnumerateByGuidAndType(
					this.Security, person.IdentityGuid, QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON, false);
				person.TrainingCollection = qualificationMaps.EnumerateByGuidAndType(
					this.Security, person.IdentityGuid, QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON, false);

				if (this.selectedTrainingType == QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON)
				{
					foreach (QualificationMapClass qualification in person.QualificationCollection)
					{
						if (qualification.ID == trainingItem.Text)
						{
							// update the existing record
							qualification.ID = trainingItem.Text;
							qualification.Number = hullNumber;
							qualification.Type = this.selectedTrainingType;
							qualification.Sequence = person.QualificationCollection.Count;
							qualification.ExpirationDate.Value = expirationDateValue;
							qualification.AssignedGuid = Guid.Parse(trainingItem.Value);
							qualification.DateCompleted.Value = completionDateValue;
							qualification.DateDue.Value = dueDateValue;
							qualification.Instructor = instructorText;
							qualification.Rating = ratingText;

							persons.Modify(this.Security, DATA_TYPE.CONFIG, person);
							bTrainingItemUpdated = true;
							break;
						}
					}
					if (bTrainingItemUpdated == false)
					{
						var qualificationMap = new QualificationMapClass
						                       {
												   IdentityGuid = Guid.Empty,
							                       ID = trainingItem.Text,
							                       Number = hullNumber,
							                       Type = this.selectedTrainingType,
							                       Sequence = person.QualificationCollection.Count,
							                       ExpirationDate = { Value = expirationDateValue },
							                       AssignedGuid = Guid.Parse(trainingItem.Value),
							                       DateCompleted = { Value = completionDateValue },
							                       DateDue = { Value = dueDateValue },
							                       Instructor = instructorText,
							                       Rating = ratingText
						                       };

						person.QualificationCollection.Add(qualificationMap);
						persons.Modify(this.Security, DATA_TYPE.CONFIG, person);
					}
				}
				else if (this.selectedTrainingType == QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON)
				{
					foreach (QualificationMapClass training in person.TrainingCollection)
					{
						if (training.ID == trainingItem.Text)
						{
							training.ID = trainingItem.Text;
							training.Number = hullNumber;
							training.Type = this.selectedTrainingType;
							training.Sequence = person.QualificationCollection.Count;
							training.ExpirationDate.Value = expirationDateValue;
							training.AssignedGuid = Guid.Parse(trainingItem.Value);
							training.DateCompleted.Value = completionDateValue;
							training.DateDue.Value = dueDateValue;
							training.Instructor = instructorText;
							training.Rating = ratingText;

							persons.Modify(this.Security, DATA_TYPE.CONFIG, person);
							bTrainingItemUpdated = true;
							break;
						}
					}
					if (bTrainingItemUpdated == false)
					{
						var qualificationMap = new QualificationMapClass
						                       {
												   IdentityGuid = Guid.Empty,
							                       ID = trainingItem.Text,
							                       Number = hullNumber,
							                       Type = this.selectedTrainingType,
							                       Sequence = person.QualificationCollection.Count,
							                       ExpirationDate = { Value = expirationDateValue },
							                       AssignedGuid = Guid.Parse(trainingItem.Value),
							                       DateCompleted = { Value = completionDateValue },
							                       DateDue = { Value = dueDateValue },
							                       Instructor = instructorText,
							                       Rating = ratingText
						                       };

						person.QualificationCollection.Add(qualificationMap);
						persons.Modify(this.Security, DATA_TYPE.CONFIG, person);
					}
				}
			}
		}

		protected void OnTrainingItemSelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateSelectedDataType();
		}

		private void UpdateSelectedDataType()
		{
			// determine the type of qualification selected
			QualificationCollectionClass qualificationCollection = null;
			QualificationCollectionClass trainingCollection = null;

			FMChannelHelper.MakeCall<IQualifications>(
				maps =>
					{
						qualificationCollection = maps.EnumerateByType(this.Security, QUALIFICATION_TYPE.PERSON_QUALIFICATION);
						trainingCollection = maps.EnumerateByType(this.Security, QUALIFICATION_TYPE.PERSON_TRAINING);
					});

			this.selectedTrainingType = QUALIFICATION_MAP_TYPE.MAX_QUALIFICATION_MAP_TYPE;
			this.Session.Add(TrainingSelectedType, this.selectedTrainingType.ToString());
			ListItem trainingItem = this.TrainingDropDownList.SelectedItem;

			// populate the training items drop down list
			foreach (QualificationClass qualification in qualificationCollection)
			{
				if (trainingItem.Text == qualification.ID)
				{
					this.selectedTrainingType = QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON;
					this.Session.Add(TrainingSelectedType, this.selectedTrainingType.ToString());

					break;
				}
			}
			foreach (QualificationClass qualification in trainingCollection)
			{
				if (trainingItem.Text == qualification.ID)
				{
					this.selectedTrainingType = QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;
					this.Session.Add(TrainingSelectedType, this.selectedTrainingType.ToString());

					break;
				}
			}
			this.UpdateControls();
		}

		private void UpdateControls()
		{
			if (this.Session[TrainingSelectedType] != null)
			{
				var typeExpression = this.Session[TrainingSelectedType] as string;

				if (typeExpression == "PERSON_TRAINING_TO_PERSON")
				{
					this.selectedTrainingType = QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;
				}
				else if (typeExpression == "PERSON_QUALIFICATION_TO_PERSON")
				{
					this.selectedTrainingType = QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON;
				}
				else
				{
					this.selectedTrainingType = QUALIFICATION_MAP_TYPE.MAX_QUALIFICATION_MAP_TYPE;
				}
			}

			this.CompletionDate.Enabled = true;
			this.DueDate.Enabled = false;
			this.ExpirationDate.Enabled = false;

			if (this.selectedTrainingType == QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON)
			{
				this.InstructorTextbox.Enabled = true;
				this.RatingTextbox.Enabled = true;
			}
			else
			{
				this.InstructorTextbox.Enabled = false;
				this.RatingTextbox.Enabled = false;

				// Instructor and rating only apply to personnel training records. If the type selected is not personnel training,
				// then blank out any instructor and rating values provided
				this.InstructorTextbox.Text = string.Empty;
				this.RatingTextbox.Text = string.Empty;
			}
		}

		private void PopulateTrainingTypeItems()
		{
			this.TrainingTypeDropdownlist.Items.Clear();


			if (this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) ||
				this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			{
				this.TrainingTypeDropdownlist.Items.Add("Training Items");
			}

			if (this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) ||
				this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS))
			{
				this.TrainingTypeDropdownlist.Items.Add("Qualification Items");
			}

			if (this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) ||
				(this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) &&
				this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)))
			{
				this.TrainingTypeDropdownlist.Items.Add("Both");
			}
		}

		protected void OnTrainingTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PopulateTrainingQualificationItems();
		}

		private void UpdateDueAndExpirationDates()
		{
			// get the select item
			ListItem trainingItem = this.TrainingDropDownList.SelectedItem;

			//make sure there is a training item before accessing it to avoid a null object reference error.
			//the trainingItem can be null if you have none configured
			if (trainingItem != null)
			{
				// load the selected item
				QualificationClass selectedTrainingItem =
					FMChannelHelper.MakeCall<IQualifications, QualificationClass>(x => x.Get(this.Security, Guid.Parse(trainingItem.Value)));

				// get the expiration date
				DateTimeOffset completionDateValue = this.CompletionDate.CurrentValue;
				DateTimeOffset dueDateValue;

				// the due date needs to be calculated per Brian B.
				if (selectedTrainingItem.Reoccurrence <= 0)
				{
					dueDateValue = completionDateValue.AddYears(100);
				}
				else
				{
					dueDateValue = completionDateValue.AddDays(selectedTrainingItem.Reoccurrence);
				}

				DateTimeOffset expirationDateValue = dueDateValue.AddDays(1);

				this.DueDate.CurrentValue = dueDateValue;

				this.ExpirationDate.CurrentValue = expirationDateValue;
			}
		}

		protected void OnCalculateDatesClick(object sender, EventArgs e)
		{
			this.UpdateDueAndExpirationDates();
		}


	}
}
