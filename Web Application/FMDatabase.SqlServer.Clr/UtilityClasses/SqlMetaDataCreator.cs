namespace FMDatabase.SqlServer.Clr.UtilityClasses
{
    using System;
    using System.Data;
    using Microsoft.SqlServer.Server;

    using FMDatabase.SqlServer.Clr.Interfaces;

    internal class SqlMetaDataCreator : ISqlMetaDataCreator
    {
        private readonly ISqlMetaDataFromObjectCreator SqlMetaDataFromObjectCreator = new SqlMetaDataFromObjectCreator();

        public SqlMetaData SqlMetaDataFromColumn(DataColumn column, out bool coerceToString)
        {
            coerceToString = false;
            SqlMetaData sqlMd;
            Type clrType = column.DataType;
            string name = column.ColumnName;

            var typeCode = this.GetTypeCode(clrType);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    sqlMd = new SqlMetaData(name, SqlDbType.Bit);
                    break;
                case TypeCode.Byte:
                    sqlMd = new SqlMetaData(name, SqlDbType.TinyInt);
                    break;
                case TypeCode.Char:
                    sqlMd = new SqlMetaData(name, SqlDbType.NVarChar, 1);
                    break;
                case TypeCode.DateTime:
                    sqlMd = new SqlMetaData(name, SqlDbType.DateTime);
                    break;
                case TypeCode.Decimal:
                    sqlMd = new SqlMetaData(name, SqlDbType.Decimal, 18, 0);
                    break;
                case TypeCode.Double:
                    sqlMd = new SqlMetaData(name, SqlDbType.Float);
                    break;
                case TypeCode.Int16:
                    sqlMd = new SqlMetaData(name, SqlDbType.SmallInt);
                    break;
                case TypeCode.Int32:
                    sqlMd = new SqlMetaData(name, SqlDbType.Int);
                    break;
                case TypeCode.Int64:
                    sqlMd = new SqlMetaData(name, SqlDbType.BigInt);
                    break;
                case TypeCode.Single:
                    sqlMd = new SqlMetaData(name, SqlDbType.Real);
                    break;
                case TypeCode.String:
                    sqlMd = new SqlMetaData(name, SqlDbType.NVarChar, column.MaxLength);
                    break;
                case TypeCode.Object:
                    sqlMd = this.SqlMetaDataFromObjectCreator.Create(name, column, clrType);
                    if (sqlMd == null)
                    {
                        // Unknown type, try to treat it as string;
                        sqlMd = new SqlMetaData(name, SqlDbType.NVarChar, column.MaxLength);
                        coerceToString = true;
                    }
                    break;

                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    throw new ArgumentException("Invalid type: " + typeCode);

                default:
                    throw new ArgumentException("Unknown type: " + clrType);
            }

            return sqlMd;
        }

        internal TypeCode GetTypeCode(Type clrType)
        {
            return Type.GetTypeCode(clrType);
        }
    }
}
