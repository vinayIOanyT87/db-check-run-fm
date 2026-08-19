
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.DataObjects;

namespace DispatchPrototype
{
	public partial class RadioFieldForm : FMBaseForm
	{
		private TransactionDO Transaction = null;

		public RadioFieldForm ( string TransID )
		{
			try
			{
				GetSecurity ( );
				InitializeComponent ( );

				Transaction = GetTransaction ( TransID );

				RadioNumberTextBox.Text = (string) Transaction.UserData["Transaction Aliases User Data 8"];
				RadioNumberTextBox.Focus ( );
				RadioNumberTextBox.SelectAll ( );

			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}

		}

		private void OKButton_Click ( object sender, EventArgs e )
		{
			try
			{
				Transaction.UserData["Transaction Aliases User Data 8"] = RadioNumberTextBox.Text;
				SaveTransaction ( Transaction );
				Close ( );
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		private void CancelBtn_Click ( object sender, EventArgs e )
		{
			Close ( );
		}

		private void RadioFieldForm_Load ( object sender, EventArgs e )
		{
			OKButton.Enabled = Security.HasRight ( RIGHT.MODIFY_DISPATCH );
		}

	}

}
