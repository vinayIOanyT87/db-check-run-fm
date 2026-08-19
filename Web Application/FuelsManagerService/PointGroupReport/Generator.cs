using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMPointCommon;
using FuelsManagerService.PointGroupReport.FilterRules;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Newtonsoft.Json;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Varec.CommonComponents.EngineeringUnitsLibrary;
using static MigraDoc.DocumentObjectModel.Shapes.Charts.Point;

namespace FuelsManagerService.PointGroupReport
{
    public class Generator
    {
        #region Constants and Fields
        private static EventLog eventLog = new EventLog("Application", ".", "FMReportGenerator");
        private static SecurityClass security = new SecurityClass();
        #endregion

        public static void Process(Guid pointGroupScheduleGuid, DateTime jobStartTime)
        {
            var pointgroupname = "";
            var pointgroupguid = "";
            var siteid = "";
            var userid = "";
            try
            {
                #region Prepare
                Trace.WriteLine("Starting Process", "Point Group Report Processing");
                security.UserGuid = Guid.Empty;
                security.LoginSiteGuid = Guids.SiteAdminGuid;
                security.SiteGuid = Guids.SiteAdminGuid;
                security.UserID = FMChannelHelper.MakeCall<IDBAccess, string>(fuelsManagerDatabaseAccess => fuelsManagerDatabaseAccess.ServiceLogin(security));

                security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
                security.AddRight(RIGHT.VIEW_USERS);

                Trace.WriteLine("Retrieving Schedule", "Point Group Report Processing");
                var schedule = FMChannelHelper.MakeCall<IPointGroupSchedules, PointGroupSchedule>(x => x.GetByPK(security, pointGroupScheduleGuid));

                if (schedule == null || schedule.PointGroupScheduleGuid == Guid.Empty)
                {
                    throw new Exception("Schedule " + pointGroupScheduleGuid.ToString() + " not found.");
                }

                security.SiteGuid = schedule.SiteGuid;
                security.LoginSiteGuid = schedule.SiteGuid;
                security.UserGuid = schedule.UserGuid;

                Trace.WriteLine("Retrieving Point Group", "Point Group Report Processing");
                var pointGroup = FMChannelHelper.MakeCall<IPointGroups, PointGroup>(x => x.Get(security, schedule.PointGroupGuid, schedule.UserGuid, schedule.SiteGuid));
                if (pointGroup == null || schedule.PointGroupGuid == Guid.Empty)
                {
                    throw new Exception("Point Group " + schedule.PointGroupGuid.ToString() + " not found.");
                }

                pointgroupname = pointGroup.ID;
                pointgroupguid = pointGroup.PointGroupGuid.ToString();
                Trace.WriteLine("Retrieving User", "Point Group Report Processing");
                var user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, schedule.UserGuid));
                userid = user.ID;
                if (user == null || user.Deleted || user.InactivityLockout)
                {
                    throw new Exception("User " + schedule.UserGuid.ToString() + " not found, has been deleted or is locked out.");
                }

