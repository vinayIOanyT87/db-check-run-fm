namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using Attributes;

	using FMBusinessObjects.DataObjects.CodedVariables;


	/// <summary>
	/// The Tank Transfer Settings class encapsulates all the necessary settings for a Tank Transfer module
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable()]
	public class VolumeTransferModuleSettings
	{
		public VolumeTransferModuleSettings()
		{
			this.TransferAdvisoryTime = 15;
		}

		#region Transfer properties
		[DataMember(Order = 0)]
		[FMExposedSetting("Transfer Advisory Time", Maximum = 120.0, Minimum = 0.0)]
		public double TransferAdvisoryTime { get; set; }

		[DataMember(Order = 1)]
		public TransferVolumeMode TransferVolumeMode { get; set; }

		[DataMember(Order = 2)]
		public TransferVolumeMode CurrentTransferVolumeMode { get; set; }


		#endregion
	}
}
