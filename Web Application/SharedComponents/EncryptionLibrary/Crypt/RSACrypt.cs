using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using Crypt.Interfaces;
//using ErrorHandler;

namespace Crypt
{
    /// <summary>
    /// Used to identify which key to use for RSA encryption and decryption
    /// </summary>
    public enum PKI
    {
        /// <summary>
        /// Use this value for using the public key
        /// </summary>
        PUBLIC_KEY,

        /// <summary>
        /// Use this value for using the private key
        /// </summary>
        PRIVATE_KEY
    }

    public class RSACrypt : CryptBase
    {
        #region Attributes
        private PKI m_encryptMode;
        private PKI m_decryptMode;
        #endregion // Attributes

        #region Properties
        public PKI EncryptMode
        {
            get { return m_encryptMode; }
            set { m_encryptMode = value; }
        }
        public PKI DecryptMode
        {
            get { return m_decryptMode; }
            set { m_decryptMode = value; }
        }
        #endregion // Properties

        #region Constructors
        /// <summary>
        /// Default constructor, by default sets encryption to use any given certificate's public key, and 
        /// decryption to use any given certificate's private key
        /// </summary>
        public RSACrypt()
        {
            EncryptMode = PKI.PUBLIC_KEY;
            DecryptMode = PKI.PRIVATE_KEY;
        }

        /// <summary>
        /// Overloaded constructor 1, lets the caller set the encryption and decryption mode
        /// </summary>
        /// <param name="a_encryptMode">the encryption mode, pass in PKI.PUBLIC_KEY to use the public key for encryption, otherwise PKI.PRIVATE_KEY</param>
        /// <param name="a_decryptMode">the decryption mode, pass in PKI.PUBLIC_KEY to use the public key for decryption, otherwise PKI.PRIVATE_KEY</param>
        public RSACrypt(PKI a_encryptMode, PKI a_decryptMode)
        {
            EncryptMode = a_encryptMode;
            DecryptMode = a_decryptMode;
        }
        #endregion // Constructors

        /// <summary>
        /// Decrypts a block of data using a specified RSA certificate, this method will use the PRIVATE_KEY
        /// on the certificate by default for decryption. To change this set the DecryptMode or use the
        /// overloaded constructor.
        /// </summary>
        /// <param name="a_ct">The block of data to decrypt</param>
        /// <param name="a_key">The RSA certificate</param>
        /// <returns>Block of decrypted data</returns>
        public override byte[] Decrypt(byte[] a_ct, IKey a_key)
        {
            return Decrypt(a_ct, a_key);
        }

        /// <summary>
        /// Decrypts a block of data using a specified RSA certificate, this method will use the PRIVATE_KEY
        /// on the certificate by default for decryption. To change this set the DecryptMode or use the
        /// overloaded constructor.
        /// </summary>
        /// <param name="a_ct">The block of data to decrypt</param>
        /// <param name="a_key">The RSA certificate</param>
        /// <param name="fOAEP">true to perform direct System.Security.Cryptography.RSA decryption using OAEP padding (only available on a computer running Microsoft Windows XP or later); otherwise, false to use PKCS#1 v1.5 padding.</param>
        /// <returns>Block of decrypted data</returns>
        public byte[] Decrypt(byte[] a_ct, IKey a_key, bool fOAEP = true)
        {
            base.VerifyKey(a_key, typeof(RSACertificate));

            using (RSACryptoServiceProvider provider = CreateCryptProvider((RSACertificate)a_key, DecryptMode))
            {
                try
                {
                    byte[] pt = provider.Decrypt(a_ct, fOAEP);
                    return pt;
                }
                finally
                {
                    provider.Clear();
                }
            }
        }

        /// <summary>
        /// Encrypts a block of data using a specified RSA certificate, this method will use the PUBLIC_KEY
        /// on the certificate by default for encryption. To change this set the EncryptMode or use the
        /// overloaded constructor.
        /// </summary>
        /// <param name="a_pt">The block of data the encrypt</param>
        /// <param name="a_key">The RSA certificate</param>
        /// <returns>Block of encrypted data</returns>
        public override byte[] Encrypt(byte[] a_pt, IKey a_key)
        {
            return Encrypt(a_pt, a_key);
        }
        /// <summary>
        /// Encrypts a block of data using a specified RSA certificate, this method will use the PUBLIC_KEY
        /// on the certificate by default for encryption. To change this set the EncryptMode or use the
        /// overloaded constructor.
        /// </summary>
        /// <param name="a_pt">The block of data the encrypt</param>
        /// <param name="a_key">The RSA certificate</param>
        /// <param name="fOAEP">true to perform direct System.Security.Cryptography.RSA decryption using OAEP padding (only available on a computer running Microsoft Windows XP or later); otherwise, false to use PKCS#1 v1.5 padding.</param>
        /// <returns>Block of encrypted data</returns>
        public byte[] Encrypt(byte[] a_pt, IKey a_key, bool fOAEP = true)
        {
            base.VerifyKey(a_key, typeof(RSACertificate));

            using (RSACryptoServiceProvider provider = CreateCryptProvider((RSACertificate)a_key, EncryptMode))
            {
                try
                {
                    byte[] ct = provider.Encrypt(a_pt, fOAEP);
                    return ct;
                }
                finally
                {
                    provider.Clear();
                }
            }
        }

        #region Protected methods

        /// <summary>
        /// Create a RSACryptoServiceProvider based on a certifiate
        /// </summary>
        /// <param name="a_cert">The RSA certificate</param>
        /// <param name="a_cryptMode">Whether to generate the provider based on the PKI.PUBLIC_KEY or PKI.PRIVATE_KEY keys</param>
        /// <returns>The created RSACryptoServiceProvider object</returns>
        protected RSACryptoServiceProvider CreateCryptProvider(RSACertificate a_cert, PKI a_cryptMode)
        {
            X509Certificate2 cert = a_cert.Certificate;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            RSAParameters rsaKey;

            if (PKI.PUBLIC_KEY == a_cryptMode)
            {
                rsaKey = ((RSACryptoServiceProvider)cert.PublicKey.Key).ExportParameters(false);
            }
            else
            {
                if (cert.HasPrivateKey)
                {

                    rsaKey = ((RSACryptoServiceProvider)cert.PrivateKey).ExportParameters(true);        // true means include private parameter
                }
                else
                {
                    throw new ArgumentException("The loaded certificate does not have a private key");
                }

            }
            provider.ImportParameters(rsaKey);
            provider.PersistKeyInCsp = false;
            return provider;
        }

        #endregion // Protected methods
    }
}
