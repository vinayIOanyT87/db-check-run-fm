
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

	public class ShelveDataEntryController : FMBaseControllerEx
	{
		protected int IntStringConvert(string val)
		{
			if (string.IsNullOrWhiteSpace(val))
			{
				return 0;
			}
			return Int32.Parse(val);
		}

		protected bool BoolStringConvert(string val)
		{
			if (string.IsNullOrWhiteSpace(val))
			{
				return false;
			}
			return Boolean.Parse(val);
		}

		protected void DoShevling(SecurityClass security, ShelveDataEntryModel model)
		{
			if (model.DoProcessing)
			{
				FMChannelHelper.MakeCall<IPointServiceManager>(
					x => x.Shelve(this.Security, model.Days, model.Hours, model.Minutes, model.OneShot, model.AlarmGuids));
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult OkButtonPress(string days, string hours, string minutes, bool oneShot, string modelString)
		{
			ShelveDataEntryModel model;
			try
			{
				model = System.Web.Helpers.Json.Decode<ShelveDataEntryModel>(modelString);
				model.Days = this.IntStringConvert(days);
				model.Hours = this.IntStringConvert(hours);
				model.Minutes = this.IntStringConvert(minutes);
				model.OneShot = oneShot;
				if (model.Days == 0 && model.Hours == 0 && model.Minutes == 0 && model.OneShot == false)
				{
					this.OnError("Invalid Entry of total shelve time equal to 0!");
					return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
				}
				if (model.OneShot)
				{
					this.DoShevling(this.Security, model);
					return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
				}
				var entryError = false;

				if (model.Days < 0 || model.Days > 999)
				{
					entryError = true;
					this.OnError("Invalid Days!");
				}
				if (model.Hours < 0 || model.Hours > 23)
				{
					entryError = true;
					this.OnError("Invalid Hours!");
				}
				if (model.Minutes < 0 || model.Minutes > 23)
				{
					entryError = true;
					this.OnError("Invalid Minutes!");
				}
				if (entryError)
				{
					return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
				}
				this.DoShevling(this.Security, model);
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