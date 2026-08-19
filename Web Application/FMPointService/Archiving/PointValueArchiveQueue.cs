namespace FMPointService.Archiving
{
	using System.Collections.Concurrent;

	using FMBusinessObjects.DataObjects;


	/// <summary>
	/// This class implements the point tag archive queue interface for queuing archive records.
	/// </summary>
	public class PointValueArchiveQueue 
	{
		private readonly ConcurrentQueue<ArchiveDataElement> queue;

		public bool IsEmpty
		{
			get
			{
				return this.queue.IsEmpty;
			}
		}

		public int Count
		{
			get
			{
				return this.queue.Count;
			}
		}

		public PointValueArchiveQueue()
		{
			this.queue = new ConcurrentQueue<ArchiveDataElement>();
		}

		public void QueueItemForArchiving( ArchiveDataElement item )
		{
			this.queue.Enqueue( item );
		}

		public bool TryDequeueItem(out ArchiveDataElement item)
		{
			return this.queue.TryDequeue(out item);
		}
	}
}
