namespace FMTransactionExportService
{
	/// <summary>
	/// Determines the schedule by which exports are executed
	/// </summary>
	public enum ExportScheduleType
	{
		/// <summary>
		/// Exports should be performed at a set interval defined by another configuration setting
		/// </summary>
		Interval = 0,

		/// <summary>
		/// Exports should be performed once daily at a fixed time defined by another configuration setting
		/// </summary>
		FixedTime = 1,
	}
}