namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Web;

   using FMBusinessObjects.DataObjects;
   using System.Web.Mvc;
   using Areas.Controllers;
   using Varec.CommonComponents.EngineeringUnitsLibrary;
   using System.ComponentModel.DataAnnotations;

   public class FCEEMappingsEditorModel : IValidatableObject
   {
      public List<FCEEMapping> FCEEMappings { get; set; }

      public List<FCEDevice> FCEDevices { get; set; }

      public SelectList FCEDevicesSelectList { get; set; }

      public Guid PointGuid { get; set; }

      public bool ReadOnly;
      public MvcHtmlString GuideOpenerScript { get; set; }
      public FCEEMappingsEditorModel()
      {
         this.FCEEMappings=new List<FCEEMapping>();
      }

      public FCEEMappingsEditorModel(List<FCEEMapping> fceeMappings, bool readOnly, Guid pointGuid, SelectList fceDevicesSelectList)
      {
         this.FCEEMappings = fceeMappings;
         if (this.FCEEMappings == null)
         {
            this.FCEEMappings = new List<FCEEMapping>();
         }
         this.ReadOnly = readOnly;
         this.PointGuid = pointGuid;
         this.FCEDevicesSelectList = fceDevicesSelectList;
      }

      public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
      {
         string err = string.Empty;
         int i = 0;
         if (FCEEMappings != null)
         {

            foreach (var fceeMapping in FCEEMappings)
            {
               i++;

               err = string.Empty;
               ValidationResult vr = Validate(i, fceeMapping.Index, fceeMapping.Device, fceeMapping.MsgType);

               if (vr != null)
               {
                  yield return vr;
               }
            }
         }
      }
        public ValidationResult Validate(int i, int? index, int? device, EDGEMESSAGETYPE msgType)
        {
            int msg = (int) msgType;
            if (index == null)// || index < 0 || index > 255)
            {
                return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value.\n");
            }
            switch (msg)
            {
                case 0:
                    return new ValidationResult($"FCEE Mappings item {i}: Invalid message type.\n");
                case 1:
                case 2:
                    if (index != 0)
                    {
                        return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0\n");

                    }
                    break;

                case 3:
                    if (index < 0 || index > 16)
                    {
                        return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-16\n");

                    }

                    break;
                case 4:
                    if (index < 0 || index > 119)
                    {
                        return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-119\n");

                    }
                    break;
                case 5:
                    if (index < 0 || index > 119)
                    {
                        return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-119\n");

                    }
                    break;
                case 6:
                    if (index < 0 || index > 31)
                    {
                        new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-31\n");

                    }
                    break;
                case 7:
                    if (index < 0 || index > 95)
                    {
                        return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-95\n");

                    }
                    break;
                case 8:
                    if (index < 0 || index > 127)
                    {
                        return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-127\n");

                    }
                    break;
                case 9:
                    if (index < 0 || index > 31)
                    {
                       return  new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-31\n");

                    }
                    break;
                case 10:
                    if (index < 0 || index > 31)
                    {
                       return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-31\n");

                    }
                    break;
                case 11:
                    if (index < 0 || index > 31)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-31\n");

                    }
                    break;
                case 12:
                    if (index < 0 || index > 3)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-3\n");

                    }
                    break;
                case 13:
                    if (index < 1 || index > 16)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 1-16\n");

                    }
                    break;
                case 14:
                    if (index < 1 || index > 16)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 1-16\n");

                    }
                    break;
                case 15:
                    if (index < 1 || index > 16)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 1-16\n");

                    }
                    break;
                case 16:
                    if (index < 1 || index > 11)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 1-11\n");

                    }
                    if (device == null || device.Value < 1 || device.Value > 9)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid device number. Valid values are between 1 and 9.\n");
                    }
                    break;
                case 17:
                    if (index < 1 || index > 11)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 1-11\n");

                    }
                    if (device == null || device.Value < 1 || device.Value > 9)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid device number. Valid values are between 1 and 9.\n");
                    }
                    break;
                case 18:
                    if (index < 1 || index > 11)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 1-11\n");

                    }
                    break;
                case 19:
                    if (index < 1 || index > 2)
                    {
                         return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 1-2\n");

                    }
                    if (device == null || device.Value < 1 || device.Value > 2)
                    {
                       return new ValidationResult($"FCEE Mappings item {i}: Invalid device number. Valid values are between 1 and 2.\n");
                    }
                    break;
                case 20:
                    if (index < 0 || index > 119)
                    {
                        return new ValidationResult($"FCEE Mappings item {i}: Invalid Index value. Valid values: 0-119\n");

                    }
                    break;
                default:
                    break;

            }
            return null;


        }


    }
}