using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.SqlServer.Management.Smo;

namespace Encrypt_Stored_Procedures
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<StoredProcedure> proceduresAlreadyEncrypted;
		private List<StoredProcedure> proceduresEncrypted;
		private List<StoredProcedure> proceduresFailed;

		private List<DatabaseDdlTrigger> triggersAlreadyEncrypted;
		private List<DatabaseDdlTrigger> triggersEncrypted;
		private List<DatabaseDdlTrigger> triggersFailed;

		private List<UserDefinedFunction> functionsAlreadyEncrypted;
		private List<UserDefinedFunction> functionsEncrypted;
		private List<UserDefinedFunction> functionsFailed;

		private List<View> viewsAlreadyEncrypted;
		private List<View> viewsEncrypted;
		private List<View> viewsFailed;

		private StringBuilder sb;

		Server server = null;

		public MainWindow()
		{
			InitializeComponent();

			ServerTextBox.Text = "localhost";
			DatabaseTextBox.Text = "ConsolidatedDB";

			InstructionsBlock.Text = "This utility will encrypt the DBO stored procedures, views, triggers, and functions for the specified database.  This cannot be undone.  You must ensure you have a backup first.  Results are automatically copied to the clipboard.";
		}

		private void CancelButton_Click( object sender, RoutedEventArgs e )
		{
			this.Close();
		}

		private void ResetLists()
		{
			proceduresAlreadyEncrypted = new List<StoredProcedure>();
			proceduresEncrypted = new List<StoredProcedure>();
			proceduresFailed = new List<StoredProcedure>();

			triggersAlreadyEncrypted = new List<DatabaseDdlTrigger>();
			triggersEncrypted = new List<DatabaseDdlTrigger>();
			triggersFailed = new List<DatabaseDdlTrigger>();

			functionsAlreadyEncrypted = new List<UserDefinedFunction>();
			functionsEncrypted = new List<UserDefinedFunction>();
			functionsFailed = new List<UserDefinedFunction>();

			viewsAlreadyEncrypted = new List<View>();
			viewsEncrypted = new List<View>();
			viewsFailed = new List<View>();
		}

		private void EncryptButton_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				ResetLists();

				sb = new StringBuilder();
				sb.AppendLine( string.Format( "Encryption run for {0} at {1}", DatabaseTextBox.Text, DateTime.Now.ToString() ) );
				sb.AppendLine();
				
				this.Cursor = Cursors.Wait;

				server = new Server( ServerTextBox.Text );
				server.ConnectionContext.MultipleActiveResultSets = true;

				Database database = server.Databases[DatabaseTextBox.Text];

				foreach ( StoredProcedure procedure in database.StoredProcedures )
				{
					EncryptProcedure( procedure );
				}

				foreach ( UserDefinedFunction function in database.UserDefinedFunctions )
				{
					EncryptFunction( function );
				}

				foreach ( View view in database.Views )
				{
					EncryptView( view );
				}

				foreach ( DatabaseDdlTrigger trigger in database.Triggers )
				{
					EncryptTrigger( trigger );
				}

				DisplayResults();

			}
			catch ( Exception except )
			{
				ResultsTextBox.Text += "\n\n" + except.Message;
			}
			finally
			{
				this.Cursor = Cursors.Arrow;
				server.ConnectionContext.Disconnect();
				server = null;
			}
		}

		private void EncryptFunction(UserDefinedFunction function)
		{
			if ( function.Schema.Equals( "dbo", StringComparison.OrdinalIgnoreCase ) )
			{
				try
				{
					if ( function.IsEncrypted == false && function.IsSystemObject == false )
					{
						function.TextMode = false;
						function.IsEncrypted = true;
						function.Alter();
						functionsEncrypted.Add( function );
					}
					else
					{
						functionsAlreadyEncrypted.Add( function );
					}
				}
				catch ( Exception except )
				{
					sb.AppendLine( function.Name + " - " + except.Message );
					functionsFailed.Add( function );
					server.ConnectionContext.Disconnect();
					server.ConnectionContext.Connect();
				}
			}
		}

		private void EncryptTrigger( DatabaseDdlTrigger trigger )
		{
			try
			{
				if ( trigger.IsEncrypted == false && trigger.IsSystemObject == false )
				{
					trigger.TextMode = false;
					trigger.IsEncrypted = true;
					trigger.Alter();
					triggersEncrypted.Add( trigger );
				}
				else
				{
					triggersAlreadyEncrypted.Add( trigger );
				}
			}
			catch ( Exception except )
			{
				sb.AppendLine( trigger.Name + " - " + except.Message );
				triggersFailed.Add( trigger );
				server.ConnectionContext.Disconnect();
				server.ConnectionContext.Connect();
			}
		}

		private void EncryptView( View view )
		{
			if ( view.Schema.Equals( "dbo", StringComparison.OrdinalIgnoreCase ) )
			{
				try
				{
					if ( view.IsEncrypted == false && view.IsSystemObject == false )
					{
							view.TextMode = false;
							view.IsEncrypted = true;
							view.Alter();
							viewsEncrypted.Add( view );
					}
					else
					{
						viewsAlreadyEncrypted.Add( view );
					}
				}
				catch ( Exception except )
				{
					sb.AppendLine( view.Name + " - " + except.Message );
					viewsFailed.Add( view );
					server.ConnectionContext.Disconnect();
					server.ConnectionContext.Connect();
				}
			}
		}

		private void EncryptProcedure( StoredProcedure procedure )
		{
			if ( procedure.Schema.Equals( "dbo", StringComparison.OrdinalIgnoreCase ) )
			{
				try
				{
					if ( procedure.IsEncrypted == false && procedure.IsSystemObject == false )
					{
						procedure.TextMode = false;
						procedure.IsEncrypted = true;
						procedure.Alter();
						proceduresEncrypted.Add( procedure );
					}
					else
					{
						proceduresAlreadyEncrypted.Add( procedure );
					}
				}
				catch ( Exception except )
				{
					sb.AppendLine( procedure.Name + " - " + except.Message );
					proceduresFailed.Add( procedure );
				}
			}
		}

		private void DisplayResults()
		{
			string dashes = "==============================================";

			sb.AppendLine( dashes );

			if ( proceduresFailed.Count == 0 )
			{
				sb.AppendLine( "All DBO stored procedures were encrypted successfully" );
			}
			else
			{
				sb.AppendLine( "Encryption FAILED for these Stored Procedures" );
				sb.AppendLine( dashes );

				foreach ( StoredProcedure procedure in proceduresFailed )
				{
					sb.AppendLine( procedure.Name );
				}
			}

			sb.AppendLine();

			if ( functionsFailed.Count == 0 )
			{
				sb.AppendLine( "All DBO user-defined functions were encrypted successfully" );
			}
			else
			{
				sb.AppendLine( "Encryption FAILED for these user-defined functions" );
				sb.AppendLine( dashes );

				foreach ( UserDefinedFunction function in functionsFailed )
				{
					sb.AppendLine( function.Name );
				}
			}

			sb.AppendLine();

			if ( viewsFailed.Count == 0 )
			{
				sb.AppendLine( "All DBO views were encrypted successfully" );
			}
			else
			{
				sb.AppendLine( "Encryption FAILED for these Views" );
				sb.AppendLine( dashes );

				foreach ( View view in viewsFailed )
				{
					sb.AppendLine( view.Name );
				}

			}

			sb.AppendLine();

			if ( triggersFailed.Count == 0 )
			{
				sb.AppendLine( "All triggers were encrypted successfully" );
			}
			else
			{
				sb.AppendLine( "Encryption FAILED for these Triggers" );
				sb.AppendLine( dashes );

				foreach( DatabaseDdlTrigger trigger in triggersFailed)
				{
					sb.AppendLine( trigger.Name );
				}
			}

			sb.AppendLine();

			sb.AppendLine( "These Stored Procedures were successfully encrypted" );
			sb.AppendLine( dashes );
			if ( proceduresEncrypted.Count == 0 )
			{
				sb.AppendLine( "none" );
			}
			else
			{
				foreach ( StoredProcedure procedure in proceduresEncrypted )
				{
					sb.AppendLine( procedure.Name );
				}
			}

			sb.AppendLine();

			sb.AppendLine( "These User-Defined Functions were successfully encrypted" );
			sb.AppendLine( dashes );
			if ( functionsEncrypted.Count == 0 )
			{
				sb.AppendLine( "none" );
			}
			else
			{
				foreach ( UserDefinedFunction function in functionsEncrypted )
				{
					sb.AppendLine( function.Name );
				}
			}

			sb.AppendLine();

			sb.AppendLine( "These Views were successfully encrypted" );
			sb.AppendLine( dashes );
			if ( viewsEncrypted.Count == 0 )
			{
				sb.AppendLine( "none" );
			}
			else
			{
				foreach ( View view in viewsEncrypted )
				{
					sb.AppendLine( view.Name );
				}

				sb.AppendLine();
				
			}

			sb.AppendLine();

			sb.AppendLine( "These Triggers were successfully encrypted" );
			sb.AppendLine( dashes );
			if ( triggersEncrypted.Count == 0 )
			{
				sb.AppendLine( "none" );
			}
			else
			{
				foreach ( DatabaseDdlTrigger trigger in triggersEncrypted )
				{
					sb.AppendLine( trigger.Name );
				}
			}

			sb.AppendLine();

			sb.AppendLine( "Stored Procedures Already Encrypted" );
			sb.AppendLine( dashes );
			if ( proceduresAlreadyEncrypted.Count == 0 )
			{
				sb.AppendLine( "none" );
			}
			else
			{
				foreach ( StoredProcedure procedure in proceduresAlreadyEncrypted )
				{
					sb.AppendLine( procedure.Name );
				}

				sb.AppendLine();

			}

			sb.AppendLine();

			sb.AppendLine( "These User-Defined Functions were already encrypted" );
			sb.AppendLine( dashes );
			if ( functionsAlreadyEncrypted.Count == 0 )
			{
				sb.AppendLine( "none" );
			}
			else
			{
				foreach ( UserDefinedFunction function in functionsAlreadyEncrypted )
				{
					sb.AppendLine( function.Name );
				}
			}

			sb.AppendLine();

			sb.AppendLine( "These Views were already encrypted" );
			sb.AppendLine( dashes );
			if ( viewsAlreadyEncrypted.Count == 0 )
			{
				sb.AppendLine( "none" );
			}
			else
			{
				foreach ( View view in viewsAlreadyEncrypted )
				{
					sb.AppendLine( view.Name );
				}

				sb.AppendLine();

			}

			sb.AppendLine();

			sb.AppendLine( "These Triggers were already encrypted" );
			sb.AppendLine( dashes );
			if ( triggersAlreadyEncrypted.Count == 0 )
			{
				sb.AppendLine( "none" );
			}
			else
			{
				foreach ( DatabaseDdlTrigger trigger in triggersAlreadyEncrypted )
				{
					sb.AppendLine( trigger.Name );
				}

				sb.AppendLine();

			}

			ResultsTextBox.Text = sb.ToString();

			Clipboard.SetText( ResultsTextBox.Text );

			MessageBox.Show( this, "Encryption run completed.  Results were copied to the clipboard and are shown in results text box.", this.Title, MessageBoxButton.OK, MessageBoxImage.Information );

		}

		private void SelectDatabaseButton_Click( object sender, RoutedEventArgs e )
		{
			SelectionWindow window = new SelectionWindow( new Server( ServerTextBox.Text ) );
			window.Owner = this;
			if ( (bool) window.ShowDialog() )
			{
				// Get the database selected
				string value = window.SelectedValue;

				value = value.Replace( "[", "" );
				value = value.Replace( "]", "" );

				this.DatabaseTextBox.Text = value;
			}
		}
	}
}
