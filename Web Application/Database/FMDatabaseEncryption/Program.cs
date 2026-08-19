using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.SqlServer.Management.Smo;

namespace FMDatabaseEncryption
{
	class Program
	{
		static private bool IsSilentMode = false;
		static private string ServerName = string.Empty;
		static private string DatabaseName = string.Empty;
		static private StringBuilder results = new StringBuilder();
		static private Server server;

		static private int numberOfProcedures = 0;
		static private int numberOfFunctions = 0;
		static private int numberOfViews = 0;
		static private int numberOfTriggers = 0;

		static void Main( string[] args )
		{
			if (CheckArguments( args ))
			{
				try
				{
					// Open connection to specified database
					server = new Server( ServerName );
					server.ConnectionContext.MultipleActiveResultSets = true;

					Database database = server.Databases[DatabaseName];

					if (database == null)
					{
						throw new ApplicationException( string.Format( "Could not find {0} on {1}", DatabaseName, ServerName ) );
					}

					WriteLine( string.Format( "Encrypting database {0} on {1}...", DatabaseName, ServerName ) );

					// Encrypt the stored procedures
					foreach (StoredProcedure procedure in database.StoredProcedures)
					{
						EncryptProcedure( procedure );
					}

					// Encrypt the functions
					foreach (UserDefinedFunction function in database.UserDefinedFunctions)
					{
						EncryptFunction( function );
					}

					// Encrypt the triggers
					foreach (View view in database.Views)
					{
						EncryptView( view );
					}

					// Encrypt the views
					foreach (DatabaseDdlTrigger trigger in database.Triggers)
					{
						EncryptTrigger( trigger );
					}

					// Give ending statistics
					results.Append( string.Format( "Encrypted {0} procedures, {1} functions, {2} triggers, and {3} views.", 
						numberOfProcedures, numberOfFunctions, numberOfTriggers, numberOfViews ) );
				}
				catch (Exception except)
				{
					results.AppendLine( except.Message );
				}

				WriteLine( results.ToString() );
			}
		}

		private static void EncryptProcedure( StoredProcedure procedure )
		{
			if (procedure.Schema.Equals( "dbo", StringComparison.OrdinalIgnoreCase ))
			{
				try
				{
					if (procedure.IsEncrypted == false && procedure.IsSystemObject == false)
					{
						procedure.TextMode = false;
						procedure.IsEncrypted = true;
						procedure.Alter();
						++numberOfProcedures;
					}
				}
				catch (Exception except)
				{
					results.AppendLine( procedure.Name + " - " + except.Message );
					server.ConnectionContext.Disconnect();
					server.ConnectionContext.Connect();
				}
			}
		}

		private static void EncryptFunction( UserDefinedFunction function )
		{
			if (function.Schema.Equals( "dbo", StringComparison.OrdinalIgnoreCase ))
			{
				try
				{
					if (function.IsEncrypted == false && function.IsSystemObject == false)
					{
						function.TextMode = false;
						function.IsEncrypted = true;
						function.Alter();
						++numberOfFunctions;
					}
				}
				catch (Exception except)
				{
					results.AppendLine( function.Name + " - " + except.Message );
					server.ConnectionContext.Disconnect();
					server.ConnectionContext.Connect();
				}
			}
		}

		private static void EncryptTrigger( DatabaseDdlTrigger trigger )
		{
			try
			{
				if (trigger.IsEncrypted == false && trigger.IsSystemObject == false)
				{
					trigger.TextMode = false;
					trigger.IsEncrypted = true;
					trigger.Alter();
					++numberOfTriggers;
				}
			}
			catch (Exception except)
			{
				results.AppendLine( trigger.Name + " - " + except.Message );
				server.ConnectionContext.Disconnect();
				server.ConnectionContext.Connect();
			}
		}

		private static void EncryptView( View view )
		{
			if (view.Schema.Equals( "dbo", StringComparison.OrdinalIgnoreCase ))
			{
				try
				{
					if (view.IsEncrypted == false && view.IsSystemObject == false)
					{
						view.TextMode = false;
						view.IsEncrypted = true;
						view.Alter();
						++numberOfViews;
					}
				}
				catch (Exception except)
				{
					results.AppendLine( view.Name + " - " + except.Message );
					server.ConnectionContext.Disconnect();
					server.ConnectionContext.Connect();
				}
			}
		}

		private static void WriteLine( string message )
		{
			if (IsSilentMode == false)
			{
				Console.WriteLine( message );
			}
		}

		private static bool CheckArguments( string[] args )
		{
			// If no parameters are specified, show the help message
			if (args.Length == 0)
			{
				PrintHelp();
				return false;
			}
			else
			{
				foreach (string arg in args)
				{
					if (arg.Equals( "-s" ))
					{
						IsSilentMode = true;
					}
					else if (string.IsNullOrEmpty( arg ) == false && arg[0].Equals( '-' ) == false)
					{
						if (string.IsNullOrEmpty( ServerName ))
						{
							ServerName = arg;
						}
						else if (string.IsNullOrEmpty( DatabaseName ))
						{
							DatabaseName = arg;
						}
						else
						{
							PrintHelp();
							return false;
						}
					}
				}
			}

			// Check Argument State
			if (string.IsNullOrEmpty( ServerName ) || string.IsNullOrEmpty( DatabaseName ))
			{
				PrintHelp();
				return false;
			}

			return true;
		}

		private static void PrintHelp()
		{
			WriteLine( "fmdbencrypt [args] <SQLServerName> <DatabaseName>" );
			WriteLine( " \t-? : Show Help" );
			WriteLine( " \t/? : Show Help" );
			WriteLine( " \t-s : silent" );
			WriteLine( string.Empty );
		}
	}
}
