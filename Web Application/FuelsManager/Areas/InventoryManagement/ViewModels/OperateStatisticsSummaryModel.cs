using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    public class OperateStatisticsSummaryModel
    {
        public OperateStatisticsSummaryModel()
        {
        }

        public List<OperateStatisticsDetailModel> SessionDetails { get; set; } = new List<OperateStatisticsDetailModel>();
    }
}