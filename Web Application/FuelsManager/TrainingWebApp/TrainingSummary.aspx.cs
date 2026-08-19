namespace FuelsManager.TrainingWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	public partial class TrainingSummary : FMAutoSubmitFormBase
	{

		#region Private attributes
		private string searchString ;
		private const string PersonnelFindString = "TrainingSummaryPersonnelFindString";
		private const string ItemFindString = "ItemFindString";
		private const string DatefilterFindString = "DateFilterFindString";
		private const string TrainingsummarySelectedPerson = "TrainingSummarySelectedPerson";
		private const string TrainingsummarySortDirection = "TrainingSummarySortDirection";
		private const string TrainingsummarySortExpresion = "TrainingSummarySortExpression";
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (this.Session["TrainingSummaryPage"] != null)
					{
						this.TrainingSummaryDataGrid.PageIndex = (int)this.Session["TrainingSummaryPage"];
						this.Session.Remove("TrainingSummaryPage");
					}

					this.Session.Remove(PersonnelFindString);
					this.Session.Remove(ItemFindString);
					this.Session.Remove(DatefilterFindString);
					this.Session.Remove(TrainingsummarySelectedPerson);
					this.Session.Remove(TrainingsummarySortDirection);
					this.Session.Remove(TrainingsummarySortExpresion);

					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

					DateTimeOffset siteTimeNow = TimeConverter.Now(site);

					this.StartDate.CurrentValue = siteTimeNow;
					this.EndDate.CurrentValue = siteTimeNow;

					// populate the DateFilterTypeDropDown drop down
					this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText("None")));
					this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText("Qualification")));
					this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText("Expiration")));
					this.DateFilterTypeDropDown.Text = "Expiration";
					this.Session.Add(DatefilterFindString, this.DateFilterTypeDropDown.Text);
					this.HistoricalDataCheckBox.Checked = false;

					if (this.Security.HasRight(RIGHT.MODIFY_TRAINING_QUAL_HISTORY) == false &&
						this.Security.HasRight(RIGHT.VIEW_TRAINING_QUAL_HISTORY) == false)
					{
						this.HistoricalDataCheckBox.Enabled = false;
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateView()
		{
			if (this.Session[PersonnelFindString] != null)
			{
				this.FindTextBox.Text = this.Session[PersonnelFindString] as string;
				this.searchString = this.Session[PersonnelFindString] as string;
			}

			if (this.Session[ItemFindString] != null)
			{
				this.ItemDropDownList.Text = this.Session[ItemFindString] as string;
			}

			if (this.Session[DatefilterFindString] != null)
			{
				this.DateFilterTypeDropDown.Text = this.Session[DatefilterFindString] as string;
			}

			var emptyDataView = new DataView();

			this.TrainingSummaryDataGrid.DataSource = emptyDataView;

			this.TrainingSummaryDataGrid.DataBind();

			ICollection persons = this.EnumeratePersons();

			this.TrainingSummaryDataGrid.DataSource = persons;

			this.TrainingSummaryDataGrid.DataBind();
			this.FindTextBox.Text = this.searchString;
			this.FindTextBox.Enabled = true;
			this.ItemDropDownList.Enabled = true;
			this.DateFilterTypeDropDown.Enabled = true;
		}

		private ICollection EnumeratePersons()
		{
			bool bCreateItemList = false;

			PersonCollectionClass personCollection;

			const PERSON_ROLE Role = PERSON_ROLE.MAX_PERSON_ROLE;

			if (this.Session[ItemFindString] == null)
			{
				this.ItemDropDownList.Items.Clear();
				this.ItemDropDownList.Items.Add(new ListItem(this.GetTranslatedText("All")));
				bCreateItemList = true;
			}

			if (string.IsNullOrEmpty(this.searchString))
			{
				personCollection =
					FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(x => x.EnumerateByRole(this.Security, Role));
			}
			else
			{
				personCollection =
					FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
						x => x.EnumerateByRoleAndFilter(this.Security, Role, this.searchString, null));
			}

			var personDataTable = new DataTable();

			personDataTable.Columns.Add("AssigneeGuid", typeof(Guid));
			personDataTable.Columns.Add("LastName", typeof(string));
			personDataTable.Columns.Add("FirstName", typeof(string));
			personDataTable.Columns.Add("Department", typeof(string));
			personDataTable.Columns.Add("Item", typeof(string));
			personDataTable.Columns.Add("Qualification Date", typeof(DateTimeOffset));
			personDataTable.Columns.Add("Expiration Date", typeof(DateTimeOffset));
			personDataTable.Columns.Add("Instructor", typeof(string));
			personDataTable.Columns.Add("Rating", typeof(string));
			personDataTable.Columns.Add("Item ID", typeof(string));
			personDataTable.Columns.Add("AssignedGuid", typeof(Guid));
			personDataTable.Columns.Add("Type", typeof(Int32));
			personDataTable.Columns.Add("UpdatedDate", typeof(string));
			personDataTable.Columns.Add("PrimaryKey", typeof(Guid));

			FMChannelHelper.MakeCall<IQualificationMaps>(
				maps =>
					{
						foreach (PersonClass person in personCollection)
						{
							person.QualificationCollection = maps.EnumerateByGuidAndType(
								this.Security,
								person.IdentityGuid,
								QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON,
								this.HistoricalDataCheckBox.Checked);

							person.TrainingCollection = maps.EnumerateByGuidAndType(
								this.Security, person.IdentityGuid, QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON, this.HistoricalDataCheckBox.Checked);

							// add the qualification items
							int iLoop;
							DataRow personDataRow;
							for (iLoop = 0; iLoop < person.QualificationCollection.Count; iLoop++)
							{
								if (this.DoesQualificationMeetFilterRequirements(person.QualificationCollection[iLoop]) == false) continue;

								personDataRow = personDataTable.NewRow();

								personDataRow["AssigneeGuid"] = person.IdentityGuid;
								personDataRow["LastName"] = person.LastName;
								personDataRow["FirstName"] = person.FirstName;
								personDataRow["Department"] = person.Department;
								personDataRow["Item"] = person.QualificationCollection[iLoop].ID;
								personDataRow["Qualification Date"] = person.QualificationCollection[iLoop].DateCompleted.Value;
								personDataRow["Expiration Date"] = person.QualificationCollection[iLoop].ExpirationDate.Value;
								personDataRow["Instructor"] = person.QualificationCollection[iLoop].Instructor;
								personDataRow["Rating"] = person.QualificationCollection[iLoop].Rating;
								personDataRow["Item ID"] = person.QualificationCollection[iLoop].Number;
								personDataRow["AssignedGuid"] = person.QualificationCollection[iLoop].AssignedGuid;
								personDataRow["Type"] = person.QualificationCollection[iLoop].Type;
								personDataRow["PrimaryKey"] = person.QualificationCollection[iLoop].IdentityGuid;

								// Convert the datetimeoffset to a string using the round trip format specifier (e.g. 2008-04-10T06:30:00.0000000-07:00)
								// Otherwise, the milliseconds will be lost when the time is retrieved when the user presses Delete for historical records,
								// and that will cause the SQL to not delete the record properly.
								personDataRow["UpdatedDate"] = person.QualificationCollection[iLoop].UpdatedDate.ToString("o");

								if (bCreateItemList
								    && this.ItemDropDownList.Items.FindByText(person.QualificationCollection[iLoop].ID) == null) this.ItemDropDownList.Items.Add(new ListItem(person.QualificationCollection[iLoop].ID));

								personDataTable.Rows.Add(personDataRow);
							}

							// add the training items
							for (iLoop = 0; iLoop < person.TrainingCollection.Count; iLoop++)
							{
								if (this.DoesQualificationMeetFilterRequirements(person.TrainingCollection[iLoop]) == false) continue;

								personDataRow = personDataTable.NewRow();

								personDataRow["AssigneeGuid"] = person.IdentityGuid;
								personDataRow["LastName"] = person.LastName;
								personDataRow["FirstName"] = person.FirstName;
								personDataRow["Department"] = person.Department;
								personDataRow["Item"] = person.TrainingCollection[iLoop].ID;
								personDataRow["Qualification Date"] = person.TrainingCollection[iLoop].DateCompleted.Value;
								personDataRow["Expiration Date"] = person.TrainingCollection[iLoop].ExpirationDate.Value;
								personDataRow["Instructor"] = person.TrainingCollection[iLoop].Instructor;
								personDataRow["Rating"] = person.TrainingCollection[iLoop].Rating;
								personDataRow["Item ID"] = person.TrainingCollection[iLoop].Number;
								personDataRow["AssignedGuid"] = person.TrainingCollection[iLoop].AssignedGuid;
								personDataRow["Type"] = person.TrainingCollection[iLoop].Type;
								personDataRow["PrimaryKey"] = person.TrainingCollection[iLoop].IdentityGuid;

								// Convert the datetimeoffset to a string using the round trip format specifier (e.g. 2008-04-10T06:30:00.0000000-07:00)
								// Otherwise, the milliseconds will be lost when the time is retrieved when the user presses Delete for historical records,
								// and that will cause the SQL to not delete the record properly.
								personDataRow["UpdatedDate"] = person.TrainingCollection[iLoop].UpdatedDate.ToString("o");

								if (bCreateItemList && this.ItemDropDownList.Items.FindByText(person.TrainingCollection[iLoop].ID) == null) this.ItemDropDownList.Items.Add(new ListItem(person.TrainingCollection[iLoop].ID));

								personDataTable.Rows.Add(personDataRow);
							}

						}
					});


			var personDataView = new DataView(personDataTable);
			// check if sorting is enabled
			if (this.Session[TrainingsummarySortDirection] != null &&
				this.Session[TrainingsummarySortExpresion] != null)
			{
				var sortExpression = this.Session[TrainingsummarySortExpresion] as string;
				var sortDirection = this.Session[TrainingsummarySortDirection] as string;
				personDataView.Sort = sortExpression + " " + sortDirection;
			}

			return personDataView;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponents();
		}

		protected void InitializeComponents()
		{
			this.TrainingSummaryDataGrid.RowDataBound += this.TrainingSummaryDataGridRowDataBound;
			this.TrainingSummaryDataGrid.RowCommand += this.TrainingSummaryDataGridRowCommandReceived;
			this.TrainingSummaryDataGrid.Sorting += this.TrainingSummaryDataGridSorting;
			this.TrainingSummaryDataGrid.PageIndexChanging += this.GridViewPageIndexChanging;
		}


		protected void TrainingSummaryDataGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			// we do this here because autocreatedcolumns do not exist as an object in the grid
			if (e.Row.RowType == DataControlRowType.DataRow ||
				e.Row.RowType == DataControlRowType.Header ||
				e.Row.RowType == DataControlRowType.Footer)
			{
				e.Row.Cells[2].Visible = false;
				e.Row.Cells[12].Visible = false;
				e.Row.Cells[13].Visible = false;
				e.Row.Cells[14].Visible = false;
				e.Row.Cells[15].Visible = false;

				if (this.HistoricalDataCheckBox.Checked)
				{
					// hide the edit button
					e.Row.Cells[0].Visible = false;

					// if the user does not have rights disable the delete button
					if (this.Security.HasRight(RIGHT.MODIFY_TRAINING_QUAL_HISTORY) == false)
					{
						e.Row.Cells[1].Visible = false;
					}
				}
				else
				{
					// hide the delete button
					e.Row.Cells[1].Visible = false;
				}

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					string qualificationString = string.Format("{0:d}", DateTimeOffset.Parse(e.Row.Cells[7].Text));
					string expirationString = string.Format("{0:d}", DateTimeOffset.Parse(e.Row.Cells[8].Text));

					e.Row.Cells[7].Text = qualificationString;
					e.Row.Cells[8].Text = expirationString;
				} 
			}

			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					var deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
					if (deleteButton != null)
					{
						deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
					}

					var editButton = (FMEditLinkButton)e.Row.FindControl("EditButton");
					if (editButton != null)
					{
						editButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}


		}

		protected void RefreshButtonOnClick(object sender, EventArgs e)
		{
			if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(PersonnelFindString);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
				this.Session.Add(PersonnelFindString, this.searchString);
			}

			if (this.ItemDropDownList == null ||
				this.ItemDropDownList.Text.Length < 1)
			{
				this.Session.Remove(ItemFindString);
			}
			else
			{
				this.Session.Add(ItemFindString, this.ItemDropDownList.Text);
			}

			if (this.DateFilterTypeDropDown == null ||
				this.DateFilterTypeDropDown.Text.Length < 1)
			{
				this.Session.Remove(DatefilterFindString);
			}
			else
			{
				this.Session.Add(DatefilterFindString, this.DateFilterTypeDropDown.Text);
			}

			// Update the page with the new contents.
			this.TrainingSummaryDataGrid.PageIndex = 0;
			this.UpdateView();
		}

		protected void FIndAllBtnOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(PersonnelFindString);
			this.Session.Remove(ItemFindString);
			this.Session.Remove(DatefilterFindString);
			this.Session.Remove(TrainingsummarySelectedPerson);
			this.Session.Remove(TrainingsummarySortDirection);
			this.Session.Remove(TrainingsummarySortExpresion);
			this.searchString = null;
			this.FindTextBox.Text = "";
			this.ItemDropDownList.Text = this.GetTranslatedText("All");
			this.DateFilterTypeDropDown.Text = this.GetTranslatedText("None");
			this.TrainingSummaryDataGrid.PageIndex = 0;
			this.UpdateView();
		}


		private DateTimeOffset GetUnformatedDate(FMDate sender)
		{
			DateTimeOffset date;

			try
			{
				date = sender.CurrentValue;
			}
			catch (FormatException)
			{
				string message = sender.ID + " is not a valid date.";
				this.RenderErrorMessage(message);
				throw new RetrieveException(message);
			}

			return date;

		}


		private bool DoesQualificationMeetFilterRequirements(QualificationMapClass qualification)
		{
			if (this.ItemDropDownList.Text != this.GetTranslatedText("All"))
			{
				if (this.ItemDropDownList.Text != qualification.ID)
					return false;
			}

			if (this.DateFilterTypeDropDown.Text != this.GetTranslatedText("None"))
			{
				DateTimeOffset localStartDate = this.GetUnformatedDate(this.StartDate);
				DateTimeOffset localEndDate = this.GetUnformatedDate(this.EndDate);

				if (this.DateFilterTypeDropDown.Text == this.GetTranslatedText("Qualification"))
				{
					if (qualification.DateCompleted.Value < localStartDate ||
						qualification.DateCompleted.Value > localEndDate)
						return false;
				}
				else if (this.DateFilterTypeDropDown.Text == this.GetTranslatedText("Expiration"))
				{
					if (qualification.ExpirationDate.Value < localStartDate ||
						qualification.ExpirationDate.Value > localEndDate)
						return false;
				}
			}

			return true;
		}

		protected void TrainingSummaryDataGridRowCommandReceived(object sender, CommandEventArgs e)
		{
			try
			{
				if (e.CommandName == "Edit")
				{
					int index = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.TrainingSummaryDataGrid.Rows[index];
					TableCell assigneeGuidCell = row.Cells[2];

					PersonClass person =
						FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, Guid.Parse(assigneeGuidCell.Text)));

					var personArrayList = new ArrayList { person };

					this.Session.Remove(TrainingsummarySelectedPerson);
					this.Session[TrainingsummarySelectedPerson] = personArrayList;

					this.Redirect("PersonForm.aspx");
				}
				else if (e.CommandName == "Delete")
				{
					int index = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.TrainingSummaryDataGrid.Rows[index];
					TableCell typeCell = row.Cells[13];
					TableCell primaryKeyCell = row.Cells[15];

					FMChannelHelper.MakeCall<IQualificationMaps>(
						maps =>
							maps.PurgeByPrimaryKey(
								this.Security,
								Guid.Parse(primaryKeyCell.Text),
								this.GetQualificationMapType(Convert.ToInt32(typeCell.Text))));
					

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

		}

		protected QUALIFICATION_MAP_TYPE GetQualificationMapType(int value)
		{
			switch (value)
			{
				case 0:
					return QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY;
				case 1:
					return QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT;
				case 2:
					return QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT;
				case 3:
					return QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON;
				case 4:
					return QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON;
				case 5:
					return QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;
				case 6:
					return QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE;
				case 7:
					return QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_EQUIPMENT_TYPE;
				case 8:
					return QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_STATION;
				case 9:
					return QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_STATION;
				case 10:
					return QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_STATION;
				case 11:
					return QUALIFICATION_MAP_TYPE.MAX_QUALIFICATION_MAP_TYPE;
			}
			return QUALIFICATION_MAP_TYPE.MAX_QUALIFICATION_MAP_TYPE;
		}

		protected void GridViewPageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			this.TrainingSummaryDataGrid.PageIndex = e.NewPageIndex;
			this.TrainingSummaryDataGrid.DataBind();
		}

		protected void TrainingSummaryDataGridSorting(object sender, GridViewSortEventArgs e)
		{
			string selectSortDirection = this.getSortDirectionString(e.SortDirection);

			if (this.Session[TrainingsummarySortExpresion] != null &&
				this.Session[TrainingsummarySortDirection] != null)
			{
				var lastSortedColumn = this.Session[TrainingsummarySortExpresion] as string;
				if (lastSortedColumn == e.SortExpression)
				{
					var lastSortDirection = this.Session[TrainingsummarySortDirection] as string;
					if (lastSortDirection == selectSortDirection &&
						selectSortDirection == "ASC")
					{
						selectSortDirection = "DESC";
					}
					else
					{
						selectSortDirection = "ASC";
					}
				}
			}

			this.Session.Add(TrainingsummarySortExpresion, e.SortExpression);
			this.Session.Add(TrainingsummarySortDirection, selectSortDirection);

			this.UpdateView();

		}

		private string getSortDirectionString(SortDirection sortDirection)
		{
			string newSortDirection;
			if (sortDirection == SortDirection.Ascending)
			{
				newSortDirection = "ASC";
			}
			else
			{
				newSortDirection = "DESC";
			}

			return newSortDirection;
		}

	}

	public class RetrieveException : Exception
	{
		public RetrieveException(string message)
			: base(message)
		{
		}
	}
}
