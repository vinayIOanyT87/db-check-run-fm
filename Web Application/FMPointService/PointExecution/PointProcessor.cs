namespace FMPointService.PointExecution
{
	using System.Threading;
	using System.Threading.Tasks;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	internal class PointProcessor 
	{
		public PointProcessingTask PointProcessingTask = new PointProcessingTask();

		private CancellationTokenSource cancellationSource;

		// ReSharper disable once NotAccessedField.Local
		private Task processingTask;

		public void Start(SecurityClass security)
		{
			security.ThrowIfNull("security");

			this.cancellationSource = new CancellationTokenSource();

			var factory = new TaskFactory(this.cancellationSource.Token);

			this.processingTask = factory.StartNew(
				() => this.PointProcessingTask.PointProcessingAction(this.cancellationSource.Token, security),
				this.cancellationSource.Token,
				TaskCreationOptions.LongRunning,
				TaskScheduler.Default);
		}

		public void Stop()
		{
			this.cancellationSource.Cancel();
			this.processingTask = null;
		}
	}
}
