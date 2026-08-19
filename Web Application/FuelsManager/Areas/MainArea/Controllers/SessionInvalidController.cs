namespace FuelsManager.Areas.MainArea.Controllers
{
	using System.Web.Mvc;

	/// <summary>
	/// Purposefully inherit from standard controller so we can properly 
	/// terminate handling of session invalid state and return to login page.
	/// </summary>
	public class SessionInvalidController : Controller
    {
	    [HttpGet]
		public ActionResult SessionInvalidIndex()
	    {
		    return this.View();
	    }
    }
}
