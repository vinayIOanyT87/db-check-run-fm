
namespace FMBusinessObjects.ServiceRequests
{
	using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
    /// Used to communicate information required for a login request
    /// </summary>
    [DataContract]
    public class SecurityLoginResponse
    {
        [DataMember]
		public int NumberOfFailedAttempts { get; set; }

		[DataMember]
		public System.DateTimeOffset LastLoginDateAndTime { get; set; }

		[DataMember]
		public string Result { get; set; }
		
        [DataMember]
        public int TimeOut { get; set; }

		[DataMember]
		public bool ChangePassword { get; set; }
		
		[DataMember]
		public int DaysUntilExpiration { get; set; }

		[DataMember]
		public SecurityClass Security { get; set; }

		[DataMember]
		public bool AdminMustChangePasswordAtWebServer { get; set; }
	}
}
