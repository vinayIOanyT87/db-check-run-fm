namespace Nspa
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Diagnostics;
    using System.Reflection;

    using ADC.Nspa.General;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Processor Base in Generic Format
	/// </summary>
	/// <typeparam name="TRequest"></typeparam>
	/// <typeparam name="TResponse"></typeparam>
	public abstract class RequestProcessorGenericBase<TRequest, TResponse> : RequestProcessorBase
		where TRequest : ExchangeRequestBase where TResponse : ExchangeResponseBase, new()
	{

		public override ExchangeRequestBase BaseRequest
		{
			get
			{
				return Request;
			}
		}

		public override ExchangeResponseBase BaseResponse
		{
			get
			{
				return Response;
			}
		}


		public TRequest Request
		{
			get; protected set; 
			
		}

		public TResponse Response
		{
			get; protected set; 
			
		}

		protected override void PreProcess(string xmlData)
		{
            try
            {
                LogMessage = string.Empty;
                var exchangeRequest = ExchangeBase.CreateExchangeFromXml<TRequest>(xmlData);
                var exchangeResponse = new TResponse() { ExchangeType = exchangeRequest.ExchangeType };
                this.Request = exchangeRequest;
                this.Response = exchangeResponse;
                exchangeResponse.Success = true;
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error encountered during preprocess of xmldata";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		protected RequestProcessorGenericBase(string newDescription)
			: base(newDescription)
		{
		}
	}

	/// <summary>
	/// Processor Base
	/// </summary>
	public abstract class RequestProcessorBase : IDisposable
	{
		internal string OperationDescription { get; private set; }

        protected SecurityClass Security;

	    protected const string StrOriginManual = "Manual";

        protected const string StrOriginDispatch = "Dispatch";

        protected const string HandheldGroupId = "ADC Handheld Users";

	    public string LogMessage { get; protected set; }

		public abstract ExchangeRequestBase BaseRequest { get; }

		public abstract ExchangeResponseBase BaseResponse { get; }

	    protected RequestProcessorBase(string newDescription)
        {
		    this.OperationDescription = newDescription;
        }


        public virtual void Dispose()
        {
            //NspaADCProcessor.NspaADCEventLog.Dispose();
        }

        protected double ConvertUnits(double source, EngineeringUnit sourceUnits, EngineeringUnit resultUnits)
        {
            // Use the accounting site conversion functions to convert
            double result = 0;

            EngineeringUnits.Convert(source, sourceUnits, ref result, resultUnits, 0);

            return result;
        }

        public void ValidateExchangeUserId(string userId)
        {
            var validUser = false;

			var exchangeUser = FMChannelHelper.MakeCall<IUsers, UserClass>(userService => userService.GetByID(this.Security, userId));

            if (!exchangeUser.IdentityGuid.IsEmpty())
            {                
                FMChannelHelper.MakeCall<IGroups>(
                    groupChannel =>
                    {
                        var groupGuid = groupChannel.GetIdentityGuid(this.Security, HandheldGroupId);
	                    var groupList = groupChannel.EnumerateByUserByGroup(this.Security, exchangeUser.IdentityGuid, groupGuid);
	                    validUser = (groupList.Count >= 1);
                    });
            }

            if (!validUser)
            {
                var errorMessage = string.Format("User Id '{0}' is not configured for handheld operations.", userId);
                throw new ArgumentOutOfRangeException(errorMessage);
            }

        }

		/// <summary>
		/// This may not be the fastest because it uses reflection.  But it makes the caller code cleaner.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="entityList">The entity list.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <returns></returns>
        protected static DataTable ListToDataTable(DataTable table, IEnumerable entityList, string tableName)
        {
            DataTable newDataTable = (table == null) ? new DataTable { TableName = tableName } : table;

            if (entityList != null)
            {
                int entityCount = 0;
                var e = entityList.GetEnumerator();
                while (e.MoveNext())
                {
                    entityCount++;
                }
                if (entityCount == 0)
                {
                    Helper.NspaADCEventLog.WriteEntry("Warning: no data found for " + tableName, EventLogEntryType.Warning);
                    return newDataTable;
                }

                PropertyInfo[] properties = null;
                foreach (var entity in entityList)
                {
                    if (properties == null)
                    {
                        properties = entity.GetType().GetProperties();
                        if (newDataTable.Columns.Count == 0)
                        {
                            foreach (var property in properties)
                            {
                                var columnType = property.PropertyType;
                                if (columnType.IsGenericType)
                                {
                                    columnType = columnType.GenericTypeArguments[0];
                                }
                                newDataTable.Columns.Add(new DataColumn(property.Name, columnType));
                            }
                        }
                    }

                    var newRow = newDataTable.NewRow();
                    foreach (var property in properties)
                    {
                        var columnValue = entity.GetType().GetProperty(property.Name).GetValue(entity, null);
                        if ((property.PropertyType.Equals(typeof(Guid)) && (Guid)columnValue == Guid.Empty) ||
                            (columnValue == null))
                        {
                            newRow[property.Name] = DBNull.Value;
                        }
                        else
                        {
                            newRow[property.Name] = columnValue;
                        }
                    }

                    newDataTable.Rows.Add(newRow);
                }
            }
            return newDataTable;
        }

		public void AddResponseError(string operation, string message)
		{
			this.AddError(this.BaseResponse, operation, message);
		}

		public void AddError(ExchangeResponseBase response, string operation, string message)
		{
			string eventLogMessage = string.Format(
				"{0} - error in {1} - {2}",
                Helper.WindowsEventLogModuleName,
				operation,
				message);
            Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
			response.ErrorList.Add(message);
			response.Success = false;
		}

		protected abstract void PreProcess(string xmlData);

		internal void Process(SecurityClass security, string xmlData)
		{
			this.Security = security;
			this.PreProcess(xmlData);
		    string logMessage = string.Format(
		        "{0} processing {1} started for client device {2}",
		        Helper.WindowsEventLogModuleName,
		        this.BaseRequest.ExchangeType,
		        this.BaseRequest.ClientHostName);
		    Helper.NspaADCEventLog.WriteEntry(logMessage, EventLogEntryType.Information);

		    try
		    {
                this.ProcessCore();
            }
            finally
		    {
                logMessage = string.Format(
                    "{0} processing {1} completed for client device {2}: {3}",
                    Helper.WindowsEventLogModuleName,
                    this.BaseRequest.ExchangeType,
                    this.BaseRequest.ClientHostName,
                    this.BaseResponse.Success ? "Success" : "Error");

                Helper.NspaADCEventLog.WriteEntry(logMessage, EventLogEntryType.Information);
            }
        }

        protected abstract void ProcessCore();
    }
}