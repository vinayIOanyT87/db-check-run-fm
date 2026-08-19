namespace FMDatabase.SqlServer.Clr.UtilityClasses
{
    using System;
    using System.Data;
    using System.Data.SqlTypes;

    using Microsoft.SqlServer.Server;

    using FMDatabase.SqlServer.Clr.Interfaces;

    internal class SqlMetaDataFromObjectCreator : ISqlMetaDataFromObjectCreator
    {
        public SqlMetaData Create(string name, DataColumn column, Type clrType)
        {
            if (clrType == typeof(System.Byte[])
                || clrType == typeof(SqlBinary)
                || clrType == typeof(SqlBytes)
                || clrType == typeof(System.Char[])
                || clrType == typeof(SqlString)
                || clrType == typeof(SqlChars))
            {
                return new SqlMetaData(name, SqlDbType.VarBinary, column.MaxLength);
            }

            if (clrType == typeof(Guid))
            {
                return new SqlMetaData(name, SqlDbType.UniqueIdentifier);
            }

            if (clrType == typeof(Object))
            {
                return new SqlMetaData(name, SqlDbType.Variant);
            }

            if (clrType == typeof(SqlBoolean))
            {
                return new SqlMetaData(name, SqlDbType.Bit);
            }

            if (clrType == typeof(SqlByte))
            {
                return new SqlMetaData(name, SqlDbType.TinyInt);
            }

            if (clrType == typeof(SqlDateTime))
            {
                return new SqlMetaData(name, SqlDbType.DateTime);
            }

            if (clrType == typeof(DateTimeOffset))
            {
                return new SqlMetaData(name, SqlDbType.DateTimeOffset);
            }

            if (clrType == typeof(SqlDouble))
            {
                return new SqlMetaData(name, SqlDbType.Float);
            }

            if (clrType == typeof(SqlGuid))
            {
                return new SqlMetaData(name, SqlDbType.UniqueIdentifier);
            }

            if (clrType == typeof(SqlInt16))
            {
                return new SqlMetaData(name, SqlDbType.SmallInt);
            }

            if (clrType == typeof(SqlInt32))
            {
                return new SqlMetaData(name, SqlDbType.Int);
            }

            if (clrType == typeof(SqlInt64))
            {
                return new SqlMetaData(name, SqlDbType.BigInt);
            }

            if (clrType == typeof(SqlMoney))
            {
                return new SqlMetaData(name, SqlDbType.Money);
            }

            if (clrType == typeof(SqlDecimal))
            {
                return new SqlMetaData(name, SqlDbType.Decimal, SqlDecimal.MaxPrecision, 0);
            }

            if (clrType == typeof(SqlSingle))
            {
                return new SqlMetaData(name, SqlDbType.Real);
            }

            if (clrType == typeof(SqlXml))
            {
                return new SqlMetaData(name, SqlDbType.Xml);
            }

            return null;
        }
    }
}
