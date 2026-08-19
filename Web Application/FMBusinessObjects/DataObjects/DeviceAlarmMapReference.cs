namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class DeviceAlarmMapReference
	{
		[DataMember]
		public Guid DeviceAlarmMapGuid;

		[DataMember]
		public UInt32? CurrentValue;


		public override bool Equals(Object obj)
		{
			var damr = obj as DeviceAlarmMapReference;

			// Check for null values and compare run-time types.
			if (damr == null)
				return false;


			return (((this.CurrentValue.HasValue && damr.CurrentValue.HasValue && this.CurrentValue == damr.CurrentValue)
						|| (!this.CurrentValue.HasValue && !damr.CurrentValue.HasValue))
						&& this.DeviceAlarmMapGuid == damr.DeviceAlarmMapGuid);
		}

		public override int GetHashCode()
		{
			return this.CurrentValue.GetHashCode();
		}

		public override string ToString()
		{
			return this.CurrentValue.ToString();
		}
	}
}
