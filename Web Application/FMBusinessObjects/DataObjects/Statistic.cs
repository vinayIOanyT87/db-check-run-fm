namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	[DataContract]
	public sealed class Statistic
	{
		[DataMember]
		public string Name;

		[DataMember]
		public double Average;

		[DataMember]
		public int Count;

		[DataMember]
		public long Min;

		[DataMember]
		public long Max;

		[DataMember]
		public long TotalMillisconds;
	}
}
