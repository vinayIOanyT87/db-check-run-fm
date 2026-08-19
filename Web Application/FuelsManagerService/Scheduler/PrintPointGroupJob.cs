using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FuelsManagerService.PointGroupReport;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManagerService.Scheduler
{
	class PrintPointGroupJob : IJob
	{
		private static EventLog eventLog = new EventLog("Application", ".", "FM Scheduler");

		public virtual Task Execute(IJobExecutionContext context)
		{
			var JobStartTime = DateTime.Now;
			var pointgroupname = "";
			var pointgroupguid = "";
			var userid = "";
			try
			{
				JobKey key = context.JobDetail.Key;
				JobDataMap dataMap = context.JobDetail.JobDataMap;

				var startTime = DateTime.Now.ToString("r");
				eventLog.WriteEntry($"Executing scheduler - {startTime} : Schedule Guid  - {dataMap.GetString("pointGroupScheduleGuid")}", EventLogEntryType.Information);

				SchedulerContext schedulerContext = schedulerContext = context.Scheduler.Context;
				SecurityClass security = (SecurityClass)schedulerContext.Get("security");

				// write event to the alarm and event log
				security.LoginSiteGuid = new Guid(dataMap.GetString("siteGuid"));
				security.SiteGuid = new Guid(dataMap.GetString("siteGuid"));

				var schedule = FMChannelHelper.MakeCall<IPointGroupSchedules, PointGroupSchedule>(x => x.GetByPK(security, new Guid(dataMap.GetString("pointGroupScheduleGuid"))));

				if (schedule == null || schedule.PointGroupScheduleGuid == Guid.Empty)
				{
					return Task.CompletedTask; ; // schedule no longer valid
				}

				var pointGroup = FMChannelHelper.MakeCall<IPointGroups, PointGroup>(x => x.Get(security, 
																							new Guid(dataMap.GetString("pointGroupGuid")),
																							new Guid(dataMap.GetString("userGuid")),
																							security.SiteGuid));
				if (pointGroup == null || pointGroup.PointGroupGuid == Guid.Empty )
				{
					return Task.CompletedTask; ; // point group no longer valid
				}

				pointgroupname = pointGroup.ID;
				pointgroupguid = pointGroup.PointGroupGuid.ToString();

				Trace.WriteLine("Retrieving User", "Point Group Report Processing");
				var user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, new Guid(dataMap.GetString("userGuid"))));
				if (user == null || user.Deleted || user.InactivityLockout)
				{
					return Task.CompletedTask; // user no longer valid
				}

				userid = user.ID;

				AlarmAndEventLogClass eventNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportStartingEventDescriptor);
				eventNotification.AssociatedData = $"Executing scheduler - {startTime} - Schedule - {dataMap.GetString("pointGroupScheduleGuid")}, point group - { pointgroupname }, user - { userid } ";
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add( security, eventNotification) );

				try
				{
					Generator.Process(schedule.PointGroupScheduleGuid, DateTime.Parse(startTime));
				}
				catch (Exception e)
				{
					eventLog.WriteEntry($"Generating Report Error: Schedule - {dataMap.GetString("pointGroupScheduleGuid")}: {e.Message}", EventLogEntryType.Error);
				}
			}
			catch (Exception ex)
			{
				FuelsManagerServiceLogger.Instance.LogError(ex);
			} 
			return Task.CompletedTask;
		}
	}
}