                Trace.WriteLine("Retrieving Site", "Point Group Report Processing");
                var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, schedule.SiteGuid, false, false, false));
                if (site == null || schedule.SiteGuid == Guid.Empty || site.Deleted)
                {
                    throw new Exception("Site " + schedule.SiteGuid.ToString() + " not found or has been deleted.");
                }

                #region uncomment when adding support for alarm priorities
                //Trace.WriteLine("Retrieving Alarm Priorities", "Point Group Report Processing");
                //security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
                //security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
                //security.AddRight(RIGHT.VIEW_ALARM_EVENT_LOGS);
                //security.AddRight(RIGHT.OPERATE_VIEW_ALARM_HISTORY);
                //security.AddRight(RIGHT.OPERATE_VIEW_ALARM_SUMMARY);
                //security.AddRight(RIGHT.OPERATE_VIEW_GRAPHICS);
                //security.AddRight(RIGHT.OPERATE_VIEW_IM_REPORTS);
                //security.AddRight(RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY);
                //security.AddRight(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY);
                //security.AddRight(RIGHT.OPERATE_VIEW_POINTS);
                //security.AddRight(RIGHT.OPERATE_VIEW_POINT_GROUPS);
                //security.AddRight(RIGHT.OPERATE_VIEW_TRENDS);
                //security.AddRight(RIGHT.OPERATE_VIEW_UNPUBLISHED);
                //var alarmPriorities = FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(x => x.Enumerate(security));
                //security.RemoveRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
                //security.RemoveRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
                //security.RemoveRight(RIGHT.VIEW_ALARM_EVENT_LOGS);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_ALARM_HISTORY);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_ALARM_SUMMARY);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_GRAPHICS);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_IM_REPORTS);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_POINTS);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_POINT_GROUPS);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_TRENDS);
                //security.RemoveRight(RIGHT.OPERATE_VIEW_UNPUBLISHED);
                #endregion

                siteid = site.SiteID;
                var datePattern = site.ShortDatePattern + " " + site.TimePattern;
                var siteTimeZone = site.TimeZone;
                var numFormatInfo = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);

                string fileNameAsConfigured = site.PointGroupDefaultFileName;
                string fileName = fileNameAsConfigured;
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = "%SiteID%_%UserID%_%PointGroupID%";
                }

                if (fileName.Contains("%SiteID%"))
                    fileName = fileName.Replace("%SiteID%", siteid);

                if (fileName.Contains("%PointGroupID%"))
                    fileName = fileName.Replace("%PointGroupID%", pointgroupname);

                if (fileName.Contains("%UserID%"))
                    fileName = fileName.Replace("%UserID%", userid);

                if (schedule.CreateNewExportFile)
                {
                    var siteTimeConverter = new SiteTimeConverter(site);
                    string dateTimePrefix = siteTimeConverter.ConvertToSiteTime(DateTime.UtcNow).ToString("_yyyyMMdd-HHmmss");
                    fileName += dateTimePrefix;
                }

                var fileExportDirectory = site.PointGroupFileExportDirectory;

                if (schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.CSV)
                {
                    fileName += ".csv";
                }
                else
                {
                    fileName += ".pdf";
                }
                #endregion

                #region BuildReport
                var pointGroupReport = new PointGroupReport(pointGroup.PointGroupColumn.ColumnsDefinition, pointGroup.PointGroupRow.RowsDefinition);
                pointGroupReport.PointGroupName = pointgroupname;
                pointGroupReport.IsLandscape = schedule.Layout == PointGroupSchedule.LayoutType.Landscape;
                var reportText = string.Empty;
                PdfDocument reportPdf = null;
                if (pointGroupReport.Columns[0].Filter != null)
                {
                    // get rows from columns point filter
                    var pointGroupFilterRules = pointGroupReport.Columns[0].Filter.ToObject<PointGroupFilterRules>();
                    var dynamicPointResults = GetDataForDynamicPoints(security, pointGroupFilterRules, new List<string>(), siteTimeZone);
                    var pointValueIdentifiers = new List<PointValueIdentifier>();
                    var hasTotalRow = pointGroupReport.Rows.Count > 0;

                    //build rows from dynamic data
                    var distinctPointGuids = dynamicPointResults.Select(x => x.PointGuid).Distinct();

                    foreach (var guid in distinctPointGuids)
                    {
                        var dynamicPoint = dynamicPointResults.First(x => x.PointGuid == guid && x.PointID == null);
                        var row = new Row()
                        {
                            Point = dynamicPoint.Value.ToString(),
                            PointGuid = guid.ToString(),
                            Type = "point"
                        };
                        if (hasTotalRow)
                        {
                            pointGroupReport.Rows.Insert(pointGroupReport.Rows.Count - 1, row);
                        }
                        else
                        {
                            pointGroupReport.Rows.Add(row);
                        }

                        pointValueIdentifiers.Add(new PointValueIdentifier(guid, PointValueType.Point, "ProductDescription"));
                        pointValueIdentifiers.Add(new PointValueIdentifier(guid, PointValueType.Point, "ProductID"));
                    }

                    var pointPropertyResults = GetPropertyValues(security, pointValueIdentifiers, siteTimeZone);

                    if (schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.CSV)
                    {
                        reportText = GenerateCsvText(dynamicPointResults, pointPropertyResults, pointGroupReport, numFormatInfo, datePattern, siteTimeZone);
                    }
                    // printing the CSV still involves generating and printing a PDF
                    if ((schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.CSV && !string.IsNullOrEmpty(schedule.Printer)) || schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.PDF)
                    {
                        reportPdf = GeneratePdf(dynamicPointResults, pointPropertyResults, pointGroupReport, siteid, numFormatInfo, datePattern, schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.PDF, siteTimeZone, schedule.FitToPage);
                    }
                }
                else
                {
                    Dictionary<string, List<PointValueMetaData>> metaData = new Dictionary<string, List<PointValueMetaData>>();
                    var tags = pointGroupReport.Columns.Select(x => x.Field).Where(x => !string.IsNullOrEmpty(x)).ToList();
                    var tagGuids = new List<Guid>();
                    var pointValueIdentifiers = new List<PointValueIdentifier>();

                    foreach (var pointGuid in pointGroupReport.Rows.Where(x => x.Type == "point").Select(x => x.PointGuid))
                    {
                        metaData[pointGuid] = GetMetaDataForStaticPoints(security, pointGuid, tags);
                        var metaDataTagGuids = metaData[pointGuid].Where(x => x.IdentityGuid == x.PointTagGuid).Select(x => x.IdentityGuid);
                        tagGuids.AddRange(metaDataTagGuids);

                        var pointValuePropertyMetaData = metaData[pointGuid].FirstOrDefault(x => x.PropertyID == "ProductDescription");
                        if (pointValuePropertyMetaData != null)
                        {
                            pointValueIdentifiers.Add(new PointValueIdentifier(pointValuePropertyMetaData.IdentityGuid, PointValueType.Point, "ProductDescription"));
                        }

                        pointValuePropertyMetaData = metaData[pointGuid].FirstOrDefault(x => x.PropertyID == "ProductID");
                        if (pointValuePropertyMetaData != null)
                        {
                            pointValueIdentifiers.Add(new PointValueIdentifier(pointValuePropertyMetaData.IdentityGuid, PointValueType.Point, "ProductID"));
                        }
                    }

                    var staticPointResults = GetDataForStaticPoints(security, tagGuids, siteTimeZone);
                    var staticPointValues = new List<PointValue>();
                    foreach (var pointTag in staticPointResults)
                    {
                        var pvi = new PointValueIdentifier(pointTag);
                        pvi.UtcTicks = 0;
                        pointValueIdentifiers.Add(pvi);
                        staticPointValues.Add(new PointValue(pointTag));
                    }
                    var pointPropertyResults = GetPropertyValues(security, pointValueIdentifiers, siteTimeZone);
                    if (schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.CSV)
                    {
                        reportText = GenerateCsvText(staticPointValues, pointPropertyResults, pointGroupReport, numFormatInfo, datePattern, siteTimeZone);
                    }
                    // printing the CSV still involves generating and printing a PDF
                    if ((schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.CSV && !string.IsNullOrEmpty(schedule.Printer)) || schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.PDF)
                    {
                        reportPdf = GeneratePdf(staticPointValues, pointPropertyResults, pointGroupReport, siteid, numFormatInfo, datePattern, schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.PDF, siteTimeZone, schedule.FitToPage);
                    }
                }
                #endregion

                #region Process
                if (schedule.ExportFileFormat == PointGroupSchedule.ExportFileType.CSV)
                {
                    if (!string.IsNullOrEmpty(fileExportDirectory))
                    {
                        try
                        {
                            using (Stream stream = File.OpenWrite(fileExportDirectory + "\\" + fileName))
                            {
                                using (var writer = new StreamWriter(stream, new UTF8Encoding(true)))
                                {
                                    writer.Write(reportText);
                                    writer.Flush();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            eventLog.WriteEntry("FMReportGenerator: Schedule: " + pointGroupScheduleGuid.ToString() + " - " + "Error saving Point Group Report CSV " + fileName, EventLogEntryType.Error);

                            AlarmAndEventLogClass eventEmailNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportCompletedEventDescriptor);
                            eventEmailNotification.AssociatedData = $"Failed to save scheduled report - {jobStartTime.ToString("MM/dd/yyyy HH:mm:ss")}, Schedule - {pointGroupScheduleGuid.ToString()}, point group - {pointgroupname} , User - {userid}";
                            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, eventEmailNotification));
                        }
                    }
                    if (!string.IsNullOrEmpty(schedule.EmailTo))
                    {
                        using (var stream = new MemoryStream())
                        using (var writer = new StreamWriter(stream, new UTF8Encoding(true)))
                        {
                            writer.Write(reportText);
                            writer.Flush();
                            stream.Position = 0;

                            Trace.WriteLine("Emailing Point Group Report CSV", "Point Group Report Processing");

                            if (!Emailing.EmailReport(schedule.EmailTo,
                                        fileName,
                                        stream,
                                        site,
                                        schedule.ExportFileFormat,
                                        out string errorMsg))
                            {
                                eventLog.WriteEntry("FMReportGenerator: Schedule: " + pointGroupScheduleGuid.ToString() + " - " + "Error emailing Point Group Report CSV " + fileName + ": " + errorMsg, EventLogEntryType.Error);

                                AlarmAndEventLogClass eventEmailNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportCompletedEventDescriptor);
                                eventEmailNotification.AssociatedData = $"Failed to email scheduled report - {jobStartTime.ToString("MM/dd/yyyy HH:mm:ss")}, Schedule - {pointGroupScheduleGuid.ToString()}, point group - {pointgroupname} , User - {userid}";
                                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, eventEmailNotification));
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(schedule.Printer))
                    {
                        // printing as a CSV still involves printing a PDF
                        using (var pdfStream = new MemoryStream())
                        {
                            Trace.WriteLine("Printing Point Group Report for CSV", "Point Group Report Processing");
                            var errorMsg = "";
                            reportPdf.Save(pdfStream, false);
                            pdfStream.Position = 0;

                            if (!Printing.PrintReport(schedule.Printer,
                                            "Letter",
                                            1,
                                            (schedule.Layout == PointGroupSchedule.LayoutType.Landscape),
                                            pdfStream,
                                            schedule.ExportFileFormat,
                                            out errorMsg))
                            {
                                eventLog.WriteEntry("FMReportGenerator: Schedule: " + pointGroupScheduleGuid.ToString() + " - " + "Error printing Point Group Report CSV " + fileName + ": " + errorMsg, EventLogEntryType.Error);

                                AlarmAndEventLogClass eventEmailNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportCompletedEventDescriptor);
                                eventEmailNotification.AssociatedData = $"Failed to print scheduled report - {jobStartTime.ToString("MM/dd/yyyy HH:mm:ss")}, Schedule - {pointGroupScheduleGuid.ToString()}, point group - {pointgroupname} , User - {userid}";
                                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, eventEmailNotification));
                            }
                        }
                    }
                }
                else
                {
                    using (var pdfStream = new MemoryStream())
                    {
                        reportPdf.Save(pdfStream, false);

                        if (!string.IsNullOrEmpty(fileExportDirectory))
                        {
                            try
                            {
                                pdfStream.Position = 0;
                                var pdfBytes = pdfStream.ToArray();
                                File.WriteAllBytes(fileExportDirectory + "\\" + fileName, pdfBytes);
                            }
                            catch (Exception ex)
                            {
                                eventLog.WriteEntry("FMReportGenerator: Schedule: " + pointGroupScheduleGuid.ToString() + " - " + "Error saving Point Group Report PDF " + fileName, EventLogEntryType.Error);

                                AlarmAndEventLogClass eventEmailNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportCompletedEventDescriptor);
                                eventEmailNotification.AssociatedData = $"Failed to save scheduled report - {jobStartTime.ToString("MM/dd/yyyy HH:mm:ss")}, Schedule - {pointGroupScheduleGuid.ToString()}, point group - {pointgroupname} , User - {userid}";
                                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, eventEmailNotification));
                            }
                        }
                        if (!string.IsNullOrEmpty(schedule.EmailTo))
                        {
                            Trace.WriteLine("Emailing Point Group Report PDF", "Point Group Report Processing");
                            pdfStream.Position = 0;
                            if (!Emailing.EmailReport(schedule.EmailTo,
                                        fileName,
                                        pdfStream,
                                        site,
                                        schedule.ExportFileFormat,
                                        out string errorMsg))
                            {
                                eventLog.WriteEntry("FMReportGenerator: Schedule: " + pointGroupScheduleGuid.ToString() + " - " + "Error emailing Point Group Report PDF " + fileName + ": " + errorMsg, EventLogEntryType.Error);

                                AlarmAndEventLogClass eventEmailNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportCompletedEventDescriptor);
                                eventEmailNotification.AssociatedData = $"Failed to email scheduled report - {jobStartTime.ToString("MM/dd/yyyy HH:mm:ss")}, Schedule - {pointGroupScheduleGuid.ToString()}, point group - {pointgroupname} , User - {userid}";
                                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, eventEmailNotification));
                            }
                        }
                        if (!string.IsNullOrEmpty(schedule.Printer))
                        {
                            Trace.WriteLine("Printing Point Group Report PDF", "Point Group Report Processing");
                            var errorMsg = "";
                            pdfStream.Position = 0;

                            if (!Printing.PrintReport(schedule.Printer,
                                            "Letter",
                                            1,
                                            (schedule.Layout == PointGroupSchedule.LayoutType.Landscape),
                                            pdfStream,
                                            schedule.ExportFileFormat,
                                            out errorMsg))
                            {
                                eventLog.WriteEntry("FMReportGenerator: Schedule: " + pointGroupScheduleGuid.ToString() + " - " + "Error printing Point Group Report PDF " + fileName + ": " + errorMsg, EventLogEntryType.Error);

                                AlarmAndEventLogClass eventEmailNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportCompletedEventDescriptor);
                                eventEmailNotification.AssociatedData = $"Failed to print scheduled report - {jobStartTime.ToString("MM/dd/yyyy HH:mm:ss")}, Schedule - {pointGroupScheduleGuid.ToString()}, point group - {pointgroupname} , User - {userid}";
                                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, eventEmailNotification));
                            }
                        }
                    }
                }
                #endregion

                eventLog.WriteEntry("FMReportGenerator: Report Generated for Schedule - " + pointGroupScheduleGuid.ToString() + ", Point Group - " + pointgroupname + ", User - " + userid, EventLogEntryType.Information);
                Trace.WriteLine("Report Generated", "Point Group Report Processing");

                AlarmAndEventLogClass eventNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportCompletedEventDescriptor);
                eventNotification.AssociatedData = $"Completed scheduled report - {jobStartTime.ToString("MM/dd/yyyy HH:mm:ss")}, Schedule - {pointGroupScheduleGuid.ToString()}, point group - {pointgroupname} , User - {userid}";
                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, eventNotification));
            }
            catch (Exception e)
            {
                eventLog.WriteEntry("FMReportGenerator: Schedule: " + pointGroupScheduleGuid.ToString() + " - " + e.Message, EventLogEntryType.Error);

                AlarmAndEventLogClass eventNotification = new AlarmAndEventLogClass(PointGroupSchedule.ScheduleReportCompletedEventDescriptor);
                eventNotification.AssociatedData = $"Failed to Complete scheduled report - {jobStartTime.ToString("MM/dd/yyyy HH:mm:ss")}, Schedule - {pointGroupScheduleGuid.ToString()}, point group - {pointgroupname} , User - {userid}";
                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, eventNotification));
                throw new Exception("FMReportGenerator: " + e.Message);
            }

        }

        private static List<PointValue> GetDataForDynamicPoints(SecurityClass security, PointGroupFilterRules filter, List<string> tagList, string siteTimeZone)
        {
            try
            {
                var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateBySiteFiltered(security, security.SiteGuid, filter, tagList));

                // we need to create a list of tags to request the values
                var returnPointNameList = new List<PointValue>();
                var pointValueIdentifierPropertiesList = new List<PointValueIdentifier>();
                var includeProductName = (tagList.FirstOrDefault(stringToCheck => stringToCheck.Contains("ProductID")) != null);
                var includeProductDescription = (tagList.FirstOrDefault(stringToCheck => stringToCheck.Contains("ProductDescription")) != null);

                foreach (var point in points)
                {
                    // add the point name as a tag since it's display in the grid
                    var pointValue = new PointValue
                    {
                        PointValueIdentifier = new PointValueIdentifier(point.PointGuid, PointValueType.Point, "point"),
                        PointGuid = point.PointGuid,
                        ID = "point",
                        Value = point.ID,
                        ValueTypeString = "System.String"
                    };
                    returnPointNameList.Add(pointValue);

                    if (includeProductName)
                    {
                        pointValueIdentifierPropertiesList.Add(new PointValueIdentifier(point.PointGuid, PointValueType.Point, "ProductID"));
                    }

                    if (includeProductDescription)
                    {
                        pointValueIdentifierPropertiesList.Add(new PointValueIdentifier(point.PointGuid, PointValueType.Point, "ProductDescription"));
                    }
                }

                // get the list of tag values
                var pointValueIdentifierList = points.SelectMany(point => point.Tags).Select(x => new PointValueIdentifier(x.Value.PointTagGuid, PointValueType.Tag, null, x.Value.WellKnownIdentityGuid)).ToList();

                var allPointValueIdentifiers = pointValueIdentifierPropertiesList.Union(pointValueIdentifierList).ToList();

                var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(security, allPointValueIdentifiers));

                if (pointValues == null)
                {
                    pointValues = new List<PointValue>(allPointValueIdentifiers.Count);
                    foreach (var pointValueIdentifier in allPointValueIdentifiers)
                    {
                        pointValues.Add(new PointValue() { PointValueIdentifier = pointValueIdentifier });
                    }
                }
                else
                {
                    foreach (var pointValue in pointValues)
                    {
                        if (pointValue.Value != null)
                        {

                            if (pointValue.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") != -1)
                            {
                                pointValue.Value = GetTranslatedText(FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString((Enum)pointValue.Value), security);
                            }

                            else if (pointValue.ValueTypeString == "System.Boolean")
                            {
                                if (pointValue.Value is bool)
                                {
                                    pointValue.Value = ((bool)pointValue.Value) ? GetTranslatedText("True", security) : GetTranslatedText("False", security);
                                }
                            }

                            else if (pointValue.ValueTypeString == "System.DateTimeOffset")
                            {
                                var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(siteTimeZone);
                                pointValue.Value = TimeZoneInfo.ConvertTime((DateTimeOffset)pointValue.Value, siteTimeZoneInfo);
                            }

                            else if ((pointValue.Value is double || pointValue.Value is float) && Double.IsNaN(Convert.ToDouble(pointValue.Value)))
                            {
                                pointValue.Value = "NaN";
                            }
                        }
                    }
                }

                // merge the list of tag values with the point names
                var returnList = returnPointNameList.Union(pointValues);
                //bool CommunicationsFailure = false;
                //var pointTagList = from p in returnList select new { PointValueIdentifier_IdentityGuid = p.PointValueIdentifier.IdentityGuid, PointValueIdentifier_PointValueType = p.PointValueIdentifier.PointValueType, PointValueIdentifier_PropertyID = p.PointValueIdentifier.PropertyID, PointValueIdentifier_UtcTicks = 0, p.PointGuid, p.Value, p.ValueTypeString, PointTagGuid = p.PointValueIdentifier.IdentityGuid, ID = (p.PointValueIdentifier.PointValueType == PointValueType.Point ? p.PointValueIdentifier.PropertyID : p.ID), p.ServerTimeStamp, p.Units, p.DecimalPlaces, p.Maximum, p.Minimum, p.QualityAbbreviation, p.EngineeringUnitsType, p.Acknowledged, p.AlarmPriorityGuid, p.AlarmState, p.WellKnownIdentityGuid, p.Access, p.InputOutputType, p.InhibitOverride, p.Status, CommunicationsFailure };

                return returnList.ToList();

            }
            catch (Exception e)
            {
                //this.OnError(e);
                return null;
            }

        }

        private static List<PointTag> GetDataForStaticPoints(SecurityClass security, List<Guid> tagGuids, string siteTimeZone)
        {
            try
            {
                var pointTags = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(x => x.GetPointTagData(security, tagGuids));

                if (pointTags == null)
                {
                    pointTags = new List<PointTag>(tagGuids.Count);
                    foreach (var tagGuid in tagGuids)
                    {
                        pointTags.Add(new PointTag() { PointTagGuid = tagGuid });
                    }
                }
                // we don't need to return all the data, only the fields we will use
                //var pointTagList = from p in pointTags select new { p.Value, p.ValueTypeString, p.PointTagGuid, p.ID, p.ServerTimeStamp, p.Units, p.DecimalPlaces, p.Maximum, p.Minimum, p.QualityAbbreviation, p.EngineeringUnitsType, p.Acknowledged, p.AlarmPriorityGuid, p.AlarmState, p.WellKnownIdentityGuid };

                return pointTags;
            }
            catch (Exception e)
            {
                //this.OnError(e);
                return new List<PointTag>();
            }
        }

        private static List<PointValueMetaData> GetMetaDataForStaticPoints(SecurityClass security, string pointGuid, List<string> tags)
        {
            try
            {
                if (tags.Count > 0)
                {
                    Guid checkPointGuid = new Guid(pointGuid);
                    var pointGuidList = new List<Guid>();
                    pointGuidList.Add(checkPointGuid);

                    var pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, Dictionary<Guid, PointTag>>>(x => x.EnumerateByPointList(security, pointGuidList, tags));

                    var tagList = new List<PointTag>();
                    var propertyList = new List<PointTag>();

                    var allTags = (pointTagDictionary.ContainsKey(checkPointGuid)) ? pointTagDictionary[checkPointGuid].Values.ToList() : new List<PointTag>();
                    foreach (var tagname in tags)
                    {
                        // check if tag name is actually a point property
                        // the tagName may actually be a setting ( we allow for name, product and product description )
                        if (tagname == "ID" || tagname == "ProductID" || tagname == "ProductDescription")
                        {
                            // we need to create the point value identifier for the column
                            var pointPropery = new PointTag { PointGuid = checkPointGuid, ID = tagname, PointTagGuid = Guid.Empty };
                            propertyList.Add(pointPropery);
                            continue;
                        }

                        int index = allTags.FindIndex(x => x.ID.ToLower(CultureInfo.InvariantCulture) == tagname.ToLower(CultureInfo.InvariantCulture) && !x.Deleted);
                        if (index >= 0)
                        {
                            var newTag = allTags[index];

                            tagList.Add(newTag);
                        }
                    }

                    bool CommunicationsFailure = false;
                    // we don't need to return all the data, only the fields we will use
                    var pointTagList = from p in tagList select new PointValueMetaData { IdentityGuid = p.PointTagGuid, PointValueType = PointValueType.Tag, PropertyID = (string)null, UtcTicks = 0, PointGuid = p.PointGuid, PointTagGuid = p.PointTagGuid, ID = p.ID, Units = p.Units, Maximum = p.Maximum, Minimum = p.Minimum, DecimalPlaces = p.DecimalPlaces, EngineeringUnitsType = p.EngineeringUnitsType, InhibitOverride = p.InhibitOverride, WellKnownIdentityGuid = p.WellKnownIdentityGuid, InputOutputType = p.InputOutputType, Status = p.Status, CommunicationsFailure = CommunicationsFailure };
                    var pointpropertyList = from p in propertyList select new PointValueMetaData { IdentityGuid = p.PointGuid, PointValueType = PointValueType.Point, PropertyID = p.ID, UtcTicks = 0, PointGuid = p.PointGuid, PointTagGuid = p.PointTagGuid, ID = p.ID, Units = p.Units, Maximum = p.Maximum, Minimum = p.Minimum, DecimalPlaces = p.DecimalPlaces, EngineeringUnitsType = p.EngineeringUnitsType, InhibitOverride = p.InhibitOverride, WellKnownIdentityGuid = p.WellKnownIdentityGuid, InputOutputType = p.InputOutputType, Status = p.Status, CommunicationsFailure = CommunicationsFailure };
                    var result = pointTagList.Concat(pointpropertyList).ToList();
                    return result;
                }

                return null;
            }
            catch (Exception except)
            {
                //this.OnError(except);
                return null;
            }

        }

        private static List<OperatePointValue> GetPropertyValues(SecurityClass security, List<PointValueIdentifier> pointValueIdentifiers, string siteTimeZone)
        {
            try
            {
                // we don't need to return all the data, only the fields we will use
                List<OperatePointValue> pointValueList = new List<OperatePointValue>(pointValueIdentifiers.Count);

                var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueDataChanges(security, pointValueIdentifiers));

                if (pointValues == null)
                {
                    foreach (var pointValueIdentifier in pointValueIdentifiers)
                    {
                        var pointValue = new PointValue() { PointValueIdentifier = pointValueIdentifier };
                        var pv = new OperatePointValue(pointValue);
                        pointValueList.Add(pv);
                    }
                }
                else
                {
                    foreach (var pointValue in pointValues)
                    {
                        if (pointValue.PointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagSiteDataGuid
                        || pointValue.PointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagUserDataGuid
                        || pointValue.PointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagDateTimeDataGuid
                        || pointValue.PointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TaglicenseExpiryDataGuid)
                        {
                            continue;
                        }


                        if (pointValue.Value != null)
                        {
                            if (pointValue.Value is List<PointValue>)
                            {
                                foreach (var value in (pointValue.Value as List<PointValue>))
                                {
                                    if (value == null)
                                    {
                                        continue;
                                    }

                                    FormatValue(value, siteTimeZone, security);
                                }
                            }
                            else
                            {
                                FormatValue(pointValue, siteTimeZone, security);
                            }
                        }

                        var pv = new OperatePointValue(pointValue);
                        pointValueList.Add(pv);
                    }
                }

                return pointValueList;
            }
            catch (Exception e)
            {
                //this.OnError(e);
                return null;
            }

        }

        private static string GenerateCsvText(List<PointValue> points, List<OperatePointValue> values, PointGroupReport report, NumberFormatInfo numFormatInfo, string datePattern, string siteTimeZone)
        {
            var output = new StringBuilder();
            // build header
            foreach (var column in report.Columns)
            {
                output.Append("\"");
                output.Append(Uri.UnescapeDataString(column.Name));
                output.Append("\",");
            }
            output.AppendLine();

            // build rows
            foreach (var row in report.Rows)
            {
                if (RowIsFiltered(row, report.Columns, points, values))
                {
                    continue;
                }
                foreach (var column in report.Columns)
                {
                    output.Append("\"");
                    if (row.Type == "subtotal")
                    {
                        if (column.Field == "point")
                        {
                            output.Append("Subtotal");
                        }
                        if (column.TotalizerConfig != null && column.TotalizerConfig.Count > 0)
                        {
                            output.Append(GetSubtotalValue(report, row, column, points, values, numFormatInfo, datePattern, siteTimeZone));
                        }
                    }
                    else if (row.Type == "total")
                    {
                        if (column.Field == "point")
                        {
                            output.Append("Total");
                        }
                        if (column.TotalizerConfig != null && column.TotalizerConfig.Count > 0)
                        {
                            output.Append(GetSubtotalValue(report, row, column, points, values, numFormatInfo, datePattern, siteTimeZone));
                        }
                    }
                    else if (row.Type != "blank" && row.Type != "empty")
                    {
                        switch (column.Field)
                        {
                            case "point":
                                output.Append(row.Point);
                                break;
                            case "ProductID":
                                output.Append(GetPropertyString(values, row.PointGuid, "ProductID"));
                                break;
                            case "ProductDescription":
                                output.Append(GetPropertyString(values, row.PointGuid, "ProductDescription"));
                                break;
                            default:
                                // find the PointValue for this column
                                var pointValue = points.FirstOrDefault(x => x.PointID == row.Point && x.ID == column.Field);
                                if (pointValue != null)
                                {
                                    output.Append(GetValueString(new OperatePointValue(pointValue), column, numFormatInfo, datePattern, false, siteTimeZone));
                                }
                                break;
                        }
                    }
                    output.Append("\",");

                }
                output.AppendLine();
            }

            return output.ToString();
        }

        private static PdfDocument GeneratePdf(List<PointValue> points, List<OperatePointValue> values, PointGroupReport report, string siteid, NumberFormatInfo numFormatInfo, string datePattern, bool useThousandsSeparator, string siteTimeZone, bool fitToPage)
        {
            #region document setup
            var pdfDoc = new PdfDocument();
            var migraDoc = new Document();
            var style = migraDoc.Styles[StyleNames.Normal];
            double scale = 1;

            style.Font.Name = "Arial";
            var section = migraDoc.AddSection();
            section.PageSetup = migraDoc.DefaultPageSetup.Clone();
            section.PageSetup.ResetPageSize();
            section.PageSetup.PageFormat = PageFormat.Letter;
            if (report.IsLandscape)
            {
                section.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                section.PageSetup.PageHeight = Unit.FromInch(8.5);
                section.PageSetup.PageWidth = Unit.FromInch(11);
            }
            else
            {
                section.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait;
                section.PageSetup.PageHeight = Unit.FromInch(11);
                section.PageSetup.PageWidth = Unit.FromInch(8.5);
            }
            section.PageSetup.LeftMargin = Unit.FromInch(0.5);
            section.PageSetup.RightMargin = Unit.FromInch(0.5);
            section.PageSetup.TopMargin = Unit.FromInch(1);
            section.PageSetup.BottomMargin = Unit.FromInch(1);
            var header = section.Headers.Primary.AddParagraph();
            if (IsFdsIm())
            {
                section.PageSetup.TopMargin = Unit.FromInch(1.25);
                header.Format.Alignment = ParagraphAlignment.Center;
                header.Format.Font.Size = report.Columns[0].FontSize + 2;
                header.Format.Font.Bold = true;
                header.AddText("CUI");
                header.Format.SpaceAfter = Unit.FromInch(0.15);
                header = section.Headers.Primary.AddParagraph();
            }
            header.Format.Alignment = ParagraphAlignment.Center;
            header.Format.Font.Size = report.Columns[0].FontSize + 2;
            header.Format.Font.Bold = true;
            header.AddText(siteid);
            header.AddText(" - ");
            header.AddText(report.PointGroupName);
            var footer = section.Footers.Primary.AddParagraph();
            Unit tabStopPosition = section.PageSetup.PageWidth - section.PageSetup.LeftMargin - section.PageSetup.RightMargin;
            footer.Format.TabStops.ClearAll();
            footer.Format.TabStops.AddTabStop(tabStopPosition, MigraDoc.DocumentObjectModel.TabAlignment.Right);
            footer.AddDateField(datePattern);
            footer.AddTab();
            footer.AddText("Page ");
            footer.AddPageField();
            footer.AddText(" of ");
            footer.AddNumPagesField();
            if (IsFdsIm())
            {
                section.PageSetup.BottomMargin = Unit.FromInch(1.25);
                footer = section.Footers.Primary.AddParagraph();
                footer.Format.Alignment = ParagraphAlignment.Center;
                footer.Format.Font.Size = report.Columns[0].FontSize + 2;
                footer.Format.Font.Bold = true;
                footer.AddText("CUI");
                footer.Format.SpaceBefore = Unit.FromInch(0.15);
            }

            var table = section.AddTable();
            table.Borders.Width = Unit.FromPoint(.5);
            table.Borders.Color = Colors.Silver;
            table.TopPadding = Unit.FromPoint(5);
            table.BottomPadding = Unit.FromPoint(5);
            #endregion

            // Calculate scale for fit-to-page
            double printableWidth;

            if (report.IsLandscape)
                printableWidth = 720; // (pageWidth - margins) * 72, (11-1) * 72
            else
                printableWidth = 540; // (pageWidth - margins) * 72, (8.5-1) * 72

            if (fitToPage)
            {
                double summedWidth = 0;
                foreach (var column in report.Columns)
                {
                    summedWidth += (column.Width);
                }

                scale = printableWidth / summedWidth;


                if (scale > 1)
                    scale = 1;
                else if (scale < .3) // 30%-100% valid range
                    scale = .3;
            }

            // build header
            double runningWidth = 0;
            int stopColIndex = 0;
            foreach (var column in report.Columns)
            {
                runningWidth += column.Width * scale;
                
                // if the running width exceeds printable width, stop adding columns so we don't run off the page
                if (runningWidth > printableWidth)
                {
                    // go back one iteration to get the true total width
                    runningWidth -= column.Width * scale;

                    // disribute the remaining width on either side of the table so the page is centered
                    var remainingPixels = printableWidth - runningWidth;
                    section.PageSetup.LeftMargin += Unit.FromPoint(remainingPixels / 2);
                    section.PageSetup.RightMargin += Unit.FromPoint(remainingPixels / 2);
                    break;
                }
                table.AddColumn(Unit.FromPoint(column.Width * scale));
                stopColIndex++;
            }

            var headerRow = table.AddRow();
            headerRow.HeadingFormat = true;
            var colIndex = 0;
            foreach (var column in report.Columns)
            {
                // if we've reached the end of the page, stop adding columns
                if (colIndex == stopColIndex)
                    break;
                headerRow[colIndex].Format.Font.Bold = true;
                var paragraph = headerRow[colIndex++].AddParagraph(Uri.UnescapeDataString(column.Name));
                paragraph.Format.Font.Size = Unit.FromPoint(column.FontSize * scale);

                if (string.IsNullOrEmpty(column.Name))
                { continue; }

                switch (column.HeaderAlignment)
                {
                    case PenAlignment.Left:
                        paragraph.Format.Alignment = ParagraphAlignment.Left;
                        break;

                    case PenAlignment.Right:
                        paragraph.Format.Alignment = ParagraphAlignment.Right;
                        break;

                    default:
                        paragraph.Format.Alignment = ParagraphAlignment.Center;
                        break;
                }
            }

            // build rows
            foreach (var row in report.Rows)
            {
                if (RowIsFiltered(row, report.Columns, points, values))
                {
                    continue;
                }

                var dataRow = table.AddRow();
                dataRow.HeightRule = RowHeightRule.AtLeast;
                dataRow.Height = Unit.FromPoint(1);
                colIndex = 0;
                dataRow[0].Format.Font.Bold = true;

                foreach (var column in report.Columns)
                {
                    if (colIndex == stopColIndex)
                        break;

                    var paragraph = dataRow[colIndex++].AddParagraph();
                    paragraph.Format.Font.Size = Unit.FromPoint(column.FontSize * scale);
                    paragraph.Format.LineSpacingRule = LineSpacingRule.Single;

                    if (string.IsNullOrEmpty(column.Name))
                    { continue; }

                    switch (column.CellAlignment)
                    {
                        case PenAlignment.Left:
                            paragraph.Format.Alignment = ParagraphAlignment.Left;
                            break;

                        case PenAlignment.Right:
                            paragraph.Format.Alignment = ParagraphAlignment.Right;
                            break;

                        default:
                            paragraph.Format.Alignment = ParagraphAlignment.Center;
                            break;
                    }
                    if (row.Type == "subtotal")
                    {
                        paragraph.Format.Font.Bold = true;
                        paragraph.Format.Font.Italic = true;
                        if (column.Field == "point")
                        {
                            paragraph.AddText("Subtotal");
                        }
                        if (column.TotalizerConfig != null && column.TotalizerConfig.Count > 0)
                        {
                            AddSubtotalValue(paragraph, report, row, column, points, values, numFormatInfo, datePattern, siteTimeZone);
                        }
                    }
                    else if (row.Type == "total")
                    {
                        paragraph.Format.Font.Bold = true;
                        paragraph.Format.Font.Italic = true;
                        if (column.Field == "point")
                        {
                            paragraph.AddText("Total");
                        }
                        if (column.TotalizerConfig != null && column.TotalizerConfig.Count > 0)
                        {
                            AddSubtotalValue(paragraph, report, row, column, points, values, numFormatInfo, datePattern, siteTimeZone);
                        }
                    }
                    else if (row.Type != "blank" && row.Type != "empty")
                    {
                        switch (column.Field)
                        {
                            case "point":
                                paragraph.AddText(row.Point);
                                break;
                            case "ProductID":
                                paragraph.AddText(GetPropertyString(values, row.PointGuid, "ProductID"));
                                break;
                            case "ProductDescription":
                                paragraph.AddText(GetPropertyString(values, row.PointGuid, "ProductDescription"));
                                break;
                            default:
                                // find the PointValue for this column
                                var pointValue = points.FirstOrDefault(x => x.PointID == row.Point && x.ID == column.Field);
                                #region uncomment this to add support for alarm priority coloring
                                //var alarmPriority = alarmPriorities.FirstOrDefault(x => x.AlarmPriorityGuid == pointValue.AlarmPriorityGuid);
                                //dataRow[colIndex - 1].Shading.Color = Color.Parse("#" + (alarmPriority?.BackgroundSteady ?? "FFFFFF"));
                                //dataRow[colIndex - 1].Format.Font.Color = Color.Parse("#" + (alarmPriority?.TextSteady ?? "000000"));
                                #endregion
                                if (pointValue != null)
                                {
                                    AddValueToParagraph(paragraph, new OperatePointValue(pointValue), column, numFormatInfo, datePattern, useThousandsSeparator, siteTimeZone);
                                }
                                break;
                        }
                    }
                }
            }

            migraDoc.Info.Title = "Point Group Report - " + report.PointGroupName;

            var pdf = new PdfDocumentRenderer();
            pdf.Document = migraDoc;
            pdf.RenderDocument();
            return pdf.PdfDocument;
        }

        public static string GetPropertyString(List<OperatePointValue> propertyData, string pointGuid, string id)
        {
            if (!string.IsNullOrEmpty(pointGuid))
            {
                var property = propertyData.FirstOrDefault(x => x.PropertyID == id && x.PointGuid == new Guid(pointGuid));
                if (property != null)
                {
                    if (!property.Access.View)
                    {
                        return "Restricted";
                    }
                    else
                    {
                        if (property.Value == null || string.IsNullOrEmpty(property.Value.ToString()))
                        {
                            return "Unknown";
                        }
                        else
                        {
                            return property.Value.ToString();
                        }
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
        public static string GetValueString(OperatePointValue pointValue, Column column, NumberFormatInfo numFormatInfo, string datePattern, bool useThousandsSeparator, string siteTimeZone)
        {
            if (!pointValue.Access.View)
            {
                return "Restricted";
            }
            else
            {
                if (pointValue.Value == null || string.IsNullOrEmpty(pointValue.Value.ToString()))
                {
                    var status = (uint)pointValue.Status;
                    var statusText = Opc.Ua.StatusCodes.GetBrowseName(status);
                    if (statusText == "Good")
                    {
                        statusText = "Unknown";
                    }
                    return statusText;
                }
                else
                {
                    var value = pointValue.Value;
                    var units = -1;
                    if (column.Unit != -1)
                    {
                        units = column.Unit;
                        //convert the value to the specified unit
                        double refValue = 0.0;
                        value = Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnits.Convert((double)pointValue.Value, pointValue.Units, (Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnit)units, refValue);
                    }
                    else
                    {
                        units = (int)pointValue.Units;
                    }

                    if (column.DecimalPlaces == -1)
                    {
                        numFormatInfo.NumberDecimalDigits = pointValue.DecimalPlaces;
                    }
                    else
                    {
                        numFormatInfo.NumberDecimalDigits = column.DecimalPlaces;
                    }

                    //don't use thousands separator
                    if (!useThousandsSeparator)
                    {
                        numFormatInfo.NumberGroupSeparator = string.Empty;
                        numFormatInfo.NumberGroupSizes = new int[] { 0 };
                    }

                    //format the value
                    if (pointValue.Value == null)
                    {
                        value = string.Empty;
                    }
                    else if (pointValue.ValueTypeString == "System.DateTimeOffset")
                    {
                        var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(siteTimeZone);
                        var dto = TimeZoneInfo.ConvertTime((DateTimeOffset)pointValue.Value, siteTimeZoneInfo);
                        value = dto.ToString(datePattern);
                    }

                    else if (pointValue.ValueTypeString == "System.DateTime")
                    {
                        value = ((DateTime)pointValue.Value).ToString(datePattern);
                    }
                    else
                    {
                        value = PointManager.FormatValue(Type.GetType(pointValue.ValueTypeString), (Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnit)units, numFormatInfo, value);
                    }

                    var outputText = value.ToString();
                    if (column.ShowUnit)
                    {
                        outputText += " [" + Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnits.GetUnitAbbreviation((Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnit)units) + "]";
                    }
                    if (column.ShowQuality && !string.IsNullOrEmpty(pointValue.QualityAbbreviation))
                    {
                        outputText += " [" + pointValue.QualityAbbreviation + "]";
                    }
                    return outputText;
                }
            }
        }
        public static void AddValueToParagraph(Paragraph paragraph, OperatePointValue pointValue, Column column, NumberFormatInfo numFormatInfo, string datePattern, bool useThousandsSeparator, string siteTimeZone)
        {
            // static point group
            if (!pointValue.Access.View)
            {
                paragraph.AddText("Restricted");
            }
            else
            {
                if (pointValue.Value == null || string.IsNullOrEmpty(pointValue.Value.ToString()))
                {
                    var status = (uint)pointValue.Status;
                    var browseName = Opc.Ua.StatusCodes.GetBrowseName(status);
                    // allow status names to be broken if too large for the table cell
                    browseName = Regex.Replace(browseName, "([a-z])([A-Z])", "$1\u200C$2");
                    if (browseName == "Good")
                    {
                        browseName = "Unknown";
                    }
                    paragraph.AddText(browseName);
                }
                else
                {
                    var value = pointValue.Value;
                    var units = -1;
                    if (column.Unit != -1)
                    {
                        units = column.Unit;
                        //convert the value to the specified unit
                        double refValue = 0.0;
                        value = Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnits.Convert((double)pointValue.Value, pointValue.Units, (Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnit)units, refValue);
                    }
                    else
                    {
                        units = (int)pointValue.Units;
                    }

                    if (column.DecimalPlaces == -1)
                    {
                        numFormatInfo.NumberDecimalDigits = pointValue.DecimalPlaces;
                    }
                    else
                    {
                        numFormatInfo.NumberDecimalDigits = column.DecimalPlaces;
                    }

                    //don't use thousands separator
                    if (!useThousandsSeparator)
                    {
                        numFormatInfo.NumberGroupSeparator = string.Empty;
                        numFormatInfo.NumberGroupSizes = new int[] { 0 };
                    }

                    //format the value
                    if (pointValue.Value == null)
                    {
                        value = string.Empty;
                    }
                    else if (pointValue.ValueTypeString == "System.DateTimeOffset")
                    {
                        var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(siteTimeZone);
                        var dto = TimeZoneInfo.ConvertTime((DateTimeOffset)pointValue.Value, siteTimeZoneInfo);
                        value = dto.ToString(datePattern);
                    }
                    else if (pointValue.ValueTypeString == "System.DateTime")
                    {
                        value = ((DateTime)pointValue.Value).ToString(datePattern);
                    }
                    else
                    {
                        value = PointManager.FormatValue(Type.GetType(pointValue.ValueTypeString), (Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnit)units, numFormatInfo, value);
                    }

                    var outputText = value.ToString();
                    if (!string.IsNullOrEmpty(outputText))
                    {
                        paragraph.AddText(outputText);
                        if (column.ShowUnit)
                        {
                            var formatted = paragraph.AddFormattedText(Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnits.GetUnitAbbreviation((Varec.CommonComponents.EngineeringUnitsLibrary.EngineeringUnit)units));
                            formatted.Subscript = true;
                        }

                        if (column.ShowQuality && !string.IsNullOrEmpty(pointValue.QualityAbbreviation))
                        {
                            var formatted = paragraph.AddFormattedText(pointValue.QualityAbbreviation);
                            formatted.Superscript = true;
                        }
                    }
                }
            }
        }

        public static bool RowIsFiltered(Row row, List<Column> columns, List<PointValue> points, List<OperatePointValue> values)
        {
            if (row.Point == "Total" || row.Point == "Subtotal")
            {
                return false;
            }
            foreach (Column column in columns)
            {
                if (string.IsNullOrEmpty(column.Field))
                { continue; }
                if (column.Field == "point")
                { continue; }
                if (column.Filter == null)
                { continue; }

                try
                {
                    var filter = column.Filter.ToObject<PointGroupTypeCheckerFilterRules>();
                    if (filter != null)
                    {
                        object value = null;
                        var valueTypeString = "System.String";
                        switch (column.Field)
                        {
                            case "ProductID":
                                value = GetPropertyString(values, row.PointGuid, "ProductID");
                                break;
                            case "ProductDescription":
                                value = GetPropertyString(values, row.PointGuid, "ProductDescription");
                                break;
                            default:
                                // find the PointValue for this column
                                var pointValue = points.FirstOrDefault(x => x.PointID == row.Point && x.ID == column.Field);
                                value = pointValue?.Value;
                                valueTypeString = pointValue?.ValueTypeString;
                                break;
                        }

                        var isFiltered = false;
                        switch (filter.Type)
                        {
                            case "boolean":
                                var boolFilter = column.Filter.ToObject<PointGroupBooleanFilterRules>();
                                isFiltered = boolFilter.IsFiltered(value, valueTypeString);
                                break;
                            case "datetimeoffset":
                                var dateTimeOffsetFilter = column.Filter.ToObject<PointGroupDateTimeOffsetFilterRules>();
                                isFiltered = dateTimeOffsetFilter.IsFiltered(value, valueTypeString);
                                break;
                            case "enum":
                                var enumFilter = column.Filter.ToObject<PointGroupEnumFilterRules>();
                                isFiltered = enumFilter.IsFiltered(value, valueTypeString);
                                break;
                            case "numeric":
                                var numericFilter = column.Filter.ToObject<PointGroupNumericFilterRules>();
                                isFiltered = numericFilter.IsFiltered(value, valueTypeString);
                                break;
                            case "string":
                                var stringFilter = column.Filter.ToObject<PointGroupStringFilterRules>();
                                isFiltered = stringFilter.IsFiltered(value, valueTypeString);
                                break;
                            default:
                                Trace.WriteLine("Unsupported Type: " + filter.Type);
                                break;
                        }

                        if (isFiltered)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }

            return false;
        }

        private static void AddSubtotalValue(Paragraph paragraph, PointGroupReport report, Row row, Column column, List<PointValue> points, List<OperatePointValue> values, NumberFormatInfo numFormatInfo, string datePattern, string siteTimeZone)
        {
            if (column.TotalizerConfig != null && column.TotalizerConfig.Keys.Contains(row.TotalizerGuid))
            {
                var calculationType = column.TotalizerConfig[row.TotalizerGuid];
                var decimalPlaces = 0;
                if (!string.IsNullOrEmpty(calculationType))
                {
                    object value = null;
                    var count = 0;
                    EngineeringUnit units = EngineeringUnit.FmuNone;
                    var valueTypeString = string.Empty;
                    foreach (var rowData in report.Rows)
                    {
                        if (RowIsFiltered(rowData, report.Columns, points, values))
                        {
                            continue;
                        }

                        var pointName = rowData.Point;
                        if (!string.IsNullOrEmpty(pointName))
                        {
                            if (pointName == "Total")
                            {
                                // there's only one, we don't need to do anything here
                            }
                            else if (pointName == "Subtotal")
                            {
                                if (row.Point != "Total")
                                {
                                    if (rowData.TotalizerGuid == row.TotalizerGuid)
                                    {
                                        // the correct subtotal is done being processed so we can break and use the stored value
                                        break;
                                    }
                                    else
                                    {
                                        // wrong group of points, so reset and start over
                                        value = null;
                                        count = 0;
                                    }
                                }
                            }
                            else
                            {
                                var pointValue = points.FirstOrDefault(x => x.PointID == pointName && x.ID == column.Field);
                                if (pointValue != null && pointValue.Value != null)
                                {
                                    switch (pointValue.ValueTypeString)
                                    {
                                        case "System.Int16":
                                        case "System.Int32":
                                        case "System.Int64":
                                            if (valueTypeString == string.Empty)
                                            {
                                                units = pointValue.Units;
                                                decimalPlaces = pointValue.DecimalPlaces;
                                                valueTypeString = pointValue.ValueTypeString;
                                            }
                                            switch (calculationType)
                                            {
                                                case "sum":
                                                case "avg":
                                                    var intSum = Convert.ToInt64(pointValue.Value);
                                                    value = value == null ? intSum : Convert.ToInt64(value) + intSum;
                                                    count += 1;
                                                    break;
                                                case "max":
                                                    var intMax = Convert.ToInt64(pointValue.Value);
                                                    value = value == null || intMax > Convert.ToInt64(value) ? intMax : Convert.ToInt64(value);
                                                    break;
                                                case "min":
                                                    var intMin = Convert.ToInt64(pointValue.Value);
                                                    value = value == null || intMin < Convert.ToInt64(value) ? intMin : Convert.ToInt64(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "System.UInt16":
                                        case "System.UInt32":
                                        case "System.UInt64":
                                            if (valueTypeString == string.Empty)
                                            {
                                                units = pointValue.Units;
                                                decimalPlaces = pointValue.DecimalPlaces;
                                                valueTypeString = pointValue.ValueTypeString;
                                            }
                                            switch (calculationType)
                                            {
                                                case "sum":
                                                case "avg":
                                                    var uintSum = Convert.ToUInt64(pointValue.Value);
                                                    value = value == null ? uintSum : Convert.ToUInt64(value) + uintSum;
                                                    count += 1;
                                                    break;
                                                case "max":
                                                    var uintMax = Convert.ToUInt64(pointValue.Value);
                                                    value = value == null || uintMax > Convert.ToUInt64(value) ? uintMax : Convert.ToUInt64(value);
                                                    break;
                                                case "min":
                                                    var uintMin = Convert.ToUInt64(pointValue.Value);
                                                    value = value == null || uintMin < Convert.ToUInt64(value) ? uintMin : Convert.ToUInt64(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "System.Double":
                                        case "System.Single":
                                            if (valueTypeString == string.Empty)
                                            {
                                                units = pointValue.Units;
                                                decimalPlaces = pointValue.DecimalPlaces;
                                                valueTypeString = pointValue.ValueTypeString;
                                            }
                                            switch (calculationType)
                                            {
                                                case "sum":
                                                case "avg":
                                                    var dblSum = Convert.ToDouble(pointValue.Value);
                                                    value = value == null ? dblSum : Convert.ToDouble(value) + dblSum;
                                                    count += 1;
                                                    break;
                                                case "max":
                                                    var dblMax = Convert.ToDouble(pointValue.Value);
                                                    value = value == null || dblMax > Convert.ToDouble(value) ? dblMax : Convert.ToDouble(value);
                                                    break;
                                                case "min":
                                                    var dblMin = Convert.ToDouble(pointValue.Value);
                                                    value = value == null || dblMin < Convert.ToDouble(value) ? dblMin : Convert.ToDouble(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "System.DateTimeOffset":
                                            if (valueTypeString == string.Empty)
                                            {
                                                units = pointValue.Units;
                                                valueTypeString = pointValue.ValueTypeString;
                                            }
                                            switch (calculationType)
                                            {
                                                case "max":
                                                    var dtoMax = (DateTimeOffset)pointValue.Value;
                                                    value = value == null || dtoMax > (DateTimeOffset)value ? dtoMax : (DateTimeOffset)value;
                                                    break;
                                                case "min":
                                                    var dtoMin = (DateTimeOffset)pointValue.Value;
                                                    value = value == null || dtoMin < (DateTimeOffset)value ? dtoMin : (DateTimeOffset)value;
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    if (calculationType == "avg")
                    {
                        if (value is Int64)
                        {
                            value = (Int64)value / count;
                        }
                        else if (value is UInt64)
                        {
                            value = (UInt64)value / (UInt64)count;
                        }
                        else if (value is double)
                        {
                            value = (double)value / count;
                        }
                    }

                    if (column.Unit != -1 && value != null)
                    {
                        //convert the value to the specified unit
                        double refValue = 0.0;
                        value = EngineeringUnits.Convert((double)value, units, (EngineeringUnit)column.Unit, refValue);
                        units = (EngineeringUnit)column.Unit;
                    }

                    if (column.DecimalPlaces == -1)
                    {
                        numFormatInfo.NumberDecimalDigits = decimalPlaces;
                    }
                    else
                    {
                        numFormatInfo.NumberDecimalDigits = column.DecimalPlaces;
                    }

                    //format the value
                    if (value == null)
                    {
                        value = string.Empty;
                    }
                    else if (valueTypeString == "System.DateTimeOffset")
                    {
                        var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(siteTimeZone);
                        var dto = TimeZoneInfo.ConvertTime((DateTimeOffset)value, siteTimeZoneInfo);
                        value = dto.ToString(datePattern);
                    }
                    else if (valueTypeString == "System.DateTime")
                    {
                        value = ((DateTime)value).ToString(datePattern);
                    }
                    else
                    {
                        value = PointManager.FormatValue(Type.GetType(valueTypeString), (EngineeringUnit)units, numFormatInfo, value);
                    }


                    paragraph.AddText(value.ToString());
                    if (column.ShowUnit && !string.IsNullOrEmpty(value?.ToString()))
                    {
                        var formatted = paragraph.AddFormattedText(EngineeringUnits.GetUnitAbbreviation((EngineeringUnit)units));
                        formatted.Subscript = true;
                    }

                }
            }
        }

        private static string GetSubtotalValue(PointGroupReport report, Row row, Column column, List<PointValue> points, List<OperatePointValue> values, NumberFormatInfo numFormatInfo, string datePattern, string siteTimeZone)
        {
            var valueString = string.Empty;

            if (column.TotalizerConfig != null && column.TotalizerConfig.Keys.Contains(row.TotalizerGuid))
            {
                var calculationType = column.TotalizerConfig[row.TotalizerGuid];
                var decimalPlaces = 0;
                if (!string.IsNullOrEmpty(calculationType))
                {
                    object value = null;
                    var count = 0;
                    EngineeringUnit units = EngineeringUnit.FmuNone;
                    var valueTypeString = string.Empty;
                    foreach (var rowData in report.Rows)
                    {
                        if (RowIsFiltered(rowData, report.Columns, points, values))
                        {
                            continue;
                        }

                        var pointName = rowData.Point;
                        if (!string.IsNullOrEmpty(pointName))
                        {
                            if (pointName == "Total")
                            {
                                // there's only one, we don't need to do anything here
                            }
                            else if (pointName == "Subtotal")
                            {
                                if (row.Point != "Total")
                                {
                                    if (rowData.TotalizerGuid == row.TotalizerGuid)
                                    {
                                        // the correct subtotal is done being processed so we can break and use the stored value
                                        break;
                                    }
                                    else
                                    {
                                        // wrong group of points, so reset and start over
                                        value = null;
                                        count = 0;
                                    }
                                }
                            }
                            else
                            {
                                var pointValue = points.FirstOrDefault(x => x.PointID == pointName && x.ID == column.Field);
                                if (pointValue != null && pointValue.Value != null)
                                {
                                    switch (pointValue.ValueTypeString)
                                    {
                                        case "System.Int16":
                                        case "System.Int32":
                                        case "System.Int64":
                                            if (valueTypeString == string.Empty)
                                            {
                                                units = pointValue.Units;
                                                decimalPlaces = pointValue.DecimalPlaces;
                                                valueTypeString = pointValue.ValueTypeString;
                                            }
                                            switch (calculationType)
                                            {
                                                case "sum":
                                                case "avg":
                                                    var intSum = Convert.ToInt64(pointValue.Value);
                                                    value = value == null ? intSum : Convert.ToInt64(value) + intSum;
                                                    count += 1;
                                                    break;
                                                case "max":
                                                    var intMax = Convert.ToInt64(pointValue.Value);
                                                    value = value == null || intMax > Convert.ToInt64(value) ? intMax : Convert.ToInt64(value);
                                                    break;
                                                case "min":
                                                    var intMin = Convert.ToInt64(pointValue.Value);
                                                    value = value == null || intMin < Convert.ToInt64(value) ? intMin : Convert.ToInt64(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "System.UInt16":
                                        case "System.UInt32":
                                        case "System.UInt64":
                                            if (valueTypeString == string.Empty)
                                            {
                                                units = pointValue.Units;
                                                decimalPlaces = pointValue.DecimalPlaces;
                                                valueTypeString = pointValue.ValueTypeString;
                                            }
                                            switch (calculationType)
                                            {
                                                case "sum":
                                                case "avg":
                                                    var uintSum = Convert.ToUInt64(pointValue.Value);
                                                    value = value == null ? uintSum : Convert.ToUInt64(value) + uintSum;
                                                    count += 1;
                                                    break;
                                                case "max":
                                                    var uintMax = Convert.ToUInt64(pointValue.Value);
                                                    value = value == null || uintMax > Convert.ToUInt64(value) ? uintMax : Convert.ToUInt64(value);
                                                    break;
                                                case "min":
                                                    var uintMin = Convert.ToUInt64(pointValue.Value);
                                                    value = value == null || uintMin < Convert.ToUInt64(value) ? uintMin : Convert.ToUInt64(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "System.Double":
                                        case "System.Single":
                                            if (valueTypeString == string.Empty)
                                            {
                                                units = pointValue.Units;
                                                decimalPlaces = pointValue.DecimalPlaces;
                                                valueTypeString = pointValue.ValueTypeString;
                                            }
                                            switch (calculationType)
                                            {
                                                case "sum":
                                                case "avg":
                                                    var dblSum = Convert.ToDouble(pointValue.Value);
                                                    value = value == null ? dblSum : Convert.ToDouble(value) + dblSum;
                                                    count += 1;
                                                    break;
                                                case "max":
                                                    var dblMax = Convert.ToDouble(pointValue.Value);
                                                    value = value == null || dblMax > Convert.ToDouble(value) ? dblMax : Convert.ToDouble(value);
                                                    break;
                                                case "min":
                                                    var dblMin = Convert.ToDouble(pointValue.Value);
                                                    value = value == null || dblMin < Convert.ToDouble(value) ? dblMin : Convert.ToDouble(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "System.DateTimeOffset":
                                            if (valueTypeString == string.Empty)
                                            {
                                                units = pointValue.Units;
                                                valueTypeString = pointValue.ValueTypeString;
                                            }
                                            switch (calculationType)
                                            {
                                                case "max":
                                                    var dtoMax = (DateTimeOffset)pointValue.Value;
                                                    value = value == null || dtoMax > (DateTimeOffset)value ? dtoMax : (DateTimeOffset)value;
                                                    break;
                                                case "min":
                                                    var dtoMin = (DateTimeOffset)pointValue.Value;
                                                    value = value == null || dtoMin < (DateTimeOffset)value ? dtoMin : (DateTimeOffset)value;
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    if (calculationType == "avg")
                    {
                        if (value is Int64)
                        {
                            value = (Int64)value / count;
                        }
                        else if (value is UInt64)
                        {
                            value = (UInt64)value / (UInt64)count;
                        }
                        else if (value is double)
                        {
                            value = (double)value / count;
                        }
                    }

                    if (column.Unit != -1 && value != null)
                    {
                        //convert the value to the specified unit
                        double refValue = 0.0;
                        value = EngineeringUnits.Convert((double)value, units, (EngineeringUnit)column.Unit, refValue);
                        units = (EngineeringUnit)column.Unit;
                    }

                    if (column.DecimalPlaces == -1)
                    {
                        numFormatInfo.NumberDecimalDigits = decimalPlaces;
                    }
                    else
                    {
                        numFormatInfo.NumberDecimalDigits = column.DecimalPlaces;
                    }

                    //format the value
                    if (value == null)
                    {
                        value = string.Empty;
                    }
                    else if (valueTypeString == "System.DateTimeOffset")
                    {
                        var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(siteTimeZone);
                        var dto = TimeZoneInfo.ConvertTime((DateTimeOffset)value, siteTimeZoneInfo);
                        value = dto.ToString(datePattern);
                    }
                    else if (valueTypeString == "System.DateTime")
                    {
                        value = ((DateTime)value).ToString(datePattern);
                    }
                    else
                    {
                        value = PointManager.FormatValue(Type.GetType(valueTypeString), (EngineeringUnit)units, numFormatInfo, value);
                    }

                    valueString = value.ToString();
                    if (column.ShowUnit && !string.IsNullOrEmpty(value?.ToString()))
                    {
                        valueString += " [" + EngineeringUnits.GetUnitAbbreviation((EngineeringUnit)units) + "]";
                    }
                }
            }
            return valueString;
        }


        public static bool IsFdsIm()
        {
            return ApplicationInformation.IsFDSIM;
        }

        public static string TranslatedText(string originalText, SecurityClass security, bool useDataDictionary)
        {
            string returnText = originalText;

            if (useDataDictionary)
            {
                if ((security != null))
                {
                    Guid siteGuid = security.SiteGuid;
                    returnText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(security.SiteGuid, originalText));
                }
            }
            else
            {
                returnText = new DataDictionaryCollectionClass()[originalText];
            }

            return returnText;
        }

        private static string GetTranslatedText(string text, SecurityClass security)
        {
            return TranslatedText(text, security, true);
        }

        public static void FormatValue(PointValue pointValue, string siteTimeZone, SecurityClass security)
        {
            if (pointValue.Value != null)
            {

                if (pointValue.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") != -1)
                {
                    pointValue.Value = GetTranslatedText(FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString((Enum)pointValue.Value), security);
                }
                else if (pointValue.ValueTypeString == "System.Boolean")
                {
                    pointValue.Value = ((bool)pointValue.Value) ? GetTranslatedText("True", security) : GetTranslatedText("False", security);
                }
                else if (pointValue.ValueTypeString == "System.DateTimeOffset")
                {
                    var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(siteTimeZone);
                    pointValue.Value = TimeZoneInfo.ConvertTime((DateTimeOffset)pointValue.Value, siteTimeZoneInfo);
                }
                else if ((pointValue.Value is double || pointValue.Value is float) && Double.IsNaN(Convert.ToDouble(pointValue.Value)))
                {
                    pointValue.Value = "NaN";
                }
            }
        }
    }
}

