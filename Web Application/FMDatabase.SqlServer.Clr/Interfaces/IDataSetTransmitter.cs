// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMDatabase.SqlServer.Clr.Interfaces
{
    using System.Data;

    internal interface IDataSetTransmitter
    {
        void Transmit(DataSet dataSet);
    }
}
