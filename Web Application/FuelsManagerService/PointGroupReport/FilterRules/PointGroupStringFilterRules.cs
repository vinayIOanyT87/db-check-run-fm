using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManagerService.PointGroupReport.FilterRules
{
    public class PointGroupStringFilterRules
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public bool IsFiltered(object value, string valueTypeString)
        {
            var isFiltered = false;

            try
            {
                if (valueTypeString == "System.String")
                {
                    if (!string.IsNullOrEmpty(Value))
                    {
                        var stringValue = (string)value;
                        isFiltered = !stringValue.Contains(Value);
                    }
                    else
                    {
                        isFiltered = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isFiltered = false;
            }

            return isFiltered;
        }
    }
}
