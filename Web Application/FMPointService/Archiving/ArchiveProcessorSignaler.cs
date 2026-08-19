namespace FMPointService.Archiving
{

	internal class ArchiveProcessorSignaler 
	{
		public void SignalExpedite()
		{
		    ArchiveProcessingTask.SignalExpedite();
		}
	}
}
