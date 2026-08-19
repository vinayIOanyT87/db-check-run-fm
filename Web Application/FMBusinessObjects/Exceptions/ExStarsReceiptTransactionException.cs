namespace FMBusinessObjects.Exceptions
{
	using System;
	class ExStarsReceiptTransactionException : ApplicationException
	{
		public ExStarsReceiptTransactionException(string msg)
			: base(msg)
		{
		}
		public ExStarsReceiptTransactionException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public ExStarsReceiptTransactionException(Exception innerException, string fmt, params object[] args)
			: base(string.Format(fmt, args), innerException)
		{
		}

		public ExStarsReceiptTransactionException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}

	}
}
