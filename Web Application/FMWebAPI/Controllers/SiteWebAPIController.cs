using FMCore.Interfaces;
using FMWebAPI.Models;
using FMWebAPIBusinessLogic.DTO.FMBusinessServerCommunication;
using FMWebAPIBusinessLogic.Interfaces.Controllers;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Web.Http;

namespace FMWebAPI.Controllers
{
    [RoutePrefix("api/Site")]
    public class SiteWebAPIController : ApiController
    {
        private readonly IFMCustomLogger _logger;
        private readonly ISiteController _siteController;
        private readonly IErrorTransactionSubmissionProxy _errorTransactionSubmissionProxy;
        public SiteWebAPIController(IFMCustomLogger logger, ISiteController site, IErrorTransactionSubmissionProxy errorTransactionSubmissionProxy)
        {
            _logger = logger;
            _siteController = site;
            _errorTransactionSubmissionProxy = errorTransactionSubmissionProxy;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public LoginResponse Login(LoginRequestDTO request)
        {
            try
            {
                return _siteController.Login(request.username, request.password, request.site);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Login failed");
                if (e.Message == "Site Not Found.")
                {
                    //do not return error if site is not found
                    return new LoginResponse()
                    {
                        LoginSuccess = false
                    };
                }
                throw;
            }
        }


        [HttpPost]
        [Route("GetLoginResponseAlreadyAuthenticated")]
        public LoginResponse GetLoginResponse([FromBody]string token)
        {
            try
            {
                return _siteController.Login(token);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Login failed");
                if (e.Message == "Site Not Found.")
                {
                    //do not return error if site is not found
                    return new LoginResponse()
                    {
                        LoginSuccess = false
                    };
                }
                throw;
            }
        }

        [Route("ping")]
        [HttpGet]
        public DateTime ping()
        {
            return DateTime.Now;
        }
    }
}
