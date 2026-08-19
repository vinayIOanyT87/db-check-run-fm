using FuelsManager.DTO;

namespace FuelsManager.Interfaces
{
    public interface IFuelManagerConfigurationFactory
    {
        FuelManagerConfiguration GetConfig();
    }
}