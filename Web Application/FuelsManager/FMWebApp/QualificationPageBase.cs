namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;


	public abstract class QualificationPageBase : FMUserControlBase
	{
		#region Constants and Fields

		protected int PriorEditItemIndex = -2;

		#endregion

		#region Properties

		protected virtual DataGrid MapGrid
		{
			get
			{
				return null;
			}
		}

		protected abstract QualificationMapCollectionClass PageMaps { get; set; }

		protected abstract QUALIFICATION_MAP_TYPE PageQualificationMapType { get; }

		protected abstract QUALIFICATION_TYPE PageQualificationType { get; }

		private int CurrentIndex
		{
			get
			{
				var qualificationDataView = (DataView)this.MapGrid.DataSource;
				int index = this.MapGrid.PageSize * this.MapGrid.CurrentPageIndex + this.MapGrid.EditItemIndex;
				DataRowView item = qualificationDataView[index];
				return (int)item[0];
			}
		}

		private QualificationMapClass CurrentMap
		{
			get
			{
				QualificationMapCollectionClass maps = this.PageMaps;
				return maps[this.CurrentIndex];
			}
		}

		#endregion

		// training items

		// Training Items

		#region Methods

		protected void AddButtonTrainingCommand(object sender, CommandEventArgs e)
		{
			QualificationMapCollectionClass qualifications = this.PageMaps;
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
																);


			var qualificationMap = new QualificationMapClass(site)
			                       {
				                       Type = this.PageQualificationMapType,
				                       Sequence = qualifications.Count,
				                       ExpirationDate = { Value = TimeConverter.Today(site) }
			                       };

			qualifications.Add(qualificationMap);
			this.MapGrid.CurrentPageIndex = (qualifications.Count - 1) / this.MapGrid.PageSize;
			this.MapGrid.EditItemIndex = (qualifications.Count - 1) % this.MapGrid.PageSize;
			this.EnableControls(false);

			try
			{
				this.UpdateTrainingView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				qualifications.RemoveAt(qualifications.Count - 1);

				if (this.MapGrid.CurrentPageIndex > 0 && this.MapGrid.EditItemIndex.Equals(0))
				{
					this.MapGrid.CurrentPageIndex--;
				}

				this.MapGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateTrainingView();
			}
		}

		protected void AddButtonCommand(object sender, CommandEventArgs e)
		{
			QualificationMapCollectionClass qualifications = this.PageMaps;
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
																);

			var qualificationMap = new QualificationMapClass(site)
			                       {
				                       Type = this.PageQualificationMapType,
				                       Sequence = qualifications.Count,
				                       ExpirationDate = { Value = TimeConverter.Today(site) }
			                       };

			qualifications.Add(qualificationMap);
			this.MapGrid.CurrentPageIndex = (qualifications.Count - 1) / this.MapGrid.PageSize;
			this.MapGrid.EditItemIndex = (qualifications.Count - 1) % this.MapGrid.PageSize;
			this.EnableControls(false);

			try
			{
				this.UpdateQualificationsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				qualifications.RemoveAt(qualifications.Count - 1);

				if (this.MapGrid.CurrentPageIndex > 0 && this.MapGrid.EditItemIndex == 0)
				{
					this.MapGrid.CurrentPageIndex--;
				}

				this.MapGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateQualificationsView();
			}
		}

		protected virtual void EnableControls(bool enable)
		{
		}

		protected ListItemCollection EnumerateQualifications()
		{
		    QualificationMapCollectionClass maps = this.PageMaps;
		    QualificationMapClass currentMap = maps[this.CurrentIndex];

			QualificationCollectionClass qualificationCollection = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
					x =>
					x.EnumerateByType(this.Security, this.PageQualificationType)
			);

			var qualificationItems = new ListItemCollection();

			foreach (QualificationClass qualification in qualificationCollection)
			{
			    bool skipDuplicateQualification = false;
                if (currentMap.AssignedGuid == Guid.Empty)
                {
                    // Preclude duplicates
                    foreach (QualificationMapClass map in maps)
                    {
                        if (qualification.IdentityGuid == map.AssignedGuid)
                        {
                            skipDuplicateQualification = true;
                            break;
                        }
                    }

                    if (skipDuplicateQualification)
                    {
                        continue;
                    }
                }

			    var newQualificationItem = new ListItem(qualification.ID, qualification.IdentityGuid.ToString());
				foreach (ListItem existingQualificationItem in qualificationItems)
				{
					if (String.Compare(existingQualificationItem.Text, newQualificationItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = qualificationItems.IndexOf(existingQualificationItem);
						qualificationItems.Insert(index, newQualificationItem);
						newQualificationItem = null;
						break;
					}
				}

				if (newQualificationItem != null)
				{
					qualificationItems.Add(newQualificationItem);
				}
			}

			if (qualificationItems.Count == 0)
			{
				string errMsg = "No new " + QualificationClass.TypeID(this.PageQualificationType) +
						(QualificationClass.TypeID(this.PageQualificationType).Contains("Training") ? " is" : " are") + " available.";

				if (errMsg.Contains("License"))
				{
					errMsg = errMsg.Replace("License", "Licenses");
				}

				if (errMsg.Contains("Tag"))
				{
					errMsg = errMsg.Replace("Tag", "Tags");
				}

				if (errMsg.Contains("Qualification"))
				{
					errMsg = errMsg.Replace("Qualification", "Qualifications");
				}

				if (errMsg.Contains("Test"))
				{
					errMsg = errMsg.Replace("Test", "Tests");
				}

				if (errMsg.Contains("Inspection"))
				{
					errMsg = errMsg.Replace("Inspection", "Inspections");
				}

				throw new Exception(errMsg);
			}

			return qualificationItems;
		}

		protected ListItemCollection EnumerateTrainings()
		{
			QualificationMapCollectionClass maps = this.PageMaps;
			QualificationMapClass currentMap = maps[this.CurrentIndex];

			QualificationCollectionClass qualificationCollection = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
					x =>
					x.EnumerateByType(this.Security, this.PageQualificationType)
			);

			var qualificationItems = new ListItemCollection();

			foreach (QualificationClass t in qualificationCollection)
			{
				QualificationClass qualification = t;

				var newQualificationItem = new ListItem(qualification.ID, qualification.IdentityGuid.ToString());
				foreach (ListItem existingQualificationItem in qualificationItems)
				{
					if (String.Compare(existingQualificationItem.Text, newQualificationItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = qualificationItems.IndexOf(existingQualificationItem);
						qualificationItems.Insert(index, newQualificationItem);
						newQualificationItem = null;
						break;
					}
				}

				if (newQualificationItem != null)
				{
					qualificationItems.Add(newQualificationItem);
				}
			}

			if (qualificationItems.Count == 0)
			{
				string errMsg = "No new " + QualificationClass.TypeID(this.PageQualificationType) + " available.";

				if (errMsg.Contains("License"))
				{
					errMsg = errMsg.Replace("License", "Licenses");
				}

				if (errMsg.Contains("Qualification"))
				{
					errMsg = errMsg.Replace("Qualification", "Qualifications");
				}

				if (errMsg.Contains("Test"))
				{
					errMsg = errMsg.Replace("Test", "Tests");
				}

				if (errMsg.Contains("Inspection"))
				{
					errMsg = errMsg.Replace("Inspection", "Inspections");
				}

				throw new Exception(errMsg);
			}

			return qualificationItems;
		}

		protected void QualificationsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				QualificationMapCollectionClass maps = this.PageMaps;
				QualificationMapClass map = maps[Convert.ToInt32(indexLabel.Text)];

				// If the user has not clicked the green check yet, delete the row.
				if (map.AssignedGuid.IsEmpty())
				{
					maps.RemoveAt(Convert.ToInt32(indexLabel.Text));

					if ((this.MapGrid.Items.Count == 1) && (this.MapGrid.CurrentPageIndex > 0))
					{
						this.MapGrid.CurrentPageIndex--;
					}
				}

				this.PriorEditItemIndex = this.MapGrid.EditItemIndex;
				this.MapGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateQualificationsView();
			}
		}

		protected void QualificationsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				QualificationMapCollectionClass maps = this.PageMaps;

				if (this.MapGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.MapGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.MapGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.MapGrid.EditItemIndex--;
				}

				maps.RemoveAt(Convert.ToInt32(indexLabel.Text));

				// Resesequence from point for deletion
				for (int iItem = Convert.ToInt32(indexLabel.Text); iItem < maps.Count; iItem++)
				{
					QualificationMapClass qualification = maps[iItem];
					qualification.Sequence--;
				}

				if (this.MapGrid.CurrentPageIndex > 0 && this.MapGrid.Items.Count == 1)
				{
					this.MapGrid.CurrentPageIndex--;
				}

				this.UpdateQualificationsView();
			}
		}

		protected void QualificationsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.MapGrid.EditItemIndex = e.Item.ItemIndex;
				this.EnableControls(false);
				this.UpdateQualificationsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.MapGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateQualificationsView();
			}
		}

		protected virtual void QualificationsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var qualificationsDropDownList = (DropDownList)e.Item.FindControl("QualificationsDropDownList");
			if (qualificationsDropDownList != null)
			{
				QualificationMapClass map = this.CurrentMap;

				if (!map.AssignedGuid.IsEmpty())
				{
					ListItemCollection items = qualificationsDropDownList.Items;
					int index = items.IndexOf(items.FindByValue(map.AssignedGuid.ToString()));
					qualificationsDropDownList.SelectedIndex = index;
				}
			}

			if ((this.MapGrid != null && this.MapGrid.EditItemIndex == e.Item.ItemIndex)
				 || this.PriorEditItemIndex == e.Item.ItemIndex)
			{
				// Now set the focus to the edit control
				Control ctrl;

				if (this.MapGrid != null && this.MapGrid.EditItemIndex == e.Item.ItemIndex)
				{
					ctrl = e.Item.FindControl("QualificationsDropDownList");
				}
				else
				{
					ctrl = e.Item.FindControl("EditButton");
				}

				if (ctrl != null)
				{
					const string Script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
					this.Page.ClientScript.RegisterStartupScript(
						this.GetType(), "page_set_focus", string.Format(Script, ctrl.ClientID));
				}
			}

			// disable edit and delete button if no security rights
			var editButton = (LinkButton)e.Item.FindControl("EditButton");
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if (editButton != null && deleteButton != null)
			{
				if ((this.PageQualificationType == QUALIFICATION_TYPE.PERSON_QUALIFICATION
					  && !this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
					  && !this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
					 || (this.PageQualificationType == QUALIFICATION_TYPE.PERSON_TRAINING
						  && !this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING)
						  && !this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
					 || (this.PageQualificationType == QUALIFICATION_TYPE.PERSON_LICENSE
						  && !this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
					 || (this.PageQualificationType == QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT
						  && !this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
					 || ((this.PageQualificationType == QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE
							|| this.PageQualificationType == QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION)
						  && !this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)))
				{
					editButton.Enabled = false;
					deleteButton.Enabled = false;
				}
			}
		}

		protected void QualificationsDataGridNoDueDateEditUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					int reOccuranceValue = 0;
					QualificationMapCollectionClass maps = this.PageMaps;
					QualificationMapClass map = maps[Convert.ToInt32(indexLabel.Text)];

					var qualificationsDropDownList = (DropDownList)e.Item.FindControl("QualificationsDropDownList");
					map.AssignedGuid = new Guid(qualificationsDropDownList.SelectedValue);
					map.ID = qualificationsDropDownList.SelectedItem.Text;

					var numberTextBox = (TextBox)e.Item.FindControl("NumberTextBox");
					if (numberTextBox != null)
					{
						map.Number = numberTextBox.Text;
					}

					var dateCompleted = (FMDate)e.Item.FindControl("DateCompleted");
					if (dateCompleted != null)
					{
						map.DateCompleted.Value = DateTimeOffset.Parse(dateCompleted.Text, map.ExpirationDate.Format);
					}

					// get the re-occurance for the selected training
					QualificationCollectionClass qualificationCollection = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
							x =>
							x.EnumerateByType(this.Security, this.PageQualificationType)
					);

					foreach (QualificationClass qualification in qualificationCollection)
					{
						if (qualification.ID == map.ID)
						{
							reOccuranceValue = qualification.Reoccurrence;
							break;
						}
					}

					// the date due field is now calculated based on the complete date and the reoccurance
					if (reOccuranceValue <= 0)
					{
						map.DateDue.Value = map.DateCompleted.Value.AddYears(100);
					}
					else
					{
						// reoccurance is in days so just add them to the completed date
						map.DateDue.Value = map.DateCompleted.Value.AddDays(reOccuranceValue);
					}

					map.ExpirationDate.Value = map.DateDue.Value.AddDays(1);

					this.PriorEditItemIndex = this.MapGrid.EditItemIndex;
					this.MapGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateQualificationsView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.UpdateQualificationsView();
			}
		}

		protected void QualificationsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.MapGrid.EditItemIndex > -1)
			{
				return;
			}

			this.MapGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateQualificationsView();
		}

		protected void QualificationsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					QualificationMapCollectionClass maps = this.PageMaps;
					QualificationMapClass map = maps[Convert.ToInt32(indexLabel.Text)];

					var qualificationsDropDownList = (DropDownList)e.Item.FindControl("QualificationsDropDownList");
					map.AssignedGuid = new Guid(qualificationsDropDownList.SelectedValue);
					map.ID = qualificationsDropDownList.SelectedItem.Text;

					var numberTextBox = (TextBox)e.Item.FindControl("NumberTextBox");
					if (numberTextBox != null)
					{
						map.Number = numberTextBox.Text;
					}

					var expirationDate = (FMDate)e.Item.FindControl("ExpirationDate");
					if (expirationDate != null)
					{
						map.ExpirationDate.Value = DateTimeOffset.Parse(expirationDate.Text, map.ExpirationDate.Format);
					}

					var dateCompleted = (FMDate)e.Item.FindControl("DateCompleted");
					if (dateCompleted != null)
					{
						map.DateCompleted.Value = DateTimeOffset.Parse(dateCompleted.Text, map.ExpirationDate.Format);
					}

					var dateDue = (FMDate)e.Item.FindControl("DateDue");
					if (dateDue != null)
					{
						map.DateDue.Value = DateTimeOffset.Parse(dateDue.Text, map.ExpirationDate.Format);
					}

					this.PriorEditItemIndex = this.MapGrid.EditItemIndex;
					this.MapGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateQualificationsView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.UpdateQualificationsView();
			}
		}

		protected void TrainingDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				QualificationMapCollectionClass maps = this.PageMaps;
				QualificationMapClass map = maps[Convert.ToInt32(indexLabel.Text)];

				// If the user has not clicked the green check yet, delete the row.
				if (map.AssignedGuid.IsEmpty())
				{
					maps.RemoveAt(Convert.ToInt32(indexLabel.Text));

					if ((this.MapGrid.Items.Count == 1) && (this.MapGrid.CurrentPageIndex > 0))
					{
						this.MapGrid.CurrentPageIndex--;
					}
				}

				this.PriorEditItemIndex = this.MapGrid.EditItemIndex;
				this.MapGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateTrainingView();
			}
		}

		protected void TrainingDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				QualificationMapCollectionClass maps = this.PageMaps;

				if (this.MapGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.MapGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.MapGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.MapGrid.EditItemIndex--;
				}

				maps.RemoveAt(Convert.ToInt32(indexLabel.Text));

				// Resesequence from point for deletion
				for (int iItem = Convert.ToInt32(indexLabel.Text); iItem < maps.Count; iItem++)
				{
					QualificationMapClass qualification = maps[iItem];
					qualification.Sequence--;
				}

				if (this.MapGrid.CurrentPageIndex > 0 && this.MapGrid.Items.Count == 1)
				{
					this.MapGrid.CurrentPageIndex--;
				}

				this.UpdateTrainingView();
			}
		}

		protected void TrainingDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.MapGrid.EditItemIndex = e.Item.ItemIndex;
				this.EnableControls(false);
				this.UpdateTrainingView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.MapGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateTrainingView();
			}
		}

		protected void TrainingDataGridNoDueDateEditUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					int reOccuranceValue = 0;
					QualificationMapCollectionClass maps = this.PageMaps;
					QualificationMapClass map = maps[Convert.ToInt32(indexLabel.Text)];

					var qualificationsDropDownList = (DropDownList)e.Item.FindControl("QualificationsDropDownList");
					map.AssignedGuid = new Guid(qualificationsDropDownList.SelectedValue);
					map.ID = qualificationsDropDownList.SelectedItem.Text;

					var numberTextBox = (TextBox)e.Item.FindControl("NumberTextBox");
					if (numberTextBox != null)
					{
						map.Number = numberTextBox.Text;
					}

					var instructorTextbox = (TextBox)e.Item.FindControl("InstructorTextbox");
					if (instructorTextbox != null)
					{
						map.Instructor = instructorTextbox.Text;
					}

					var dateCompleted = (FMDate)e.Item.FindControl("DateCompleted");
					if (dateCompleted != null)
					{
						map.DateCompleted.Value = DateTimeOffset.Parse(dateCompleted.Text, map.ExpirationDate.Format);
					}

					// get the re-occurance for the selected training
					QualificationCollectionClass qualificationCollection = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, this.PageQualificationType)
																);

					foreach (QualificationClass qualification in qualificationCollection)
					{
						if (qualification.ID == map.ID)
						{
							reOccuranceValue = qualification.Reoccurrence;
							break;
						}
					}

					// the date due field is now calculated based on the complete date and the reoccurance
					if (reOccuranceValue <= 0)
					{
						// even though the requirement is reoccurance = 0 then due date equals completion date the testers want it changed.
						// so if occurance equals zoro we will add 100 years onto the due date
						map.DateDue.Value = map.DateCompleted.Value.AddYears(100);
					}
					else
					{
						// reoccurance is in days so just add them to the completed date
						map.DateDue.Value = map.DateCompleted.Value.AddDays(reOccuranceValue);
					}

					map.ExpirationDate.Value = map.DateDue.Value.AddDays(1);
					var ratingTextbox = (TextBox)e.Item.FindControl("RatingTextbox");
					if (ratingTextbox != null)
					{
						map.Rating = ratingTextbox.Text;
					}

					this.PriorEditItemIndex = this.MapGrid.EditItemIndex;
					this.MapGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateTrainingView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.UpdateTrainingView();
			}
		}

		protected void TrainingDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.MapGrid.EditItemIndex > -1)
			{
				return;
			}

			this.MapGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateTrainingView();
		}

		protected void TrainingDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					QualificationMapCollectionClass maps = this.PageMaps;
					QualificationMapClass map = maps[Convert.ToInt32(indexLabel.Text)];

					var qualificationsDropDownList = (DropDownList)e.Item.FindControl("QualificationsDropDownList");
					map.AssignedGuid = new Guid(qualificationsDropDownList.SelectedValue);
					map.ID = qualificationsDropDownList.SelectedItem.Text;

					var numberTextBox = (TextBox)e.Item.FindControl("NumberTextBox");
					if (numberTextBox != null)
					{
						map.Number = numberTextBox.Text;
					}

					var expirationDate = (FMDate)e.Item.FindControl("ExpirationDate");
					if (expirationDate != null)
					{
						map.ExpirationDate.Value = DateTimeOffset.Parse(expirationDate.Text, map.ExpirationDate.Format);
					}

					var instructorTextbox = (TextBox)e.Item.FindControl("InstructorTextbox");
					if (instructorTextbox != null)
					{
						map.Instructor = instructorTextbox.Text;
					}

					var dateCompleted = (FMDate)e.Item.FindControl("DateCompleted");
					if (dateCompleted != null)
					{
						map.DateCompleted.Value = DateTimeOffset.Parse(dateCompleted.Text, map.ExpirationDate.Format);
					}

					var dateDue = (FMDate)e.Item.FindControl("DateDue");
					if (dateDue != null)
					{
						map.DateDue.Value = DateTimeOffset.Parse(dateDue.Text, map.ExpirationDate.Format);
					}

					var ratingTextbox = (TextBox)e.Item.FindControl("RatingTextbox");
					if (ratingTextbox != null)
					{
						map.Rating = ratingTextbox.Text;
					}

					this.PriorEditItemIndex = this.MapGrid.EditItemIndex;
					this.MapGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateTrainingView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.UpdateTrainingView();
			}
		}

		protected void UpdateQualificationsView()
		{
			this.MapGrid.DataSource = this.EnumerateQualificationsMap();
			this.MapGrid.DataBind();
		}

		protected void UpdateTrainingView()
		{
			this.MapGrid.DataSource = this.EnumerateTrainingMap();
			this.MapGrid.DataBind();
		}

		private ICollection EnumerateQualificationsMap()
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetBasic(this.Security, this.Security.SiteGuid)
																);

			DateTimeFormatInfo formatInfo = site.GetDateTimeFormatInfo();

			QualificationMapCollectionClass qualifications = this.PageMaps;

			var mapDataTable = new DataTable();

			mapDataTable.Columns.Add("Index", typeof(Int32));
			mapDataTable.Columns.Add("QualificationID", typeof(string));
			mapDataTable.Columns.Add("ID", typeof(string));
			mapDataTable.Columns.Add("DateCompleted", typeof(string));
			mapDataTable.Columns.Add("DateDue", typeof(string));
			mapDataTable.Columns.Add("ExpirationDate", typeof(string));

			if (qualifications != null)
			{
				for (int iItem = 0; iItem < qualifications.Count; iItem++)
				{
					DataRow mapDataRow = mapDataTable.NewRow();

					QualificationMapClass qualification = qualifications[iItem];
					qualification.DateDue.Format = formatInfo;
					qualification.ExpirationDate.Format = formatInfo;
					qualification.DateCompleted.Format = formatInfo;
					mapDataRow["Index"] = iItem;
					mapDataRow["QualificationID"] = qualification.ID;
					mapDataRow["ID"] = qualification.Number;
					mapDataRow["DateCompleted"] = qualification.DateCompleted.ToString();
					mapDataRow["DateDue"] = qualification.DateDue.ToString();
					mapDataRow["ExpirationDate"] = qualification.ExpirationDate.ToString();

					mapDataTable.Rows.Add(mapDataRow);
				}
			}

			var qualificationDataView = new DataView(mapDataTable);
			return qualificationDataView;
		}

		private ICollection EnumerateTrainingMap()
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetBasic(this.Security, this.Security.SiteGuid)
																);

			DateTimeFormatInfo dateFormat = site.GetDateTimeFormatInfo();

			QualificationMapCollectionClass qualifications = this.PageMaps;

			var mapDataTable = new DataTable();

			mapDataTable.Columns.Add("Index", typeof(Int32));
			mapDataTable.Columns.Add("QualificationID", typeof(string));
			mapDataTable.Columns.Add("ID", typeof(string));
			mapDataTable.Columns.Add("Instructor", typeof(string));
			mapDataTable.Columns.Add("DateCompleted", typeof(string));
			mapDataTable.Columns.Add("DateDue", typeof(string));
			mapDataTable.Columns.Add("ExpirationDate", typeof(string));
			mapDataTable.Columns.Add("Rating", typeof(string));

			if (qualifications != null)
			{
				for (int iItem = 0; iItem < qualifications.Count; iItem++)
				{
					DataRow mapDataRow = mapDataTable.NewRow();

					QualificationMapClass qualification = qualifications[iItem];
					qualification.DateCompleted.Format = dateFormat;
					qualification.DateDue.Format = dateFormat;
					qualification.ExpirationDate.Format = dateFormat;
					mapDataRow["Index"] = iItem;
					mapDataRow["QualificationID"] = qualification.ID;
					mapDataRow["ID"] = qualification.Number;
					mapDataRow["Instructor"] = qualification.Instructor;
					mapDataRow["DateCompleted"] = qualification.DateCompleted.ToString();
					mapDataRow["DateDue"] = qualification.DateDue.ToString();
					mapDataRow["ExpirationDate"] = qualification.ExpirationDate.ToString();
					mapDataRow["Rating"] = qualification.Rating;

					mapDataTable.Rows.Add(mapDataRow);
				}
			}

			var qualificationDataView = new DataView(mapDataTable);
			return qualificationDataView;
		}

		#endregion
	}
}