/// <summary>
///   File name:	TempPasswordDO.cs
///   Purpose:	Temporary Password Data Object
///	Comments:	
///	Author(s):	Whaylon Coleman
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:				By:						Reason:
///		----------		--------------------	----------------------------------
///		2012-06-26		W.Coleman				Initial creation.
using System.Data.SqlClient;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// This class is responsible for retrieving the user's temporary password hint.
	/// </summary>
	public class TempPasswordDO
	{
		/// <summary>
		/// This method retrieves the user's temporary password from the tblUser's table.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="loginRequest"></param>
		public void GetSelectCommand(SqlCommand cmd, SecurityLoginRequest loginRequest)
		{
			cmd.CommandText = "Select PasswordHint from tblUsers where UserId = @UserId";
			cmd.CommandType = System.Data.CommandType.Text;
			cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = (string.IsNullOrEmpty(loginRequest.UserID)) ? "Administrator" : loginRequest.UserID;
		}
	}
}
