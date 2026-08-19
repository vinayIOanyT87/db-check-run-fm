namespace FMBusinessObjects.Interfaces
{
	using FMBusinessObjects.DataObjects;

	public interface IFuelsManagerModule
	{
		ModuleInputOutputCollection GetInputOutputCollection(string calculationName);
	}
}
