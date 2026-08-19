using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RTUWebAPI.Models;
using RTUWebAPI.Services;

namespace RTUWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AvailableModulesController : RTUControllerBase
	{
		// GET: api/AvailableModules
		[HttpGet]
		public ActionResult Get(string filename)
		{
			var availableConfiguration =  new AvailableConfigurationService().GetAvailableConfiguration(true, filename);
			return JsonWithErrorMessages(availableConfiguration);
		}
	}
}
