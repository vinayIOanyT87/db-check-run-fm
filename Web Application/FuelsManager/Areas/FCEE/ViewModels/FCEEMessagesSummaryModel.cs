using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FuelsManager.Areas.FCEE.ViewModels
{
    [Serializable]
    public class FCEEMessagesSummaryModel : IValidatableObject
    {
        public int page { get; set; }
        public int pageMax { get; set; }
        public int? message { get; set; }
        public string imei { get; set; }
        public int? index { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public IEnumerable<FCEEMessage> displayedFceeMessages { get; set; }
        public IEnumerable<string> imeiList { get; set; }
        public List<FCEEMessage> fceeMessages { get; set; }
        public MvcHtmlString GuideOpenerScript { get; set; }        
        public FCEEMessagesSummaryModel()       
        {
            this.fceeMessages = new List<FCEEMessage>();
        }

        public FCEEMessagesSummaryModel(List<FCEEMessage> messages, int page)
        {
            this.fceeMessages = messages;
            this.imeiList = fceeMessages.Select(x => x.ImeiNumber).Distinct();
            this.page = page;
            this.pageMax = Math.Min((int) Math.Ceiling((double) messages.Count / 100), 10);
            this.message = null;
        }

        public void FCEEMessagesPage()
        {
            this.displayedFceeMessages = this.fceeMessages.Take(1000).Skip(100 * (this.page - 1)).Take(100);
        }

        public void FCEEMessagesFilterByType(int? message)
        {
            this.fceeMessages = this.fceeMessages.FindAll(x => (int) x.MsgType == message);
        }

        public void FCEEMessagesFilterByImei(string imei)
        {
            this.fceeMessages = this.fceeMessages.FindAll(x => x.ImeiNumber.CompareTo(imei) == 0);
        }

        public void FCEEMessagesFilterByIndex(int? index)
        {
            this.fceeMessages = this.fceeMessages.FindAll(x => x.Index == index);
        }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            DateTime d1;
            DateTime d2;
            if (DateTime.TryParse(startDate, out d1) && DateTime.TryParse(endDate, out d2))
            {
                if (d1 > d2)
                {
                    yield return new ValidationResult("End Date must not be before Start Date.");

                }
            }
        }
    }

}