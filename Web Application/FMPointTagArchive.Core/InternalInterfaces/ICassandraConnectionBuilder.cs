namespace FMPointTagArchive.Core.InternalInterfaces
{
    internal interface ICassandraConnectionBuilder
    {
        string[] Nodes { get; set; }

	    string Build(string cassandraConfiguration);


		string Build(string cassandraConfiguration, string cassandraUsername, string cassandraPassword);
    }
}
