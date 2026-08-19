namespace FMBusinessObjects.Exceptions
{
	using System;

	public class ExStarsSegmentException : ApplicationException
	{
		public ExStarsSegmentException(string msg)
			: base(msg)
		{
		}
		public ExStarsSegmentException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public ExStarsSegmentException(Exception innerException, string fmt, params object[] args)
			: base(string.Format(fmt, args), innerException)
		{
		}

		public ExStarsSegmentException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}


	}
}
