namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects.CodedVariables;

	[DataContract(Namespace = "")]
	[KnownType(typeof(MovementType))]
	[Serializable()]
	public class MovementModuleSettings
	{
		public const short MovementDiscreteAlarm_Normal = 0;
		public const short MovementDiscreteAlarm_ControlAlarm = 1;

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementModuleSettings()
		{
				this.Init();
		}
		#endregion

		#region Properties
		[DataMember(Order = 0)] public bool InterlockSourceDestinationSetpoints { get; set; }
		[DataMember(Order = 1)] public bool DeleteAfterCompletion { get; set; }
		[DataMember(Order = 2)] public string OrderNumber { get; set; }
		[DataMember(Order = 3)] public string Comment { get; set; }
		[DataMember(Order = 4)] public bool HandGaugeData { get; set; }
		[DataMember(Order = 5)] public bool SendToAccounting { get; set; }
		[DataMember(Order = 6)] public string Ticket { get; set; }
		[DataMember(Order = 7)] public string Printer { get; set; }
		[DataMember(Order = 8)] public bool UseControlTagStartStop { get; set; }
		[DataMember(Order = 9)] public Guid ControlTagGuid { get; set; }
		[DataMember(Order = 10)] public bool StopHaltBasedOnZeroFlow { get; set; }
		[DataMember(Order = 11)] public bool StartTimeBasedOnNonZeroFlow { get; set; }
		[DataMember(Order = 12)] public int? ZeroFlowHoldOffTime { get; set; }
		[DataMember(Order = 13)] public bool SetPendingStatus { get; set; }
		[DataMember(Order = 14)] public DateTimeOffset? PlannedStartDateTime { get; set; }
		[DataMember(Order = 15)] public List<MovementNodeData> MovementNodeDataList { get; set; }
		[DataMember(Order = 16)] public bool DeleteAfterStop { get; set; }
		[DataMember(Order = 17)] public MovementType Type { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.MovementNodeDataList = new List<MovementNodeData>();

			this.InterlockSourceDestinationSetpoints	= true;
			this.DeleteAfterCompletion					= false;
			this.DeleteAfterStop							= false;
			this.OrderNumber								= string.Empty;
			this.Comment									= string.Empty;
			this.HandGaugeData							= false;
			this.SendToAccounting						= false;
			this.Ticket										= string.Empty;
			this.Printer									= string.Empty;
			this.UseControlTagStartStop				= false;
			this.ControlTagGuid							= Guid.Empty;
			this.StopHaltBasedOnZeroFlow				= false;
			this.StartTimeBasedOnNonZeroFlow			= false;
			this.ZeroFlowHoldOffTime					= null;
			this.SetPendingStatus						= false;
			this.PlannedStartDateTime					= null;
			this.Type										= MovementType.Transfer;
		}
		#endregion
	}
}
