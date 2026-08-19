// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaDataExtractor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Extracts metadata for SQL transmission.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMDatabase.SqlServer.Clr.UtilityClasses
{
    using System;
    using System.Data;
    using Microsoft.SqlServer.Server;

    using FMDatabase.SqlServer.Clr.Interfaces;

    internal class MetaDataExtractor : IMetaDataExtractor
    {
        private readonly ISqlMetaDataCreator SqlMetaDataCreator = new SqlMetaDataCreator();

        public SqlMetaData[] Extract(DataTable dataTable, out bool[] coerceToString)
        {
            if ( dataTable == null )
            {
                throw new ArgumentNullException("dataTable");
            }

            SqlMetaData[] metaDataResult = new SqlMetaData[dataTable.Columns.Count];
            coerceToString = new bool[dataTable.Columns.Count];
            for (int index = 0; index < dataTable.Columns.Count; index++)
            {
                DataColumn column = dataTable.Columns[index];
                metaDataResult[index] = this.SqlMetaDataCreator.SqlMetaDataFromColumn(column, out coerceToString[index]);
            }

            return metaDataResult;
        }
    }
}
