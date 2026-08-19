namespace FMBusinessObjects.Exceptions
{
    using System;

    public class SyncCommunicationException : ApplicationException
	{
		public SyncCommunicationException(string message, Exception expIn) : base(message, expIn)
		{
			
		}
	}
}
