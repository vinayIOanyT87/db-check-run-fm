namespace FuelsManager.Areas.UserAdministrationArea.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FuelsManager.Areas.Controllers;
    using FuelsManager.Areas.UserAdministrationArea.ViewModels;

    using Newtonsoft.Json;

    public class UserConfigurationAdminAuditController : FMBaseController
    {
        #region Data members
        public const string TotalRecordCountSessionKey = "AdminAuditController.TotalRecordCount";
        private readonly EventLog eventLog;
        private readonly string[] typeIds = {
                "Additive Profile - Additive",
                "Additive Profiles",
                "Alarm And Events",
                "Alarm Priorities",
                "Allocation - LineItem",
                "Allocations",
                "Application Strings",
                "Archived Users",
                "Asset Tracking Detail",
                "Asset Tracking Device",
                "Asset Tracking Icon Configuration",
                "Asset Tracking Map Configuration",
                "Auto Distribution Reason Code",
                "Auto Distribution Rule",
                "Auto Distribution Rule - Manager",
                "Auto Distribution Rule - Manager Group",
                "Auto Distribution Rule - Owner",
                "Auto Distribution Rule - Owner Group",
                "Auto Distribution Rule - Product",
                "Auto Distribution Rule - Product Group",
                "Auto Distribution Rule - Transaction Alias",
                "Closeout",
                "Companies",
                "Company - Authorized Carrier",
                "Company - Certificate and Permit",
                "Company - Product",
                "Company - Role",
                "Company - Schedule",
                "Company - Unavailable Inventory",
                "Company Group - Company",
                "Company Group - Product",
                "Configuration Settings",
                "Controller - Memo",
                "Data Dictionary",
                "Data Exchange Profiles",
                "Data Exchange Profiles - Ship To - Load ID",
                "Delivery Locations",
                "Dispatch Configuration",
                "Dispatch Grid",
                "Dispatch Grid - Column",
                "Dispatch Toolbar",
                "Dispatch Toolbar - Command",
                "E-mail Group - Category",
                "E-mail Group - E-mail Address",
                "E-mail Group - Priority",
                "E-mail Groups",
                "Enterprise Export/Import Settings",
                "Equipment",
                "Equipment - Maintenance",
                "Equipment - Process Variable",
                "Equipment - Quality Tag",
                "Equipment - Tag and License",
                "Equipment - Test and Inspection",
                "Equipment Appointment",
                "Equipment Type",
                "Equipment Type - Aircraft Tank",
                "Equipment Type - Required Qualifications",
                "Equipment Type - Required Training",
                "Export Requests",
                "External Station",
                "External Station General Configuration",
                "External Station - Product",
                "FCE Device",
                "FCEE Mapping",
                "Field Level Configuration",
                "Fuel Card",
                "Gates",
                "General Configuration",
                "House Cards",
                "Ledger Aggregate Column",
                "Ledger View - Product",
                "Ledger View - User Group",
                "List View - List View Field",
                "List Views",
                "Load Arm - Arm Permissive",
                "Load Arm - Component",
                "Load Arm - Component Permissive",
                "Load Arm - External Component",
                "Load Arm - External Component Blend Percentage",
                "Load Arm - External Component Permissive",
                "Load Arm - Injector",
                "Load Arm - Injector Permissive",
                "Load Arm - No Additive Permissive",
                "Load Arm - Process Variable",
                "Load Arm - Recipe",
                "Load Arm - Recipe Permissive",
                "Loading Hierarchy",
                "Loading Hierarchy - Load ID",
                "Maintenance Reasons",
                "Message",
                "Meters",
                "Notes",
                "Off-Loading Hierarchy",
                "Off-Loading Hierarchy - Load ID",
                "OPC Connections",
                "Owner Closeout",
                "Person - Schedule",
                "Personnel",
                "Personnel - License",
                "Personnel - Qualifications",
                "Personnel - Role",
                "Personnel - Training",
                "Personnel Appointment",
                "Product - Dot Hazardous Message",
                "Product - Footnote",
                "Product - Product Message",
                "Product Blend - Component",
                "Product Group - Entry Message",
                "Product Group - Exit Message",
                "Product Group - Product",
                "Products",
                "Qualifications",
                "Quality Tags",
                "Query Default Fields",
                "Query Settings",
                "Query Storage",
                "Query Storage - User Group",
                "Report - User Group",
                "Report Assignment",
                "Report Groups",
                "Reserve Level",
                "Ship To - Footnote",
                "Shipper - Footnote",
                "Site - Additive Profile",
                "Site - Alarm & Events",
                "Site - Alarm Event Category",
                "Site - Alarm Priority",
                "Site - All Report Configuration",
                "Site - Allocation Group",
                "Site - Auto Distribution Reason Code",
                "Site - Auto Distribution Rule",
                "Site - Company",
                "Site - Company Certificate And Permit",
                "Site - Company Group",
                "Site - Company Type",
                "Site - Data Dictionary",
                "Site - Delivery Location",
                "Site - Dispatch Configuration",
                "Site - Dot Hazardous Message",
                "Site - E-mail Address",
                "Site - E-mail Group",
                "Site - Entry Message",
                "Site - Equipment",
                "Site - Equipment Appointment",
                "Site - Equipment Tag and License",
                "Site - Equipment Test and Inspection",
                "Site - Equipment Type",
                "Site - Exit Message",
                "Site - Footnote",
                "Site - Fuel Card",
                "Site - Holiday",
                "Site - Ledger Aggregate Column",
                "Site - Ledger View",
                "Site - List View",
                "Site - Person",
                "Site - Personnel Appointment",
                "Site - Personnel License",
                "Site - Personnel Qualification",
                "Site - Personnel Training",
                "Site - Process Variable",
                "Site - Process Variable Message",
                "Site - Product",
                "Site - Product Group",
                "Site - Product Message",
                "Site - Quality Tag",
                "Site - Query Settings",
                "Site - Schedule",
                "Site - Site",
                "Site - Tank Appointment",
                "Site - Test",
                "Site - Test Set",
                "Site - Transaction Alias",
                "Site - User",
                "Site - User Data Configuration",
                "Site - User Group",
                "Site Ancillary Data",
                "Sites",
                "State - Footnote",
                "Station - Load Arm",
                "Station - Permissive",
                "Station - Process Variable",
                "Station - Required Qualifications",
                "Station - Required Training",
                "Stations",
                "Synchronization Settings",
                "System Settings",
                "Tank - Maintenance",
                "Tank - Meter",
                "Tank - Process Variable",
                "Tank - Quality Tag",
                "Tank Appointment",
                "Tank Group - Tanks",
                "Tank Groups",
                "Tanks",
                "Test",
                "Test - Equipment",
                "Test - Tank",
                "Test Set",
                "Test Set - Equipment",
                "Test Set - Tank",
                "Test Set - Test",
                "Transaction Alias - Associated Alias",
                "Transaction Alias - Fields",
                "Transaction Alias - Line Item User Data",
                "Transaction Alias - Line Item User Data Fields",
                "Transaction Alias - Product Exclusion",
                "Transaction Alias - Status",
                "Transaction Alias - User Data",
                "Transaction Alias - User Data Fields",
                "Transaction Alias - User Group",
                "Transaction Alias User Data",
                "Transaction Aliases",
                "Transaction Line Item User Data",
                "Transaction Line Items",
                "Transaction Notes",
                "Transaction PIDX",
                "Transaction Signature",
                "Transaction Sub Line Items",
                "Transaction Transport Line Items",
                "Transactions",
                "User - Menu Favorite",
                "User Data",
                "User Group - Company",
                "User Group - Right",
                "User Group - User",
                "User Groups",
                "Users"
            };
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public UserConfigurationAdminAuditController()
        {
            this.eventLog = new EventLog("Application", ".", "FuelsManager");
        }
        #endregion

        #region Action methods
        /// <summary>
        /// This method will get the audit filters data for site,
        /// action ID, and type ID. It is called from the Administration
        /// & Audit UI page.
        /// </summary>
        /// <returns>Returns a results object with the data.</returns>
        [HttpPost]
        public ActionResult GetAuditFilterData()
        {
            var results = new AuditResultClass();

            try
            {
                var sourceFilterDataModel = new AuditSourceFilterDataModel
                {
                    SiteList = this.GetSiteFilterData(),
                    ActionIdList = this.GetActionIdFilterData(),
                    TypeIdList = this.GetTypeIdFilterData()
                };

                results.SourceFilterDataModel = sourceFilterDataModel;
                return this.Json(results);
            }
            catch (Exception ex)
            {
                const string ErrorMsg = "Error retrieving Site filter data.";
                results.ErrorFlag = true;
                results.ErrorMsg = ErrorMsg;

                this.eventLog.WriteEntry(ErrorMsg + ": " + ex.Message, EventLogEntryType.Error);               
                return this.Json(results);
            }
        }

        /// <summary>
        /// This method is called from the Administration & Audit UI page to retrieve
        /// ID filter data.
        /// </summary>
        /// <param name="filters">The ID filter data is dependent on other filters.</param>
        /// <returns>Returns a Result object with the information.</returns>
        [HttpPost]
        public ActionResult GetAuditFilterDataForId(string filters)
        {
            var results = new AuditResultClass();

            try
            {
                AuditViewFilterModel filterModel = this.DeserializeAuditViewFilterModel(filters);
                results.IdFilterList = this.GetIdFilterData(filterModel);

                return this.Json(results);
            }
            catch (Exception ex)
            {
                const string ErrorMsg = "Error retrieving ID filter data.";
                results.ErrorFlag = true;
                results.ErrorMsg = ErrorMsg;

                this.eventLog.WriteEntry(ErrorMsg + ": " + ex.Message, EventLogEntryType.Error);
                return this.Json(results);
            }
        }

        /// <summary>
        /// This method will retrieve the audit data based on the filter selection.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="filterInfoStr">The filters to query on.</param>
        /// <param name="draw"></param>
        /// <param name="start"></param>
        /// <returns>Return a results object with the data.</returns>
        [HttpPost]
        public ActionResult GetAuditData(string draw, int start, int length, string filterInfoStr)
        {
            var auditDataReccordList = new List<AuditDataRecordModel>();
            Guid currentSiteGuid = this.Security.SiteGuid;

            JsonResult result;

            if (string.IsNullOrEmpty(filterInfoStr))
            {
                const string ErrMsg = "The Audit View Filter object is corrupt.";
                this.eventLog.WriteEntry(ErrMsg, EventLogEntryType.Error);
                result = this.BuildDataTableResult(ErrMsg, draw, auditDataReccordList, 0, 0);
                return result;
            }

            bool includeMemberSites = false;
            AuditViewFilterModel filterModel = this.DeserializeAuditViewFilterModel(filterInfoStr);

            if (filterModel.HasDate == false)
            {
                result = this.BuildDataTableResult(string.Empty, draw, auditDataReccordList, 0, 0);
                return result;
            }

            try
            {
                Tuple<DateTimeOffset, DateTimeOffset> dateRange = this.GetFilterBeginEndDates(filterModel);

                this.Security.SiteGuid = this.ConvertToGuid(filterModel.SiteGuidStr);
                DateTimeOffset beginDate = dateRange.Item1;
                DateTimeOffset endDate = dateRange.Item2;
                int batchNumber = this.CalculateBatchRecordNumber(start, length);

                // This means that an ALL section has been made, so include member sites.
                if (this.Security.SiteGuid == Guid.Empty)
                {
                    includeMemberSites = true;
                }

                var auditLogCollection = FMChannelHelper.MakeCall<IAuditLogs, AuditLogCollectionClass>(x => x.EnumerateByBatch(
                                    this.Security,
                                    beginDate,
                                    endDate,
                                    filterModel.ActionId,
                                    filterModel.TypeId,
                                    filterModel.Id,
                                    filterModel.Source,
                                    this.UseDataDictionary,
                                    includeMemberSites,
                                    length,
                                    batchNumber));

                this.Security.SiteGuid = currentSiteGuid;

                if (auditLogCollection == null || auditLogCollection.AuditLogList.Count == 0)
                {
                    result = this.BuildDataTableResult(string.Empty, draw, auditDataReccordList, 0, 0);
                    return result;
                }

                foreach (AuditLogClass auditLog in auditLogCollection.AuditLogList)
                {
                    var auditDataRecordModel = new AuditDataRecordModel
                                                {
                                                    ActionId    = auditLog.ActionId ?? string.Empty,
                                                    TypeId      = auditLog.TypeId ?? string.Empty,
                                                    Id          = auditLog.ID ?? string.Empty,
                                                    PropertyId  = auditLog.PropertyId ?? string.Empty,
                                                    NewValue    = auditLog.NewValue ?? string.Empty,
                                                    OldValue    = auditLog.OldValue ?? string.Empty,
                                                    SiteId      = auditLog.SiteID ?? string.Empty,
                                                    Source      = auditLog.SourceNode ?? string.Empty
                                                };

                    if (auditLog.AuditedDate != null)
                    {
                        auditDataRecordModel.AuditDateTime = (DateTimeOffset)(auditLog.AuditedDate);
                    }

                    auditDataReccordList.Add(auditDataRecordModel);
                }

                // Total record count.
                int totalRecords;

                // Filter record count.
                int recFilter;

                // Save the full record count for the first set of records, because
                // that is the only time it is returned from service for performance
                // reasons.
                if (batchNumber == 1)
                {
                    this.Session.Add(TotalRecordCountSessionKey, auditLogCollection.FullRecordCount);
                    totalRecords = auditLogCollection.FullRecordCount;
                    recFilter = auditLogCollection.FullRecordCount;
                }
                else
                {
                    totalRecords = (int)this.Session[TotalRecordCountSessionKey];
                    recFilter = (int)this.Session[TotalRecordCountSessionKey];
                }

                // Loading drop down lists. 
                result = this.BuildDataTableResult(string.Empty, draw, auditDataReccordList, totalRecords, recFilter);
                return result;
            }
            catch (Exception ex)
            {
                this.Security.SiteGuid = currentSiteGuid;
                string errorMsg = "Error retrieving audit data. " + ex.Message;
                this.eventLog.WriteEntry(errorMsg);
                result = this.BuildDataTableResult(errorMsg, draw, auditDataReccordList, 0, 0);
                return result;
            }        
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will convert a GUID string into a GUID.
        /// </summary>
        /// <param name="inGuid"></param>
        /// <returns>If success, returns a GUID, otherwise it returns an empty GUID.</returns>
        private Guid ConvertToGuid(string inGuid)
        {
            Guid newGuid;
            if (Guid.TryParse(inGuid, out newGuid))
            {
                return newGuid;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// This method will convert a date time offset string into a date time offset.
        /// </summary>
        /// <param name="dateTimeStr">String with the date time offset value.</param>
        /// <returns>Returns a date time offset object.</returns>
        private DateTimeOffset ConvertDateTime(string dateTimeStr)
        {
            var currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                                    x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

            string dateTimeFormatStr = currentSite.ShortDatePattern + " " + currentSite.TimePattern;
            var currentDate = DateTimeOffset.Now;

            if (string.IsNullOrEmpty(dateTimeStr))
            {
                return currentDate;
            }

            DateTime outDateTime;
            if (DateTime.TryParseExact(dateTimeStr, dateTimeFormatStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
            {
                return new DateTimeOffset(outDateTime);
            }

            return currentDate;
        }

        /// <summary>
        /// This method will deserialize the audit view filter model.
        /// </summary>
        /// <param name="modelStr">Audit view filter model as a string.</param>
        /// <returns>Returns the Audit View Filter Model as an object.</returns>
        private AuditViewFilterModel DeserializeAuditViewFilterModel(string modelStr)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var model = JsonConvert.DeserializeObject<AuditViewFilterModel>(modelStr, jsonSerializerSettings);

            return model;
        }

        /// <summary>
        /// This method retrieves the sites for a given user.
        /// </summary>
        /// <returns>Returns a list of the Site Filter Model.</returns>
        private List<AuditSourceSiteFilterModel> GetSiteFilterData()
        {
            AuditSourceSiteFilterModel sourceSiteFilterModel;
            var siteList = new List<AuditSourceSiteFilterModel>();
            var currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, true, false, false));

            if (currentSite.SiteGroup)
            {
                sourceSiteFilterModel = new AuditSourceSiteFilterModel { SiteId = "{All}", SiteGuid = Guid.Empty };
                siteList.Add(sourceSiteFilterModel);

                sourceSiteFilterModel = new AuditSourceSiteFilterModel { SiteId = currentSite.ID, SiteGuid = currentSite.SiteGuid };
                siteList.Add(sourceSiteFilterModel);

                foreach (SiteToSiteMapClass childSiteMap in currentSite.SiteToSiteMapCollection)
                {
                    sourceSiteFilterModel = new AuditSourceSiteFilterModel { SiteId = childSiteMap.ChildSiteID, SiteGuid = childSiteMap.ChildSiteGuid };
                    siteList.Add(sourceSiteFilterModel);
                }
            }
            else
            {
                sourceSiteFilterModel = new AuditSourceSiteFilterModel { SiteId = currentSite.ID, SiteGuid = currentSite.SiteGuid };
                siteList.Add(sourceSiteFilterModel);
            }

            siteList.Sort((x, y) => string.Compare(x.SiteId, y.SiteId, StringComparison.Ordinal));
            return siteList;
        }

        /// <summary>
        /// This method returns a list of action IDs.
        /// </summary>
        /// <returns>Returns a list of the Action ID Filter Model.</returns>
        private List<AuditSourceActionIdFilterModel> GetActionIdFilterData()
        {
            var actionIdList = new List<AuditSourceActionIdFilterModel>();

            var sourceActionIdFilterModel = new AuditSourceActionIdFilterModel {ActionId = this.GetTranslatedText("Add"), ActionIdValue = "Add" };
            actionIdList.Add(sourceActionIdFilterModel);

            sourceActionIdFilterModel = new AuditSourceActionIdFilterModel { ActionId = this.GetTranslatedText("Modify"), ActionIdValue = "Modify" };
            actionIdList.Add(sourceActionIdFilterModel);

            sourceActionIdFilterModel = new AuditSourceActionIdFilterModel { ActionId = this.GetTranslatedText("Purge"), ActionIdValue = "Purge" };
            actionIdList.Add(sourceActionIdFilterModel);

            return actionIdList;
        }

        /// <summary>
        /// This method returns a list of thpe IDs.
        /// </summary>
        /// <returns>Returns a list of the Type ID Filter Model.</returns>
        private List<AuditSourceTypeIdFilterModel> GetTypeIdFilterData()
        {
            var typeIdList = new List<AuditSourceTypeIdFilterModel>();

            foreach (string typeId in this.typeIds)
            {
                var sourceTypeIdFilterModel = new AuditSourceTypeIdFilterModel { TypeId = this.GetTranslatedText(typeId), TypeIdValue = typeId };
                typeIdList.Add(sourceTypeIdFilterModel);
            }

            return typeIdList;
        }

        /// <summary>
        /// This method will retrieve the ID field from the audit table based on the parameters
        /// passed in.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Returns a list of AuditSourceIdFilterModels.</returns>
        private List<AuditSourceIdFilterModel> GetIdFilterData(AuditViewFilterModel filters)
        {
            var idFilterList = new List<AuditSourceIdFilterModel>();

            if (string.IsNullOrEmpty(filters.TypeId))
            {
                return idFilterList;
            }

            Guid siteGuid;
            if (Guid.TryParse(filters.SiteGuidStr, out siteGuid) == false)
            {
                return idFilterList;
            }

            if (siteGuid == Guid.Empty)
            {
                return idFilterList;
            }

            Tuple<DateTimeOffset, DateTimeOffset> dateRange = this.GetFilterBeginEndDates(filters);
            DateTimeOffset beginningDateTime = dateRange.Item1;
            DateTimeOffset endingDateTime = dateRange.Item2;

            var lstAuditLogIds = FMChannelHelper.MakeCall<IAuditLogs, List<string>>(x => x.EnumerateAuditLogIds(
                                this.Security, siteGuid, beginningDateTime, endingDateTime, filters.ActionId, filters.TypeId));

            if (lstAuditLogIds == null || lstAuditLogIds.Count == 0)
            {
                return idFilterList;
            }

            foreach (string id in  lstAuditLogIds)
            {
                if (string.IsNullOrEmpty(id) == false)
                {
                    var idFilterModel = new AuditSourceIdFilterModel { Id = id, IdValue = id };
                    idFilterList.Add(idFilterModel);
                }
            }

            return idFilterList;
        }

        /// <summary>
        /// This method will convert the filter dates from Days or an actually date string.
        /// </summary>
        /// <param name="filters">The filter that contains the dates.</param>
        /// <returns>Returns the begin and end dates.</returns>
        private Tuple<DateTimeOffset, DateTimeOffset> GetFilterBeginEndDates(AuditViewFilterModel filters)
        {
            DateTimeOffset endDate;
            DateTimeOffset beginDate;
            var dateRange = new Tuple<DateTimeOffset, DateTimeOffset>(DateTimeOffset.Now, DateTimeOffset.Now);

            if (string.IsNullOrEmpty(filters.BeginDateStr) || string.IsNullOrEmpty(filters.EndDateStr))
            {
                return dateRange;
            }

            if (filters.BeginDateStr.Contains("Days"))
            {
                string[] parts = filters.BeginDateStr.Split('_');
                if (parts.Length < 2)
                {
                    return dateRange;
                }

                int numberDays;
                if (int.TryParse(parts[1], out numberDays) == false)
                {
                    numberDays = 0;
                }

                numberDays = numberDays * -1;
                TimeSpan timeSpan = TimeSpan.FromDays(numberDays);
                endDate = DateTimeOffset.Now;
                beginDate = endDate.Add(timeSpan);

                dateRange = new Tuple<DateTimeOffset, DateTimeOffset>(beginDate, endDate);
                return dateRange;
            }

            beginDate = this.ConvertDateTime(filters.BeginDateStr);
            endDate = this.ConvertDateTime(filters.EndDateStr);
            dateRange = new Tuple<DateTimeOffset, DateTimeOffset>(beginDate, endDate);

            return dateRange;
        }

        /// <summary>
        /// This method will calculate the next batch number that is used in the
        /// SP to retrieve the next set of records. It is calculated by the
        /// next starting record "start" divided by the number of records to
        /// display "length".
        /// </summary>
        /// <param name="start">Starting record number.</param>
        /// <param name="length">The number of records to display.</param>
        /// <returns></returns>
        private int CalculateBatchRecordNumber(int start, int length)
        {
            if (start == 0) return 1;

            int batchNumber = (start / length) + 1;
            return batchNumber;
        }

        /// <summary>
        /// This method will return a JSON Result object that contains an error message if it is supplied.
        /// Otherwise, it will return the result without the error message.
        /// </summary>
        /// <param name="errorMsg">The error message.</param>
        /// <param name="draw">The datatable draw number.</param>
        /// <param name="auditDataReccordList">The empty audit record list.</param>
        /// <param name="totalRecords">Total number of records for the query.</param>
        /// <param name="filterRecs">Total number of records being filtered.</param>
        /// <returns>Return a JSON Result object.</returns>
        private JsonResult BuildDataTableResult(string errorMsg, string draw, List<AuditDataRecordModel> auditDataReccordList, int totalRecords, int filterRecs)
        {
            JsonResult auditTableResult;

            if (string.IsNullOrEmpty(errorMsg) == false)
            {
                auditTableResult = this.Json(new
                                        {
                                            draw            = Convert.ToInt32(draw),
                                            recordsTotal    = 0,
                                            recordsFiltered = 0,
                                            data            = auditDataReccordList,
                                            error           = errorMsg
                                        }, JsonRequestBehavior.AllowGet);

                return auditTableResult;
            }

            auditTableResult = this.Json(new
                                {
                                    draw            = Convert.ToInt32(draw),
                                    recordsTotal    = totalRecords,
                                    recordsFiltered = filterRecs,
                                    data            = auditDataReccordList
                                }, JsonRequestBehavior.AllowGet);

            return auditTableResult;
        }
        #endregion
    }

    #region Results Class
    [Serializable]
    public class AuditResultClass
    {
        #region Properties
        public string ErrorMsg { get; set; }
        public bool ErrorFlag { get; set; }
        public List<AuditDataRecordModel> AuditRecords { get; set; }
        public List<AuditSourceIdFilterModel> IdFilterList { get; set; }
        public AuditSourceFilterDataModel SourceFilterDataModel { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructors
        /// </summary>
        public AuditResultClass()
        {
            this.Init();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initail state.
        /// </summary>
        private void Init()
        {
            this.ErrorFlag = false;
            this.ErrorMsg = string.Empty;
            this.AuditRecords = new List<AuditDataRecordModel>();
        }
        #endregion
    }
    #endregion
}