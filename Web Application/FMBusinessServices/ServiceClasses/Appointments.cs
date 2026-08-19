using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for NotessClass.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AppointmentsClass : IDependency, IAppointments
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public AppointmentsClass()
		{
		}

		private void Validate(AppointmentClass Appointment)
		{

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AppointmentClass Appointment)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Appointment == null)
			{
				throw new ArgumentNullException("Appointment");
			}

			if (!security.HasRight(RIGHT.MODIFY_APPOINTMENTS))
				throw new FMInsufficientRightsException();

			Validate(Appointment);

			Appointment.UpdatedDate = Appointment.CreatedDate;
			Appointment.UpdatedBy = security.UserID;

			Appointment.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(Appointment);
			EntityToSiteMaps.Add(security, EntityToSiteMap, GetType().GUID);

			return Appointment.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AppointmentClass Appointment)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Appointment == null)
			{
				throw new ArgumentNullException("Appointment");
			}

			Validate(Appointment);

			AppointmentClass OldAppointment = Get(security, Appointment.IdentityGuid);
			if (OldAppointment.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Appointment Not Found"));
			}


			Appointment.UpdatedDate = DateTimeOffset.Now;
			Appointment.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, Appointment.EntityType, Appointment.IdentityGuid);

			if (Appointment.SiteGuid != OldAppointment.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
					EntityToSiteMaps.Purge(security, EntityToSiteMap);

				// Create Entity to Site Map
				EntityToSiteMapClass NewEntityToSiteMap = new EntityToSiteMapClass(Appointment);
				EntityToSiteMaps.Add(security, NewEntityToSiteMap, GetType().GUID);
			}

			// Verify that new ID will not conflict with EntityToSiteMaps
			else
			{
				foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
				{
					Guid siteGuid = security.SiteGuid;
					security.SiteGuid = EntityToSiteMap.SiteGuid;
					Guid identityGuid = GetIdentityGuid(security, Appointment.ID);
					security.SiteGuid = siteGuid;

					if (identityGuid != Guid.Empty
				  && identityGuid != EntityToSiteMap.IdentityGuid)
						throw (new Exception("Test Set Exits"));
				}
			}

		}

		public AppointmentClass Get(SecurityClass security, Guid identityGuid)
		{
			return this.GetByIncludeTests(security, identityGuid, false);
		}

		public AppointmentClass GetByIncludeTests(SecurityClass security, Guid identityGuid, bool includeTests)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			AppointmentClass Appointment = new AppointmentClass();
			Appointment.IdentityGuid = identityGuid;

			if (identityGuid != Guid.Empty)
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					Appointment.EnumerateByIdentityGuid(cmd, security, ContextUtil.IsInTransaction, Appointment.IdentityGuid);
					Appointment.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return Appointment;
		}

		public Guid GetIdentityGuid(SecurityClass security, string ID)
		{
			AppointmentClass Appointment = null;

			if (security == null)
				throw new ArgumentNullException("Security");

			if (ID == "{All}"
			|| ID == "{Unassigned}"
			|| ID == "{None}")
				return Guid.Empty;

			Appointment = new AppointmentClass();
			Appointment.ID = ID;

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				Appointment.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return Appointment.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid appointmentGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			AppointmentClass Appointment = Get(security, appointmentGuid);
			if (Appointment.IdentityGuid == Guid.Empty)
			{
				return;
			}

            // Purge from EntityToSiteMap
            var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, Appointment.EntityType, Appointment.IdentityGuid);
			foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
				EntityToSiteMaps.Purge(security, EntityToSiteMap);


			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByAssetID(SecurityClass security, Guid assetID, string appointmentType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			AppointmentClass Appointment = new AppointmentClass();
			Appointment.AssociatedType = appointmentType;

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.PurgeByAssetIDSQL(cmd, assetID);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}

		public AppointmentCollectionClass EnumerateScheduledAndOverdue(SecurityClass security, DateTimeOffset startDate, string appointmentType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			AppointmentCollectionClass appointmentCollection = new AppointmentCollectionClass();
			AppointmentClass appointment = new AppointmentClass();

			startDate = startDate.AddDays(1);

			DataSet set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				appointment.EnumerateScheduledAndOverdueSQL(cmd, security, startDate, appointmentType, ContextUtil.IsInTransaction);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				appointment = new AppointmentClass();
				appointment.Load(set);
				appointment.DueDate = appointment.StartDateObject.Value;
				appointmentCollection.Add(appointment);
				table.Rows.RemoveAt(0);
			}

			return appointmentCollection;
		}

		public AppointmentCollectionClass EnumerateByAssetGuid(SecurityClass security, string appointmentType, Guid entityGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			AppointmentCollectionClass AppointmentCollection = new AppointmentCollectionClass();
			AppointmentClass Appointment = new AppointmentClass();

			DataSet Set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.EnumerateByAssetGuidSQL(cmd, security, ContextUtil.IsInTransaction, appointmentType, entityGuid);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				Appointment = new AppointmentClass();
				Appointment.Load(Set);
				AppointmentCollection.Add(Appointment);

				Table.Rows.RemoveAt(0);
			}

			return AppointmentCollection;
		}

		public AppointmentCollectionClass EnumerateByStartStopTime(SecurityClass security, string appointmentType, DateTimeOffset StartDate, DateTimeOffset EndDate)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			AppointmentCollectionClass AppointmentCollection = new AppointmentCollectionClass();
			AppointmentClass Appointment = new AppointmentClass();

			// add one day to the end so the selection makes sense
			EndDate = EndDate.AddDays(1.0);

			DataSet Set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.EnumerateAll(cmd, security, ContextUtil.IsInTransaction, appointmentType);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				Appointment = new AppointmentClass();
				Appointment.Load(Set);
				// calculate the scheduled times for this object based on the start, stop and object parameters
				CreateDataRecordsBasedOnStartAndStopTimes(security, AppointmentCollection, Appointment, StartDate, EndDate, Set, false);

				Table.Rows.RemoveAt(0);
			}

			return AppointmentCollection;
		}

		public AppointmentClass EnumerateAppointmentByIdentityGuid(SecurityClass security, Guid appointmentGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			AppointmentCollectionClass AppointmentCollection = new AppointmentCollectionClass();
			AppointmentClass Appointment = new AppointmentClass();

			DataSet Set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.EnumerateByIdentityGuid(cmd, security, ContextUtil.IsInTransaction, appointmentGuid);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 1)
			{
				Appointment = new AppointmentClass();
				Appointment.Load(Set);
				AppointmentCollection.Add(Appointment);
				Table.Rows.RemoveAt(0);
			}
			else
				throw new ArgumentNullException("Invalid Identity Guid");

			return Appointment;
		}

		private void CreateDataRecordsBasedOnStartAndStopTimes(SecurityClass security,
																AppointmentCollectionClass AppointmentCollection,
																AppointmentClass Appointment,
																DateTimeOffset StartDate,
																DateTimeOffset EndDate,
																DataSet Set,
																Boolean FindOnlyNextOne)
		{
			DateTimeOffset LastDateTime;
			DateTimeOffset LastAssignedDateTime;
			int iDaysToSubtract = 0;

			// check that the start date is within range
			if (Appointment._StartDate.Value > EndDate)
				return;

			// always start with the due date equal to the start date
			Appointment.DueDate = Appointment._StartDate.Value;
			LastAssignedDateTime = Appointment.DueDate;
			LastAssignedDateTime = LastAssignedDateTime.AddYears(-100);

			// load the site holiday configuration here so we only due this once per schedule record
			SchedulesClass Schedules = new SchedulesClass();

			ScheduleCollectionClass HolidayScheduleCollection;
			HolidayScheduleCollection = Schedules.EnumerateByEntityGuidAndType(security,
																				security.SiteGuid,
																				SCHEDULE_TYPE.HOLIDAY_TYPE);

			// determine if this record needs to be added
			if (Appointment._StartDate.Value >= StartDate &&
				Appointment._StartDate.Value < EndDate)
			{
				LastDateTime = Appointment.DueDate;
				AppointmentCollection.Add(Appointment);
				LastAssignedDateTime = Appointment.DueDate;
				iDaysToSubtract = CheckAppointmentIsOnAWeekendOrHoliday(security,
																		Appointment.DueDate,
																		Appointment.ScheduleOnWeekends,
																		Appointment.ScheduleOnHolidays,
																		HolidayScheduleCollection);
				Appointment.DueDate = Appointment.DueDate.AddDays(iDaysToSubtract);
				// anytime we add a object we need to create a new one and reload the data
				if (Appointment.DueDate < EndDate &&
					Appointment.DueDate >= StartDate)
				{
					Appointment = new AppointmentClass();
					Appointment.Load(Set);
					Appointment.DueDate = LastDateTime;
				}
			}

			// if this is a single appointment we are out of here
			if (Appointment.AppointmentIsSingle == true)
				return;

			int currentColCount = AppointmentCollection.Count;

			// calculate the appointments based on the entered paramenters
			while (Appointment.DueDate < EndDate)
			{
				if (FindOnlyNextOne && currentColCount < AppointmentCollection.Count)
				{
					return; //break out for performance reasons because we only care about the next due one
				}

				if (Appointment.AppointmentPeriod == 1)	// daily
				{
					Appointment.DueDate = Appointment.DueDate.AddDays(Appointment.AppointmentTimeInterval);
					if (Appointment.DueDate < EndDate)
					{
						iDaysToSubtract = 0;
						// check if this day falls on a weekend and use weekend is selected
						iDaysToSubtract = CheckAppointmentIsOnAWeekendOrHoliday(security,
																				Appointment.DueDate,
																				Appointment.ScheduleOnWeekends,
																				Appointment.ScheduleOnHolidays,
																				HolidayScheduleCollection);

						LastDateTime = Appointment.DueDate;
						Appointment.DueDate = Appointment.DueDate.AddDays(iDaysToSubtract);
						if (LastAssignedDateTime != Appointment.DueDate &&
							Appointment.DueDate >= StartDate)
						{
							LastAssignedDateTime = Appointment.DueDate;
							AppointmentCollection.Add(Appointment);
							Appointment = new AppointmentClass();
							Appointment.Load(Set);
						}
						Appointment.DueDate = LastDateTime;
					}
				}
				else if (Appointment.AppointmentPeriod == 2)	// weekly
				{
					Appointment.DueDate = Appointment.DueDate.AddDays(Appointment.AppointmentTimeInterval * 7);
					if (Appointment.DueDate < EndDate)
					{
						iDaysToSubtract = 0;
						// check if this day falls on a weekend and use weekend is selected
						iDaysToSubtract = CheckAppointmentIsOnAWeekendOrHoliday(security,
																				Appointment.DueDate,
																				Appointment.ScheduleOnWeekends,
																				Appointment.ScheduleOnHolidays,
																				HolidayScheduleCollection);

						LastDateTime = Appointment.DueDate;
						Appointment.DueDate = Appointment.DueDate.AddDays(iDaysToSubtract);
						if (LastAssignedDateTime != Appointment.DueDate &&
							Appointment.DueDate >= StartDate)
						{
							LastAssignedDateTime = Appointment.DueDate;
							AppointmentCollection.Add(Appointment);
							Appointment = new AppointmentClass();
							Appointment.Load(Set);
						}
						Appointment.DueDate = LastDateTime;
					}
				}
				else if (Appointment.AppointmentPeriod == 3)	// monthly
				{
					if (Appointment.AppointmentOption2Selected == false)
					{
						Appointment.DueDate = Appointment.DueDate.AddMonths(Appointment.AppointmentReoccuranceInterval);
						if (Appointment.DueDate < EndDate)
						{
							iDaysToSubtract = 0;
							// check if this day falls on a weekend and use weekend is selected
							iDaysToSubtract = CheckAppointmentIsOnAWeekendOrHoliday(security,
																					Appointment.DueDate,
																					Appointment.ScheduleOnWeekends,
																					Appointment.ScheduleOnHolidays,
																					HolidayScheduleCollection);

							LastDateTime = Appointment.DueDate;
							Appointment.DueDate = Appointment.DueDate.AddDays(iDaysToSubtract);
							if (LastAssignedDateTime != Appointment.DueDate &&
								Appointment.DueDate >= StartDate)
							{
								LastAssignedDateTime = Appointment.DueDate;
								AppointmentCollection.Add(Appointment);
								Appointment = new AppointmentClass();
								Appointment.Load(Set);
							}
							Appointment.DueDate = LastDateTime;
						}
					}
					else
					{
						bool bFound = false;
						int CurrentMonthWeek = 1;
						if (Appointment.AppointmentTimeOptionSelection != 5)
						{
							// set the due date to the begining of the month that is selected
							Appointment.DueDate = Appointment.DueDate.AddMonths(Appointment.AppointmentMonthSelection);
							Appointment.DueDate = Appointment.DueDate.AddDays(-1.0 * (Appointment.DueDate.Day - 1));
							DayOfWeek WeekDay = Appointment.DueDate.DayOfWeek;
							while (bFound == false)
							{
								if (CurrentMonthWeek == 1 &&
									Appointment.AppointmentTimeOptionSelection == 1 &&
									System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}
								if (CurrentMonthWeek == 2 &&
									Appointment.AppointmentTimeOptionSelection == 2 &&
									System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}
								if (CurrentMonthWeek == 3 &&
									Appointment.AppointmentTimeOptionSelection == 3 &&
									System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}
								if (CurrentMonthWeek == 4 &&
									Appointment.AppointmentTimeOptionSelection == 4 &&
									System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}

								Appointment.DueDate = Appointment.DueDate.AddDays(1.0);
								if (WeekDay == Appointment.DueDate.DayOfWeek)
								{
									++CurrentMonthWeek;
								}
							}
						}
						else
						{
							Appointment.DueDate = Appointment.DueDate.AddMonths(Appointment.AppointmentMonthSelection);
							Appointment.DueDate.AddMonths(1);
							Appointment.DueDate = Appointment.DueDate.AddDays(-1.0 * (Appointment.DueDate.Day - 1));
							while (bFound == false)
							{
								if (System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}

								Appointment.DueDate = Appointment.DueDate.AddDays(1.0);
							}
						}
						if (Appointment.DueDate < EndDate)
						{
							iDaysToSubtract = 0;
							// check if this day falls on a weekend and use weekend is selected
							iDaysToSubtract = CheckAppointmentIsOnAWeekendOrHoliday(security,
																					Appointment.DueDate,
																					Appointment.ScheduleOnWeekends,
																					Appointment.ScheduleOnHolidays,
																					HolidayScheduleCollection);

							LastDateTime = Appointment.DueDate;
							Appointment.DueDate = Appointment.DueDate.AddDays(iDaysToSubtract);
							if (LastAssignedDateTime != Appointment.DueDate &&
								Appointment.DueDate >= StartDate)
							{
								LastAssignedDateTime = Appointment.DueDate;
								AppointmentCollection.Add(Appointment);
								Appointment = new AppointmentClass();
								Appointment.Load(Set);
							}
							Appointment.DueDate = LastDateTime;
						}
					}
				}
				else if (Appointment.AppointmentPeriod == 4)	// yearly
				{
					if (Appointment.AppointmentOption2Selected == false)
					{
						Appointment.DueDate = Appointment.DueDate.AddYears(1);	// always add one year
						if (Appointment.DueDate < EndDate)
						{
							iDaysToSubtract = 0;
							// check if this day falls on a weekend and use weekend is selected
							iDaysToSubtract = CheckAppointmentIsOnAWeekendOrHoliday(security,
																					Appointment.DueDate,
																					Appointment.ScheduleOnWeekends,
																					Appointment.ScheduleOnHolidays,
																					HolidayScheduleCollection);

							LastDateTime = Appointment.DueDate;
							Appointment.DueDate = Appointment.DueDate.AddDays(iDaysToSubtract);
							if (LastAssignedDateTime != Appointment.DueDate &&
								Appointment.DueDate >= StartDate)
							{
								LastAssignedDateTime = Appointment.DueDate;
								AppointmentCollection.Add(Appointment);
								Appointment = new AppointmentClass();
								Appointment.Load(Set);
							}
							Appointment.DueDate = LastDateTime;
						}
					}
					else
					{
						Appointment.DueDate = Appointment.DueDate.AddYears(1);	// always add one year
						bool bFound = false;
						int CurrentMonthWeek = 1;
						if (Appointment.AppointmentTimeOptionSelection != 5)
						{
							Appointment.DueDate = Appointment.DueDate.AddDays(-1.0 * (Appointment.DueDate.Day - 1));
							DayOfWeek WeekDay = Appointment.DueDate.DayOfWeek;
							while (bFound == false)
							{
								if (CurrentMonthWeek == 1 &&
									Appointment.AppointmentTimeOptionSelection == 1 &&
									System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}
								if (CurrentMonthWeek == 2 &&
									Appointment.AppointmentTimeOptionSelection == 2 &&
									System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}
								if (CurrentMonthWeek == 3 &&
									Appointment.AppointmentTimeOptionSelection == 3 &&
									System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}
								if (CurrentMonthWeek == 4 &&
									Appointment.AppointmentTimeOptionSelection == 4 &&
									System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}

								Appointment.DueDate = Appointment.DueDate.AddDays(1.0);
								if (WeekDay == Appointment.DueDate.DayOfWeek)
								{
									++CurrentMonthWeek;
								}
							}
						}
						else
						{
							Appointment.DueDate = Appointment.DueDate.AddMonths(Appointment.AppointmentMonthSelection);
							Appointment.DueDate.AddMonths(1);
							Appointment.DueDate = Appointment.DueDate.AddDays(-1.0 * (Appointment.DueDate.Day - 1));
							while (bFound == false)
							{
								if (System.Convert.ToInt32(Appointment.DueDate.DayOfWeek) == Appointment.AppointmentDayOfTheWeek)
								{
									bFound = true;
									break;
								}

								Appointment.DueDate = Appointment.DueDate.AddDays(1.0);
							}
						}
						if (Appointment.DueDate < EndDate)
						{
							iDaysToSubtract = 0;
							// check if this day falls on a weekend and use weekend is selected
							iDaysToSubtract = CheckAppointmentIsOnAWeekendOrHoliday(security,
																					Appointment.DueDate,
																					Appointment.ScheduleOnWeekends,
																					Appointment.ScheduleOnHolidays,
																					HolidayScheduleCollection);

							LastDateTime = Appointment.DueDate;
							Appointment.DueDate = Appointment.DueDate.AddDays(iDaysToSubtract);
							if (LastAssignedDateTime != Appointment.DueDate &&
								Appointment.DueDate >= StartDate)
							{
								LastAssignedDateTime = Appointment.DueDate;
								AppointmentCollection.Add(Appointment);
								Appointment = new AppointmentClass();
								Appointment.Load(Set);
							}
							Appointment.DueDate = LastDateTime;
						}
					}
				}
				else
					return;
			}

		}

		private int CheckAppointmentIsOnAWeekendOrHoliday(SecurityClass security,
														DateTimeOffset DueDate,
														bool UseWeekends,
														bool UseHolidays,
														ScheduleCollectionClass HolidayScheduleCollection)
		{
			int ReturnValue = 0;
			DateTimeOffset LocalDueDate = DueDate;

			// set the local due date to midnight since we only care about the day and the ToOADate() will round up if the time is after noon
			LocalDueDate = LocalDueDate.AddHours(-LocalDueDate.Hour);
			LocalDueDate = LocalDueDate.AddMinutes(-LocalDueDate.Minute);
			LocalDueDate = LocalDueDate.AddSeconds(-LocalDueDate.Second);

		ReCheckValues:
			if (UseWeekends == false)
			{
				// if the appointment falls on a week end set it to the previous friday
				DayOfWeek Day = LocalDueDate.DayOfWeek;
				if (Day == DayOfWeek.Sunday)
				{
					// subtract two days
					ReturnValue += -2;
					LocalDueDate = LocalDueDate.AddDays(-2);

				}
				else if (Day == DayOfWeek.Saturday)
				{
					// subtract 1 day
					ReturnValue += -1;
					LocalDueDate = LocalDueDate.AddDays(-1);

				}
			}

			if (UseHolidays == false)
			{
				DateTimeOffset AppointmentDay = LocalDueDate;

				foreach (ScheduleClass HolidaySchedule in HolidayScheduleCollection)
				{
					if (HolidaySchedule.Enabled == true)
					{
						if (HolidaySchedule.HolidayDate == AppointmentDay)
						{
							ReturnValue += -1;
							LocalDueDate = LocalDueDate.AddDays(-1.0);
							goto ReCheckValues;
						}
					}
				}
			}
			return ReturnValue;
		}

		public DateTimeOffset GetNextQCDate(SecurityClass security, Guid typeGuid, Guid testSetDefinitionGuid, string assetType, DateTimeOffset QCDate)
		{
			// this routine returns the next qc date with the supplied test set removed if it is for the current qc date

			if (security == null)
				throw new ArgumentNullException("Security");

			// AssetType = datadictionary string for Tanks,Equipment or personnel
			var minQCDateToCheck = QCDate;

			var appointmentCollection = new AppointmentCollectionClass();
			var appointment = new AppointmentClass();

			// make sure the supplied QCDate is at midnight for the selected day
			QCDate = TimeConverter.ToStartOfDay(QCDate);
			// the passed in value is the last qc due date so we need to increment by one day
			QCDate = QCDate.AddDays(1);

			DateTimeOffset calculatedDateTimeOffset = QCDate;

			// Modified returned date to be tomorrow instead of yesterday. This fixes bug #7534.
			DateTimeOffset returnedDateTimeOffset = QCDate.AddDays(1);

			DataSet set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				appointment.EnumerateEquipmentQCItems(cmd, security, ContextUtil.IsInTransaction, typeGuid, assetType);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DateTimeOffset? siteTimeNow = null;

			//need to get the earliest date incase there is another appointment that would happen before the one just entered
			foreach (DataRow dr in set.Tables[0].Rows)
			{
				if (testSetDefinitionGuid != Guid.Empty)
				{
					if (((dr.IsNull("TestSetDefintionGuid")) ? Guid.Empty : (Guid) dr["TestSetDefinitionGuid"]) != testSetDefinitionGuid)
					{
						continue; //this breaks out if we only want the specifed test and the current record is not for that testset
					}
				}

				if (((string) dr["AppointmentCategory"]).ToUpper().Equals("MAINTENANCE"))
				{
					if (false == siteTimeNow.HasValue)
					{
						//only get the site time once because we need it later on
						var sites = new SitesClass();
						var site = sites.Get(security, security.SiteGuid, false);
						var timeConverter = new SiteTimeConverter(site);
						siteTimeNow = timeConverter.Today().AddDays(1); //use tomorrow for maintenance items
					}

					continue; //maintenance are always checked from current date and time
				}

				if ((DateTimeOffset) dr["StartDate"] < minQCDateToCheck)
				{
					minQCDateToCheck = (DateTimeOffset) dr["StartDate"];
				}
			}

			minQCDateToCheck = minQCDateToCheck.Date;
			calculatedDateTimeOffset = minQCDateToCheck;
			returnedDateTimeOffset = minQCDateToCheck;

			IncrementDateandTryAgain:

			bool validDateTimeCalculationPosible = false;
			calculatedDateTimeOffset = calculatedDateTimeOffset.AddYears(7);
			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return returnedDateTimeOffset;
			}

			while (table.Rows.Count != 0)
			{
				appointment = new AppointmentClass();
				appointment.Load(set);

				if (testSetDefinitionGuid == Guid.Empty
				|| appointment.TestSetDefinitionGuid == testSetDefinitionGuid) //check to see if we want to process this testset
				{

					if (appointment.AppointmentIsSingle == true && DateTimeOffset.Parse(appointment.StartDate) < QCDate)
					{
					}
					else
					{
						if (appointment.AppointmentCategory.ToUpper().Equals("MAINTENANCE"))
						{
							//maintenance always needs from today forward since the date never gets updated
							CreateDataRecordsBasedOnStartAndStopTimes(security, appointmentCollection, appointment, (DateTimeOffset) siteTimeNow, calculatedDateTimeOffset, set, true);
						}

						else
						// calculate the scheduled times for this object based on the start, stop and object parameters
						{
							this.CreateDataRecordsBasedOnStartAndStopTimes(
							security, appointmentCollection, appointment, QCDate, calculatedDateTimeOffset, set, true);
						}
						validDateTimeCalculationPosible = true;
					}
				}

				table.Rows.RemoveAt(0);
			}

			// if it is not posible to calculate the next time just return qc date - 1 day
			if (validDateTimeCalculationPosible == false)
			{
				return returnedDateTimeOffset;
			}

			// make sure we do not include the test set passed in
			RecheckCollection:

			foreach (AppointmentClass appointmentData in appointmentCollection)
			{
				// we only need to check the month day and year
				if (appointmentData.TestSetDefinitionGuid == testSetDefinitionGuid &&
					appointmentData.DueDate.Month == QCDate.Month &&
					appointmentData.DueDate.Day == QCDate.Day &&
					appointmentData.DueDate.Year == QCDate.Year)
				{
					appointmentCollection.Remove(appointmentData);
					goto RecheckCollection;
				}
			}

			// check the returned collection to see if we got anything
			if (appointmentCollection.Count == 0)
			{
				goto IncrementDateandTryAgain;
			}

			// set the returned date equal to the max date sent to the database enquiry
			returnedDateTimeOffset = calculatedDateTimeOffset;

			foreach (AppointmentClass appointmentData in appointmentCollection)
			{
				if (appointmentData.DueDate < returnedDateTimeOffset)
				{
					returnedDateTimeOffset = appointmentData.DueDate;
				}
			}

			return returnedDateTimeOffset;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateAppointmentBasedOnDueDate(SecurityClass security, Guid testSetDefinitionGuid, Guid entityGuid, bool bEquipment, DateTimeOffset dueDate)
		{
			// Check parameters
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dueDate == null)
			{
				throw new ArgumentNullException("dueDate");
			}

			// Look for an appointment
			AppointmentCollectionClass appointments = EnumerateBasedOnTestSetAndEntity(security, testSetDefinitionGuid, bEquipment, entityGuid);

			// Update the appointment if we find one
			if (appointments.Count > 0)
			{
				AppointmentClass appointment = appointments[0];
				appointment.StartDate = dueDate.ToString("d");

				Modify(security, appointment);
			}
		}

		public AppointmentCollectionClass EnumerateBasedOnTestSetAndEntity(SecurityClass security, Guid testSetDefinitionGuid, bool bEquipment, Guid equipmentGuid)
		{
			// Check parameters
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			AppointmentClass appointment = new AppointmentClass();
			AppointmentCollectionClass appointmentCollection = new AppointmentCollectionClass();

			DataSet set = null;
			if (bEquipment)
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					appointment.EnumerateBasedOnTestSetAndEquipmentSQL(cmd, security, testSetDefinitionGuid, equipmentGuid, ContextUtil.IsInTransaction);
					set = ConsolidatedDA.GetDataSet(cmd, security);
				}
			}
			else
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					appointment.EnumerateBasedOnTestSetAndTankSQL(cmd, security, testSetDefinitionGuid, equipmentGuid, ContextUtil.IsInTransaction);
					set = ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				appointment = new AppointmentClass();
				appointment.Load(set);
				appointment.DueDate = appointment.StartDateObject.Value;
				appointmentCollection.Add(appointment);
				table.Rows.RemoveAt(0);
			}

			return appointmentCollection;

		}

		public DateTimeOffset GetQCDateForTestSet(SecurityClass security, Guid equipmentGuid, Guid testSetDefinitionGuid, string assetType, DateTimeOffset startDate, DateTimeOffset endDate)
		{
			// this routine returns the next date for the supplied test set guid
			// AssetType = datadictionary string for Tanks,Equipment or personnel
			DateTimeOffset ReturnedDateTime;

			if (security == null)
				throw new ArgumentNullException("Security");

			AppointmentCollectionClass AppointmentCollection = new AppointmentCollectionClass();
			AppointmentClass Appointment = new AppointmentClass();

			// make sure the supplied dates are at midnight for the selected day
			endDate = TimeConverter.ToStartOfDay(endDate);

			startDate = TimeConverter.ToStartOfDay(startDate);

			ReturnedDateTime = startDate.AddDays(-1);

			DataSet Set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.EnumerateEquipmentQCItems(cmd, security, ContextUtil.IsInTransaction, equipmentGuid, assetType);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
				return ReturnedDateTime;

			while (Table.Rows.Count != 0)
			{
				Appointment = new AppointmentClass();
				Appointment.Load(Set);
				if (Appointment.TestSetDefinitionGuid == testSetDefinitionGuid)
				{
					// calculate the scheduled times for this object based on the start, stop and object parameters
					CreateDataRecordsBasedOnStartAndStopTimes(
						security, AppointmentCollection, Appointment, startDate, endDate, Set, true);
				}

				Table.Rows.RemoveAt(0);
			}

			// check the returned collection to see if we got anything
			if (AppointmentCollection.Count == 0)
				return ReturnedDateTime;

			// set the returned date equal to the max date sent to the database enquiry
			ReturnedDateTime = endDate;

			foreach (AppointmentClass AppointmentData in AppointmentCollection)
			{
				if (AppointmentData.DueDate < ReturnedDateTime)
					ReturnedDateTime = AppointmentData.DueDate;
			}

			return ReturnedDateTime;
		}

		public DateTimeOffset GetNextQCDateForAsset(SecurityClass security, Guid equipmentGuid, string assetType, DateTimeOffset startDate)
		{
			// this routine returns next date for any qc scheduled
			// AssetType = datadictionary string for Tanks,Equipment or personnel
			DateTimeOffset CalculatedDateTime;
			DateTimeOffset ReturnedDateTime;
			bool ValidDateTimeCalculationPosible = false;

			if (security == null)
				throw new ArgumentNullException("Security");

			AppointmentCollectionClass AppointmentCollection = new AppointmentCollectionClass();
			AppointmentClass Appointment = new AppointmentClass();

			// make sure the supplied QCDate is at midnight for the selected day
			startDate = TimeConverter.ToStartOfDay(startDate);
			// the passed in value in the last qc due date so we need to increment by one day
			startDate = startDate.AddDays(1);

			CalculatedDateTime = startDate;

			ReturnedDateTime = startDate.AddDays(-1);

			DataSet Set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Appointment.EnumerateEquipmentQCItems(cmd, security, ContextUtil.IsInTransaction, equipmentGuid, assetType);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DateTimeOffset? siteTimeNow = null;

			//need to get the earliest date incase there is another appointment that would happen before the one just entered
			foreach (DataRow dr in Set.Tables[0].Rows)
			{
				if (((string)dr["AppointmentCategory"]).ToUpper().Equals("MAINTENANCE"))
				{
					if (false == siteTimeNow.HasValue)
					{
						//only get the site time once because we need it later on
						var sites = new SitesClass();
						var site = sites.Get(security, security.SiteGuid, false);
						var timeConverter = new SiteTimeConverter(site);
						siteTimeNow = timeConverter.Today().AddDays(1); //use tomorrow for maintenance items
					}

					continue; //maintenance are always checked from current date and time
				}
			}



			ValidDateTimeCalculationPosible = false;
			CalculatedDateTime = CalculatedDateTime.AddYears(7);
			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
				return ReturnedDateTime;

			while (Table.Rows.Count != 0)
			{
				Appointment = new AppointmentClass();
				Appointment.Load(Set);
				if (Appointment.AppointmentIsSingle == true &&
					DateTimeOffset.Parse(Appointment.StartDate) < startDate)
				{
				}
				else if (Appointment.TestSetDefinitionGuid == Guid.Empty)
				{
				}
				else
				{
					if (Appointment.AppointmentCategory.ToUpper().Equals("MAINTENANCE"))
					{
						//maintenance always needs from today forward since the date never gets updated
						CreateDataRecordsBasedOnStartAndStopTimes(security, AppointmentCollection, Appointment, (DateTimeOffset) siteTimeNow, CalculatedDateTime, Set, true);
					}
					else
					{
						// calculate the scheduled times for this object based on the start, stop and object parameters
						CreateDataRecordsBasedOnStartAndStopTimes(security, AppointmentCollection, Appointment, startDate, CalculatedDateTime, Set, true);
					}
					ValidDateTimeCalculationPosible = true;
				}

				Table.Rows.RemoveAt(0);
			}

			// if it is not posible to calculate the next time just return qc date - 1 day
			if (ValidDateTimeCalculationPosible == false ||
				AppointmentCollection.Count == 0)
				return ReturnedDateTime;

			// set the returned date equal to the max date sent to the database enquiry
			ReturnedDateTime = CalculatedDateTime;

			foreach (AppointmentClass AppointmentData in AppointmentCollection)
			{
				if (AppointmentData.DueDate < ReturnedDateTime)
					ReturnedDateTime = AppointmentData.DueDate;
			}

			return ReturnedDateTime;
		}
	}
}
