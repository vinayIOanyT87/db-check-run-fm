namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Runtime.InteropServices;
	using System.Web.Mvc;
    using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class PointTemplatesController : FMBaseControllerEx
	{
        #region Public methods
        [HttpGet]
		public ActionResult PointTemplatesIndex()
		{
			var context = this.Session[PointTemplatesFilterContext.SessionKey] as PointTemplatesFilterContext;
			var model = new PointTemplatesModel(context, this.Security);
			model.DeleteEnabled = (this.Security.HasRight(RIGHT.MODIFY_POINT_TEMPLATES));
         model.HasFCEERight = this.Security.HasRight(RIGHT.MODIFY_FCEE_DATA);

         var pointTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
															x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));
			var pointTypeList = new List<KeyValuePair<string, string>>();

			foreach (var pointType in pointTypes)
			{
				var pointTypeSelectItem = new KeyValuePair<string, string>( pointType.IdentityGuid.ToString(), pointType.ID );
				pointTypeList.Add(pointTypeSelectItem);
			}
			model.PointTypeList = pointTypeList;

			try
			{
				// Retrieve the point templates by type.
				model.Templates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.Security, null));

				this.Session[PointTemplatesFilterContext.SessionKey] = new PointTemplatesFilterContext(model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PointTemplatesIndex(PointTemplatesModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var context = new PointTemplatesFilterContext(model);
					this.Session[PointTemplatesFilterContext.SessionKey] = context;

					// TODO: Handle post-back
				}
         }
         catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ButtonSubmit(PointTemplatesModel model, string command)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					if (command.Equals("addButton", StringComparison.InvariantCultureIgnoreCase))
					{
						// TODO: redirect to new point template
						return this.RedirectToAction("PointTemplateDetail");
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return this.View("PointTemplatesIndex", model);
			}

			return this.RedirectToAction("PointTemplatesIndex", model);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult AddPointTemplate(string id, string description, string type)
		{
			try
			{
				var duplicateTemplateGuid = FMChannelHelper.MakeCall<IPointTemplates, Guid?>( x => x.GetDuplicate(this.Security, id, this.Security.SiteGuid));
				if (duplicateTemplateGuid == null || duplicateTemplateGuid == new Guid())
				{
					PointTemplate pointTemplate = new PointTemplate();
					pointTemplate.PointTemplateGuid = Guid.NewGuid();
					pointTemplate.ID = id;
					pointTemplate.Description = description;
					pointTemplate.PointTemplateTypeGuid = new Guid(type);
					pointTemplate.SiteGuid = this.Security.SiteGuid;
					pointTemplate.Standard = false;
					pointTemplate.LevelUnit = EngineeringUnit.FmlFtIn16Th;
					pointTemplate.TemperatureUnit = EngineeringUnit.FmtDegF;
					pointTemplate.DensityUnit = EngineeringUnit.FmdDegApi;
					pointTemplate.PressureUnit = EngineeringUnit.FmpPsi;
					pointTemplate.FlowUnit = EngineeringUnit.FmvfGpm;
					pointTemplate.VolumeUnit = EngineeringUnit.FmvUsGal;
					pointTemplate.MassUnit = EngineeringUnit.FmmLb;
					pointTemplate.VelocityUnit = EngineeringUnit.FmvrFpm; 
					pointTemplate.MassFlowUnit = EngineeringUnit.FmmfLbHr;
					pointTemplate.LevelDecimalPlaces = 0;
					pointTemplate.TemperatureDecimalPlaces = 2;
					pointTemplate.DensityDecimalPlaces = 2;
					pointTemplate.PressureDecimalPlaces = 2;
					pointTemplate.FlowDecimalPlaces = 2;
					pointTemplate.VolumeDecimalPlaces = 2;
					pointTemplate.MassDecimalPlaces = 2;
					pointTemplate.VelocityDecimalPlaces = 2;
					pointTemplate.MassFlowDecimalPlaces = 2;
					pointTemplate.LevelMaximum = 40;
					pointTemplate.LevelMinimum = 0;
					pointTemplate.TemperatureMaximum = 300.0;
					pointTemplate.TemperatureMinimum = -300.0;
					pointTemplate.DensityMaximum = 100;
					pointTemplate.DensityMinimum = 0;
					pointTemplate.PressureMaximum = 30.0;
					pointTemplate.PressureMinimum = 0;
					pointTemplate.VolumetricFlowMaximum = 1000.00;
					pointTemplate.VolumetricFlowMinimum = -1000.00;
					pointTemplate.VolumeMaximum = 10000.0;
					pointTemplate.VolumeMinimum = 0;
					pointTemplate.MassMaximum = 10000000.0;
					pointTemplate.MassMinimum = 0;
					pointTemplate.VelocityMaximum = 10.00;
					pointTemplate.VelocityMinimum = -10.00;
					pointTemplate.MassFlowMaximum = 3000.00;
					pointTemplate.MassFlowMinimum = -3000.00;
					pointTemplate.ProfileImageGuid = new Guid( "01C81D1D-2BE2-430E-8352-B4AFFE70EC42") ;

					FMChannelHelper.MakeCall<IPointTemplates>(x => x.Add(this.Security, pointTemplate));

					this.ModelState.Clear();
					this.AddSuccess(this.GetTranslatedText("Save Successful"));

					return this.JsonWithErrorMessages( new { pointTemplateGuid = pointTemplate.PointTemplateGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					this.ModelState.Clear();
					return this.JsonWithErrorMessages( new { pointTemplateGuid = duplicateTemplateGuid, duplicateFound = true }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string pointToDeleteId)
		{
			var context = this.Session[PointTemplatesFilterContext.SessionKey] as PointTemplatesFilterContext;

			try
			{
				var pointTemplateGuid = new Guid(pointToDeleteId);
				FMChannelHelper.MakeCall<IPointTemplates>(x => x.Purge(this.Security, pointTemplateGuid));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return this.View("PointTemplatesIndex", context.Model);
			}

			return this.RedirectToAction("PointTemplatesIndex");
		}

		[HttpGet]
		public ActionResult PointTemplateDetail(string id)
		{
			var model = new PointTemplateDetailModel();

         try
         {
				model.HasFCEERight = this.Security.HasRight(RIGHT.MODIFY_FCEE_DATA);
				if (string.IsNullOrEmpty(id) || id.Equals("PointTemplatesIndex", StringComparison.InvariantCultureIgnoreCase))
				{
					model.Template = new PointTemplate();
				}
				else
				{
					var pointTemplateGuid = new Guid(id);
					model.Template = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));
                    model.DefaultDrawingGuidString = (model.Template.DefaultDrawingGuid == null) ? Guid.Empty.ToString().ToLower() : model.Template.DefaultDrawingGuid.ToString().ToLower();
                    model.AssociatedDrawings = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(x => x.EnumerateAvailableDrawingNamesByPointTemplate(this.Security,pointTemplateGuid));
                }

				model.ModifyEnabled = this.Security.HasRight(RIGHT.MODIFY_POINT_TEMPLATES)
											&& (this.Security.SiteGuid == model.Template.SiteGuid
											|| model.Template.SiteGuid == Guid.Empty);

				this.Session[PointTemplateDetailModel.SessionKey] = model;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PointTemplateDetail(PointTemplateDetailModel modelReturn)
		{
			PointTemplateDetailModel model = null;

			if (this.ModelState.IsValid)
			{
				try
				{
					model = (PointTemplateDetailModel)this.Session[PointTemplateDetailModel.SessionKey];
					//model.Template.ID = modelReturn.Template.ID;
                    Guid defaultDrawingGuid = Guid.Empty;
                    if (Guid.TryParse(modelReturn.DefaultDrawingGuidString, out defaultDrawingGuid))
                    {
                        if (defaultDrawingGuid != Guid.Empty)
                        {
                            model.Template.DefaultDrawingGuid = defaultDrawingGuid;
                        }
                        else
                        {
                            model.Template.DefaultDrawingGuid = null;
                        }
                    }
                    else
                    {
                        model.Template.DefaultDrawingGuid = null;
                    }

					if (model.Template.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<IPointTemplates>(x => x.Modify(this.Security, model.Template));
					}
					else
					{
						FMChannelHelper.MakeCall<IPointTemplates>(x => x.Add(this.Security, model.Template));
					}
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
					return this.View(model);
				}
			}

			return this.RedirectToAction("PointTemplatesIndex");
		}
       #endregion

    }
}
