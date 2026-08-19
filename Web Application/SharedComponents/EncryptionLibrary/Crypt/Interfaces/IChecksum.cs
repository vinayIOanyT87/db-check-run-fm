using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crypt.Interfaces
{
    /// <summary>
    /// Interface for creating checksums
    /// </summary>
    public interface IChecksum
    {
        /// <summary>
        /// Computes a checksum using the defined algorithm
        /// </summary>
        /// <param name="a_text">The input to produce the checksum for</param>
        /// <returns>the checksum, this can be varying length depending on the algorithm used</returns>
        byte[] Checksum(byte[] a_text);

        /// <summary>
        /// Performs a CRC check on the defined algorithm of the implementing class
        /// </summary>
        /// <param name="a_text">The input to check the checksum for</param>
        /// <param name="a_checksum">Expected checksum</param>
        /// <returns>true if the CRC succeeds, otherwise false</returns>
        bool CRC(byte[] a_text, byte[] a_checksum);
    }
}
