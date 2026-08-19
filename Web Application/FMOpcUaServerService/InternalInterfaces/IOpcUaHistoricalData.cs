// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOpcUaHistoricalData.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMOpcUaServerService.InternalInterfaces
{
    using System;
    using System.Data;
    using FMBusinessObjects.DataObjects;
    using Opc.Ua;

    internal interface IOpcUaHistoricalData
    {
        DataSet ReadArchiveHistory(
            SecurityClass security,
            DateTime startTime,
            DateTime endTime,
            bool isModified,
            NodeId nodeID);
    }
}
