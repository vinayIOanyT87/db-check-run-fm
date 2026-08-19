using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManagerService.PointGroupReport
{
    public class Row
    {
        public Row() { }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Point { get; set; }
        public string PointGuid { get; set; }
        public string TotalizerGuid { get; set; }
    }
}