using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FMBusinessObjects.DataObjects
{
	[Flags]
	public enum LeakDetectionError
	{
		None = 0x0,
		InvalidIndex = 0x1,
		LeakrateToHigh = 0x2,
		TestFailed = 0x4,
		ConnectionFailed = 0x8,
		SqlError = 0x10,
		NotEnoughData = 0x20,
		OverDeltaTemp = 0x40,
		NotEnoughMemory = 0x80,
		ArchiveAccessError = 0x100,
		NoMovement = 0x200,
		OverCertRate = 0x400,
	}

	[DataContract]
	public class LeakAnalysisResult
	{
		public LeakAnalysisResult()
        {
			AnalysisStatusMessage = new List<string>();
			TestResult = string.Empty;
		}

		[DataMember]
		public double LeakRate { get; set; }
		[DataMember]
		public double CertRate { get; set; }
		[DataMember]
		public double LeakThreshold { get; set; }
		[DataMember]
		public double StandardDeviation { get; set; }
		[DataMember]
		public double MinValue { get; set; }
		[DataMember]
		public double MaxValue { get; set; }
		[DataMember]
		public double MinimumFillPercentage { get; set; }
		[DataMember]
		public double Average { get; set; }
		[DataMember]
		public double MinTemperature { get; set; }
		[DataMember]
		public double MaxTemperature { get; set; }
		[DataMember]
		public double DeltaTemperature { get; set; }
		[DataMember]
		public LeakDetectionError AnalysisStatus { get; set; }
		[DataMember]
		public List<string> AnalysisStatusMessage { get; set; }
		[DataMember]
		public string TestResult { get; set; }
		[DataMember]
		public uint NumSamples { get; set; }
		[DataMember]
		public long ReportTime { get; set; }
		[DataMember]
		public DateTimeOffset StartTime { get; set; }
		[DataMember]
		public DateTimeOffset StopTime { get; set; }
		[DataMember]
		public double UsableSampleTime { get; set; }
		[DataMember]
		public double GraphMinValue { get; set; }
		[DataMember]
		public double GraphMaxValue { get; set; }
		[DataMember]
		public double GraphTemperatureDelta { get; set; }
		[DataMember]
		public double LevelStart { get; set; }
		[DataMember]
		public double LevelEnd { get; set; }
        [DataMember]
        public double? PressureStart { get; set; }
        [DataMember]
        public double? PressureEnd { get; set; }
        [DataMember]
		public double WaterLevelStart { get; set; }
		[DataMember]
		public double WaterLevelEnd { get; set; }
		[DataMember]
		public ushort MinGaugeTestTime { get; set; }
		[DataMember]
		public Guid GaugeType { get; set; }
		[DataMember]
		public Guid LeakRecordId { get; set; }
		[DataMember]
		public EngineeringUnit VolumeUnits { get; set; }
		[DataMember]
		public EngineeringUnit TemperatureUnits { get; set; }
        [DataMember]
        public EngineeringUnit PressureUnits { get; set; }
        [DataMember]
		public EngineeringUnit LeakRateUnits { get; set; }
		[DataMember]
		public int VolumePrecision { get; set; }
		[DataMember]
		public int TemperaturePrecision { get; set; }
        [DataMember]
        public int PressurePrecision { get; set; }
        [DataMember]
		public int LeakRatePrecision { get; set; }

		public bool EnableReportPrint { get; set; }

		public static string GetDispalyMessgae(LeakDetectionError error)
        {
            string message = string.Empty;
            if (error == LeakDetectionError.None)
            {
                // leakAnalysisResult.AnalysisStatusMessage = string.Empty;
            }
            else if (error == LeakDetectionError.TestFailed)
            {
                //leakAnalysisResult.AnalysisStatusMessage = string.Empty;
            }
            else if (error == LeakDetectionError.InvalidIndex)
            {
                message = "Invalid Index";
            }
            else if (error == LeakDetectionError.LeakrateToHigh)
            {
                message = "Calculated Leak Rate is above Leak Threshold";
            }
            else if (error == LeakDetectionError.ConnectionFailed)
            {
                message = "Connection Failed";
            }
            else if (error == LeakDetectionError.SqlError)
            {
                message = "Error occurred in database";
            }
            else if (error == LeakDetectionError.NotEnoughData)
            {
                // leakAnalysisResult.AnalysisStatusMessage = string.Empty;
            }
            else if (error == LeakDetectionError.OverDeltaTemp)
            {
                message = "Delta temperature is out of range for gauge";
            }
            else if (error == LeakDetectionError.NotEnoughMemory)
            {
                message = "Not enough memory on server";
            }
            else if (error == LeakDetectionError.ArchiveAccessError)
            {
                message = "Archive Access Error";
            }
            else if (error == LeakDetectionError.NoMovement)
            {
                message = "No movement, possible invalid data";
            }
			else if (error == LeakDetectionError.OverCertRate)
			{
				message = "Leak Rate is above Certification Leak Rate";
			}
			return message;
        }
	}
}
