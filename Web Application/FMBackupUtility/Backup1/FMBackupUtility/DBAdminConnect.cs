using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace FMBackupUtility
{
    class DBAdminConnect
    {
        private static string sUserID;

        public static string getConnectionString(string userID, string db)
        {
            sUserID = userID;
//            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder("Data Source = localhost; Initial Catalog = ConsolidatedDB;");
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder("Data Source = localhost;");
            connectionString.Add("Initial Catalog", db);
            connectionString.Add("Integrated Security", "false");
            connectionString.Add("Network Library", "dbmssocn");
            connectionString.Add("pwd", getDBPassword());
            connectionString.Add("User ID", userID);
//            connectionString.AsynchronousProcessing = true;
            return connectionString.ToString();
        }

        public static string getDBPassword()
        {
            // Algorithm is to take a SHA-1 hash of the bytes of the ASCII representation
            // of the user ID followed by the bytes of the salt "{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}"
            ASCIIEncoding encoding = new ASCIIEncoding();
            SHA1 sha = new SHA1CryptoServiceProvider();

            // Split out for obfuscation purposes
            // Probably something more thorough required later

            //Updated to ensure that UserID is always uppercase.
            StringBuilder newData = new StringBuilder(sUserID.ToUpper());
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
    }
}
