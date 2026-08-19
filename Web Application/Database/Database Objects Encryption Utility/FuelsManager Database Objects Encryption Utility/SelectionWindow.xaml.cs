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
using System.Windows.Shapes;

using Microsoft.SqlServer.Management.Smo;

namespace Encrypt_Stored_Procedures
{
	/// <summary>
	/// Interaction logic for SelectionWindow.xaml
	/// </summary>
	public partial class SelectionWindow : Window
	{
		public SelectionWindow( Server server )
		{
			InitializeComponent();

			this.Title = "Select Database";

			listBox1.ItemsSource = server.Databases;
			if ( server.Databases.Count > 0 )
			{
				listBox1.SelectedIndex = 0;
			}

			listBox1.MouseDoubleClick += new MouseButtonEventHandler( listBox1_MouseDoubleClick );

		}

		void listBox1_MouseDoubleClick( object sender, MouseButtonEventArgs e )
		{
			this.OKButton_Click( null, null );
		}

		public string SelectedValue
		{
			get
			{
				if ( listBox1.Items.Count > 0 && listBox1.SelectedIndex >= 0 )
				{
					return listBox1.SelectedValue.ToString();
				}

				return string.Empty;

			}
		}

		private void CancelButton_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = false;
			this.Close();
		}

		private void OKButton_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = true;
			this.Close();
		}
	}
}
