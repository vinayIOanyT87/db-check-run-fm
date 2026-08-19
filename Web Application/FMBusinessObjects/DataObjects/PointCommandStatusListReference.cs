namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Threading.Tasks;

	[DataContract(Namespace = "")]
	[Serializable]
	public class PointCommandStatusListReference
	{
		[DataMember(Order = 0)]
		public Guid PointCommandStatusListGuid;

		[DataMember(Order = 1)]
		public int? CurrentValue;

		[DataMember(Order = 2)]
		public string CurrentKey;

		public PointCommandStatusListReference()
		{
		}

		public override bool Equals(Object obj)
		{
			var pcsrl = obj as PointCommandStatusListReference;

			// Check for null values and compare run-time types.
			if (pcsrl == null)
				return false;


			return (((CurrentValue.HasValue && pcsrl.CurrentValue.HasValue && CurrentValue == pcsrl.CurrentValue)
						|| (!CurrentValue.HasValue && !pcsrl.CurrentValue.HasValue)) 
						&& ((CurrentKey == null && pcsrl.CurrentKey == null)
						|| (CurrentKey != null && CurrentKey.Equals(pcsrl.CurrentKey)))
						&& PointCommandStatusListGuid == pcsrl.PointCommandStatusListGuid);
		}

		public override int GetHashCode()
		{
			return CurrentValue.GetHashCode();
		}

		public override string ToString()
		{
			return this.CurrentKey;
		}
	}
}
