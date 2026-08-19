using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crypt.Interfaces;
using ErrorHandler;
using System.Reflection;

namespace Crypt
{
    /// <summary>
    /// Base class with shared functionalities for all encryptors
    /// </summary>
    public class EncryptorBase
    {
        /// <summary>
        /// Encrypts a block of bytes
        /// </summary>
        /// <param name="a_pt">The plain text / block of bytes to be encrypted</param>
        /// <param name="a_key">The key to use for encryption</param>
        /// <remarks>This method is polymorphic, the base functionality is not implemented</remarks>
        /// <returns>The computed block of cipher-text</returns>
        public virtual byte[] Encrypt(byte[] a_pt, IKey a_key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decrypts a block of bytes
        /// </summary>
        /// <param name="a_ct">The cipher text / block of bytes to be decrypted</param>
        /// <param name="a_key">The key to use for the decryption</param>
        /// <remarks>This method is polymorphic, the base functionality is not implemented</remarks>
        /// <returns>The decrypted block of cipher text using the given key</returns>
        public virtual byte[] Decrypt(byte[] a_ct, IKey a_key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies if the key used is of a compatible / expected type
        /// </summary>
        /// <param name="a_key">The key used</param>
        /// <param name="a_expected">The compatible / expected type</param>
        protected void VerifyKey(IKey a_key, Type a_expected)
        {
            if (a_key.GetType() != a_expected)
            {
                throw new Error(ErrorCode.CRYPTO_WRONG_KEY_TYPE,
                        MethodBase.GetCurrentMethod().ToString(),
                        "Expected RSACertificate but got " + a_key.GetType().ToString());
            }
        }
    }
}
