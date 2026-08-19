using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using FuelsManager.Areas.Controllers;

namespace FuelsManager.Areas.FCEE.ViewModels
{
    [Serializable]
    public class FCEEMappingModel : IValidatableObject
    {
        public Dictionary<Guid, FCEEMappingWithDevice> FCEEMappingList;
        public List<SelectListItem> FCEDevices;
        public List<Point> Points;
        public Guid SiteGuid;
        public bool ReadOnly;

        private string pointId;
        private string imei;
        private int? msg;
        private int? index;
        private int? device;
        private int? tagSelection;

        [MinLength(36, ErrorMessage = "Invalid Point.")]
        [MaxLength(36, ErrorMessage = "Invalid Point.")]
        [Required]
        public string PointId
        {
            get
            {
                return pointId;
            }
            set
            {
                pointId = value;
            }
        }

        [MinLength(36, ErrorMessage = "Invalid FCE Device.")]
        [MaxLength(36, ErrorMessage = "Invalid FCE Device.")]
        [Required]
        public string Imei
        {
            get
            {
                return imei;
            }
            set
            {
                imei = value;
            }
        }
        [Required]
        public int? Msg
        {
            get
            {
                return msg;
            }
            set
            {
                msg = value;
            }
        }
        [Required]
        [Range(0, 255)]
        public int? Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        [Range(1, 9)]
        public int? Device
        {
            get
            {
                return device;
            }
            set
            {
                device = value;
            }
        }

        [Range(0, 3)]
        public int? TagSelection
        {
            get
            {
                return tagSelection;
            }
            set
            {
                tagSelection = value;
            }
        }
        public FCEEMappingModel()
        {
            this.FCEEMappingList = new Dictionary<Guid, FCEEMappingWithDevice>();
        }
        public FCEEMappingModel(Guid siteGuid, Dictionary<Guid, FCEEMappingWithDevice> fceeMappingList, List<SelectListItem> fceDevices)
        {
            this.SiteGuid = siteGuid;
            this.FCEEMappingList = fceeMappingList;
            this.FCEDevices = fceDevices;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (msg == null || msg < 1 || msg > 21)
            {
                yield return new ValidationResult("Invalid message type.\n");
            }
            if (index == null)// || index < 0 || index > 255)
            {
                yield return new ValidationResult("Invalid Index value.\n");
            }
            switch (msg.Value)
            {
                case 1:
                case 2:
                    if (index != 0)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0\n");

                    }
                    break;

                case 3:
                    if (index < 0 || index > 16)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-16\n");

                    }

                    break;
                case 4:
                    if (index < 0 || index > 119)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-119\n");

                    }
                    break;
                case 5:
                    if (index < 0 || index > 119)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-119\n");

                    }
                    break;
                case 6:
                    if (index < 0 || index > 31)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-31\n");

                    }
                    break;
                case 7:
                    if (index < 0 || index > 95)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-95\n");

                    }
                    break;
                case 8:
                    if (index < 0 || index > 127)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-127\n");

                    }
                    break;
                case 9:
                    if (index < 0 || index > 31)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-31\n");

                    }
                    break;
                case 10:
                    if (index < 0 || index > 31)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-31\n");

                    }
                    break;
                case 11:
                    if (index < 0 || index > 31)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-31\n");

                    }
                    break;
                case 12:
                    if (index < 0 || index > 3)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-3\n");

                    }
                    break;
                case 13:
                    if (index < 1 || index > 16)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 1-16\n");

                    }
                    break;
                case 14:
                    if (index < 1 || index > 16)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 1-16\n");

                    }
                    break;
                case 15:
                    if (index < 1 || index > 16)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 1-16\n");

                    }
                    break;
                case 16:
                    if (index < 1 || index > 11)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 1-11\n");

                    }
                    if (device == null || device.Value < 1 || device.Value > 9)
                    {
                        yield return new ValidationResult("Invalid device number. Valid values are between 1 and 9.\n");
                    }
                    break;
                case 17:
                    if (index < 1 || index > 11)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 1-11\n");

                    }
                    if (device == null || device.Value < 1 || device.Value > 9)
                    {
                        yield return new ValidationResult("Invalid device number. Valid values are between 1 and 9.\n");
                    }
                    break;
                case 18:
                    if (index < 1 || index > 11)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 1-11\n");

                    }
                    break;
                case 19:
                    if (index < 1 || index > 2)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 1-2\n");

                    }
                    if (device == null || device.Value < 1 || device.Value > 2)
                    {
                        yield return new ValidationResult("Invalid device number. Valid values are between 1 and 2.\n");
                    }
                    break;
                case 20:
                    if (index < 0 || index > 119)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-119\n");

                    }
                    break;
                case 21:
                    if (index < 0 || index > 48)
                    {
                        yield return new ValidationResult("Invalid Index value. Valid values: 0-48\n");

                    }
                    break;
                default:
                    break;

            }



        }
    }
}