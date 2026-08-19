using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    public enum ChangedDataSet
    {
        None = 0,
        Start = 1,
        End = 2,
        Diff = 3,
    }

    public class PointCalculatorData
    {
        public List<PointTag> StartTags { get; set; } = null;
        public List<PointTag> EndTags { get; set; } = null;
        public List<PointTag> DiffTags { get; set; } = null;
        public bool IsBatchMode { get; set; } = true;
        public BatchModeKey BatchModeKey { get; set; } = BatchModeKey.None;
        public ChangedDataSet ChangedDataSet { get; set; } = ChangedDataSet.None;
        public string ChangedPointTagId { get; set; } = string.Empty;
    }
}
