namespace TransactionFields
{
	using System;

	[Serializable()]
	public class FMFieldRequiredException : Exception
	{
		public FMFieldRequiredException() : base("Required field missing.")
		{
		}
	}
}
