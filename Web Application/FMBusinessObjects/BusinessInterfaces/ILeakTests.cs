namespace FMBusinessObjects.BusinessInterfaces
{
	using FMBusinessObjects.DataObjects;

	using System;
	using System.ServiceModel;

	[ServiceContract]
	public interface ILeakTests
	{
		[OperationContract]
		LeakDetectionError Run(SecurityClass security, Point point, LeakAnalysisType leakAnalysisType, LeakAnalysisMethod leakAnalysisMethod, DateTimeOffset start, DateTimeOffset end, ref LeakAnalysisResult leakAnalysisResult);

		[OperationContract]
		bool CleanupLeakReportData(SecurityClass security, Guid LeakReportId);
	}
}
