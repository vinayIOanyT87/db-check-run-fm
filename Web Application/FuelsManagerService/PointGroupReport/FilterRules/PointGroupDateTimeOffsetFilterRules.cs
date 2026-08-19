using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManagerService.PointGroupReport.FilterRules
{
    public class PointGroupDateTimeOffsetFilterRules
    {
        public string Type { get; set; }
        public string Operator { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string Description { get; set; }

        public bool IsFiltered(object value, string valueTypeString)
        {
            var isFiltered = false;

            try
            {
                if (valueTypeString == "System.DateTimeOffset")
                {
                    if (value != null)
                    {
                        var dateTimeOffsetValue = (DateTimeOffset)value;
                        MinValue += " " + dateTimeOffsetValue.ToString("+00:00");
                        var minValue = DateTimeOffset.Parse(MinValue);
                        DateTimeOffset maxValue;
                        switch (Operator)
                        {
                            case "equal":
                                isFiltered = dateTimeOffsetValue != minValue;
                                break;
                            case "not_equal":
                                isFiltered = dateTimeOffsetValue == minValue;
                                break;
                            case "greater":
                                isFiltered = dateTimeOffsetValue <= minValue;
                                break;
                            case "greater_equal":
                                isFiltered = dateTimeOffsetValue < minValue;
                                break;
                            case "less":
                                isFiltered = dateTimeOffsetValue >= minValue;
                                break;
                            case "less_equal":
                                isFiltered = dateTimeOffsetValue > minValue;
                                break;
                            case "between":
                                maxValue = DateTimeOffset.Parse(MaxValue);
                                isFiltered = dateTimeOffsetValue < minValue || dateTimeOffsetValue > maxValue;
                                break;
                            case "not_between":
                                maxValue = DateTimeOffset.Parse(MaxValue);
                                isFiltered = dateTimeOffsetValue >= minValue && dateTimeOffsetValue <= maxValue;
                                break;
                        }
                    }
                    else
                    {
                        isFiltered = !(Operator == "not_equal" || Operator == "not_between");
                    }
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
