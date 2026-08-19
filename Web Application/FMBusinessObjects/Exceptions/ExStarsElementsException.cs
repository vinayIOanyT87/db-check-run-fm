namespace FMBusinessObjects.Exceptions
{
	using System;

	public class ExStarsElementsException : ApplicationException
	{
		public ExStarsElementsException(string msg)
			: base(msg)
		{
		}

		public ExStarsElementsException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}

		public ExStarsElementsException(Exception innerException, string fmt, params object[] args)
			: base(string.Format(fmt, args), innerException)
		{
		}
	}

}
