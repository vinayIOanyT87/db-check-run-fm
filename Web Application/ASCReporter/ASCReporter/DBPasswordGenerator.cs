using System;
using System.Collections.Generic;
using System.Text;

namespace ASCReporter
{
	/*  ZZNotUsed
	/// <summary>
	/// Class for FuelsManager Defense password generation
	/// </summary>
	/// <remarks>
	/// This class is used to generate the passwords used by FuelsManager Defense
	/// Currently, it only generates the Database password based on the database id
	/// Future expansion will includ the application password encryption as well
	/// </remarks>
	class DBPasswordGenerator
	{
		#region Attributes
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.  Currently does nothing
		/// </summary>
		public DBPasswordGenerator()
		{
		}
		#endregion

		#region Members
		/// <summary>
		/// generates a FuelsManager Defense database password for a user id
		/// </summary>
		/// <param name="userID">User ID for which to generate a database password</param>
		/// <returns>Generated database password</returns>
		static public string getDBPassword(string userID)
		{
			// Algorithm is to take a SHA-1 hash of the bytes of the ASCII representation
			// of the user ID followed by the bytes of the salt "{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}"
			ASCIIEncoding	encoding = new ASCIIEncoding();
			SHA1 sha = new SHA1CryptoServiceProvider();

			// Split out for obfuscation purposes
			// Probably something more thorough required later

			//Eric Simmons
			//08-10-2007
			//Updated to ensure that UserID is always uppercase.
			//resolves CSI #5049
			StringBuilder	newData = new StringBuilder(userID.ToUpper());
			newData.Append('{'); 
			newData.Append('0'); 
			newData.Append('1'); 
			newData.Append('A'); 
			newData.Append('F'); 
			newData.Append('E'); 
			newData.Append('B'); 
			newData.Append('D'); 
			newData.Append('3'); 
			newData.Append('-'); 
			newData.Append('7'); 
			newData.Append('8'); 
			newData.Append('C'); 
			newData.Append('D'); 
			newData.Append('-'); 
			newData.Append('4'); 
			newData.Append('B'); 
			newData.Append('1'); 
			newData.Append('5');
			newData.Append('-');
			newData.Append('A'); 
			newData.Append('B'); 
			newData.Append('9'); 
			newData.Append('B');
			newData.Append('-');
			newData.Append('F'); 
			newData.Append('4'); 
			newData.Append('A'); 
			newData.Append('A'); 
			newData.Append('1'); 
			newData.Append('C'); 
			newData.Append('0'); 
			newData.Append('E'); 
			newData.Append('2'); 
			newData.Append('D'); 
			newData.Append('9'); 
			newData.Append('B'); 
			newData.Append('}'); 
			byte[] userIDBytes = encoding.GetBytes(newData.ToString());
			//byte[]	saltBytes = encoding.GetBytes("{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}");

			byte[] pwdBytes = sha.ComputeHash(userIDBytes);

			newData.Length = 0;
			foreach (byte pwdByte in pwdBytes)
			{
				newData.Append(pwdByte.ToString("x2")); // x indicates hexidecimal integer, 2 (the precision) is
																		// the minimum number of digits.  Output will be zero
																		// padded on the left as necessary
			}
			return newData.ToString();
		}
		#endregion
	}
	*/
}
