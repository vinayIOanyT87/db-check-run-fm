namespace FMDatabase.SqlServer.Clr.Interfaces
{
    using Microsoft.SqlServer.Server;
    using System.Data;

    internal interface ISqlMetaDataCreator
    {
        SqlMetaData SqlMetaDataFromColumn(DataColumn column, out bool coerceToString);
    }
}
