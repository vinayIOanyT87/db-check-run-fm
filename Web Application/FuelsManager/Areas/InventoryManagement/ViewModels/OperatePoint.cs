using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using FMBusinessObjects.DataObjects;

    [Serializable]
    public class OperatePoint
    {
        public OperatePoint()
        {
            this.Values = new List<PointValue>();
        }
        public string PointID { get; set; }
        public Guid PointGuid { get; set; }
        public Guid PointTemplateGuid { get; set; }
        public Guid? PointDetailDrawingGuid { get; set; }
        public List<PointValue> Values { get; set; }
    }
}