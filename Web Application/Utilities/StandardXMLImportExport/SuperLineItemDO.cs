using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for SuperLineItemDO.
	/// </summary>
	public class SuperLineItemDO : TransactionLineItemDO
	{

		#region Properties
		public string ContractNumber
		{
			get { return contractNumber; }
			set { contractNumber = value; }
		}
		public string CLIN
		{
			get { return clin; }
			set { clin = value; }
		}
		public string ArmNumber
		{
			get { return armNumber; }
			set { armNumber = value; }
		}
		public string LineNumber
		{
			get { return lineNumber; }
			set { lineNumber = value; }
		}
		public string OperatorID
		{
			get { return operatorID; }
			set { operatorID = value; }
		}
		public string BatchNumber
		{
			get { return batchNumber; }
			set { batchNumber = value; }
		}
		public VDouble LineFill
		{
			get { return lineFill; }
			set { lineFill = value; }
		}
		public VDouble BottomVolume
		{
			get { return bottomVolume; }
			set { bottomVolume = value; }
		}
		public VDouble NetCapacity
		{
			get { return netCapacity; }
			set { netCapacity = value; }
		}
		public char TankStatus
		{
			get { return tankStatus; }
			set { tankStatus = value; }
		}
		public string Pit
		{
			get { return pit; }
			set { pit = value; }
		}
		public VDateTime RequestedDateTime
		{
			get { return requestedDateTime; }
			set { requestedDateTime = value; }
		}
		public VDateTime DispatchedDateTime
		{
			get { return dispatchedDateTime; }
			set { dispatchedDateTime = value; }
		}
		public VDateTime AcknowledgedDateTime
		{
			get { return acknowledgedDateTime; }
			set { acknowledgedDateTime = value; }
		}
		public VDateTime OnLocationTime
		{
			get { return onLocationTime; }
			set { onLocationTime = value; }
		}
		public VDateTime ValidationDateTime
		{
			get { return validationDateTime; }
			set { validationDateTime = value; }
		}
		public VDateTime CompletionDateTime
		{
			get { return completionDateTime; }
			set { completionDateTime = value; }
		}
		public VDouble ReceiptVariance
		{
			get { return receiptVariance; }
			set { receiptVariance = value; }
		}
		public VDouble DifferentialPressure
		{
			get { return differentialPressure; }
			set { differentialPressure = value; }
		}
		public VDouble LoadRackVariance
		{
			get { return loadRackVariance; }
			set { loadRackVariance = value; }
		}
		public VDouble FreezePoint
		{
			get { return freezePoint; }
			set { freezePoint = value; }
		}
		public EquipmentDO DestinationEQ1
		{
			get { return destinationEQ1; }
			set { destinationEQ1 = value; }
		}
		public EquipmentDO DestinationEQ2
		{
			get { return destinationEQ2; }
			set { destinationEQ2 = value; }
		}
		public EquipmentDO DestinationEQ3
		{
			get { return destinationEQ3; }
			set { destinationEQ3 = value; }
		}
		public EquipmentDO SourceEQ1
		{
			get { return sourceEQ1; }
			set { sourceEQ1 = value; }
		}
		public EquipmentDO SourceEQ2
		{
			get { return sourceEQ2; }
			set { sourceEQ2 = value; }
		}
		public EquipmentDO SourceEQ3
		{
			get { return sourceEQ3; }
			set { sourceEQ3 = value; }
		}
		public MeterReadingDO MeterReading
		{
			get { return meterReading; }
			set { meterReading = value; }
		}
		public int TransactionStatus
		{
			get { return transactionStatus; }
			set { transactionStatus = value; }
		}
		public bool DeleteFlag
		{
			get { return deleteFlag; }
			set { deleteFlag = value; }
		}
		public System.Collections.ArrayList SubLineItems
		{
			get { return subLineItems; }
		}
		#endregion Properties
		
		public SuperLineItemDO()
		{
		}
	}
}
 