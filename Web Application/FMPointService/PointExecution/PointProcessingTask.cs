namespace FMPointService.PointExecution
{
	using System;
	using System.Threading;

	using FMBusinessObjects.DataObjects;

	using Logging;
	using System.Configuration;

	/// <summary>
	/// This class is responsible for directing the task of processing points.
	/// </summary>
	internal class PointProcessingTask 
	{
		private bool? UsePointLogicEngine = null;

		private enum ProcessorEvents
		{
			Cancellation = 0,
		}

		public EventLogger EventLogger = new EventLogger();

		public PointExecutor PointExecutor = new PointExecutor();


		/// <summary>
		/// The main point processing queue.
		/// </summary>
		internal static readonly PointProcessingQueue PointProcessingQueue = new PointProcessingQueue();

		/// <summary>
		/// Set to signal the point processor to expedite processing.
		/// </summary>
		protected static readonly AutoResetEvent ExpediteProcessingEvent = new AutoResetEvent( false );

		/// <summary>
		/// The main point processing scan loop (action).
		/// </summary>
		public void PointProcessingAction(CancellationToken cancellationToken, SecurityClass security)
		{
			WaitHandle[] events = { cancellationToken.WaitHandle, ExpediteProcessingEvent };

			this.UsePointLogicEngine = bool.Parse(ConfigurationManager.AppSettings["UsePointLogicEngine"]);

			while (true)
			{
				try
				{
					var eventThatSignaled = (ProcessorEvents)WaitHandle.WaitAny(events);

					if (eventThatSignaled == ProcessorEvents.Cancellation)
					{
						break;
					}

					this.PointExecutor.ExecutePoints(security, PointProcessingQueue.DequeueAll());
				}
				catch (Exception except)
				{
					try
					{
						this.EventLogger.Error("PointProcessingTask: " + except);
					}
					catch(Exception e)
					{
						this.EventLogger.Error(e.Message + "\n\r StackTrace: \n\r" + except.StackTrace);
					}
				}
			}

			PointProcessingQueue.DequeueAll();
		}

		/// <summary>
		/// Signals the point processing task to wake up and start processing points before the next
		/// scheduled period timeout.
		/// </summary>
		public static void SignalExpedite()
		{
			ExpediteProcessingEvent.Set();
		}
	}
}
