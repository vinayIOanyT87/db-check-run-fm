using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManagerService.PointGroupReport.FilterRules
{
    public class PointGroupEnumFilterRules
    {
        public string Type { get; set; }
        public List<string> Value { get; set; }
        public string Description { get; set; }

        public bool IsFiltered(object value, string valueTypeString)
        {
            var isFiltered = false;

            try
            {
                if (valueTypeString.StartsWith("FMBusinessObjects.DataObjects.CodedVariables"))
                {
                    if (value != null)
                    {
                        isFiltered = !Value.Contains(value.ToString());
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
