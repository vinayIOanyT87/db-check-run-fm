namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]

	public class PointValueAccess
	{
		[DataMember]
		public bool View { get; set; }
		[DataMember]
		public bool Modify { get; set; }
		[DataMember]
		public bool ExceedRange { get; set; }
		[DataMember]
		public bool Override { get; set; }

		public PointValueAccess()
		{
			View = true;
			Modify = true;
			ExceedRange = true;
			Override = true;
		}
	}
}
