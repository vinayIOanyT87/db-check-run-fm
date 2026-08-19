using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FuelsManagerService.PointGroupReport
{
    public class Column
    {
        public Column() { }
        public string Name { get; set; }
        public PenAlignment HeaderAlignment
        {
            get
            {
                var alignment = PenAlignment.Center;
                if (HeaderCssClass.Contains("text-left"))
                {
                    alignment = PenAlignment.Left;
                }
                else if (HeaderCssClass.Contains("text-right"))
                {
                    alignment = PenAlignment.Right;
                }
                return alignment;
            }
        }
        public PenAlignment CellAlignment
        {
            get
            {
                var alignment = PenAlignment.Center;
                if (CssClass.Contains("text-left"))
                {
                    alignment = PenAlignment.Left;
                }
                else if (CssClass.Contains("text-right"))
                {
                    alignment = PenAlignment.Right;
                }
                return alignment;
            }
        }
        public int FontSize
        {
            get
            {
                if (string.IsNullOrEmpty(CssClass))
                { return 14; }

                var pattern = @"grid-font-(\d+)";
                var match = Regex.Match(CssClass, pattern, RegexOptions.IgnoreCase);

                if (match.Success && int.TryParse(match.Groups[1].Value, out var fontSize))
                {
                    return fontSize;
                }
                else
                {
                    return 14;
                }
            }
        }
        public int Width { get; set; }
        public string Id { get; set; }
        public string Field { get; set; }
        public Newtonsoft.Json.Linq.JObject Filter { get; set; }
        public bool ShowUnit { get; set; } = false;
        public bool ShowQuality { get; set; } = false;
        public int DecimalPlaces { get; set; } = -1;
        public int Unit { get; set; } = -1;

        public string HeaderCssClass { get; set; } = string.Empty;
        public string CssClass { get; set; } = string.Empty;

        public Dictionary<string, string> TotalizerConfig { get; set; }
    }
}