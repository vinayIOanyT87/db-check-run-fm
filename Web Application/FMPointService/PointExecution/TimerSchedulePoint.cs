namespace FMPointService.PointExecution
{
	using System;
	using FMPointCommon;

	using global::FMPointService.ThreadSupport;

	public class TimerSchedulePoint : ISMRTimerAction
	{
		public void PerformAction(Guid pointGuid)
		{
			PointExecutionQueuer.StaticQueuePointForProcessing(pointGuid);
		}
	}
}
