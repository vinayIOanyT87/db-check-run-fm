using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class FMStandingOfferException : ApplicationException
	{
		#region Public data members
		public const string ExceptionMessage = "{0}";
		#endregion

		#region Private data members
		#endregion

		#region Constructors
		/// <summary>
		/// This constructor creates a standing offer (aka price list) exception accepting an Exception.
		/// </summary>
		/// <param name="exception"></param>
		public FMStandingOfferException ( Exception exception )
			: base ( string.Format ( CultureInfo.CurrentCulture, ExceptionMessage, exception.Message ) )
		{
		}

		/// <summary>
		/// his constructor creates a standing offer (aka price list) exception accepting a string message.
		/// </summary>
		/// <param name="message"></param>
		public FMStandingOfferException ( string message )
			: base ( string.Format ( CultureInfo.CurrentCulture, ExceptionMessage, message ) )
		{
		}

		public FMStandingOfferException ( SerializationInfo info, StreamingContext context )
			: base ( info, context )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return true if the application is to continue
		/// process. Otherwise, it will return false.
		/// </summary>
		public bool ContinueOn { get; set; }

		#endregion
	}
}
