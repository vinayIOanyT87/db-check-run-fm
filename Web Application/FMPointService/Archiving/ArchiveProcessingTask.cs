namespace FMPointService.Archiving
{
	using System;
	using System.Threading;

	using FMBusinessObjects.DataObjects;

	using ThreadSupport;
	using Logging;

	internal class ArchiveProcessingTask 
	{
		private enum ProcessorEvents
		{
				Cancellation = 0,
				Timeout = WaitHandle.WaitTimeout
		}

		public ArchiveInitializer ArchiveInitializer = new ArchiveInitializer();

		public ArchiveManager ArchiveManager = new ArchiveManager();

		public EventLogger EventLogger = new EventLogger();

		/// <summary>
		/// Set to signal the point processor to expedite processing.
		/// </summary>
		protected static readonly AutoResetEvent ExpediteProcessingEvent = new AutoResetEvent(false);

		private static readonly TimeSpan OneSecondWait = new TimeSpan(0, 0, 1);

		private static readonly TimeSpan ThirtySecondWait = new TimeSpan(0, 0, 30);

		private static bool ArchiveProcessInitialized = false;

		/// <summary>
		/// The main point processing scan loop (action).
		/// </summary>
		public void ProcessingAction(CancellationToken cancellationToken, SecurityClass security)
		{
			WaitHandle[] events1 = { cancellationToken.WaitHandle, ExpediteProcessingEvent };
			WaitHandle[] events2 = { cancellationToken.WaitHandle };
			var events = events1;

			var timeout = OneSecondWait;
			ArchiveProcessInitialized = false;
			int lastDayAllArchiveDataSent = DateTime.Now.Day;

			while (true)
			{
				try
				{
					if (ArchiveProcessInitialized == false) // archive process not initialized
					{
						ArchiveProcessInitialized = this.Initialize(security);
						if (ArchiveProcessInitialized == false)
						{
							timeout = ThirtySecondWait;
							events = events2;
						}
						else
						{
							timeout = OneSecondWait;
							events = events1;
						}
					}

					var eventThatSignaled = (ProcessorEvents)WaitHandle.WaitAny(events, timeout);

					if (eventThatSignaled == ProcessorEvents.Cancellation)
					{
						if (ArchiveProcessInitialized)
						{
							this.ArchiveManager.ProcessArchiveQueue(security);
						}

						break;
					}

					if (ArchiveProcessInitialized)
					{
						this.ArchiveManager.ProcessArchiveQueue(security);

						if (ThreadSharedData.Instance().EnableArchiveData)
						{
							// if this is midnight UTC archive all of the points
							DateTime currentUtctimeNow = DateTime.UtcNow;
							if (currentUtctimeNow.Day != lastDayAllArchiveDataSent)
							{
								lastDayAllArchiveDataSent = currentUtctimeNow.Day;
								ThreadSharedData.Instance().ArchiveAllPoints();
							}
						}


						timeout = OneSecondWait;
						events = events1;
					}
				}
				catch (Exception except)
				{
					this.EventLogger.Error("ArchiveProcessingTask: " + except.Message);
					timeout = ThirtySecondWait;
					events = events2;
				}
			}
		}

		private bool Initialize(SecurityClass security)
		{
			try
			{
				this.ArchiveInitializer.Initialize(security);
			}
			catch (Exception except)
			{
				// Archive Initialization can also occur during Synchronization and AddArchiveData.  A timeout here indicates
				// another thread in FMBusinessServices is processing and the archive has been intialized.
				if (!except.Message.Contains("timeout acquiring write lock"))
				{
					this.EventLogger.Error("Archive Task Initialize: " + except);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Signals the archive processing task to wake up and start processing before the next
		/// scheduled period timeout.
		/// </summary>
		public static void SignalExpedite()
		{
			if (ThreadSharedData.Instance().EnableArchiveData)
			{
				ExpediteProcessingEvent.Set();
			}
		}
	}
}
