
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using CodedVariables;


	[DataContract(Namespace = "")]
	[Serializable()]
	public class QuantityModuleSettings
	{
		#region Public Properties
		[DataMember(Order = 0)]
		public VolumeCalculationType VolumeCalculationType { get; set; }

		[DataMember(Order = 1)]
		public MassOrWeightCalculationType MassOrWeightCalculationType { get; set; }

		#endregion

		#region Constructors
		public QuantityModuleSettings()
		{
			this.VolumeCalculationType = VolumeCalculationType.API1995Calculations;
			this.MassOrWeightCalculationType = MassOrWeightCalculationType.Mass;
		}
		#endregion
	}
}