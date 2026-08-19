// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppointmentDetailPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AddAppointmentPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.AppointmentWebApp
{
	using System;
	using System.Globalization;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;
    using FMCore;
	using FMWebApp;

	public partial class AddAppointmentPage : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		public const string DateFormat = "yyyy-MM-dd";

		private const string AppointmentGuid = "SelectedAppointmentGuid";
		private const string PreviousAppointment = "PreviousAppointment";
		private const string AppointmentObject = "AppointmentObject";

		private AppointmentClass Appointment;

		private SiteClass CurrentSite;

		#endregion

		#region Properties

		protected bool IsGetTestScheduleMode
		{
			get
			{
				return (this.Session[AppointmentSummary.AppointmentMode] as string).DefaultIfNullOrEmpty("NORMAL")
				                                                                    .Equals("GETTEST");
			}
		}

		#endregion

		// Returns the day of the year based on day of the week, month and year. 
		// Ordinal o inidcates if it is first=1, second, third, forth, or last=5 
		// day(that is Sunday, Monday,...,Saturday) of the week.
		#region Public Methods and Operators

		public AppointmentClass UpdateData()
		{
			this.Appointment = this.Session[AppointmentObject] as AppointmentClass;

			if (this.SelectedForListBox.SelectedItem == null)
			{
				return null;
			}


			// verify that the user fields are entered properly
			if (this.CategoryComboBox.SelectedItem.Text != this.GetTranslatedText("Quality Control")
			    && this.CategoryComboBox.SelectedItem.Text != this.GetTranslatedText("Training"))
			{
				if (this.DescriptionTextBox.Text == string.Empty)
				{
					throw new Exception("Description Required");
				}
			}

			if (Convert.ToInt32(this.DurationTextBox.Text) <= 0)
			{
				throw new Exception("Invalid Duration Entered");
			}

			if (Convert.ToInt32(this.DailyTextBox.Text) <= 0)
			{
				this.DailyTextBox.Text = "1";
			}

			if (string.IsNullOrEmpty(this.Appointment.CreatedBy))
			{
				this.Appointment.CreatedBy = this.Security.UserID;
				this.Appointment.CreatedDate = DateTimeOffset.Now;
			}

			if (Convert.ToInt32(this.MonthDayTextBox.Text) <= 0 || Convert.ToInt32(this.MonthDayTextBox.Text) > 31)
			{
				this.MonthDayTextBox.Text = "1";
			}

			if (Convert.ToInt32(this.ReOccuresTextBox.Text) <= 0)
			{
				this.ReOccuresTextBox.Text = "1";
			}

			if (Convert.ToInt32(this.TextBox1.Text) <= 0)
			{
				this.TextBox1.Text = "1";
			}

			if (Convert.ToInt32(this.YearlyDayOption1TextBox.Text) <= 0
			    || Convert.ToInt32(this.YearlyDayOption1TextBox.Text) > 31)
			{
				this.YearlyDayOption1TextBox.Text = "1";
			}

			this.Appointment.Deleted = false;
			if (this.CategoryComboBox.SelectedItem != null
				&& this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Quality Control"))
			{
				ListItem testCaseli = this.TestSetDropDownList.SelectedItem;
				if (testCaseli == null)
				{
					throw new Exception("Must provide a Test Set.");
				}

				this.Appointment.Description = testCaseli.Text;
				this.Appointment.TestSetDefinitionGuid = Guid.Parse(testCaseli.Value);
			}
			else if (this.CategoryComboBox.SelectedItem != null
				&& this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Training"))
			{
				ListItem testCaseli = this.TestSetDropDownList.SelectedItem;
				if (testCaseli == null)
				{
					throw new Exception("Must provide a Training.");
				}

				this.Appointment.Description = testCaseli.Text;
				this.Appointment.TestSetDefinitionGuid = Guid.Empty;
			}
			else
			{
				this.Appointment.Description = this.DescriptionTextBox.Text;
				this.Appointment.TestSetDefinitionGuid = Guid.Empty;
			}

			if (TypeDropDownList.SelectedItem != null)
			{
				this.Appointment.AssociatedType = this.TypeDropDownList.SelectedItem.Text;
			}

			ListItem li = this.SelectedForListBox.SelectedItem;
			li.Selected = false;
			this.Appointment.AssociatedTypeGuid = Guid.Parse(li.Value);
			this.Appointment.AssetText = li.Text;

			if (this.CategoryComboBox.SelectedItem != null)
			{
				this.Appointment.AppointmentCategory = this.CategoryComboBox.SelectedItem.Text;
			}

			this.Appointment.Duration = Convert.ToInt32(this.DurationTextBox.Text);

			this.Appointment.AppointmentIsSingle = this.SingleRadioButton.Checked;

			this.Appointment.AppointmentPeriodText = this.GetTranslatedText("Single");
			this.Appointment.AppointmentPeriod = 0;
			this.Appointment.AppointmentTimeInterval = 1;
			this.Appointment.AppointmentDayOfTheWeekText = this.GetTranslatedText("Monday");
			this.Appointment._AppointmentReoccuranceInterval = 1;
			this.Appointment.AppointmentOption2Selected = false;
			this.Appointment.AppointmentTimeOptionSelectionText = this.GetTranslatedText("First");
			this.Appointment.AppointmentMonthSelectionText = this.GetTranslatedText("January");

			DateTimeOffset localDateTime = this.StartDate.CurrentValue;

			if (this.SingleRadioButton.Checked == false)
			{
				if (this.PeriodComboBox.SelectedItem != null)
				{
					this.Appointment.AppointmentPeriodText = this.PeriodComboBox.SelectedItem.Text;
				}

				if (this.PeriodComboBox.SelectedItem != null && this.PeriodComboBox.SelectedItem.Text == this.GetTranslatedText("Daily"))
				{
					this.Appointment.AppointmentTimeInterval = Convert.ToInt32(this.DailyTextBox.Text);
					this.Appointment.AppointmentPeriod = 1;
				}
				else if (this.PeriodComboBox.SelectedItem != null && this.PeriodComboBox.SelectedItem.Text == this.GetTranslatedText("Weekly"))
				{
					this.Appointment.AppointmentTimeInterval = Convert.ToInt32(this.DailyTextBox.Text);
					this.Appointment.AppointmentDayOfTheWeekText = this.DayOfTheWeekDownList.SelectedItem.Text;
					this.Appointment.AppointmentDayOfTheWeek = Convert.ToInt32(this.DayOfTheWeekDownList.SelectedItem.Value);
					this.Appointment.AppointmentPeriod = 2;

					// reset the start date to match the selected date if the user did not enter the information correctly
					DayOfWeek Day = this.StartDate.CurrentValue.DayOfWeek;
					if ((this.Appointment.AppointmentDayOfTheWeek == 0 && Day != DayOfWeek.Sunday)
					    || (this.Appointment.AppointmentDayOfTheWeek == 1 && Day != DayOfWeek.Monday)
					    || (this.Appointment.AppointmentDayOfTheWeek == 2 && Day != DayOfWeek.Tuesday)
					    || (this.Appointment.AppointmentDayOfTheWeek == 3 && Day != DayOfWeek.Wednesday)
					    || (this.Appointment.AppointmentDayOfTheWeek == 4 && Day != DayOfWeek.Thursday)
					    || (this.Appointment.AppointmentDayOfTheWeek == 5 && Day != DayOfWeek.Friday)
					    || (this.Appointment.AppointmentDayOfTheWeek == 6 && Day != DayOfWeek.Saturday))
					{
						int iDaysToOffset = this.Appointment.AppointmentDayOfTheWeek - Convert.ToInt32(Day);

						// make sure we always go forward
						if (iDaysToOffset < 0)
						{
							iDaysToOffset += 7;
						}

						localDateTime = this.StartDate.CurrentValue.AddDays(iDaysToOffset);
					}
				}
				else if (this.PeriodComboBox.SelectedItem != null && this.PeriodComboBox.SelectedItem.Text == this.GetTranslatedText("Monthly"))
				{
					this.Appointment.AppointmentOption2Selected = this.MonthlySelectByDayAndMonth.Checked;
					this.Appointment.AppointmentPeriod = 3;
					if (this.MonthlySelectByDayAndMonth.Checked == false)
					{
						// option 1
						this.Appointment.AppointmentTimeInterval = Convert.ToInt32(this.MonthDayTextBox.Text);
						this.Appointment.AppointmentReoccuranceInterval = Convert.ToInt32(this.ReOccuresTextBox.Text);

						// we need to rescale the start time to match the day that was selected
						if (this.Appointment.AppointmentTimeInterval != localDateTime.Day)
						{
							while (this.Appointment.AppointmentTimeInterval != localDateTime.Day)
							{
								localDateTime = localDateTime.AddDays(1.0);
							}
						}
					}
					else
					{
						// option2
						this.Appointment.AppointmentTimeOptionSelectionText = this.MonthDayDropDownList.SelectedItem.Text;
						this.Appointment.AppointmentTimeOptionSelection = Convert.ToInt32(this.MonthDayDropDownList.SelectedItem.Value);
						this.Appointment.AppointmentDayOfTheWeekText = this.MonthDayOfTheWeekDropDownList.SelectedItem.Text;
						this.Appointment.AppointmentDayOfTheWeek = Convert.ToInt32(this.MonthDayOfTheWeekDropDownList.SelectedItem.Value);
						this.Appointment.AppointmentMonthSelection = Convert.ToInt32(this.TextBox1.Text);
						int days = this.GetDayOfYear(
							this.Appointment.AppointmentTimeOptionSelection, 
							this.Appointment.AppointmentDayOfTheWeek, 
							localDateTime.Month, 
							localDateTime.Year);

						// make sure we always go forward
						if (days < localDateTime.DayOfYear)
						{
							localDateTime = localDateTime.AddMonths(1);
						}

						// set the start date based on the selected options
						int CurrentMonthWeek = 1;
						if (this.Appointment.AppointmentTimeOptionSelection != 5)
						{
							// set the day equal to the first day of the month
							// the military considers Sunday the first day of the week
							localDateTime = localDateTime.AddDays(-1.0 * (localDateTime.Day - 1));
							DayOfWeek WeekDay = localDateTime.DayOfWeek;
							while (true)
							{
								if (CurrentMonthWeek == 1 && this.Appointment.AppointmentTimeOptionSelection == 1
								    && Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								if (CurrentMonthWeek == 2 && this.Appointment.AppointmentTimeOptionSelection == 2
								    && Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								if (CurrentMonthWeek == 3 && this.Appointment.AppointmentTimeOptionSelection == 3
								    && Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								if (CurrentMonthWeek == 4 && this.Appointment.AppointmentTimeOptionSelection == 4
								    && Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}
								
								localDateTime = localDateTime.AddDays(1.0);
								if (WeekDay == localDateTime.DayOfWeek)
								{
									++CurrentMonthWeek;
								}
							}
						}
						else
						{
							localDateTime = localDateTime.AddMonths(1);
							localDateTime = localDateTime.AddDays(-1.0 * (localDateTime.Day - 1));
							while (true)
							{
								if (Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								localDateTime = localDateTime.AddDays(-1.0);
							}
						}
					}
				}
				else if (this.PeriodComboBox.SelectedItem != null && this.PeriodComboBox.SelectedItem.Text == this.GetTranslatedText("Yearly"))
				{
					this.Appointment.AppointmentOption2Selected = this.YearlyHappensOnThe.Checked;
					this.Appointment.AppointmentPeriod = 4;
					if (this.YearlyHappensOnThe.Checked == false)
					{
						// option 1
						this.Appointment.AppointmentMonthSelectionText = this.YearlyMonthOption1DownList.SelectedItem.Text;
						this.Appointment.AppointmentMonthSelection = Convert.ToInt32(this.YearlyMonthOption1DownList.SelectedItem.Value);
						this.Appointment.AppointmentDayOfTheMonth = Convert.ToInt32(this.YearlyDayOption1TextBox.Text);

						// scale the start date to the correct value
						int iMonthsToOffset = this.Appointment.AppointmentMonthSelection - localDateTime.Month;

						// make sure we always go forward
						if (iMonthsToOffset < 0 || (iMonthsToOffset == 0 && this.Appointment.AppointmentDayOfTheMonth < localDateTime.Day)
						    || (iMonthsToOffset == 0 && this.Appointment.AppointmentDayOfTheMonth == localDateTime.Day
						        && this.Appointment._StartDate.Value.TimeOfDay <= localDateTime.TimeOfDay))
						{
							iMonthsToOffset += 12;
						}

						localDateTime = localDateTime.AddMonths(iMonthsToOffset);

						// set the date value to what is entered
						int iDaysToOffset = this.Appointment.AppointmentDayOfTheMonth - localDateTime.Day;
						
						localDateTime = localDateTime.AddDays(iDaysToOffset);
					}
					else
					{
						// option 2
						this.Appointment.AppointmentTimeOptionSelectionText = this.MonthDayDropDownList.SelectedItem.Text;
						this.Appointment.AppointmentTimeOptionSelection = Convert.ToInt32(this.MonthDayDropDownList.SelectedItem.Value);
						this.Appointment.AppointmentMonthSelectionText = this.YearlyMonthOption2DownList.SelectedItem.Text;
						this.Appointment.AppointmentMonthSelection = Convert.ToInt32(this.YearlyMonthOption2DownList.SelectedItem.Value);
						this.Appointment.AppointmentDayOfTheWeekText = this.MonthDayOfTheWeekDropDownList.SelectedItem.Text;
						this.Appointment.AppointmentDayOfTheWeek = Convert.ToInt32(this.MonthDayOfTheWeekDropDownList.SelectedItem.Value);

						// calculate the date based on the entered data
						int iMonthsToOffset = this.Appointment.AppointmentMonthSelection - localDateTime.Month;

						// make sure we always go forward
						int days = this.GetDayOfYear(
							this.Appointment.AppointmentTimeOptionSelection, 
							this.Appointment.AppointmentDayOfTheWeek, 
							this.Appointment.AppointmentMonthSelection, 
							localDateTime.Year);

						// make sure we always go forward
						if (days < localDateTime.DayOfYear)
						{
							iMonthsToOffset += 12;
						}

						localDateTime = localDateTime.AddMonths(iMonthsToOffset);

						int CurrentMonthWeek = 1;
						if (this.Appointment.AppointmentTimeOptionSelection != 5)
						{
							// set the day equal to the first day of the month
							localDateTime = localDateTime.AddDays(-1.0 * (localDateTime.Day - 1));
							DayOfWeek WeekDay = localDateTime.DayOfWeek;
							while (true)
							{
								if (CurrentMonthWeek == 1 && this.Appointment.AppointmentTimeOptionSelection == 1
								    && Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								if (CurrentMonthWeek == 2 && this.Appointment.AppointmentTimeOptionSelection == 2
								    && Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								if (CurrentMonthWeek == 3 && this.Appointment.AppointmentTimeOptionSelection == 3
								    && Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								if (CurrentMonthWeek == 4 && this.Appointment.AppointmentTimeOptionSelection == 4
								    && Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								localDateTime = localDateTime.AddDays(1.0);
								if (WeekDay == localDateTime.DayOfWeek)
								{
									++CurrentMonthWeek;
								}
							}
						}
						else
						{
							localDateTime = localDateTime.AddMonths(1);
							localDateTime = localDateTime.AddDays(-1.0 * (localDateTime.Day - 1));
							while (true)
							{
								if (Convert.ToInt32(localDateTime.DayOfWeek) == this.Appointment.AppointmentDayOfTheWeek)
								{
									break;
								}

								localDateTime = localDateTime.AddDays(-1.0);
							}
						}
					}
				}
			}

			this.Appointment.ScheduleOnWeekends = this.WeekendCheckBox.Checked;
			this.Appointment.ScheduleOnHolidays = this.HolidayCheckBox.Checked;
			this.Appointment.StartDate = localDateTime.ToString();
			this.Appointment.SiteGuid = this.Security.SiteGuid;

			Session.Add(PreviousAppointment, this.Appointment);

			return this.Appointment;
		}

		#endregion

		#region Methods

		protected void CategoryDropDownListSelectedIndexChanged(object source, EventArgs e)
		{
			if (CategoryComboBox.SelectedItem == null)
			{
				return;
			}

			if (this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Quality Control"))
			{
				this.LoadTestSetDropDownList();

				this.PeriodComboBox.SelectByText(this.GetTranslatedText("Daily"));
			}
			else if (this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Training"))
			{
				this.LoadTrainingSetDropDownList();
			}

			this.SetControlVisibilityBasedOnTypeSelected();
		}

		protected void LoadTestSetDropDownList()
		{
			this.TestSetDropDownList.Items.Clear();

			TestSetCollectionClass testSetsCollection = FMChannelHelper.MakeCall<ITestSets, TestSetCollectionClass>(x => x.Enumerate(this.Security, null, null));

			foreach (TestSetClass testSet in testSetsCollection)
			{
				var li = new ListItem { Text = testSet.ID, Value = testSet.IdentityGuid.ToString() };
				this.TestSetDropDownList.Items.Add(li);
			}
		}

		protected void LoadTrainingSetDropDownList()
		{

			this.TestSetDropDownList.Items.Clear();

			QualificationCollectionClass qualificationCollection =
				FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
					x => x.EnumerateByType(this.Security, QUALIFICATION_TYPE.PERSON_TRAINING));

			foreach (QualificationClass TrainingItem in qualificationCollection)
			{
				this.TestSetDropDownList.Items.Add(TrainingItem.ID);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void OnMonthlyOptionsCheckedChanged(object source, EventArgs e)
		{
			this.SetControlVisibilityBasedOnTypeSelected();
		}

		protected void OnSingleReOccuringCheckedChanged(object source, EventArgs e)
		{
			this.SetControlVisibilityBasedOnTypeSelected();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.CurrentSite =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						x => x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false));

				if (!this.Page.IsPostBack)
				{
					if (this.Session[AppointmentGuid] == null && Session[PreviousAppointment] == null)
					{
						this.Session.Remove(AppointmentObject);
						this.Appointment = new AppointmentClass();
						this.ResetDisplayForAdd();
						this.LoadForDropDownList();
						this.Session.Add(AppointmentObject, this.Appointment);
						this.CategoryDropDownListSelectedIndexChanged(null, null);
					}
					else if (Session[PreviousAppointment] != null)
					{
						this.Appointment = (AppointmentClass)Session[PreviousAppointment];
						this.SetDisplayWithAppointmentParameters();
						TypeDropDownList.Enabled = true;
						SelectedForListBox.Enabled = true;
						CategoryComboBox.Enabled = true;
						Session.Remove(PreviousAppointment);
					}
					else
					{
						var appointmentGuid = this.Session[AppointmentGuid] as string;
						this.Session.Remove(AppointmentObject);

						if (string.IsNullOrEmpty(appointmentGuid))
						{
							throw new Exception("AppointmentDetail: Load Expects Appointment Guid in session.");
						}

						this.Appointment =
							FMChannelHelper.MakeCall<IAppointments, AppointmentClass>(
								x => x.EnumerateAppointmentByIdentityGuid(this.Security, Guid.Parse(appointmentGuid)));
						this.Session.Add(AppointmentObject, this.Appointment);
						this.SetDisplayWithAppointmentParameters();
					}

					this.CategoryDropDownListSelectedIndexChanged(null, null);
					this.SetControlVisibilityBasedOnTypeSelected();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PeriodComboBox_OnSelectionChanged(object source, EventArgs e)
		{
			this.SetControlVisibilityBasedOnTypeSelected();
		}

		protected void TypeDropDownListSelectedIndexChanged(object source, EventArgs e)
		{
			this.LoadForDropDownList();
			this.SetControlVisibilityBasedOnTypeSelected();
		}

		protected void WeekendCheckBox_OnCheckedChanged(object source, EventArgs e)
		{
			this.SetControlVisibilityBasedOnTypeSelected();
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.TransferBacktoCallingForm();
		}

		private string GetAppointmentInformation()
		{
			string message = string.Empty;

			message += string.Format("Start:        {0}\n", this.StartDate.Text);

			if (CategoryComboBox.SelectedItem != null)
			{
				message += string.Format("Category:        {0}\n", this.CategoryComboBox.SelectedItem.Text);
			}

			message += string.Format("Description:    {0}\n", this.DescriptionTextBox.Text);

			if (TypeDropDownList.SelectedItem != null)
			{
				if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Personnel"))
				{
					message += this.GetEntityText(this.SelectedForListBox, this.GetTranslatedText("Personnel"));
				}
				else if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tank"))
				{
					message += this.GetEntityText(this.SelectedForListBox, this.GetTranslatedText("Tank"));
				}
				else if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
				{
					message += this.GetEntityText(this.SelectedForListBox, this.GetTranslatedText("Equipment"));
				}
			}

			return message;
		}

		private int GetDayOfYear(int o, int day, int month, int year)
		{
			var r = new DateTime(year, month, 1);
			var dow = (int)r.DayOfWeek;
			if (o == 5)
			{
				r = r.AddMonths(1);
				r = r.AddDays(-1);
				dow = (int)r.DayOfWeek;
				r = r.AddDays(day - dow);
				if (day > dow)
				{
					r = r.AddDays(-7);
				}
			}
			else
			{
				r = r.AddDays((o - 1) * 7 + day - dow);
				if (day < dow)
				{
					r = r.AddDays(7);
				}
			}

			return r.DayOfYear;
		}

		private string GetEntityText(FMListBox listBox, string nameText)
		{
			string message = string.Empty;

			if (listBox.SelectedItem != null)
			{
				if (listBox.SelectionCount > 1)
				{
					message += "\n" + nameText + ":\n" + listBox.SelectedItem.Text;

					foreach (ListItem item in listBox.Items)
					{
						if (item.Selected)
						{
							if (item.Equals(listBox.SelectedItem) == false)
							{
								message += ", " + item.Text;
							}
						}
					}
				}
				else
				{
					message += string.Format("{0}:        {1}\n", nameText, listBox.SelectedItem.Text);
				}
			}

			return message;
		}

		private void HiddenButtonClick(object sender, EventArgs e)
		{
			if (this.SaveProcessing())
			{
				this.TransferBacktoCallingForm();
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OKCommand;
			this.New.Command += this.NewCommand;
			this.Cancel.Command += this.CancelCommand;
			this.HiddenButton.Click += this.HiddenButtonClick;
		}

		private void LoadForDropDownList()
		{
			this.CategoryComboBox.Clear();
			this.SelectedForListBox.Items.Clear();

			if (TypeDropDownList.SelectedItem == null)
			{
				return;
			}

			// display the selection based on the type selection
			if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Personnel"))
			{

				// populate with available personnell
				const PERSON_ROLE Role = PERSON_ROLE.MAX_PERSON_ROLE;
				PersonCollectionClass personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(x => x.EnumerateByRoleSortByName(this.Security, Role));

				foreach (PersonClass Person in personCollection)
				{
					string UserCombinedName = Person.LastName;
					if (Person.FirstName.Length > 0)
					{
						UserCombinedName += " " + Person.FirstName;
					}

					if (Person.MiddleName.Length > 0)
					{
						UserCombinedName += " " + Person.MiddleName;
					}

					var li = new ListItem(UserCombinedName, Person.MasterRecordGuid.ToString());
					this.SelectedForListBox.Items.Add(li);
				}

				this.CategoryComboBox.Items.Add(new ListItem(this.GetTranslatedText("Personal")));
				this.CategoryComboBox.Items.Add(new ListItem(this.GetTranslatedText("Medical")));
				this.CategoryComboBox.Items.Add(new ListItem(this.GetTranslatedText("Training")));
			}
			else if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
			{
				EquipmentCollectionClass equipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateManagedEquipment(this.Security));

				foreach (EquipmentClass Equipment in equipmentCollection)
				{
					var li = new ListItem(Equipment.ID, Equipment.MasterRecordGuid.ToString());
					this.SelectedForListBox.Items.Add(li);
				}

				if (!this.IsGetTestScheduleMode)
				{
					this.CategoryComboBox.Items.Add(new ListItem(this.GetTranslatedText("Maintenance")));
				}
				else
				{
					this.CategoryComboBox.Items.Add(new ListItem(this.GetTranslatedText("Quality Control")));
				}
			}
			else if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tanks"))
			{
				TankCollectionClass tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(x => x.Enumerate(this.Security));

				foreach (TankClass Tank in tankCollection)
				{
					var li = new ListItem(Tank.ID, Tank.IdentityGuid.ToString());
					this.SelectedForListBox.Items.Add(li);
				}

				if (!this.IsGetTestScheduleMode)
				{
					this.CategoryComboBox.Items.Add(new ListItem(this.GetTranslatedText("Maintenance")));
				}
				else
				{
					this.CategoryComboBox.Items.Add(new ListItem(this.GetTranslatedText("Quality Control")));
				}
			}
		}

		private void LoadStaticComboBoxes()
		{
			this.TypeDropDownList.Items.Clear();

			if (!this.IsGetTestScheduleMode && Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING))
			{
				this.TypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Personnel")));
			}
			if (Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS))
			{
				this.TypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Equipment")));
				this.TypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Tanks")));
			}

			this.PeriodComboBox.Items.Clear();
			this.PeriodComboBox.Sort = false;
			this.PeriodComboBox.Items.Add(new ListItem(this.GetTranslatedText("Daily")));
			this.PeriodComboBox.Items.Add(new ListItem(this.GetTranslatedText("Weekly")));
			this.PeriodComboBox.Items.Add(new ListItem(this.GetTranslatedText("Monthly")));
			this.PeriodComboBox.Items.Add(new ListItem(this.GetTranslatedText("Yearly")));
			this.PeriodComboBox.Style.Add("Z-INDEX", "-1");
		}

		private void NewCommand(object sender, CommandEventArgs e)
		{
			this.Session.Add("Refresh", 1);
			this.OKCommand(sender, e);
		}

		private void OKCommand(object sender, CommandEventArgs e)
		{
			try
			{
				string message;

				if (this.SelectedForListBox.SelectedItem == null)
				{
					throw new ApplicationException("Must select at least one asset for which to schedule.");
				}

				// check for invalid characters in the description field
				const string InvalidString = "'";
				char[] InvalidChars = InvalidString.ToCharArray();
				if (this.DescriptionTextBox.Text.IndexOfAny(InvalidChars) != -1)
				{
					throw new Exception("Description field contains invalid characters : " + InvalidString);
				}

				if (this.CategoryComboBox.SelectedItem.Text != this.GetTranslatedText("Quality Control")
				    && this.CategoryComboBox.SelectedItem.Text != this.GetTranslatedText("Training"))
				{
					if (string.IsNullOrEmpty(this.DescriptionTextBox.Text))
					{
						throw new ApplicationException("Description is required.");
					}
				}
				else
				{
					if (this.TestSetDropDownList.SelectedItem == null)
					{
						if (this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Training"))
						{
							throw new ApplicationException("Must provide Training.");
						}

						if (this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Quality Control"))
						{
							throw new ApplicationException("Must provide Test Set.");
						}
					}

					this.DescriptionTextBox.Text = this.TestSetDropDownList.SelectedItem.Text;
				}

				if (this.SingleRadioButton.Checked)
				{
					message = "Confirm single appointment for:\n\n";
				}
				else
				{
					message = "Confirm recurring appointment for:\n\n";
				}

				message += this.GetAppointmentInformation();

				string confirmProcessScript = "if(confirm(" + HttpUtility.JavaScriptStringEncode(message, true) + ")) {"
				                              + "document.getElementById('" + this.HiddenButton.ClientID + "').click();" + "}";

				ScriptManager.RegisterStartupScript(this, this.GetType(), "ConfirmProcessScript_Key", confirmProcessScript, true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ResetDisplayForAdd()
		{
			DateTimeOffset siteTimeToday = TimeConverter.Today(this.CurrentSite);
			this.StartDate.Text = siteTimeToday.ToString(this.CurrentSite.GetDateTimeFormatInfo());

			this.DurationTextBox.Text = "1";
			this.SingleRadioButton.Checked = true;
			this.LoadStaticComboBoxes();
			this.Appointment.AppointmentTimeInterval = 1;
			this.Appointment.AppointmentDayOfTheWeekText = this.GetTranslatedText("Monday");
			this.Appointment.AppointmentTimeOptionSelectionText = this.GetTranslatedText("First");
			this.Appointment.AppointmentMonthSelectionText = this.GetTranslatedText("January");
			this.Appointment.AppointmentDayOfTheMonth = 1;

			this.DailyTextBox.Text = "1";
			this.MonthDayTextBox.Text = "1";
			this.ReOccuresTextBox.Text = "1";
			this.TextBox1.Text = "1";
			this.YearlyDayOption1TextBox.Text = "1";
		}

		private bool SaveProcessing()
		{
			try
			{
				if (this.Session[AppointmentObject] == null)
				{
					throw new ApplicationException("Invalid Appointment Object");
				}

				while ((this.Appointment = this.UpdateData()) != null)
				{
					if (this.Appointment.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<IAppointments>(x => x.Modify(this.Security, this.Appointment));
					}
					else
					{
						FMChannelHelper.MakeCall<IAppointments>(x => x.Add(this.Security, this.Appointment));
					}

					// if this is a qc with a test set check the qc due date on the equipment to see if it has to change
					if (this.CategoryComboBox.SelectedItem != null
						&& this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Quality Control")
					    && this.Appointment.AssociatedType == this.GetTranslatedText("Equipment")
					    && this.Appointment.TestSetDefinitionGuid != Guid.Empty)
					{
							// get the associated equipment
							EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
								x => x.Get(this.Security, this.Appointment.AssociatedTypeGuid));

						if (equipment != null)
						{
							DateTimeOffset DateNow = TimeConverter.Today().AddMilliseconds(-1);

							DateTimeOffset qcdate =
								FMChannelHelper.MakeCall<IAppointments, DateTimeOffset>(
									x =>
									x.GetQCDateForTestSet(
										this.Security, 
										this.Appointment.AssociatedTypeGuid, 
										this.Appointment.TestSetDefinitionGuid, 
										this.GetTranslatedText("Equipment"), 
										DateNow, 
										equipment._QCDate.Value));

							if (qcdate > DateNow && qcdate < equipment._QCDate.Value)
							{
								equipment._QCDate.Value = qcdate;
								FMChannelHelper.MakeCall<IEquipments>(x => x.Modify(this.Security, equipment));
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return false;
			}

			return true;
		}

		private void SetControlVisibilityBasedOnTypeSelected()
		{
			this.Appointment = this.Session[AppointmentObject] as AppointmentClass;

			// turn pretty much everything off
			this.DailyOptionLable.Visible = false;
			this.DailyTextBox.Visible = false;
			this.TimePeriodLabel.Visible = false;
			this.PeriodLabel.Visible = false;
			this.PeriodComboBox.Visible = false;
			this.WeeklyHappensOnLabel.Visible = false;
			this.DayOfTheWeekDownList.Visible = false;

			this.MonthlySelectMonthDay.Visible = false;
			this.MonthDayTextBox.Visible = false;
			this.ReoccuresLabel.Visible = false;
			this.ReOccuresTextBox.Visible = false;
			this.MonthLabel.Visible = false;

			this.MonthlySelectByDayAndMonth.Visible = false;
			this.MonthDayDropDownList.Visible = false;
			this.MonthDayOfTheWeekDropDownList.Visible = false;
			this.MonthOfLabel.Visible = false;
			this.TextBox1.Visible = false;
			this.MonthOptin1MonthLabel.Visible = false;

			this.YearlyHappensEveryYearOn.Visible = false;
			this.YearlyHappensOnThe.Visible = false;
			this.YearlyMonthOption1DownList.Visible = false;
			this.YearlyDayOption1TextBox.Visible = false;
			this.YearlyMonthOption2DownList.Visible = false;

			this.DescriptionLabel.Visible = false;
			this.DescriptionTextBox.Visible = false;

			this.TestSetLabel.Visible = false;
			this.TestSetDropDownList.Visible = false;

			this.SingleRadioButton.Enabled = true;
			if (this.CategoryComboBox != null && this.CategoryComboBox.SelectedItem != null
				&& this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Quality Control"))
			{
				this.TestSetLabel.Text = this.GetTranslatedText("Test Set");
				this.TestSetLabel.Visible = true;
				this.TestSetDropDownList.Visible = true;
				this.SingleRadioButton.Checked = false;
				this.ReOccuringRadioButton.Checked = true;
				this.Appointment.AppointmentIsSingle = false;
			}
			else if (this.CategoryComboBox != null && this.CategoryComboBox.SelectedItem != null
				&& this.CategoryComboBox.SelectedItem.Text == this.GetTranslatedText("Training"))
			{
				this.TestSetLabel.Text = this.GetTranslatedText("Training");
				this.TestSetLabel.Visible = true;
				this.TestSetDropDownList.Visible = true;
			}
			else
			{
				this.DescriptionLabel.Visible = true;
				this.DescriptionTextBox.Visible = true;
				this.PeriodComboBox.Enabled = true;
			}

			if (this.ReOccuringRadioButton.Checked)
			{
				this.PeriodLabel.Visible = true;
				this.PeriodComboBox.Visible = true;
				this.PeriodComboBox.Style.Add("Z-INDEX", "-1");

				if (PeriodComboBox.SelectedItem != null)
				{
					if (this.PeriodComboBox.SelectedItem.Text == this.GetTranslatedText("Daily"))
					{
						this.DailyOptionLable.Visible = true;
						this.DailyTextBox.Visible = true;

						// default the value to one day
						this.DailyTextBox.Text = this.Appointment.AppointmentTimeInterval.ToString(CultureInfo.InvariantCulture);
						this.TimePeriodLabel.Visible = true;
						this.TimePeriodLabel.Text = this.GetTranslatedText("Day(s)");
					}
					else if (this.PeriodComboBox.SelectedItem.Text == this.GetTranslatedText("Weekly"))
					{
						this.DailyOptionLable.Visible = true;
						this.DailyTextBox.Visible = true;

						// default the value to one week
						this.DailyTextBox.Text = this.Appointment.AppointmentTimeInterval.ToString(CultureInfo.InvariantCulture);
						this.TimePeriodLabel.Visible = true;
						this.TimePeriodLabel.Text = this.GetTranslatedText("Week(s)");

						// load the drop dwon list based on the users selections
						this.WeeklyHappensOnLabel.Visible = true;
						this.DayOfTheWeekDownList.Items.Clear();
						this.DayOfTheWeekDownList.Sort = false;
						this.DayOfTheWeekDownList.Visible = true;
						if (this.WeekendCheckBox.Checked)
						{
							this.DayOfTheWeekDownList.Items.Add(new ListItem(this.GetTranslatedText("Sunday"), "0"));
						}

						this.DayOfTheWeekDownList.Items.Add(new ListItem(this.GetTranslatedText("Monday"), "1"));
						this.DayOfTheWeekDownList.Items.Add(new ListItem(this.GetTranslatedText("Tuesday"), "2"));
						this.DayOfTheWeekDownList.Items.Add(new ListItem(this.GetTranslatedText("Wednesday"), "3"));
						this.DayOfTheWeekDownList.Items.Add(new ListItem(this.GetTranslatedText("Thursday"), "4"));
						this.DayOfTheWeekDownList.Items.Add(new ListItem(this.GetTranslatedText("Friday"), "5"));
						if (this.WeekendCheckBox.Checked)
						{
							this.DayOfTheWeekDownList.Items.Add(new ListItem(this.GetTranslatedText("Saturday"), "6"));
						}

						if (this.WeekendCheckBox.Checked == false
						    && (this.Appointment.AppointmentDayOfTheWeekText == this.GetTranslatedText("Sunday")
							   || this.Appointment.AppointmentDayOfTheWeekText == this.GetTranslatedText("Saturday")))
						{
							this.DayOfTheWeekDownList.SelectedIndex =
								this.DayOfTheWeekDownList.Items.IndexOf(
									this.DayOfTheWeekDownList.Items.FindByText(this.GetTranslatedText("Monday")));
						}
						else
						{
							this.DayOfTheWeekDownList.SelectedIndex =
								this.DayOfTheWeekDownList.Items.IndexOf(
									this.DayOfTheWeekDownList.Items.FindByText(this.Appointment.AppointmentDayOfTheWeekText));
						}
					}
					else if (this.PeriodComboBox.SelectedItem.Text == this.GetTranslatedText("Monthly"))
					{
						this.MonthlySelectMonthDay.Visible = true;
						if (this.MonthlySelectMonthDay.Checked == false && this.MonthlySelectByDayAndMonth.Checked == false)
						{
							this.MonthlySelectMonthDay.Checked = true;
						}

						this.MonthlySelectByDayAndMonth.Visible = true;
						this.MonthDayTextBox.Visible = true;
						if (this.MonthDayTextBox.Text == string.Empty)
						{
							this.MonthDayTextBox.Text = "1";
						}

						this.ReoccuresLabel.Visible = true;
						this.ReOccuresTextBox.Visible = true;
						if (this.ReOccuresTextBox.Text == string.Empty)
						{
							this.ReOccuresTextBox.Text = "1";
						}

						this.MonthLabel.Visible = true;
						this.MonthDayDropDownList.Items.Clear();
						this.MonthDayDropDownList.Visible = true;

						// load the list with the available days
						this.MonthDayOfTheWeekDropDownList.Items.Clear();
						this.MonthDayOfTheWeekDropDownList.Visible = true;
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("First"), "1"));
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Second"), "2"));
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Third"), "3"));
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Fourth"), "4"));
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Last"), "5"));

						this.MonthDayDropDownList.SelectedIndex =
							this.MonthDayDropDownList.Items.IndexOf(
								this.MonthDayDropDownList.Items.FindByText(this.Appointment.AppointmentTimeOptionSelectionText));

						if (this.WeekendCheckBox.Checked)
						{
							this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Sunday"), "0"));
						}

						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Monday"), "1"));
						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Tuesday"), "2"));
						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Wednesday"), "3"));
						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Thursday"), "4"));
						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Friday"), "5"));
						if (this.WeekendCheckBox.Checked)
						{
							this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Saturday"), "6"));
						}

						if (this.WeekendCheckBox.Checked == false
						    && (this.Appointment.AppointmentDayOfTheWeekText == this.GetTranslatedText("Sunday")
							   || this.Appointment.AppointmentDayOfTheWeekText == this.GetTranslatedText("Saturday")))
						{
							this.MonthDayOfTheWeekDropDownList.SelectedIndex =
								this.MonthDayOfTheWeekDropDownList.Items.IndexOf(
									this.MonthDayOfTheWeekDropDownList.Items.FindByText(this.GetTranslatedText("Monday")));
						}
						else
						{
							this.MonthDayOfTheWeekDropDownList.SelectedIndex =
								this.MonthDayOfTheWeekDropDownList.Items.IndexOf(
									this.MonthDayOfTheWeekDropDownList.Items.FindByText(this.Appointment.AppointmentDayOfTheWeekText));
						}

						this.MonthOfLabel.Visible = true;
						this.TextBox1.Visible = true;
						this.MonthOptin1MonthLabel.Visible = true;

						this.MonthDayDropDownList.Enabled = true;
						this.MonthDayOfTheWeekDropDownList.Enabled = true;
						this.MonthOfLabel.Enabled = true;
						this.TextBox1.Enabled = true;
						this.MonthOptin1MonthLabel.Enabled = true;
						this.MonthDayTextBox.Enabled = true;
						this.ReoccuresLabel.Enabled = true;
						this.ReOccuresTextBox.Enabled = true;
						this.MonthLabel.Enabled = true;

						// disable the controls based on the radio selection
						if (this.MonthlySelectMonthDay.Checked)
						{
							this.MonthDayDropDownList.Enabled = false;
							this.MonthDayOfTheWeekDropDownList.Enabled = false;
							this.MonthOfLabel.Enabled = false;
							this.TextBox1.Enabled = false;
							this.MonthOptin1MonthLabel.Enabled = false;
						}
						else
						{
							this.MonthDayTextBox.Enabled = false;
							this.ReoccuresLabel.Enabled = false;
							this.ReOccuresTextBox.Enabled = false;
							this.MonthLabel.Enabled = false;
						}
					}
					else if (this.PeriodComboBox.SelectedItem.Text == this.GetTranslatedText("Yearly"))
					{
						this.YearlyHappensEveryYearOn.Visible = true;
						this.YearlyHappensOnThe.Visible = true;
						if (this.YearlyHappensEveryYearOn.Checked == false && this.YearlyHappensOnThe.Checked == false)
						{
							this.YearlyHappensEveryYearOn.Checked = true;
						}

						this.YearlyMonthOption1DownList.Visible = true;
						this.YearlyDayOption1TextBox.Visible = true;

						// for yearly we reuse the monthly MonthDayDropDownList and MonthDayOfTheWeekDropDownList lists
						this.MonthDayDropDownList.Items.Clear();
						this.MonthDayDropDownList.Visible = true;
						this.MonthDayOfTheWeekDropDownList.Items.Clear();
						this.MonthDayOfTheWeekDropDownList.Visible = true;
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("First"), "1"));
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Second"), "2"));
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Third"), "3"));
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Fourth"), "4"));
						this.MonthDayDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Last"), "5"));
						this.MonthDayDropDownList.SelectedIndex =
							this.MonthDayDropDownList.Items.IndexOf(
								this.MonthDayDropDownList.Items.FindByText(this.Appointment.AppointmentTimeOptionSelectionText));

						if (this.WeekendCheckBox.Checked)
						{
							this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Sunday"), "0"));
						}

						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Monday"), "1"));
						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Tuesday"), "2"));
						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Wednesday"), "3"));
						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Thursday"), "4"));
						this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Friday"), "5"));
						if (this.WeekendCheckBox.Checked)
						{
							this.MonthDayOfTheWeekDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Saturday"), "6"));
						}

						if (this.WeekendCheckBox.Checked == false
						    && (this.Appointment.AppointmentDayOfTheWeekText == this.GetTranslatedText("Sunday")
							   || this.Appointment.AppointmentDayOfTheWeekText == this.GetTranslatedText("Saturday")))
						{
							this.MonthDayOfTheWeekDropDownList.SelectedIndex =
								this.MonthDayOfTheWeekDropDownList.Items.IndexOf(
									this.MonthDayOfTheWeekDropDownList.Items.FindByText(this.GetTranslatedText("Monday")));
						}
						else
						{
							this.MonthDayOfTheWeekDropDownList.SelectedIndex =
								this.MonthDayOfTheWeekDropDownList.Items.IndexOf(
									this.MonthDayOfTheWeekDropDownList.Items.FindByText(this.Appointment.AppointmentDayOfTheWeekText));
						}

						this.MonthOfLabel.Visible = true;
						this.YearlyMonthOption2DownList.Visible = true;

						// populate the month lists
						this.YearlyMonthOption2DownList.Items.Clear();
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("January"), "1"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("February"), "2"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("March"), "3"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("April"), "4"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("May"), "5"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("June"), "6"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("July"), "7"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("August"), "8"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("September"), "9"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("October"), "10"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("November"), "11"));
						this.YearlyMonthOption2DownList.Items.Add(new ListItem(this.GetTranslatedText("December"), "12"));

						this.YearlyMonthOption2DownList.SelectedIndex =
							this.YearlyMonthOption2DownList.Items.IndexOf(
								this.YearlyMonthOption2DownList.Items.FindByText(this.Appointment.AppointmentMonthSelectionText));

						this.YearlyMonthOption1DownList.Items.Clear();
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("January"), "1"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("February"), "2"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("March"), "3"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("April"), "4"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("May"), "5"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("June"), "6"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("July"), "7"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("August"), "8"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("September"), "9"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("October"), "10"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("November"), "11"));
						this.YearlyMonthOption1DownList.Items.Add(new ListItem(this.GetTranslatedText("December"), "12"));

						this.YearlyMonthOption1DownList.SelectedIndex =
							this.YearlyMonthOption2DownList.Items.IndexOf(
								this.YearlyMonthOption2DownList.Items.FindByText(this.Appointment.AppointmentMonthSelectionText));

						this.YearlyDayOption1TextBox.Text = this.Appointment.AppointmentDayOfTheMonth.ToString(CultureInfo.InvariantCulture);
						this.YearlyMonthOption2DownList.Enabled = true;
						this.MonthDayDropDownList.Enabled = true;
						this.MonthDayOfTheWeekDropDownList.Enabled = true;
						this.MonthOfLabel.Enabled = true;
						this.YearlyMonthOption1DownList.Enabled = true;
						this.YearlyDayOption1TextBox.Enabled = true;

						// enable/disable the controls based on the selection
						if (this.YearlyHappensEveryYearOn.Checked)
						{
							this.YearlyMonthOption2DownList.Enabled = false;
							this.MonthDayDropDownList.Enabled = false;
							this.MonthDayOfTheWeekDropDownList.Enabled = false;
							this.MonthOfLabel.Enabled = false;
						}
						else
						{
							this.YearlyMonthOption1DownList.Enabled = false;
							this.YearlyDayOption1TextBox.Enabled = false;
						}
					}
				}
			}
		}

		private void SetDisplayWithAppointmentParameters()
		{
			this.StartDate.Text = this.Appointment.StartDate;
			this.DurationTextBox.Text = this.Appointment.Duration.ToString(CultureInfo.InvariantCulture);
			if (this.Appointment.AppointmentIsSingle == false)
			{
				this.SingleRadioButton.Checked = false;
				this.ReOccuringRadioButton.Checked = true;
			}
			else
			{
				this.SingleRadioButton.Checked = true;
				this.ReOccuringRadioButton.Checked = false;
			}

			this.LoadStaticComboBoxes();

			// set the control values and disable the appropriate controls
			if (TypeDropDownList.SelectedItem != null)
			{
				this.TypeDropDownList.SelectedItem.Text = this.Appointment.AssociatedType;
			}

			this.LoadForDropDownList();

			this.SelectedForListBox.Items.Clear();
			if (TypeDropDownList.SelectedItem != null)
			{
				if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Personnel"))
				{
					PersonClass person =
						FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, this.Appointment.AssociatedTypeGuid));

					string UserCombinedName = person.LastName;

					if (person.FirstName.Length > 0)
					{
						UserCombinedName += " " + person.FirstName;
					}

					if (person.MiddleName.Length > 0)
					{
						UserCombinedName += " " + person.MiddleName;
					}

					var li = new ListItem(UserCombinedName, person.MasterRecordGuid.ToString());
					this.SelectedForListBox.Items.Add(li);
				}
				else if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
				{
					EquipmentClass equipment =
						FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
							x => x.Get(this.Security, this.Appointment.AssociatedTypeGuid));

					this.SelectedForListBox.Items.Add(new ListItem(equipment.ID, equipment.MasterRecordGuid.ToString()));
				}
				else if (this.TypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tanks"))
				{
					TankClass tank =
						FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(this.Security, this.Appointment.AssociatedTypeGuid));

					this.SelectedForListBox.Items.Add(new ListItem(tank.ID, tank.IdentityGuid.ToString()));
				}
			}

			if (this.Appointment.TestSetDefinitionGuid != Guid.Empty)
			{
				this.LoadTestSetDropDownList();
				this.TestSetDropDownList.SelectedIndex =
					this.TestSetDropDownList.Items.IndexOf(this.TestSetDropDownList.Items.FindByText(this.Appointment.Description));
			}
			else if (this.Appointment.AppointmentCategory == this.GetTranslatedText("Training"))
			{
				this.LoadTrainingSetDropDownList();
				this.TestSetDropDownList.SelectedIndex =
					this.TestSetDropDownList.Items.IndexOf(this.TestSetDropDownList.Items.FindByText(this.Appointment.Description));
			}

			this.SelectedForListBox.SelectedIndex =
				this.SelectedForListBox.Items.IndexOf(this.SelectedForListBox.Items.FindByText(this.Appointment.AssetText));

			if (CategoryComboBox.SelectedItem != null)
			{
				this.CategoryComboBox.SelectedItem.Text = this.Appointment.AppointmentCategory;
			}

			this.TypeDropDownList.Enabled = false;
			this.SelectedForListBox.Enabled = false;
			this.CategoryComboBox.Enabled = false;
			this.DescriptionTextBox.Text = this.Appointment.Description;
			this.WeekendCheckBox.Checked = this.Appointment.ScheduleOnWeekends;
			this.HolidayCheckBox.Checked = this.Appointment.ScheduleOnHolidays;
			this.PeriodComboBox.Text = this.Appointment.AppointmentPeriodText;
			this.DailyTextBox.Text = this.Appointment.AppointmentTimeInterval.ToString(CultureInfo.InvariantCulture);
			this.MonthDayTextBox.Text = this.Appointment.AppointmentTimeInterval.ToString(CultureInfo.InvariantCulture);
			this.ReOccuresTextBox.Text = this.Appointment.AppointmentReoccuranceInterval.ToString(CultureInfo.InvariantCulture);
			this.DayOfTheWeekDownList.Text = this.Appointment.AppointmentDayOfTheWeekText;

			this.MonthDayDropDownList.Text = this.Appointment.AppointmentTimeOptionSelectionText;

			if (this.Appointment.AppointmentOption2Selected == false)
			{
				this.MonthlySelectMonthDay.Checked = true;
				this.MonthlySelectByDayAndMonth.Checked = false;
				this.YearlyHappensEveryYearOn.Checked = true;
				this.YearlyHappensOnThe.Checked = false;
			}
			else
			{
				this.MonthlySelectMonthDay.Checked = false;
				this.MonthlySelectByDayAndMonth.Checked = true;
				this.YearlyHappensEveryYearOn.Checked = false;
				this.YearlyHappensOnThe.Checked = true;
			}

			this.TextBox1.Text = this.Appointment.AppointmentMonthSelection.ToString(CultureInfo.InvariantCulture);

			this.YearlyDayOption1TextBox.Text = this.Appointment.AppointmentDayOfTheMonth.ToString(CultureInfo.InvariantCulture);
		}

		private void TransferBacktoCallingForm()
		{
			if (this.Session["Refresh"] != null)
			{
				this.Session.Remove("Refresh");
				this.Session.Remove(PreviousAppointment);
				this.Session.Remove(AppointmentGuid);
				this.Redirect(this.Request.RawUrl);
			}
			else
			{
				this.Redirect("AppointmentSummary.aspx?Mode=" + this.Session[AppointmentSummary.AppointmentMode]);
			}
		}

		#endregion
	}
}