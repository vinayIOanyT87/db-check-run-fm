/// <summary>
/// 
/// File name:	ChangePasswordForm.cs
/// 
/// Purpose:	
/// 
/// Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 2009 
///            This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///				
/// Author(s):	
/// 
/// Modification History:
///	Date:			By:					Reason:
///	----------	----------------	-----------------------------------------------
///	2009-09-24	I.Orndorff			- Modified "okButton_Click()" to rehash user password before 
///											  adding it back to security. This addresses bug #7146.
///											  
/// 2009-09-30  C. Knight           - Modified okButton_Click() to no longer hash user password
///                                     before adding back to security.  WI 6214
///											
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace DispatchPrototype
{
	public partial class ChangePasswordForm : Form
	{
		public ChangePasswordForm ( )
		{
			InitializeComponent ( );
			CenterToScreen ( );
		}

		private void okButton_Click ( object sender, EventArgs e )
		{
			try
			{
				if (newPasswordTextBox.Text != reenterPasswordTextBox.Text)
				{
					throw new Exception ( "Password vs. Re-enter Password does not match" );
				}

				SecurityClass security = AppDomain.CurrentDomain.GetData ( "Security" ) as SecurityClass;
				if (security == null)
				{
					throw new Exception ( "Security not in AppDomain" );
				}

				FMChannelFactory<IUsers> usersClient = new FMChannelFactory<IUsers> ( );
				IUsers users = usersClient.CreateProxy ( );
				UserClass user = users.Get ( security, security.UserGuid );

				FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
				ISites sites = sitesClient.CreateProxy ( );

				if (!sites.CheckCurrentPassword ( user, passwordTextBox.Text ))
				{
					throw new ApplicationException ( "Current password entered incorrectly" );
				}

				string oldPassword = user.Password;
				user.Password = newPasswordTextBox.Text;
				user.ChangePassword = false;
				user.PasswordTimestamp = DateTime.UtcNow;
				users.ModifyWithPasswordHistory ( security, user, oldPassword );

				security.Password = user.Password;

				DialogResult = DialogResult.OK;
			}

			catch (Exception exception)
			{
				newPasswordTextBox.Text = "";
				reenterPasswordTextBox.Text = "";
				MessageBox.Show ( this, exception.Message, this.Text );
			}
		}

		private void cancelButton_Click ( object sender, EventArgs e )
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
