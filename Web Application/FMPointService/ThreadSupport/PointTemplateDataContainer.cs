namespace FMPointService.ThreadSupport
{
	using FMBusinessObjects.DataObjects;
	using System.Reflection;

	public class PointTemplateDataContainer
	{
		public PointTemplatePointServiceData PointTemplatePointServiceData { get; set; }

		public Assembly PointTemplateLogicAssembly { get; set; }
	}
}
