
namespace FuelsManager.Areas.FCEE.Controllers
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Web;
   using System.Web.Mvc;

   using FuelsManager.Areas.Controllers;
   using FuelsManager.Areas.FCEE.ViewModels;

   using FMBusinessObjects.BusinessInterfaces;
   using FMBusinessObjects.ChannelFactories;
   using FMBusinessObjects.DataObjects;
   using System.Web.Script.Serialization;
   using static TransactionFields.TransactionContext;
   using System.Text.Encodings.Web;
   using System.Data.SqlClient;

   public class FCEDeviceEditorController : FMBaseControllerEx
   {
      [HttpGet, ValidateJsonAntiForgeryToken]
      public ActionResult FCEDeviceEditor(string fceDeviceGuidString)
      {
         FCEDevice fceDevice;
         FCEDeviceEditorModel fceDeviceEditorModel;


         try
         {
            var fceDeviceGuid = new Guid(fceDeviceGuidString);
            if (fceDeviceGuid != Guid.Empty)
            {
               fceDevice = FMChannelHelper.MakeCall<IFCEDevices, FCEDevice>(x => x.Get(this.Security, fceDeviceGuid));
            }
            else
            {
               fceDevice = new FCEDevice();
            }
            fceDeviceEditorModel = new FCEDeviceEditorModel(fceDevice);
         }

         catch (Exception except)
         {
            this.OnError(except);
            return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }

         return PartialViewWithErrorMessages("FCEDeviceEditor", fceDeviceEditorModel, JsonRequestBehavior.AllowGet);
      }

      [HttpPost]
      [ValidateJsonAntiForgeryToken]
      public ActionResult SaveFCEDevice(FCEDevice fceDevice)
      {
         FCEDeviceEditorModel fceDeviceEditorModel = new FCEDeviceEditorModel(fceDevice);
         if (!TryValidateModel(fceDeviceEditorModel, nameof(FCEDeviceEditorModel)))
         {
            return this.JsonWithErrorMessages(null);
         }
         if (fceDevice.MinTime > fceDevice.MaxTime) {
            this.OnError(string.Format("{0} {1} cannot be greater than {2} {3}.", TranslateText("MinTime"), fceDevice.MinTime, TranslateText("MaxTime"), fceDevice.MaxTime));
            return this.JsonWithErrorMessages(null);
         }
         try
         {
            FCEDevice loadedfceDevice = FMChannelHelper.MakeCall<IFCEDevices, FCEDevice>(x => x.GetbyIMEI(this.Security, fceDevice.ImeiNumber));
            if (loadedfceDevice != null && loadedfceDevice.FCEDeviceGuid != Guid.Empty && loadedfceDevice.FCEDeviceGuid != fceDevice.FCEDeviceGuid)
            {
                this.OnError(string.Format("{0} must be unique.", TranslateText("Imei Number")));
                return this.JsonWithErrorMessages(null);

            }
            if (fceDevice.FCEDeviceGuid == Guid.Empty)
            {
                FMChannelHelper.MakeCall<IFCEDevices>(x => x.Add(this.Security, fceDevice));
            }
            else
            {
               FMChannelHelper.MakeCall<IFCEDevices>(x => x.Modify(this.Security, fceDevice));
            }
         }
         catch (Exception except)
         {
            this.OnError(except);
            return this.JsonWithErrorMessages(null);

         }

         return this.JsonWithErrorMessages("", JsonRequestBehavior.AllowGet);
      }
   }
}