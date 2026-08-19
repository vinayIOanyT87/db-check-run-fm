using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManagerService.PointGroupReport.FilterRules
{
    public class PointGroupNumericFilterRules
    {
        public string Type { get; set; }
        public int Unit { get; set; }
        public string Operator { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string Description { get; set; }

        public bool IsFiltered(object value, string valueTypeString)
        {
            var isFiltered = false;

            try
            {
                if (value != null)
                {
                    if (valueTypeString == "System.Double" || valueTypeString == "System.Single")
                    {
                        var doubleValue = (double)value;
                        var minValue = double.Parse(MinValue);
                        double maxValue;
                        switch (Operator)
                        {
                            case "equal":
                                isFiltered = doubleValue != minValue;
                                break;
                            case "not_equal":
                                isFiltered = doubleValue == minValue;
                                break;
                            case "greater":
                                isFiltered = doubleValue <= minValue;
                                break;
                            case "greater_equal":
                                isFiltered = doubleValue < minValue;
                                break;
                            case "less":
                                isFiltered = doubleValue >= minValue;
                                break;
                            case "less_equal":
                                isFiltered = doubleValue > minValue;
                                break;
                            case "between":
                                maxValue = double.Parse(MaxValue);
                                isFiltered = doubleValue < minValue || doubleValue > maxValue;
                                break;
                            case "not_between":
                                maxValue = double.Parse(MaxValue);
                                isFiltered = doubleValue >= minValue && doubleValue <= maxValue;
                                break;
                        }
                    }
                    else if (valueTypeString == "System.Int16" || valueTypeString == "System.Int32" || valueTypeString == "System.Int64")
                    {
                        var longValue = (long)value;
                        var minValue = long.Parse(MinValue);
                        double maxValue;
                        switch (Operator)
                        {
                            case "equal":
                                isFiltered = longValue != minValue;
                                break;
                            case "not_equal":
                                isFiltered = longValue == minValue;
                                break;
                            case "greater":
                                isFiltered = longValue <= minValue;
                                break;
                            case "greater_equal":
                                isFiltered = longValue < minValue;
                                break;
                            case "less":
                                isFiltered = longValue >= minValue;
                                break;
                            case "less_equal":
                                isFiltered = longValue > minValue;
                                break;
                            case "between":
                                maxValue = long.Parse(MaxValue);
                                isFiltered = longValue < minValue || longValue > maxValue;
                                break;
                            case "not_between":
                                maxValue = long.Parse(MaxValue);
                                isFiltered = longValue >= minValue && longValue <= maxValue;
                                break;
                        }
                    }
                    else if (valueTypeString == "System.UInt16" || valueTypeString == "System.UInt32" || valueTypeString == "System.UInt64")
                    {
                        var uLongValue = (ulong)value;
                        var minValue = ulong.Parse(MinValue);
                        double maxValue;
                        switch (Operator)
                        {
                            case "equal":
                                isFiltered = uLongValue != minValue;
                                break;
                            case "not_equal":
                                isFiltered = uLongValue == minValue;
                                break;
                            case "greater":
                                isFiltered = uLongValue <= minValue;
                                break;
                            case "greater_equal":
                                isFiltered = uLongValue < minValue;
                                break;
                            case "less":
                                isFiltered = uLongValue >= minValue;
                                break;
                            case "less_equal":
                                isFiltered = uLongValue > minValue;
                                break;
                            case "between":
                                maxValue = ulong.Parse(MaxValue);
                                isFiltered = uLongValue < minValue || uLongValue > maxValue;
                                break;
                            case "not_between":
                                maxValue = ulong.Parse(MaxValue);
                                isFiltered = uLongValue >= minValue && uLongValue <= maxValue;
                                break;
                        }
                    }
                }
                else
                {
                    isFiltered = !(Operator == "not_equal" || Operator == "not_between");
                }
            }
            catch (Exception ex)
            {
                isFiltered = !(Operator == "not_equal" || Operator == "not_between");
            }

            return isFiltered;
        }
    }
}
