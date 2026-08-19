using FMWebAPIBusinessLogic.DTO.FMBusinessServerCommunication;

namespace FMWebAPIBusinessLogic.Interfaces.Controllers
{
    public interface ISiteController
    {
        /// <summary>
        /// standard login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        LoginResponse Login(string username, string password, string site);
        /// <summary>
        /// Will check the token and return the inital login response
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        LoginResponse Login(string token);
        bool CheckToken(string token);
    }
}
