using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FuelsManagerService.Scheduler;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FuelsManagerService
{
	internal static class FMScheduler
	{
		#region Constants and Fields

		/// <summary>
		/// Stops processing
		/// </summary>
		private static readonly ManualResetEvent KillEvent = new ManualResetEvent(false);

		/// <summary>
		/// The thread responsible for processing
		/// </summary>
		private static Thread processThread = null;
		private static IScheduler scheduler;
		private static EventLog eventLog = new EventLog("Application", ".", "FM Scheduler");

		#endregion

		/// <summary>
		/// Starts execution of the ProcessThread.
		/// </summary>
		/// <param name="security">
		/// Contains Security Information.
		/// </param>
		internal static void StartProcessThread(SecurityClass security)
		{
			processThread = new Thread(() => ProcessScan(security));
			processThread.Start();
		}

		/// <summary>
		///     Stops the ProcessThread.
		/// </summary>
		internal static void StopProcessThread()
		{
			if (scheduler != null)
			{
				eventLog.WriteEntry($"Scheduler ended", EventLogEntryType.Information);
				scheduler.Shutdown();
			}
			KillEvent.Set();

			if (processThread != null)
			{
				processThread.Join();
			}
		}

		/// <summary>
		/// Processes the scan.
		/// </summary>
		/// <param name="security">The security.</param>
		private static async void ProcessScan(SecurityClass security)
		{
			eventLog.WriteEntry($"Scheduler started", EventLogEntryType.Information);

			WaitHandle[] events = { KillEvent };
			DateTime start = DateTime.Now;
			TimeSpan tenMinutes = TimeSpan.FromMinutes(10);
			int waitInterval = 60000;

			// construct a scheduler factory
			StdSchedulerFactory factory = new StdSchedulerFactory();

			// get a scheduler
			scheduler = await factory.GetScheduler();
			await scheduler.Start();
			scheduler.Context.Add("security", security);

			PointGroupScheduleCollection schedules = new PointGroupScheduleCollection();



			while (0 != WaitHandle.WaitAny(events, waitInterval, true))
			{
				try
				{

					var newSchedules = FMChannelHelper.MakeCall<IPointGroupSchedules, PointGroupScheduleCollection>(x => x.EnumerateAll(security));
					foreach( var newSchedule in newSchedules)
					{
						var existingSchedule = schedules.FirstOrDefault(x => x.PointGroupScheduleGuid == newSchedule.PointGroupScheduleGuid);

						// if new schedule
						if (existingSchedule == null)
						{
							IJobDetail pointGroupReportJob = JobBuilder.Create<PrintPointGroupJob>()
								.UsingJobData("pointGroupGuid", newSchedule.PointGroupGuid.ToString())
								.UsingJobData("siteGuid", newSchedule.SiteGuid.ToString())
								.UsingJobData("userGuid", newSchedule.UserGuid.ToString())
								.UsingJobData("pointGroupScheduleGuid", newSchedule.PointGroupScheduleGuid.ToString())
								.WithIdentity(newSchedule.PointGroupScheduleGuid.ToString(), "pointgroupreportgroup")
								.Build();
							// single fire jobs
							if (newSchedule.CronSchedule == "* * * * *")
							{
								// if job is expired then don't do anything
								if (newSchedule.StartSchedule < DateTime.Now) 
								{

								} else {
									// Trigger the job to run on the next round minute
									ITrigger trigger = TriggerBuilder.Create()
										 .WithIdentity(newSchedule.PointGroupScheduleGuid.ToString(), "pointgroupreportgroup")
										 .StartAt(new DateTimeOffset(newSchedule.StartSchedule,
												  TimeZoneInfo.Local.GetUtcOffset(newSchedule.StartSchedule)))
										 .WithSimpleSchedule(x => x.WithRepeatCount(0).WithIntervalInMinutes(1).WithMisfireHandlingInstructionNextWithRemainingCount())
										 .Build();

								// Tell quartz to schedule the job using our trigger
								await scheduler.ScheduleJob(pointGroupReportJob, trigger);

								}
							} else {
								// Trigger the job to run on the next round minute
								TriggerBuilder triggerbld = TriggerBuilder.Create()
									 .WithIdentity(newSchedule.PointGroupScheduleGuid.ToString(), "pointgroupreportgroup")
									 .StartAt(new DateTimeOffset(newSchedule.StartSchedule,
												  TimeZoneInfo.Local.GetUtcOffset(newSchedule.StartSchedule)))
									.WithCronSchedule(newSchedule.CronSchedule, x => x.WithMisfireHandlingInstructionDoNothing());

								if (newSchedule.EndSchedule.StartsWith("d"))
								{
									var endDatestr = newSchedule.EndSchedule.Replace("d ", "");
									triggerbld.EndAt(DateTimeOffset.Parse(endDatestr));
								}

								ITrigger trigger = triggerbld.Build();

								// Tell quartz to schedule the job using our trigger
								await scheduler.ScheduleJob(pointGroupReportJob, trigger);
							}

								schedules.Add(newSchedule);

						} else {
							// if schedule has been updated we need to update the trigger
							if( newSchedule.StartSchedule != existingSchedule.StartSchedule ||
								newSchedule.CronSchedule != existingSchedule.CronSchedule ||
								newSchedule.EndSchedule != existingSchedule.EndSchedule)
							{

								// retrieve the trigger
								var oldTrigger = await scheduler.GetTrigger(new TriggerKey(newSchedule.PointGroupScheduleGuid.ToString(), "pointgroupreportgroup"));
								if (oldTrigger != null ){
									TriggerBuilder tb = oldTrigger.GetTriggerBuilder();
									// obtain a builder that would produce the trigger
									if (newSchedule.CronSchedule == "* * * * *")
									{
										// update the schedule associated with the builder, and build the new trigger
										ITrigger newTrigger = tb.StartAt(new DateTimeOffset(newSchedule.StartSchedule,
												  TimeZoneInfo.Local.GetUtcOffset(newSchedule.StartSchedule)))
												  .WithSimpleSchedule( x => x.WithRepeatCount(0).WithIntervalInMinutes(1).WithMisfireHandlingInstructionNextWithRemainingCount())
											.Build();
										var test = await scheduler.RescheduleJob(oldTrigger.Key, newTrigger);

									}
									else
									{
										tb.StartAt(new DateTimeOffset(newSchedule.StartSchedule,
												  TimeZoneInfo.Local.GetUtcOffset(newSchedule.StartSchedule)));
										if (newSchedule.EndSchedule.StartsWith("d"))
										{
											var endDatestr = newSchedule.EndSchedule.Replace("d ", "");
											tb.EndAt(DateTimeOffset.Parse(endDatestr));
										} else
										{
											tb.EndAt(null);
										}

										ITrigger newTrigger = tb.WithCronSchedule(newSchedule.CronSchedule, x =>x.WithMisfireHandlingInstructionDoNothing())
												.Build();
										var test =  scheduler.RescheduleJob(oldTrigger.Key, newTrigger);

									}

								} else {
									IJobDetail pointGroupReportJob = JobBuilder.Create<PrintPointGroupJob>()
													.UsingJobData("pointGroupGuid", newSchedule.PointGroupGuid.ToString())
													.UsingJobData("siteGuid", newSchedule.SiteGuid.ToString())
													.UsingJobData("userGuid", newSchedule.UserGuid.ToString())
													.UsingJobData("pointGroupScheduleGuid", newSchedule.PointGroupScheduleGuid.ToString())
													.WithIdentity(newSchedule.PointGroupScheduleGuid.ToString(), "pointgroupreportgroup")
													.Build();


									// single fire jobs
									if (newSchedule.CronSchedule == "* * * * *")
									{
										// if job is expired then don't do anything
										if (newSchedule.StartSchedule < DateTime.Now)
										{

										}
										else
										{
											// Trigger the job to run on the next round minute
											TriggerBuilder triggerbld = TriggerBuilder.Create()
												 .WithIdentity(newSchedule.PointGroupScheduleGuid.ToString(), "pointgroupreportgroup")
												 .StartAt(new DateTimeOffset(newSchedule.StartSchedule,
														  TimeZoneInfo.Local.GetUtcOffset(newSchedule.StartSchedule)))
												.WithSimpleSchedule(x => x.WithRepeatCount(0).WithIntervalInMinutes(1).WithMisfireHandlingInstructionNextWithRemainingCount());
											ITrigger trigger = triggerbld.Build();

											// Tell quartz to schedule the job using our trigger
											await scheduler.ScheduleJob(pointGroupReportJob, trigger);

										}
									}
									else
									{
										// Trigger the job to run on the next round minute
										TriggerBuilder triggerbld = TriggerBuilder.Create()
											 .WithIdentity(newSchedule.PointGroupScheduleGuid.ToString(), "pointgroupreportgroup")
											.StartAt(new DateTimeOffset(newSchedule.StartSchedule,
												  TimeZoneInfo.Local.GetUtcOffset(newSchedule.StartSchedule)))
											.WithCronSchedule(newSchedule.CronSchedule, x => x.WithMisfireHandlingInstructionDoNothing());

										if (newSchedule.EndSchedule.StartsWith("d"))
										{
											var endDatestr = newSchedule.EndSchedule.Replace("d ", "");
											triggerbld.EndAt( DateTimeOffset.Parse(endDatestr));
										}

										ITrigger trigger = triggerbld.Build();

										// Tell quartz to schedule the job using our trigger
										await scheduler.ScheduleJob(pointGroupReportJob, trigger);
									}

								}

								existingSchedule.StartSchedule = newSchedule.StartSchedule;
								existingSchedule.CronSchedule = newSchedule.CronSchedule;
								existingSchedule.EndSchedule = newSchedule.EndSchedule;
							}

						}

					}

					// check for any existing schedule that is not in the database because it has been deleted
					var allJobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
					foreach (var jobKey in allJobKeys)
					{
						IJobDetail jobDetail = await scheduler.GetJobDetail(jobKey);
						if( newSchedules.Any( x => x.PointGroupScheduleGuid.ToString() == jobDetail.Key.Name) == false)
						{
							await scheduler.DeleteJob(jobKey);
						}
					}

				}
				catch (Exception ex)
				{
					FuelsManagerServiceLogger.Instance.LogError(ex);
				}
			}
		}

	}
}
