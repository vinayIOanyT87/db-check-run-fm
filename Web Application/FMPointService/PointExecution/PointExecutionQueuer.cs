namespace FMPointService.PointExecution
{
	using System;

	using global::FMPointService.ThreadSupport;

	public class PointExecutionQueuer 
	{

		/// <summary>
		/// This method will queue the specified pointGuid for execution if
		/// the point it represents has not already been marked for calculation.
		/// </summary>
		/// <param name="pointGuid">The identity guid of the point to queue.</param>
		public void QueuePointForProcessing(Guid pointGuid)
		{
			PointExecutionQueuer.StaticQueuePointForProcessing(pointGuid);
		}

		//Put this in here so I had a static reference to this function 
		//I believe above function should have been static in the first place.
		public static void StaticQueuePointForProcessing(Guid pointGuid)
		{
			var threadSharedData = ThreadSharedData.Instance();

			// If we can mark the point for processing, queue it
			if (threadSharedData.SetPointNeedsCalculation(pointGuid))
			{
				// Add the point to the queue.  If the point was already marked for calculation
				// we will assume it was already in the queue.  If for some reason it is not
				// in the queue, it will get mopped up during periodic execution because of the
				// marked for calculation flag being set.
				PointProcessingTask.PointProcessingQueue.QueuePointForProcessing(pointGuid);
			}

		}
	}
}
