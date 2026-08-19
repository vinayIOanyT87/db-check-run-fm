
namespace FuelsManager.ServiceProcess.Interfaces
{
    public interface IServiceProcessor
    {
        /// <summary>
        /// Starts the main worker thread in the implemented Processor class
        /// </summary>
        void StartProcessThread();

        /// <summary>
        /// Stops the main worker thread in the implemented Processor class
        /// </summary>
        void StopProcessThread();
    }
}
