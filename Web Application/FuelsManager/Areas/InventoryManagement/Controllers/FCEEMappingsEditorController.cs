
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

   using FMBusinessObjects.Constants;

   using FuelsManager.FMWebApp;

   using global::FMWebApp;
   using System.Runtime.Serialization;
   using FuelsManager.Areas.InventoryManagement.ViewModels;

    public class FCEEMappingsEditorController : FMBaseControllerEx
   {
      private bool CanModifyFCEEData()
      {
         return this.Security.HasRight(RIGHT.MODIFY_FCEE_DATA);
      }

      private JsonResult FCEEMappingModificationNotAllowed()
      {
         this.OnError(this.GetTranslatedText("User does not have right (Modify FCEE Data)."));
         return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
      }

      // GET: 
      [HttpGet, ValidateJsonAntiForgeryToken]
      public ActionResult FCEEMappingsEditor(Guid pointGuid)
      {
         try
         {
            var fceeMappingList = FMChannelHelper.MakeCall<IFCEEServiceManager, Dictionary<Guid, FCEEMapping>>(x => x.EnumerateByPointGuid(this.Security, pointGuid)).Values.ToList();
            var fceDevices = FMChannelHelper.MakeCall<IFCEDevices, List<FCEDevice>>(x => x.EnumerateBySiteGuid(this.Security, this.Security.SiteGuid));
            List<SelectListItem> fceDevicesSelectList = new List<SelectListItem>(Enumerable.Empty<SelectListItem>());
            foreach (var fceDevice in fceDevices)
            {
               fceDevicesSelectList.Add(new SelectListItem
               {
                  Value = fceDevice.FCEDeviceGuid.ToString(),
                  Text = fceDevice.FriendlyName + "(" + fceDevice.ImeiNumber + ")"
               });
            }
            var model = new FCEEMappingsEditorModel(fceeMappingList, false, pointGuid, new SelectList(fceDevicesSelectList, "Value", "Text"));
            var menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;
            string js = menuData.GetHelpUrl(true) + "CustomModuleProgrammersGuide.pdf";
            string jscript = "window.open('" + HttpUtility.JavaScriptStringEncode(js) + "')";
            model.GuideOpenerScript = new MvcHtmlString(jscript);

            model.ReadOnly = !this.CanModifyFCEEData();

            return this.PartialViewWithErrorMessages("FCEEMappingsEditor", model, JsonRequestBehavior.AllowGet);
         }
         catch (Exception ex)
         {
            string msg = ex.Message;
            this.OnError(new Exception(this.GetTranslatedText("Error Getting FCEE Mappings")));
            return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult SaveFCEEMappings(FCEEMappingsEditorModel model)
      {
         if (!this.CanModifyFCEEData())
         {
            return this.FCEEMappingModificationNotAllowed();
         }

         if (ModelState.IsValid == false)
         {
            JsonResult res = this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            return res;

         }
         PointTagCollection pointTags = new PointTagCollection();
         if (model.FCEEMappings != null)
         {
            if (model.FCEEMappings.Exists(m1=> { 
               return model.FCEEMappings.Exists(m2 => {
                  return (m2.FCEEMappingGuid != m1.FCEEMappingGuid
                     && m2.Index == m1.Index
                     && m2.PointGuid == m1.PointGuid
                     && m2.MsgType == m1.MsgType
                     && m2.FCEDeviceGuid == m1.FCEDeviceGuid
                     && ((m2.Device == null && m1.Device == null) || (m2.Device != null && m1.Device != null && m2.Device.Value == m1.Device.Value)));


               }); 
            }))
            {
               this.OnError("Duplicate mappings found.");
               return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);

            }

            if (model.FCEEMappings.Exists(m => {
                return ((int)m.MsgType != 18 && (int)m.MsgType >= 16 && (int)m.MsgType <= 19 && ( m.Device == null || m.Device.Value < 1 || m.Device.Value> 9));
            }))
            {
                this.OnError("Invalid device number. Valid values are between 1 and 9.");
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);

            }
            Point point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, model.PointGuid));
            var prevFceeMappingList = FMChannelHelper.MakeCall<IFCEEServiceManager, Dictionary<Guid, FCEEMapping>>(x => x.EnumerateByPointGuid(this.Security, model.PointGuid)).Values.ToList();

            foreach (var newFceeMapping in model.FCEEMappings)
            {
                FCEEMapping oldFceeMapping = prevFceeMappingList.FindByGuid(newFceeMapping.FCEEMappingGuid);
                if (oldFceeMapping == null)
                {
                    //User newly added this fcee mapping item (newFceeMapping)
                    string[] tagIDs = { };
                    if (newFceeMapping.MsgType == EDGEMESSAGETYPE.GenericScalingPoint)
                    {
                        if (newFceeMapping.TagSelection != TAGSELECTIONTYPE.None)
                        {
                            //new mapping with a tag selection
                            string tagName = FCEEMapping.GetTagSelectionTypeTagName(newFceeMapping.TagSelection);

                            if (string.IsNullOrWhiteSpace(tagName) == false)
                            {
                                tagIDs = new string[] { tagName };
                            }
                        }
                    }
                    else
                    {
                        tagIDs = FCEEMapper.MessageTypeToTagIDs(newFceeMapping.MsgType);
                    }
                    string[] emptytagIDs = { };
                    UpdatePointTags(point, tagIDs, emptytagIDs, pointTags);
                }
                        
            }
            
            foreach (var oldFceeMapping in prevFceeMappingList)
            {
               FCEEMapping newFceeMapping = model.FCEEMappings.FindByGuid(oldFceeMapping.FCEEMappingGuid);
               if (newFceeMapping == null)
               {
                  //User deleted this item in UI
                  string[] emptytagIDs = { };
                  UpdatePointTags(point, emptytagIDs, GetPointTagIDs(oldFceeMapping), pointTags);
                  FMChannelHelper.MakeCall<IFCEEServiceManager>(x => x.Purge(this.Security, oldFceeMapping.FCEEMappingGuid));
               }
               else 
               {
                    if (newFceeMapping.MsgType != oldFceeMapping.MsgType || newFceeMapping.TagSelection != oldFceeMapping.TagSelection)
                    {
                        //user edited this fcce mapping item.
                        UpdatePointTags(point, GetPointTagIDs(newFceeMapping), GetPointTagIDs(oldFceeMapping), pointTags);
                    }
               }
            }
            if (pointTags.Count > 0)
            {
                FMChannelHelper.MakeCall<IPointTags>(x => { foreach (var tag in pointTags) { x.Modify(this.Security, tag); } });
            }

            FMChannelHelper.MakeCall<IFCEEServiceManager>(x => x.UpdateFCEEMappings(this.Security, model.FCEEMappings));
            this.AddSuccess("Save Successful");
            return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }

         else
         {
            this.AddSuccess("Nothing to save");
            return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }
      }

        private void UpdatePointTags(Point point, string[] tagIDs, string[] previousTagIDs, PointTagCollection pointTags)
        {
                foreach (var tagID in previousTagIDs)
                {
                    var tagKeyValuePair = point.Tags.FirstOrDefault(x => x.Value.ID.Equals(tagID, StringComparison.OrdinalIgnoreCase));
                    if (tagKeyValuePair.Value != null)
                    {
                        PointTag tag = tagKeyValuePair.Value as PointTag;
                        if (!tagIDs.Contains(tagID))
                        {
                            tag.InputOutputType = tag.LastInputOutputType;
                            tag.LastInputOutputType = PointTemplateTag.PointTagInputOutputType.UnAssigned;
                            FMChannelHelper.MakeCall<IPointTags>(x => x.Modify(this.Security, tag));
                        }
                    }
                }

                foreach (var tagID in tagIDs)
                {
                    var tagKeyValuePair = point.Tags.FirstOrDefault(x => x.Value.ID.Equals(tagID, StringComparison.OrdinalIgnoreCase));
                    if (tagKeyValuePair.Value != null)
                    {
                        PointTag tag = tagKeyValuePair.Value as PointTag;
                        if (!previousTagIDs.Contains(tagID))
                        {
                            tag.LastInputOutputType = tag.InputOutputType;
                            tag.InputOutputType = PointTemplateTag.PointTagInputOutputType.FCEE;
                            FMChannelHelper.MakeCall<IPointTags>(x => x.Modify(this.Security, tag));
                        }
                    }
                }

        }

        private string[] GetPointTagIDs (FCEEMapping mapping) {
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

        [NonAction]
      public static IEnumerable<SelectListItem> GetMsgTypes()
      {
         return GetEnumSelectList<EDGEMESSAGETYPE>().ToList().GetRange(0, 22); 
      }

      public static IEnumerable<SelectListItem> TagSelectionTypes()
        {
            return GetEnumSelectList<TAGSELECTIONTYPE>().ToList().GetRange(0, 4);
        }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult AddFCEEMapping(FCEEMappingsEditorModel model)
      {
         if (!this.CanModifyFCEEData())
         {
            return this.FCEEMappingModificationNotAllowed();
         }

         FCEEMapping fceeMapping = new FCEEMapping() { IdentityGuid = Guid.NewGuid() };
         fceeMapping.PointGuid = model.PointGuid;
         fceeMapping.Index = 0;
         fceeMapping.FCEDeviceGuid = null;

         var fceeMappingList = model.FCEEMappings;
         var fceDevices = FMChannelHelper.MakeCall<IFCEDevices, List<FCEDevice>>(x => x.EnumerateBySiteGuid(this.Security, this.Security.SiteGuid));
         fceeMappingList.Add(fceeMapping);
         List<SelectListItem> fceDevicesSelectList = new List<SelectListItem>(Enumerable.Empty<SelectListItem>());
         foreach (var fceDevice in fceDevices)
         {
            fceDevicesSelectList.Add(new SelectListItem
            {
               Value = fceDevice.FCEDeviceGuid.ToString(),
               Text = fceDevice.FriendlyName + "(" + fceDevice.ImeiNumber + ")"
            });
         }
         model.FCEDevicesSelectList = new SelectList(fceDevicesSelectList, "Value", "Text");
         model.FCEDevices = fceDevices;
         model.ReadOnly = false;
         this.AddSuccess("Save Successful");
         ModelState.Clear();
         return this.PartialViewWithErrorMessages("FCEEMappingsEditor", model, JsonRequestBehavior.AllowGet);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult DeleteFCEEMapping(FCEEMappingsEditorModel model, Guid fceeMappingGuid)
      {
         if (!this.CanModifyFCEEData())
         {
            return this.FCEEMappingModificationNotAllowed();
         }

         var fceDevices = FMChannelHelper.MakeCall<IFCEDevices, List<FCEDevice>>(x => x.EnumerateBySiteGuid(this.Security, this.Security.SiteGuid));
         List<SelectListItem> fceDevicesSelectList = new List<SelectListItem>(Enumerable.Empty<SelectListItem>());
         foreach (var fceDevice in fceDevices)
         {
            fceDevicesSelectList.Add(new SelectListItem
            {
               Value = fceDevice.FCEDeviceGuid.ToString(),
               Text = fceDevice.FriendlyName + "(" + fceDevice.ImeiNumber + ")"
            });
         }
         FCEEMapping fCEEMapping = model.FCEEMappings.FindByGuid(fceeMappingGuid);
         if (fCEEMapping != null)
         {
            model.FCEEMappings.Remove(fCEEMapping);
         }
         model.FCEEMappings = model.FCEEMappings;
         model.ReadOnly = !this.CanModifyFCEEData();
         model.FCEDevicesSelectList = new SelectList(fceDevicesSelectList, "Value", "Text");
         ModelState.Clear();
         JsonResult returnVal = this.PartialViewWithErrorMessages("FCEEMappingsEditor", model, JsonRequestBehavior.AllowGet);
         return returnVal;
      }
   }
}
