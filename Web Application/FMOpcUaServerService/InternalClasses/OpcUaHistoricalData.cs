// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OPCUAHistoricalData.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMOpcUaServerService.InternalClasses
{
    using System;
    using System.Data;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMOpcUaServerService.InternalInterfaces;
    using Opc.Ua;

    internal class OpcUaHistoricalData : IOpcUaHistoricalData
	{
        public DataSet ReadArchiveHistory(SecurityClass security, DateTime startTime, DateTime endTime, bool isModified, NodeId nodeID)
        {
            // verify that the passed in node is valid
            // read the data from the passed in parameters and return
            if (nodeID.IdType.ToString() != "Guid")
            {
                return null;
            }

            string stTagGuid = nodeID.Identifier.ToString().ToUpper();

            var archiveDataSet =
                FMChannelHelper.MakeCall<ISQLServerArchiveDataAccess, DataSet>(
                    x => x.ReadArchiveRecord(security, startTime, endTime, stTagGuid));

            // the returned set contains the startup, shutdown and data records for the selected tag
            // organize the data in the correct format
            return archiveDataSet;
        }
    }
}
