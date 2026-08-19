namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.DataObjects.CodedVariables;
	using System;
	using System.Collections.Generic;
    using System.Xml.Serialization;
    using static FMBusinessObjects.DataObjects.MovementNodeData;

	/// <summary>
	/// This class is the model for the movement node editor.
	/// </summary>
	[Serializable]
	public class MovementModuleSettingsEditorModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementModuleSettingsEditorModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public List<MovementNodeModel> MovementNodeModelList { get; set; }
		public string NewId { get; set; }
		public string PointId { get; set; }
		public Guid PointGuid { get; set; }
		public string PointPropertyId { get; set; }
		public Guid PointPropertyGuid { get; set; }
		public bool Readonly { get; set; }
		public bool IsTemplatePoint { get; set; }

		// Setup section
		public bool InterlockSourceDestinationSetpoints { get; set; }
		public bool IncludeHandgaugeValues { get; set; }
		public MovementType Type {get; set;}

		// Recording section
		public string OrderNumber { get; set; }
		public string Comment { get; set; }
		public bool SendToAccounting { get; set; }

		// Start times section
		public bool DeleteAfterCompletion { get; set; }
		public bool DeleteAfterStop { get; set; }
		public bool UseControlTagStartStop { get; set; }
		public Guid SelectedControlTagGuid { get; set; }
		public bool StopHaltBasedOnZeroFlow { get; set; }
		public bool StartTimeBasedOnNonZeroFlow { get; set; }
		public int? ZeroFlowHoldOffTime { get; set; }
		public bool SetPendingStatus { get; set; }
		public string PlannedStartDateTime { get; set; }

		// Create new movement section
		public bool EnableCreateNewSection { get; set; }

		public string NumberGroupSeparator { get; set; }
		public string NumberDecimalSeparator { get; set; }
		public int[] NumberGroupSizes { get; set; }
		public string ShortDatePattern { get; set; }
		public string TimePattern { get; set; }
		public string TimeZone { get; set; }

		public bool IsLaunchedFromSummary { get; set; }
		public bool IsActive { get; set; }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Init()
		{
			this.MovementNodeModelList	= new List<MovementNodeModel>();
			this.PointId = string.Empty;
			this.PointGuid	= Guid.Empty;
			this.PointPropertyId	= string.Empty;
			this.PointPropertyGuid = Guid.Empty;
			this.IsTemplatePoint = false;

			this.InterlockSourceDestinationSetpoints = false;
			this.DeleteAfterCompletion = false;
			this.DeleteAfterStop	= false;

			this.OrderNumber = string.Empty;
			this.Comment = string.Empty;
			this.IncludeHandgaugeValues = false;
			this.SendToAccounting = false;

			this.UseControlTagStartStop = false;
			this.SelectedControlTagGuid = Guid.Empty;
			this.StopHaltBasedOnZeroFlow = false;
			this.StartTimeBasedOnNonZeroFlow = false;
			this.ZeroFlowHoldOffTime = null;
			this.SetPendingStatus = false;
			this.PlannedStartDateTime = string.Empty;

			this.EnableCreateNewSection = false;

			this.NumberGroupSeparator = string.Empty;
			this.NumberDecimalSeparator = string.Empty;
			this.NumberGroupSizes = new int[1];
			this.ShortDatePattern = string.Empty;
			this.TimePattern = string.Empty;
			this.TimeZone = string.Empty;

			this.IsLaunchedFromSummary = false;
		}		
		#endregion
	}

	[Serializable]
	public class MovementNodeModel: IComparable<MovementNodeModel>
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementNodeModel()
		{
				this.Init();
		}
        #endregion
        
		#region Public Methods
        // Default comparer for Part type.
        public int CompareTo(MovementNodeModel node)
		{
			// A null value means that this object is greater.
			if (node == null)
				return 1;

			else
				return this.MovementNodeId.CompareTo(node.MovementNodeId);
		}
        #endregion

        #region Properties
        public Guid MovementNodeGuid { get; set; }
		public string MovementNodeId { get; set; }
		public TransferDirection TransferDirection { get; set; }
		public string TransferTarget { get; set; }
		public bool IndividualNodeControl { get; set; }
		public TransferModes TransferMode { get; set; }
		public string Units { get; set; }
		public NodeModuleType ModuleType { get; set; }

		public string LevelProductUnits { get; set; }
		public string VolumeUnits { get; set; }

		public int IntLevelUnits { get; set; }
		public int IntVolumeUnits { get; set; }

		public TransferVolumeMode NodeTransferVolumeMode { get; set; }

		public string TransferModeName 
		{ 
			get
			{
				switch(this.TransferMode)
				{
					case TransferModes.Level:
						return "Level";
					case TransferModes.Batch:
							return "Batch";
					case TransferModes.Inactive:
							return "Inactive";
					default:
							return "Inactive";
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.MovementNodeGuid	= Guid.Empty;
			this.TransferDirection  = TransferDirection.Source;
			this.TransferTarget = "0.0";
			this.TransferMode		= TransferModes.Inactive;
			this.MovementNodeId	= string.Empty;
			this.Units = string.Empty;
			this.LevelProductUnits = string.Empty;
			this.VolumeUnits = string.Empty;
			this.ModuleType = NodeModuleType.StandardTank;
		}
		#endregion
	}

	[Serializable]
	public class MovementNodeDropdownModel
	{
		#region Data members
		private Guid movementNodeGuid;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementNodeDropdownModel()
		{
				this.Init();
		}
		#endregion

		#region Properties
		public string MovementNodeId { get; set; }
		public string MovementNodeGuidStr { get; private set; }
		public string EmptyGuid { get; private set; }
		public Guid MovementNodeGuid 
		{ 
			get { return this.movementNodeGuid; } 
			set
			{
				this.movementNodeGuid = value;
				this.MovementNodeGuidStr = this.movementNodeGuid.ToString();
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
				this.MovementNodeId = string.Empty;
				this.MovementNodeGuid = Guid.Empty;
				this.EmptyGuid = Guid.Empty.ToString();
		}
		#endregion
	}

	[Serializable]
	public class MovementTicketModel
    {
		#region Constuctors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementTicketModel()
        {
			this.Init();
        }
		#endregion

		#region Properties
		public string TicketName { get; set; }
		public string TicketValue { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.TicketName = string.Empty;
			this.TicketValue = string.Empty;
		}
		#endregion
	}

	[Serializable]
	public class MovementPrinterModel
	{
		#region Constuctors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementPrinterModel()
        {
			this.Init();
        }
		#endregion

		#region Properties
		public string PrinterName { get; set; }
		public string PrinterValue { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.PrinterName = string.Empty;
			this.PrinterValue = string.Empty;
		}
		#endregion
	}

	[Serializable]
	public class MovementControlTagsModel
	{
		#region Constuctors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementControlTagsModel()
      {
			this.Init();
      }
		#endregion

		#region Properties
		public string ControlTagName { get; set; }
		public string ControlTagValue { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
      {
			this.ControlTagName = string.Empty;
			this.ControlTagValue = string.Empty;
		}
		#endregion
	}
}