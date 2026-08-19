using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManagerService.PointGroupReport.FilterRules
{
    public class PointGroupBooleanFilterRules
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public bool IsFiltered(object value, string valueTypeString)
        {
            var isFiltered = false;

            try
            {
                if (valueTypeString == "System.Boolean")
                {
                    if (value != null)
                    {
                        var boolValue = bool.Parse(value.ToString());
                        var testValue = bool.Parse(Value);
                        isFiltered = boolValue != testValue;
                    }
                    else
                    {
                        isFiltered = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isFiltered = true;
            }

            return isFiltered;
        }
    }
}
