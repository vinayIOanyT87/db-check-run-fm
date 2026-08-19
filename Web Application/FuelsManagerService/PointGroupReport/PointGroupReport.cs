using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FuelsManagerService.PointGroupReport
{
    public class PointGroupReport
    {
        public PointGroupReport(string columnDefinition, string rowDefinition)
        {
            Columns = JsonConvert.DeserializeObject<List<Column>>(columnDefinition);
            Rows = JsonConvert.DeserializeObject<List<Row>>(rowDefinition);
        }

        #region Properties
        public string PointGroupName { get; set; }
        public Guid PointGroupGuid { get; set; }
        public Guid PointGroupScheduleGuid { get; set; }
        public List<Row> Rows { get; set; }
        public List<Column> Columns { get; set; }
        public bool IsLandscape { get; set; }
        #endregion

        #region Methods
        #endregion
    }
}