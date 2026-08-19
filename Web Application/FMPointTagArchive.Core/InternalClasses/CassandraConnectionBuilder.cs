namespace FMPointTagArchive.Core.InternalClasses
{
	using CqlSharp;
	using System;

	using FMPointTagArchive.Core.InternalInterfaces;

	internal class CassandraConnectionBuilder : ICassandraConnectionBuilder
	{
		public string[] Nodes { get; set; }

		public string Build(string cassandraConfiguration)
		{
			if (string.IsNullOrEmpty(cassandraConfiguration))
			{
				throw new ArgumentNullException("cassandraConfiguration");
			}

			this.Nodes = cassandraConfiguration.Split(new[] { ',', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var config = new CqlConnectionStringBuilder();
			config.Servers = this.Nodes;
			config.MaxConnectionsPerNode = Math.Max(4, this.Nodes.Length);
			config.MaxConnectionIdleTime = 1;
			config.UseBuffering = true;
			config.CommandTimeout = 60;

			return config.ConnectionString;
		}

		public string Build(string cassandraConfiguration, string cassandraUsername, string cassandraPassword)
		{
			if (string.IsNullOrEmpty(cassandraConfiguration))
			{
				throw new ArgumentNullException("cassandraConfiguration");
			}

			this.Nodes = cassandraConfiguration.Split(new[] { ',', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var config = new CqlConnectionStringBuilder();
			config.Servers = this.Nodes;
			config.MaxConnectionsPerNode = Math.Max(4, this.Nodes.Length);
			config.MaxConnectionIdleTime = 1;
			config.UseBuffering = true;
			config.CommandTimeout = 60;
			config.Username = cassandraUsername;
			config.Password = cassandraPassword;

			return config.ConnectionString;
		}
	}
}
