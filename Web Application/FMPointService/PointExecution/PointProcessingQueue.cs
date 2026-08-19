namespace FMPointService.PointExecution
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;


	internal class PointProcessingQueue 
	{
		private readonly ConcurrentQueue<Guid> queue;

		public int Count
		{
			get
			{
				return this.queue.Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.queue.IsEmpty;
			}
		}

		public PointProcessingQueue()
		{
			this.queue = new ConcurrentQueue<Guid>();
		}

		public void QueuePointForProcessing(Guid pointGuid)
		{
			this.queue.Enqueue(pointGuid);
			PointProcessingTask.SignalExpedite();
		}

		public bool TryDequeueItem(out Guid pointGuid)
		{
			return this.queue.TryDequeue(out pointGuid);
		}

		public List<Guid> DequeueAll()
		{
			var pointGuids = new List<Guid>(this.queue.Count);
			Guid pointGuid;

			while (this.TryDequeueItem(out pointGuid))
			{
				pointGuids.Add(pointGuid);
			}

			return pointGuids;
		}
	}
}
