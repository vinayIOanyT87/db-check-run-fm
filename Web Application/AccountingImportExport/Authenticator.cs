/// <summary>
/// FILE NAME:		Authenticator.cs
///	PURPOSE:		
///
///	COMMENTS:
///		Copyright (C) Varec, Inc. Norcross, GA, USA, 2009
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Endress+Hauser.
///
///	AUTHOR(S):	
///	VERSION:		1.0.0  Current version
///
///	MODIFICATION HISTORY:
///		Date:			By:						Reason:
///		---------	-----------------		-----------------------------------------------------------------------------
///		2009-08-11	I.Orndorff				- Added DaysUntilExpiration parameter to "Sites.Login()". This addresses task #5267.
/// </summary>

namespace AccountingImportExport
{
	/// <summary>
	/// This class allows us to bypass the standard (windows) authentication
	/// </summary>
	[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand,
		 Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
	public class Authenticator : Microsoft.Web.Services2.Security.Tokens.UsernameTokenManager
	{
		#region Attributes
		#endregion Attributes

		#region Overrides

		protected override void VerifyPassword(Microsoft.Web.Services2.Security.Tokens.UsernameToken token, string authenticatedPassword)
		{
		}

		protected override string AuthenticateToken(Microsoft.Web.Services2.Security.Tokens.UsernameToken token)
		{
			return token.Password;
		}

		#endregion Overrides
	}
}
