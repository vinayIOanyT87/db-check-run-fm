using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crypt.Interfaces;
using System.Security.Cryptography;

namespace Crypt
{
    /// <summary>
    /// Checksum using SHA-1 algorithm
    /// </summary>
    public class SHAChecksum : IChecksum
    {
        /// <summary>
        /// Computes the SHA-1 checksum
        /// </summary>
        /// <param name="a_text">the block of bytes to compute the checksum for</param>
        /// <returns>The checksum computed</returns>
        public byte[] Checksum(byte[] a_text)
        {
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] cs = sha1.ComputeHash(a_text);
                return cs;
            }
        }

        /// <summary>
        /// Performs a cyclic redundancy check on the block of text
        /// </summary>
        /// <param name="a_text">The block of data to perform the CRC on</param>
        /// <param name="a_checksum">The expected checksum of the data</param>
        /// <returns>true if CRC passes, otherwise false</returns>
        public bool CRC(byte[] a_text, byte[] a_checksum)
        {
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] expected = sha1.ComputeHash(a_text);

                return a_checksum.Equals(expected);
            }
        }

	    /// <summary>
	    /// Computes the SHA-1 checksum in string format
	    /// </summary>
	    /// <param name="value">the block of bytes to compute the checksum for</param>
	    /// <returns>The checksum computed</returns>
	    public string HashString(string value)
	    {
		    StringBuilder Sb = new StringBuilder();
		    Encoding enc = Encoding.UTF8;
		    byte[] hashBytes = this.Checksum(enc.GetBytes(value));
		    foreach (Byte b in hashBytes)
		    {
			    Sb.Append(b.ToString("x2"));
		    }
		    return Sb.ToString();
	    }

	}
}
