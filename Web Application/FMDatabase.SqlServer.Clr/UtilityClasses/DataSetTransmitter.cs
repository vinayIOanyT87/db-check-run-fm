// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTransmitter.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Provides helper methods for use in producing Clr stored procedures.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMDatabase.SqlServer.Clr.UtilityClasses
{
    using System;
    using System.Data;
    using System.Linq;
    using Microsoft.SqlServer.Server;

    using FMDatabase.SqlServer.Clr.Interfaces;

    internal class DataSetTransmitter : IDataSetTransmitter
    {
        private readonly IMetaDataExtractor MetaDataExtractor = new MetaDataExtractor();

        private SqlPipe sqlPipe;

        public void Transmit(DataSet dataSet)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }

            if (dataSet.Tables.Count == 0)
            {
                throw new ArgumentException("No data returned from processor.");
            }

            dataSet.Tables.Cast<DataTable>().All(table =>
            {
                this.TransmitTable(table);
                return true;
            });
        }

        internal void TransmitTable(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            bool[] coerceToString; // Do we need to coerce this column to string?
            SqlMetaData[] metaData = this.MetaDataExtractor.Extract(table, out coerceToString);

            SqlDataRecord record = new SqlDataRecord(metaData);

            this.GetSqlPipe();

            this.SendResultsStart(record);

            try
            {
                table.Rows.Cast<DataRow>().All(row =>
                {
                    for (int index = 0; index < record.FieldCount; index++)
                    {
                        object value = row[index];
                        if (null != value && coerceToString[index])
                        {
                            value = value.ToString();
                        }

                        record.SetValue(index, value);
                    }

                    this.SendResultsRow(record);

                    return true;
                });
            }
            finally
            {
                this.SendResultsEnd();
            }
        }

        internal void SendResultsEnd()
        {
            this.sqlPipe.SendResultsEnd();
        }

        internal void SendResultsRow(SqlDataRecord record)
        {
            this.sqlPipe.SendResultsRow(record);
        }

        internal void SendResultsStart(SqlDataRecord record)
        {
            this.sqlPipe.SendResultsStart(record);
        }

        internal void GetSqlPipe()
        {
            this.sqlPipe = this.GetActiveSqlPipe();
            if (this.sqlPipe == null)
            {
                throw new Exception("SqlContext pipeline not present.");
            }
        }

        internal SqlPipe GetActiveSqlPipe()
        {
            return SqlContext.Pipe;
        }
    }
}
