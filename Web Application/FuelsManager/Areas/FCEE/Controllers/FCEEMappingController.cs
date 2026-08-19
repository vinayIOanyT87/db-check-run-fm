
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
 
   public class FCEEMappingController : FMBaseControllerEx
   {
      // GET:
      public ActionResult FCEEMappingView()
      {
         var fceeMappingDict = FMChannelHelper.MakeCall<IFCEEServiceManager, Dictionary<Guid, FCEEMappingWithDevice>>(x => x.EnumerateBySiteGuidWithDevice(this.Security, this.Security.SiteGuid));
         var fceDevices = FMChannelHelper.MakeCall<IFCEDevices, List<FCEDevice>>(x => x.EnumerateBySiteGuid(this.Security, this.Security.SiteGuid));

         List<SelectListItem> fceDevicesList = new List<SelectListItem>(Enumerable.Empty<SelectListItem>());
         foreach (var fceDevice in fceDevices)
         {
            fceDevicesList.Add(new SelectListItem
            {
               Value = fceDevice.FCEDeviceGuid.ToString(),
               Text = fceDevice.FriendlyName + "(" + fceDevice.ImeiNumber + ")"
            });
         }

         var points = FMChannelHelper.MakeCall<IPoints, List<Point>>(
                     x => x.EnumerateForSummaryWithCategories(this.Security, this.Security.SiteGuid, includeDictionaries: false));

         var model = new FCEEMappingModel(this.Security.SiteGuid, fceeMappingDict, fceDevicesList);
         model.Points = points;
         var menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;
         string js = menuData.GetHelpUrl(true) + "CustomModuleProgrammersGuide.pdf";
         string jscript = "window.open('" + HttpUtility.JavaScriptStringEncode(js) + "')";

         model.ReadOnly = !this.Security.HasRight(RIGHT.MODIFY_FCEE_DATA);

         return this.View(model);
      }

      /// <summary>
      /// This method will handle the delete action.
      /// </summary>
      /// <param name="id">The ID of the item to delete.</param>
      /// <returns>Returns the view.</returns>
      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult DeleteFCEEMapping(string id)
      {
         try
         {
            Guid fCEEMappingGuid = Guid.Empty;
            if (Guid.TryParse(id, out fCEEMappingGuid) == false)
            {
               return this.JsonWithErrorMessages("Invalid mapping identifier.");
            }

            //get previous mapping
            var previousFCEEMapping = FMChannelHelper.MakeCall<IFCEEServiceManager, FCEEMapping>(x => x.Get(this.Security, fCEEMappingGuid));
           
            Point point = null;
            if(previousFCEEMapping.PointGuid.HasValue)
            {
                point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, previousFCEEMapping.PointGuid.Value));
                if (point == null)
                {
                   return this.JsonWithErrorMessages("Invalid point.");
                }
            }
            else
            {
                return this.JsonWithErrorMessages("Invalid point.");
            }

            string[] emptytagIDs = { };
            UpdatePointTags(null, emptytagIDs, point, GetPointTagIDs(previousFCEEMapping));

            var mappingGuid = new Guid(id);
            FMChannelHelper.MakeCall<IFCEEServiceManager>(x => x.Purge(this.Security, mappingGuid));
         }
         catch (Exception except)
         {
            this.OnError(except);
         }
         return this.JsonWithErrorMessages(null);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult EditFCEEMapping(string id, string pointId, string imei, int? msg, int? index, int? device, int? tagSelection = 0)
      {
         FCEEMappingModel fceeMappingModel = new FCEEMappingModel();
         fceeMappingModel.PointId = pointId;
         fceeMappingModel.Imei = imei;
         fceeMappingModel.Index = index;
         fceeMappingModel.Msg = msg;
         fceeMappingModel.Device = device;
         fceeMappingModel.TagSelection = tagSelection;

         if (!TryValidateModel(fceeMappingModel))
         {
            return this.JsonWithErrorMessages(null);
         }
         Guid pointGuid = Guid.Empty;
         Guid deviceGuid = Guid.Empty;
         string errmsg = string.Empty;
         var fceemapping = new FCEEMapping();


         if (string.IsNullOrWhiteSpace(pointId) || Guid.TryParse(pointId, out pointGuid) == false)
         {
            errmsg = string.Format("Invalid {0}.", FMBaseController.TranslateText("Point"));
         }
         else if (string.IsNullOrWhiteSpace(imei) || Guid.TryParse(imei, out deviceGuid) == false)
         {
            errmsg = string.Format("Invalid {0}.", FMBaseController.TranslateText("FCE Device"));
         }
         else if (msg == null || msg.HasValue == false)
         {
            errmsg = string.Format("A valid {0} must be specified.", FMBaseController.TranslateText("Message Type"));
         }
         else if (index == null || index.HasValue == false)
         {
            errmsg = string.Format("{0} must be specified.", FMBaseController.TranslateText("Index"));
         }
         else
         {
            try
            {
               fceemapping.MsgType = (EDGEMESSAGETYPE)msg.Value;
            }
            catch
            {
               errmsg = string.Format("Invalid {0}.", FMBaseController.TranslateText("Message Type"));
            }

         }
         if (string.IsNullOrWhiteSpace(errmsg) == false)
         {
            this.OnError(errmsg);
            JsonResult result = this.JsonWithErrorMessages(errmsg);
            return result;
         }

         try
         {
            var fceDevice = FMChannelHelper.MakeCall<IFCEDevices, FCEDevice>(x => x.Get(this.Security, deviceGuid));

            if (fceDevice == null)
            {
               errmsg = "Invalid device.";
               this.OnError(errmsg);
               JsonResult result = this.JsonWithErrorMessages(errmsg);
               return result;
            }
            var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, pointGuid));
            if (point == null)
            {
               errmsg = "Invalid point.";
               this.OnError(errmsg);
               JsonResult result = this.JsonWithErrorMessages(errmsg);
               return result;

            }
            fceemapping.PointGuid = pointGuid;
            fceemapping.FCEDeviceGuid = deviceGuid;
            fceemapping.Index = index.Value;
            fceemapping.Device = device;
            fceemapping.TagSelection = (TAGSELECTIONTYPE)tagSelection;


            if (string.IsNullOrWhiteSpace(id))
            { // new mapping
               var fceeMappingList = FMChannelHelper.MakeCall<IFCEEServiceManager, Dictionary<Guid, FCEEMapping>>(x => x.EnumerateByPointGuid(this.Security, pointGuid)).Values.ToList();
               if (fceeMappingList != null && fceeMappingList.Count > 0 &&

                   fceeMappingList.Exists(m2 => {
                     return (m2.FCEEMappingGuid != fceemapping.FCEEMappingGuid
                        && m2.Index == fceemapping.Index
                        && m2.PointGuid == fceemapping.PointGuid
                        && m2.MsgType == fceemapping.MsgType
                        && m2.FCEDeviceGuid == fceemapping.FCEDeviceGuid
                        && ((m2.Device == null && fceemapping.Device == null) || (m2.Device != null && fceemapping.Device != null && m2.Device.Value == fceemapping.Device.Value)));
              
               }))
               {
                  this.OnError("FCEE mapping already exist.");
                  return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
               }
               FMChannelHelper.MakeCall<IFCEEServiceManager>(x => x.Add(this.Security, fceemapping));
               string[] tagIDs = { };
               if (fceemapping.MsgType == EDGEMESSAGETYPE.GenericScalingPoint)
               {
                    if (tagSelection != null && tagSelection > 0)
                    {
                        string tagName = FCEEMapping.GetTagSelectionTypeTagName((TAGSELECTIONTYPE)tagSelection);

                        if (string.IsNullOrWhiteSpace(tagName) == false)
                        {
                            tagIDs = new string[] { tagName };
                        }
                    }
               }
               else
               {
                    tagIDs = FCEEMapper.MessageTypeToTagIDs(fceemapping.MsgType);
               }
               string[] emptyPreviousTagIDs = { };
               UpdatePointTags(point, tagIDs, null, emptyPreviousTagIDs);
            }
            else
            { // edit mapping
               Guid fCEEMappingGuid = Guid.Empty;
               if (Guid.TryParse(id, out fCEEMappingGuid) == false)
               {
                  return this.JsonWithErrorMessages("Invalid mapping identifier.");
               }

               //get previous mapping
               var previousFCEEMapping = FMChannelHelper.MakeCall<IFCEEServiceManager, FCEEMapping>(x => x.Get(this.Security, fCEEMappingGuid));

               Point previousPoint = null;
               if (previousFCEEMapping.PointGuid.HasValue)
               {
                   previousPoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, previousFCEEMapping.PointGuid.Value));
                   if (previousPoint == null)
                   {
                       return this.JsonWithErrorMessages("Invalid point.");
                   }
               }
               else
               {
                   return this.JsonWithErrorMessages("Invalid point.");
               }

               fceemapping.FCEEMappingGuid = fCEEMappingGuid;
               FMChannelHelper.MakeCall<IFCEEServiceManager>(x => x.Modify(this.Security, fceemapping));

               UpdatePointTags(point, GetPointTagIDs(fceemapping), previousPoint, GetPointTagIDs(previousFCEEMapping));
            }
         }
         catch (Exception except)
         {
            this.OnError(except);
         }
         return this.JsonWithErrorMessages(null);
      }

        private void UpdatePointTags(Point point, string[] tagIDs, Point previousPoint, string[] previousTagIDs)
        {
            if(previousPoint != null) { 
                foreach (var tagID in previousTagIDs)
                {
                    var tagKeyValuePair = previousPoint.Tags.FirstOrDefault(x => x.Value.ID.Equals(tagID, StringComparison.OrdinalIgnoreCase));
                    if (tagKeyValuePair.Value != null)
                    {
                        PointTag tag = tagKeyValuePair.Value as PointTag;
                        if(point == null || point.PointGuid != previousPoint.PointGuid || !tagIDs.Contains(tagID)) 
                        { 
                            tag.InputOutputType = tag.LastInputOutputType;
                            tag.LastInputOutputType = PointTemplateTag.PointTagInputOutputType.UnAssigned;
                            FMChannelHelper.MakeCall<IPointTags>(x => x.Modify(this.Security, tag));
                        }
                    }
                }
            }

            if(point != null) { 
                foreach (var tagID in tagIDs)
                {
                    var tagKeyValuePair = point.Tags.FirstOrDefault(x => x.Value.ID.Equals(tagID, StringComparison.OrdinalIgnoreCase));
                    if (tagKeyValuePair.Value != null)
                    {
                        PointTag tag = tagKeyValuePair.Value as PointTag;
                        if (previousPoint == null || point.PointGuid != previousPoint.PointGuid || !previousTagIDs.Contains(tagID))
                        {
                            tag.LastInputOutputType = tag.InputOutputType; 
                            tag.InputOutputType = PointTemplateTag.PointTagInputOutputType.FCEE;
                            FMChannelHelper.MakeCall<IPointTags>(x => x.Modify(this.Security, tag));
                        }
                    }
                }
            }
        }

        private string[] GetPointTagIDs(FCEEMapping mapping)
        {
            string[] tagIDs = { };
            if ((EDGEMESSAGETYPE)mapping.MsgType == EDGEMESSAGETYPE.GenericScalingPoint)
            {
                if (mapping.TagSelection > 0)
                {
                    string tagName = FCEEMapping.GetTagSelectionTypeTagName((TAGSELECTIONTYPE)mapping.TagSelection);

                    if (string.IsNullOrWhiteSpace(tagName) == false)
                    {
                        tagIDs = new string[] { tagName };
                    }
                }
            }
            else
            {
                tagIDs = FCEEMapper.MessageTypeToTagIDs(mapping.MsgType);
            }
            return tagIDs;
        }

        [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult AddFCEEMapping(string id)
      {
         try
         {
            var mappingGuid = new Guid(id);
            var fceemapping = new FCEEMapping();
            FMChannelHelper.MakeCall<IFCEEServiceManager>(x => x.Add(this.Security, fceemapping));
         }
         catch (Exception except)
         {
            this.OnError(except);
         }
         return this.JsonWithErrorMessages(null);
      }
   }
}