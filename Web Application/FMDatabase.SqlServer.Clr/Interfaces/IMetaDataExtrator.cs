namespace FMDatabase.SqlServer.Clr.Interfaces
{
    using System.Data;

    using Microsoft.SqlServer.Server;

    internal interface IMetaDataExtractor
    {
        SqlMetaData[] Extract(DataTable dataTable, out bool[] coerceToString);
    }
}
