
namespace FuelsManager.Areas.FCEE.Controllers
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

	using FMBusinessObjects.Constants;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;
	 using System.Runtime.Serialization;
	 using FuelsManager.Areas.FCEE.ViewModels;
    using FuelsManager.Areas.FCEE.Controllers;


    public class FCEEMappingEditorController : FMBaseControllerEx
    {
        // GET: 
        [HttpGet, ValidateJsonAntiForgeryToken]
        public ActionResult FCEEMappingEditor(string FCEEMappingGuidString)
        {
            FCEEMapping FCEEMapping;
            FCEEMappingEditorModel FCEEMappingEditorModel;

            try
            {
                var FCEEMappingGuid = new Guid(FCEEMappingGuidString);
                if (FCEEMappingGuid != Guid.Empty)
                {
                    FCEEMapping = FMChannelHelper.MakeCall<IFCEEServiceManager, FCEEMapping>(x => x.Get(this.Security, FCEEMappingGuid));
                }
                else
                {
                    FCEEMapping = new FCEEMapping
                    {
                    };
                }

                FCEEMappingEditorModel = new FCEEMappingEditorModel(FCEEMapping, !this.Security.HasRight(RIGHT.MODIFY_POINTS));
            }
            catch (Exception except)
            {
                this.OnError(except);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            return PartialViewWithErrorMessages("FCEEMappingEditor", FCEEMappingEditorModel, JsonRequestBehavior.AllowGet);
        }
    }
    }