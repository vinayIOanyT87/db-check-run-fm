using FuelsManager.Areas.InventoryManagement.ViewModels;
using FuelsManager.Areas.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Varec.CommonComponents.EngineeringUnitsLibrary;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
    public class PointHistoryTabController : FMBaseControllerEx
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public PartialViewResult GetPointHistoryTabView(string tabId, string controlId, string pointId, string pointGuid)
        {
            const string POINT_HISTORY_HEADER_MENU = @"[
    {
      ""iconCssClass"": ""header-menu-cell-alignment"",
      ""title"": ""Cell Alignment"",
      ""items"": [
        {
          ""iconCssClass"": ""header-menu-cell-alignment-left"",
          ""title"": ""Left"",
          ""command"": ""left-align""
        },
        {
          ""iconCssClass"": ""header-menu-cell-alignment-center"",
          ""title"": ""Center"",
          ""command"": ""center-align""
        },
        {
          ""iconCssClass"": ""header-menu-cell-alignment-right"",
          ""title"": ""Right"",
          ""command"": ""right-align""
        }
      ]

    },
    {
      ""iconCssClass"": ""header-menu-add-column"",
      ""title"": ""Insert Column"",
      ""items"": [
        {
          ""iconCssClass"": ""header-menu-tag"",
          ""title"": ""Tag"",
          ""command"": ""insert-column-tag""
        },
        {
          ""iconCssClass"": ""header-menu-empty-column"",
          ""title"": ""Empty Column"",
          ""command"": ""insert-empty-column""
        }
      ]
    },
    {
      ""iconCssClass"": ""header-menu-delete"",
      ""title"": ""Delete Column"",
      ""command"": ""delete-column""
    },
    {
      ""iconCssClass"": ""header-menu-rename"",
      ""title"": ""Rename"",
      ""command"": ""rename""
    },
    {
      ""iconCssClass"": ""header-menu-set-display-precision"",
      ""title"": ""Set Display Precision"",
      ""command"": ""changeprecision""
    },
    {
      ""iconCssClass"": ""header-menu-set-display-unit"",
      ""title"": ""Set Display Unit"",
      ""command"": ""changeunit""
    },
    {
      ""iconCssClass"": ""header-menu-show-unit"",
      ""title"": ""Show Units"",
      ""command"": ""showunits""
    }
  ]";
            try
            {
                var pointHistory = FMChannelHelper.MakeCall<IPointHistories, PointHistory>(x => x.Get(this.Security, this.Security.UserGuid, this.Security.SiteGuid));

                var pointHistoryObject = new PointHistoryTabModel();
                pointHistoryObject.TabId = tabId;
                pointHistoryObject.ControlId = controlId;
                pointHistoryObject.PointGuid = new Guid(pointGuid);
                pointHistoryObject.ID = pointId;
                pointHistoryObject.Name = pointId;

                if (pointHistory.ColumnsDefinition != null)
                { //return what was in the database
                    pointHistoryObject.Start = pointHistory.StartDate.ToString("MM/d/yyyy hh:mm tt");
                    pointHistoryObject.IntervalQuantity = pointHistory.IntervalQuantity;
                    pointHistoryObject.Interval = (PointHistoryInterval)pointHistory.IntervalType;
                    pointHistoryObject.RangeQuantity = pointHistory.RangeQuantity;
                    pointHistoryObject.Range = (PointHistoryRange)pointHistory.RangeType;
                    pointHistoryObject.Columns = pointHistory.ColumnsDefinition;
                }
                else
                { //return default values
                    pointHistoryObject.Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("MM/d/yyyy hh:mm tt");
                    pointHistoryObject.IntervalQuantity = 1;
                    pointHistoryObject.Interval = PointHistoryInterval.Hour;
                    pointHistoryObject.RangeQuantity = 1;
                    pointHistoryObject.Range = PointHistoryRange.Day;
                    pointHistoryObject.Columns = @"[
                        { ""name"": ""Date/Time"", ""resizable"": true, ""sortable"": false, ""minWidth"": 30, ""rerenderOnResize"": false, ""headerCssClass"": ""text-center grid-font-14"", ""defaultSortAsc"": true, ""focusable"": true, ""selectable"": true, ""width"": 160, ""id"": ""DateTime"", ""field"": ""Date Time"", ""cssClass"": ""ui-state-default text-center grid-font-14"", ""header"": { ""menu"": { ""items"": " + POINT_HISTORY_HEADER_MENU + @"} }, ""previousWidth"": 160 },
    		            { ""name"": ""Level Product"", ""resizable"": true, ""sortable"": false, ""minWidth"": 30, ""rerenderOnResize"": false, ""headerCssClass"": ""text-center grid-font-14"", ""defaultSortAsc"": true, ""focusable"": true, ""selectable"": true, ""width"": 100, ""id"": ""LevelProduct"", ""field"": ""Level Product"", ""cssClass"": ""text-center grid-font-14"", ""DecimalPlaces"": -1, ""header"": { ""menu"": { ""items"":" + POINT_HISTORY_HEADER_MENU + @"} }, ""previousWidth"": 100 },
    		            { ""name"": ""Temperature Product"", ""resizable"": true, ""sortable"": false, ""minWidth"": 30, ""rerenderOnResize"": false, ""headerCssClass"": ""text-center grid-font-14"", ""defaultSortAsc"": true, ""focusable"": true, ""selectable"": true, ""width"": 150, ""id"": ""TemperatureProduct"", ""field"": ""Temperature Product"", ""cssClass"": ""text-center grid-font-14"", ""DecimalPlaces"": -1, ""header"": { ""menu"": { ""items"":" + POINT_HISTORY_HEADER_MENU + @"} }, ""previousWidth"": 150 },
    		            { ""name"": ""Density Product Standard"", ""resizable"": true, ""sortable"": false, ""minWidth"": 30, ""rerenderOnResize"": false, ""headerCssClass"": ""text-center grid-font-14"", ""defaultSortAsc"": true, ""focusable"": true, ""selectable"": true, ""width"": 180, ""id"": ""DensityProductStandard"", ""field"": ""Density Product Standard"", ""cssClass"": ""text-center grid-font-14"", ""DecimalPlaces"": -1, ""header"": { ""menu"": { ""items"":" + POINT_HISTORY_HEADER_MENU + @"} }, ""previousWidth"": 180 },
    		            { ""name"": ""Volume Gross Observed"", ""resizable"": true, ""sortable"": false, ""minWidth"": 30, ""rerenderOnResize"": false, ""headerCssClass"": ""text-center grid-font-14"", ""defaultSortAsc"": true, ""focusable"": true, ""selectable"": true, ""width"": 170, ""id"": ""VolumeGrossObserved"", ""field"": ""Volume Gross Observed"", ""cssClass"": ""text-center grid-font-14"", ""DecimalPlaces"": -1, ""header"": { ""menu"": { ""items"":" + POINT_HISTORY_HEADER_MENU + @"} }, ""previousWidth"": 170 },
    		            { ""name"": ""Volume Correction for Temperature and Pressure"", ""resizable"": true, ""sortable"": false, ""minWidth"": 30, ""rerenderOnResize"": false, ""headerCssClass"": ""text-center grid-font-14"", ""defaultSortAsc"": true, ""focusable"": true, ""selectable"": true, ""width"": 335, ""id"": ""VolumeCorrectionforTemperatureandPressure"", ""field"": ""Volume Correction for Temperature and Pressure"", ""cssClass"": ""text-center grid-font-14"", ""DecimalPlaces"": -1, ""header"": { ""menu"": { ""items"":" + POINT_HISTORY_HEADER_MENU + @"} }, ""previousWidth"": 335 },
    		            { ""name"": ""Volume Net Standard"", ""resizable"": true, ""sortable"": false, ""minWidth"": 30, ""rerenderOnResize"": false, ""headerCssClass"": ""text-center grid-font-14"", ""defaultSortAsc"": true, ""focusable"": true, ""selectable"": true, ""width"": 150, ""id"": ""VolumeNetStandard"", ""field"": ""Volume Net Standard"", ""cssClass"": ""text-center grid-font-14"", ""DecimalPlaces"": -1, ""header"": { ""menu"": { ""items"":" + POINT_HISTORY_HEADER_MENU + @"} }, ""previousWidth"": 150 },
    		            { ""name"": """", ""resizable"": true, ""sortable"": false, ""minWidth"": 30, ""rerenderOnResize"": false, ""headerCssClass"": ""text-center grid-font-14"", ""defaultSortAsc"": true, ""focusable"": true, ""selectable"": true, ""width"": 30, ""id"": ""Empty0"", ""field"": """", ""cssClass"": ""ui-state-default text-center grid-font-14"", ""header"": { ""menu"": { ""items"":" + POINT_HISTORY_HEADER_MENU + @"} }, ""previousWidth"": 30 }
    	            ]";
                }

                return this.PartialView("PointHistoryTabView", pointHistoryObject);
            }
            catch (Exception e)
            {
                this.OnError(e);
                throw;
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult GetPointHistoryData(string pointGuid, int interval, int intervalQuantity, int range, int rangeQuantity, string startString, List<string> tagIds)
        {
            try
            {
                //interval: minute=0, hour=1, day=2
                //range: hour=0, day=1, month=2, year=3
                var intervalFrequency = 1;
                if (interval == 1)
                { intervalFrequency = 60; }
                if (interval == 2)
                { intervalFrequency = 60 * 24; }

                var start = DateTimeOffset.Parse(startString);
                DateTimeOffset end;

                switch (range)
                {
                    case 0:
                        end = start.AddHours(rangeQuantity);
                        break;

                    case 1:
                        end = start.AddDays(rangeQuantity);
                        break;

                    case 2:
                        end = start.AddMonths(rangeQuantity);
                        break;

                    case 3:
                        end = start.AddYears(rangeQuantity);
                        break;

                    default:
                        end = start;
                        break;
                }

                int numberOfSamplesPerPen;
                switch (interval)
                {
                    case 0:
                        //end = end.AddMinutes(1);
                        numberOfSamplesPerPen = (int)Math.Ceiling((end - start).TotalMinutes / intervalFrequency);
                        break;

                    case 1:
                        //end = end.AddHours(1);
                        numberOfSamplesPerPen = (int)Math.Ceiling((end - start).TotalHours / intervalFrequency * 60);
                        break;

                    case 2:
                        //end = end.AddDays(1);
                        numberOfSamplesPerPen = (int)Math.Ceiling((end - start).TotalDays / intervalFrequency * 60 * 24);
                        break;


                    default:
                        numberOfSamplesPerPen = 0;
                        break;
                }

                // lookup tag guids from point guid and tag ids
                var tagGuids = new List<Guid>();
                foreach (var tagId in tagIds)
                {
                    var guid = FMChannelHelper.MakeCall<IPointTags, Guid>(x => x.GetIdentityGuid(this.Security, tagId, new Guid(pointGuid)));
                    tagGuids.Add(guid);
                }
                var trendArchivePreData = FMChannelHelper.MakeCall<IPointTagArchive, List<List<TrendArchiveDataElement>>>(x => x.GetHistoryArchiveData(this.Security, tagGuids, start.AddDays(-1), start, 1));
                var trendArchiveData = FMChannelHelper.MakeCall<IPointTagArchive, List<List<TrendArchiveDataElement>>>(x => x.GetHistoryArchiveData(this.Security, tagGuids, start, end, numberOfSamplesPerPen));
                for (var i = 0; i < trendArchiveData.Count; i++)
                {
                    //remove the first and last values if null
                    if (trendArchiveData[i].Count > 0 && string.IsNullOrEmpty(trendArchiveData[i][0].Value))
                    {
                        trendArchiveData[i].RemoveAt(0);
                    }
                    if (trendArchiveData[i].Count > 0 && string.IsNullOrEmpty(trendArchiveData[i][trendArchiveData[i].Count - 1].Value))
                    {
                        trendArchiveData[i].RemoveAt(trendArchiveData[i].Count - 1);
                    }
                    trendArchiveData[i].Add(trendArchivePreData[i][1]); //always grab the middle (2nd) result as it has the actual timestamp
                }

                var historyArchiveData = new List<HistoryArchiveDataElement>();

                var dt = start;
                var j = 0;
                while (dt <= end)
                {
                    var element = new HistoryArchiveDataElement();
                    element.Time = dt;
                    if (dt <= DateTimeOffset.Now)
                    {
                        for (int i = 0; i < tagGuids.Count; i++)
                        {
                            element.Values.Add(tagIds[i], trendArchiveData[i].Where(x => x.ValueTimeStamp <= dt).OrderByDescending(x => x.ValueTimeStamp).FirstOrDefault()?.Value);
                        }
                    }
                    historyArchiveData.Add(element);
                    dt = dt.AddMinutes(intervalQuantity * intervalFrequency);
                    j++;
                }

                return this.JsonWithErrorMessages(historyArchiveData);
            }
            catch (Exception e)
            {
                this.OnError(e);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class HistoryArchiveDataElement
    {
        public DateTimeOffset Time { get; set; }
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }
}