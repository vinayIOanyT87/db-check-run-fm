namespace FMBusinessObjects.DataObjects
{
	using FMBusinessObjects.DataObjects.CodedVariables;
	using System;
	using System.Runtime.Serialization;

	[DataContract(Namespace = "")]
	[Serializable()]
	public class MovementNodeData
	{
		#region Public data members
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementNodeData()
		{
			this.Init();
		}
		#endregion

		#region Properties
		[DataMember(Order = 0)] public Guid MovementNodeGuid { get; set; }
		[DataMember(Order = 1)] public TransferDirection TransferDirection { get; set; }
		[DataMember(Order = 2)] public double? TransferTarget { get; set; }
		[DataMember(Order = 3)] public TransferModes TransferMode { get; set; }
		[DataMember(Order = 4)] public bool IndividualNodeControl { get; set; } 
		[DataMember(Order = 5)] public string Units { get; set; }
		[DataMember(Order = 6)] public int IntLevelUnits { get; set; }
		[DataMember(Order = 7)] public int IntVolumeUnits { get; set; }
		[DataMember(Order = 8)] public NodeModuleType ModuleType { get; set; }
		[DataMember(Order = 9)] public TransferVolumeMode NodeTransferVolumeMode { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.MovementNodeGuid	= Guid.Empty;
			this.TransferDirection  = TransferDirection.Source;
			this.TransferTarget	= 0.0;
			this.TransferMode		= TransferModes.Batch;
			this.IndividualNodeControl = false;
			this.Units = string.Empty;
		}
		#endregion
	}
}
