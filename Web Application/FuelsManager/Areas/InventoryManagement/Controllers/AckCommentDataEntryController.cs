
namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	public class AckCommentDataEntryController : FMBaseControllerEx
	{
		public static void AckWithComment(SecurityClass security, AckCommentDataEntryModel model)
		{
			FMChannelHelper.MakeCall<IAlarmStatus>(x => x.AcknowledgeAlarms(security, model.Comment, model.AlarmGuids));
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult OkButtonPress(string comment, string modelString)
		{
			AckCommentDataEntryModel model;
			try
			{
				model = System.Web.Helpers.Json.Decode<AckCommentDataEntryModel>(modelString);
				model.Comment = comment;
				if (model.DoProcessing)
				{
					AckWithComment(this.Security, model);
				}
				return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}
	}
}