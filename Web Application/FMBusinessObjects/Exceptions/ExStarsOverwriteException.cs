namespace FMBusinessObjects.Exceptions
{
	using System;
	/// <summary>
	/// Data was about to be overwritten
	/// </summary>
	public class ExStarsOverwriteException : ApplicationException
	{
		public ExStarsOverwriteException(string msg)
			: base(msg)
		{
		}

		public ExStarsOverwriteException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}

		public ExStarsOverwriteException(Exception innerException, string fmt, params object[] args)
			: base(string.Format(fmt, args), innerException)
		{
		}
	}
}
