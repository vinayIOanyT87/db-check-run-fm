namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;


	public class QualificationsFormBase : FMFormBase
	{
		#region Constants and Fields

		protected int PriorEditItemIndex = -2;
		protected const string SortExpression = "QualificationsFormBase.SortExpression";
		protected const string SortDirection = "QualificationsFormBase.SortDirection";

		#endregion

		#region Properties

		protected virtual DataGrid ApplicationDataGrid
		{
			get
			{
				return null;
			}
		}

		protected virtual QUALIFICATION_TYPE QualificationType
		{
			get
			{
				return QUALIFICATION_TYPE.MAX_QUALIFICATION_TYPE;
			}
		}

		#endregion

		#region Methods

		protected void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var qualificationCollection = (QualificationCollectionClass)this.Session["QualificationCollection"];
			var qualification = new QualificationClass { Type = this.QualificationType };
			qualificationCollection.Add(qualification);
			this.ApplicationDataGrid.CurrentPageIndex = (qualificationCollection.Count - 1) / this.ApplicationDataGrid.PageSize;
			this.ApplicationDataGrid.EditItemIndex = (qualificationCollection.Count - 1) % this.ApplicationDataGrid.PageSize;
			this.EnableControls(false);
			this.UpdateView();
		}

		protected virtual void EnableControls(bool enable)
		{
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void QualificationsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					var qualificationCollection = (QualificationCollectionClass)this.Session["QualificationCollection"];
					QualificationClass qualification = qualificationCollection[Convert.ToInt32(indexLabel.Text)];
					if (qualification.IdentityGuid.IsEmpty())
					{
						qualificationCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));

						if (this.ApplicationDataGrid.Items.Count == 1 && this.ApplicationDataGrid.CurrentPageIndex > 0)
						{
							this.ApplicationDataGrid.CurrentPageIndex--;
						}
					}
					else
					{
						QualificationClass originalQualification = FMChannelHelper.MakeCall<IQualifications, QualificationClass>(
																	 x =>
																	 x.Get(this.Security, qualification.IdentityGuid)
																);

						qualification.ID = originalQualification.ID;
						qualification.Description = originalQualification.Description;
					}

					this.EnableControls(true);
					this.PriorEditItemIndex = this.ApplicationDataGrid.EditItemIndex;
					this.ApplicationDataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void QualificationsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					var qualificationCollection = (QualificationCollectionClass)this.Session["QualificationCollection"];

					QualificationClass qualification = qualificationCollection[Convert.ToInt32(indexLabel.Text)];

					const string InUseMessage = "Selected item is in use. It cannot be deleted.\n\r";

					// Non Empty IdentityGuid indicates Qualification has been committed to database
					if (!qualification.IdentityGuid.IsEmpty())
					{
						this.GetSecurity();

						// check if this qualification or training item  is currently being used
						QualificationMapCollectionClass qualificationMapCollection =
							FMChannelHelper.MakeCall<IQualificationMaps, QualificationMapCollectionClass>(
																	 x =>
																	 x.EnumerateWhereQualificationOrTrainingIsUsed(this.Security, qualification.IdentityGuid)
																);
						if (qualificationMapCollection.Count > 0)
						{
							this.ErrorHandler("Fuelsmanager", InUseMessage);
							return;
						}

						FMChannelHelper.MakeCall<IQualifications>(
																	 x =>
																	 x.Purge(this.Security, qualification.IdentityGuid)
																);
					}

					if (this.ApplicationDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.ApplicationDataGrid.EditItemIndex = -1;
						this.EnableControls(true);
					}
					else if (this.ApplicationDataGrid.EditItemIndex > e.Item.ItemIndex)
					{
						this.ApplicationDataGrid.EditItemIndex--;
					}

					qualificationCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));

					if (this.ApplicationDataGrid.CurrentPageIndex > 0
						 && this.ApplicationDataGrid.CurrentPageIndex * this.ApplicationDataGrid.PageSize
						 >= qualificationCollection.Count)
					{
						this.ApplicationDataGrid.CurrentPageIndex--;
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void QualificationsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.EnableControls(false);
			this.ApplicationDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.UpdateView();
		}

		protected void QualificationsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			this.GetSecurity();

			var editButton = (LinkButton)e.Item.FindControl("EditButton");
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if (editButton != null && deleteButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[1];//bds

				editButton.Enabled = false;
				deleteButton.Enabled = false;

				if (((QualificationType == QUALIFICATION_TYPE.PERSON_QUALIFICATION
				|| QualificationType == QUALIFICATION_TYPE.PERSON_LICENSE)
				&& Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)))
				{
					editButton.Enabled = true;
					deleteButton.Enabled = true;
				}
				if ((QualificationType == QUALIFICATION_TYPE.PERSON_TRAINING)
				&& (Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING)))
				{
					editButton.Enabled = true;
					deleteButton.Enabled = true;
				}

				if (QualificationType == QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT
				&& Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
				{
					editButton.Enabled = true;
					deleteButton.Enabled = true;
				}

				if ((QualificationType == QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE
					|| QualificationType == QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION)
					&& Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
				{
					editButton.Enabled = true;
					deleteButton.Enabled = true;
				}

				if (Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
				{
					editButton.Enabled = false;
					deleteButton.Enabled = false;
				}
			}

			if ((this.ApplicationDataGrid != null && this.ApplicationDataGrid.EditItemIndex == e.Item.ItemIndex)
				 || this.PriorEditItemIndex == e.Item.ItemIndex)
			{
				// Now set the focus to the edit control
				Control ctrl;

				if (this.ApplicationDataGrid != null && this.ApplicationDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					ctrl = e.Item.FindControl("IDTextBox");
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
		}

		protected void QualificationsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.ApplicationDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.ApplicationDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void QualificationsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				bool recalculateDatesWhereUsed = false;
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					var qualificationCollection = (QualificationCollectionClass)this.Session["QualificationCollection"];

					var idTextBox = (TextBox)e.Item.FindControl("IDTextBox");
					var descriptionTextBox = (TextBox)e.Item.FindControl("DescriptionTextBox");

					var durationTextBox = (TextBox)e.Item.FindControl("DurationTextBox");
					var reoccurrenceTextbox = (TextBox)e.Item.FindControl("ReoccurrenceTextbox");

					QualificationClass qualification = qualificationCollection[Convert.ToInt32(indexLabel.Text)];
					if (idTextBox.Text.Length == 0)
					{
						throw new Exception("ID is required");
					}

					qualification.ID = idTextBox.Text;
					qualification.Description = descriptionTextBox.Text;
					if (durationTextBox != null)
					{
						int nHours; // this is just for validation
						if (!(int.TryParse(durationTextBox.Text, out nHours)))
						{
							throw new Exception("Duration must be a numeric value in hours.");
						}

						if (Convert.ToInt32(durationTextBox.Text) < 1)
						{
							throw new Exception("Duration must be greater than or equal to 1.");
						}

						qualification.Duration = Convert.ToInt32(durationTextBox.Text);
					}
					else
					{
						qualification.Duration = 0;
					}

					if (reoccurrenceTextbox != null)
					{
						int nDays; // this is just for validation
						if (!(int.TryParse(reoccurrenceTextbox.Text, out nDays)))
						{
							throw new Exception("Reoccurrence must be a numeric value for number of days.");
						}

						if (qualification.Reoccurrence != Convert.ToInt32(reoccurrenceTextbox.Text))
						{
							recalculateDatesWhereUsed = true;
						}

						qualification.Reoccurrence = Convert.ToInt32(reoccurrenceTextbox.Text);
					}
					else
					{
						if (qualification.Reoccurrence != 0)
						{
							recalculateDatesWhereUsed = true;
						}

						qualification.Reoccurrence = 0;
					}

					this.GetSecurity();

					if (qualification.IdentityGuid.IsEmpty())
					{
						qualification.IdentityGuid = FMChannelHelper.MakeCall<IQualifications, Guid>(
																	 x =>
																	 x.Add(this.Security, qualification)
																);

						qualification.SiteGuid = this.Security.SiteGuid;
					}
					else
					{
						FMChannelHelper.MakeCall<IQualifications>(
																	 x =>
																	 x.Modify(this.Security, qualification)
																);
					}

					// after we modify the qualification we need to find out where it is used and recalculate the dates
					if (recalculateDatesWhereUsed)
					{
						QualificationMapCollectionClass qualificationMapCollection =
							FMChannelHelper.MakeCall<IQualificationMaps, QualificationMapCollectionClass>(
																	 x =>
																	 x.EnumerateWhereQualificationOrTrainingIsUsed(this.Security, qualification.IdentityGuid)
																);


						foreach (QualificationMapClass qaulificationmap in qualificationMapCollection)
						{
							if (qualification.Reoccurrence <= 0)
							{
								// if no reoccurance then just add 100 years
								qaulificationmap.DateDue.Value = qaulificationmap.DateCompleted.Value.AddYears(100);
							}
							else
							{
								// reoccurance is in days so just add them to the completed date
								qaulificationmap.DateDue.Value = qaulificationmap.DateCompleted.Value.AddDays(qualification.Reoccurrence);
							}

							qaulificationmap.ExpirationDate.Value = qaulificationmap.DateDue.Value.AddDays(1);
							FMChannelHelper.MakeCall<IQualificationMaps>(
																	 x =>
																	 x.Modify(this.Security, qaulificationmap)
																);
						}
					}

					this.EnableControls(true);
					this.PriorEditItemIndex = this.ApplicationDataGrid.EditItemIndex;
					this.ApplicationDataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.UpdateView();
			}
		}

		protected void QualificationsFormBaseLoad(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
			{
				this.SetPageFocus();
			}
		}

		protected void SetPageFocus()
		{
			const string Script = "<script language=\"jscript\">\n" + "var AddButton=document.getElementById(\"AddButton2\");\n"
			                      + "if(!AddButton.disabled)\n" + "AddButton.focus();\n" + "</script>\n";

			this.Page.ClientScript.RegisterStartupScript(this.GetType(), "page_set_focus", Script);
		}

		protected virtual void UpdateView()
		{
			this.UpdateView(null);
		}

		protected void UpdateView(FMPageSizeDropDown pageSizeDropDown)
		{
			if (this.Session[SortExpression] != null && this.Session[SortDirection] != null)
			{
				ICollection data = this.EnumerateQualifications();
				var dataView = data as DataView;

				if (dataView != null)
				{
					dataView.Sort = String.Format("{0} {1}", this.Session[SortExpression], this.Session[SortDirection]);

					pageSizeDropDown.SetPageSize(this.ApplicationDataGrid, dataView.Count);
					this.ApplicationDataGrid.DataSource = dataView;
				}
				this.ApplicationDataGrid.DataBind();
			}
			else
			{
				ICollection data = this.EnumerateQualifications();

				if (pageSizeDropDown != null)
				{
					pageSizeDropDown.SetPageSize(this.ApplicationDataGrid, data.Count);
				}

				this.ApplicationDataGrid.DataSource = data;
				this.ApplicationDataGrid.DataBind();
			}
		}

		/// <summary>
		/// This method will handle the Sort Command event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <param name="pageSizeDropDown"></param>
		protected void QualificationsDataGridSortCommand(object source, DataGridSortCommandEventArgs e, FMPageSizeDropDown pageSizeDropDown)
		{
			try
			{
				var sortExpression = this.Session[SortExpression] as string;
				var sortDirection = this.Session[SortDirection] as string;

				if (e.SortExpression != sortExpression)
				{
					this.Session[SortExpression] = e.SortExpression;
					this.Session[SortDirection] = "ASC";
				}
				else
				{
					if (sortDirection == "DESC")
					{
						this.Session[SortDirection] = "ASC";
					}
					else
					{
						this.Session[SortDirection] = "DESC";
					}
				}

				this.ApplicationDataGrid.CurrentPageIndex = 0;
				this.UpdateView(pageSizeDropDown);
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}



		private ICollection EnumerateQualifications()
		{
			var qualificationCollection = (QualificationCollectionClass)this.Session["QualificationCollection"];

			var mapDataTable = new DataTable();

			mapDataTable.Columns.Add("SiteGuid", typeof(Guid));
			mapDataTable.Columns.Add("Index", typeof(Int32));
			mapDataTable.Columns.Add("ID", typeof(string));
			mapDataTable.Columns.Add("Description", typeof(string));

			// bds
			mapDataTable.Columns.Add("Duration", typeof(Int32));
			mapDataTable.Columns.Add("Reoccurrence", typeof(Int32));

			for (int iItem = 0; iItem < qualificationCollection.Count; iItem++)
			{
				DataRow mapDataRow = mapDataTable.NewRow();

				QualificationClass qualification = qualificationCollection[iItem];
				mapDataRow["SiteGuid"] = qualification.SiteGuid;
				mapDataRow["Index"] = iItem;
				mapDataRow["ID"] = qualification.ID;
				mapDataRow["Description"] = qualification.Description;
				mapDataRow["Duration"] = qualification.Duration;
				mapDataRow["Reoccurrence"] = qualification.Reoccurrence;

				mapDataTable.Rows.Add(mapDataRow);
			}

			var qualificationDataView = new DataView(mapDataTable);
			return qualificationDataView;
		}

		private void InitializeComponent()
		{
			this.Load += this.QualificationsFormBaseLoad;
		}

		#endregion
	}
}