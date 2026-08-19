namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using Attributes;
	using System.Xml.Serialization;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects.CodedVariables;

	/// <summary>
	/// The Tank Transfer Settings class encapsulates all the necessary settings for a Tank Transfer module
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable()]
	public class TankTransferModuleSettings
	{
		public TankTransferModuleSettings()
		{
			this.TransferAdvisoryTime = 15;
			this.TransferVolumeMode = TransferVolumeMode.GrossObservedVolume;
			this.CurrentTransferVolumeMode = TransferVolumeMode.GrossObservedVolume;
		}

		#region Tank Transfer properties
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
