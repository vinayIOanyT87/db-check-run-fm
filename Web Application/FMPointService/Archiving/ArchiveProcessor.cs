namespace FMPointService.Archiving
{
	using System.Threading;
	using System.Threading.Tasks;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	internal class ArchiveProcessor
	{
		public ArchiveProcessorSignaler ArchiveProcessorSignaler = new ArchiveProcessorSignaler();

		public ArchiveProcessingTask ArchiveProcessingTask = new ArchiveProcessingTask();

		private CancellationTokenSource cancellationSource;

		// ReSharper disable once NotAccessedField.Local
		private Task processingTask;

		public void Start(SecurityClass security)
		{
			security.ThrowIfNull("security");

			this.cancellationSource = new CancellationTokenSource();

			var factory = new TaskFactory(this.cancellationSource.Token);

			this.processingTask = factory.StartNew(
				() => this.ArchiveProcessingTask.ProcessingAction(this.cancellationSource.Token, security),
				this.cancellationSource.Token,
				TaskCreationOptions.LongRunning,
				TaskScheduler.Default);
		}

		public void Stop()
		{
			if (this.cancellationSource != null)
			{
				this.cancellationSource.Cancel();
				this.cancellationSource = null;
			}
			this.processingTask = null;
		}

		public void SignalExpedite()
		{
			this.ArchiveProcessorSignaler.SignalExpedite();
		}
	}
}
