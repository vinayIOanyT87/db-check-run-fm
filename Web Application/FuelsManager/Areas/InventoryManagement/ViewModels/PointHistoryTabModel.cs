using System;
using System.Collections.Generic;
using System.Linq;
using FMBusinessObjects.DataObjects;
using FuelsManager.Areas.Controllers;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    public enum PointHistoryInterval
    {
        Minute = 0,
        Hour = 1,
        Day = 2,
    }

    public enum PointHistoryRange
    {
        Hour = 0,
        Day = 1,
        Month = 2,
        Year = 3,
    }
    [Serializable]
    public class PointHistoryTabModel : FMBaseModel
    {
        public string TabId;
        public string ControlId;
        public Guid PointGuid;
        public string ID;
        public string Name;
        public string Start;
        public PointHistoryInterval Interval;
        public int IntervalQuantity;
        public PointHistoryRange Range;
        public int RangeQuantity;
        public string Columns;
        public int FontSize;

        public PointHistoryTabModel() {
            this.TabId = string.Empty;
            this.ControlId = string.Empty;
            this.PointGuid = Guid.Empty;
            this.ID = string.Empty;
            this.Name = string.Empty;
            this.Start = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            this.Interval = PointHistoryInterval.Hour;
            this.IntervalQuantity = 1;
            this.Range = PointHistoryRange.Day;
            this.RangeQuantity = 4;
            this.Columns = string.Empty;
            this.FontSize = 14;
        }

        public string IntervalString { get { return ((int)this.Interval).ToString(); } }
        public string RangeString { get { return ((int)this.Range).ToString(); } }
    }
}