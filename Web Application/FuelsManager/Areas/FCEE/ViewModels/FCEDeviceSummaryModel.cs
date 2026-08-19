using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ReportSvr2005;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelsManager.Areas.FCEE.ViewModels
{
    [Serializable]
    public class FCEDeviceSummaryModel
    {
        public List<FCEDevice> fceDevices { get; set; }
        public MvcHtmlString GuideOpenerScript { get; set; }
        public bool ReadOnly { get; set; }

        public FCEDeviceSummaryModel()
        {
            this.fceDevices = new List<FCEDevice>();
        }
        public FCEDeviceSummaryModel(List<FCEDevice> fceDevices)
        {
            this.fceDevices = fceDevices;
        }
    }
}