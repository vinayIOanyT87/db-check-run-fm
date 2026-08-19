namespace FMDatabase.SqlServer.Clr.Interfaces
{
    using System;
    using System.Data;

    using Microsoft.SqlServer.Server;

    internal interface ISqlMetaDataFromObjectCreator
    {
        SqlMetaData Create(string name, DataColumn column, Type clrType);
    }
}
